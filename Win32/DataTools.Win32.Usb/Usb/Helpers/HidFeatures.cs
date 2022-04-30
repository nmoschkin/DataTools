// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: UsbApi
//         HID Feature Code Manipulation
//
//         Enums are documented in part from the API documentation at MSDN.
//         Other knowledge and references obtained through various sources
//         and all is considered public domain/common knowledge.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using System;

using DataTools.Win32;
using DataTools.Win32.Memory;

namespace DataTools.Win32.Usb
{
    public static class HidFeatures
    {
        /// <summary>
        /// Enumerates all HID devices in a specific HID class.
        /// </summary>
        /// <param name="u">The HID usage page type devices to return.</param>
        /// <returns>An array of HidDeviceInfo objects.</returns>
        /// <remarks></remarks>
        public static HidDeviceInfo[] HidDevicesByUsage(HidUsagePage u)
        {
            var devs = DeviceEnum.EnumerateDevices<HidDeviceInfo>(DevProp.GUID_DEVINTERFACE_HID);
            HidDeviceInfo[] devOut = null;
            int c = 0;
            foreach (var blurb in devs)
            {
                if (blurb.HidUsagePage == u || u == HidUsagePage.Undefined)
                {
                    Array.Resize(ref devOut, c + 1);
                    devOut[c] = blurb;
                    c += 1;
                }
            }

            return devOut;
        }

        /// <summary>
        /// Opens a HID device for access.
        /// </summary>
        /// <param name="device">The HidDeviceInfo object of the device.</param>
        /// <param name="write">Attempt to open with write access.</param>
        /// <returns>A handle to the open device (close with CloseHid).</returns>
        /// <remarks></remarks>
        public static IntPtr OpenHid(HidDeviceInfo device, bool write = false)
        {
            IntPtr hhid;
           
            try
            {
                if (write)
                {
                    hhid = IO.CreateFile(device.DevicePath, IO.FILE_WRITE_ACCESS, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, IntPtr.Zero, IO.OPEN_EXISTING, IO.FILE_ATTRIBUTE_NORMAL, default);
                }
                else
                {
                    hhid = IO.CreateFile(device.DevicePath, 0, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, IntPtr.Zero, IO.OPEN_EXISTING, IO.FILE_ATTRIBUTE_NORMAL, default);
                }

                if (hhid.IsInvalidHandle()) return IntPtr.Zero;
            }
            catch
            {
                return IntPtr.Zero;
            }

            return hhid;
        }

        /// <summary>
        /// Closes a HID device handle.
        /// </summary>
        /// <param name="handle">The handle of the device to be freed.</param>
        /// <remarks></remarks>
        public static void CloseHid(IntPtr handle)
        {
            if (!handle.IsInvalidHandle())
                User32.CloseHandle(handle);
        }

        /// <summary>
        /// Retrieves a feature from the device.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="code"></param>
        /// <param name="datalen"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static HidFeatureValue? GetHIDFeature(HidDeviceInfo device, byte code, int datalen = 16)
        {
            var ch = false;

            if (!device.IsHidOpen)
            {
                device.OpenHid();
                ch = true;
            }

            var res = GetHIDFeature(device.hHid, code, datalen);

            if (ch) device.CloseHid();

            return res;
        }

        /// <summary>
        /// Retrieves a feature from the device.
        /// </summary>
        /// <param name="hhid"></param>
        /// <param name="code"></param>
        /// <param name="datalen"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static HidFeatureValue? GetHIDFeature(IntPtr hhid, byte code, int datalen = 16)
        {
            HidFeatureValue? result;

            using (var mm = new SafePtr())
            {
                try
                {
                    mm.AllocZero(datalen);
                    mm.ByteAt(0L) = code;
                    if (UsbLibHelpers.HidD_GetFeature(hhid, mm, (int)mm.Length))
                    {
                        result = new HidFeatureValue(code, mm.LongAt(1));
                    }
                    else
                    {
                        result = null;
                    }

                    mm.Free();
                }
                catch
                {
                    mm.Free();
                    return null;
                }

                return result;
            }

        }
    }
}