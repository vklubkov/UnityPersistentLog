using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Runs a callback with the element itself as a parameter.
        [NotNull] public static T WithSelf<T>([NotNull] this T element, [CanBeNull] Action<T> callback)
                where T : VisualElement {
            callback?.Invoke(element);
            return element;
        }

        /// Runs a callback with the element itself and custom data as parameters.
        [NotNull] public static T WithSelf<T, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [CanBeNull] Action<T, TData> callback)
                where T : VisualElement {
            callback?.Invoke(element, data);
            return element;
        }
    }
}