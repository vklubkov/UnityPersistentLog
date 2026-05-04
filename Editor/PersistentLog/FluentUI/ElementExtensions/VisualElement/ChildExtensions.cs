using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Adds the child element returned via a callback.
        [NotNull] public static T WithAdd<T, TChild>([NotNull] this T element, [CanBeNull] Func<TChild> callback)
                where T : VisualElement
                where TChild : VisualElement {
            var child = callback?.Invoke();
            if (child == null)
                return element;

            element.Add(child);
            return element;
        }

        /// Adds the child element returned via a callback with custom data as a parameter.
        [NotNull] public static T WithAdd<T, TChild, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [CanBeNull] Func<TData, TChild> callback)
                where T : VisualElement
                where TChild : VisualElement {
            var child = callback?.Invoke(data);
            if (child == null)
                return element;

            element.Add(child);
            return element;
        }

        /// Adds the child element returned via a callback with the element itself and custom data as parameters.
        [NotNull] public static T WithAdd<T, TChild, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [CanBeNull] Func<T, TData, TChild> callback)
                where T : VisualElement
                where TChild : VisualElement {
            var child = callback?.Invoke(element, data);
            if (child == null)
                return element;

            element.Add(child);
            return element;
        }

        /// Adds the child element returned via a callback and returns the child element as the out parameter.
        [NotNull] public static T WithAdd<T, TChild>(
            [NotNull] this T element,
            [CanBeNull] out TChild result,
            [CanBeNull] Func<TChild> callback)
                where T : VisualElement
                where TChild : VisualElement {
            result = null;
            var child = callback?.Invoke();
            if (child == null)
                return element;

            element.Add(child);
            result = child;
            return element;
        }
    }
}