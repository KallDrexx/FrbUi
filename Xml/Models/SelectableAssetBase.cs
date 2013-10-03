using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models
{
    public abstract class SelectableAssetBase : AssetBase
    {
        #region XML

        [XmlAttribute]
        public string StandardAnimationChainName { get; set; }

        [XmlAttribute]
        public string FocusedAnimationChainName { get; set; }

        [XmlAttribute]
        public string PushedAnimationChainName { get; set; }

        [XmlIgnore]
        public bool? PushedWithFocus { get; set; }

        [XmlIgnore]
        public bool PushedWithFocusValueSpecified { get { return PushedWithFocus.HasValue; } }

        [XmlAttribute("PushedWithFocus")]
        public bool PushedWithFocusValue
        {
            get { return PushedWithFocus ?? default(bool); }
            set { PushedWithFocus = value; }
        }

        #endregion

        protected void SetupSelectableProperties(ISelectable selectable)
        {
            if (selectable == null)
                throw new ArgumentNullException("selectable");

            selectable.PushedWithFocus = PushedWithFocus ?? false;

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
