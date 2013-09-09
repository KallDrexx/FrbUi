using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FrbUi.SelectableGroupings;

namespace FrbUi.Xml.Models.SelectableGroupings
{
    public class SequentialGroupItem : SelectableGroupItemBase
    {
        public override void AddToGroup(ISelectableControlGroup group, Dictionary<string, ILayoutable> namedControls)
        {
            var sequentialGroup = group as SequentialSelectableGroup;
            if (sequentialGroup == null)
                throw new InvalidOperationException(
                    "Cannot add a sequential group item to a non-sequential selectable group");

            var selectableControl = GetSelectableControl(namedControls);
            sequentialGroup.Add(selectableControl);
        }
    }
}