using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static System.String;
using static PersistentLog.LogWindow;
using static Newtonsoft.Json.JsonConvert;
using static Newtonsoft.Json.MemberSerialization;

using static UnityEditor.SessionState;
using static UnityEngine.Application;
using static UnityEngine.Awaitable;

#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#else
using UnityEditor;
using static UnityEditor.AssemblyReloadEvents;
#endif

namespace PersistentLog {
#if UNITY_6000_5_OR_NEWER
    public static partial class LogInterceptor {
#else
    public static class LogInterceptor {
#endif
        [JsonObject(OptIn)]
        class PackageState {
            [JsonProperty("isRunning")] public bool IsRunning { get; set; }
            [JsonProperty("isWindowOpen")] public bool IsWindowOpen { get; set; }
            [JsonProperty("enableAutomatedLogFile")] public bool EnableAutomatedLogFile { get; set; }
            [JsonProperty("logFilePath")] public string LogFilePath { get; set; }
            [JsonProperty("previousLogFilePath")] public string PreviousLogFilePath { get; set; }
            [JsonProperty("extension")] public string Extension { get; set; }

            public LogWindow Window { get; set; }
            public List<LogItemInfo> Precache { get; set; }
        }

        [JsonObject(OptIn)]
        class PersistentState {
            [JsonProperty("packages")] public Dictionary<string, PackageState> Packages { get; set; } = new();
        }

        const string _stateKey = "PersistentLog";

        static PersistentState _persistentState;
        static bool _isSubscribed;

#if UNITY_6000_5_OR_NEWER
        [OnCodeInitializing]
        static void OnCodeInitializing() => OnCreate();

        [OnCodeUnloading]
        static void OnCodeUnloading() => OnDestroy();
#else
        [InitializeOnLoadMethod]
        static void Initialize() {
            afterAssemblyReload += OnAfterAssemblyReload;
            beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        static void OnAfterAssemblyReload() => OnCreate();
        static void OnBeforeAssemblyReload() => OnDestroy();
#endif

        static bool HasRunningPackages =>
            _persistentState?.Packages.Values.Any(packageState => packageState.IsRunning) ?? false;

        static bool IsProcessingPackages =>
            _persistentState?.Packages.Values.Any(packageState =>
                packageState.Precache != null || packageState.IsRunning) ?? false;

        static void OnCreate() {
            var defaultJson = SerializeObject(new PersistentState(), new StringEnumConverter());
            var json = GetString(_stateKey, defaultJson);
            _persistentState = DeserializeObject<PersistentState>(json, new StringEnumConverter()) ?? new();

            Subscribe(condition: HasRunningPackages);

            foreach (var (packageName, packageState) in _persistentState.Packages) {
                if (packageState.IsWindowOpen)
                    packageState.Window = GetWindow(packageName, packageState);
            }
        }

        static void OnDestroy() => Unsubscribe();

        public static void ShowWindow(string packageName, string logFilePath) {
            if (IsNullOrEmpty(packageName))
                return;

            if (_persistentState == null)
                return;

            if (_persistentState.Packages.TryGetValue(packageName, out var packageState)) {
                if (packageState.IsWindowOpen && packageState.Window != null)
                    return;
            }
            else {
                packageState = new PackageState();
                _persistentState.Packages.Add(packageName, packageState);
            }

            packageState.Window = GetWindow(packageName, packageState);
            packageState.IsWindowOpen = true;

            (packageState.LogFilePath, packageState.PreviousLogFilePath, packageState.Extension) =
                GetLogFilePaths(packageName, logFilePath);

            SavePersistentState();

            packageState.Window.LoadLastLogIfNeeded(packageState.LogFilePath, packageState.PreviousLogFilePath);
        }

        public static void Awake(string packageName) {
            if (IsNullOrEmpty(packageName))
                return;

            if (_persistentState == null)
                return;

            if (!_persistentState.Packages.TryGetValue(packageName, out var packageState)) {
                packageState = new PackageState();
                _persistentState.Packages.Add(packageName, packageState);
                SavePersistentState();
            }

            packageState.Precache = new List<LogItemInfo>();

            Subscribe();
        }

        public static void Start(
            string packageName,
            string logFilePath,
            bool enableAutomatedLogFile = true,
            bool automaticallyOpenLogWindow = true) {
            if (IsNullOrEmpty(packageName))
                return;

            if (_persistentState == null)
                return;

            if (!_persistentState.Packages.TryGetValue(packageName, out var packageState)) {
                packageState = new PackageState();
                _persistentState.Packages.Add(packageName, packageState);
            }

            packageState.EnableAutomatedLogFile = enableAutomatedLogFile;

            if (enableAutomatedLogFile) {
                (packageState.LogFilePath, packageState.PreviousLogFilePath, packageState.Extension) =
                    GetLogFilePaths(packageName, logFilePath);

                LogFile.New(packageState.LogFilePath, packageState.PreviousLogFilePath);
            }

            if (packageState.Window == null && automaticallyOpenLogWindow) {
                packageState.Window = GetWindow(packageName, packageState);
                packageState.IsWindowOpen = true;
            }

            if (packageState.Window != null)
                packageState.Window.Clear();

            packageState.IsRunning = true;
            SavePersistentState();

            Subscribe();

            if (packageState.Precache != null) {
                foreach (var logItem in packageState.Precache)
                    OnLogMessageReceived(packageState, logItem);

                packageState.Precache = null;
            }
        }

        public static void Stop(string packageName) {
            _persistentState?.Packages.Remove(packageName);
            SavePersistentState();
            Unsubscribe(condition: !IsProcessingPackages);
        }

        static async void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type) {
            try {
                var dateTime = DateTime.Now;
                await MainThreadAsync();

                if (_persistentState == null || _persistentState.Packages.Count == 0)
                    return;

                var logItem = new LogItemInfo(dateTime, type, condition, stackTrace);
                foreach (var packageState in _persistentState.Packages.Values) {
                    if (packageState.Precache != null)
                        packageState.Precache.Add(logItem);
                    else if (packageState.IsRunning)
                        OnLogMessageReceived(packageState, logItem);
                }
            } catch (Exception) {
                // ignored
            }
        }

        static void OnLogMessageReceived(PackageState packageState, LogItemInfo logItem) {
            if (packageState.EnableAutomatedLogFile)
                LogFile.Append(packageState.LogFilePath, logItem);

            packageState.Window?.Log(packageState.LogFilePath, logItem);
        }

        static LogWindow GetWindow(string packageName, PackageState packageState) => Get(
            packageName,
            onDestroy: () => {
                packageState.IsWindowOpen = false;
                SavePersistentState();
            });


        static void Subscribe(bool condition = true) {
            if (!_isSubscribed && condition) {
                logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
                _isSubscribed = true;
            }
        }

        static void Unsubscribe(bool condition = true) {
            if (!_isSubscribed || !condition)
                return;

            logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
            _isSubscribed = false;
        }

        static void SavePersistentState() {
            var json = SerializeObject(_persistentState, new StringEnumConverter());
            SetString(_stateKey, json);
        }

        static (string Current, string Previous, string Extension)
            GetLogFilePaths(string packageName, string logFilePath) {
            if (IsNullOrEmpty(logFilePath))
                logFilePath = $"Logs/{packageName}.log";

            var (fileName, extension) = logFilePath.SplitFileName();
            if (IsNullOrEmpty(fileName))
                return (null, null, null);

            var parent = logFilePath.GetParent();
            if (parent == null)
                return (null, null, null);

            if (parent != "" && !Directory.Exists(parent) && !parent.CreatePath())
                return (null, null, null);

            var previousLogFilePath =
                IsNullOrEmpty(extension)
                    ? $"{fileName}-prev"
                    : $"{fileName}-prev.{extension}";

            return (logFilePath, previousLogFilePath, extension);
        }
    }
}
