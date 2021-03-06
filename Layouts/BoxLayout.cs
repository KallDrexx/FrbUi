﻿using System;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall.ManagedSpriteGroups;
using FlatRedBall.Graphics;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

namespace FrbUi.Layouts
{
    public class BoxLayout : ILayoutManager, ISelectable
    {
        public enum Alignment { Default, Inverse, Centered }
        public enum Direction { Up, Down, Left, Right }

        #region Variables

        private readonly SpriteFrame _backgroundSprite;
        private readonly List<KeyValuePair<ILayoutable, Alignment>> _items;
        private Layer _layer;
        private bool _recalculateLayout;
        private float _margin;
        private float _spacing;
        private Direction _currentDirection;
        private float _alpha;
        private AxisAlignedRectangle _border;
        private double _lastLayoutFrame;

        private string _standardAnimationChainName;
        private string _focusedAnimationChainName;
        private string _pushedAnimationChainName;

        #endregion

        #region Properties

        public LayoutableEvent OnSizeChangeHandler { get; set; }
        public LayoutableEvent OnAddedToLayout { get; set; }
        public LayoutableEvent OnFocused { get; set; }
        public LayoutableEvent OnFocusLost { get; set; }
        public LayoutableEvent OnPushed { get; set; }
        public LayoutableEvent OnPushReleased { get; set; }
        public LayoutableEvent OnClicked { get; set; }

        public IEnumerable<ILayoutable> Items { get { return _items.Select(x => x.Key).ToArray(); } }
        public SelectableState CurrentSelectableState { get; set; }
        public ILayoutable ParentLayout { get; set; }
        public string Tag { get; set; }

        public float BackgroundAlpha { get { return _backgroundSprite.Alpha; } set { _backgroundSprite.Alpha = value; } }
        public AnimationChainList BackgroundAnimationChains { get { return _backgroundSprite.AnimationChains; } set { _backgroundSprite.AnimationChains = value; } }
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

        public string CurrentAnimationChainName 
        {
            get { return _backgroundSprite.CurrentChainName; }
            set { _backgroundSprite.CurrentChainName = value; }
        }

        public string CurrentBackgroundAnimationChainName
        {
            get { return _backgroundSprite.CurrentChainName; } 
            set
            {
                _backgroundSprite.CurrentChainName = value;
                _backgroundSprite.Alpha = _alpha;
            }
        }

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

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;

                if (_backgroundSprite.Texture != null)
                    BackgroundAlpha = value;

                // Update the alpha values of all child objects
                foreach (var item in _items)
                    item.Key.Alpha = value;
            }
        }

        public Direction CurrentDirection
        {
            get { return _currentDirection; }
            set
            {
                _currentDirection = value;
                _recalculateLayout = true;
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

        public bool PushedWithFocus { get; set; }

        public bool ShowBorder
        {
            get { return _border != null; }
            set
            {
                // Only do something if the border status is changing
                if (ShowBorder == value)
                    return;

                if (value)
                {
                    _border = ShapeManager.AddAxisAlignedRectangle();
                    _border.ScaleX = ScaleX;
                    _border.ScaleY = ScaleY;
                    _border.AttachTo(_backgroundSprite, false);
                }
                else
                {
                    ShapeManager.Remove(_border);
                    _border = null;
                }
            }
        }

        #endregion

        #region Methods

        public BoxLayout()
        {
            _alpha = 1;

            _items = new List<KeyValuePair<ILayoutable, Alignment>>();
            _backgroundSprite = new SpriteFrame();
            _backgroundSprite.TextureBorderWidth = 0.5f;
            _backgroundSprite.PixelSize = 0.5f;
            _backgroundSprite.Borders = SpriteFrame.BorderSides.All;
            _backgroundSprite.RelativeZ = -0.1f;
            _backgroundSprite.Alpha = (_backgroundSprite.Texture == null ? 0 : _alpha);
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

        public void AddItem(ILayoutable item, Alignment alignment = Alignment.Default)
        {
            foreach (var pair in _items)
                if (pair.Key == item)
                    return; // already part of this layout

            _items.Add(new KeyValuePair<ILayoutable, Alignment>(item, alignment));
            item.AttachTo(_backgroundSprite, true);
            item.RelativeZ = 0.1f;
            item.Alpha = _alpha;
            item.ParentLayout = this;
            _recalculateLayout = true;

            if (item.OnAddedToLayout != null)
                item.OnAddedToLayout(this);

            PerformLayout();

            item.OnSizeChangeHandler = sender =>
            {
                _recalculateLayout = true;

                // If a recalculation already happened this frame, manually trigger a new one
                if (Math.Abs(_lastLayoutFrame - TimeManager.CurrentTime) > double.Epsilon)
                    ForceUpdateDependencies();
            };
        }

        public void RemoveItem(ILayoutable item)
        {
            int index = 0;
            bool found = false;
            for (int x = 0; x < _items.Count; x++)
            {
                index = x;
                found = _items[x].Key == item;
                if (found)
                    break;
            }

            if (!found)
                return; // Nothing to remove

            _items.RemoveAt(index);

            // Only detach it if the item is still attached to this
            if (item.Parent == _backgroundSprite)
            {
                item.Detach();
                item.OnSizeChangeHandler = null;
            }
        }

        public void UpdateDependencies(double currentTime)
        {
            ForceUpdateDependencies();
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

            foreach (var pair in _items)
            {
                if (pair.Key.Parent == _backgroundSprite)
                    pair.Key.Destroy();
            }

            _items.Clear();

            if (_border != null)
                ShapeManager.Remove(_border);
        }

        public ILayoutable Clone()
        {
            var clonedLayout = UiControlManager.Instance.CreateControl<BoxLayout>();
            clonedLayout.Alpha = Alpha;
            clonedLayout.BackgroundAnimationChains = BackgroundAnimationChains;
            clonedLayout.CurrentAnimationChainName = CurrentAnimationChainName;
            clonedLayout.FocusedAnimationChainName = FocusedAnimationChainName;
            clonedLayout.StandardAnimationChainName = StandardAnimationChainName;
            clonedLayout.PushedAnimationChainName = PushedAnimationChainName;
            clonedLayout.BackgroundAlpha = BackgroundAlpha;
            clonedLayout.CurrentDirection = CurrentDirection;
            clonedLayout.CurrentSelectableState = CurrentSelectableState;
            clonedLayout.IgnoresParentVisibility = IgnoresParentVisibility;
            clonedLayout.Layer = Layer;
            clonedLayout.Margin = Margin;
            clonedLayout.OnAddedToLayout = OnAddedToLayout;
            clonedLayout.OnClicked = OnClicked;
            clonedLayout.OnFocusLost = OnFocusLost;
            clonedLayout.OnFocused = OnFocused;
            clonedLayout.OnPushReleased = OnPushReleased;
            clonedLayout.OnPushed = OnPushed;
            clonedLayout.OnSizeChangeHandler = OnSizeChangeHandler;
            clonedLayout.ScaleX = ScaleX;
            clonedLayout.ScaleY = ScaleY;
            clonedLayout.ShowBorder = ShowBorder;
            clonedLayout.Spacing = Spacing;
            clonedLayout.Tag = Tag;
            clonedLayout.Visible = Visible;
            clonedLayout.PushedWithFocus = PushedWithFocus;

            foreach (var itemData in _items)
            {
                var clonedItem = itemData.Key.Clone();
                clonedLayout.AddItem(clonedItem, itemData.Value);
            }

            return clonedLayout;
        }

        protected virtual void PerformLayout()
        {
            // Remove any items that this is no longer the parent of
            for (int x = _items.Count - 1; x >= 0; x--)
            {
                var item = _items[x];
                if (item.Key.Parent != _backgroundSprite)
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

                default:
                    PerformHorizontalLayout(true);
                    break;
            }

            // Mark this frame as having had a recalculation performed
            _lastLayoutFrame = TimeManager.CurrentTime;
        }

        protected virtual void PerformVerticalLayout(bool increasing)
        {
            // Calculate the width and height
            float width = 0;
            float height = 0;

            foreach (var pair in _items)
            {
                height += (pair.Key.ScaleY);
                if (pair.Key.ScaleX > width)
                    width = pair.Key.ScaleX;
            }

            // Add the margins and spacings
            width += Margin;
            height += (Margin + ((Spacing / 2) * (_items.Count - 1)));

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
            foreach (var pair in _items)
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
                    switch (pair.Value)
                    {
                        case Alignment.Inverse:
                            pair.Key.RelativeX = (currentX * -1) - pair.Key.ScaleX;
                            break;
                        case Alignment.Centered:
                            pair.Key.RelativeX = 0;
                            break;
                        default:
                            pair.Key.RelativeX = currentX + pair.Key.ScaleX;
                            break;
                    }

                    pair.Key.RelativeY = currentY + pair.Key.ScaleY;
                    currentY += (pair.Key.ScaleY * 2);
                }
                else
                {
                    switch (pair.Value)
                    {
                        case Alignment.Inverse:
                            pair.Key.RelativeX = (currentX * -1) - pair.Key.ScaleX;
                            break;
                        case Alignment.Centered:
                            pair.Key.RelativeX = 0;
                            break;
                        default:
                            pair.Key.RelativeX = currentX + pair.Key.ScaleX;
                            break;
                    }

                    pair.Key.RelativeY = currentY - pair.Key.ScaleY;
                    currentY -= (pair.Key.ScaleY * 2);
                }

                firstItem = false;
            }
        }

        protected virtual void PerformHorizontalLayout(bool increasing)
        {
            // Calculate the width and height
            float halfWidth = 0;
            float halfHeight = 0;
            foreach (var pair in _items)
            {
                halfWidth += (pair.Key.ScaleX);
                if (pair.Key.ScaleY > halfHeight)
                    halfHeight = pair.Key.ScaleY;
            }

            // Add the margins
            halfWidth += (Margin + ((Spacing / 2) * (_items.Count - 1)));
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
            foreach (var pair in _items)
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
                    switch (pair.Value)
                    {
                        case Alignment.Inverse:
                            pair.Key.RelativeY = (currentY * -1) + pair.Key.ScaleY;
                            break;

                        case Alignment.Centered:
                            pair.Key.RelativeY = 0;
                            break;

                        default:
                            pair.Key.RelativeY = currentY - pair.Key.ScaleY;
                            break;
                    }

                    pair.Key.RelativeX = currentX + pair.Key.ScaleX;
                    currentX += (pair.Key.ScaleX * 2);
                }
                else
                {
                    switch (pair.Value)
                    {
                        case Alignment.Inverse:
                            pair.Key.RelativeY = (currentY * -1) + pair.Key.ScaleY;
                            break;

                        case Alignment.Centered:
                            pair.Key.RelativeY = 0;
                            break;

                        default:
                            pair.Key.RelativeY = currentY - pair.Key.ScaleY;
                            break;
                    }

                    pair.Key.RelativeX = currentX - pair.Key.ScaleX;
                    currentX -= (pair.Key.ScaleX * 2);
                }

                firstItem = false;
            }
        }

        #endregion
    }
}
