using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public abstract class AssetBase
    {
        #region XML

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Tag { get; set; }

        [XmlIgnore]
        public float? ScaleX { get; set; }

        [XmlIgnore]
        public bool ScaleXValueSpecified { get { return ScaleX.HasValue; } }

        [XmlAttribute("ScaleX")]
        public float ScaleXValue
        {
            get { return ScaleX ?? default(float); }
            set { ScaleX = value; }
        }

        [XmlIgnore]
        public float? ScaleY { get; set; }

        [XmlIgnore]
        public bool ScaleYValueSpecified { get { return ScaleY.HasValue; } }

        [XmlAttribute("ScaleY")]
        public float ScaleYValue
        {
            get { return ScaleY ?? default(float); }
            set { ScaleY = value; }
        }

        [XmlIgnore]
        public bool? Visible { get; set; }

        [XmlIgnore]
        public bool VisibleValueSpecified { get { return Visible.HasValue; } }

        [XmlAttribute("Visible")]
        public bool VisibleValue
        {
            get { return Visible ?? default(bool); }
            set { Visible = value; }
        }

        [XmlIgnore]
        public float? RelativeX { get; set; }

        [XmlIgnore]
        public bool RelativeXValueSpecified { get { return RelativeX.HasValue; } }

        [XmlAttribute("RelativeX")]
        public float RelativeXValue
        {
            get { return RelativeX ?? default(float); }
            set { RelativeX = value; }
        }

        [XmlIgnore]
        public float? RelativeY { get; set; }

        [XmlIgnore]
        public bool RelativeYValueSpecified { get { return RelativeY.HasValue; } }

        [XmlAttribute("RelativeY")]
        public float RelativeYValue
        {
            get { return RelativeY ?? default(float); }
            set { RelativeY = value; }
        }

        [XmlIgnore]
        public float? RelativeZ { get; set; }

        [XmlIgnore]
        public bool RelativeZValueSpecified { get { return RelativeZ.HasValue; } }

        [XmlAttribute("RelativeZ")]
        public float RelativeZValue
        {
            get { return RelativeZ ?? default(float); }
            set { RelativeX = value; }
        }

        #endregion
        
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

            layoutable.ScaleX = ScaleX ?? 0f;
            layoutable.ScaleY = ScaleY ?? 0f;
            layoutable.Visible = Visible ?? false;
            layoutable.RelativeX = RelativeX ?? 0f;
            layoutable.RelativeY = RelativeY ?? 0f;
            layoutable.RelativeZ = RelativeZ ?? 0f;
            layoutable.Tag = Tag;

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
