using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Math;
using FlatRedBall;

namespace FrbUi
{
    public interface ILayoutable : IVisible, IScalable, IPositionable
    {
        LayoutableEvent OnSizeChangeHandler { get; set; }

        float RelativeX { get; set; }
        float RelativeY { get; set; }
        float RelativeZ { get; set; }
        float Alpha { get; set; }
        Layer Layer { get; }
        ILayoutable ParentLayout { get; set; }

        void Activity();
        void AttachTo(PositionedObject obj, bool changeRelative);
        void AddToManagers(Layer layer);
        void UpdateDependencies(double currentTime);
        void ForceUpdateDependencies();
        void Destroy();
    }
}
