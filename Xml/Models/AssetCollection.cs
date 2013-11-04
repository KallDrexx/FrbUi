using System.Collections.Generic;
using System.Xml.Serialization;
using FrbUi.Xml.Models.Controls;
using FrbUi.Xml.Models.Fonts;
using FrbUi.Xml.Models.Layouts;
using FrbUi.Xml.Models.SelectableGroupings;

namespace FrbUi.Xml.Models
{
    [XmlRoot("UserInterfacePackage")]
    public class AssetCollection
    {
        [XmlArray]
        [XmlArrayItem("BoxLayout", typeof(BoxLayoutXml))]
        [XmlArrayItem("GridLayout", typeof(GridLayoutXml))]
        [XmlArrayItem("CircularLayout", typeof(CircularLayoutXml))]
        [XmlArrayItem("SimpleLayout", typeof(SimpleLayoutXml))]
        [XmlArrayItem("Button", typeof(ButtonXml))]
        [XmlArrayItem("LayoutableSprite", typeof(LayoutableSpriteXml))]
        [XmlArrayItem("LayoutableText", typeof(LayoutableTextXml))]
        public List<AssetXmlBase> Controls { get; set; }

        [XmlArray]
        [XmlArrayItem("SelectableGroup", typeof(SelectableGroupXml))]
        public List<SelectableGroupXml> SelectableGroups { get; set; }

        [XmlArray]
        [XmlArrayItem("BitmapFont", typeof(BitmapFontXml))]
        public List<BitmapFontXml> BitmapFonts { get; set; } 
    }
}
