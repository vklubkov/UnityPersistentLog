using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class BaseVerticalCollectionViewExtensions {
        /// Adds a `selectionChanged` callback.
        [NotNull] public static T WhenSelectionChanged<T>(
            [NotNull] this T element,
            [CanBeNull] Action<IEnumerable<object>> selectionChanged)
                where T : BaseVerticalCollectionView {
            element.selectionChanged += selectionChanged;
            return element;
        }

        /// Triggers ClearSelection.
        [NotNull] public static T WithClearSelection<T>([NotNull] this T element)
                where T : BaseVerticalCollectionView {
            element.ClearSelection();
            return element;
        }

        /// Sets the selectionType.
        [NotNull] public static T WithSelectionType<T>([NotNull] this T element, SelectionType selectionType)
                where T : BaseVerticalCollectionView {
            element.selectionType = selectionType;
            return element;
        }
    }
}