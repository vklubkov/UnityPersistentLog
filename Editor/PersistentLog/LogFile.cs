using System.Collections.Generic;
using System.IO;
using static System.IO.File;
using static System.String;
using static PersistentLog.LogSerializer;
using static UnityEditor.EditorUtility;

namespace PersistentLog {
    internal static class LogFile {
        public static void New(string currentPath, string previousPath) {
            if (IsNullOrEmpty(currentPath))
                return;

            var parent = currentPath.GetParent();
            if (parent == null)
                return;

            if (parent != "" && !Directory.Exists(parent) && !parent.CreatePath())
                return;

            if (Exists(currentPath)) {
                Copy(currentPath, previousPath, overwrite: true);
                Delete(currentPath);
            }
        }

        public static string LoadLastLog(string currentPath, string previousPath, List<LogItemInfo> allLogs) {
            string path;
            if (Exists(currentPath))
                path = currentPath;
            else if (Exists(previousPath))
                path = previousPath;
            else
                return null;

            if (IsNullOrEmpty(path))
                return null;

            ReadAllLogs(path, allLogs);
            return path;
        }

        public static void Append(string path, LogItemInfo logItem) {
            if (IsNullOrEmpty(path))
                return;

            var parent = path.GetParent();
            if (parent == null)
                return;

            if (parent != "" && !Directory.Exists(parent) && !parent.CreatePath())
                return;

            var serialized = SerializeLogItem(Exists(path), logItem);
            AppendAllText(path, serialized);
        }

        public static string Write(string package, string directory, string extension, List<LogItemInfo> logs) {
            if (IsNullOrEmpty(package))
                return null;

            var path = SaveFilePanel($"Save {package} Log", directory ?? "", $"{package}.log", extension ?? "");
            if (IsNullOrEmpty(path))
                return null;

            WriteAllLogs(path, logs);
            return path;
        }

        static void WriteAllLogs(string path, List<LogItemInfo> logs) {
            var serialized = SerializeLog(logs);
            WriteAllText(path, serialized);
        }

        public static string Read(string package, string directory, string extension, List<LogItemInfo> logs) {
            if (IsNullOrEmpty(package))
                return null;

            var path = OpenFilePanel($"Load {package} Log", directory ?? "", extension ?? "");
            if (IsNullOrEmpty(path))
                return null;

            ReadAllLogs(path, logs);
            return path;
        }

        static void ReadAllLogs(string path, List<LogItemInfo> logs) {
            logs.Clear();
            var serialized = ReadAllText(path);
            DeserializeLog(serialized, logs);
        }
    }
}
