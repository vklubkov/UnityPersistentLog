using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    internal static class BaseListViewExtensions {
        [NotNull] public static T WithMakeNoneElement<T>(
            [NotNull] this T element,
            [CanBeNull] Func<VisualElement> makeNoneElement)
                where T : BaseListView {
            element.makeNoneElement = makeNoneElement;
            return element;
        }
    }
}