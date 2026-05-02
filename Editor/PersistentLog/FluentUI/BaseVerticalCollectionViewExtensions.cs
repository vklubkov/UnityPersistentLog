using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static class BaseVerticalCollectionViewExtensions {
        [NotNull] public static T WithClearSelection<T>([NotNull] this T element)
                where T : BaseVerticalCollectionView {
            element.ClearSelection();
            return element;
        }

        [NotNull] public static T WithSelectionType<T>([NotNull] this T element, SelectionType selectionType)
                where T : BaseVerticalCollectionView {
            element.selectionType = selectionType;
            return element;
        }

        [NotNull] public static T WithVirtualizationMethod<T>(
            [NotNull] this T element,
            CollectionVirtualizationMethod virtualizationMethod)
                where T : BaseVerticalCollectionView {
            element.virtualizationMethod = virtualizationMethod;
            return element;
        }

        [NotNull] public static T WithRebuild<T>([NotNull] this T element)
                where T : BaseVerticalCollectionView {
            element.Rebuild();
            return element;
        }

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