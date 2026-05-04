using JetBrains.Annotations;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Adds a `pointer up link tag` callback.
        [NotNull] public static T WithOnPointerUpLinkTag<T>(
            [NotNull] this T element,
            [NotNull] EventCallback<PointerUpLinkTagEvent> callback)
                where T : VisualElement {
            element.RegisterCallback(callback);
            return element;
        }
    }
}