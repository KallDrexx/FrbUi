using System;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.ManagedSpriteGroups;
using FlatRedBall.Math.Geometry;
using FrbUi.Data;

namespace FrbUi.Layouts
{
    public class GridLayout : ILayoutManager, ISelectable
    {
        #region Fields

        private readonly SpriteFrame _backgroundSprite;
        private readonly DataGrid<ILayoutable, GridAlignment> _items;
        private Layer _layer;
        private bool _recalculateLayout;
        private float _margin;
        private float _spacing;
        private float _alpha;
        private AxisAlignedRectangle _border;
        private string _standardAnimationChainName;
        private string _focusedAnimationChainName;
        private string _pushedAnimationChainName;

        #endregion

        #region Properties

        public LayoutableEvent OnFocused { get; set; }
        public LayoutableEvent OnFocusLost { get; set; }
        public LayoutableEvent OnPushed { get; set; }
        public LayoutableEvent OnPushReleased { get; set; }
        public LayoutableEvent OnClicked { get; set; }
        public SelectableState CurrentSelectableState { get; set; }        
        public LayoutableEvent OnSizeChangeHandler { get; set; }

        public bool PushedWithFocus { get; set; }
        public string CurrentAnimationChainName { get; set; }
        public IEnumerable<ILayoutable> Items { get { return _items.Items; } }

        public int ColumnCount { get; protected set; }
        public int RowCount { get; protected set; }
        public bool KeepBackgroundAlphaSynced { get; set; }
        public ILayoutable ParentLayout { get; set; }

        public float BackgroundAlpha { get { return _backgroundSprite.Alpha; } set { _backgroundSprite.Alpha = value; } }
        public AnimationChainList BackgroundAnimationChains { get { return _backgroundSprite.AnimationChains; } set { _backgroundSprite.AnimationChains = value; } }
        public string CurrentBackgroundAnimationChainName { get { return _backgroundSprite.CurrentChainName; } set { _backgroundSprite.CurrentChainName = value; } }
        public float RelativeX { get { return _backgroundSprite.RelativeX; } set { _backgroundSprite.RelativeX = value; } }
        public float RelativeY { get { return _backgroundSprite.RelativeY; } set { _backgroundSprite.RelativeY = value; } }
        public float RelativeZ { get { return _backgroundSprite.RelativeZ; } set { _backgroundSprite.RelativeZ = value; } }
        public bool AbsoluteVisible { get { return _backgroundSprite.AbsoluteVisible; } }
        public bool IgnoresParentVisibility { get { return _backgroundSprite.IgnoresParentVisibility; } set { _backgroundSprite.IgnoresParentVisibility = value; } }
        public bool Visible { get { return _backgroundSprite.Visible; } set { _backgroundSprite.Visible = value; } }
        public float ScaleXVelocity { get { return _backgroundSprite.ScaleXVelocity; } set { _backgroundSprite.ScaleXVelocity = value; } }
        public float ScaleYVelocity { get { return _backgroundSprite.ScaleYVelocity; } set { _backgroundSprite.ScaleYVelocity = value; } }
        public float X { get { return _backgroundSprite.X; } set { _backgroundSprite.X = value; } }
        public float Y { get { return _backgroundSprite.Y; } set { _backgroundSprite.Y = value; } }
        public float Z { get { return _backgroundSprite.Z; } set { _backgroundSprite.Z = value; } }
        public float XAcceleration { get { return _backgroundSprite.XAcceleration; } set { _backgroundSprite.XAcceleration = value; } }
        public float YAcceleration { get { return _backgroundSprite.YAcceleration; } set { _backgroundSprite.YAcceleration = value; } }
        public float ZAcceleration { get { return _backgroundSprite.ZAcceleration; } set { _backgroundSprite.ZAcceleration = value; } }
        public float XVelocity { get { return _backgroundSprite.XVelocity; } set { _backgroundSprite.XVelocity = value; } }
        public float YVelocity { get { return _backgroundSprite.YVelocity; } set { _backgroundSprite.YVelocity = value; } }
        public float ZVelocity { get { return _backgroundSprite.ZVelocity; } set { _backgroundSprite.ZVelocity = value; } }
        public IVisible Parent { get { return _backgroundSprite.Parent as IVisible; } }

        public Layer Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;
                SpriteManager.AddToLayer(_backgroundSprite, value);
            }
        }

        public float ScaleX
        {
            get { return _backgroundSprite.ScaleX; }
            set
            {
                _backgroundSprite.ScaleX = value;
                if (OnSizeChangeHandler != null)
                    OnSizeChangeHandler(this);

                if (_border != null)
                    _border.ScaleX = value;
            }
        }

        public float ScaleY
        {
            get { return _backgroundSprite.ScaleY; }
            set
            {
                _backgroundSprite.ScaleY = value;
                if (OnSizeChangeHandler != null)
                    OnSizeChangeHandler(this);

                if (_border != null)
                    _border.ScaleY = value;
            }
        }

        public bool ShowBorder
        {
            get { return _border != null; }
            set
            {
                if (_border == null && value)
                {
                    _border = ShapeManager.AddAxisAlignedRectangle();
                    _border.AttachTo(_backgroundSprite, false);
                    _border.ScaleX = ScaleX;
                    _border.ScaleY = ScaleY;
                }
                else if (_border != null && !value)
                {
                    ShapeManager.Remove(_border);
                    _border = null;
                }
            }
        }

        public float Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                _recalculateLayout = true;
            }
        }

        public float Spacing
        {
            get { return _spacing; }
            set
            {
                _spacing = value;
                _recalculateLayout = true;
            }
        }

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;

                if (KeepBackgroundAlphaSynced)
                    BackgroundAlpha = value;

                // Update the alpha values of all child objects
                foreach (var item in _items.Items)
                    item.Alpha = value;
            }
        }

        public string StandardAnimationChainName
        {
            get { return _standardAnimationChainName; }
            set
            {
                if (!_backgroundSprite.ContainsChainName(value))
                    throw new InvalidOperationException("The animation chain list does not contain a chain with the name of " + value);

                _standardAnimationChainName = value;

                // If we should be displaying this animation chain, activate it
                if (CurrentSelectableState == SelectableState.NotSelected)
                    _backgroundSprite.CurrentChainName = value;
            }
        }

        public string FocusedAnimationChainName
        {
            get { return _focusedAnimationChainName; }
            set
            {
                if (!_backgroundSprite.ContainsChainName(value))
                    throw new InvalidOperationException("The animation chain list does not contain a chain with the name of " + value);

                _focusedAnimationChainName = value;

                // If we should be displaying this animation chain, activate it
                if (CurrentSelectableState == SelectableState.Focused)
                    _backgroundSprite.CurrentChainName = value;
            }
        }

        public string PushedAnimationChainName
        {
            get { return _pushedAnimationChainName; }
            set
            {
                if (!_backgroundSprite.ContainsChainName(value))
                    throw new InvalidOperationException("The animation chain list does not contain a chain with the name of " + value);

                _pushedAnimationChainName = value;

                // If we should be displaying this animation chain, activate it
                if (CurrentSelectableState == SelectableState.Pushed)
                    _backgroundSprite.CurrentChainName = value;
            }
        }

        #endregion

        #region Methods

        public GridLayout()
        {
            _items = new DataGrid<ILayoutable, GridAlignment>();
            _backgroundSprite = new SpriteFrame();
            _backgroundSprite.PixelSize = 0.5f;
            _backgroundSprite.Alpha = 0f;
        }

        public void Activity()
        {
            _alpha = 1f;
        }

        public void AttachTo(PositionedObject obj, bool changeRelative)
        {
            _backgroundSprite.AttachTo(obj, changeRelative);
        }

        public void Detach()
        {
            _backgroundSprite.Detach();
        }

        public void AddToManagers(Layer layer)
        {
            SpriteManager.AddSpriteFrame(_backgroundSprite);
            Layer = layer;
        }

        public void AddItem(ILayoutable item, int rowIndex, int columnIndex, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
        {
            if (columnIndex < 0)
                throw new InvalidOperationException("Item cannot be placed in a column index less than 0");

            if (rowIndex < 0)
                throw new InvalidOperationException("Item cannot be placed in a row index less than 0");

            // Ignore null items
            if (item == null)
                return;

            // If this item is already tracked, remove it from the current position 
            //   and place the item in the new position
            if (_items.Contains(item))
                _items.Remove(item);

            // Add the item to the grid collection
            if (_items[rowIndex, columnIndex] != null)
                throw new InvalidOperationException(
                    string.Format("An item already exists for this grid at row {0} column {1}", rowIndex, columnIndex));

            var alignment = new GridAlignment
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment
            };

            _items.Add(item, alignment, rowIndex, columnIndex);

            // Attach the item to the background sprite
            item.AttachTo(_backgroundSprite, false);
            item.RelativeZ = 0.1f;
            item.Alpha = _alpha;
            item.ParentLayout = this;
            _recalculateLayout = true;

            item.OnSizeChangeHandler = sender => _recalculateLayout = true;
        }

        public void RemoveItem(ILayoutable item)
        {
            if (!_items.Contains(item))
                return;

            _items.Remove(item);

            // Only detach it if the item is still attached to this
            if (item.Parent == _backgroundSprite)
            {
                item.Detach();
                item.OnSizeChangeHandler = null;
            }
        }

        public void UpdateDependencies(double currentTime)
        {
            // Update the dependencies on all children
            foreach (var layoutable in Items )
                layoutable.UpdateDependencies(currentTime);

            PerformLayout();
            _backgroundSprite.UpdateDependencies(currentTime);
        }

        public void ForceUpdateDependencies()
        {
            // Force all dependencies to be updated for all children
            foreach (var layoutable in Items)
                layoutable.ForceUpdateDependencies();

            PerformLayout();
            _backgroundSprite.ForceUpdateDependencies();
        }

        public void Destroy()
        {
            _backgroundSprite.Detach();
            SpriteManager.RemoveSpriteFrame(_backgroundSprite);

            // Remove all the items from the datagrid
            var items = _items.Items.ToArray();
            for (int x = items.Length - 1; x >= 0; x--)
            {
                var item = items[x];

                if (item.Parent == _backgroundSprite)
                    item.Destroy();
            }

            _items.Clear();
        }

        protected virtual void PerformLayout()
        {
            // Remove any items that this is no longer the parent of
            var allItems = _items.Items.ToArray();
            foreach (var gridItem in allItems)
            {
                if (gridItem.Parent != _backgroundSprite)
                {
                    _recalculateLayout = true;
                    _items.Remove(gridItem);
                }
            }

            // If we aren't flagged to redo the layout, don't bother
            if (!_recalculateLayout)
                return; 

            // Reset the flag so we don't reset the layout again until something changes
            _recalculateLayout = false;

            // First recalculate the scaleX and scaleY so we know the offset positions
            Dictionary<int, float> columnMaxWidths;
            Dictionary<int, float> rowMaxHeights;
            RecalculateScales(out columnMaxWidths, out rowMaxHeights);

            // Now lay out all the items
            ExecuteLayout(columnMaxWidths, rowMaxHeights);
        }

        protected virtual void RecalculateScales(out Dictionary<int, float> columnMaxWidths, out Dictionary<int, float> rowMaxHeights)
        {
            RowCount = 0;
            ColumnCount = 0;

            columnMaxWidths = new Dictionary<int, float>();
            rowMaxHeights = new Dictionary<int, float>();

            for (int row = 0; row < _items.RowCount; row++)
            {
                for (int col = 0; col < _items.ColumnCount; col++)
                {
                    var item = _items[row, col];
                    if (item == null)
                        continue;

                    var itemWidth = item.ScaleX * 2;
                    var itemHeight = item.ScaleY * 2;

                    // Check if this is the widest item in the column so far
                    if (!columnMaxWidths.ContainsKey(col))
                        columnMaxWidths.Add(col, itemWidth);
                    else if (columnMaxWidths[col] < itemWidth)
                        columnMaxWidths[col] = itemWidth;

                    // Check if this is the tallist item in the row so far
                    if (!rowMaxHeights.ContainsKey(row))
                        rowMaxHeights.Add(row, itemHeight);
                    else if (rowMaxHeights[row] < itemHeight)
                        rowMaxHeights[row] = itemHeight;

                    // update max indices
                    if (ColumnCount < col)
                        ColumnCount = col;

                    if (RowCount < row)
                        RowCount = row;
                }
            }

            // Change the row/column counts from indices to conts
            RowCount++;
            ColumnCount++;

            // If no rows or columns, assume it has no height or width
            if (RowCount == 0 || ColumnCount == 0)
            {
                ScaleX = 0;
                ScaleY = 0;
                return;
            }

            // Calculate the total width
            float width = (Margin * 2);
            for (int x = 0; x < ColumnCount; x++)
            {
                if (x > 0)
                    width += Spacing;

                if (columnMaxWidths.ContainsKey(x))
                    width += columnMaxWidths[x];
            }

            // Calculate the total height
            float height = (Margin * 2);
            for (int x = 0; x < RowCount; x++)
            {
                if (x > 0)
                    height += Spacing;

                if (rowMaxHeights.ContainsKey(x))
                    height += rowMaxHeights[x];
            }

            // Set the scales
            ScaleX = (width / 2);
            ScaleY = (height / 2);
        }

        protected virtual void ExecuteLayout(Dictionary<int, float> columnMaxWidths, Dictionary<int, float> rowMaxHeights)
        {
            // Loop through all the items and place them
            for (int row = 0; row < _items.RowCount; row++)
            {
                for (int col = 0; col < _items.ColumnCount; col++)
                {
                    var gridItem = _items[row, col];
                    if (gridItem == null)
                        continue;

                    // If we have an item in this cell, there must be a max width or height defined
                    if (!columnMaxWidths.ContainsKey(col))
                        throw new InvalidOperationException(
                            string.Format("Grid item exists at {0} but no max width found", col));

                    if (!rowMaxHeights.ContainsKey(row))
                        throw new InvalidOperationException(
                            string.Format("Grid item exists at {0} but no max height found", row));

                    // Offsets begin at the top left
                    float xOffset = (0 - ScaleX + Margin);
                    float yOffset = (ScaleY - Margin);

                    // Calculate the x offset based on the max widths up to this point
                    //  increase x offset as items head to the right
                    for (int i = 0; i < col; i++)
                    {
                        if (columnMaxWidths.ContainsKey(i))
                            xOffset += columnMaxWidths[i];

                        if (i < (ColumnCount - 1))
                            xOffset += Spacing;
                    }

                    // Calculate the y offset based on the max heights
                    //  Decrease the y offset as items head downward
                    for (int i = 0; i < row; i++)
                    {
                        if (rowMaxHeights.ContainsKey(i))
                            yOffset -= rowMaxHeights[i];

                        if (i < (RowCount - 1))
                            yOffset -= Spacing;
                    }

                    // Figure out the alignment offsets
                    var alignment = _items.GetItemMetadata(gridItem);
                    float alignmentOffsetX;
                    float alignmentOffsetY;

                    switch (alignment.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Center:
                            alignmentOffsetX = (columnMaxWidths[col] / 2) - gridItem.ScaleX;
                            break;

                        case HorizontalAlignment.Right:
                            alignmentOffsetX = (columnMaxWidths[col] - gridItem.ScaleX);
                            break;

                        case HorizontalAlignment.Left:
                        default:
                            alignmentOffsetX = 0;
                            break;
                    }

                    switch (alignment.VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            alignmentOffsetY = (rowMaxHeights[row] / 2) - gridItem.ScaleY;
                            break;

                        case VerticalAlignment.Bottom:
                            alignmentOffsetY = (rowMaxHeights[row] + gridItem.ScaleY);
                            break;

                        case VerticalAlignment.Top:
                        default:
                            alignmentOffsetY = 0;
                            break;
                    }

                    // Position the offsets to account for the center of the object
                    xOffset += gridItem.ScaleX + alignmentOffsetX;
                    yOffset -= gridItem.ScaleY + alignmentOffsetY;

                    // Place the item
                    gridItem.RelativeX = xOffset;
                    gridItem.RelativeY = yOffset;
                }
            }
        }

        #endregion

        public enum HorizontalAlignment { Left, Center, Right }
        public enum VerticalAlignment { Top, Center, Bottom }

        private class GridAlignment
        {
            public HorizontalAlignment HorizontalAlignment { get; set; }
            public VerticalAlignment VerticalAlignment { get; set; }
        }       
    }
}
