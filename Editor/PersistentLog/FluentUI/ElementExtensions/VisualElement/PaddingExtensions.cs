using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the left padding style.
        [NotNull] public static T WithPaddingLeft<T>([NotNull] this T element, StyleLength padding)
                where T : VisualElement {
            element.style.paddingLeft = padding;
            return element;
        }

        /// Sets the right padding style.
        [NotNull] public static T WithPaddingRight<T>([NotNull] this T element, StyleLength padding)
                where T : VisualElement {
            element.style.paddingRight = padding;
            return element;
        }

        /// Sets the left and right padding style.
        [NotNull] public static T WithPaddingLeftRight<T>([NotNull] this T element, StyleLength padding)
                where T : VisualElement {
            element.style.paddingLeft = padding;
            element.style.paddingRight = padding;
            return element;
        }

        /// Sets the padding style.
        [NotNull] public static T WithPadding<T>([NotNull] this T element, StyleLength padding)
                where T : VisualElement {
            element.style.paddingLeft = padding;
            element.style.paddingRight = padding;
            element.style.paddingTop = padding;
            element.style.paddingBottom = padding;
            return element;
        }
    }
}