using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the view data key.
        [NotNull] public static T WithViewDataKey<T>([NotNull] this T element, [CanBeNull] string viewDataKey)
                where T : VisualElement {
            element.viewDataKey = viewDataKey;
            return element;
        }
    }
}