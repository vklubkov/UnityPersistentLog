using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class BaseVerticalCollectionViewExtensions {
        /// Scrolls to the last item in the collection
        [NotNull] public static T WithScrollToLastItem<T>([NotNull] this T element)
                where T : BaseVerticalCollectionView {
            if (element.itemsSource == null)
                return element;

            var lastIndex = element.itemsSource.Count - 1;
            if (lastIndex < 0)
                return element;

            element.ScrollToItem(lastIndex);
            return element;
        }
    }
}