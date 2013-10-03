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
        [XmlElement("BoxLayout", typeof(BoxLayout))]
        [XmlElement("GridLayout", typeof(GridLayout))]
        [XmlElement("CircularLayout", typeof(CircularLayout))]
        [XmlElement("SimpleLayout", typeof(SimpleLayout))]
        [XmlElement("Button", typeof(Button))]
        [XmlElement("LayoutableSprite", typeof(LayoutableSprite))]
        [XmlElement("LayoutableText", typeof(LayoutableText))]
        public AssetBase Item { get; set; }
    }
}
