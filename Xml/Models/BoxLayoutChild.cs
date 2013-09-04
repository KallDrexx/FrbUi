using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public class BoxLayoutChild : LayoutableChildBase
    {
        [XmlAttribute]
        public Alignment ItemAlignment { get; set; }

        public enum Alignment
        {
            [XmlEnum] Default,
            [XmlEnum] Inverse,
            [XmlEnum] Centered
        }
    }
}
