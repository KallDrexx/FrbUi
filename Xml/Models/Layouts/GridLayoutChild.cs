using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models.Layouts
{
    public class GridLayoutChild : LayoutableChildBase
    {
        public enum HorizontalAlignments { Left, Center, Right }
        public enum VerticalAlignments { Top, Center, Bottom }

        [XmlAttribute]
        public int RowIndex { get; set; }

        [XmlAttribute]
        public int ColumnIndex { get; set; }

        [XmlIgnore]
        public HorizontalAlignments? HorizontalAlignment { get; set; }

        [XmlIgnore]
        public bool HorizontalAlignmentValueSpecified { get { return HorizontalAlignment.HasValue; } }

        [XmlAttribute("HorizontalAlignment")]
        public HorizontalAlignments HorizontalAlignmentValue
        {
            get { return HorizontalAlignment ?? default(HorizontalAlignments); }
            set { HorizontalAlignment = value; }
        }

        [XmlIgnore]
        public VerticalAlignments? VerticalAlignment { get; set; }

        [XmlIgnore]
        public bool VerticalAlignmentValueSpecified { get { return VerticalAlignment.HasValue; } }

        [XmlAttribute("VerticalAlignment")]
        public VerticalAlignments VerticalAlignmentValue
        {
            get { return VerticalAlignment ?? default(VerticalAlignments); }
            set { VerticalAlignment = value; }
        }
    }
}
