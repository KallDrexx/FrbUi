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
    public class Button : ILayoutable, IWindow
    {
        public enum ButtonState { Active, Disabled, Focused, Pushed }

        protected SpriteFrame _backgroundSprite;
        protected Layer _layer;
        protected float _lastScaleX;
        protected float _lastScaleY;
        protected bool _pushedWithFocus;
        protected Text _label;
        protected bool _paused;

        public ILayoutableEvent OnSizeChangeHandler { get; set; }
        public ILayoutableEvent OnFocused { get; set; }
        public ILayoutableEvent OnFocusLost { get; set; }
        public ILayoutableEvent OnPushed { get; set; }
        public ILayoutableEvent OnReleased { get; set; }

        #region Properties

        /// <summary>
        /// If true, the button can only change states via explicit method calls
        /// instead of IWindow events
        /// </summary>
        public bool IgnoreCursorEvents { get; set; }
        public ButtonState CurrentButtonState { get; protected set; }

        public bool Enabled
        {
            get { return CurrentButtonState != ButtonState.Disabled; }
            set
            {
                if (value && !Enabled)
                    CurrentButtonState = ButtonState.Active;
                else if (!value)
                    CurrentButtonState = ButtonState.Disabled;
            }
        }

        public float Alpha { get { return _backgroundSprite.Alpha; } set { _backgroundSprite.Alpha = value; } }
        public AnimationChainList AnimationChains { get { return _backgroundSprite.AnimationChains; } set { _backgroundSprite.AnimationChains = value; } }
        public string CurrentChainName { get { return _backgroundSprite.CurrentChainName; } set { _backgroundSprite.CurrentChainName = value; } }
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
            if (CurrentButtonState != ButtonState.Active)
                return;

            CurrentButtonState = ButtonState.Focused;

            if (OnFocused != null)
                OnFocused(this);
        }

        public void LoseFocus()
        {
            // Focus can only be lost if we are not disabled or active
            if (CurrentButtonState == ButtonState.Disabled || CurrentButtonState == ButtonState.Active)
                return;

            CurrentButtonState = ButtonState.Active;
            if (OnFocusLost != null)
                OnFocusLost(this);
        }

        public void Push()
        {
            // Button must be focused to be pushed
            if (CurrentButtonState != ButtonState.Focused && CurrentButtonState != ButtonState.Active)
                return;

            _pushedWithFocus = (CurrentButtonState == ButtonState.Focused);
            CurrentButtonState = ButtonState.Pushed;
            if (OnPushed != null)
                OnPushed(this);
        }

        public void Release()
        {
            if (CurrentButtonState != ButtonState.Pushed)
                return;

            if (_pushedWithFocus)
                CurrentButtonState = ButtonState.Focused;
            else
                CurrentButtonState = ButtonState.Active;

            _pushedWithFocus = false;

            if (OnReleased != null)
                OnReleased(this);
        }

        public void Press()
        {
            Push();
            Release();
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
                Release();
            }
        }

        public void CallRollOff()
        {
            if (!IgnoreCursorEvents)
                LoseFocus();
        }

        public void CallRollOn()
        {
            if (!IgnoreCursorEvents)
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

                //    // both pushing and clicking can occur in one frame because of buffered input
                //    if (cursor.PrimaryClick) 
                //    {
                //        //if (cursor.WindowPushed == this)
                //        //{
                //        //    if (Click != null)
                //        //    {
                //        //        Click(this);
                //        //    }
                //        //    if (cursor.PrimaryClickNoSlide && ClickNoSlide != null)
                //        //    {
                //        //        ClickNoSlide(this);
                //        //    }

                //        //    // if (cursor.PrimaryDoubleClick && DoubleClick != null)
                //        //    //   DoubleClick(this);
                //        //}
                //        //else
                //        //{
                //        //    if (SlideOnClick != null)
                //        //    {
                //        //        SlideOnClick(this);
                //        //    }
                //        //}
                //    }
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
