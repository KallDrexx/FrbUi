using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.ManagedSpriteGroups;
using FlatRedBall.Graphics;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.LayoutManagers
{
    public class BoxLayoutManager : ILayoutable
    {
        public enum Alignment { Default, Inverse }
        public enum Direction { Up, Down, Left, Right }

        protected SpriteFrame _backgroundSprite;
        protected Layer _layer;
        protected Dictionary<ILayoutable, Alignment> _items;
        protected bool _recalculateLayout;

        public BoxLayoutManager()
        {
            _items = new Dictionary<ILayoutable, Alignment>();
            _backgroundSprite = new SpriteFrame();
            _backgroundSprite.PixelSize = 0.5f;
            _backgroundSprite.Alpha = 0f;
        }

        public ILayoutableEvent OnSizeChangeHandler { get; set; }

        #region Properties

        public Direction CurrentDirection { get; set; }
        public float Margin { get; set; }
        public float Spacing { get; set; }

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

        #endregion

        #region Methods

        public void Activity()
        {
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

        public void AddItem(ILayoutable item, bool inverseAlignment = false)
        {
            if (_items.ContainsKey(item))
                return;

            var alignment = (inverseAlignment ? Alignment.Inverse : Alignment.Default);
            _items.Add(item, alignment);
            item.AttachTo(_backgroundSprite, false);
            _recalculateLayout = true;
            PerformLayout();

            item.OnSizeChangeHandler = new ILayoutableEvent(delegate(ILayoutable sender)
            {
                _recalculateLayout = true;
            });
        }

        public void UpdateDependencies(double currentTime)
        {
            PerformLayout();
            _backgroundSprite.UpdateDependencies(currentTime);
        }

        public void ForceUpdateDependencies()
        {
            PerformLayout();
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

        protected virtual void PerformLayout()
        {
            // Remove any items that this is no longer the parent of
            for (int x = _items.Count - 1; x >= 0; x--)
            {
                var item = _items.Keys.ElementAt(x);
                if (item.Parent != _backgroundSprite)
                {
                    _items.Remove(item);
                    _recalculateLayout = true;
                }
            }

            if (!_recalculateLayout)
                return; // Not flagged to actually reset the layout

            // Reset the flag so we don't reset the layout again
            _recalculateLayout = false;

            switch (CurrentDirection)
            {
                case Direction.Up:
                    PerformVerticalLayout(true);
                    break;

                case Direction.Down:
                    PerformVerticalLayout(false);
                    break;

                case Direction.Left:
                    PerformHorizontalLayout(false);
                    break;

                case Direction.Right:
                default:
                    PerformHorizontalLayout(true);
                    break;
            }
        }

        protected virtual void PerformVerticalLayout(bool increasing)
        {
            // Calculate the width and height
            float width = 0;
            float height = 0;

            foreach (var item in _items.Keys)
            {
                height += (item.ScaleY);
                if (item.ScaleX > width)
                    width = item.ScaleX;
            }

            // Add the margins and spacings
            width += Margin;
            height += (Margin + ((Spacing / 2) * _items.Count - 1));

            // Set the scales
            ScaleX = (width);
            ScaleY = (height);

            // Compute the Scale properties
            float currentX, currentY;

            if (increasing)
            {
                // bottom left corner
                currentX = 0 - ScaleX + Margin;
                currentY = 0 - ScaleY + Margin;
            }
            else
            {
                // top left corner
                currentX = 0 - ScaleX + Margin;
                currentY = ScaleY - Margin;
            }

            bool firstItem = true;
            foreach (var item in _items.Keys)
            {
                if (!firstItem)
                {
                    if (increasing)
                        currentY += Spacing;
                    else
                        currentY -= Spacing;
                }

                // Since the x/y position will point to the center, we need to account for that
                if (increasing)
                {
                    if (_items[item] == Alignment.Inverse)
                        item.RelativeX = (currentX * -1) - item.ScaleX;
                    else
                        item.RelativeX = currentX + item.ScaleX;

                    item.RelativeY = currentY + item.ScaleY;
                    currentY += (item.ScaleY * 2);
                }
                else
                {
                    if (_items[item] == Alignment.Inverse)
                        item.RelativeX = (currentX * -1) - item.ScaleX;
                    else
                        item.RelativeX = currentX + item.ScaleX;

                    item.RelativeY = currentY - item.ScaleY;
                    currentY -= (item.ScaleY * 2);
                }

                firstItem = false;
            }
        }

        protected virtual void PerformHorizontalLayout(bool increasing)
        {
            // Calculate the width and height
            float halfWidth = 0;
            float halfHeight = 0;
            foreach (var item in _items.Keys)
            {
                halfWidth += (item.ScaleX);
                if (item.ScaleY > halfHeight)
                    halfHeight = item.ScaleY;
            }

            // Add the margins
            halfWidth += (Margin + ((Spacing / 2) * _items.Count - 1));
            halfHeight += Margin;

            // Set the manager's Scale properties
            ScaleX = (halfWidth);
            ScaleY = (halfHeight);

            // Calculate the positioning of all the controls
            float currentX, currentY;

            if (increasing)
            {
                // Top left corner
                currentX = 0 - ScaleX + Margin;
                currentY = ScaleY - Margin;
            }
            else
            {
                // Top right corner
                currentX = ScaleX - Margin;
                currentY = ScaleY - Margin;
            }

            bool firstItem = true;
            foreach (var item in _items.Keys)
            {
                if (!firstItem)
                {
                    if (increasing)
                        currentX += Spacing;
                    else
                        currentX -= Spacing;
                }

                // Since the x/y position will point to the center, we need to account for that
                if (increasing)
                {
                    if (_items[item] == Alignment.Inverse)
                        item.RelativeY = (currentY * -1) + item.ScaleY;
                    else
                        item.RelativeY = currentY - item.ScaleY;

                    item.RelativeX = currentX + item.ScaleX;
                    currentX += (item.ScaleX * 2);
                }
                else
                {
                    if (_items[item] == Alignment.Inverse)
                        item.RelativeY = (currentY * -1) + item.ScaleY;
                    else
                        item.RelativeY = currentY - item.ScaleY;

                    item.RelativeX = currentX - item.ScaleX;
                    currentX -= (item.ScaleX * 2);
                }

                firstItem = false;
            }
        }

        #endregion
    }
}
