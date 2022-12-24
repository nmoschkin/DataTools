using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct HSLDATA
    {
        [FieldOffset(0)]
        public Hue Hue;

        [FieldOffset(8)]
        public double Saturation;

        [FieldOffset(16)]
        public double Lightness;

        public override string ToString()
        {
            return $"HSL({Hue}, {Saturation}, {Lightness})";
        }
    }
}