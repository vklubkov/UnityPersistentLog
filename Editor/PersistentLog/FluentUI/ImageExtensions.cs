using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace PersistentLog {
    internal static class ImageExtensions {
        [NotNull] public static T WithImage<T>([NotNull] this T element, [CanBeNull] Texture image)
                where T : Image {
            element.image = image;
            return element;
        }
    }
}