using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FrbUi.SelectableGroupings
{
    public class GridSelectableGroup : ISelectableControlGroup
    {
        //private readonly List<List<ISelectable>> _items; 

        public bool Destroyed { get; private set; }
        public bool LoopFocus { get; set; }

        public GridSelectableGroup()
        {
            //_items = new List<SelectableItem>();
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
            //if (_items.Count > 0)
            //{
            //    if (rowIndex < _items.Count && columnIndex < _items[0].Count)
            //    {
            //        ]
            //    }
            //}

            //foreach (var location in _items)
            //{
            //    if (location.Row == rowIndex && location.Column == columnIndex)
            //        throw new InvalidOperationException(
            //            string.Format("An item already exists at row {0} column {1} in the GridSelectableGroup", rowIndex, columnIndex));

            //    if (location.Item == selectable)
            //        throw new InvalidOperationException("Selectable item already exists in the GridSelectableGroup");
            //}

            // If we got here, the specified spot in the grid is free

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
            throw new NotImplementedException();
        }

        public bool Remove(ISelectable selectable)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            Destroyed = true;
            //_items.Clear();
        }
    }
}
