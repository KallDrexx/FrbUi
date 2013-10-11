using System;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall.ManagedSpriteGroups;
using FlatRedBall.Graphics;
using FlatRedBall;
using FrbUi.Positioning;

namespace FrbUi.Layouts
{
    public class SimpleLayout : ILayoutManager
    {
        private readonly SpriteFrame _backgroundSprite;
        private Layer _layer;
        private Dictionary<ILayoutable, OverallPosition> _items;
        private float _alpha;
        private bool _isFullScreen;

        public LayoutableEvent OnSizeChangeHandler { get; set; }
        public LayoutableEvent OnAddedToLayout { get; set; }

        #region Properties

        public ILayoutable ParentLayout { get; set; }
        public string Tag { get; set; }
        public IEnumerable<ILayoutable> Items { get { return _items.Keys.AsEnumerable(); } }
        public float BackgroundAlpha { get { return _backgroundSprite.Alpha; } set { _backgroundSprite.Alpha = value; } }
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
            }
        }

        public bool FullScreen
        {
            get { return _isFullScreen; }
            set
            {
                if (_isFullScreen == value)
                    return; // No change needed

                if (!_isFullScreen)
                {
                    AttachTo(SpriteManager.Camera, false);
                    RelativeX = 0;
                    RelativeY = 0;
                    RelativeZ = -40;
                    ScaleX = Layer.LayerCameraSettings.OrthogonalWidth / 2;
                    ScaleY = Layer.LayerCameraSettings.OrthogonalHeight / 2;
                }
                else
                {
                    Detach();
                }

                _isFullScreen = value;
            }
        }

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;

                if (_backgroundSprite.Texture != null)
                    BackgroundAlpha = value;

                // Update the alpha values of all child objects
                foreach (var item in _items.Keys)
                    item.Alpha = value;
            }
        }

        public string CurrentAnimationChainName
        {
            get { return _backgroundSprite.CurrentChainName; }
            set
            {
                _backgroundSprite.CurrentChainName = value;
                _backgroundSprite.Alpha = _backgroundSprite.Texture == null
                                              ? 0
                                              : _alpha;
            }
        }

        #endregion

        #region Methods

        public SimpleLayout()
        {
            _alpha = 1f;
            _items = new Dictionary<ILayoutable, OverallPosition>();
            _backgroundSprite = new SpriteFrame
            {
                PixelSize = 0.5f,
                Alpha = 0f
            };
        }

        public void Activity()
        {
        }

        public void AttachTo(PositionedObject obj, bool changeRelative)
        {
            _backgroundSprite.AttachTo(obj, changeRelative);
        }

        public void AttachObject(PositionedObject obj, bool changeRelative)
        {
            obj.AttachTo(_backgroundSprite, changeRelative);
        }

        public void AttachTo(ILayoutable obj, bool changeRelative)
        {
            obj.AttachObject(_backgroundSprite, changeRelative);
        }

        public void AttachObject(ILayoutable obj, bool changeRelatative)
        {
            obj.AttachTo(_backgroundSprite, changeRelatative);
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

        public void UpdateDependencies(double currentTime)
        {
            // Update the dependencies on all children
            foreach (var layoutable in Items)
                layoutable.UpdateDependencies(currentTime);

            _backgroundSprite.UpdateDependencies(currentTime);
        }

        public void ForceUpdateDependencies()
        {
            // Force all dependencies to be updated for all children
            foreach (var layoutable in Items)
                layoutable.ForceUpdateDependencies();

            _backgroundSprite.ForceUpdateDependencies();
        }

        public void Destroy()
        {
            _backgroundSprite.Detach();
            SpriteManager.RemoveSpriteFrame(_backgroundSprite);

            foreach (var item in _items.Keys)
            {
                if (item.Parent == _backgroundSprite)
                    item.Destroy();
            }

            _items.Clear();
        }

        public ILayoutable Clone()
        {
            var clonedLayout = UiControlManager.Instance.CreateControl<SimpleLayout>();
            clonedLayout.Alpha = Alpha;
            clonedLayout.BackgroundAlpha = BackgroundAlpha;
            clonedLayout.FullScreen = FullScreen;
            clonedLayout.IgnoresParentVisibility = IgnoresParentVisibility;
            clonedLayout.Layer = Layer;
            clonedLayout.OnAddedToLayout = OnAddedToLayout;
            clonedLayout.OnSizeChangeHandler = OnSizeChangeHandler;
            clonedLayout.CurrentAnimationChainName = CurrentAnimationChainName;
            clonedLayout.ScaleX = ScaleX;
            clonedLayout.ScaleY = ScaleY;
            clonedLayout.Tag = Tag;
            clonedLayout.Visible = Visible;

            foreach (var item in _items)
            {
                var clonedItem = item.Key.Clone();
                clonedLayout.AddItem(clonedItem, item.Value.HorizontalPosition, item.Value.VerticalPosition, item.Value.LayoutOrigin);
            }

            return clonedLayout;
        }

        public void AddItem(ILayoutable item, HorizontalPosition horizontalPosition, VerticalPosition verticalPosition, LayoutOrigin layoutFrom = LayoutOrigin.Center)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var position = new OverallPosition
            {
                HorizontalPosition = horizontalPosition, 
                VerticalPosition = verticalPosition,
                LayoutOrigin = layoutFrom
            };

            _items.Add(item, position);
            item.Alpha = _alpha;
            item.AttachTo(_backgroundSprite, true);
            item.RelativeZ = 0.1f;
            item.ParentLayout = this;

            if (item.OnAddedToLayout != null)
                item.OnAddedToLayout(this);

            PositionItem(item, horizontalPosition, verticalPosition, layoutFrom);

            // When the size changes, make sure to reposition the item so it's still in the same spot
            item.OnSizeChangeHandler = new LayoutableEvent(delegate(ILayoutable sender) { PositionItem(item, horizontalPosition, verticalPosition, layoutFrom); });
        }

        public void RemoveItem(ILayoutable item)
        {
            if (!_items.ContainsKey(item))
                return;

            _items.Remove(item);

            // Only detach it if the item is still attached to this
            if (item.Parent == _backgroundSprite)
            {
                item.Detach();
                item.OnSizeChangeHandler = null;
            }
        }

        private void PositionItem(ILayoutable item, HorizontalPosition horizontalPosition, VerticalPosition verticalPosition, LayoutOrigin layoutFrom)
        {
            // Make sure this is still the item's parent
            if (item.Parent != _backgroundSprite)
            {
                if (_items.ContainsKey(item))
                    _items.Remove(item);

                return;
            }

            // Position the item correctly
            float posX;
            float posY;

            // Figure out starting horizontal and vertical coordinates
            if (horizontalPosition.Alignment == HorizontalPosition.PositionAlignment.Left)
            {
                posX = 0 - ScaleX;
            }
            else if (horizontalPosition.Alignment == HorizontalPosition.PositionAlignment.Right)
            {
                posX = ScaleX;
            }
            else
            {
                posX = 0;
            }

            if (verticalPosition.Alignment == VerticalPosition.PositionAlignment.Top)
            {
                posY = ScaleY;
            }
            else if (verticalPosition.Alignment == VerticalPosition.PositionAlignment.Bottom)
            {
                posY = 0 - ScaleY;
            }
            else
            {
                posY = 0;
            }

            // Calculate offsets
            if (horizontalPosition.OffsetIsPercentage)
            {
                var total = ScaleX * 2;
                posX += (horizontalPosition.Offset * total);
            }
            else
            {
                posX += horizontalPosition.Offset;
            }

            if (verticalPosition.OffsetIsPercentage)
            {
                var total = ScaleY * 2;
                posY += (verticalPosition.Offset * total);
            }
            else
            {
                posY += verticalPosition.Offset;
            }

            // Adjust for the specified layout origin
            switch (layoutFrom)
            {
                case LayoutOrigin.TopLeft:
                    posX += item.ScaleX;
                    posY -= item.ScaleY;
                    break;

                case LayoutOrigin.TopRight:
                    posX -= item.ScaleX;
                    posY -= item.ScaleY;
                    break;

                case LayoutOrigin.BottomLeft:
                    posX += item.ScaleX;
                    posY += item.ScaleY;
                    break;

                case LayoutOrigin.BottomRight:
                    posX -= item.ScaleX;
                    posY += item.ScaleY;
                    break;

                case LayoutOrigin.Center:
                default:
                    // PosX and PosY remain at center
                    break;
            }

            // Set the item's position to the calculated spot
            item.RelativeX = posX;
            item.RelativeY = posY;
        }

        #endregion
    }
}
