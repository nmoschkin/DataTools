using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Data.Common;

namespace DataTools.Win32
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct LargeInteger
    {
        [FieldOffset(0)]
        internal int LowPart;

        [FieldOffset(4)]
        internal int HighPart;

        [FieldOffset(0)]
        internal long QuadPart;
        
        public static explicit operator long(LargeInteger val)
        {
            return val.QuadPart;
        }

        public static explicit operator LargeInteger(long val)
        {
            return new LargeInteger
            {
                QuadPart = val
            };
        }
    }
}
