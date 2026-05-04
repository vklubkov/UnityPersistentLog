using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class BaseVerticalCollectionViewExtensions {
        /// Adds an `itemsChosen` callback.
        [NotNull] public static T WhenItemsChosen<T>(
            [NotNull] this T element,
            [CanBeNull] Action<IEnumerable<object>> itemsChosen)
                where T : BaseVerticalCollectionView {
            element.itemsChosen += itemsChosen;
            return element;
        }

        /// Triggers the collection rebuild.
        [NotNull] public static T WithRebuild<T>([NotNull] this T element)
                where T : BaseVerticalCollectionView {
            element.Rebuild();
            return element;
        }
    }
}