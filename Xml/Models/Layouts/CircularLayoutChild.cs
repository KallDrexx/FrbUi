using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models.Layouts
{
    public class CircularLayoutChild : LayoutableChildBase
    {
        private float? _radiusOffset;
        private float? _degreeOffset;

        [XmlIgnore]
        public bool RadiusOffsetValueSpecified { get { return _radiusOffset.HasValue; } }

        [XmlAttribute("RadiusOffset")]
        public float RadiusOffsetValue
        {
            get { return _radiusOffset ?? default(float); }
            set { _radiusOffset = value; }
        }

        [XmlIgnore]
        public bool DegreeOffsetValueSpecified { get { return _degreeOffset.HasValue; } }

        [XmlAttribute("DegreeOffset")]
        public float DegreeOffsetValue
        {
            get { return _degreeOffset ?? default(float); }
            set { _degreeOffset = value; }
        }
    }
}
