using System.Collections.ObjectModel;

namespace FrbUi.SelectableGroupings
{
    public class SequentialSelectableGroup : Collection<ISelectable>, ISelectableControlGroup
    {
        public bool Destroyed { get; private set; }
        public bool LoopFocus { get; set; }
        public ISelectable CurrentlyFocusedItem { get; private set; }

        public void FocusNextControl()
        {
            FocusSequentialControl(true);
        }

        public void FocusPreviousControl()
        {
            FocusSequentialControl(false);
        }

        public void ClickFocusedControl()
        {
            if (CurrentlyFocusedItem == null)
                return;

            CurrentlyFocusedItem.Click();
        }

        public void UnfocusCurrentControl()
        {
            if (CurrentlyFocusedItem != null)
            {
                CurrentlyFocusedItem.LoseFocus();
                CurrentlyFocusedItem = null;
            }
        }

        public void Destroy()
        {
            Destroyed = true;
            Clear();
        }

        protected override void InsertItem(int index, ISelectable item)
        {
            if (Destroyed)
                return;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];
            base.RemoveItem(index);

            if (removedItem == CurrentlyFocusedItem)
                FocusNextControl();
        }

        private void FocusSequentialControl(bool focusNext)
        {
            var nextFocusableItem = (ISelectable)null;

            if (Count > 0)
            {
                // Figure out what index to start at
                //   If we have a currently focused item start from there
                //   Otherwise start at the beginning
                int startIndex = -1;
                if (CurrentlyFocusedItem != null)
                    startIndex = this.IndexOf(CurrentlyFocusedItem);

                // If we are on the first (or last if not focusing next) element
                //  stop iterating
                if (focusNext)
                {
                    if (startIndex >= (Count - 1) && !LoopFocus)
                        return;
                }
                else
                {
                    if (startIndex <= 0 && !LoopFocus)
                        return;
                }

                int curIndex = startIndex;
                do
                {
                    // Move onto the next sequential control
                    if (focusNext)
                    {
                        curIndex++;
                        if (curIndex >= Count)
                            curIndex = 0;
                    }
                    else
                    {
                        curIndex--;
                        if (curIndex < 0)
                            curIndex = (Count - 1);
                    }

                    nextFocusableItem = this[curIndex];
                    if (nextFocusableItem != null)
                    {
                        // If the control is disableable, only focus on it if it's enabled
                        var disableable = nextFocusableItem as IDisableable;
                        if (disableable == null || disableable.Enabled)
                            break;

                        // The control is disabled so ignore it
                        nextFocusableItem = null;
                    }
                } while (curIndex != startIndex);
            }

            // Unfocus the previous item and focus the next one
            if (CurrentlyFocusedItem != null)
                CurrentlyFocusedItem.LoseFocus();

            CurrentlyFocusedItem = nextFocusableItem;
            if (CurrentlyFocusedItem != null)
                CurrentlyFocusedItem.Focus();
        }
    }
}
