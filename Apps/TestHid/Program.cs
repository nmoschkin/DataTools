using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTools.Essentials.Collections;
using DataTools.Text;
using DataTools.Win32;
using DataTools.Win32.Usb;
using DataTools.Win32.Usb.Power;
using Newtonsoft.Json;

namespace TestHid
{

    public class ExampleRecord
    {
        public ExampleRecord()
        {

            var size = Random.Shared.Next(10, 2048);
            var sb = new StringBuilder();

            for (var i = 0; i < size; i++)
            {
                char bm = (char)((byte)' ' + (byte)Random.Shared.Next(0, 56));

                sb.Append(bm);
            }
            R = sb.ToString();
        }

        public string A
        {
            get; set;
        } = $"Example {DateTime.Now:dddd}";

        public int B
        {
            get; set;
        } = (int)Random.Shared.Next();

        public DateTime D
        {
            get; set;
        } = DateTime.Now;

        public Guid GV
        {
            get; set;
        } = Guid.NewGuid();

        public string R
        {
            get; set;
        }

        public override string ToString()
        {
            return $"{A}, Reality {B} as {GV:O}";
        }
        public override bool Equals(object obj)
        {
            if (obj is ExampleRecord other)
            {
                if (other.A != A) return false;
                if (other.B != B) return false;
                if (other.D != D) return false;
                if (other.GV != GV) return false;
                if (other.R != R) return false;
                return true;
            }
            return base.Equals(obj);
        }
    }

    public static class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var someSettings = new JsonSerializerSettings();
            var getSettingsFunc = () =>
            {
                return someSettings;
            };
            
            JsonConvert.DefaultSettings = getSettingsFunc;
            
            using (var diskCol = new DiskCollection<ExampleRecord>($"{folder}\\simple_col.txt"))
            {
                var ridx = 192;
                var ridx2 = 785;
                var cc = 0;
                ExampleRecord testobj = null;
                var time1 = DateTime.Now;
                var time2 = DateTime.Now;
                var diff = time2 - time1;
                List<ExampleRecord> recovered = new List<ExampleRecord>();

                // Cold Enumeration
                time1 = DateTime.Now;
                foreach (var rec in diskCol)
                {
                    recovered.Add(rec);
                    if (cc == ridx)
                    {
                        testobj = rec;
                    }
                    cc++;
                }

                time2 = DateTime.Now;
                diff = time2 - time1;
                Console.WriteLine($"Cold Enumeration {cc:#,##0} records: {diff}");
                
                // Reset for next test
                cc = 0;
                recovered.Clear();

                // Clear and populate
                diskCol.Clear();
                time1 = DateTime.Now;
                var recs = 20480;
                for (var i = 0; i < recs; i++)
                {
                    diskCol.Add(new ExampleRecord());
                }
                time2 = DateTime.Now;
                diff = time2 - time1;

                Console.WriteLine($"Write {recs:#,##0} records: {diff}");
                Console.WriteLine($"Record size is now {diskCol.RecordSize}");

                // Enumerate
                time1 = DateTime.Now;
                foreach (var rec in diskCol)
                {
                    recovered.Add(rec);
                    if (cc == ridx)
                    {
                        testobj = rec;
                    }
                    cc++;
                }

                time2 = DateTime.Now;
                diff = time2 - time1;
                Console.WriteLine($"Enumerate {cc:#,##0} records: {diff}");

                // Spread
                time1 = DateTime.Now;
                List<ExampleRecord> testList = [.. diskCol];
                time2 = DateTime.Now;
                diff = time2 - time1;

                Console.WriteLine($"Spread to new list: {diff}");

                // Retrieve Item #1
                time1 = DateTime.Now;
                var testobj2 = diskCol[ridx];
                time2 = DateTime.Now;
                diff = time2 - time1;

                Console.WriteLine($"Retrieve item randomly at {ridx}: {diff}");

                // Retrieve Item #2
                time1 = DateTime.Now;
                var testobj3 = diskCol[ridx2];
                time2 = DateTime.Now;
                diff = time2 - time1;

                Console.WriteLine($"Retrieve item randomly at {ridx2}: {diff}");

                // Test equality
                time1 = DateTime.Now;
                var pass = testobj2.Equals(testobj);
                time2 = DateTime.Now;
                diff = time2 - time1;

                Console.WriteLine($"Test equality result is {pass}: {diff}");

                if (pass)
                {
                    // Remove two items
                    time1 = DateTime.Now;
                    diskCol.RemoveAt(ridx);
                    diskCol.RemoveAt(ridx2);
                    time2 = DateTime.Now;
                    diff = time2 - time1;
                    Console.WriteLine($"Delete 2 records at index {ridx}, {ridx2}: {diff}");
                }

                Console.WriteLine($"Record size is now {diskCol.RecordSize}");

                // Compact
                time1 = DateTime.Now;
                diskCol.Compact();
                time2 = DateTime.Now;
                diff = time2 - time1;
                Console.WriteLine($"Compact: {diff}");
                Console.WriteLine($"Record size is now {diskCol.RecordSize}");

                Console.WriteLine($"Size on Disk: {diskCol.Size:#,##0}");

                //diskCol.Clear();
            }

            Console.WriteLine("Press any key to continue...");
            Console.Read();


            DeviceInfo[] hids = HidDeviceInfo.EnumerateHidDevices();
            DeviceInfo[] batts = DeviceEnum.EnumerateDevices<DeviceInfo>(DevProp.GUID_DEVICE_BATTERY);
            var test = HidFeatures.HidDevicesByUsage(HidUsagePage.PowerDevice1);
            var battery = hids.Where((e) => e.DeviceClass == DeviceClassEnum.Battery).ToList().FirstOrDefault();

            if (battery == null)
            {
                battery = batts.FirstOrDefault();
            }

            if (battery == null)
            {
                Environment.Exit(0);
                return;
            }

            var batt2 = HidPowerDeviceInfo.CreateFromHidDevice(battery);
            batt2.OpenHid();

            var mstr = batt2.Manufacturer;
            var mstr2 = batt2.HidManufacturer;
            var pstr = batt2.ProductString;
            var sstr = batt2.SerialNumber;

            Console.Clear();
            Console.CursorVisible = false;

            Task.Delay(1000).Wait();

            bool printed = false;

            //PrintValCaps(batt2, batt2.FeatureValueCaps);
            //PrintValCaps(batt2, batt2.InputValueCaps);
            //PrintValCaps(batt2, batt2.OutputValueCaps);

            //Environment.Exit(0);

            Console.WriteLine("Press any key to begin...");
            Console.Read();

            Console.WriteLine("\x1b[2J\x1b[H");

            while (true)
            {
                Console.WriteLine("\x1b[H");

                Console.WriteLine(batt2.FriendlyName);
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

                Task.Delay(500).Wait();
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