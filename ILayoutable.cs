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

        void AttachTo(PositionedObject obj, bool changeRelative);
        float RelativeX { get; set; }
        float RelativeY { get; set; }
    }
}
