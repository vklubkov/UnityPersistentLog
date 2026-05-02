using System.IO;
using static System.String;

namespace PersistentLog {
    public static class PathExtensions {
        public static string GetParent(this string path) {
            try {
                return Path.GetDirectoryName(path)?.Replace('\\', '/');
            }
            catch {
                return null;
            }
        }

        public static bool CreatePath(this string path) {
            try {
                Directory.CreateDirectory(path);
                return true;
            }
            catch {
                return false;
            }
        }

        public static (string FileName, string Extension) SplitFileName(this string path) {
            if (IsNullOrEmpty(path))
                return (Empty, Empty);

            try {
                var filenameWithoutExtension = path.RemoveExtension();
                var extension = path.GetExtension()?.TrimStart('.') ?? "";
                return (filenameWithoutExtension, extension);
            } catch {
                return (Empty, Empty);
            }
        }

        static string RemoveExtension(this string path) => Path.ChangeExtension(path, null)?.Replace('\\', '/');
        static string GetExtension(this string path) => Path.GetExtension(path);
    }
}