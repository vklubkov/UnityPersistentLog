using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the minimum height style.
        [NotNull] public static T WithMinHeight<T>([NotNull] this T element, StyleLength height)
                where T : VisualElement {
            element.style.minHeight = height;
            return element;
        }

        /// Sets the width and height style.
        [NotNull] public static T WithSize<T>([NotNull] this T element, StyleLength size)
                where T : VisualElement {
            element.style.width = size;
            element.style.height = size;
            return element;
        }
    }
}