using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrbUi.Positioning
{
    public class VerticalPosition
    {
        public enum PositionAlignment { Top, Center, Bottom }

        public PositionAlignment Alignment { get; protected set; }
        public float Offset { get; protected set; }
        public bool OffsetIsPercentage { get; protected set; }

        protected VerticalPosition()
        {

        }

        public static VerticalPosition PercentFromTop(float pct)
        {
            return new VerticalPosition
            {
                Alignment = PositionAlignment.Top,
                Offset = pct / 100,
                OffsetIsPercentage = true
            };
        }

        public static VerticalPosition PercentFromCenter(float pct)
        {
            return new VerticalPosition
            {
                Alignment = PositionAlignment.Center,
                Offset = pct / 100,
                OffsetIsPercentage = true
            };
        }

        public static VerticalPosition PercentFromBottom(float pct)
        {
            return new VerticalPosition
            {
                Alignment = PositionAlignment.Bottom,
                Offset = pct / 100,
                OffsetIsPercentage = true
            };
        }

        public static VerticalPosition OffsetFromTop(float pct)
        {
            return new VerticalPosition
            {
                Alignment = PositionAlignment.Top,
                Offset = pct,
                OffsetIsPercentage = false
            };
        }

        public static VerticalPosition OffsetFromCenter(float pct)
        {
            return new VerticalPosition
            {
                Alignment = PositionAlignment.Center,
                Offset = pct,
                OffsetIsPercentage = false
            };
        }

        public static VerticalPosition OffsetFromBottom(float pct)
        {
            return new VerticalPosition
            {
                Alignment = PositionAlignment.Bottom,
                Offset = pct,
                OffsetIsPercentage = false
            };
        }
    }
}
