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
    internal unsafe struct LARGE_INTEGER
    {
        [FieldOffset(0)]
        internal int LowPart;

        [FieldOffset(4)]
        internal int HighPart;

        [FieldOffset(0)]
        internal long QuadPart;
        
        public static explicit operator long(LARGE_INTEGER val)
        {
            return val.QuadPart;
        }

        public static explicit operator LARGE_INTEGER(long val)
        {
            return new LARGE_INTEGER
            {
                QuadPart = val
            };
        }
    }
}
