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

    /// <summary>
    /// HidP button caps structure
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct HidPButtonCaps : ICaps
    {
        /// <summary>
        /// Usage page
        /// </summary>
        [FieldOffset(0)]
        public HidUsagePage UsagePage;

        /// <summary>
        /// Report ID
        /// </summary>
        [FieldOffset(2)]
        public byte ReportID;

        /// <summary>
        /// Is alias
        /// </summary>
        [FieldOffset(3)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsAlias;

        /// <summary>
        /// Bit field
        /// </summary>
        [FieldOffset(4)]
        public ushort BitField;

        /// <summary>
        /// Link collection
        /// </summary>
        [FieldOffset(6)]
        public ushort LinkCollection;

        /// <summary>
        /// Link usage
        /// </summary>
        [FieldOffset(8)]
        public ushort LinkUsage;

        /// <summary>
        /// Link usage page
        /// </summary>
        [FieldOffset(10)]
        public HidUsagePage LinkUsagePage;

        /// <summary>
        /// Is numeric range
        /// </summary>
        [FieldOffset(12)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsRange;

        /// <summary>
        /// Is string range
        /// </summary>
        [FieldOffset(13)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsStringRange;

        /// <summary>
        /// Is designator range
        /// </summary>
        [FieldOffset(14)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsDesignatorRange;

        /// <summary>
        /// Is absolute
        /// </summary>
        [FieldOffset(15)]
        [MarshalAs(UnmanagedType.U1)]
        public bool IsAbsolute;

        /// <summary>
        /// Report count
        /// </summary>
        [FieldOffset(16)]
        [MarshalAs(UnmanagedType.U2)]
        public ushort ReportCount;

        /// <summary>
        /// Reserved2
        /// </summary>
        [FieldOffset(18)]
        public ushort Reserved2;

        // Is Not Range

        /// <summary>
        /// Usage
        /// </summary>
        [FieldOffset(56)]
        public ushort Usage;

        /// <summary>
        /// Reserved1
        /// </summary>
        [FieldOffset(58)]
        public ushort Reserved1;

        /// <summary>
        /// String index
        /// </summary>
        [FieldOffset(60)]
        public ushort StringIndex;

        /// <summary>
        /// Reserved3
        /// </summary>
        [FieldOffset(62)]
        public ushort Reserved3;

        /// <summary>
        /// Designator index
        /// </summary>
        [FieldOffset(64)]
        public ushort DesignatorIndex;

        /// <summary>
        /// Reserved4
        /// </summary>
        [FieldOffset(66)]
        public ushort Reserved4;

        /// <summary>
        /// Data index
        /// </summary>
        [FieldOffset(68)]
        public ushort DataIndex;

        /// <summary>
        /// Reserved5
        /// </summary>
        [FieldOffset(70)]
        public ushort Reserved5;


        // Is Range

        /// <summary>
        /// Range usage minimum
        /// </summary>
        [FieldOffset(56)]
        public ushort UsageMin;

        /// <summary>
        /// Range usage maximum
        /// </summary>
        [FieldOffset(58)]
        public ushort UsageMax;

        /// <summary>
        /// Range string minimum
        /// </summary>
        [FieldOffset(60)]
        public ushort StringMin;

        /// <summary>
        /// Range string maximum
        /// </summary>
        [FieldOffset(62)]
        public ushort StringMax;

        /// <summary>
        /// Range designator minimum
        /// </summary>
        [FieldOffset(64)]
        public ushort DesignatorMin;

        /// <summary>
        /// Range designator maximum
        /// </summary>
        [FieldOffset(66)]
        public ushort DesignatorMax;

        /// <summary>
        /// Range data index minimum
        /// </summary>
        [FieldOffset(68)]
        public ushort DataIndexMin;

        /// <summary>
        /// Range data index maximum
        /// </summary>
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

        /// <summary>
        /// Create a shallow copy of this structure
        /// </summary>
        /// <returns></returns>
        public HidPButtonCaps Clone()
        {
            return (HidPButtonCaps)MemberwiseClone();
        }
        
        /// <summary>
        /// Prints the usage page
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{UsagePage}";
        }

    }

}
