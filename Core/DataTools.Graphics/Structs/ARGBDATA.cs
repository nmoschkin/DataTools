using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct ARGBDATA
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;

        public override string ToString()
        {
            return $"ARGB({Alpha}, {Red}, {Green}, {Blue})";
        }
    }
}