using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct BGRDATA
    {
        public byte Red;
        public byte Green;
        public byte Blue;

        public override string ToString()
        {
            return $"BRG({Blue}, {Green}, {Red})";
        }
    }
}