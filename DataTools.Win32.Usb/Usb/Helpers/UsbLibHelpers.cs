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
    
    
    internal static class UsbLibHelpers
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
          HidPReportType ReportType,
          ref HidPValueCaps[] ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        [DllImport("hid.dll")]
        internal static extern bool HidP_GetValueCaps(
          HidPReportType ReportType,
          IntPtr ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        [DllImport("hid.dll")]
        internal static extern bool HidP_GetButtonCaps(
          HidPReportType ReportType,
          ref HidPButtonCaps[] ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        [DllImport("hid.dll")]
        internal static extern bool HidP_GetButtonCaps(
          HidPReportType ReportType,
          IntPtr ValueCaps,
          ref ushort ValueCapsLength,
          IntPtr PreparsedData
        );


        public static HidPButtonCaps[] GetButtonCaps(HidPReportType reportType, IntPtr ppd)
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



        public static HidPValueCaps[] GetValueCaps(HidPReportType reportType, IntPtr ppd)
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


                var featBtn = GetButtonCaps(HidPReportType.HidP_Feature, ppd);
                var fbmap = LinkButtonCollections(featBtn);

                var featVal = GetValueCaps(HidPReportType.HidP_Feature, ppd);
                var fvmap = LinkValueCollections(featVal);

                var inBtn = GetButtonCaps(HidPReportType.HidP_Input, ppd);
                var ibmap = LinkButtonCollections(inBtn);

                var inVal = GetValueCaps(HidPReportType.HidP_Input, ppd);
                var ivmap = LinkValueCollections(inVal);

                var outBtn = GetButtonCaps(HidPReportType.HidP_Output, ppd);
                var obmap = LinkButtonCollections(outBtn);

                var outVal = GetValueCaps(HidPReportType.HidP_Output, ppd);
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

        public static Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> LinkValueCollections(IList<HidPValueCaps> valCaps)
        {
            var result = new Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>();

            foreach (var item in valCaps)
            {
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
            }

            return result;
        }


        public static Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> LinkButtonCollections(IList<HidPButtonCaps> valCaps)
        {
            var result = new Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>();

            foreach (var item in valCaps)
            {
                
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
