using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Graphics;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Controls
{
    public class LayoutableSprite : ILayoutable
    {
        private readonly Sprite _sprite;

        #region Properties

        public LayoutableEvent OnSizeChangeHandler { get; set; }
        public LayoutableEvent OnAddedToLayout { get; set; }

        public Layer Layer { get; private set; }
        public ILayoutable ParentLayout { get; set; }
        public string Tag { get; set; }

        public AnimationChainList AnimationChains
        {
            get { return _sprite.AnimationChains; }
            set { _sprite.AnimationChains = value; }
        }

        public string CurrentAnimationChainName
        {
            get { return _sprite.CurrentChainName; }
            set { _sprite.CurrentChainName = value; }
        }

        public float PixelSize
        {
            get { return _sprite.PixelSize; }
            set { _sprite.PixelSize = value; }
        }

        public float RelativeX
        {
            get { return _sprite.RelativeX; }
            set { _sprite.RelativeX = value; }
        }

        public float RelativeY
        {
            get { return _sprite.RelativeY; }
            set { _sprite.RelativeY = value; }
        }

        public float RelativeZ
        {
            get { return _sprite.RelativeZ; }
            set { _sprite.RelativeZ = value; }
        }

        public float Alpha
        {
            get { return _sprite.Alpha; }
            set { _sprite.Alpha = value; }
        }

        public bool AbsoluteVisible
        {
            get { return _sprite.AbsoluteVisible; }
        }

        public bool IgnoresParentVisibility
        {
            get { return _sprite.IgnoresParentVisibility; }
            set { _sprite.IgnoresParentVisibility = value; }
        }

        public IVisible Parent
        {
            get { return _sprite.Parent as IVisible; }
        }

        public bool Visible
        {
            get { return _sprite.Visible; }
            set { _sprite.Visible = value; }
        }

        public float ScaleX
        {
            get { return _sprite.ScaleX; }
            set
            {
                _sprite.ScaleX = value;
                if (OnSizeChangeHandler != null)
                    OnSizeChangeHandler(this);
            }
        }

        public float ScaleXVelocity
        {
            get { return _sprite.ScaleXVelocity; }
            set { _sprite.ScaleXVelocity = value; }
        }

        public float ScaleY
        {
            get { return _sprite.ScaleY; }
            set
            {
                _sprite.ScaleY = value;
                if (OnSizeChangeHandler != null)
                    OnSizeChangeHandler(this);
            }
        }

        public float ScaleYVelocity
        {
            get { return _sprite.ScaleYVelocity; }
            set { _sprite.ScaleYVelocity = value; }
        }

        public float XAcceleration
        {
            get { return _sprite.XAcceleration; }
            set { _sprite.XAcceleration = value; }
        }

        public float XVelocity
        {
            get { return _sprite.XVelocity; }
            set { _sprite.XVelocity = value; }
        }

        public float YAcceleration
        {
            get { return YAcceleration; }
            set { _sprite.YAcceleration = value; }
        }

        public float YVelocity
        {
            get { return YVelocity; }
            set { _sprite.YVelocity = value; }
        }

        public float ZAcceleration
        {
            get { return _sprite.ZAcceleration; }
            set { _sprite.ZAcceleration = value; }
        }

        public float ZVelocity
        {
            get { return _sprite.ZVelocity; }
            set { _sprite.ZVelocity = value; }
        }

        public float X
        {
            get { return _sprite.X; }
            set { _sprite.X = value; }
        }

        public float Y
        {
            get { return _sprite.Y; }
            set { _sprite.Y = value; }
        }

        public float Z
        {
            get { return _sprite.Z; }
            set { _sprite.Z = value; }
        }

        public float RelativeRotationZ
        {
            get { return _sprite.RelativeRotationZ; }
            set { _sprite.RelativeRotationZ = value; }
        }

        #endregion

        #region Methods

        public LayoutableSprite()
        {
            _sprite = new Sprite
            {
                PixelSize = 0.5f, 
                UseAnimationRelativePosition = false
            };
        }

        public void Activity()
        {
        }

        public void AttachTo(PositionedObject obj, bool changeRelative)
        {
            _sprite.AttachTo(obj, changeRelative);
        }

        public void AttachObject(PositionedObject obj, bool changeRelative)
        {
            obj.AttachTo(_sprite, changeRelative);
        }

        public void AttachTo(ILayoutable obj, bool changeRelative)
        {
            obj.AttachObject(_sprite, changeRelative);
        }

        public void AttachObject(ILayoutable obj, bool changeRelatative)
        {
            obj.AttachTo(_sprite, changeRelatative);
        }

        public void Detach()
        {
            _sprite.Detach();
        }

        public void AddToManagers(Layer layer)
        {
            SpriteManager.AddToLayer(_sprite, layer);
            Layer = layer;
        }

        public void UpdateDependencies(double currentTime)
        {
        }

        public void ForceUpdateDependencies()
        {
        }

        public void Destroy()
        {
            SpriteManager.RemoveSprite(_sprite);
        }

        #endregion
    }
}
