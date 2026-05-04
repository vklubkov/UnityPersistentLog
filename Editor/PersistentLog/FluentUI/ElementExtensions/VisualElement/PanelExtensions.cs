using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Adds an `attach to panel` callback with the element itself as the parameter.
        [NotNull] public static T WithOnAttachToPanel<T>(
            [NotNull] this T element,
            [NotNull] EventCallback<AttachToPanelEvent, T> callback)
                where T : VisualElement {
            element.RegisterCallback(callback, element);
            return element;
        }
    }
}