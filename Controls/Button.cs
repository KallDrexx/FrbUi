using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Gui;
using FlatRedBall.ManagedSpriteGroups;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall;
using System.Collections.ObjectModel;

namespace FrbUi.Controls
{
    public class Button : ILayoutable, IWindow, ISelectable, IDisableable
    {
        protected SpriteFrame _backgroundSprite;
        protected Layer _layer;
        protected Text _label;
        protected bool _pushedWithFocus;
        protected bool _paused;

        protected string _standardAnimationChainName;
        protected string _focusedAnimationChainName;
        protected string _pushedAnimationChainName;
        protected string _disabledAnimationChainName;   

        #region Events

        public ILayoutableEvent OnSizeChangeHandler { get; set; }
        public ILayoutableEvent OnFocused { get; set; }
        public ILayoutableEvent OnFocusLost { get; set; }
        public ILayoutableEvent OnPushed { get; set; }
        public ILayoutableEvent OnClicked { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// If true, the button can only change states via explicit method calls
        /// instead of IWindow events
        /// </summary>
        public bool IgnoreCursorEvents { get; set; }

        /// <summary>
        ///  If true, the button isn't focused when the cursor hovers over the button
        /// </summary>
        public bool IgnoreCursorFocus { get; set; }

        public SelectableState CurrentSelectableState { get; protected set; }

        public bool Enabled { get; set; }
        public float Alpha { get { return _backgroundSprite.Alpha; } set { _backgroundSprite.Alpha = value; } }
        public AnimationChainList AnimationChains { get { return _backgroundSprite.AnimationChains; } set { _backgroundSprite.AnimationChains = value; } }
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
        public string Name { get { return "FrbUi Button"; } set { throw new NotImplementedException(); } }
        public string Text { get { return _label.DisplayText; } set { _label.DisplayText = value; }}

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

        public string StandardAnimationChainName
        {
            get { return _standardAnimationChainName; }
            set
            {
                if (!_backgroundSprite.ContainsChainName(value))
                    throw new InvalidOperationException("The animation chain list does not contain a chain with the name of " + value);

                _standardAnimationChainName = value;

                // We we should be displaying this animation chain, activate it
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

                // We we should be displaying this animation chain, activate it
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

                // We we should be displaying this animation chain, activate it
                if (CurrentSelectableState == SelectableState.Pushed)
                    _backgroundSprite.CurrentChainName = value;
            }
        }

        public string DisabledAnimationChainName
        {
            get { return _disabledAnimationChainName; }
            set
            {
                if (!_backgroundSprite.ContainsChainName(value))
                    throw new InvalidOperationException("The animation chain list does not contain a chain with the name of " + value);

                _disabledAnimationChainName = value;

                // If we are currently in the disabled state, switch to that chain
                if (!Enabled)
                    _backgroundSprite.CurrentChainName = value;
            }
        }

        #endregion

        #region Methods

        public Button()
        {
            _backgroundSprite = new SpriteFrame();
            _label = new Text();
            _label.AttachTo(_backgroundSprite, false);
            _label.SetPixelPerfectScale(SpriteManager.Camera);
            _label.HorizontalAlignment = HorizontalAlignment.Center;
        }

        public void Focus()
        {
            // A button can only be focused if it's in an active state
            if (CurrentSelectableState != SelectableState.NotSelected)
                return;

            CurrentSelectableState = SelectableState.Focused;
            if (_focusedAnimationChainName != null)
                _backgroundSprite.CurrentChainName = _focusedAnimationChainName;

            if (OnFocused != null)
                OnFocused(this);
        }

        public void LoseFocus()
        {
            // Focus can only be lost if we are not disabled or active
            if (!Enabled || CurrentSelectableState == SelectableState.NotSelected)
                return;

            CurrentSelectableState = SelectableState.NotSelected;
            if (_standardAnimationChainName != null)
                _backgroundSprite.CurrentChainName = _standardAnimationChainName;

            if (OnFocusLost != null)
                OnFocusLost(this);
        }

        public void Push()
        {
            // Button must be focused to be pushed
            if (CurrentSelectableState != SelectableState.Focused && CurrentSelectableState != SelectableState.NotSelected)
                return;

            _pushedWithFocus = (CurrentSelectableState == SelectableState.Focused);
            CurrentSelectableState = SelectableState.Pushed;
            if (_pushedAnimationChainName != null)
                _backgroundSprite.CurrentChainName = _pushedAnimationChainName;

            if (OnPushed != null)
                OnPushed(this);
        }

        public void Click()
        {
            if (CurrentSelectableState != SelectableState.Pushed && CurrentSelectableState != SelectableState.Focused)
                return;

            if (_pushedWithFocus)
            {
                CurrentSelectableState = SelectableState.Focused;
                if (_focusedAnimationChainName != null)
                    _backgroundSprite.CurrentChainName = _focusedAnimationChainName;
            }
            else
            {
                CurrentSelectableState = SelectableState.NotSelected;
                if (_standardAnimationChainName != null)
                    _backgroundSprite.CurrentChainName = _standardAnimationChainName;
            }

            _pushedWithFocus = false;

            if (OnClicked != null)
                OnClicked(this);
        }

        public void Press()
        {
            Push();
            Click();
        }

        public void ResizeAroundText(float horizontalMargin, float VerticalMargin)
        {
            ScaleX = _label.ScaleX + horizontalMargin;
            ScaleY = _label.ScaleY + VerticalMargin;
        }

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
            CurrentSelectableState = SelectableState.NotSelected;

            SpriteManager.AddSpriteFrame(_backgroundSprite);
            GuiManager.AddWindow(this);
            TextManager.AddText(_label);
            TextManager.AddToLayer(_label, layer);

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
            TextManager.RemoveText(_label);
            GuiManager.RemoveWindow(this);
        }

        #endregion

        #region IWindow Implementation

        public bool MovesWhenGrabbed { get; set; }
        public SpriteFrame SpriteFrame { get { return _backgroundSprite; } set { throw new NotImplementedException(); } }
        public bool GuiManagerDrawn { get { return false; } set { throw new NotImplementedException(); } }
        public ReadOnlyCollection<IWindow> Children { get { return null; } }
        public ReadOnlyCollection<IWindow> FloatingChildren { get { return null; } }
        IWindow IWindow.Parent { get { return null; } set { throw new NotImplementedException(); } }

        public void Activity(FlatRedBall.Camera camera)
        {
        }

        public void CallClick()
        {
            if (!IgnoreCursorEvents)
            {
                Push();
                Click();
            }
        }

        public void CallRollOff()
        {
            if (!IgnoreCursorEvents)
                LoseFocus();
        }

        public void CallRollOn()
        {
            if (!IgnoreCursorEvents && !IgnoreCursorFocus)
                Focus();
        }

        public void OnDragging()
        {
        }

        public void OnLosingFocus()
        {
        }

        public void OnResize()
        {
            throw new NotImplementedException();
        }

        public void OnResizeEnd()
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            throw new NotImplementedException();
        }

        public bool GetParentVisibility()
        {
            throw new NotImplementedException();
        }

        public bool IsPointOnWindow(float x, float y)
        {
            throw new NotImplementedException();
        }

        public bool OverlapsWindow(IWindow otherWindow)
        {
            throw new NotImplementedException();
        }

        public void SetScaleTL(float newScaleX, float newScaleY, bool keepTopLeftStatic)
        {
            throw new NotImplementedException();
        }

        public void SetScaleTL(float newScaleX, float newScaleY)
        {
            throw new NotImplementedException();
        }

        public void TestCollision(Cursor cursor)
        {
            if (HasCursorOver(cursor))
            {
                cursor.WindowOver = this;

                if (cursor.PrimaryPush)
                {
                    cursor.WindowPushed = this;
                    Press();
                    cursor.GrabWindow(this);
                }
            }
        }

        public void UpdateDependencies()
        {

        }

        public float WorldUnitRelativeX
        {
            get
            {
                return _backgroundSprite.RelativePosition.X;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public float WorldUnitRelativeY
        {
            get
            {
                return _backgroundSprite.RelativePosition.Y;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public float WorldUnitX
        {
            get
            {
                return _backgroundSprite.Position.X;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public float WorldUnitY
        {
            get
            {
                return _backgroundSprite.Position.Y;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool HasCursorOver(Cursor cursor)
        {
            if (_paused)
                return false;

            if (!AbsoluteVisible)
                return false;

            if (Layer != null && !Layer.Visible)
                return false;

            if (!cursor.IsOn(Layer))
                return false;

            if (cursor.IsOn3D(_backgroundSprite, Layer))
                return true;

            return false;
        }

        public bool IgnoredByCursor
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
