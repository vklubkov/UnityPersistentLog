using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    internal static class TextElementExtensions {
        [NotNull] public static T WithText<T>([NotNull] this T element, [CanBeNull] string text)
                where T : TextElement {
            element.text = text;
            return element;
        }

        [NotNull] public static T WithRichText<T>([NotNull] this T element, bool value = true)
                where T : TextElement {
            element.enableRichText = value;
            return element;
        }
    }
}