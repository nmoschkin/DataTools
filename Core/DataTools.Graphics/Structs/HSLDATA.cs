using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents color data in HSL format
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct HSLDATA
    {
        /// <summary>
        /// Hue
        /// </summary>
        [FieldOffset(0)]
        public Hue Hue;

        /// <summary>
        /// Saturation
        /// </summary>
        [FieldOffset(8)]
        public double Saturation;

        /// <summary>
        /// Lightness
        /// </summary>
        [FieldOffset(16)]
        public double Lightness;

        /// <summary>
        /// Convert to a string with the form 'HSL(h,s,l)'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"HSL({Hue}, {Saturation}, {Lightness})";
        }
    }
}