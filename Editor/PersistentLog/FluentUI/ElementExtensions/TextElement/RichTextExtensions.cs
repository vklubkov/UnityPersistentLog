using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class TextElementExtensions {
        /// Enables or disables rich text.
        [NotNull] public static T WithRichText<T>([NotNull] this T element, bool enable = true)
                where T : TextElement {
            element.enableRichText = enable;
            return element;
        }
    }
}