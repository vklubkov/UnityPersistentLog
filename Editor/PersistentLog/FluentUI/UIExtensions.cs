using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PersistentLog {
    internal static class UIExtensions {
        [NotNull] public static VisualElement VisualElement() => new();
        [NotNull] public static Label Label() => new();
        [NotNull] public static Image Image() => new();
        [NotNull] public static ToolbarToggle ToolbarToggle() => new();
        [NotNull] public static ToolbarSearchField ToolbarSearchField() => new();
        [NotNull] public static Toolbar Toolbar() => new();
        [NotNull] public static TextField TextField() => new();
        [NotNull] public static ScrollView ScrollView() => new();

        [NotNull] public static Button Button(Action clickEvent) => new(clickEvent);

        [NotNull] public static TwoPaneSplitView TwoPaneSplitView(
            int fixedPaneIndex,
            float fixedPaneStartDimension,
            TwoPaneSplitViewOrientation orientation)
                => new(fixedPaneIndex, fixedPaneStartDimension, orientation);

        [NotNull] public static ListView ListView(
            IList itemsSource,
            float itemHeight = -1f,
            Func<VisualElement> makeItem = null,
            Action<VisualElement, int> bindItem = null)
                => new(itemsSource, itemHeight, makeItem, bindItem);
    }
}