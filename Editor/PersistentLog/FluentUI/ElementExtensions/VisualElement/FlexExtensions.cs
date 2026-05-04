using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the flex shrink style.
        [NotNull] public static T WithFlexShrink<T>([NotNull] this T element, StyleFloat flexShrink)
                where T : VisualElement {
            element.style.flexShrink = flexShrink;
            return element;
        }

        /// Sets the flex direction style.
        [NotNull] public static T WithFlexDirection<T>([NotNull] this T element, FlexDirection flexDirection)
                where T : VisualElement {
            element.style.flexDirection = flexDirection;
            return element;
        }

        /// Sets the flex grow style.
        [NotNull] public static T WithFlexGrow<T>([NotNull] this T element, StyleFloat flexGrow)
                where T : VisualElement {
            element.style.flexGrow = flexGrow;
            return element;
        }
    }
}