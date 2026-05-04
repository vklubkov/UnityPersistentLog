using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Adds a `focus` callback with custom data as the parameter.
        [NotNull] public static T WithOnFocus<T, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [NotNull] EventCallback<FocusEvent, TData> callback)
                where T : VisualElement {
            element.RegisterCallback(callback, data);
            return element;
        }
    }
}