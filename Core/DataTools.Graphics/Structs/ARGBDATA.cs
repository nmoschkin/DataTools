using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents color data in ARGB byte format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct ARGBDATA
    {
        /// <summary>
        /// Alpha channel
        /// </summary>
        public byte Alpha;

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
        /// Convert to a string with the form 'ARGB(a,r,g,b)'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ARGB({Alpha}, {Red}, {Green}, {Blue})";
        }
    }
}