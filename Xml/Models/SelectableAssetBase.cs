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
            selectable.PushedAnimationChainName = PushedAnimationChainName;
            selectable.FocusedAnimationChainName = FocusedAnimationChainName;
            selectable.StandardAnimationChainName = StandardAnimationChainName;
            selectable.CurrentAnimationChainName = StandardAnimationChainName;
        }
    }
}
