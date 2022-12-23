using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public class PreparsedData : SafeHandle
    {
        public override bool IsInvalid => handle.IsInvalidHandle();

        public PreparsedData(nint hHid) : base((nint)(-1), true)
        {
            if (hHid.IsInvalidHandle()) throw new ArgumentException();

            nint ppd = nint.Zero;
            if (UsbLibHelpers.HidD_GetPreparsedData(hHid, ref ppd))
            {
                handle = ppd;
            }
            else throw new NativeException();
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                UsbLibHelpers.HidD_FreePreparsedData(handle);
                handle = (nint)(-1);
            }

            return true;
        }

        public static implicit operator nint(PreparsedData ppd) => ppd.handle;
    }

}
