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

        void Activity();
        void AttachTo(PositionedObject obj, bool changeRelative);
    }
}
