using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models.Controls
{
    public class LayoutableSprite : AssetBase
    {
        [XmlAttribute]
        public string AchxName { get; set; }

        [XmlAttribute]
        public string InitialAnimationChainName { get; set; }

        [XmlIgnore]
        public float? RelativeRotationZDegrees { get; set; }

        [XmlIgnore]
        public bool RelativeRotationZDegreesValueSpecified { get { return RelativeRotationZDegrees.HasValue; } }

        [XmlAttribute("RelativeRotationZDegrees")]
        public float RelativeRotationZDegreesValue
        {
            get { return RelativeRotationZDegrees ?? default(float); }
            set { RelativeRotationZDegrees = value; }
        }

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls)
        {
            var control = UiControlManager.Instance.CreateControl<FrbUi.Controls.LayoutableSprite>();
            SetBaseILayoutableProperties(control, namedControls);

            // Since rotation value is relative, we need to set this in the after added event
            control.OnAddedToLayout += delegate {
                control.RelativeRotationZ = (float)(Math.PI * ((RelativeRotationZDegrees ?? 0) / 180));
                control.OnAddedToLayout = null;
            };   

            if (!string.IsNullOrWhiteSpace(AchxName))
                control.AnimationChains = FlatRedBallServices.Load<AnimationChainList>(AchxName, contentManagerName);

            if (!string.IsNullOrWhiteSpace(InitialAnimationChainName))
                control.CurrentAnimationChainName = InitialAnimationChainName;

            return control;
        }
    }
}
