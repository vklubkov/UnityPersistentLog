using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static class TextInputBaseFieldExtensions {
        [NotNull] public static T WithReadOnly<T, TValueType>([NotNull] this T element, bool value = true)
                where T : TextInputBaseField<TValueType> {
            element.isReadOnly = value;
            return element;
        }
    }
}