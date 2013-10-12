using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FrbUi.Positioning;

namespace FrbUi.Xml.Models.Layouts
{
    public class SimpleLayout : AssetBase
    {
        #region Values

        private bool? _isFullScreen;
        private List<SimpleLayoutChild> _children;

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

        [XmlArray]
        public List<SimpleLayoutChild> Children
        {
            get { return _children; }
            set { _children = value; }
        } 

        #endregion

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls)
        {
            var layout = UiControlManager.Instance.CreateControl<FrbUi.Layouts.SimpleLayout>();
            SetBaseILayoutableProperties(layout, namedControls);
            layout.FullScreen = _isFullScreen ?? false;

            foreach (var child in _children)
            {
                var item = child.Item.GenerateILayoutable(contentManagerName, namedControls);
                HorizontalPosition horizontalPosition;
                VerticalPosition verticalPosition;
                LayoutOrigin origin;

                // Figure out the origin
                switch (child.OriginValue)
                {
                    case SimpleLayoutChild.LayoutOrigin.TopLeft:
                        origin = LayoutOrigin.TopLeft;
                        break;

                    case SimpleLayoutChild.LayoutOrigin.TopRight:
                        origin = LayoutOrigin.TopRight;
                        break;

                    case SimpleLayoutChild.LayoutOrigin.BottomLeft:
                        origin = LayoutOrigin.BottomLeft;
                        break;

                    case SimpleLayoutChild.LayoutOrigin.BottomRight:
                        origin = LayoutOrigin.BottomRight;
                        break;

                    case SimpleLayoutChild.LayoutOrigin.BottomCenter:
                        origin = LayoutOrigin.BottomCenter;
                        break;

                    case SimpleLayoutChild.LayoutOrigin.TopCenter:
                        origin = LayoutOrigin.TopCenter;
                        break;

                    case SimpleLayoutChild.LayoutOrigin.Center:
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
