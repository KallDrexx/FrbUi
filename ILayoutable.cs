using System;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Math;
using FlatRedBall;

namespace FrbUi
{
    public interface ILayoutable : IVisible, IScalable, IPositionable
    {
        LayoutableEvent OnSizeChangeHandler { get; set; }
        LayoutableEvent OnAddedToLayout { get; set; }

        float RelativeX { get; set; }
        float RelativeY { get; set; }
        float RelativeZ { get; set; }
        float Alpha { get; set; }
        Layer Layer { get; }
        ILayoutable ParentLayout { get; set; }

        /// <summary>
        /// Any extra data relating to to the layoutable object that may
        /// be used by custom code (mostly during events)
        /// </summary>
        string Tag { get; set; }

        void Activity();
        void AttachTo(PositionedObject obj, bool changeRelative);
        void AttachObject(PositionedObject obj, bool changeRelative);
        void AttachTo(ILayoutable obj, bool changeRelative);
        void AttachObject(ILayoutable obj, bool changeRelative);
        void Detach();
        void AddToManagers(Layer layer);
        void UpdateDependencies(double currentTime);
        void ForceUpdateDependencies();
        void Destroy();
    }
}
