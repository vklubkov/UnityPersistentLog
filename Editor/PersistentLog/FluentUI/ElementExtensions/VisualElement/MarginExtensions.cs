using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the left margin style.
        [NotNull] public static T WithMarginLeft<T>([NotNull] this T element, StyleLength margin)
                where T : VisualElement {
            element.style.marginLeft = margin;
            return element;
        }

        /// Sets the margin style.
        [NotNull] public static T WithMargin<T>(
            [NotNull] this T element,
            StyleLength leftRight,
            StyleLength topBottom)
                where T : VisualElement {
            element.style.marginLeft = leftRight;
            element.style.marginRight = leftRight;
            element.style.marginTop = topBottom;
            element.style.marginBottom = topBottom;
            return element;
        }
    }
}