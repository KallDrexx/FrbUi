using System;
using FrbUi.Data;

namespace FrbUi.SelectableGroupings
{
    public class GridSelectableGroup : ISelectableControlGroup
    {
        private readonly DataGrid<ISelectable, object> _items;
        private ISelectable _focusedItem;

        public bool Destroyed { get; private set; }
        public bool LoopFocus { get; set; }

        public GridSelectableGroup()
        {
            _items = new DataGrid<ISelectable, object>();
        }

        public void AddItem(ISelectable selectable, int rowIndex, int columnIndex)
        {
            if (Destroyed)
                throw new InvalidOperationException("Cannot add a control to a destroyed group");

            if (selectable == null)
                throw new ArgumentNullException("selectable");

            if (rowIndex < 0)
                throw new ArgumentException("Cannot add an item to a row index less than 0");

            if (columnIndex < 0)
                throw new ArgumentException("Cannot add an item to a column index less than 0");

            // Make sure this item doesn't already exist or an item doesn't exist in this row/column
            if (_items.Contains(selectable))
                throw new InvalidOperationException("Selectable item already exists in the GridSelectableGroup");

            // Make sure the row/column index is available
            if (_items[rowIndex, columnIndex] != null)
                throw new InvalidOperationException(
                    string.Format("An item already exists at row {0} column {1} in the GridSelectableGroup", rowIndex, columnIndex));

            // If we got here, the specified spot in the grid is free
            _items.Add(selectable, null, rowIndex, columnIndex);
        }

        public void FocusNextControlInRow()
        {
            var direction = DataGrid<ISelectable, object>.GridSearchDirection.NextInRow;
            FindAndFocusControl(direction);
        }

        public void FocusPreviousControlInRow()
        {
            var direction = DataGrid<ISelectable, object>.GridSearchDirection.PrevInRow;
            FindAndFocusControl(direction);
        }

        public void FocusNextControlInColumn()
        {
            var direction = DataGrid<ISelectable, object>.GridSearchDirection.NextInColumn;
            FindAndFocusControl(direction);
        }

        public void FocusPreviousControlInColumn()
        {
            var direction = DataGrid<ISelectable, object>.GridSearchDirection.PrevInColumn;
            FindAndFocusControl(direction);
        }

        public void ClickFocusedControl()
        {
            throw new NotImplementedException();
        }

        public void UnfocusCurrentControl()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ISelectable selectable)
        {
            return _items.Contains(selectable);
        }

        public bool Remove(ISelectable selectable)
        {
            if (_items.Contains(selectable))
                _items.Remove(selectable);

            return true;
        }

        public void Destroy()
        {
            Destroyed = true;
            _items.Clear();
        }

        private void FocusItem(ISelectable item)
        {
            if (item == null)
                return;

            if (_focusedItem != null)
                _focusedItem.LoseFocus();

            _focusedItem = item;
            _focusedItem.Focus();
        }

        private void FindAndFocusControl(DataGrid<ISelectable, object>.GridSearchDirection direction)
        {
            var currentItem = _focusedItem;
            ISelectable nextItem;

            // Keep getting the next item until no item is returned (no item in the direction)
            //   or a non-disabled item is returne
            while (true)
            {
                nextItem = _items.FindClosestItem(currentItem, direction);

                // If the found item is disabled, go to the next
                var disablable = nextItem as IDisableable;
                if (disablable != null)
                {
                    if (!disablable.Enabled)
                    {
                        currentItem = nextItem;
                        continue;
                    }
                }

                // If we got here we got an acceptable result
                break;
            }

            FocusItem(nextItem);
        }
    }
}
