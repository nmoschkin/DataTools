﻿


using DataTools.Win32;
using DataTools.Win32.Memory;
using DataTools.Win32.Usb;

using DataTools.MathTools;
using System.Runtime.InteropServices;

using static DataTools.Win32.IO;
using static DataTools.Win32.User32;

namespace TestHid
{

    public static class Program
    {

        public static void Main(string[] args)
        {
            var hids = HidDeviceInfo.EnumerateHidDevices();


            var battery = hids.Where((e) => e.DeviceClass == DeviceClassEnum.Battery).ToList().FirstOrDefault();


            var batt2 = HidPowerDeviceInfo.CreateFromHidDevice(battery);

            if (battery != null)
            {
                battery.PopulateDeviceCaps();

                PrintValCaps(battery, battery.FeatureValueCaps);
                PrintValCaps(battery, battery.InputValueCaps);
                PrintValCaps(battery, battery.OutputValueCaps);

                PrintButtonCaps(battery, battery.FeatureButtonCaps);
                PrintButtonCaps(battery, battery.InputButtonCaps);
                PrintButtonCaps(battery, battery.OutputButtonCaps);
            }

        }


        public static void PrintValCaps(HidDeviceInfo battery, HidPValueCaps[] caps)
        {
            var psys = HidPowerDevicePageInfo.Instance;
            var bsys = HidBatteryDevicePageInfo.Instance;

            foreach (var feature in caps)
            {
                if (feature.IsRange)
                {
                    Console.WriteLine($"Range Feature At {feature.Usage:xx}");
                }
                if (feature.UsagePage == (HidUsagePage)0x84 || feature.UsagePage == (HidUsagePage)0x85)
                {
                    HidPowerUsageInfo? usage;
                    
                    if (feature.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        usage = (HidPowerUsageInfo?)psys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
                    }
                    else
                    {
                        usage = (HidPowerUsageInfo?)bsys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
                    }

                    int ires = 0;
                    battery.HidGetFeature(feature.ReportID, ref ires);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {ires}");

                        var lusage = psys.Where((x) => x.UsageId == feature.LinkUsage && x.UsageName != "Reserved").FirstOrDefault();
                        if (lusage != null)
                        {
                            Console.WriteLine($"    Linked Usage: {lusage}");
                        }
                    }
                    else
                    {
                        var lusage = psys.Where((x) => x.UsageId == feature.LinkUsage && x.UsageName != "Reserved").FirstOrDefault();
                        if (lusage != null)
                        {
                            Console.WriteLine($"Linked Usage: {lusage}, Value: {ires}");
                        }
                    }
                }
            }

        }

        public static void PrintButtonCaps(HidDeviceInfo battery, HidPButtonCaps[] caps)
        {
            var psys = HidPowerDevicePageInfo.Instance;
            var bsys = HidBatteryDevicePageInfo.Instance;

            foreach (var feature in caps)
            {
                if (feature.IsRange)
                {
                    Console.WriteLine($"Range Feature At {feature.Usage:xx}");
                }
                if (feature.UsagePage == (HidUsagePage)0x84 || feature.UsagePage == (HidUsagePage)0x85)
                {

                    HidPowerUsageInfo? usage;

                    if (feature.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        usage = (HidPowerUsageInfo?)psys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
                    }
                    else
                    {
                        usage = (HidPowerUsageInfo?)bsys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
                    }

                    int ires = 0;

                    battery.HidGetFeature(feature.ReportID, ref ires);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {ires}");

                        var lusage = psys.Where((x) => x.UsageId == feature.LinkUsage && x.UsageName != "Reserved").FirstOrDefault();
                        if (lusage != null)
                        {
                            Console.WriteLine($"    Linked Usage: {lusage}");
                        }
                    }
                }
            }
        }
    }
}