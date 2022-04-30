using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{



//    USAGE UsagePage;
//    UCHAR ReportID;
//    BOOLEAN IsAlias;

//    USHORT BitField;
//    USHORT LinkCollection;   // A unique internal index pointer

//    USAGE LinkUsage;
//    USAGE LinkUsagePage;

//    BOOLEAN IsRange;
//    BOOLEAN IsStringRange;
//    BOOLEAN IsDesignatorRange;
//    BOOLEAN IsAbsolute;

//    USHORT ReportCount;   // Available in API version >= 2 only.

//    USHORT Reserved2;

//    ULONG Reserved[9];
//    union {
//        struct {
//            USAGE UsageMin, UsageMax;
//    USHORT StringMin, StringMax;
//    USHORT DesignatorMin, DesignatorMax;
//    USHORT DataIndexMin, DataIndexMax;
//}
//Range;
//struct  {
//            USAGE Usage, Reserved1;
//USHORT StringIndex, Reserved2;
//USHORT DesignatorIndex, Reserved3;
//USHORT DataIndex, Reserved4;
//        } NotRange;
//    };


    [StructLayout(LayoutKind.Explicit)]
    public struct HidPButtonCaps : ICaps
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
        public HidUsagePage LinkUsagePage;

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
        [MarshalAs(UnmanagedType.U2)]
        public ushort ReportCount;

        [FieldOffset(18)]
        public ushort Reserved2;

        // Is Not Range

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


        // Is Range

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

        ushort ICaps.Usage => Usage;
        ushort ICaps.LinkCollection => LinkCollection;
        ushort ICaps.LinkUsage => LinkUsage;

        HidUsagePage ICaps.UsagePage => UsagePage;

        HidUsagePage ICaps.LinkUsagePage => LinkUsagePage;

        bool ICaps.IsRange => IsRange;

        ushort ICaps.UsageMin => UsageMin;
        ushort ICaps.UsageMax => UsageMax;

        byte ICaps.ReportID => ReportID;

        bool ICaps.IsButton => true;

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        public HidPButtonCaps Clone()
        {
            return (HidPButtonCaps)MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{UsagePage}";
        }

    }

}
