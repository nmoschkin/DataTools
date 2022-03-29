

using DataTools.Console;
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
            batt2.OpenHid();
            
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
                    ColorConsole.WriteLine($"({{Yellow}}{val.UsageType}{{Reset}}) {{White}}{TextTools.Separate(val.UsageName)}{{Reset}} ({{DarkCyan}}{val.UsageId:X2}{{Reset}}) {val.ReportType}                         ");

                    foreach (var item in val)
                    {
                        if (item.UsageName.Contains("Time"))
                        {
                            ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Green}}{new TimeSpan(0, 0, (int?)item.Value ?? 0)}{{Reset}}                           ");
                        }
                        else
                        {
                            
                            if (item.IsButton)
                            {
                                ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{{(item.ButtonValue ? "Green" : "Red")}}}{item.ButtonValue}{{Reset}}                                                  ");
                            }
                            else if (item.Value is string || item.Value is DeviceChemistry)
                            {
                                ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Yellow}}{item.Value}{{Reset}}                                                  ");
                            }
                            else if (item.Value is Enum)
                            {
                                ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Yellow}}{TextTools.Separate(item.Value.ToString())}{{Reset}}                                                  ");
                            }
                            else
                            {

                                double vv = 0d;

                                if (item.Value is HidFeatureValue fv)
                                {
                                    vv = fv.Value;
                                }
                                else if (item.Value is int ival)
                                {
                                    vv = ival;
                                }
                                else if (item.Value is long lval)
                                {
                                    vv = lval;
                                }

                                if (item.UsageName == "Voltage" || item.UsageName.Contains("Current") || item.UsageName.Contains("Frequency"))
                                {
                                    vv /= 10;
                                }

                                ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Green}}{vv}{{Reset}}                                                  ");
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
                        ColorConsole.WriteLine($"({{Yellow}}{val.UsageType}{{Reset}}) {{White}}{TextTools.Separate(val.UsageName)}{{Reset}} ({{DarkCyan}}{val.UsageId:X2}{{Reset}}) {val.ReportType}                         ");

                        foreach (var item in val)
                        {
                            if (item.UsageName.Contains("Time"))
                            {
                                ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Green}}{new TimeSpan(0, 0, (int?)item.Value ?? 0)}{{Reset}}                           ");
                            }
                            else
                            {

                                if (item.IsButton)
                                {
                                    ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{{(item.ButtonValue ? "Green" : "Red")}}}{item.ButtonValue}{{Reset}}                                                  ");
                                }
                                else if (item.Value is string || item.Value is DeviceChemistry)
                                {
                                    ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Yellow}}{item.Value}{{Reset}}                                                  ");
                                }
                                else if (item.Value is Enum)
                                {
                                    ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Yellow}}{TextTools.Separate(item.Value.ToString())}{{Reset}}                                                  ");
                                }
                                else
                                {
                                    double vv = 0d;

                                    if (item.Value is HidFeatureValue fv)
                                    {
                                        vv = fv.Value;
                                    }
                                    else if (item.Value is int ival)
                                    {
                                        vv = ival;
                                    }
                                    else if (item.Value is long lval)
                                    {
                                        vv = lval;
                                    }

                                    if (item.UsageName == "Voltage" || item.UsageName.Contains("Current") || item.UsageName.Contains("Frequency"))
                                    {
                                        vv /= 10;
                                    }

                                    ColorConsole.WriteLine($"    ({{Yellow}}{item.UsageType}{{Reset}}) {{White}}{TextTools.Separate(item.UsageName)}{{Reset}} ({{DarkCyan}}{item.UsageId:x2}{{Reset}}): {{Green}}{vv}{{Reset}}                                                  ");
                                }
                            }

                        }

                        Console.WriteLine("                                                                      ");
                    }

                    Console.WriteLine("                                                                      ");
                    printed = true;
                }

                Task.Delay(500);
            }

            batt2.CloseHid();


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
                    HidUsageInfo? usage;
                    
                    if (feature.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        usage = (HidPowerUsageInfo?)psys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
                    }
                    else
                    {
                        usage = (HidUsageInfo?)bsys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
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

                    HidUsageInfo? usage;

                    if (feature.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        usage = (HidUsageInfo?)psys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
                    }
                    else
                    {
                        usage = (HidUsageInfo?)bsys.Where((x) => x.UsageId == feature.Usage).FirstOrDefault();
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