using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Math;
using FlatRedBall;

namespace FrbUi
{
    public delegate void ILayoutableEvent(ILayoutable sender);

    public interface ILayoutable : IVisible, IScalable, IPositionable
    {
        ILayoutableEvent OnSizeChangeHandler { get; set; }

        float RelativeX { get; set; }
        float RelativeY { get; set; }
        float RelativeZ { get; set; }
        float Alpha { get; set; }
        Layer Layer { get; }

        void Activity();
        void AttachTo(PositionedObject obj, bool changeRelative);
        void AddToManagers(Layer layer);
        void UpdateDependencies(double currentTime);
        void ForceUpdateDependencies();
        void Destroy();
    }
}
