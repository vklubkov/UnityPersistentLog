using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Adds a `geometry changed` callback automatically removed after invoked once.
        [NotNull] public static T WithOnGeometryChangedOnce<T>(
            [NotNull] this T element,
            [NotNull] EventCallback<GeometryChangedEvent> callback)
                where T : VisualElement {
            element.RegisterCallbackOnce(callback);
            return element;
        }
    }
}