﻿using System.Collections.Generic;
using System.Xml.Serialization;
using FrbUi.Xml.Models.Controls;
using FrbUi.Xml.Models.Layouts;
using FrbUi.Xml.Models.SelectableGroupings;

namespace FrbUi.Xml.Models
{
    [XmlRoot("UserInterfacePackage")]
    public class AssetCollection
    {
        [XmlArray]
        [XmlArrayItem("BoxLayout", typeof(BoxLayout))]
        [XmlArrayItem("GridLayout", typeof(GridLayout))]
        [XmlArrayItem("CircularLayout", typeof(CircularLayout))]
        [XmlArrayItem("SimpleLayout", typeof(SimpleLayout))]
        [XmlArrayItem("Button", typeof(Button))]
        [XmlArrayItem("LayoutableSprite", typeof(LayoutableSprite))]
        [XmlArrayItem("LayoutableText", typeof(LayoutableText))]
        public List<AssetBase> Controls { get; set; }

        [XmlArray]
        public List<SelectableGroup> SelectableGroups { get; set; }
    }
}