using System;
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
        private readonly SpriteFrame _backgroundSprite;
        private Layer _layer;
        private readonly Text _label;
        private bool _paused;
        private bool _enabled;
        private float _alpha;

        private string _standardAnimationChainName;
        private string _focusedAnimationChainName;
        private string _pushedAnimationChainName;
        private string _disabledAnimationChainName;   

        #region Events

        public LayoutableEvent OnSizeChangeHandler { get; set; }
        public LayoutableEvent OnAddedToLayout { get; set; }
        public LayoutableEvent OnFocused { get; set; }
        public LayoutableEvent OnFocusLost { get; set; }
        public LayoutableEvent OnPushed { get; set; }
        public LayoutableEvent OnPushReleased { get; set; }
        public LayoutableEvent OnClicked { get; set; }

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

        public bool PushedWithFocus { get; set; }
        public ILayoutable ParentLayout { get; set; }
        public string Tag { get; set; }
        public SelectableState CurrentSelectableState { get; set; }
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
        public float Pixelsize { get { return _backgroundSprite.PixelSize; } set { _backgroundSprite.PixelSize = value; } }
        public SpriteFrame.BorderSides Border { get { return _backgroundSprite.Borders; } set { _backgroundSprite.Borders = value; } }
        public string CurrentAnimationChainName
        {
            get { return _backgroundSprite.CurrentChainName; } 
            set
            {
                _backgroundSprite.CurrentChainName = value;
                _backgroundSprite.Alpha = _backgroundSprite.Texture == null ? 0 : _alpha;
            }
        }

        public string Text
        {
            get { return _label.DisplayText; } 
            set 
            { 
                _label.DisplayText = value; 

                // Reset the scaling
                _label.SetPixelPerfectScale(Layer);
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

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                // Don't do anything if the enabled state isn't changing
                if (_enabled == value)
                    return;

                // Set the _enabled variable and reset the button state to not selected
                _enabled = value;
                CurrentSelectableState = SelectableState.NotSelected;

                // Switch to the new state
                if (value)
                {
                    if (!string.IsNullOrEmpty(_standardAnimationChainName))
                        _backgroundSprite.CurrentChainName = _standardAnimationChainName;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_disabledAnimationChainName))
                        _backgroundSprite.CurrentChainName = _disabledAnimationChainName;
                }
            }
        }

        public float Alpha 
        { 
            get { return _alpha; } 
            set 
            {
                _alpha = value;
                _label.Alpha = value;

                _backgroundSprite.Alpha = _backgroundSprite.Texture == null 
                    ? 0 
                    : value;
            } 
        }

        #endregion

        #region Methods

        public Button()
        {
            _paused = false;
            _enabled = true;
            _alpha = 0;
            _backgroundSprite = new SpriteFrame
            {
                PixelSize = 0.5f, 
                Borders = SpriteFrame.BorderSides.All,
                ScaleX = 20f,
                ScaleY = 20f,
                Alpha = 0f
            };

            _label = new Text();
            _label.AttachTo(_backgroundSprite, false);
            _label.RelativeZ = 0.1f;
            _label.HorizontalAlignment = HorizontalAlignment.Center;
        }

        public void ResizeAroundText(float horizontalMargin, float verticalMargin)
        {
            ScaleX = _label.ScaleX + horizontalMargin;
            ScaleY = _label.ScaleY + verticalMargin;
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

        public ILayoutable Clone()
        {
            var clonedItem = UiControlManager.Instance.CreateControl<Button>();
            clonedItem.AnimationChains = AnimationChains;
            clonedItem.StandardAnimationChainName = StandardAnimationChainName;
            clonedItem.PushedAnimationChainName = PushedAnimationChainName;
            clonedItem.FocusedAnimationChainName = FocusedAnimationChainName;
            clonedItem.DisabledAnimationChainName = DisabledAnimationChainName;
            clonedItem.CurrentAnimationChainName = CurrentAnimationChainName;
            clonedItem.Alpha = Alpha;
            clonedItem.Border = Border;
            clonedItem.Enabled = Enabled;
            clonedItem.CurrentSelectableState = CurrentSelectableState;
            clonedItem.IgnoreCursorEvents = IgnoreCursorEvents;
            clonedItem.IgnoreCursorFocus = IgnoreCursorFocus;
            clonedItem.Layer = Layer;
            clonedItem.OnAddedToLayout = OnAddedToLayout;
            clonedItem.OnClicked = OnClicked;
            clonedItem.OnFocusLost = OnFocusLost;
            clonedItem.OnFocused = OnFocused;
            clonedItem.OnPushReleased = OnPushReleased;
            clonedItem.OnPushed = OnPushed;
            clonedItem.OnSizeChangeHandler = OnSizeChangeHandler;
            clonedItem.IgnoresParentVisibility = IgnoresParentVisibility;
            clonedItem.ScaleX = ScaleX;
            clonedItem.ScaleY = ScaleY;
            clonedItem.Tag = Tag;
            clonedItem.Text = Text;
            clonedItem.Visible = Visible;

            return clonedItem;
        }

        #endregion

        #region IWindow Implementation

        public bool MovesWhenGrabbed { get; set; }
        public SpriteFrame SpriteFrame { get { return _backgroundSprite; } set { throw new NotImplementedException(); } }
        public bool GuiManagerDrawn { get { return false; } set { throw new NotImplementedException(); } }
        public ReadOnlyCollection<IWindow> Children { get { return null; } }
        public ReadOnlyCollection<IWindow> FloatingChildren { get { return null; } }
        IWindow IWindow.Parent { get { return null; } set { throw new NotImplementedException(); } }

        public void CallClick()
        {
            if (!IgnoreCursorEvents)
            {
                this.Click();
            }
        }

        public void Activity(Camera camera)
        {
        }

        public void CallRollOff()
        {
            if (!IgnoreCursorEvents)
                this.LoseFocus();
        }

        public void CallRollOn()
        {
            if (!IgnoreCursorEvents && !IgnoreCursorFocus)
                this.Focus();
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
            if (HasCursorOver(cursor) && !IgnoreCursorEvents)
            {
                cursor.WindowOver = this;

                if (cursor.PrimaryPush)
                {
                    cursor.WindowPushed = this;
                    this.Click();
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
