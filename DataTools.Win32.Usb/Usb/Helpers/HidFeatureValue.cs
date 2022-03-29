using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using DataTools.Win32.Memory;

namespace DataTools.Win32.Usb
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HidFeatureValue 
    {
        [FieldOffset(0)]
        public byte ReportID;
        
        [FieldOffset(1)]
        public long Value;

        public unsafe void CopyTo(void* buffer)
        {
            byte* ptr = (byte*)buffer;

            *ptr = ReportID;
            ptr++;

            *((long*)ptr) = Value;
        }

        public void CopyTo(byte[] buffer)
        {
            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    CopyTo(ptr);
                }
            }
        }

        public void CopyTo(IntPtr buffer)
        {
            unsafe
            {
                CopyTo((void*)buffer);
            }
        }

        public HidFeatureValue(byte reportId, long value)
        {
            ReportID = reportId;
            Value = value;
        }

        public static implicit operator long(HidFeatureValue val)
        {
            return val.Value;
        }

        public static explicit operator HidFeatureValue(long val)
        {
            return new HidFeatureValue(0, val);
        }

        public static explicit operator int(HidFeatureValue val)
        {
            return (int)val.Value;
        }

        public static explicit operator short(HidFeatureValue val)
        {
            return (short)val.Value;
        }

    }

}
