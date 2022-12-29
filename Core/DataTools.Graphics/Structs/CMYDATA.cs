using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct CMYDATA
    {
        public byte Cyan;
        public byte Magenta;
        public byte Yellow;

        public override string ToString()
        {
            return $"CMY({Cyan}, {Magenta}, {Yellow})";
        }
    }
}