using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FrbUi.SelectableGroupings;

namespace FrbUi.Xml.Models.SelectableGroupings
{
    public class SelectableGroup
    {
        public enum GroupType { Sequential, Grid }

        #region XML

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public GroupType SelectableGroupType { get; set; }

        [XmlArray]
        [XmlArrayItem("SequentialSelectableGroupItem", typeof(SequentialGroupItem))]
        [XmlArrayItem("GridSelectableGroupItem", typeof(GridSelectableGroupItem))]
        public List<SelectableGroupItemBase> Controls { get; set; }

        #endregion

        public void GenerateSelectableControlGroup(Dictionary<string, ILayoutable> namedControls, 
                                                    Dictionary<string, ISelectableControlGroup> namedSelectableGroups)
        {
            ISelectableControlGroup selectableGroup;

            switch (SelectableGroupType)
            {
                case GroupType.Grid:
                    selectableGroup = UiControlManager.Instance
                                                      .CreateSelectableControlGroup<GridSelectableGroup>();
                    break;

                case GroupType.Sequential:
                default:
                    selectableGroup = UiControlManager.Instance
                                                      .CreateSelectableControlGroup<SequentialSelectableGroup>();
                    break;
            }

            foreach (var groupItem in Controls)
                groupItem.AddToGroup(selectableGroup, namedControls);

            if (!string.IsNullOrWhiteSpace(Name))
                namedSelectableGroups[Name] = selectableGroup;
        }
    }
}
