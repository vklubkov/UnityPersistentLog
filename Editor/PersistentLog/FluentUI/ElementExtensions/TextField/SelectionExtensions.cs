using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class TextFieldExtensions {
        // Sets the selection range.
        [NotNull] public static T WithSelectRange<T>([NotNull] this T element, int cursorIndex, int selectionIndex)
            where T : TextField {
            element.SelectRange(cursorIndex, selectionIndex);
            return element;
        }

        // Clears selection.
        [NotNull] public static T WithClearSelection<T>([NotNull] this T element)
                where T : TextField {
            element.SelectRange(0, 0);
            return element;
        }
    }
}