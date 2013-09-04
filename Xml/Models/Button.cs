using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public class Button : AssetBase
    {
        [XmlAttribute]
        public string AnimationChains { get; set; }

        [XmlAttribute]
        public string StandardAnimationChainName { get; set; }

        [XmlAttribute]
        public string FocusedAnimationChainName { get; set; }

        [XmlAttribute]
        public string PushedAnimationChainName { get; set; }

        [XmlAttribute]
        public string DisabledAnimationChainName { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        [XmlAttribute]
        public bool IgnoreCursorEvents { get; set; }

        public override ILayoutable GenerateILayoutable()
        {
            var button = UiControlManager.Instance.CreateControl<Controls.Button>();
            SetBaseILayoutableProperties(button);

            // TODO: Add setting of the Animation chain list
            button.StandardAnimationChainName = StandardAnimationChainName;
            button.FocusedAnimationChainName = FocusedAnimationChainName;
            button.PushedAnimationChainName = PushedAnimationChainName;
            button.DisabledAnimationChainName = DisabledAnimationChainName;
            button.Text = Text;
            button.IgnoreCursorEvents = IgnoreCursorEvents;

            return button;
        }
    }
}
