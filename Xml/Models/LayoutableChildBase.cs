using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FrbUi.Xml.Models.Controls;
using FrbUi.Xml.Models.Layouts;
using FrbUi.Xml.Models.SelectableGroupings;

namespace FrbUi.Xml.Models
{
    public abstract class LayoutableChildBase
    {
        [XmlElement("BoxLayout", typeof(BoxLayoutXml))]
        [XmlElement("GridLayout", typeof(GridLayoutXml))]
        [XmlElement("CircularLayout", typeof(CircularLayoutXml))]
        [XmlElement("SimpleLayout", typeof(SimpleLayoutXml))]
        [XmlElement("Button", typeof(ButtonXml))]
        [XmlElement("LayoutableSprite", typeof(LayoutableSpriteXml))]
        [XmlElement("LayoutableText", typeof(LayoutableTextXml))]
        public AssetXmlBase Item { get; set; }
    }
}
