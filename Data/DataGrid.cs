using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrbUi.Data
{
    public class DataGrid<TData, TMetadata> 
        where TData : class 
    {
        private readonly List<List<TData>> _itemPositions;
        private readonly Dictionary<TData, GridPosition> _knownItems;
        private int _rowCount;
        private int _columnCount;

        public int ItemCount { get { return _knownItems.Count;  } }
        public int RowCount { get { return _itemPositions.Count; } }
        public int ColumnCount
        {
            get
            {
                if (_itemPositions.Count > 0)
                    return _itemPositions[0].Count;

                return 0;
            }
        }

        public TData this[int rowIndex, int columnIndex]
        {
            get
            {
                return IndexExists(rowIndex, columnIndex)
                           ? _itemPositions[rowIndex][columnIndex]
                           : null;
            }
        }

        public IEnumerable<TData> Items
        {
            get
            {
                foreach (var key in _knownItems.Keys)
                    yield return key;
            }
        }

        public DataGrid()
        {
            _itemPositions = new List<List<TData>>();
            _knownItems = new Dictionary<TData, GridPosition>();
        }

        public void Add(TData item, TMetadata metadata, int rowIndex, int columnIndex)
        {
            // Make sure row and column indexes are valid
            if (rowIndex < 0)
                throw new ArgumentException("An item can't be added to a row with an index less than zero");

            if (columnIndex < 0)
                throw new ArgumentException("An item can't be added to a column with an index less thanzero");

            // Check if the item already exists in the grid.  An item may only exist at one spot in the grid at a time
            if (_knownItems.ContainsKey(item))
                throw new InvalidOperationException("This item can't be added to the grid when it already exists elsewhere in the grid");

            // Make sure no other item exists in the specific spot
            if (IndexExists(rowIndex, columnIndex) && _itemPositions[rowIndex][columnIndex] != null)
                throw new InvalidOperationException(
                    string.Format("This item can't be added to the grid as another item already exists at row {0} column {1}", rowIndex, columnIndex));

            // If the item is already in the current bounds, simply add the item to the position
            if (!IndexExists(rowIndex, columnIndex))
            {
                // Expand the lists to accomidate for that position
                while (_itemPositions.Count <= rowIndex)
                    _itemPositions.Add(new List<TData>());

                var row = _itemPositions[rowIndex];
                while (row.Count <= columnIndex)
                    row.Add(null);
            }

            // Add this item to the known items list
            _itemPositions[rowIndex][columnIndex] = item;
            _knownItems.Add(item, new GridPosition(rowIndex, columnIndex, metadata));

            // If this item has a higher row or column index than previously, update the values
            if (_rowCount <= rowIndex)
                _rowCount = rowIndex + 1;

            if (_columnCount <= columnIndex)
                _columnCount = columnIndex + 1;
        }

        public void Remove(TData item)
        {
            GridPosition position;
            if (!_knownItems.TryGetValue(item, out position))
                throw new InvalidOperationException("An item can't be removed if it doesn't exist in the grid");

            // Remove the item from the grid areas
            _knownItems.Remove(item);
            _itemPositions[position.Row][position.Column] = null;

            // If this item was in the last position in the row, resize the list up to the next to last item
            var row = _itemPositions[position.Row];
            if (row.Count - 1 == position.Column)
            {
                for (int x = row.Count - 1; x >= 0; x--)
                {
                    if (row[x] != null)
                        break;

                    row.RemoveAt(x);
                }
            }

            CalculateMaxCounts();
        }

        public void Clear()
        {
            _knownItems.Clear();
            _itemPositions.Clear();
            _rowCount = 0;
            _columnCount = 0;
        }

        public void ItemPosition(TData item, out int rowIndex, out int columnIndex)
        {
            GridPosition position;
            if (!_knownItems.TryGetValue(item, out position))
                throw new InvalidOperationException("The datagrid does not contain the specified item");

            rowIndex = position.Row;
            columnIndex = position.Column;
        }

        public bool Contains(TData item)
        {
            return _knownItems.ContainsKey(item);
        }

        public TMetadata GetItemMetadata(TData item)
        {
            if (!_knownItems.ContainsKey(item))
                throw new InvalidOperationException("The specified item does not exist");

            return _knownItems[item].Metadata;
        }

        public void UpdateItemMetadata(TData item, TMetadata metadata)
        {
            if (!_knownItems.ContainsKey(item))
                throw new InvalidOperationException("The specified item does not exist");

            _knownItems[item].Metadata = metadata;
        }

        public TData FindClosestItem(TData referenceItem, GridSearchDirection direction, bool loopfocus = false)
        {
            int? startRow = null;
            int? startColumn = null;

            if (referenceItem != null)
            {
                GridPosition position;
                if (!_knownItems.TryGetValue(referenceItem, out position))
                    throw new InvalidOperationException("The reference item does not exist in the grid");

                startColumn = position.Column;
                startRow = position.Row;
            }

            // Search the grid for the next item
            TData foundItem = null;
            List<TData> row;

            switch (direction)
            {
                case GridSearchDirection.NextInRow:
                    foundItem = FindNextInRow(startRow, startColumn, loopfocus);
                    break;

                case GridSearchDirection.PrevInRow:
                    foundItem = FindPrevInRow(startRow, startColumn, loopfocus);
                    break;

                case GridSearchDirection.NextInColumn:
                    foundItem = FindNextInColumn(startRow, startColumn, loopfocus);
                    break;

                case GridSearchDirection.PrevInColumn:
                    foundItem = FindPrevInColumn(startRow, startColumn, loopfocus);
                    break;
            }

            return foundItem;
        }

        private TData FindPrevInColumn(int? startRow, int? startColumn, bool loopfocus)
        {
            TData foundItem = null;

            // If no start row/column was given, start at the bottom left of the grid
            if (startRow == null || startColumn == null)
            {
                startRow = _rowCount;
                startColumn = 0;
            }

            var rowIndex = (startRow == 0 && loopfocus)
                               ? _itemPositions.Count - 1
                               : startRow.Value - 1;

            while (rowIndex >= 0)
            {
                if (_itemPositions[rowIndex].Count - 1 >= startColumn)
                {
                    if (_itemPositions[rowIndex][startColumn.Value] != null)
                    {
                        foundItem = _itemPositions[rowIndex][startColumn.Value];
                        break;
                    }
                }

                rowIndex--;

                // If focus is looping, loop around if less than zero or break out if we are on the original row
                if (loopfocus)
                {
                    if (rowIndex < 0)
                        rowIndex = _itemPositions.Count - 1;

                    if (rowIndex == startRow.Value)
                        break;
                }
            }

            return foundItem;
        }

        private TData FindNextInColumn(int? startRow, int? startColumn, bool loopfocus)
        {
            TData foundItem = null;

            // If no start row/column was given, start at the top left of the grid
            if (startRow == null || startColumn == null)
            {
                startRow = -1;
                startColumn = 0;
            }

            var rowIndex = (startRow == _itemPositions.Count - 1 && loopfocus)
                               ? 0
                               : startRow.Value + 1;

            while (rowIndex < _itemPositions.Count)
            {
                if (_itemPositions[rowIndex].Count - 1 >= startColumn)
                {
                    if (_itemPositions[rowIndex][startColumn.Value] != null)
                    {
                        foundItem = _itemPositions[rowIndex][startColumn.Value];
                        break;
                    }
                }

                rowIndex++;

                // If focus is looping, loop around if less than zero or break out if we are on the original row
                if (loopfocus)
                {
                    if (rowIndex >= _itemPositions.Count)
                        rowIndex = 0;

                    if (rowIndex == startRow.Value)
                        break;
                }
            }

            return foundItem;
        }

        private TData FindPrevInRow(int? startRow, int? startColumn, bool loopfocus)
        {
            TData foundItem = null;

            // If no start row/column was given, start at the top right of the grid
            if (startRow == null || startColumn == null)
            {
                startRow = 0;
                startColumn = _columnCount;
            }

            var row = _itemPositions[startRow.Value];
            var columnIndex = (startColumn == 0 && loopfocus)
                                  ? row.Count - 1
                                  : (int)startColumn - 1;

            while (columnIndex >= 0)
            {
                if (row[columnIndex] != null)
                {
                    foundItem = row[columnIndex];
                    break;
                }

                columnIndex--;

                // If we are before the beginning and we are looping focus, loop around
                if (loopfocus)
                {
                    if (columnIndex < 0)
                        columnIndex = row.Count - 1;

                    if (columnIndex == startColumn)
                        break;
                }
            }

            return foundItem;
        }

        private TData FindNextInRow(int? startRow, int? startColumn, bool loopfocus)
        {
            TData foundItem = null;

            // If no start row/column was given, start at the top left of the grid
            if (startRow == null || startColumn == null)
            {
                startRow = 0;
                startColumn = -1;
            }

            var row = _itemPositions[startRow.Value];
            var columnIndex = (startColumn == (row.Count - 1) && loopfocus)
                                  ? 0
                                  : (int)startColumn + 1;

            while (columnIndex < row.Count)
            {
                if (row[columnIndex] != null)
                {
                    foundItem = row[columnIndex];
                    break;
                }

                columnIndex++;

                // If we are before the beginning and we are looping focus, loop around
                if (loopfocus)
                {
                    if (columnIndex > row.Count)
                        columnIndex = 0;

                    if (columnIndex == startColumn)
                        break;
                }
            }

            return foundItem;
        }

        private bool IndexExists(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || columnIndex < 0)
                return false;

            if (rowIndex >= _itemPositions.Count)
                return false;

            if (columnIndex >= _itemPositions[rowIndex].Count)
                return false;

            return true;
        }

        private void CalculateMaxCounts()
        {
            int rowcount = 0;
            int colcount = 0;

            for (int x = 0; x < _itemPositions.Count; x++)
            {
                if (_itemPositions[x].Count > colcount)
                    colcount = _itemPositions[x].Count;

                // Don't count this row for the row count if it's empty
                if (_itemPositions[x].Count != 0)
                    rowcount = x + 1;
            }

            _columnCount = colcount;
            _rowCount = rowcount;
        }

        private class GridPosition
        {
            public int Row { get; private set; }
            public int Column { get; private set; }
            public TMetadata Metadata { get; set; }

            public GridPosition(int row, int column, TMetadata metadata)
            {
                Row = row;
                Column = column;
                Metadata = metadata;
            }
        }

        public enum GridSearchDirection { NextInColumn, PrevInColumn, NextInRow, PrevInRow }
    }
}
