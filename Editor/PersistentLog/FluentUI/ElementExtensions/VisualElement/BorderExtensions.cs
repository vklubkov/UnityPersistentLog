using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the top border width style.
        [NotNull] public static T WithBorderTopWidth<T>([NotNull] this T element, StyleFloat width)
                where T : VisualElement {
            element.style.borderTopWidth = width;
            return element;
        }

        /// Sets the border width style.
        [NotNull] public static T WithBorderWidth<T>([NotNull] this T element, StyleFloat width)
                where T : VisualElement {
            element.style.borderTopWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderBottomWidth = width;
            element.style.borderRightWidth = width;
            return element;
        }
    }
}