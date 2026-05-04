using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Runs a callback with the query result and custom data as parameters.
        [NotNull] public static T WithQ<T, TQuery, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [CanBeNull] Action<TQuery, TData> callback)
                where T : VisualElement
                where TQuery : VisualElement {
            var found = element.Q<TQuery>();
            if (found == null)
                return element;

            callback?.Invoke(found, data);
            return element;
        }
    }
}