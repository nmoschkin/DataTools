using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct ARGBDATA
    {
        public byte Alpha;
        public byte Red;
        public byte Green;
        public byte Blue;

        public override string ToString()
        {
            return $"ARGB({Alpha}, {Red}, {Green}, {Blue})";
        }
    }
}