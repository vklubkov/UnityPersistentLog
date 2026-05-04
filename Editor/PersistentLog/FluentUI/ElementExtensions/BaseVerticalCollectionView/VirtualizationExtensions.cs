using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class BaseVerticalCollectionViewExtensions {
        /// Sets the virtualization method.
        [NotNull] public static T WithVirtualizationMethod<T>(
            [NotNull] this T element,
            CollectionVirtualizationMethod virtualizationMethod)
                where T : BaseVerticalCollectionView {
            element.virtualizationMethod = virtualizationMethod;
            return element;
        }
    }
}