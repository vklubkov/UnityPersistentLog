using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static System.String;
using static System.StringComparison;
using static PersistentLog.LogSerializer;
using static PersistentLog.StackTrace;
using static Newtonsoft.Json.JsonConvert;
using static Newtonsoft.Json.MemberSerialization;
using static PersistentLog.LogFile;
using static UnityEditor.EditorGUIUtility;
using static UnityEditor.EditorUserSettings;
using static UnityEngine.Color;
using static UnityEngine.TextAnchor;
using static UnityEngine.UIElements.CollectionVirtualizationMethod;
using static UnityEngine.UIElements.FlexDirection;
using static UnityEngine.UIElements.TwoPaneSplitViewOrientation;
using static UnityEngine.UIElements.WhiteSpace;

namespace PersistentLog {
    internal class LogWindow : EditorWindow {
        [JsonObject(OptIn)]
        class PermanentState  {
            [JsonProperty("lastOpenedFilePath")] public string LastOpenedFilePath { get; set; }
        }

        const string _mainKey = "PersistentLog";

        static GUIContent _infoIcon;
        static GUIContent _warningIcon;
        static GUIContent _errorIcon;

        static Texture InfoIcon => (_infoIcon ??= IconContent("console.infoicon.sml")).image;
        static Texture WarningIcon => (_warningIcon ??= IconContent("console.warnicon.sml")).image;
        static Texture ErrorIcon => (_errorIcon ??= IconContent("console.erroricon.sml")).image;

        Action _onDestroy;

        bool _isInitialized;

        [SerializeField] string _packageName;
        [SerializeField] string _serializedLogs;

        [SerializeField] string _filterText = "";
        [SerializeField] bool _showInfo = true;
        [SerializeField] bool _showWarnings = true;
        [SerializeField] bool _showErrors = true;

        [SerializeField] string _stackTraceValue = "";

        [SerializeField] string _currentPath = "";

        static PermanentState _permanentState;

        List<LogItemInfo> _allLogs = new();
        readonly List<LogItemInfo> _filteredLogs = new();

        Label _infoCountLabel;
        Label _warningCountLabel;
        Label _errorCountLabel;

        Button _openFolderButton;

        ListView _listView;
        TextField _stackTraceField;

        public static LogWindow Get(string packageName, Action onDestroy) {
            LogWindow window = null;
            if (HasOpenInstances<LogWindow>()) {
                var foundWindows = (LogWindow[])Resources.FindObjectsOfTypeAll(typeof(LogWindow));
                foreach (var foundWindow in foundWindows) {
                    if (foundWindow.titleContent.text == $"{packageName} Log") {
                        window = foundWindow;
                        break;
                    }
                }
            }

            if (window == null)
                window = CreateWindow<LogWindow>($"{packageName} Log");

            window._packageName = packageName;

            if (onDestroy != null)
                window._onDestroy += onDestroy;

            return window;
        }

        void OnEnable() {
            var userJson = GetConfigValue(_mainKey + ":" + _packageName);
            _permanentState = IsNullOrEmpty(userJson) ? new() : DeserializeObject<PermanentState>(userJson) ?? new();

            var allLogs = DeserializeObject<List<LogItemInfo>>(_serializedLogs ?? "", new StringEnumConverter()) ?? new();
            if (allLogs.Count != 0) {
                if (_allLogs.Count != 0)
                    allLogs.AddRange(_allLogs);

                _allLogs = allLogs;
                _serializedLogs = SerializeObject(_allLogs, new StringEnumConverter());
            }

            _isInitialized = true;
        }

        void OnDisable() {
            _permanentState = null;
            _isInitialized = false;
        }

        void OnDestroy() {
            _onDestroy?.Invoke();
            _onDestroy = null;
        }

        public void LoadLastLogIfNeeded(string current, string previous) {
            if (_allLogs.Count != 0)
                return;

            _currentPath = LoadLastLog(current, previous, _allLogs);
            _openFolderButton?.AsEnabled(!IsNullOrEmpty(_currentPath));
            _serializedLogs = SerializeObject(_allLogs, new StringEnumConverter());
            _listView?.WithClearSelection();
            RefreshFilteredList(shouldScrollToLastItem: false);
        }

        public void Log(string path, LogItemInfo logItem) {
            _allLogs.Add(logItem);
            if (!_isInitialized)
                return;

            _currentPath = path ?? "";
            _openFolderButton?.AsEnabled(!IsNullOrEmpty(_currentPath));
            _serializedLogs = SerializeObject(_allLogs, new StringEnumConverter());
            RefreshFilteredList(shouldScrollToLastItem: true);
        }

        public void CreateGUI() {
            var root = rootVisualElement;

            CreateFilterBar(parent: root, window: this);
            CreateSplitView(parent: root, window: this);
            CreateBottomBar(parent: root, window: this);

            RefreshFilteredList(shouldScrollToLastItem: false);

            static void CreateFilterBar(VisualElement parent, LogWindow window)
                => UIExtensions.Toolbar()
                    .WithChild(data: window, static window
                        => UIExtensions.ToolbarSearchField()
                            .WithValue(window._filterText)
                            .WithFlexGrow(1)
                            .WithValueChangedCallback(data: window,
                                static (ChangeEvent<string> evt, LogWindow window) => {
                                    window._filterText = evt.newValue;
                                    window.RefreshFilteredList(shouldScrollToLastItem: false);
                                }))
                    .WithChild(data: window, static window
                        => CreateFilterToggle(LogType.Log, window._showInfo, out window._infoCountLabel)
                            .WithValueChangedCallback(data: window,
                                static (ChangeEvent<bool> evt, LogWindow window) => {
                                    window._showInfo = evt.newValue;
                                    window.RefreshFilteredList(shouldScrollToLastItem: false);
                                }))
                    .WithChild(data: window, static window
                        => CreateFilterToggle(LogType.Warning, window._showWarnings, out window._warningCountLabel)
                            .WithValueChangedCallback(data: window,
                                static (ChangeEvent<bool> evt, LogWindow window) => {
                                    window._showWarnings = evt.newValue;
                                    window.RefreshFilteredList(shouldScrollToLastItem: false);
                                }))
                    .WithChild(data: window, static window
                        => CreateFilterToggle(LogType.Error, window._showErrors, out window._errorCountLabel)
                            .WithValueChangedCallback(data: window,
                                static (ChangeEvent<bool> evt, LogWindow window) => {
                                    window._showErrors = evt.newValue;
                                    window.RefreshFilteredList(shouldScrollToLastItem: false);
                                }))
                    .AddTo(parent);

            static ToolbarToggle CreateFilterToggle(LogType type, bool initialState, out Label countLabel)
                => UIExtensions.ToolbarToggle()
                    .WithValue(initialState)
                    .WithPaddingLeftRight(4)
                    .WithChild(data: type, static type
                        => UIExtensions.Image()
                            .WithImage(GetIcon(type))
                            .WithSize(16))
                    .WithChild(out countLabel, static ()
                        => UIExtensions.Label()
                            .WithValue("0")
                            .WithMarginLeft(2)
                            .WithTextAlign(MiddleLeft));

            static void CreateSplitView(VisualElement parent, LogWindow window)
                => UIExtensions.TwoPaneSplitView(0, 100, Vertical)
                    .WithFlexGrow(1)
                    .WithViewDataKey("LogWindow_SplitView")
                    .WithChild(data: window, static (splitView, window) => CreateListView(splitView, window))
                    .WithChild(data: window,
                        static (splitView, window) => CreateStackTraceScrollView(splitView, window))
                    .AddTo(parent);

            static ListView CreateListView(VisualElement parent, LogWindow window) {
                var listView = UIExtensions.ListView(window._filteredLogs, -1, MakeItem, window.BindItem)
                    .WithVirtualizationMethod(DynamicHeight)
                    .WithFlexGrow(1)
                    .WithSelectionType(SelectionType.Multiple)
                    .WithViewDataKey("LogWindow_ListView")
                    .WithMakeNoneElement(() => new VisualElement())
                    .WithManipulator(static listView
                        => new ContextualMenuManipulator(evt => {
                            var selectedItems = listView.selectedItems.Cast<LogItemInfo>().ToList();
                            if (selectedItems.Count == 0)
                                return;

                            evt.menu.AppendAction("Copy", _
                                => systemCopyBuffer = SerializeLog(selectedItems));

                            evt.menu.AppendAction("Copy Compact", _
                                => systemCopyBuffer = SerializeMessages(selectedItems));
                        }))
                    .AddTo(parent);

                listView.selectionChanged += objects => {
                    var selectedItems = objects.Cast<LogItemInfo>().ToList();
                    if (selectedItems.Count == 0)
                        return;

                    var firstSelected = selectedItems[0];
                    if (firstSelected == null) {
                        listView.WithClearSelection();
                        return;
                    }

                    var message = HighlightLine(firstSelected.Message);
                    var stackTrace = FormatStackTrace(firstSelected.StackTrace);
                    stackTrace = Join('\n', message, stackTrace);

                    window._stackTraceField.value = stackTrace;
                    window._stackTraceValue = stackTrace;
                };

                listView.itemsChosen += objects => {
                    if (objects.FirstOrDefault() is LogItemInfo item)
                        OpenCodeForItem(item);
                };

                window._listView = listView;
                return listView;
            }

            static VisualElement MakeItem()
                => UIExtensions.VisualElement()
                    .WithFlexDirection(Row)
                    .WithAlignItems(Align.FlexStart)
                    .WithMinHeight(24)
                    .WithChild(static ()
                        => UIExtensions.Image()
                            .WithSize(16)
                            .WithFlexShrink(0)
                            .WithMargin(leftRight: 4, topBottom: 0))
                    .WithChild(static ()
                        => UIExtensions.Label()
                            .WithFlexGrow(1)
                            .WithTextAlign(MiddleLeft)
                            .WithWhiteSpace(Normal));

            static ScrollView CreateStackTraceScrollView(VisualElement parent, LogWindow window)
                => UIExtensions.ScrollView()
                    .WithFlexGrow(1)
                    .WithBorderTopWidth(1)
                    .WithChild(window, static window
                        => window._stackTraceField = UIExtensions.TextField()
                            .AsMultiline()
                            .AsReadOnly()
                            .WithFlexGrow(1)
                            .WithWhiteSpace(Normal)
                            .WithPadding(4)
                            .WithBackgroundColor(clear)
                            .WithBorderWidth(0)
                            .WithCallback(static (AttachToPanelEvent _, TextField stackTraceField)
                                => stackTraceField.WithQ("unity-text-input", static textInput => textInput?
                                    .WithBackgroundColor(clear)
                                    .WithBorderWidth(0)
                                    .WithWhiteSpace(Normal)))
                            .WithQ(static (TextField stackTraceField, TextElement textElement) => textElement
                                .WithRichText()
                                .WithCallback(data: stackTraceField, static (FocusEvent _, TextField stackTraceField)
                                    => stackTraceField.WithClearSelection())
                                .WithCallback(static (PointerUpLinkTagEvent evt) => {
                                    var link = evt.linkID;
                                    if (IsNullOrEmpty(link))
                                        return;

                                    var parts = link.Split(':');
                                    if (parts.Length != 2)
                                        return;

                                    var path = parts[0];
                                    var lineNumber = int.Parse(parts[1]);
                                    OpenCodeForLine(path, lineNumber);
                                }))
                            .WithValue(window._stackTraceValue))
                    .AddTo(parent);

            static void CreateBottomBar(VisualElement parent, LogWindow window)
                => UIExtensions.VisualElement()
                    .WithFlexDirection(Row)
                    .WithChild(data: window, static window
                        => UIExtensions.VisualElement()
                            .WithFlexDirection(Row)
                            .WithFlexGrow(1)
                            .WithChild(data: window, static window
                                => window._openFolderButton = UIExtensions.Button(window.OpenFolder)
                                    .WithText("Open Folder")
                                    .AsEnabled(!IsNullOrEmpty(window._currentPath))))
                    .WithChild(data: window, static window
                        => UIExtensions.VisualElement()
                            .WithFlexDirection(Row)
                            .WithChild(data: window, static window
                                => UIExtensions.Button(window.Clear)
                                    .WithText("Clear"))
                            .WithChild(data: window, static window
                                => UIExtensions.Button(window.Load)
                                    .WithText("Load"))
                            .WithChild(data: window, static window
                                => UIExtensions.Button(window.Save)
                                    .WithText("Save")))
                    .AddTo(parent);
        }

        void BindItem(VisualElement element, int index) => BindItem(element, _filteredLogs[index]);

        static void BindItem(VisualElement element, LogItemInfo logItem)
            => element
                .WithQ(data: logItem, static (VisualElement _, Label label, LogItemInfo logItem)
                    => label.WithText($"[{logItem.Timestamp:HH:mm:ss}] {logItem.Message}"))
                .WithQ(data: logItem, static (VisualElement _, Image icon, LogItemInfo logItem)
                    => icon.WithImage(GetIcon(logItem.Type)));

        static Texture GetIcon(LogType type) => type switch {
            LogType.Error or LogType.Assert or LogType.Exception => ErrorIcon,
            LogType.Warning => WarningIcon,
            _ => InfoIcon
        };

        void OpenFolder() {
            if (!IsNullOrEmpty(_currentPath))
                EditorUtility.RevealInFinder(_currentPath);
        }

        public void Clear() {
            _listView?.WithClearSelection();

            _allLogs.Clear();
            _serializedLogs = SerializeObject(_allLogs, new StringEnumConverter());

            _stackTraceField?.WithValue("");
            _stackTraceValue = "";

            _openFolderButton?.AsEnabled(false);
            _currentPath = "";

            RefreshFilteredList();
        }

        void Load() {
            _listView?.WithClearSelection();

            var directory =
                _permanentState.LastOpenedFilePath?.GetParent()
                ?? Application.dataPath.GetParent() + "/Logs";

            var path = Read(_packageName, directory, "", _allLogs);
            if (IsNullOrEmpty(path))
                return;

            _permanentState.LastOpenedFilePath = path;
            SavePermanentState();

            _currentPath = path;
            _openFolderButton?.AsEnabled(!IsNullOrEmpty(_currentPath));
            _serializedLogs = SerializeObject(_allLogs, new StringEnumConverter());
            RefreshFilteredList(shouldScrollToLastItem: false);
        }

        void Save() {
            var directory =
                _permanentState.LastOpenedFilePath?.GetParent()
                ?? Application.dataPath.GetParent() + "/Logs";

           var path = Write(_packageName, directory, "", _allLogs);
            if (IsNullOrEmpty(path))
                return;

            _permanentState.LastOpenedFilePath = path;
            SavePermanentState();

            _currentPath = path;
            _openFolderButton?.AsEnabled(!IsNullOrEmpty(_currentPath));
        }

        void SavePermanentState() {
            var json = SerializeObject(_permanentState, new StringEnumConverter());
            SetConfigValue(_mainKey + ":" + _packageName, json);
        }

        void RefreshFilteredList(bool shouldScrollToLastItem = false) {
            _filteredLogs.Clear();

            int infoCount = 0, warningCount = 0, errorCount = 0;

            foreach (var logItem in _allLogs) {
                Count(logItem, ref infoCount, ref warningCount, ref errorCount);

                if (!MatchesTypeFilter(this, logItem))
                    continue;

                if (!MatchesTextFilter(this, logItem))
                    continue;

                _filteredLogs.Add(logItem);
            }

            if (_infoCountLabel != null)
                _infoCountLabel.text = infoCount.ToString();

            if (_warningCountLabel != null)
                _warningCountLabel.text = warningCount.ToString();

            if (_errorCountLabel != null)
                _errorCountLabel.text = errorCount.ToString();

            _listView?
                .WithRebuild()
                .WithCondition(condition: shouldScrollToLastItem, static listView => listView
                    .WithScrollToLastItem()
                    .WithCallbackOnce(static (GeometryChangedEvent evt) => {
                        if (evt.target is ListView listView)
                            listView.WithScrollToLastItem();
                    }));

            static void Count(LogItemInfo logItem, ref int infoCount, ref int warningCount, ref int errorCount) {
                switch (logItem.Type) {
                    case LogType.Log:
                        infoCount++;
                        break;
                    case LogType.Warning:
                        warningCount++;
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                    case LogType.Assert:
                        errorCount++;
                        break;
                }
            }

            static bool MatchesTypeFilter(LogWindow window, LogItemInfo logItem) => logItem.Type switch {
                LogType.Log => window._showInfo,
                LogType.Warning => window._showWarnings,
                LogType.Error or LogType.Exception or LogType.Assert => window._showErrors,
                _ => false
            };

            static bool MatchesTextFilter(LogWindow window, LogItemInfo logItem) =>
                IsNullOrEmpty(window._filterText) ||
                logItem.Message?.IndexOf(window._filterText, OrdinalIgnoreCase) >= 0;
        }
    }
}