using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models.Controls
{
    public class LayoutableText : AssetBase
    {
        [XmlAttribute]
        public string Text { get; set; }

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls)
        {
            var control = UiControlManager.Instance.CreateControl<FrbUi.Controls.LayoutableText>();
            SetBaseILayoutableProperties(control, namedControls);

            control.DisplayText = Text;
            return control;
        }
    }
}
