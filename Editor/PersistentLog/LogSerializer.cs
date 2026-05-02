using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using static System.Enum;
using static System.String;
using static System.DateTime;
using static System.StringSplitOptions;

namespace PersistentLog {
    internal static class LogSerializer {
        const string _itemsSeparator = "\n\n\n";
        const char _valueSeparator = ':';
        const string _indent = "  ";
        const string _multiline = "|";

        public static string SerializeMessages(IEnumerable<LogItemInfo> logs) =>
            Join('\n', logs.Select(SerializeMessage));

        static string SerializeMessage(LogItemInfo logItem) =>
            $"{logItem.Timestamp:HH:mm:ss.fff} {logItem.Message}";

        public static string SerializeLog(IEnumerable<LogItemInfo> logs) =>
            Join(_itemsSeparator, logs.Select(SerializeLogItem));

        public static string SerializeLogItem(bool prependSeparator, LogItemInfo logItem) =>
            prependSeparator
                ? _itemsSeparator + SerializeLogItem(logItem)
                : SerializeLogItem(logItem);

        static string SerializeLogItem(LogItemInfo logItem) {
            var serialized = $"Timestamp{_valueSeparator} {logItem.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\n";
            serialized += $"Type{_valueSeparator} {logItem.Type}\n";
            serialized += $"Message{_valueSeparator} {_multiline}\n{Indent(logItem.Message)}\n";
            serialized += $"StackTrace{_valueSeparator} {_multiline}\n{Indent(logItem.StackTrace)}";
            return serialized;

            static string Indent(string text) => Join("\n", (text ?? "").Split('\n').Select(line => _indent + line));
        }

        public static void DeserializeLog(string serialized, List<LogItemInfo> logs) {
            var entries = serialized.Split(new[] { _itemsSeparator }, RemoveEmptyEntries);
            foreach (var entry in entries) {
                var logItem = DeserializeLogItem(entry);
                if (logItem != null)
                    logs.Add(logItem);
            }
        }

        static LogItemInfo DeserializeLogItem(string serialized) {
            DateTime timestamp = default;
            var type = "";
            var messageLines = new List<string>();
            var stackTraceLines = new List<string>();

            List<string> currentLines = null;

            var lines = serialized.Split('\n');
            foreach (var line in lines) {
                if (line.StartsWith(_indent) && currentLines != null) {
                    currentLines.Add(line[2..]);
                    continue;
                }

                currentLines = null;
                var colonIndex = line.IndexOf(_valueSeparator);
                if (colonIndex == -1)
                    continue;

                var field = line[..colonIndex].Trim();
                var value = line[(colonIndex + 1)..].Trim();

                switch (field) {
                    case "Timestamp":
                        TryParseExact(value, "yyyy-MM-dd HH:mm:ss.fff", null, DateTimeStyles.None, out timestamp);
                        break;
                    case "Type":
                        type = value;
                        break;
                    case "Message":
                        currentLines = messageLines;
                        if (value != _multiline)
                            messageLines.Add(value);

                        break;
                    case "StackTrace":
                        currentLines = stackTraceLines;
                        if (value != _multiline)
                            stackTraceLines.Add(value);

                        break;
                }
            }

            var message = Join("\n", messageLines);
            var stackTrace = Join("\n", stackTraceLines);
            if (timestamp == default || IsNullOrEmpty(type))
                return null;

            LogItemInfo logItemInfo = null;
            if (TryParse<LogType>(type, out var logType))
                logItemInfo = new LogItemInfo(timestamp, logType, message, stackTrace);

            return logItemInfo;
        }
    }
}
