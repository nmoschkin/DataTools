using DataTools.MathTools.Polar;

using System;
using System.Runtime.InteropServices;

namespace DataTools.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct W32POINT
    {
        public int x;
        public int y;

        public W32POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator LinearCoordinates(W32POINT source)
        {
            return new LinearCoordinates(source.x, source.y);
        }

        public static explicit operator W32POINT(LinearCoordinates source)
        {
            return new W32POINT((int)source.X, (int)source.Y);
        }
    }
}