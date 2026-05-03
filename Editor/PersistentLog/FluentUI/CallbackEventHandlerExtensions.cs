using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    internal static class CallbackEventHandlerExtensions {
        [NotNull] public static T WithCallback<T, TEventType>(
            [NotNull] this T element,
            [NotNull] EventCallback<TEventType> callback)
                where T : CallbackEventHandler
                where TEventType : EventBase<TEventType>, new() {
            element.RegisterCallback(callback);
            return element;
        }

        [NotNull] public static T WithCallback<T, TEventType>(
            [NotNull] this T element,
            [NotNull] EventCallback<TEventType, T> callback)
                where T : CallbackEventHandler
                where TEventType : EventBase<TEventType>, new() {
            element.RegisterCallback(callback, element);
            return element;
        }

        [NotNull] public static T WithCallback<T, TEventType, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [NotNull] EventCallback<TEventType, TData> callback)
                where T : CallbackEventHandler
                where TEventType : EventBase<TEventType>, new() {
            element.RegisterCallback(callback, data);
            return element;
        }

        [NotNull] public static T WithCallbackOnce<T, TEventType>(
            [NotNull] this T element,
            [NotNull] EventCallback<TEventType> callback)
                where T : CallbackEventHandler
                where TEventType : EventBase<TEventType>, new() {
            element.RegisterCallbackOnce(callback);
            return element;
        }
    }
}