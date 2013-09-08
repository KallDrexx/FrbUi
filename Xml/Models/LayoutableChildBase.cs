﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FrbUi.Xml.Models.Controls;
using FrbUi.Xml.Models.Layouts;

namespace FrbUi.Xml.Models
{
    public abstract class LayoutableChildBase
    {
        [XmlElement("BoxLayout", typeof(BoxLayout))]
        [XmlElement("Button", typeof(Button))]
        public AssetBase Item { get; set; }
    }
}
