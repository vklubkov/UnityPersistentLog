using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Adds the element to the specified parent.
        [NotNull] public static T AddTo<T, TParent>([NotNull] this T element, [NotNull] TParent parent)
                where T : VisualElement
                where TParent : VisualElement {
            parent.Add(element);
            return element;
        }
    }
}