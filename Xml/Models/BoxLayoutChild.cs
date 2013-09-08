using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public class BoxLayoutChild : LayoutableChildBase
    {
        [XmlIgnore]
        public Alignment? ItemAlignment { get; set; }

        [XmlIgnore]
        public bool ItemAlignmentValueSpecified { get { return ItemAlignment.HasValue; } }

        [XmlAttribute("ItemAlignment")]
        public Alignment ItemAlignmentValue
        {
            get { return ItemAlignment ?? default(Alignment); }
            set { ItemAlignment = value; }
        }

        public enum Alignment
        {
            [XmlEnum] Default,
            [XmlEnum] Inverse,
            [XmlEnum] Centered
        }
    }
}
