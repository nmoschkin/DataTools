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
using System.Text;

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

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        internal static extern bool HidD_GetIndexedString(
          [In] IntPtr HidDeviceObject,
          [In] int StringIndex,

          StringBuilder Buffer,
          [In] int BufferLength
        );

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        internal static extern bool HidD_GetIndexedString(
          [In] IntPtr HidDeviceObject,
          [In] int StringIndex,

          IntPtr Buffer,
          [In] int BufferLength
        );


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

        [DllImport("hid.dll")]
        internal static extern HidResult HidP_GetUsages(
          HidReportType ReportType,
          HidUsagePage UsagePage,
          ushort LinkCollection,
          IntPtr UsageList,
          [In, Out] ref uint UsageLength,
          IntPtr PreparsedData,
          IntPtr Report,
          uint ReportLength
        );


        public static (ushort, ushort)[] GetButtonStatesRaw(HidDeviceInfo device, HidReportType reportType, HidPButtonCaps buttonCaps)
        {
            return GetButtonStatesRaw(device, reportType, new[] { buttonCaps });
        }

        public static (ushort, ushort)[] GetButtonStatesRaw(HidDeviceInfo device, HidReportType reportType, IEnumerable<HidPButtonCaps> buttonCaps)
        {
            uint ulen;
            var output = new List<(ushort, ushort)>();

            IntPtr hhid;
            IntPtr ppd = default;
            
            var caps = device.HidCaps;

            hhid = HidFeatures.OpenHid(device);
            HidD_GetPreparsedData(hhid, ref ppd);

            using (var buffer = new SafePtr())
            {
                using (var rptbuff = new SafePtr())
                {
                    foreach (var btn in buttonCaps)
                    {

                        if (reportType == HidReportType.Feature)
                        {
                            buffer.Alloc(sizeof(ushort) * caps.NumberFeatureButtonCaps);
                            rptbuff.Alloc(caps.FeatureReportByteLength + 1);

                            rptbuff.ByteAt(0) = btn.ReportID;
                            HidD_GetFeature(hhid, rptbuff, (int)rptbuff.Length);

                            ulen = (uint)buffer.Length / sizeof(ushort);

                            HidResult res = HidP_GetUsages(reportType, btn.UsagePage, btn.LinkCollection, buffer, ref ulen, ppd, rptbuff, (uint)rptbuff.Length - 1);

                            if (res == HidResult.HIDP_STATUS_SUCCESS)
                            {
                                for (int i = 0; i < ulen; i++)
                                {
                                    ushort us = buffer.UShortAt(i);
                                    output.Add((btn.LinkCollection, us));
                                }
                            }

                        }
                        else if (reportType == HidReportType.Input)
                        {
                            buffer.Alloc(sizeof(ushort) * caps.NumberInputButtonCaps);
                            rptbuff.Alloc(caps.InputReportByteLength + 1);

                            rptbuff.ByteAt(0) = btn.ReportID;
                            HidD_GetFeature(hhid, rptbuff, (int)rptbuff.Length);

                            ulen = (uint)buffer.Length / sizeof(ushort);

                            var res = HidP_GetUsages(reportType, btn.UsagePage, btn.LinkCollection, buffer, ref ulen, ppd, rptbuff, (uint)rptbuff.Length - 1);
                            if (res == HidResult.HIDP_STATUS_SUCCESS)
                            {
                                for (int i = 0; i < ulen; i++)
                                {
                                    ushort us = buffer.UShortAt(i);
                                    output.Add((btn.LinkCollection, us));
                                }
                            }
                        }

                        else if (reportType == HidReportType.Output)
                        {
                            buffer.Alloc(sizeof(ushort) * caps.NumberOutputButtonCaps);
                            rptbuff.Alloc(caps.OutputReportByteLength + 1);

                            rptbuff.ByteAt(0) = btn.ReportID;
                            HidD_GetFeature(hhid, rptbuff, (int)rptbuff.Length);

                            ulen = (uint)buffer.Length / sizeof(ushort);

                            var res = HidP_GetUsages(reportType, btn.UsagePage, btn.LinkCollection, buffer, ref ulen, ppd, rptbuff, (uint)rptbuff.Length - 1);

                            if (res == HidResult.HIDP_STATUS_SUCCESS)
                            {
                                for (int i = 0; i < ulen; i++)
                                {
                                    ushort us = buffer.UShortAt(i);
                                    output.Add((btn.LinkCollection, us));
                                }
                            }
                        }

                    }


                }

            }

            HidD_FreePreparsedData(ppd);
            HidFeatures.CloseHid(hhid);

            return output.ToArray();

        }

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

                if (hHid == (IntPtr)(-1)) return false;

                HidD_GetPreparsedData(hHid, ref ppd);
                HidD_GetAttributes(hHid, out attr);

                HidP_GetCaps(ppd, out caps);

                device.HidCaps = caps;

                var featBtn = GetButtonCaps(HidReportType.Feature, ppd);
                ExpandCaps(ref featBtn);
                                
                var fbmap = LinkButtonCollections(featBtn);

                var featVal = GetValueCaps(HidReportType.Feature, ppd);
                ExpandCaps(ref featVal);

                var fvmap = LinkValueCollections(featVal);

                var inBtn = GetButtonCaps(HidReportType.Input, ppd);
                ExpandCaps(ref inBtn);

                var ibmap = LinkButtonCollections(inBtn);

                var inVal = GetValueCaps(HidReportType.Input, ppd);
                ExpandCaps(ref inVal);

                var ivmap = LinkValueCollections(inVal);

                var outBtn = GetButtonCaps(HidReportType.Output, ppd);
                ExpandCaps(ref outBtn);

                var obmap = LinkButtonCollections(outBtn);

                var outVal = GetValueCaps(HidReportType.Output, ppd);
                ExpandCaps(ref outVal);

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

        private static void ExpandCaps(ref HidPValueCaps[]? data)
        {
            if (data == null) return;

            var l = new List<HidPValueCaps>();

            foreach (var item in data)
            {
                if (item.IsRange)
                {
                    for (ushort i = item.UsageMin; i <= item.UsageMax; i++)
                    {
                        var citem = item.Clone();
                        citem.IsRange = false;
                        citem.Usage = i;

                        l.Add(citem);
                    }
                }
                else
                {
                    l.Add(item);
                }
            }

            data = l.ToArray();
        }

        private static void ExpandCaps(ref HidPButtonCaps[]? data)
        {
            if (data == null) return;

            var l = new List<HidPButtonCaps>();

            foreach (var item in data)
            {
                if (item.IsRange && item.UsageMin < item.UsageMax)
                {
                    for (ushort i = item.UsageMin; i <= item.UsageMax; i++)
                    {
                        var citem = item.Clone();
                        citem.IsRange = false;
                        citem.Usage = i;

                        l.Add(citem);
                    }
                }
                else
                {
                    l.Add(item);
                }
            }

            data = l.ToArray();
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
                    if (!result.TryGetValue((item.LinkUsagePage, item.LinkCollection), out var list))
                    {
                        list = new List<HidPValueCaps>();
                        result.Add((item.LinkUsagePage, item.LinkCollection), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }

                if (item.LinkUsage != 0)
                {
                    if (!result.TryGetValue((item.LinkUsagePage, item.LinkUsage), out var list))
                    {
                        list = new List<HidPValueCaps>();
                        result.Add((item.LinkUsagePage, item.LinkUsage), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }
                
                if (item.LinkUsage == 0 && item.LinkCollection == 0)
                {
                    if (!result.TryGetValue((item.UsagePage, 0), out var list))
                    {
                        list = new List<HidPValueCaps>();
                        result.Add((item.UsagePage, 0), list);
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
                    if (!result.TryGetValue((item.LinkUsagePage, item.LinkUsage), out var list))
                    {
                        list = new List<HidPButtonCaps>();
                        result.Add((item.LinkUsagePage, item.LinkUsage), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }

                if (item.LinkUsage == 0 && item.LinkCollection == 0)
                {
                    if (!result.TryGetValue((item.UsagePage, 0), out var list))
                    {
                        list = new List<HidPButtonCaps>();
                        result.Add((item.UsagePage, 0), list);
                    }

                    var test = list.Where(x => x.Usage == item.Usage).FirstOrDefault();
                    if (test.Usage == 0) list.Add(item);
                }
            }

            return result;
        }

    }
}
