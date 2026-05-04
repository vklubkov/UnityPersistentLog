using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the background color style.
        [NotNull] public static T WithBackgroundColor<T>([NotNull] this T element, StyleColor color)
                where T : VisualElement {
            element.style.backgroundColor = color;
            return element;
        }
    }
}