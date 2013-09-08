using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models
{
    public class BoxLayout : SelectableAssetBase
    {
        [XmlAttribute] public float Spacing { get; set; }
        [XmlAttribute] public float Margin { get; set; }
        [XmlAttribute] public LayoutDirection CurrentDirection { get; set; }
        [XmlAttribute] public float BackgroundAlpha { get; set; }
        [XmlAttribute] public string BackgroundAchxFile { get; set; }

        [XmlArray] public List<BoxLayoutChild> Children { get; set; }

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

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls)
        {
            var layout = UiControlManager.Instance.CreateControl<Layouts.BoxLayout>();
            SetBaseILayoutableProperties(layout, namedControls);
            layout.Spacing = Spacing;
            layout.Margin = Margin;
            layout.BackgroundAlpha = BackgroundAlpha;
            layout.BackgroundAnimationChains = FlatRedBallServices.Load<AnimationChainList>(BackgroundAchxFile, contentManagerName);
            SetupSelectableProperties(layout);

            switch (CurrentDirection)
            {
                case LayoutDirection.Up:
                    layout.CurrentDirection = Layouts.BoxLayout.Direction.Up;
                    break;

                case LayoutDirection.Down:
                    layout.CurrentDirection = Layouts.BoxLayout.Direction.Down;
                    break;

                case LayoutDirection.Left:
                    layout.CurrentDirection = Layouts.BoxLayout.Direction.Left;
                    break;

                case LayoutDirection.Right:
                    layout.CurrentDirection = Layouts.BoxLayout.Direction.Right;
                    break;
            }

            foreach (var child in Children)
            {
                Layouts.BoxLayout.Alignment childAlignment;
                switch (child.ItemAlignment)
                {
                    case BoxLayoutChild.Alignment.Centered:
                        childAlignment = Layouts.BoxLayout.Alignment.Centered;
                        break;

                    case BoxLayoutChild.Alignment.Inverse:
                        childAlignment = Layouts.BoxLayout.Alignment.Inverse;
                        break;

                    default:
                        childAlignment = Layouts.BoxLayout.Alignment.Default;
                        break;
                }

                var childLayoutable = child.Item.GenerateILayoutable(contentManagerName, namedControls);
                layout.AddItem(childLayoutable, childAlignment);
            }

            return layout;
        }
    }
}
