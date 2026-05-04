using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the align items style.
        [NotNull] public static T WithAlignItems<T>([NotNull] this T element, StyleEnum<Align> alignItems)
                where T : VisualElement {
            element.style.alignItems = alignItems;
            return element;
        }
    }
}