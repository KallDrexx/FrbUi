using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.ManagedSpriteGroups;
using FlatRedBall.Graphics;
using FlatRedBall;
using FrbUi.Positioning;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Layouts
{
    public class SimpleLayout : ILayoutable
    {
        protected SpriteFrame _backgroundSprite;
        protected Layer _layer;
        protected Dictionary<ILayoutable, OverallPosition> _items;
        protected bool _isFullScreen;
        protected float _alpha;

        public SimpleLayout()
        {
            _items = new Dictionary<ILayoutable, OverallPosition>();
            _backgroundSprite = new SpriteFrame();
            _backgroundSprite.PixelSize = 0.5f;
            _backgroundSprite.Alpha = 0f;
        }

        public ILayoutableEvent OnSizeChangeHandler { get; set; }

        #region Properties

        public bool KeepBackgroundAlphaSynced { get; set; }
        public IEnumerable<ILayoutable> Items { get { return _items.Keys.AsEnumerable(); } }
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
                foreach (var item in _items.Keys)
                    item.Alpha = value;
            }
        }

        #endregion

        #region Methods

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

        public void UpdateDependencies(double currentTime)
        {
            _backgroundSprite.UpdateDependencies(currentTime);
        }

        public void ForceUpdateDependencies()
        {
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

        public void AddItem(ILayoutable item, HorizontalPosition horizontalPosition, VerticalPosition verticalPosition, LayoutOrigin layoutFrom = LayoutOrigin.Center)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var position = new OverallPosition { HorizontalPosition = horizontalPosition, VerticalPosition = verticalPosition };
            _items.Add(item, position);
            item.Alpha = _alpha;
            item.AttachTo(_backgroundSprite, false);

            PositionItem(item, horizontalPosition, verticalPosition, layoutFrom);

            // When the size changes, make sure to reposition the item so it's still in the same spot
            item.OnSizeChangeHandler = new ILayoutableEvent(delegate(ILayoutable sender) { PositionItem(item, horizontalPosition, verticalPosition, layoutFrom); });
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
