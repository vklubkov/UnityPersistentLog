using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class BaseListViewExtensions {
        /// Sets the makeNoneElement callback.
        [NotNull] public static T WithMakeNoneElement<T>(
            [NotNull] this T element,
            [CanBeNull] Func<VisualElement> makeNoneElement)
                where T : BaseListView {
            element.makeNoneElement = makeNoneElement;
            return element;
        }
    }
}