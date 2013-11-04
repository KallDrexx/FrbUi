using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models.Controls
{
    public class LayoutableSpriteXml : AssetXmlBase
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

        [XmlIgnore]
        public float? PixelSize { get; set; }

        [XmlIgnore]
        public bool PixelSizeValueSpecified { get { return PixelSize.HasValue; } }

        [XmlAttribute("PixelSize")]
        public float PixelSizeValue
        {
            get { return PixelSize ?? default(float); }
            set { PixelSize = value; }
        }

        [XmlIgnore]
        public float? TextureScale { get; set; }

        [XmlIgnore]
        public bool TextureScaleValueSpecified { get { return TextureScale.HasValue; } }

        [XmlAttribute("TextureScale")]
        public float TextureScaleValue
        {
            get { return TextureScale ?? default(float); }
            set { TextureScale = value; }
        }

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls, Dictionary<string, BitmapFont> namedFonts)
        {
            var control = UiControlManager.Instance.CreateControl<FrbUi.Controls.LayoutableSprite>();

            // Since rotation value is relative, we need to set this in the after added event
            control.OnAddedToLayout += delegate {
                control.RelativeRotationZ = (float)(Math.PI * ((RelativeRotationZDegrees ?? 0) / 180));
                control.OnAddedToLayout = null;
            };   

            if (!string.IsNullOrWhiteSpace(AchxName))
                control.AnimationChains = FlatRedBallServices.Load<AnimationChainList>(AchxName, contentManagerName);

            if (!string.IsNullOrWhiteSpace(InitialAnimationChainName))
                control.CurrentAnimationChainName = InitialAnimationChainName;

            SetBaseILayoutableProperties(control, namedControls);
            if (PixelSize != null)
                control.PixelSize = PixelSize.Value;

            if (TextureScale != null)
                control.TextureScale = TextureScale.Value;

            return control;
        }
    }
}
