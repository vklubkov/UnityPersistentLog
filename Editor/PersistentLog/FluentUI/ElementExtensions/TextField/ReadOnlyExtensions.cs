using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class TextFieldExtensions {
        // Sets element as isReadOnly.
        [NotNull] public static T AsReadOnly<T>([NotNull] this T element, bool value = true)
                where T : TextField {
            element.isReadOnly = value;
            return element;
        }
    }
}