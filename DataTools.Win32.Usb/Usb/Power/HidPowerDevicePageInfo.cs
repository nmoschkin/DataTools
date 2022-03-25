using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DataTools.Win32.Usb
{
    public class HidPowerDevicePageInfo : HidUsagePageInfo<HidPowerUsageInfo>
    {

        public static HidPowerDevicePageInfo Instance { get; protected set; }

        static HidPowerDevicePageInfo()
        {
            Instance = new HidPowerDevicePageInfo();
        }

        protected override void Parse(params object[] values)
        {


            string s;

            if (values[0].GetType() == typeof(string))
            {
                object[] vparms = values.ToArray();
                s = (string)values[0];

                var sp = s.Split("-");

                ushort begin = ushort.Parse(sp[0], System.Globalization.NumberStyles.HexNumber);
                ushort end = ushort.Parse(sp[1], System.Globalization.NumberStyles.HexNumber);


                for (int i = begin; i <= end; i++)
                {
                    vparms[0] = i;
                    Parse(vparms);
                }

            }
            else
            {
                string sname = (string)values[1];
                string type;
                bool i = false, o = false, f = false, r = false, w = false;

                HidUsageType t = (HidUsageType)0;

                if (values.Length > 2)
                {
                    type = (string)values[2];
                    var st = type.Split("/");
                    foreach(var sx in st)
                    {
                        var tt = (HidUsageType)Enum.Parse(typeof(HidUsageType), sx);
                        t |= tt;
                    }
                }

                var std = "";

                if (values.Length == 5)
                {
                    std = (string)values[4];
                    // There is no I,O,F

                    s = (string)values[3];
                    r = (s == "R/W" || s == "R/O");
                    w = (s == "R/W");

                }
                else if (values.Length == 6)
                {
                    // There is I,O,F
                    std = (string)values[5];
                    var iof = (string)values[3];


                    i = iof.Contains("I");
                    o = iof.Contains("O");
                    f = iof.Contains("F");  

                    s = (string)values[4];
                    r = (s == "R/W" || s == "R/O");
                    w = (s == "R/W");

                }

                items.Add(new HidPowerUsageInfo()
                {
                    UsageId = (ushort)(int)values[0],
                    UsageName = sname,
                    UsageType = t,
                    Standard = std,
                    Input = i,
                    Output = o,
                    Feature = f,
                    AccessWrite = w,
                    AccessRead = r
                });

            }
            
        }

        protected HidPowerDevicePageInfo() : base(0x84)
        {
            Parse(0x01, "Name", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x02, "PresentStatus", "CL", "N/A", "4.1.1");
            Parse(0x03, "ChangedStatus", "CL", "N/A", "4.1.1");
            Parse(0x04, "UPS", "CA", "N/A", "4.1.1");
            Parse(0x05, "PowerSupply", "CA", "N/A", "4.1.1");
            Parse("06-0F", "Reserved");
            Parse(0x10, "BatterySystem", "CP", "N/A", "4.1.1");
            Parse(0x11, "BatterySystemID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x12, "Battery", "CP", "N/A", "4.1.1", "DC");
            Parse(0x13, "BatteryID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x14, "Charger", "CP", "N/A", "4.1.1");
            Parse(0x15, "ChargerID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x16, "PowerConverter", "CP", "N/A", "4.1.1");
            Parse(0x17, "PowerConverterID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x18, "OutletSystem", "CP", "N/A", "4.1.1");
            Parse(0x19, "OutletSystemID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x1A, "Input", "CP", "N/A", "4.1.1");
            Parse(0x1B, "InputID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x1C, "Output", "CP", "N/A", "4.1.1");
            Parse(0x1D, "OutputID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x1E, "Flow", "CP", "N/A", "4.1.1");
            Parse(0x1F, "FlowID", "Item", "I,F", "R/W", "4.1.1");
            Parse(0x20, "Outlet", "CP", "N/A", "4.1.1");
            Parse(0x21, "OutletID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x22, "Gang", "CL/CP", "N/A", "4.1.1");
            Parse(0x23, "GangID", "SV", "I,F", "R/W", "4.1.1");
            Parse(0x24, "PowerSummary", "CL/CP", "", "4.1.1");
            Parse(0x25, "PowerSummaryID", "SV", "I,F", "R/W", "4.1.1");
            Parse("26-2F", "Reserved");
            Parse(0x30, "Voltage", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x31, "Current", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x32, "Frequency", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x33, "ApparentPower", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x34, "ActivePower", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x35, "PercentLoad", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x36, "Temperature", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x37, "Humidity", "DV", "I,F", "R/O", "4.1.2");
            Parse(0x38, "BadCount", "DV", "I,F", "R/O", "4.1.2");
            Parse("39-3F", "Reserved");
            Parse(0x40, "ConfigVoltage", "SV/DV", "F", "R/W", "4.1.3");
            Parse(0x41, "ConfigCurrent", "SV/DV", "F", "R/W", "4.1.3");
            Parse(0x42, "ConfigFrequency", "SV/DV", "F", "R/W", "4.1.3");
            Parse(0x43, "ConfigApparentPower", "SV/DV", "F", "R/W", "4.1.3");
            Parse(0x44, "ConfigActivePower", "SV/DV", "F", "R/W", "4.1.3");
            Parse(0x45, "ConfigPercentLoad", "SV/DV", "F", "R/W", "4.1.3");
            Parse(0x46, "ConfigTemperature", "SV/DV", "F", "R/W", "4.1.3");
            Parse(0x47, "ConfigHumidity", "SV/DV", "F", "R/W", "4.1.3");
            Parse("48-4F", "Reserved");
            Parse(0x50, "SwitchOnControl", "DV", "F", "R/W", "4.1.4");
            Parse(0x51, "SwitchOffControl", "DV", "F", "R/W", "4.1.4");
            Parse(0x52, "ToggleControl", "DV", "F", "R/W", "4.1.4");
            Parse(0x53, "LowVoltageTransfer", "DV", "F", "R/W", "4.1.4");
            Parse(0x54, "HighVoltageTransfer", "DV", "F", "R/W", "4.1.4");
            Parse(0x55, "DelayBeforeReboot", "DV", "F", "R/W", "4.1.4");
            Parse(0x56, "DelayBeforeStartup", "DV", "F", "R/W", "4.1.4");
            Parse(0x57, "DelayBeforeShutdown", "DV", "F", "R/W", "4.1.4");
            Parse(0x58, "Test", "DV", "F", "R/W", "4.1.4");
            Parse(0x59, "ModuleReset", "DV", "F", "R/W", "4.1.4");
            Parse(0x5A, "AudibleAlarmControl", "DV", "F", "R/W", "4.1.4");
            Parse("5B-5F", "Reserved");
            Parse(0x60, "Present", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x61, "Good", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x62, "InternalFailure", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x63, "VoltageOutOfRange", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x64, "FrequencyOutOfRange", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x65, "Overload", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x66, "OverCharged", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x67, "OverTemperature", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x68, "ShutdownRequested", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x69, "ShutdownImminent", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x6A, "Reserved", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x6B, "SwitchOn/Off", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x6C, "Switchable", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x6D, "Used", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x6E, "Boost", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x6F, "Buck", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x70, "Initialized", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x71, "Tested", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x72, "AwaitingPower", "DF", "I,O,F", "R/W", "4.1.5");
            Parse(0x73, "CommunicationLost", "DF", "I,O,F", "R/W", "4.1.5");
            Parse("74-FC", "Reserved", "DF");
            Parse(0xFD, "Manufacturer", "SV", "F", "R/O", "4.1.6");
            Parse(0xFE, "Product", "SV", "F", "R/O", "4.1.6");
            Parse(0xFF, "SerialNumber", "SV", "F", "R/O", "4.1.6");


        }




    }
}
