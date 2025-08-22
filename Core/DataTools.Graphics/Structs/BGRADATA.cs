using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents color data in BRGA byte format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct BGRADATA
    {
        /// <summary>
        /// Blue channel
        /// </summary>
        public byte Blue;

        /// <summary>
        /// Green channel
        /// </summary>
        public byte Green;

        /// <summary>
        /// Red channel
        /// </summary>
        public byte Red;

        /// <summary>
        /// Alpha channel
        /// </summary>
        public byte Alpha;

        /// <summary>
        /// Convert to a string with the form 'BRGA(b,r,g,a)'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"BRGA({Blue}, {Green}, {Red}, {Alpha})";
        }
    }
}