using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models.Layouts
{
    public class GridLayoutXml : SelectableAssetBase
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
        public float? Alpha { get; set; }

        [XmlIgnore]
        public bool AlphaValueSpecified { get { return Alpha.HasValue; } }

        [XmlAttribute("Alpha")]
        public float AlphaValue
        {
            get { return Alpha ?? default(float); }
            set { Alpha = value; }
        }

        [XmlAttribute]
        public string BackgroundAchxFile { get; set; }

        [XmlElement("Child")]
        public List<GridLayoutXmlChild> Children { get; set; } 

        #endregion

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls, Dictionary<string, BitmapFont> namedFonts)
        {
            var layout = UiControlManager.Instance.CreateControl<FrbUi.Layouts.GridLayout>();
            SetBaseILayoutableProperties(layout, namedControls);
            layout.Spacing = Spacing ?? 0f;
            layout.Margin = Margin ?? 0f;
            layout.Alpha = Alpha ?? 1f;

            if (!string.IsNullOrWhiteSpace(BackgroundAchxFile))
                layout.BackgroundAnimationChains = FlatRedBallServices.Load<AnimationChainList>(BackgroundAchxFile, contentManagerName);

            SetupSelectableProperties(layout);

            foreach (var child in Children.Where(x => x.Item != null))
            {
                FrbUi.Layouts.GridLayout.HorizontalAlignment horizontalAlignment;
                FrbUi.Layouts.GridLayout.VerticalAlignment verticalAlignment;

                switch (child.HorizontalAlignment)
                {
                    case GridLayoutXmlChild.HorizontalAlignments.Center:
                        horizontalAlignment = FrbUi.Layouts.GridLayout.HorizontalAlignment.Center;
                        break;

                    case GridLayoutXmlChild.HorizontalAlignments.Right:
                        horizontalAlignment = FrbUi.Layouts.GridLayout.HorizontalAlignment.Right;
                        break;

                    default:
                        horizontalAlignment = FrbUi.Layouts.GridLayout.HorizontalAlignment.Left;
                        break;
                }

                switch (child.VerticalAlignment)
                {
                    case GridLayoutXmlChild.VerticalAlignments.Center:
                        verticalAlignment = FrbUi.Layouts.GridLayout.VerticalAlignment.Center;
                        break;

                    case GridLayoutXmlChild.VerticalAlignments.Bottom:
                        verticalAlignment = FrbUi.Layouts.GridLayout.VerticalAlignment.Bottom;
                        break;

                    default:
                        verticalAlignment = FrbUi.Layouts.GridLayout.VerticalAlignment.Top;
                        break;
                }

                var item = child.Item.GenerateILayoutable(contentManagerName, namedControls, namedFonts);
                layout.AddItem(item, child.RowIndex, child.ColumnIndex, horizontalAlignment, verticalAlignment);
            }

            return layout;
        }
    }
}
