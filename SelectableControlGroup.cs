using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FrbUi
{
    public class SelectableControlGroup : Collection<ISelectable>
    {
        protected ISelectable _focusedItem;
        protected bool _destroyed;

        public bool Destroyed { get { return _destroyed; } }

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
            if (_focusedItem == null)
                return;

            _focusedItem.Click();
        }

        public void Destroy()
        {
            _destroyed = true;
            Clear();
        }

        protected override void InsertItem(int index, ISelectable item)
        {
            if (_destroyed)
                return;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];
            base.RemoveItem(index);

            if (removedItem == _focusedItem)
                FocusNextControl();
        }

        protected void FocusSequentialControl(bool focusNext)
        {
            var nextFocusableItem = (ISelectable)null;

            if (Count > 0)
            {
                // Figure out what index to start at
                //   If we have a currently focused item start from there
                //   Otherwise start at the beginning
                int startIndex = 0;
                if (_focusedItem != null)
                {
                    startIndex = this.IndexOf(_focusedItem);
                    startIndex = (focusNext ? startIndex++ : startIndex--);

                    if (startIndex >= Count)
                        startIndex = 0;

                    if (startIndex < 0)
                        startIndex = (Count - 1);
                }

                int curIndex = startIndex;
                do
                {
                    nextFocusableItem = this[curIndex];
                    if (nextFocusableItem != null)
                    {
                        // If the control is disableable, only focus on it if it's enabled
                        var disableable = nextFocusableItem as IDisableable;
                        if (disableable == null | disableable.Enabled)
                            break;

                        // The control is disabled so ignore it
                        nextFocusableItem = null;
                    }

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
                } while (curIndex != startIndex);
            }

            // Unfocus the previous item and focus the next one
            if (_focusedItem != null)
                _focusedItem.LoseFocus();

            _focusedItem = nextFocusableItem;
            if (_focusedItem != null)
                _focusedItem.Focus();
        }
    }
}
