namespace FrbUi
{
    public interface ISelectableControlGroup
    {
        bool Destroyed { get; }
        bool LoopFocus { get; set; }
        void ClickFocusedControl();
        void UnfocusCurrentControl();
        bool Contains(ISelectable selectable);
        bool Remove(ISelectable selectable);
        void Destroy();
    }
}
