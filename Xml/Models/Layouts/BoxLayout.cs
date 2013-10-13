using System.Collections.Generic;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models.Layouts
{
    public class BoxLayout : SelectableAssetBase
    {
        #region XML

        [XmlIgnore]
        public float? Spacing { get; set; }

        [XmlIgnore]
        public bool SpacingValueSpecified { get { return Spacing.HasValue; } }

        [XmlAttribute("Spacing")]
        public float SpacingValue
        {
            get { return Spacing ?? default(float); }
            set { Spacing = value; }
        }

        [XmlIgnore]
        public float? Margin { get; set; }

        [XmlIgnore]
        public bool MarginValueSpecified { get { return Margin.HasValue; } }

        [XmlAttribute("Margin")]
        public float MarginValue
        {
            get { return Margin ?? default(float); }
            set { Margin = value; }
        }

        [XmlIgnore]
        public LayoutDirection? Direction { get; set; }

        [XmlIgnore]
        public bool DirectionValueSpecified { get { return Direction.HasValue; } }

        [XmlAttribute("Direction")]
        public LayoutDirection DirectionValue
        {
            get { return Direction ?? default(LayoutDirection); }
            set { Direction = value; }
        }

        [XmlIgnore]
        public float? BackgroundAlpha { get; set; }

        [XmlIgnore]
        public bool BackgroundAlphaValueSpecified { get { return BackgroundAlpha.HasValue; } }

        [XmlAttribute("BackgroundAlpha")]
        public float BackgroundAlphaValue
        {
            get { return BackgroundAlpha ?? default(float); }
            set { BackgroundAlpha = value; }
        }

        [XmlAttribute]
        public string BackgroundAchxFile { get; set; }

        [XmlArray]
        public List<BoxLayoutChild> Children { get; set; }

        #endregion


        public enum LayoutDirection
        {
            [XmlEnum] Up,
            [XmlEnum] Down,
            [XmlEnum] Left,
            [XmlEnum] Right
        }

        public BoxLayout()
        {
            BackgroundAlpha = 1f;
        }

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls, Dictionary<string, BitmapFont> namedFonts)
        {
            var layout = UiControlManager.Instance.CreateControl<FrbUi.Layouts.BoxLayout>();
            SetBaseILayoutableProperties(layout, namedControls);
            layout.Spacing = Spacing ?? 0f;
            layout.Margin = Margin ?? 0f;
            layout.BackgroundAlpha = BackgroundAlpha ?? 1f;

            if (!string.IsNullOrWhiteSpace(BackgroundAchxFile))
                layout.BackgroundAnimationChains = FlatRedBallServices.Load<AnimationChainList>(BackgroundAchxFile, contentManagerName);

            SetupSelectableProperties(layout);

            switch (Direction)
            {
                case LayoutDirection.Up:
                    layout.CurrentDirection = FrbUi.Layouts.BoxLayout.Direction.Up;
                    break;

                case LayoutDirection.Down:
                    layout.CurrentDirection = FrbUi.Layouts.BoxLayout.Direction.Down;
                    break;

                case LayoutDirection.Left:
                    layout.CurrentDirection = FrbUi.Layouts.BoxLayout.Direction.Left;
                    break;

                case LayoutDirection.Right:
                    layout.CurrentDirection = FrbUi.Layouts.BoxLayout.Direction.Right;
                    break;
            }

            foreach (var child in Children)
            {
                FrbUi.Layouts.BoxLayout.Alignment childAlignment;
                switch (child.ItemAlignment)
                {
                    case BoxLayoutChild.Alignment.Centered:
                        childAlignment = FrbUi.Layouts.BoxLayout.Alignment.Centered;
                        break;

                    case BoxLayoutChild.Alignment.Inverse:
                        childAlignment = FrbUi.Layouts.BoxLayout.Alignment.Inverse;
                        break;

                    default:
                        childAlignment = FrbUi.Layouts.BoxLayout.Alignment.Default;
                        break;
                }

                var childLayoutable = child.Item.GenerateILayoutable(contentManagerName, namedControls, namedFonts);
                layout.AddItem(childLayoutable, childAlignment);
            }

            return layout;
        }
    }
}
