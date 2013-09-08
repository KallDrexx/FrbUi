using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public abstract class SelectableAssetBase : AssetBase
    {   
        [XmlAttribute] public string StandardAnimationChainName { get; set; }
        [XmlAttribute] public string FocusedAnimationChainName { get; set; }
        [XmlAttribute] public string PushedAnimationChainName { get; set; }
        [XmlAttribute] public bool PushedWithFocus { get; set; }

        protected void SetupSelectableProperties(ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException("selectable");

            selectable.PushedWithFocus = PushedWithFocus;

            if (!string.IsNullOrWhiteSpace(PushedAnimationChainName))
                selectable.PushedAnimationChainName = PushedAnimationChainName;

            if (!string.IsNullOrWhiteSpace(FocusedAnimationChainName))
                selectable.FocusedAnimationChainName = FocusedAnimationChainName;

            if (!string.IsNullOrWhiteSpace(StandardAnimationChainName))
            {
                selectable.StandardAnimationChainName = StandardAnimationChainName;
                selectable.CurrentAnimationChainName = StandardAnimationChainName;
            }
        }
    }
}
