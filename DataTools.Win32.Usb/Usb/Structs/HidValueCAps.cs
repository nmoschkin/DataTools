using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{


    [StructLayout(LayoutKind.Explicit)]
    public struct HidPValueCaps
    {
        [FieldOffset(0)]
        public HidUsagePage UsagePage;

        [FieldOffset(2)]
        public byte ReportID;

        [FieldOffset(3)]
        [MarshalAs(UnmanagedType.U1)]        
        public bool IsAlias;

        [FieldOffset(4)]
        public ushort BitField;

        [FieldOffset(6)]
        public ushort LinkCollection;

        [FieldOffset(8)]
        public ushort LinkUsage;

        [FieldOffset(10)]
        public ushort LinkUsagePage;

        [FieldOffset(12)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsRange;

        [FieldOffset(13)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsStringRange;

        [FieldOffset(14)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsDesignatorRange;

        [FieldOffset(15)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsAbsolute;

        [FieldOffset(16)]
        [MarshalAs(UnmanagedType.U1)]
        public bool HasNull;

        [FieldOffset(17)]
        public byte Reserved;

        [FieldOffset(18)]
        public ushort BitSize;

        [FieldOffset(20)]
        public ushort ReportCount;

        //[FieldOffset(22)]
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 5)]
        //public ushort[] Reserved2;

        [FieldOffset(32)]
        public uint UnitsExp;

        [FieldOffset(36)]
        public uint Units;

        [FieldOffset(40)]
        public int LogicalMin;

        [FieldOffset(44)]
        public int LogicalMax;

        [FieldOffset(48)]
        public int PhysicalMin;

        [FieldOffset(52)]
        public int PhysicalMax;




        [FieldOffset(56)]
        public ushort UsageMin;

        [FieldOffset(58)]
        public ushort UsageMax;

        [FieldOffset(60)]
        public ushort StringMin;

        [FieldOffset(62)]
        public ushort StringMax;

        [FieldOffset(64)]
        public ushort DesignatorMin;

        [FieldOffset(66)]
        public ushort DesignatorMax;

        [FieldOffset(68)]
        public ushort DataIndexMin;

        [FieldOffset(70)]
        public ushort DataIndexMax;




        [FieldOffset(56)]
        public ushort Usage;

        [FieldOffset(58)]
        public ushort Reserved1;

        [FieldOffset(60)]
        public ushort StringIndex;

        [FieldOffset(62)]
        public ushort Reserved3;

        [FieldOffset(64)]
        public ushort DesignatorIndex;

        [FieldOffset(66)]
        public ushort Reserved4;

        [FieldOffset(68)]
        public ushort DataIndex;

        [FieldOffset(70)]
        public ushort Reserved5;

        public override string ToString()
        {
            return $"{UsagePage}";
        }

    }
}
