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
            var nextItem = _items.FindClosestItem(_focusedItem,
                                                  DataGrid<ISelectable, object>.GridSearchDirection.NextInRow);

            FocusItem(nextItem);
        }

        public void FocusPreviousControlInRow()
        {
            var nextItem = _items.FindClosestItem(_focusedItem,
                                                  DataGrid<ISelectable, object>.GridSearchDirection.PrevInRow);

            FocusItem(nextItem);
        }

        public void FocusNextControlInColumn()
        {
            var nextItem = _items.FindClosestItem(_focusedItem,
                                                  DataGrid<ISelectable, object>.GridSearchDirection.NextInColumn);

            FocusItem(nextItem);
        }

        public void FocusPreviousControlInColumn()
        {
            var nextItem = _items.FindClosestItem(_focusedItem,
                                                  DataGrid<ISelectable, object>.GridSearchDirection.PrevInColumn);

            FocusItem(nextItem);
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
    }
}
