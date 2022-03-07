


using DataTools.Win32;
using DataTools.Win32.Memory;
using DataTools.Win32.Usb;

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


            var battery = hids.Where((e) => e.DeviceClass == DeviceClassEnum.Battery).ToList().First();

            battery.PopulateDeviceCaps();

            var psys = new HidPowerDevicePageInfo();
            var bsys = new HidBatterySystemPageInfo();


            foreach (var feature in battery.FeatureValueCaps)
            {
                if (feature.UsagePage == (HidUsagePage)0x84)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();
                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {result}");
                    }
                }
                else if (feature.UsagePage == (HidUsagePage)0x85)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();

                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Battery Device Usage {usage}, Value: {result}");
                    }

                }
            }


            foreach (var feature in battery.FeatureButtonCaps)
            {
                if (feature.UsagePage == (HidUsagePage)0x84)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();
                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {result}");
                    }
                }
                else if (feature.UsagePage == (HidUsagePage)0x85)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();

                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Battery Device Usage {usage}, Value: {result}");
                    }

                }
            }




            foreach (var feature in battery.InputValueCaps)
            {
                if (feature.UsagePage == (HidUsagePage)0x84)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();
                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {result}");
                    }
                }
                else if (feature.UsagePage == (HidUsagePage)0x85)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();

                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Battery Device Usage {usage}, Value: {result}");
                    }

                }
            }



            foreach (var feature in battery.InputButtonCaps)
            {
                if (feature.UsagePage == (HidUsagePage)0x84)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();
                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {result}");
                    }
                }
                else if (feature.UsagePage == (HidUsagePage)0x85)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();

                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Battery Device Usage {usage}, Value: {result}");
                    }

                }
            }



            foreach (var feature in battery.OutputValueCaps)
            {
                if (feature.UsagePage == (HidUsagePage)0x84)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();
                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {result}");
                    }
                }
                else if (feature.UsagePage == (HidUsagePage)0x85)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();

                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Battery Device Usage {usage}, Value: {result}");
                    }

                }
            }




            foreach (var feature in battery.OutputButtonCaps)
            {
                if (feature.UsagePage == (HidUsagePage)0x84)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();
                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Power Device Usage {usage}, Value: {result}");
                    }
                }
                else if (feature.UsagePage == (HidUsagePage)0x85)
                {
                    var usage = psys.Where((x) => x.UsageId == feature.Usage).First();

                    int result = 0;
                    battery.HidGetFeature(feature.ReportID, ref result);

                    if (usage != null)
                    {
                        Console.WriteLine($"Found Battery Device Usage {usage}, Value: {result}");
                    }

                }
            }






        }
    }
}