using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall.Graphics;

namespace FrbUi.Xml.Models.Controls
{
    public class LayoutableText : AssetBase
    {
        [XmlAttribute]
        public string Text { get; set; }

        [XmlAttribute]
        public string FontName { get; set; }

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls, Dictionary<string, BitmapFont> namedFonts)
        {
            var control = UiControlManager.Instance.CreateControl<FrbUi.Controls.LayoutableText>();
            SetBaseILayoutableProperties(control, namedControls);

            control.DisplayText = Text;

            if (!string.IsNullOrWhiteSpace(FontName))
            {
                BitmapFont font;
                if (!namedFonts.TryGetValue(FontName, out font))
                {
                    throw new InvalidOperationException(
                        string.Format("LayoutableText tried to use font {0} which is not setup in the UI xml", FontName));
                }

                control.Font = font;
            }

            return control;
        }
    }
}
