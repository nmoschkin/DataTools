using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct BGRADATA
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;

        public override string ToString()
        {
            return $"BRGA({Blue}, {Green}, {Red}, {Alpha})";
        }
    }
}