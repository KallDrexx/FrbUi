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
    }
}
