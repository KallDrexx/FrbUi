using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Graphics;
using FlatRedBall;
using FlatRedBall.ManagedSpriteGroups;
using Microsoft.Xna.Framework;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

namespace FrbUi.LayoutManagers
{
    public class CircularLayoutManager : ILayoutable
    {
        protected const float FULL_CIRCLE = 2 * (float)Math.PI;

        protected SpriteFrame _backgroundSprite;
        protected Layer _layer;
        protected Dictionary<ILayoutable, CircularPosition> _items;
        protected bool _recalculateLayout;
        protected AxisAlignedRectangle _border;

        public CircularLayoutManager()
        {
            _items = new Dictionary<ILayoutable, CircularPosition>();
            _backgroundSprite = new SpriteFrame();
            _backgroundSprite.PixelSize = 0.5f;
            _backgroundSprite.Alpha = 0f;
        }

        public ILayoutableEvent OnSizeChangeHandler { get; set; }

        #region Properties

        public IEnumerable<ILayoutable> Items { get { return _items.Keys.AsEnumerable(); } }
        public ArrangementMode CurrentArrangementMode { get; set; }
        public float MinDegreeOffset { get; set; }
        public float StartingDegrees { get; set; }
        public float Radius { get; set; }
        public float Margin { get; set; }

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

                if (ShowBorder)
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

                if (ShowBorder)
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

        public void AddItem(ILayoutable item, float radiusOffset = 0, float degreeOffset = 0)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!_items.ContainsKey(item))
            {
                // Add the item to the list
                _items.Add(item, new CircularPosition());
                item.AttachTo(_backgroundSprite, false);
            }

            // Calculate the item's position
            _items[item].RadiusOffset = radiusOffset;
            _items[item].RadianOffset = MathHelper.ToRadians(degreeOffset);

            // Trigger a recalulation of the layout
            _recalculateLayout = true;

            // Set the size to realculate when a control changes 
            item.OnSizeChangeHandler = new ILayoutableEvent(delegate(ILayoutable sender) { _recalculateLayout = true; });
        }

        public void UpdateDependencies(double currentTime)
        {
            RecalculateLayout();
        }

        public void ForceUpdateDependencies()
        {
            RecalculateLayout();
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

        protected void PositionItem(ILayoutable item, ILayoutable lastItem = null)
        {
            var position = _items[item];
            var lastPosition = (lastItem != null ? _items[lastItem] : (CircularPosition)null);
            float startingRadians = MathHelper.ToRadians(StartingDegrees);
            float absoluteRadians;

            switch (CurrentArrangementMode)
            {
                case ArrangementMode.Clockwise:
                    absoluteRadians = CalculateClockwisePosition(position, lastPosition, startingRadians);
                    break;

                case ArrangementMode.CounterClockwise:
                    absoluteRadians = CalculateClockwisePosition(position, lastPosition, startingRadians, true);
                    break;

                case ArrangementMode.EvenlySpaced:
                    absoluteRadians = CalculateEvenlySpacedPosition(position, lastPosition, startingRadians);
                    break;

                case ArrangementMode.Manual:
                default:
                    absoluteRadians = startingRadians + position.RadianOffset;
                    break;
            }

            position.AbsoluteRadians = absoluteRadians;
            float xCoord = (float)Math.Cos(absoluteRadians) * (Radius + position.RadiusOffset);
            float yCoord = (float)Math.Sin(absoluteRadians) * (Radius + position.RadiusOffset);

            item.RelativeX = xCoord;
            item.RelativeY = yCoord;
        }

        protected float CalculateEvenlySpacedPosition(CircularPosition position, CircularPosition lastPosition, float startingRadians)
        {
            float absoluteRadians;

            if (lastPosition == null)
            {
                // If no last item provided, assume this is the first
                absoluteRadians = startingRadians + position.RadianOffset;
            }
            else
            {
                float spacing = FULL_CIRCLE / _items.Count;
                absoluteRadians = lastPosition.AbsoluteRadians + spacing + position.RadianOffset;
            }

            return absoluteRadians;
        }

        protected float CalculateClockwisePosition(CircularPosition position, CircularPosition lastPosition, float startingRadians, bool reverse = false)
        {
            float absoluteRadians;
            float minRadianOffset = MathHelper.ToRadians(MinDegreeOffset);

            // Position it min offset away from the previous
            //   if no last position (first item being laid out) lay it out on the starting degree
            if (reverse)
            {
                if (lastPosition != null)
                    absoluteRadians = lastPosition.AbsoluteRadians + minRadianOffset + position.RadianOffset;
                else
                    absoluteRadians = startingRadians + position.RadianOffset;
            }
            else
            {
                if (lastPosition != null)
                    absoluteRadians = lastPosition.AbsoluteRadians - minRadianOffset - position.RadianOffset;
                else
                    absoluteRadians = startingRadians - position.RadianOffset;
            }

            return absoluteRadians;
        }

        protected void RecalculateLayout()
        {
            if (!_recalculateLayout)
                return;

            var lastItem = (ILayoutable)null;

            // Go through all the items and find the min and max positions of items
            //   The origin is always 
            float? minX = null, minY = null, maxX = null, maxY = null;

            for (int x = _items.Count - 1; x >= 0; x--)
            {
                var item = _items.Keys.ElementAt(x);

                // Make sure the item still has this as its parent
                if (item.Parent != _backgroundSprite)
                {
                    _items.Remove(item);
                    continue;
                }

                // Update the item's position
                PositionItem(item, lastItem);

                float leftX = item.RelativeX - item.ScaleX;
                float rightX = item.RelativeX + item.ScaleX;
                float topY = item.RelativeY + item.ScaleY;
                float bottomY = item.RelativeY - item.ScaleY;

                // If the item has any bounds outside of the current min/max, 
                //    set it as the new min/max
                if (minX == null || minX > leftX)
                    minX = leftX;

                if (maxX == null || maxX < rightX)
                    maxX = rightX;

                if (minY == null || minY > bottomY)
                    minY = bottomY;

                if (maxY == null || maxY < topY)
                    maxY = topY;

                lastItem = item;
            }

            // Recalculate scale to whatever the maximum values are
            ScaleX = (Math.Max(Math.Abs(minX.Value), Math.Abs(maxX.Value))) + Margin;
            ScaleY = (Math.Max(Math.Abs(minY.Value), Math.Abs(maxY.Value))) + Margin;
            _recalculateLayout = false;
        }

        #endregion

        public enum ArrangementMode { Clockwise, EvenlySpaced, Manual, CounterClockwise }

        public class CircularPosition
        {
            public float RadianOffset { get; set; }
            public float RadiusOffset { get; set; }
            public float AbsoluteRadians { get; set; }
        }
    }
}
