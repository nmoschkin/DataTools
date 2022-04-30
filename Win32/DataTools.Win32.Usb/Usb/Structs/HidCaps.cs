

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public struct HidCaps
    {
        
        public ushort Usage;
        public HidUsagePage UsagePage;
        public ushort InputReportByteLength;
        public ushort OutputReportByteLength;
        public ushort FeatureReportByteLength;
        
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType=UnmanagedType.U2, SizeConst = 17)]
        public ushort[] Reserved;

        public ushort NumberLinkCollectionNodes;

        public ushort NumberInputButtonCaps;
        public ushort NumberInputValueCaps;
        public ushort NumberInputDataIndices;

        public ushort NumberOutputButtonCaps;
        public ushort NumberOutputValueCaps;
        public ushort NumberOutputDataIndices;

        public ushort NumberFeatureButtonCaps;
        public ushort NumberFeatureValueCaps;
        public ushort NumberFeatureDataIndices;

    }
}
