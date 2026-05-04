using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class TextFieldExtensions {
        /// Runs a callback with the element itself and the element's child TextElement as parameters.
        [NotNull] public static T WithOnTextElement<T>(
            [NotNull] this T element,
            [CanBeNull] Action<T, TextElement> callback)
                where T : TextField {
            var textElement = element.Q<TextElement>();
            if (textElement == null) {
                Debug.LogWarning("Fluent Editor UIElements: child TextElement not " +
                                 $"found for element of type {element.GetType().Name}");

                return element;
            }

            callback?.Invoke(element, textElement);
            return element;
        }
    }
}