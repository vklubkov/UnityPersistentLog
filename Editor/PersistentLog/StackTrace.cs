using UnityEditorInternal;
using static System.IO.File;
using static System.String;
using static System.StringComparison;
using static System.StringSplitOptions;
using static UnityEditor.FileUtil;

#if !UNITY_6000_5_OR_NEWER
using static System.IO.Path;
#endif

namespace PersistentLog {
    internal static class StackTrace {
        public static string FormatStackTrace(string stackTrace) {
            if (IsNullOrEmpty(stackTrace))
                return "";

            var lines = stackTrace.Split(new[] { '\n', '\r' }, RemoveEmptyEntries);
            for (var i = 0; i < lines.Length; i++)
                lines[i] = HighlightLine(lines[i]);

            return Join("\n", lines);
        }

        public static string HighlightLine(string line) =>
            HandleLink(line, out var richText, out _, out _) ? richText : line;

        public static void OpenCodeForItem(LogItemInfo item) {
            if (!IsNullOrEmpty(item.StackTrace)) {
                var lines = item.StackTrace.Split(new[] { '\n', '\r' }, RemoveEmptyEntries);
                foreach (var line in lines) {
                    if (line.Contains("AutoDependencies.LogFile") ||
                        line.Contains("AutoDependencies.LogInterceptor") ||
                        line.Contains("AutoDependencies.LogItemInfo") ||
                        line.Contains("AutoDependencies.LogSerializer") ||
                        line.Contains("AutoDependencies.LogWindow") ||
                        line.Contains("AutoDependencies.StackTrace") ||
                        line.Contains("UnityEngine.Debug")) {
                        continue;
                    }

                    if (HandleLink(line, out _, out var filePath, out var lineNumber) &&
                        OpenCodeForLine(filePath, lineNumber)) {
                        return;
                    }
                }
            }

            if (!IsNullOrEmpty(item.Message) &&
                HandleLink(item.Message, out _, out var filePath1, out var lineNumber1)) {
                OpenCodeForLine(filePath1, lineNumber1);
            }
        }

        static bool HandleLink(string line, out string richText, out string path, out int lineNumber) {
            richText = null;
            path = null;
            lineNumber = -1;

            if (IsNullOrEmpty(line))
                return false;

            if (HandleCompilerError(line, out richText, out path, out lineNumber))
                return true;

            if (HandleAt(line, out richText, out path, out lineNumber))
                return true;

            if (HandleIn(line, out richText, out path, out lineNumber))
                return true;

            return false;

            // `path(line,column):`
            static bool HandleCompilerError(string line, out string richText, out string path, out int lineNumber) {
                richText = null;
                path = null;
                lineNumber = -1;

                try {
                    var openParenthesisIndex = line.IndexOf('(');
                    if (openParenthesisIndex <= 0)
                        return false;

                    var closeParenthesisIndex = line.IndexOf("):", startIndex: openParenthesisIndex, Ordinal);
                    if (closeParenthesisIndex <= 0)
                        return false;

                    var foundPath = line[..openParenthesisIndex].Trim();
                    if (!IsValidFile(foundPath))
                        return false;

                    path = foundPath;
                    var insideParentheses = line.Substring(
                        startIndex: openParenthesisIndex + 1,
                        length: closeParenthesisIndex - openParenthesisIndex - 1);

                    var insideParenthesesParts = insideParentheses.Split(',');
                    if (insideParenthesesParts.Length < 1)
                        return false;

                    var foundLineNumber = insideParenthesesParts[0].Trim();
                    if (!int.TryParse(foundLineNumber, out lineNumber))
                        return false;

                    var id = $"{foundPath}:{foundLineNumber}";
                    var value = $"{foundPath}({insideParentheses})";
                    var link = MakeRichTextLink(id, value);
                    richText = link + ":" + line[(closeParenthesisIndex + 2)..];
                    return true;
                } catch {
                    return false;
                }
            }

            // ` (at path:line)`
            static bool HandleAt(string line, out string richText, out string path, out int lineNumber) {
                richText = null;
                path = null;
                lineNumber = -1;

                try {
                    var atIndex = line.IndexOf(" (at ", Ordinal);
                    if (atIndex <= 0)
                        return false;

                    var colonIndex = line.LastIndexOf(':');
                    if (colonIndex <= atIndex)
                        return false;

                    var closeParenthesisIndex = line.LastIndexOf(')');
                    if (closeParenthesisIndex <= colonIndex)
                        return false;

                    var foundPath = line
                        .Substring(startIndex: atIndex + 5, length: colonIndex - (atIndex + 5))
                        .Trim();

                    if (!IsValidFile(foundPath))
                        return false;

                    path = foundPath;

                    var foundLineNumber = line
                        .Substring(startIndex: colonIndex + 1, length: closeParenthesisIndex - colonIndex - 1)
                        .Trim();

                    if (!int.TryParse(foundLineNumber, out lineNumber))
                        return false;

                    var prefix = line[..(atIndex + 5)];
                    var value = $"{foundPath}:{foundLineNumber}";
                    var link = MakeRichTextLink(id: value, value);
                    richText = prefix + link + ")" + line[(closeParenthesisIndex + 1)..];
                    return true;
                }
                catch {
                    return false;
                }
            }

            // ` in path:line`
            static bool HandleIn(string line, out string richText, out string path, out int lineNumber) {
                richText = null;
                path = null;
                lineNumber = -1;

                try {
                    var inIndex = line.IndexOf(" in ", Ordinal);
                    if (inIndex <= 0)
                        return false;

                    var colonIndex = line.LastIndexOf(':');
                    if (colonIndex <= inIndex)
                        return false;

                    var foundPath = line.Substring(inIndex + 4, colonIndex - (inIndex + 4)).Trim();
                    if (IsValidFile(foundPath))
                        return false;

                    path = foundPath;

                    var foundLineNumber = line[(colonIndex + 1)..].Trim();
                    var lineNumberSpaceIndex = foundLineNumber.IndexOf(' ');
                    if (lineNumberSpaceIndex != -1)
                        foundLineNumber = foundLineNumber[..lineNumberSpaceIndex];

                    if (!int.TryParse(foundLineNumber, out lineNumber))
                        return false;

                    var prefix = line[..(inIndex + 4)];
                    var value = $"{foundPath}:{foundLineNumber}";
                    var link = MakeRichTextLink(id: value, value);
                    var suffix = lineNumberSpaceIndex == -1 ? "" : line[(colonIndex + 1 + lineNumberSpaceIndex)..];
                    richText = prefix + link + suffix;
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        static string MakeRichTextLink(string id, string value) =>
            $"<color=#4C7EFF><u><link=\"{id}\">{value}</link></u></color>";

        public static bool OpenCodeForLine(string filePath, int lineNumber) {
            try {
                return IsValidFile(filePath) && InternalEditorUtility.OpenFileAtLineExternal(filePath, lineNumber);
            }
            catch {
                return false;
            }
        }

        static bool IsValidFile(string path) {
            if (IsNullOrEmpty(path))
                return false;

            if (path.StartsWith("."))
                path = path.TrimStart('.').TrimStart('/').TrimStart('\\');

#if UNITY_6000_5_OR_NEWER
            var absolutePath = PathToAbsolutePath(path);
#else
            var absolutePath = GetPhysicalPath(path);
            absolutePath = GetFullPath(absolutePath).Replace('\\', '/');
#endif
            return !IsNullOrEmpty(absolutePath) && Exists(absolutePath);
        }
    }
}