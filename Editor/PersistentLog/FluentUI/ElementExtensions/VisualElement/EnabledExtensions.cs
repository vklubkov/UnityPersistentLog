using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets element as enabled.
        [NotNull] public static T AsEnabled<T>([NotNull] this T element, bool value = true)
                where T : VisualElement {
            element.SetEnabled(value);
            return element;
        }
    }
}