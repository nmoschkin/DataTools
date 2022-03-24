// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: UsbApi
//         USB-related structures, enums and functions.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using DataTools.Win32.Memory;
using DataTools.Win32.Usb;

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static DataTools.Win32.User32;

[assembly: InternalsVisibleTo("TLModel")]

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Various helper functions for interfacing with the USB HID system.
    /// </summary>
    public static class UsbLibHelpers
    {

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetProductString(IntPtr HidDeviceObject, IntPtr Buffer, int BufferLength);
        
        [DllImport("hid.dll")]
        internal static extern bool HidD_GetInputReport(IntPtr HidDeviceObject, IntPtr ReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetAttributes(IntPtr HidDeviceObject, out HidAttributes attributes);

        [DllImport("hid.dll")]
        internal static extern bool HidP_GetCaps(IntPtr ppd, out HidCaps attributes);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetFeature(IntPtr HidDeviceObject, IntPtr Buffer, int BufferLength);
        
        [DllImport("hid.dll", EntryPoint = "HidD_GetFeature")]
        internal static extern bool HidD_GetFeatureL(IntPtr HidDeviceObject, ref long Buffer, int BufferLength);
        
        [DllImport("hid.dll")]
        internal static extern bool HidD_SetFeature(IntPtr HidDeviceObject, IntPtr Buffer, int BufferLength);
        
        [DllImport("hid.dll")]
        internal static extern bool HidD_GetManufacturerString(IntPtr HidDeviceObject, IntPtr Buffer, int BufferLength);
        
        [DllImport("hid.dll")]
        internal static extern bool HidD_GetSerialNumberString(IntPtr HidDeviceObject, IntPtr Buffer, int BufferLength);
        [DllImport("hid.dll")]
        internal static extern bool HidD_GetPhysicalDescriptor(IntPtr HidDeviceObject, IntPtr Buffer, int BufferLength);
        [DllImport("hid.dll")]
        internal static extern bool HidD_GetPreparsedData(IntPtr HidDeviceObject, ref IntPtr PreparsedData);
        [DllImport("hid.dll")]
        internal static extern bool HidD_FreePreparsedData(IntPtr PreparsedData);


        [DllImport("hid.dll")]
        internal static extern bool HidP_GetValueCaps(
          HidReportType ReportType,
          ref HidPValueCaps[] ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        [DllImport("hid.dll")]
        internal static extern bool HidP_GetValueCaps(
          HidReportType ReportType,
          IntPtr ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        [DllImport("hid.dll")]
        internal static extern bool HidP_GetButtonCaps(
          HidReportType ReportType,
          ref HidPButtonCaps[] ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        [DllImport("hid.dll")]
        internal static extern bool HidP_GetButtonCaps(
          HidReportType ReportType,
          IntPtr ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        /// <summary>
        /// Get the button caps for the specified report type.
        /// </summary>
        /// <param name="reportType">The <see cref="HidReportType"/>.</param>
        /// <param name="ppd">The pointer to preparsed data.</param>
        /// <returns>An array of <see cref="HidPButtonCaps"/> structures.</returns>
        public static HidPButtonCaps[] GetButtonCaps(HidReportType reportType, IntPtr ppd)
        {
            ushort ncaps = 0;

            HidP_GetButtonCaps(reportType, IntPtr.Zero, ref ncaps, ppd);

            if (ncaps <= 0) return new HidPButtonCaps[0];

            var result = new HidPButtonCaps[ncaps];
            var hz = Marshal.SizeOf<HidPButtonCaps>();

            using(var mm = new SafePtr(ncaps * hz))
            {
                HidP_GetButtonCaps(reportType, mm.DangerousGetHandle(), ref ncaps, ppd);

                int j = 0;
                for (int i = 0; i < mm.Length; i += hz)
                {
                    result[j++] = mm.ToStructAt<HidPButtonCaps>(i);
                }

                return result;
            }

        }



        /// <summary>
        /// Get the value caps for the specified report type.
        /// </summary>
        /// <param name="reportType">The <see cref="HidReportType"/>.</param>
        /// <param name="ppd">The pointer to preparsed data.</param>
        /// <returns>An array of <see cref="HidPValueCaps"/> structures.</returns>
        public static HidPValueCaps[] GetValueCaps(HidReportType reportType, IntPtr ppd)
        {
            ushort ncaps = 0;

            HidP_GetValueCaps(reportType, IntPtr.Zero, ref ncaps, ppd);

            if (ncaps <= 0) return new HidPValueCaps[0];

            var result = new HidPValueCaps[ncaps];
            var hz = Marshal.SizeOf<HidPValueCaps>();

            using (var mm = new SafePtr(ncaps * hz))
            {
                HidP_GetValueCaps(reportType, mm.DangerousGetHandle(), ref ncaps, ppd);

                int j = 0;

                for (int i = 0; i < mm.Length; i += hz)
                {
                    result[j++] = mm.ToStructAt<HidPValueCaps>(i);
                }

                return result;
            }

        }

        /// <summary>
        /// Populate all device capabilities for the given HID class device object.
        /// </summary>
        /// <param name="device">The device to populate.</param>
        /// <returns>True if the device was successfully opened and read.</returns>
        public static bool PopulateDeviceCaps(HidDeviceInfo device)
        {

            try
            {

                IntPtr hHid = HidFeatures.OpenHid(device);
                IntPtr ppd = default;
                HidAttributes attr;
                HidCaps caps;

                HidD_GetPreparsedData(hHid, ref ppd);

                HidD_GetAttributes(hHid, out attr);

                HidP_GetCaps(ppd, out caps);


                var featBtn = GetButtonCaps(HidReportType.Feature, ppd);
                var fbmap = LinkButtonCollections(featBtn);

                var featVal = GetValueCaps(HidReportType.Feature, ppd);
                var fvmap = LinkValueCollections(featVal);

                var inBtn = GetButtonCaps(HidReportType.Input, ppd);
                var ibmap = LinkButtonCollections(inBtn);

                var inVal = GetValueCaps(HidReportType.Input, ppd);
                var ivmap = LinkValueCollections(inVal);

                var outBtn = GetButtonCaps(HidReportType.Output, ppd);
                var obmap = LinkButtonCollections(outBtn);

                var outVal = GetValueCaps(HidReportType.Output, ppd);
                var ovmap = LinkValueCollections(outVal);

                device.FeatureButtonCaps = featBtn;
                device.FeatureValueCaps = featVal;
                device.InputButtonCaps = inBtn;
                device.InputValueCaps = inVal;
                device.OutputButtonCaps = outBtn;
                device.OutputValueCaps = outVal;

                device.LinkedFeatureButtons = fbmap;
                device.LinkedFeatureValues = fvmap;

                device.LinkedInputButtons = ibmap;
                device.LinkedInputValues = ivmap;

                device.LinkedOutputButtons = obmap;
                device.LinkedOutputValues = ovmap;

                HidD_FreePreparsedData(ppd);
                CloseHandle(hHid);

            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Link value capabilities by grouped collection or linked usage.
        /// </summary>
        /// <param name="valCaps">The value capabilities structures to link</param>
        /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> where the key is a tuple of <see cref="HidUsagePage"/> and <see cref="int"/> and the value is a <see cref="IList{T}"/> of <see cref="HidPValueCaps"/>.</returns>
        public static Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> LinkValueCollections(IList<HidPValueCaps> valCaps)
        {
            var result = new Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>();

            foreach (var item in valCaps)
            {
                if (item.LinkCollection != 0)
                {
                    if (!result.TryGetValue((item.UsagePage, item.LinkCollection), out var list))
                    {
                        list = new List<HidPValueCaps>();
                        result.Add((item.UsagePage, item.LinkCollection), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }
                
                if (item.LinkUsage != 0)
                {
                    if (!result.TryGetValue((item.UsagePage, item.LinkUsage), out var list))
                    {
                        list = new List<HidPValueCaps>();
                        result.Add((item.UsagePage, item.LinkUsage), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }

                else if (item.LinkCollection == 0 && item.LinkUsage == 0)
                {
                    if (!result.TryGetValue((item.UsagePage, 0), out var list))
                    {
                        list = new List<HidPValueCaps>();
                        result.Add((item.UsagePage, item.LinkUsage), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }

            }

            return result;
        }

        /// <summary>
        /// Link button capabilities by grouped collection or linked usage.
        /// </summary>
        /// <param name="btnCaps">The button capabilities structures to link</param>
        /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> where the key is a tuple of <see cref="HidUsagePage"/> and <see cref="int"/> and the value is a <see cref="IList{T}"/> of <see cref="HidPButtonCaps"/>.</returns>
        public static Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> LinkButtonCollections(IList<HidPButtonCaps> btnCaps)
        {
            var result = new Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>();

            foreach (var item in btnCaps)
            {


                if (item.LinkCollection != 0)
                {
                    if (!result.TryGetValue((item.UsagePage, item.LinkCollection), out var list))
                    {
                        list = new List<HidPButtonCaps>();
                        result.Add((item.UsagePage, item.LinkCollection), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }
                if (item.LinkUsage != 0)
                {
                    if (!result.TryGetValue((item.UsagePage, item.LinkUsage), out var list))
                    {
                        list = new List<HidPButtonCaps>();
                        result.Add((item.UsagePage, item.LinkUsage), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }
            }

            return result;
        }

    }
}
