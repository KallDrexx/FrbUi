using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models.Layouts
{
    public class SimpleLayoutXmlChild : LayoutableChildBase
    {
        public enum LayoutOrigin { Center, TopLeft, TopRight, BottomLeft, BottomRight, BottomCenter, TopCenter };

        #region Values

        private LayoutOrigin? _origin;
        private float? _horizontalPercentFromLeft;
        private float? _horizontalPercentFromCenter;
        private float? _horizontalPercentFromRight;
        private float? _horizontalOffsetFromLeft;
        private float? _horizontalOffsetFromCenter;
        private float? _horizontalOffsetFromRight;
        private float? _verticalPercentFromTop;
        private float? _verticalPercentFromCenter;
        private float? _verticalPercentFromBottom;
        private float? _verticalOffsetFromTop;
        private float? _verticalOffsetFromCenter;
        private float? _verticalOffsetFromBottom;

        #endregion

        #region XML

        [XmlIgnore]
        public bool OriginValueSpecified { get { return _origin.HasValue; } }

        [XmlAttribute("Origin")]
        public LayoutOrigin OriginValue
        {
            get { return _origin ?? default(LayoutOrigin); }
            set { _origin = value; }
        }

        [XmlIgnore]
        public bool HorizontalPercentFromLeftValueSpecified { get { return _horizontalPercentFromLeft.HasValue; } }

        [XmlAttribute("HorizontalPercentFromLeft")]
        public float HorizontalPercentFromLeftValue
        {
            get { return _horizontalPercentFromLeft ?? default(float); }
            set { _horizontalPercentFromLeft = value; }
        }

        [XmlIgnore]
        public bool HorizontalPercentFromCenterValueSpecified { get { return _horizontalPercentFromCenter.HasValue; } }

        [XmlAttribute("HorizontalPercentFromCenter")]
        public float HorizontalPercentFromCenterValue
        {
            get { return _horizontalPercentFromCenter ?? default(float); }
            set { _horizontalPercentFromCenter = value; }
        }

        [XmlIgnore]
        public bool HorizontalPercentFromRightValueSpecified { get { return _horizontalPercentFromRight.HasValue; } }

        [XmlAttribute("HorizontalPercentFromRight")]
        public float HorizontalPercentFromRightValue
        {
            get { return _horizontalPercentFromRight ?? default(float); }
            set { _horizontalPercentFromRight = value; }
        }

        [XmlIgnore]
        public bool HorizontalOffsetFromLeftValueSpecified { get { return _horizontalOffsetFromLeft.HasValue; } }

        [XmlAttribute("HorizontalOffsetFromLeft")]
        public float HorizontalOffsetFromLeftValue
        {
            get { return _horizontalOffsetFromLeft ?? default(float); }
            set { _horizontalOffsetFromLeft = value; }
        }

        [XmlIgnore]
        public bool HorizontalOffsetFromCenterValueSpecified { get { return _horizontalOffsetFromCenter.HasValue; } }

        [XmlAttribute("HorizontalOffsetFromCenter")]
        public float HorizontalOffsetFromCenterValue
        {
            get { return _horizontalOffsetFromCenter ?? default(float); }
            set { _horizontalOffsetFromCenter = value; }
        }

        [XmlIgnore]
        public bool HorizontalOffsetFromRightValueSpecified { get { return _horizontalOffsetFromRight.HasValue; } }

        [XmlAttribute("HorizontalOffsetFromRight")]
        public float HorizontalOffsetFromRightValue
        {
            get { return _horizontalOffsetFromRight ?? default(float); }
            set { _horizontalOffsetFromRight = value; }
        }

        [XmlIgnore]
        public bool VerticalPercentFromTopValueSpecified { get { return _verticalPercentFromTop.HasValue; } }

        [XmlAttribute("VerticalPercentFromTop")]
        public float VerticalPercentFromTopValue
        {
            get { return _verticalPercentFromTop ?? default(float); }
            set { _verticalPercentFromTop = value; }
        }

        [XmlIgnore]
        public bool VerticalPercentFromCenterValueSpecified { get { return _verticalPercentFromCenter.HasValue; } }

        [XmlAttribute("VerticalPercentFromCenter")]
        public float VerticalPercentFromCenterValue
        {
            get { return _verticalPercentFromCenter ?? default(float); }
            set { _verticalPercentFromCenter = value; }
        }

        [XmlIgnore]
        public bool VerticalPercentFromBottomValueSpecified { get { return _verticalPercentFromBottom.HasValue; } }

        [XmlAttribute("VerticalPercentFromBottom")]
        public float VerticalPercentFromBottomValue
        {
            get { return _verticalPercentFromBottom ?? default(float); }
            set { _verticalPercentFromBottom = value; }
        }

        [XmlIgnore]
        public bool VerticalOffsetFromTopValueSpecified { get { return _verticalOffsetFromTop.HasValue; } }

        [XmlAttribute("VerticalOffsetFromTop")]
        public float VerticalOffsetFromTopValue
        {
            get { return _verticalOffsetFromTop ?? default(float); }
            set { _verticalOffsetFromTop = value; }
        }

        [XmlIgnore]
        public bool VerticalOffsetFromCenterValueSpecified { get { return _verticalOffsetFromCenter.HasValue; } }

        [XmlAttribute("VerticalOffsetFromCenter")]
        public float VerticalOffsetFromCenterValue
        {
            get { return _verticalOffsetFromCenter ?? default(float); }
            set { _verticalOffsetFromCenter = value; }
        }

        [XmlIgnore]
        public bool VerticalOffsetFromBottomValueSpecified { get { return _verticalOffsetFromBottom.HasValue; } }

        [XmlAttribute("VerticalOffsetFromBottom")]
        public float VerticalOffsetFromBottomValue
        {
            get { return _verticalOffsetFromBottom ?? default(float); }
            set { _verticalOffsetFromBottom = value; }
        }

        #endregion
    }
}
