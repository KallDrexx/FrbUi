using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall.Graphics;
using FrbUi.Positioning;

namespace FrbUi.Xml.Models.Layouts
{
    public class SimpleLayoutXml : AssetXmlBase
    {
        #region Values

        private bool? _isFullScreen;
        private List<SimpleLayoutXmlChild> _children;

        #endregion

        #region XML

        [XmlIgnore]
        public bool IsFullScreenValueSpecified { get { return _isFullScreen.HasValue; } }

        [XmlAttribute("IsFullScreen")]
        public bool IsFullScreenValue
        {
            get { return _isFullScreen ?? default(bool); }
            set { _isFullScreen = value; }
        }

        [XmlElement("Child")]
        public List<SimpleLayoutXmlChild> Children
        {
            get { return _children; }
            set { _children = value; }
        } 

        #endregion

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls, Dictionary<string, BitmapFont> namedFonts)
        {
            var layout = UiControlManager.Instance.CreateControl<FrbUi.Layouts.SimpleLayout>();
            SetBaseILayoutableProperties(layout, namedControls);
            layout.FullScreen = _isFullScreen ?? false;

            foreach (var child in _children)
            {
                var item = child.Item.GenerateILayoutable(contentManagerName, namedControls, namedFonts);
                HorizontalPosition horizontalPosition;
                VerticalPosition verticalPosition;
                LayoutOrigin origin;

                // Figure out the origin
                switch (child.OriginValue)
                {
                    case SimpleLayoutXmlChild.LayoutOrigin.TopLeft:
                        origin = LayoutOrigin.TopLeft;
                        break;

                    case SimpleLayoutXmlChild.LayoutOrigin.TopRight:
                        origin = LayoutOrigin.TopRight;
                        break;

                    case SimpleLayoutXmlChild.LayoutOrigin.BottomLeft:
                        origin = LayoutOrigin.BottomLeft;
                        break;

                    case SimpleLayoutXmlChild.LayoutOrigin.BottomRight:
                        origin = LayoutOrigin.BottomRight;
                        break;

                    case SimpleLayoutXmlChild.LayoutOrigin.BottomCenter:
                        origin = LayoutOrigin.BottomCenter;
                        break;

                    case SimpleLayoutXmlChild.LayoutOrigin.TopCenter:
                        origin = LayoutOrigin.TopCenter;
                        break;

                    case SimpleLayoutXmlChild.LayoutOrigin.Center:
                    default:
                        origin = LayoutOrigin.Center;
                        break;
                }

                // Figure out horizontal position
                if (child.HorizontalPercentFromCenterValueSpecified)
                    horizontalPosition = HorizontalPosition.PercentFromCenter(child.HorizontalPercentFromCenterValue);
                else if (child.HorizontalPercentFromLeftValueSpecified)
                    horizontalPosition = HorizontalPosition.PercentFromLeft(child.HorizontalPercentFromLeftValue);
                else if (child.HorizontalPercentFromRightValueSpecified)
                    horizontalPosition = HorizontalPosition.PercentFromRight(child.HorizontalPercentFromRightValue);
                else if (child.HorizontalOffsetFromCenterValueSpecified)
                    horizontalPosition = HorizontalPosition.OffsetFromCenter(child.HorizontalOffsetFromCenterValue);
                else if (child.HorizontalOffsetFromLeftValueSpecified)
                    horizontalPosition = HorizontalPosition.OffsetFromLeft(child.HorizontalOffsetFromLeftValue);
                else if (child.HorizontalOffsetFromRightValueSpecified)
                    horizontalPosition = HorizontalPosition.OffsetFromRight(child.HorizontalOffsetFromRightValue);
                else
                    horizontalPosition = HorizontalPosition.OffsetFromLeft(0);

                // Figure out vertical position
                if (child.VerticalPercentFromCenterValueSpecified)
                    verticalPosition = VerticalPosition.PercentFromCenter(child.VerticalPercentFromCenterValue);
                else if (child.VerticalPercentFromTopValueSpecified)
                    verticalPosition = VerticalPosition.PercentFromTop(child.VerticalPercentFromTopValue);
                else if (child.VerticalPercentFromBottomValueSpecified)
                    verticalPosition = VerticalPosition.PercentFromBottom(child.VerticalPercentFromBottomValue);
                else if (child.VerticalOffsetFromCenterValueSpecified)
                    verticalPosition = VerticalPosition.OffsetFromCenter(child.VerticalOffsetFromCenterValue);
                else if (child.VerticalOffsetFromTopValueSpecified)
                    verticalPosition = VerticalPosition.OffsetFromTop(child.VerticalOffsetFromTopValue);
                else if (child.VerticalOffsetFromBottomValueSpecified)
                    verticalPosition = VerticalPosition.OffsetFromBottom(child.VerticalOffsetFromBottomValue);
                else
                    verticalPosition = VerticalPosition.OffsetFromTop(0);

                layout.AddItem(item, horizontalPosition, verticalPosition, origin);
            }

            return layout;
        }
    }
}
