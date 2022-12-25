// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: UsbApi
//         USB-related structures, enums and functions.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Memory;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

[assembly: InternalsVisibleTo("TLModel")]

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Various helper functions for interfacing with the USB HID system.
    /// </summary>
    public static class UsbLibHelpers
    {
        [DllImport("hid.dll")]
        internal static extern bool HidD_GetProductString(nint HidDeviceObject, nint Buffer, int BufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetInputReport(nint HidDeviceObject, nint ReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetAttributes(nint HidDeviceObject, out HidAttributes attributes);

        [DllImport("hid.dll")]
        internal static extern bool HidP_GetCaps(nint ppd, out HidCaps attributes);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetFeature(nint HidDeviceObject, nint Buffer, int BufferLength);

        [DllImport("hid.dll", EntryPoint = "HidD_GetFeature")]
        internal static extern bool HidD_GetFeatureL(nint HidDeviceObject, ref long Buffer, int BufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_SetFeature(nint HidDeviceObject, nint Buffer, int BufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetManufacturerString(nint HidDeviceObject, nint Buffer, int BufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetSerialNumberString(nint HidDeviceObject, nint Buffer, int BufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetPhysicalDescriptor(nint HidDeviceObject, nint Buffer, int BufferLength);

        [DllImport("hid.dll")]
        internal static extern bool HidD_GetPreparsedData(nint HidDeviceObject, ref nint PreparsedData);

        [DllImport("hid.dll")]
        internal static extern bool HidD_FreePreparsedData(nint PreparsedData);

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        internal static extern bool HidD_GetIndexedString(
          [In] nint HidDeviceObject,
          [In] int StringIndex,

          StringBuilder Buffer,
          [In] int BufferLength
        );

        [DllImport("hid.dll", CharSet = CharSet.Unicode)]
        internal static extern bool HidD_GetIndexedString(
          [In] nint HidDeviceObject,
          [In] int StringIndex,

          nint Buffer,
          [In] int BufferLength
        );

        [DllImport("hid.dll")]
        internal static extern bool HidP_GetValueCaps(
          HidReportType ReportType,
          ref HidPValueCaps[] ValueCaps,
          ref ushort ValueCapsLength,
          nint PreparsedData
        );

        [DllImport("hid.dll")]
        internal static extern bool HidP_GetValueCaps(
          HidReportType ReportType,
          nint ValueCaps,
          ref ushort ValueCapsLength,
          nint PreparsedData
        );

        [DllImport("hid.dll")]
        internal static extern bool HidP_GetButtonCaps(
          HidReportType ReportType,
          ref HidPButtonCaps[] ValueCaps,
          ref ushort ValueCapsLength,
          nint PreparsedData
        );

        [DllImport("hid.dll")]
        internal static extern bool HidP_GetButtonCaps(
          HidReportType ReportType,
          nint ValueCaps,
          ref ushort ValueCapsLength,
          nint PreparsedData
        );

        [DllImport("hid.dll")]
        internal static extern HidResult HidP_GetUsages(
          HidReportType ReportType,
          HidUsagePage UsagePage,
          ushort LinkCollection,
          nint UsageList,
          [In, Out] ref uint UsageLength,
          nint PreparsedData,
          nint Report,
          uint ReportLength
        );

        [DllImport("hid.dll")]
        internal static extern HidResult HidP_GetScaledUsageValue(
          HidReportType ReportType,
          HidUsagePage UsagePage,
          ushort LinkCollection,
          ushort Usage,
          [Out] out long UsageValue,
          nint PreparsedData,
          nint Report,
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

            nint hhid;

            var caps = device.HidCaps;
            var ch = false;

            if (!device.IsHidOpen)
            {
                if (!device.OpenHid()) return new (ushort, ushort)[0];
                else ch = true;
            }

            hhid = device.DangerousGetHidDeviceHandle();

            if (hhid == 0)
            {
                return new (ushort, ushort)[0];
            }

            using (var ppd = new PreparsedData(hhid))
            {
                using (var buffer = new DataTools.Memory.SafePtr())
                {
                    using (var rptbuff = new DataTools.Memory.SafePtr())
                    {
                        foreach (var btn in buttonCaps)
                        {
                            if (reportType == HidReportType.Feature)
                            {
                                buffer.Alloc(sizeof(ushort) * caps.NumberFeatureButtonCaps);
                                rptbuff.Alloc(caps.FeatureReportByteLength + 1);
                            }
                            else if (reportType == HidReportType.Input)
                            {
                                buffer.Alloc(sizeof(ushort) * caps.NumberInputButtonCaps);
                                rptbuff.Alloc(caps.InputReportByteLength + 1);
                            }
                            else
                            {
                                buffer.Alloc(sizeof(ushort) * caps.NumberOutputButtonCaps);
                                rptbuff.Alloc(caps.OutputReportByteLength + 1);
                            }

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
                    }
                }
            }

            if (ch) device.CloseHid();
            return output.ToArray();
        }

        public static HidFeatureValue? GetScaledValue(HidDeviceInfo device, HidReportType reportType, HidPValueCaps valueCaps)
        {
            var result = GetScaledValues(device, reportType, new[] { valueCaps });
            if (result.Length == 0) return null;
            return result[0].Item3;
        }

        public static (ushort, ushort, HidFeatureValue)[] GetScaledValues(HidDeviceInfo device, HidReportType reportType, IEnumerable<HidPValueCaps> valueCaps)
        {
            var output = new List<(ushort, ushort, HidFeatureValue)>();

            nint hhid;

            var caps = device.HidCaps;
            var ch = false;

            if (!device.IsHidOpen)
            {
                if (!device.OpenHid()) return new (ushort, ushort, HidFeatureValue)[0];
                else ch = true;
            }

            hhid = device.DangerousGetHidDeviceHandle();

            if (hhid == 0)
            {
                return new (ushort, ushort, HidFeatureValue)[0];
            }

            using (var ppd = new PreparsedData(hhid))
            {
                using (var rptbuff = new DataTools.Memory.SafePtr())
                {
                    foreach (var val in valueCaps)
                    {
                        if (reportType == HidReportType.Feature)
                        {
                            rptbuff.Alloc(caps.FeatureReportByteLength + 1);
                        }
                        else if (reportType == HidReportType.Input)
                        {
                            rptbuff.Alloc(caps.InputReportByteLength + 1);
                        }
                        else
                        {
                            rptbuff.Alloc(caps.OutputReportByteLength + 1);
                        }

                        rptbuff.ByteAt(0) = val.ReportID;
                        HidD_GetFeature(hhid, rptbuff, (int)rptbuff.Length);

                        long retVal;

                        HidResult res = HidP_GetScaledUsageValue(reportType, val.UsagePage, val.LinkCollection, val.Usage, out retVal, ppd, rptbuff, (uint)rptbuff.Length - 1);

                        if (res == HidResult.HIDP_STATUS_SUCCESS)
                        {
                            output.Add((val.LinkCollection, val.Usage, new HidFeatureValue(val.ReportID, retVal)));
                        }
                    }
                }
            }

            if (ch) device.CloseHid();
            return output.ToArray();
        }

        /// <summary>
        /// Get the button caps for the specified report type.
        /// </summary>
        /// <param name="reportType">The <see cref="HidReportType"/>.</param>
        /// <param name="ppd">The pointer to preparsed data.</param>
        /// <returns>An array of <see cref="HidPButtonCaps"/> structures.</returns>
        public static HidPButtonCaps[] GetButtonCaps(HidReportType reportType, PreparsedData ppd)
        {
            ushort ncaps = 0;

            HidP_GetButtonCaps(reportType, nint.Zero, ref ncaps, ppd);

            if (ncaps <= 0) return new HidPButtonCaps[0];

            var result = new HidPButtonCaps[ncaps];
            var hz = Marshal.SizeOf<HidPButtonCaps>();

            using (var mm = new SafePtr(ncaps * hz))
            {
                HidP_GetButtonCaps(reportType, mm, ref ncaps, ppd);

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
        public static HidPValueCaps[] GetValueCaps(HidReportType reportType, PreparsedData ppd)
        {
            ushort ncaps = 0;

            HidP_GetValueCaps(reportType, nint.Zero, ref ncaps, ppd);

            if (ncaps <= 0) return new HidPValueCaps[0];

            var result = new HidPValueCaps[ncaps];
            var hz = Marshal.SizeOf<HidPValueCaps>();

            using (var mm = new SafePtr(ncaps * hz))
            {
                HidP_GetValueCaps(reportType, mm, ref ncaps, ppd);

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
        /// <param name="hHid">Optional pre-opened HID device handle.</param>
        /// <returns>True if the device was successfully opened and read.</returns>
        /// <remarks>
        /// If you provide a device handle, you must free the resource, yourself.
        /// </remarks>
        public static bool PopulateDeviceCaps(HidDeviceInfo device, nint? hHid = null)
        {
            try
            {
                var hhid = hHid ?? HidFeatures.OpenHid(device);
                if (hhid == 0) return false;

                HidAttributes attr;

                HidCaps caps;

                if (hhid == (nint)(-1)) return false;

                using (var ppd = new PreparsedData(hhid))
                {
                    HidD_GetAttributes(hhid, out attr);
                    HidP_GetCaps(ppd, out caps);

                    device.HidCaps = caps;

                    HidPButtonCaps[]? featBtn;
                    HidPButtonCaps[]? inBtn;
                    HidPButtonCaps[]? outBtn;

                    HidPValueCaps[]? featVal;
                    HidPValueCaps[]? inVal;
                    HidPValueCaps[]? outVal;

                    Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>? fbmap = null;
                    Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>? ibmap = null;
                    Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>? obmap = null;

                    Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>? fvmap = null;
                    Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>? ivmap = null;
                    Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>? ovmap = null;

                    featBtn = GetButtonCaps(HidReportType.Feature, ppd);
                    inBtn = GetButtonCaps(HidReportType.Input, ppd);
                    outBtn = GetButtonCaps(HidReportType.Output, ppd);

                    featVal = GetValueCaps(HidReportType.Feature, ppd);
                    inVal = GetValueCaps(HidReportType.Input, ppd);
                    outVal = GetValueCaps(HidReportType.Output, ppd);

                    if (featBtn != null)
                    {
                        ExpandCaps(ref featBtn);
                        if (featBtn != null)
                        {
                            fbmap = LinkButtonCollections(featBtn);
                            device.FeatureButtonCaps = featBtn;
                            device.LinkedFeatureButtons = fbmap;
                        }
                    }

                    if (inBtn != null)
                    {
                        ExpandCaps(ref inBtn);
                        if (inBtn != null)
                        {
                            ibmap = LinkButtonCollections(inBtn);

                            device.InputButtonCaps = inBtn;
                            device.LinkedInputButtons = ibmap;
                        }
                    }

                    if (outBtn != null)
                    {
                        ExpandCaps(ref outBtn);
                        if (outBtn != null)
                        {
                            obmap = LinkButtonCollections(outBtn);

                            device.OutputButtonCaps = outBtn;
                            device.LinkedOutputButtons = obmap;
                        }
                    }

                    if (featVal != null)
                    {
                        ExpandCaps(ref featVal);
                        if (featVal != null)
                        {
                            fvmap = LinkValueCollections(featVal);

                            device.FeatureValueCaps = featVal;
                            device.LinkedFeatureValues = fvmap;
                        }
                    }

                    if (inVal != null)
                    {
                        ExpandCaps(ref inVal);
                        if (inVal != null)
                        {
                            ivmap = LinkValueCollections(inVal);

                            device.InputValueCaps = inVal;
                            device.LinkedInputValues = ivmap;
                        }
                    }

                    if (outVal != null)
                    {
                        ExpandCaps(ref outVal);
                        if (outVal != null)
                        {
                            ovmap = LinkValueCollections(outVal);

                            device.OutputValueCaps = outVal;
                            device.LinkedOutputValues = ovmap;
                        }
                    }
                }

                if (hHid == null) HidFeatures.CloseHid(hhid);
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