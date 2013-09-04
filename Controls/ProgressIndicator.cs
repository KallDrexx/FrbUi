using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Gui;
using FlatRedBall.ManagedSpriteGroups;

namespace FrbUi.Controls
{
    public class ProgressIndicator : ILayoutable
    {
        private readonly Sprite _backgroundSprite;
        private readonly Sprite _foregroundSprite;
        private float _alpha;
        private int _currentValue;

        #region Properties

        public LayoutableEvent OnSizeChangeHandler { get; set; }
        public Layer Layer { get; set; }
        public ILayoutable ParentLayout { get; set; }

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
        public string Name { get { return "FrbUi Progress Indicator"; } set { throw new NotImplementedException(); } }

        #region Animations

        public AnimationChainList BackgroundAnimationChains
        {
            get { return _backgroundSprite.AnimationChains; } 
            set { _backgroundSprite.AnimationChains = value; }
        }

        public AnimationChainList IndicatorAnimationChains
        {
            get { return _foregroundSprite.AnimationChains; }
            set { _foregroundSprite.AnimationChains = value; }
        }

        public string CurrentBackgroundAnimationChainName
        {
            get { return _backgroundSprite.CurrentChainName; }
            set
            {
                _backgroundSprite.CurrentChainName = value;
                _backgroundSprite.Alpha = _backgroundSprite.Texture == null ? 0 : _alpha;
            }
        }

        public string CurrentIndicatorAnimationChainName
        {
            get { return _foregroundSprite.CurrentChainName; }
            set
            {
                _foregroundSprite.CurrentChainName = value;
            }
        }

        #endregion

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

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;

                _backgroundSprite.Alpha = _backgroundSprite.Texture == null
                    ? 0
                    : value;
            }
        }

        public int MaxValue { get; set; }

        public int Value
        {
            get { return _currentValue; }
            set
            {
                if (value > MaxValue)
                    throw new ArgumentException("The value specified is greater than the maximum value");

                if (value < 0)
                    throw new ArgumentException("The value specified is less than zero, the minimum value");

                var startScale = _foregroundSprite.ScaleX;
                _currentValue = value;
                var ratio = _currentValue / (float)MaxValue;
                _foregroundSprite.RightTextureCoordinate = ratio;

                // Reset position to left justify
                _foregroundSprite.RelativeX = (_foregroundSprite.RelativeX - startScale) + _foregroundSprite.ScaleX;

            }
        }
        
        #endregion

        #region Methods

        public ProgressIndicator()
        {
            _alpha = 1;
            MaxValue = 100;
            _currentValue = 100;

            _backgroundSprite = new Sprite
            {
                PixelSize = 0.5f,
                Alpha = 0f
            };

            _foregroundSprite = new Sprite
            {
                PixelSize = 0.5f,
                Alpha = _alpha
            };

            _foregroundSprite.AttachTo(_backgroundSprite, true);
            _foregroundSprite.RelativeZ = 0.01f;
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
            SpriteManager.AddSprite(_backgroundSprite);
            SpriteManager.AddToLayer(_backgroundSprite, layer);

            SpriteManager.AddSprite(_foregroundSprite);
            SpriteManager.AddToLayer(_foregroundSprite, layer);

            Layer = layer;
        }

        public void Destroy()
        {
            SpriteManager.RemoveSprite(_backgroundSprite);
            SpriteManager.RemoveSprite(_foregroundSprite);
        }

        public void UpdateDependencies(double currentTime)
        {
            _backgroundSprite.UpdateDependencies(currentTime);
        }

        public void ForceUpdateDependencies()
        {
            _backgroundSprite.ForceUpdateDependencies();
        }

        #endregion
    }
}
