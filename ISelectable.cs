namespace FrbUi
{
    public enum SelectableState { NotSelected, Focused, Pushed }

    public interface ISelectable
    {
        LayoutableEvent OnFocused { get; set; }
        LayoutableEvent OnFocusLost { get; set; }
        LayoutableEvent OnPushed { get; set; }
        LayoutableEvent OnPushReleased { get; set; }
        LayoutableEvent OnClicked { get; set; }

        SelectableState CurrentSelectableState { get; }
        string StandardAnimationChainName { get; set; }
        string FocusedAnimationChainName { get; set; }
        string PushedAnimationChainName { get; set; }

        void Focus();
        void LoseFocus();
        void Push();
        void ReleasePush();
        void Click();
    }
}
