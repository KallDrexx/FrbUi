namespace FrbUi
{
    public interface ISelectableControlGroup
    {
        void ClickFocusedControl();
        void UnfocusCurrentControl();
        bool Contains(ISelectable selectable);
        bool Remove(ISelectable selectable);
        void Destroy();
    }
}
