using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static class ListViewExtensions {
        [NotNull] public static T WithMakeItem<T>(
            [NotNull] this T element,
            [CanBeNull] Func<VisualElement> makeItem)
                where T : ListView {
            element.makeItem = makeItem;
            return element;
        }

        [NotNull] public static T WithBindItem<T>(
            [NotNull] this T element,
            [CanBeNull] Action<VisualElement, int> bindItem)
                where T : ListView {
            element.bindItem = bindItem;
            return element;
        }
    }
}