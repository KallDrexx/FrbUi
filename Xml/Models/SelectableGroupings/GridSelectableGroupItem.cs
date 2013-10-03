using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FrbUi.SelectableGroupings;

namespace FrbUi.Xml.Models.SelectableGroupings
{
    public class GridSelectableGroupItem : SelectableGroupItemBase
    {
        [XmlAttribute]
        public int RowIndex { get; set; }

        [XmlAttribute]
        public int ColumnIndex { get; set; }

        public override void AddToGroup(ISelectableControlGroup group, Dictionary<string, ILayoutable> namedControls)
        {
            var gridGroup = group as GridSelectableGroup;
            if (gridGroup == null)
            {
                throw new InvalidOperationException(
                    "Cannot add a grid selectable item to a non-grid selectable group");
            }

            var selectableControl = GetSelectableControl(namedControls);
            gridGroup.AddItem(selectableControl, RowIndex, ColumnIndex);
        }
    }
}
