using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class VisualElementExtensions {
        /// Sets the text alignment style.
        [NotNull] public static T WithTextAlign<T>([NotNull] this T element, TextAnchor textAlign)
                where T : VisualElement {
            element.style.unityTextAlign = textAlign;
            return element;
        }
    }
}