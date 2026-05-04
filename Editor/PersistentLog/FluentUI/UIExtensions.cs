using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static partial class UIExtensions {


        /// Creates a VisualElement
        [NotNull] public static VisualElement VisualElement() => new();


        /// Creates an Image
        [NotNull] public static Image Image() => new();


        /// Creates a Label
        [NotNull] public static Label Label() => new();


        /// Creates a ScrollView
        [NotNull] public static ScrollView ScrollView() => new();


        /// Creates a Button
        [NotNull] public static Button Button(Background iconImage, Action clickEvent = null) => 
            new(iconImage, clickEvent);

        /// Creates a Button
        [NotNull] public static Button Button(Action clickEvent) => new(clickEvent);


        /// Creates a TextField
        [NotNull] public static TextField TextField() => new();


        /// Creates a Toolbar
        [NotNull] public static Toolbar Toolbar() => new();


        /// Creates a ToolbarToggle
        [NotNull] public static ToolbarToggle ToolbarToggle() => new();


        /// Creates a ToolbarSearchField
        [NotNull] public static ToolbarSearchField ToolbarSearchField() => new();


        /// Creates a TwoPaneSplitView
        [NotNull] public static TwoPaneSplitView TwoPaneSplitView(
            int fixedPaneIndex, float fixedPaneStartDimension, TwoPaneSplitViewOrientation orientation)
                => new(fixedPaneIndex, fixedPaneStartDimension, orientation);


        /// Creates a ListView
        [NotNull] public static ListView ListView(
            IList itemsSource,
            float itemHeight = -1f,
            Func<VisualElement> makeItem = null,
            Action<VisualElement, int> bindItem = null)
                => new(itemsSource, itemHeight, makeItem, bindItem);
    }
}