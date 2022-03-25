


using DataTools.Win32;
using DataTools.Win32.Memory;
using DataTools.Win32.Usb;

using DataTools.MathTools;
using System.Runtime.InteropServices;

using static DataTools.Win32.IO;
using static DataTools.Win32.User32;
using DataTools.Text;
using System.Text;

namespace TestHid
{

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            

            var hids = HidDeviceInfo.EnumerateHidDevices();


            var battery = hids.Where((e) => e.DeviceClass == DeviceClassEnum.Battery).ToList().FirstOrDefault();


            var batt2 = HidPowerDeviceInfo.CreateFromHidDevice(battery);

            var mstr = batt2.Manufacturer;
            var mstr2 = batt2.HidManufacturer;
            var pstr = batt2.ProductString;
            var sstr = batt2.SerialNumber;

            Console.Clear();
            Console.CursorVisible = false;
            Console.WindowHeight = 100;

            Task.Delay(1000).Wait();

            bool printed = false;


            //PrintValCaps(batt2, batt2.FeatureValueCaps);
            //PrintValCaps(batt2, batt2.InputValueCaps);
            //PrintValCaps(batt2, batt2.OutputValueCaps);

            //Environment.Exit(0);

            while (true)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;

                if (Console.KeyAvailable) break;

                var vals = batt2.RefreshDynamicValues();

                foreach (var val in vals)
                {
                    Console.WriteLine($"({val.Key.UsageType}) {TextTools.Separate(val.Key.UsageName)} ({val.Key.UsageId:X2}) {val.Key.ReportType}                         ");

                    foreach (var item in val.Value)
                    {
                        if (item.UsageName.Contains("Time"))
                        {
                            Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {new TimeSpan(0, 0, (int?)item.Value ?? 0)}                           ");
                        }
                        else
                        {
                            
                            if (item.IsButton)
                            {
                                Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {item.ButtonValue}                                                  ");
                            }
                            else if (item.Value is string || item.Value is DeviceChemistry)
                            {
                                Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {item.Value}                                                  ");
                            }
                            else if (item.Value is Enum)
                            {
                                Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {TextTools.Separate(item.Value.ToString())}                                                  ");
                            }
                            else
                            {
                                double vv = (int?)item.Value ?? 0d;

                                if (item.UsageName == "Voltage" || item.UsageName.Contains("Current") || item.UsageName.Contains("Frequency"))
                                {
                                    vv /= 10;
                                }

                                Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {vv}                                                  ");
                            }
                        }
                    }

                    Console.WriteLine("                                                                      ");
                }

                if (!printed)
                {
                    var svals = batt2.GetFeatureValues(HidUsageType.CP | HidUsageType.CL | HidUsageType.CA, HidUsageType.SV | HidUsageType.SF, true);

                    foreach (var val in svals)
                    {
                        Console.WriteLine($"({val.Key.UsageType}) {TextTools.Separate(val.Key.UsageName)} ({val.Key.UsageId:X2}) {val.Key.ReportType}                          ");

                        foreach (var item in val.Value)
                        {

                            if (item.UsageName.Contains("Time"))
                            {
                                Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {new TimeSpan(0, 0, (int?)item.Value ?? 0)}                           ");
                            }
                            else
                            {
                                if (item.IsButton)
                                {
                                    Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {item.ButtonValue}                                                  ");
                                }
                                else if (item.Value is string || item.Value is DeviceChemistry)
                                {
                                    Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {item.Value}                                                  ");
                                }
                                else if (item.Value is Enum)
                                {
                                    Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {TextTools.Separate(item.Value.ToString())}                                                  ");
                                }
                                else
                                {
                                    double vv = (int?)item.Value ?? 0d;

                                    if (item.UsageName == "Voltage" || item.UsageName.Contains("Current") || item.UsageName.Contains("Frequency"))
                                    {
                                        vv /= 10;
                                    }

                                    Console.WriteLine($"    ({item.UsageType}) {TextTools.Separate(item.UsageName)} ({item.UsageId:x2}): {vv}                                                  ");
                                }
                            }
                        }

                        Console.WriteLine("                                                                      ");
                    }

                    Console.WriteLine("                                                                      ");


                    printed = true;
                }

                Task.Delay(500);
                Environment.Exit(0);    
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
                    battery.HidGetFeature(feature.ReportID, out ires);

                    if (usage != null)
                    {
                        Console.WriteLine($"\r\nFound Power Device Usage {usage} ({usage.UsageId:x2})\r\nPage {feature.UsagePage}, Value: {ires}");

                        var lusage = psys.Where((x) => x.UsageId == feature.LinkUsage && x.UsageName != "Reserved").FirstOrDefault();
                        if (lusage != null)
                        {
                            Console.WriteLine($"    Linked Usage: {lusage}  ({lusage.UsageId:x2})");
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

                    battery.HidGetFeature(feature.ReportID, out ires);

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