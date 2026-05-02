using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    // ReSharper disable once InconsistentNaming
    public static class INotifyValueChangedExtensions {
        [NotNull] public static T WithValue<T, TValue>([NotNull] this T element, TValue value)
                where T : INotifyValueChanged<TValue> {
            element.value = value;
            return element;
        }

        [NotNull] public static T WithValueChangedCallback<T, TValue, TData>(
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