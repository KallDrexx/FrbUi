using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public abstract class AssetBase
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public float RelativeX { get; set; }
        [XmlAttribute] public float RelativeY { get; set; }
        [XmlAttribute] public float RelativeZ { get; set; }
        [XmlAttribute] public float ScaleX { get; set; }
        [XmlAttribute] public float ScaleY { get; set; }
        [XmlAttribute] public bool Visible { get; set; }

        protected AssetBase()
        {
            Visible = true;
        }

        public abstract ILayoutable GenerateILayoutable();

        protected void SetBaseILayoutableProperties(ILayoutable layoutable)
        {
            if (layoutable == null)
                throw new ArgumentNullException("layoutable");

            layoutable.ScaleX = ScaleX;
            layoutable.ScaleY = ScaleY;
            layoutable.Visible = Visible;
        }
    }
}
