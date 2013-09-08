using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models
{
    public class Button : SelectableAssetBase
    {
        [XmlAttribute] public string AchxFile { get; set; }
        [XmlAttribute] public string DisabledAnimationChainName { get; set; }
        [XmlAttribute] public string Text { get; set; }
        [XmlAttribute] public bool IgnoreCursorEvents { get; set; }

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls)
        {
            var button = UiControlManager.Instance.CreateControl<Controls.Button>();
            SetBaseILayoutableProperties(button, namedControls);

            button.AnimationChains = FlatRedBallServices.Load<AnimationChainList>(AchxFile, contentManagerName);
            SetupSelectableProperties(button);

            if (!string.IsNullOrWhiteSpace(DisabledAnimationChainName))
                button.DisabledAnimationChainName = DisabledAnimationChainName;

            button.Text = Text;
            button.IgnoreCursorEvents = IgnoreCursorEvents;
            return button;
        }
    }
}
