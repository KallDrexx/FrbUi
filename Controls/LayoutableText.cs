using FlatRedBall.Graphics;
using FlatRedBall;

namespace FrbUi.Controls
{
    public class LayoutableText : ILayoutable
    {
        private readonly Text _text;

        #region Properties

        public LayoutableEvent OnSizeChangeHandler { get; set; }
        public LayoutableEvent OnAddedToLayout { get; set; }

        public Layer Layer { get; private set; }
        public ILayoutable ParentLayout { get; set; }
        public string Tag { get; set; }

        public string DisplayText
        {
            get { return _text.DisplayText; }
            set
            {
                _text.DisplayText = value;
                if (OnSizeChangeHandler != null)
                    OnSizeChangeHandler(this);

                // Reset the scaling
                _text.SetPixelPerfectScale(Layer);
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

        public float RelativeZ
        {
            get { return _text.RelativeZ; }
            set { _text.RelativeZ = value; }
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

        public float Red
        {
            get { return _text.Red; }
            set { _text.Red = value; }
        }

        public float Green
        {
            get { return _text.Green; }
            set { _text.Green = value; }
        }

        public float Blue
        {
            get { return _text.Blue; }
            set { _text.Blue = value; }
        }

        public BitmapFont Font
        {
            get { return _text.Font; }
            set { _text.Font = value; }
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

        public void AttachObject(PositionedObject obj, bool changeRelative)
        {
            obj.AttachTo(_text, changeRelative);
        }

        public void AttachTo(ILayoutable obj, bool changeRelative)
        {
            obj.AttachObject(_text, changeRelative);
        }

        public void AttachObject(ILayoutable obj, bool changeRelatative)
        {
            obj.AttachTo(_text, changeRelatative);
        }

        public void Detach()
        {
            _text.Detach();
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

        public ILayoutable Clone()
        {
            var clonedItem = UiControlManager.Instance.CreateControl<LayoutableText>();
            clonedItem.Alpha = Alpha;
            clonedItem.Blue = Blue;
            clonedItem.Green = Green;
            clonedItem.Red = Red;
            clonedItem.DisplayText = DisplayText;
            clonedItem.IgnoresParentVisibility = IgnoresParentVisibility;
            clonedItem.Layer = Layer;
            clonedItem.OnAddedToLayout = OnAddedToLayout;
            clonedItem.OnSizeChangeHandler = OnSizeChangeHandler;
            clonedItem.ScaleX = ScaleX;
            clonedItem.ScaleY = ScaleY;
            clonedItem.Tag = Tag;
            clonedItem.Visible = Visible;

            return clonedItem;
        }

        #endregion
    }
}
