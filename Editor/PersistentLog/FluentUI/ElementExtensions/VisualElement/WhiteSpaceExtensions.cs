using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the white space style.
        [NotNull] public static T WithWhiteSpace<T>([NotNull] this T element, StyleEnum<WhiteSpace> whiteSpace)
                where T : VisualElement {
            element.style.whiteSpace = whiteSpace;
            return element;
        }
    }
}