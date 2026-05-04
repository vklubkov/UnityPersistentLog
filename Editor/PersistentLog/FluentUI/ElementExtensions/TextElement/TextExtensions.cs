using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class TextElementExtensions {
        /// Sets the text.
        [NotNull] public static T WithText<T>([NotNull] this T element, string text)
                where T : TextElement {
            element.text = text;
            return element;
        }
    }
}