using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    internal static class TextFieldExtensions {
        [NotNull] public static T AsMultiline<T>([NotNull] this T element, bool value = true)
                where T : TextField {
            element.multiline = value;
            return element;
        }

        [NotNull] public static T AsReadOnly<T>([NotNull] this T element, bool value = true)
            where T : TextInputBaseField<string> {
            element.isReadOnly = value;
            return element;
        }

        [NotNull] public static T WithClearSelection<T>([NotNull] this T element)
                where T : TextInputBaseField<string> {
            element.SelectRange(0, 0);
            return element;
        }
    }
}