using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents color data in CMY byte format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct CMYDATA
    {
        /// <summary>
        /// Cyan channel
        /// </summary>
        public byte Cyan;

        /// <summary>
        /// Magenta channel
        /// </summary>
        public byte Magenta;

        /// <summary>
        /// Yellow channel
        /// </summary>
        public byte Yellow;

        /// <summary>
        /// Convert to a string with the form 'CMY(c,m,y)'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"CMY({Cyan}, {Magenta}, {Yellow})";
        }
    }
}