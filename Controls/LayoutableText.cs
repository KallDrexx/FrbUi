using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Graphics;
using FlatRedBall;

namespace FrbUi.Controls
{
    public class LayoutableText : ILayoutable
    {
        private readonly Text _text;

        #region Properties

        public ILayoutableEvent OnSizeChangeHandler { get; set; }
        public Layer Layer { get; private set; }

        public string DisplayText
        {
            get { return _text.DisplayText; }
            set 
            { 
                _text.DisplayText = value;
                if (OnSizeChangeHandler != null)
                    OnSizeChangeHandler(this);
            }
        }

        public float RelativeX
        {
            get { return _text.RelativeX; }
            set { _text.RelativeX = value; }
        }

        public float RelativeY
        {
            get { return _text.RelativeY; }
            set { _text.RelativeY = value; }
        }

        public float Alpha
        {
            get { return _text.Alpha; }
            set { _text.Alpha = value; }
        }

        public bool AbsoluteVisible
        {
            get { return _text.AbsoluteVisible; }
        }

        public bool IgnoresParentVisibility
        {
            get { return _text.IgnoresParentVisibility; }
            set { _text.IgnoresParentVisibility = value; }
        }

        public IVisible Parent
        {
            get { return _text.Parent as IVisible; }
        }

        public bool Visible
        {
            get { return _text.Visible; }
            set { _text.Visible = value; }
        }

        public float ScaleX
        {
            get { return _text.ScaleX; }
            set
            {
                /* Texts can't be resized */
            }
        }

        public float ScaleXVelocity
        {
            get { return 0; }
            set
            {
                /* Text objects don't have scale velocity */
            }
        }

        public float ScaleY
        {
            get { return _text.ScaleY; }
            set
            {
                /* Texts can't be resized */
            }
        }

        public float ScaleYVelocity
        {
            get { return 0; }
            set
            {
                /* Text objects don't have scale velocity */
            }
        }

        public float XAcceleration
        {
            get { return _text.XAcceleration; }
            set { _text.XAcceleration = value; }
        }

        public float XVelocity
        {
            get { return _text.XVelocity; }
            set { _text.XVelocity = value; }
        }

        public float YAcceleration
        {
            get { return YAcceleration; }
            set { _text.YAcceleration = value; }
        }

        public float YVelocity
        {
            get { return YVelocity; }
            set { _text.YVelocity = value; }
        }

        public float ZAcceleration
        {
            get { return _text.ZAcceleration; }
            set { _text.ZAcceleration = value; }
        }

        public float ZVelocity
        {
            get { return _text.ZVelocity; }
            set { _text.ZVelocity = value; }
        }

        public float X
        {
            get { return _text.X; }
            set { _text.X = value; }
        }

        public float Y
        {
            get { return _text.Y; }
            set { _text.Y = value; }
        }

        public float Z
        {
            get { return _text.Z; }
            set { _text.Z = value; }
        }

        #endregion

        #region Methods

        public LayoutableText()
        {
            _text = new Text
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        public void Activity()
        {
        }

        public void AttachTo(PositionedObject obj, bool changeRelative)
        {
            _text.AttachTo(obj, changeRelative);
        }

        public void AddToManagers(Layer layer)
        {
            TextManager.AddToLayer(_text, layer);
            _text.SetPixelPerfectScale(layer);
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
            TextManager.RemoveText(_text);
        }

        #endregion
    }
}
