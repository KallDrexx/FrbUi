using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.ManagedSpriteGroups;
using FlatRedBall.Gui;
using System.Collections.ObjectModel;
using FlatRedBall.Graphics;
using FlatRedBall;

namespace FrbUi.Controls
{
    public class UiButton : SpriteFrame, ILayoutable, IWindow
    {
        protected float _lastScaleX;
        protected float _lastScaleY;
        protected bool _pushedWithFocus;
        protected Text _label;
        protected bool _paused;

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

        public string Text
        {
            get { return _label.DisplayText; }
            set { _label.DisplayText = value; }
        }

        public ILayoutableEvent OnSizeChangeHandler { get; set; }
        public event ILayoutableEvent OnFocused;
        public event ILayoutableEvent OnFocusLost;
        public event ILayoutableEvent OnPushed;
        public event ILayoutableEvent OnReleased;

        public UiButton() : base()
        {
            // Set defaults 
            PixelSize = 0.5f;
            _label = new Text();
            _label.AttachTo(this, false);
            _label.SetPixelPerfectScale(SpriteManager.Camera);
            _label.HorizontalAlignment = HorizontalAlignment.Center;
            TextManager.AddText(_label);
        }

        public virtual void Activity()
        {
            // Check if the size has changed
            if (_lastScaleX != ScaleX || _lastScaleY != ScaleY)
                if (OnSizeChangeHandler != null)
                    OnSizeChangeHandler(this);

            _lastScaleX = ScaleX;
            _lastScaleY = ScaleY;
        }

        public void ResizeAroundText(float horizontalMargin, float verticalMargin)
        {
            ScaleX = _label.ScaleX + horizontalMargin;
            ScaleY = _label.ScaleY + verticalMargin;
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

        public void AddToManagers(Layer layer)
        {
            if (layer != null)
            {
                SpriteManager.AddToLayer(this, layer);
                TextManager.AddToLayer(_label, layer);
            }

            GuiManager.AddWindow(this);
            Layer = layer;
        }

        #region IWindow Implementation

        public bool MovesWhenGrabbed { get; set; }
        public SpriteFrame SpriteFrame { get { return this; } set { throw new NotImplementedException(); } }
        public new IWindow Parent { get { return null; } set { throw new NotImplementedException(); } }
        public bool GuiManagerDrawn { get { return false; } set { throw new NotImplementedException(); } }
        public new ReadOnlyCollection<IWindow> Children { get { return null; } }
        public ReadOnlyCollection<IWindow> FloatingChildren { get { return null; } }
        public Layer Layer { get; protected set; }

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
                return RelativePosition.X;  
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
                return RelativePosition.Y;
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
                return Position.X;
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
                return Position.Y;
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

            if (cursor.IsOn3D(this, Layer))
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

        public enum ButtonState { Active, Disabled, Focused, Pushed }
    }
}
