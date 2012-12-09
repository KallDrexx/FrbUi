using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrbUi
{
    public interface IClickable
    {
        ILayoutableEvent OnFocused { get; set; }
        ILayoutableEvent OnFocusLost { get; set; }
        ILayoutableEvent OnPushed { get; set; }
        ILayoutableEvent OnClicked { get; set; }

        string StandardAnimationChainName { get; set; }
        string FocusedAnimationChainName { get; set; }
        string PushedAnimationChainName { get; set; }

        void Focus();
        void LoseFocus();
        void Push();
        void Click();
    }
}
