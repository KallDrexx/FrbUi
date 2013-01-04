using System;

namespace FrbUi
{
    public enum SelectableState { NotSelected, Focused, Pushed }

    public interface ISelectable
    {
        LayoutableEvent OnFocused { get; set; }
        LayoutableEvent OnFocusLost { get; set; }
        LayoutableEvent OnPushed { get; set; }
        LayoutableEvent OnPushReleased { get; set; }
        LayoutableEvent OnClicked { get; set; }

        SelectableState CurrentSelectableState { get; set; }
        string CurrentAnimationChainName { get; set; }

        string StandardAnimationChainName { get; set; }
        string FocusedAnimationChainName { get; set; }
        string PushedAnimationChainName { get; set; }

        bool PushedWithFocus { get; set; }
    }

    public static class SelectableExtensions
    {
        public static void Focus(this ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException("selectable");

            // If the control can be disabled, make sure it's enabled
            var disableable = selectable as IDisableable;
            if (disableable != null && !disableable.Enabled)
                return;

            // The control can only be focused if it's in an active state
            if (selectable.CurrentSelectableState != SelectableState.NotSelected)
                return;

            selectable.CurrentSelectableState = SelectableState.Focused;
            if (selectable.FocusedAnimationChainName != null)
                selectable.CurrentAnimationChainName = selectable.FocusedAnimationChainName;

            if (selectable.OnFocused != null)
                selectable.OnFocused(selectable as ILayoutable);
        }

        public static void LoseFocus(this ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException("selectable");

            // If the control can be disabled, make sure it's enabled
            var disableable = selectable as IDisableable;
            if (disableable != null && !disableable.Enabled)
                return;

            // Focus can only be lost if it's focused
            if (selectable.CurrentSelectableState != SelectableState.Focused)
                return;

            selectable.CurrentSelectableState = SelectableState.NotSelected;
            if (selectable.StandardAnimationChainName != null)
                selectable.CurrentAnimationChainName = selectable.StandardAnimationChainName;

            if (selectable.OnFocusLost != null)
                selectable.OnFocusLost(selectable as ILayoutable);
        }

        public static void Push(this ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException("selectable");

            // If the control can be disabled, make sure it's enabled
            var disableable = selectable as IDisableable;
            if (disableable != null && !disableable.Enabled)
                return;

            // Push can only occur if it's focused or not selected
            if (selectable.CurrentSelectableState != SelectableState.Focused || selectable.CurrentSelectableState != SelectableState.NotSelected)
                return;

            selectable.PushedWithFocus = (selectable.CurrentSelectableState == SelectableState.Focused);
            selectable.CurrentSelectableState = SelectableState.Pushed;

            if (selectable.PushedAnimationChainName != null)
                selectable.CurrentAnimationChainName = selectable.PushedAnimationChainName;

            if (selectable.OnPushed != null)
                selectable.OnPushed(selectable as ILayoutable);
        }

        public static void ReleasePush(this ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException("selectable");

            // If the control can be disabled, make sure it's enabled
            var disableable = selectable as IDisableable;
            if (disableable != null && !disableable.Enabled)
                return;

            // Release can only occur if it's currently pushed
            if (selectable.CurrentSelectableState != SelectableState.Pushed)
                return;

            if (selectable.PushedWithFocus)
            {
                selectable.CurrentSelectableState = SelectableState.Focused;
                if (selectable.FocusedAnimationChainName != null)
                    selectable.CurrentAnimationChainName = selectable.FocusedAnimationChainName;
            }
            else
            {
                selectable.CurrentSelectableState = SelectableState.NotSelected;
                if (selectable.StandardAnimationChainName != null)
                    selectable.CurrentAnimationChainName = selectable.StandardAnimationChainName;
            }

            selectable.PushedWithFocus = false;

            if (selectable.OnPushReleased != null)
                selectable.OnPushReleased(selectable as ILayoutable);
        }

        public static void Click(this ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException("selectable");

            // If the control can be disabled, make sure it's enabled
            var disableable = selectable as IDisableable;
            if (disableable != null && !disableable.Enabled)
                return;

            // If the control hasn't been pushed yet, make sure it is
            if (selectable.CurrentSelectableState != SelectableState.Pushed)
                selectable.Push();

            // Release the push
            selectable.ReleasePush();

            // Call onclick handler
            if (selectable.OnClicked != null)
                selectable.OnClicked(selectable as ILayoutable);
        }
    }
}
