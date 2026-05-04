using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class TextFieldExtensions {
        /// Sets element as multiline.
        [NotNull] public static T AsMultiline<T>([NotNull] this T element, bool value = true)
            where T : TextField {
            element.multiline = value;
            return element;
        }
    }
}