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
        [XmlAttribute] public float? RelativeX { get; set; }
        [XmlAttribute] public float RelativeY { get; set; }
        [XmlAttribute] public float RelativeZ { get; set; }
        [XmlAttribute] public float ScaleX { get; set; }
        [XmlAttribute] public float ScaleY { get; set; }
        [XmlAttribute] public bool Visible { get; set; }

        protected AssetBase()
        {
            Visible = true;
        }

        public abstract ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls);

        protected void SetBaseILayoutableProperties(ILayoutable layoutable, Dictionary<string, ILayoutable> namedControls)
        {
            if (layoutable == null)
                throw new ArgumentNullException("layoutable");

            if (namedControls == null)
                throw new ArgumentNullException("namedControls");

            layoutable.ScaleX = ScaleX;
            layoutable.ScaleY = ScaleY;
            layoutable.Visible = Visible;

            if (!string.IsNullOrWhiteSpace(Name))
            {
                if (namedControls.ContainsKey(Name))
                    throw new InvalidOperationException(
                        string.Format("Multiple controls are named {0}.  Each explicitely named control must have a unique name", Name));

                namedControls.Add(Name, layoutable);
            }
        }
    }
}
