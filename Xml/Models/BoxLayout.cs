using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public class BoxLayout : AssetBase
    {
        [XmlAttribute] public float Spacing { get; set; }
        [XmlAttribute] public float Margin { get; set; }
        [XmlAttribute] public LayoutDirection CurrentDirection { get; set; }
        [XmlAttribute] public float BackgroundAlpha { get; set; }
        [XmlAttribute] public string BackgroundAnimationChains { get; set; }
        [XmlAttribute] public string InitialBackgroundAnimationChainName { get; set; }

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

        public override ILayoutable GenerateILayoutable()
        {
            var layout = UiControlManager.Instance.CreateControl<Layouts.BoxLayout>();
            SetBaseILayoutableProperties(layout);
            layout.Spacing = Spacing;
            layout.Margin = Margin;
            layout.BackgroundAlpha = BackgroundAlpha;

            // TODO: Add animation chains somehow

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

                var childLayoutable = child.Item.GenerateILayoutable();
                layout.AddItem(childLayoutable, childAlignment);
            }

            return layout;
        }
    }
}
