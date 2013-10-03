using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FrbUi.Xml.Models.SelectableGroupings
{
    public abstract class SelectableGroupItemBase
    {
        [XmlAttribute]
        public string ControlName { get; set; }

        public abstract void AddToGroup(ISelectableControlGroup group, Dictionary<string, ILayoutable> namedControls);

        protected ISelectable GetSelectableControl(Dictionary<string, ILayoutable> namedControls)
        {
            ILayoutable control;
            if (!namedControls.TryGetValue(ControlName, out control))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Tried to add the {0} control to a selectable group but no control by that name was found",
                        ControlName));
            }

            var selectableControl = control as ISelectable;
            if (selectableControl == null)
            {
                throw new InvalidOperationException(
                    string.Format("The {0} control is not selectable, and thus can't be added to a selectable group",
                                  ControlName));
            }
            return selectableControl;
        }
    }
}
