using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Adds the manipulator returned via a callback with the element itself as a parameter.
        [NotNull] public static T WithManipulator<T>(
            [NotNull] this T element,
            [CanBeNull] Func<T, IManipulator> create)
                where T : VisualElement {
            element.AddManipulator(create?.Invoke(element));
            return element;
        }
    }
}