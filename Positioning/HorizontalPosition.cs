using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrbUi.Positioning
{
    public class HorizontalPosition
    {
        public enum PositionAlignment { Left, Center, Right }

        public PositionAlignment Alignment { get; private set; }
        public float Offset { get; private set; }
        public bool OffsetIsPercentage { get; private set; }

        private HorizontalPosition()
        {

        }

        public static HorizontalPosition PercentFromLeft(float pct)
        {
            return new HorizontalPosition
            {
                Alignment = PositionAlignment.Left,
                Offset = pct / 100,
                OffsetIsPercentage = true
            };
        }

        public static HorizontalPosition PercentFromCenter(float pct)
        {
            return new HorizontalPosition
            {
                Alignment = PositionAlignment.Center,
                Offset = pct / 100,
                OffsetIsPercentage = true
            };
        }

        public static HorizontalPosition PercentFromRight(float pct)
        {
            return new HorizontalPosition
            {
                Alignment = PositionAlignment.Right,
                Offset = pct / 100,
                OffsetIsPercentage = true
            };
        }

        public static HorizontalPosition OffsetFromLeft(float pct)
        {
            return new HorizontalPosition
            {
                Alignment = PositionAlignment.Left,
                Offset = pct,
                OffsetIsPercentage = false
            };
        }

        public static HorizontalPosition OffsetFromCenter(float pct)
        {
            return new HorizontalPosition
            {
                Alignment = PositionAlignment.Center,
                Offset = pct,
                OffsetIsPercentage = false
            };
        }

        public static HorizontalPosition OffsetFromRight(float pct)
        {
            return new HorizontalPosition
            {
                Alignment = PositionAlignment.Right,
                Offset = pct,
                OffsetIsPercentage = false
            };
        }
    }
}
