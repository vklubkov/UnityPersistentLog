using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace PersistentLog {
    public static class VisualElementExtensions {
        [NotNull] public static T AddTo<T, TParent>([NotNull] this T element, [NotNull] TParent parent)
                where T : VisualElement
                where TParent : VisualElement {
            parent.Add(element);
            return element;
        }

        [NotNull] public static T WithCondition<T>(
            [NotNull] this T element,
            bool condition,
            [CanBeNull] Action<T> callback)
                where T : VisualElement {
            if (!condition)
                return element;

            callback?.Invoke(element);
            return element;
        }

        [NotNull] public static T WithQ<T, TQuery>([NotNull] this T element, [CanBeNull] Action<T, TQuery> callback)
                where T : VisualElement
                where TQuery : VisualElement {
            var found = element.Q<TQuery>();
            if (found == null)
                return element;

            callback?.Invoke(element, found);
            return element;
        }

        [NotNull] public static T WithQ<T, TQuery, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [CanBeNull] Action<T, TQuery, TData> callback)
                where T : VisualElement
                where TQuery : VisualElement {
            var found = element.Q<TQuery>();
            if (found == null)
                return element;

            callback?.Invoke(element, found, data);
            return element;
        }

        [NotNull] public static T WithQ<T>(
            [NotNull] this T element,
            [CanBeNull] string name,
            [CanBeNull] Action<VisualElement> callback)
                where T : VisualElement {
            var found = element.Q(name);
            if (found == null)
                return element;

            callback?.Invoke(found);
            return element;
        }

        [NotNull] public static T WithChild<T, TChild>([NotNull] this T element, [CanBeNull] Func<TChild> callback)
            where T : VisualElement
            where TChild : VisualElement {
            var child = callback?.Invoke();
            if (child == null)
                return element;

            element.Add(child);
            return element;
        }

        [NotNull] public static T WithChild<T, TChild, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [CanBeNull] Func<TData, TChild> callback)
            where T : VisualElement
            where TChild : VisualElement {
            var child = callback?.Invoke(data);
            if (child == null)
                return element;

            element.Add(child);
            return element;
        }

        [NotNull] public static T WithChild<T, TChild, TData>(
            [NotNull] this T element,
            [CanBeNull] TData data,
            [CanBeNull] Func<T, TData, TChild> callback)
                where T : VisualElement
                where TChild : VisualElement {
            var child = callback?.Invoke(element, data);
            if (child == null)
                return element;

            element.Add(child);
            return element;
        }

        [NotNull] public static T WithChild<T, TChild>(
            [NotNull] this T element,
            [CanBeNull] out TChild result,
            [CanBeNull] Func<TChild> callback)
                where T : VisualElement
                where TChild : VisualElement {
            result = null;
            var child = callback?.Invoke();
            if (child == null)
                return element;

            element.Add(child);
            result = child;
            return element;
        }

        [NotNull] public static T WithViewDataKey<T>([NotNull] this T element, [CanBeNull] string viewDataKey)
                where T : VisualElement {
            element.viewDataKey = viewDataKey;
            return element;
        }

        [NotNull] public static T WithFlexDirection<T>([NotNull] this T element, FlexDirection flexDirection)
                where T : VisualElement {
            element.style.flexDirection = flexDirection;
            return element;
        }

        [NotNull] public static T WithFlexGrow<T>([NotNull] this T element, StyleFloat flexGrow)
                where T : VisualElement {
            element.style.flexGrow = flexGrow;
            return element;
        }

        [NotNull] public static T WithFlexShrink<T>([NotNull] this T element, StyleFloat flexShrink)
                where T : VisualElement {
            element.style.flexShrink = flexShrink;
            return element;
        }

        [NotNull] public static T WithAlignItems<T>([NotNull] this T element, StyleEnum<Align> alignItems)
                where T : VisualElement {
            element.style.alignItems = alignItems;
            return element;
        }

        [NotNull] public static T WithTextAlign<T>([NotNull] this T element, StyleEnum<TextAnchor> textAlign)
                where T : VisualElement {
            element.style.unityTextAlign = textAlign;
            return element;
        }

        [NotNull] public static T WithWhiteSpace<T>([NotNull] this T element, StyleEnum<WhiteSpace> whiteSpace)
                where T : VisualElement {
            element.style.whiteSpace = whiteSpace;
            return element;
        }

        [NotNull] public static T WithPaddingLeftRight<T>([NotNull] this T element, StyleLength padding)
                where T : VisualElement {
            element.style.paddingLeft = padding;
            element.style.paddingRight = padding;
            return element;
        }

        [NotNull] public static T WithPadding<T>([NotNull] this T element, StyleLength padding)
                where T : VisualElement {
            element.style.paddingLeft = padding;
            element.style.paddingRight = padding;
            element.style.paddingTop = padding;
            element.style.paddingBottom = padding;
            return element;
        }

        [NotNull] public static T WithMarginLeft<T>([NotNull] this T element, StyleLength margin)
                where T : VisualElement {
            element.style.marginLeft = margin;
            return element;
        }

        [NotNull] public static T WithMargin<T>(
            [NotNull] this T element,
            StyleLength leftRight,
            StyleLength topBottom)
                where T : VisualElement {
            element.style.marginLeft = leftRight;
            element.style.marginRight = leftRight;
            element.style.marginTop = topBottom;
            element.style.marginBottom = topBottom;
            return element;
        }

        [NotNull] public static T WithSize<T>([NotNull] this T element, StyleLength size)
                where T : VisualElement {
            element.style.width = size;
            element.style.height = size;
            return element;
        }

        [NotNull] public static T WithMinHeight<T>([NotNull] this T element, StyleLength height)
                where T : VisualElement {
            element.style.minHeight = height;
            return element;
        }

        [NotNull] public static T AsEnabled<T>([NotNull] this T element, bool value)
                where T : VisualElement {
            element.SetEnabled(value);
            return element;
        }

        [NotNull] public static T WithBackgroundColor<T>([NotNull] this T element, StyleColor color)
                where T : VisualElement {
            element.style.backgroundColor = color;
            return element;
        }

        [NotNull] public static T WithBorderTopWidth<T>([NotNull] this T element, StyleFloat width)
                where T : VisualElement {
            element.style.borderTopWidth = width;
            return element;
        }

        [NotNull] public static T WithBorderWidth<T>([NotNull] this T element, StyleFloat width)
                where T : VisualElement {
            element.style.borderLeftWidth = width;
            element.style.borderRightWidth = width;
            element.style.borderTopWidth = width;
            element.style.borderBottomWidth = width;
            return element;
        }

        [NotNull] public static T WithManipulator<T>(
            [NotNull] this T element,
            [CanBeNull] Func<T, IManipulator> createManipulator)
                where T : VisualElement {
            var manipulator = createManipulator?.Invoke(element);
            if (manipulator == null)
                return element;

            element.AddManipulator(manipulator);
            return element;
        }
    }
}