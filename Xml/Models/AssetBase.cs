using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public abstract class AssetBase
    {
        #region XML Values

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public float ScaleX { get; set; }

        [XmlAttribute]
        public float ScaleY { get; set; }

        [XmlAttribute]
        public bool Visible { get; set; }

        [XmlIgnore]
        public float? RelativeX { get; set; }

        [XmlIgnore]
        public bool RelativeXValueSpecified
        {
            get { return RelativeX.HasValue; }
        }

        [XmlIgnore]
        public float? RelativeY { get; set; }

        [XmlIgnore]
        public bool RelativeYValueSpecified
        {
            get { return RelativeY.HasValue; }
        }

        [XmlIgnore]
        public float? RelativeZ { get; set; }

        [XmlIgnore]
        public bool RelativeZValueSpecified
        {
            get { return RelativeZ.HasValue; }
        }

        [XmlAttribute("RelativeX")]
        public float RelativeXValue
        {
            get { return RelativeX ?? default(float); }
            set { RelativeX = value; }
        }

        [XmlAttribute("RelativeY")]
        public float RelativeYValue
        {
            get { return RelativeY ?? default(float); }
            set { RelativeY = value; }
        }

        [XmlAttribute("RelativeZ")]
        public float RelativeZValue
        {
            get { return RelativeZ ?? default(float); }
            set { RelativeZ = value; }
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

            layoutable.ScaleX = ScaleX;
            layoutable.ScaleY = ScaleY;
            layoutable.Visible = Visible;
            layoutable.RelativeX = RelativeXValue;
            layoutable.RelativeY = RelativeYValue;
            layoutable.RelativeZ = RelativeZValue;

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
