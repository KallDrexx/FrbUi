using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrbUi
{
    public enum SelectableState { NotSelected, Focused, Pushed }

    public interface ISelectable
    {
        ILayoutableEvent OnFocused { get; set; }
        ILayoutableEvent OnFocusLost { get; set; }
        ILayoutableEvent OnPushed { get; set; }
        ILayoutableEvent OnClicked { get; set; }

        SelectableState CurrentSelectableState { get; }
        string StandardAnimationChainName { get; set; }
        string FocusedAnimationChainName { get; set; }
        string PushedAnimationChainName { get; set; }

        void Focus();
        void LoseFocus();
        void Push();
        void Click();
    }
}
