using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    [XmlRoot("Assets")]
    public class AssetCollection
    {
        [XmlArray]
        [XmlArrayItem("BoxLayout", typeof(BoxLayout))]
        [XmlArrayItem("Button", typeof(Button))]
        public List<AssetBase> Assets { get; set; } 
    }
}
