using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class ImageExtensions {
        /// Sets a texture as the image.
        [NotNull] public static T WithImage<T>([NotNull] this T element, [CanBeNull] Texture texture)
            where T : Image {
            element.image = texture;
            return element;
        }
    }
}