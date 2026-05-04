using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    // ReSharper disable once InconsistentNaming
    public static partial class INotifyValueChangedExtensions {
        /// Sets the value of the element.
        [NotNull] public static T WithValue<T, TValue>([NotNull] this T element, TValue value)
                where T : INotifyValueChanged<TValue> {
            element.value = value;
            return element;
        }
    }
}