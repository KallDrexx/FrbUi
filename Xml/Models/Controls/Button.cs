using System.Collections.Generic;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models.Controls
{
    public class Button : SelectableAssetBase
    {
        #region XML

        [XmlAttribute]
        public string AchxFile { get; set; }

        [XmlAttribute]
        public string DisabledAnimationChainName { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        [XmlIgnore]
        public bool? IgnoreCursorEvents { get; set; }

        [XmlIgnore]
        public bool IgnoreCursorEventsValueSpecified { get { return IgnoreCursorEvents.HasValue; } }

        [XmlAttribute("IgnoreCursorEvents")]
        public bool IgnoreCursorEventsValue
        {
            get { return IgnoreCursorEvents ?? default(bool);}
            set { IgnoreCursorEvents = value; }
        }

        #endregion

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls, Dictionary<string, BitmapFont> namedFonts)
        {
            var button = UiControlManager.Instance.CreateControl<FrbUi.Controls.Button>();
            SetBaseILayoutableProperties(button, namedControls);

            if (!string.IsNullOrWhiteSpace(AchxFile))
                button.AnimationChains = FlatRedBallServices.Load<AnimationChainList>(AchxFile, contentManagerName);

            SetupSelectableProperties(button);

            if (!string.IsNullOrWhiteSpace(DisabledAnimationChainName))
                button.DisabledAnimationChainName = DisabledAnimationChainName;

            button.Text = Text;
            button.IgnoreCursorEvents = IgnoreCursorEvents ?? false;
            return button;
        }
    }
}
