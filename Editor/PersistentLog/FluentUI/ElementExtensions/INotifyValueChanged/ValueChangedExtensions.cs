using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    // ReSharper disable once InconsistentNaming
    public static partial class INotifyValueChangedExtensions {
        /// Sets a `value changed` callback with custom data as the parameter.
        [NotNull] public static T WithOnValueChanged<T, TValue, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [NotNull] EventCallback<ChangeEvent<TValue>, TData> callback)
                where T : INotifyValueChanged<TValue> {
            // See UnityEngine.UIElements.INotifyValueChangedExtensions.RegisterValueChangedCallback
            if (!(element is CallbackEventHandler callbackEventHandler))
                return element;

            callbackEventHandler.RegisterCallback(callback, data);
            return element;
        }
    }
}