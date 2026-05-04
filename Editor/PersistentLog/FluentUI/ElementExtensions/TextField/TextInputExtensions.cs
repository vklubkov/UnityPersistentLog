using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using static UnityEngine.Debug;

namespace PersistentLog {
    public static partial class TextFieldExtensions {
        /// Runs a callback with the child TextInput as a parameter.
        [NotNull] public static T WithOnTextInput<T>(
            [NotNull] this T element,
            [CanBeNull] Action<VisualElement> callback)
                where T : TextField {
            var textInput = element.Q("unity-text-input");
            if (textInput == null) {
                LogWarning("Fluent Editor UIElements: child element with name `unity-text-input` " +
                           $"not found for element of type {element.GetType().Name}");

                return element;
            }

            callback?.Invoke(textInput);
            return element;
        }
    }
}