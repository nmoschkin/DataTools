using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents color data in BRG byte format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct BGRDATA
    {
        /// <summary>
        /// Red channel
        /// </summary>
        public byte Red;

        /// <summary>
        /// Green channel
        /// </summary>
        public byte Green;

        /// <summary>
        /// Blue channel
        /// </summary>
        public byte Blue;

        /// <summary>
        /// Convert to a string with the form 'BRG(b,r,g)'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"BRG({Blue}, {Green}, {Red})";
        }
    }
}