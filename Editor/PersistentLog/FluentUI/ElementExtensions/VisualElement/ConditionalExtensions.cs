using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Runs conditional callbacks with the element itself as a parameter.
        [NotNull] public static T WithCondition<T>(
            [NotNull] this T element,
            bool condition,
            [CanBeNull] Action<T> onTrue = null,
            [CanBeNull] Action<T> onFalse = null)
                where T : VisualElement {
            if (condition)
                onTrue?.Invoke(element);
            else
                onFalse?.Invoke(element);

            return element;
        }
    }
}