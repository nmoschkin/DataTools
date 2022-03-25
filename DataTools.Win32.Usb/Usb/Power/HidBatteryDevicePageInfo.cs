using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public class HidBatteryDevicePageInfo : HidUsagePageInfo<HidBatteryUsageInfo>
    {
        public static HidBatteryDevicePageInfo Instance { get; protected set; }

        static HidBatteryDevicePageInfo()
        {
            Instance = new HidBatteryDevicePageInfo();
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

                if (sp.Length > 1)
                {
                    ushort end = ushort.Parse(sp[1], System.Globalization.NumberStyles.HexNumber);

                    for (int i = begin; i <= end; i++)
                    {
                        vparms[0] = i;
                        Parse(vparms);
                    }
                }
                else
                {
                    vparms[0] = begin;
                    Parse(vparms);
                }

            }
            else
            {
                string sname = (string)values[1];
                string type;
                var sbml = false;
                bool i = false, o = false, f = false, r = false, w = false;

                HidUsageType t = (HidUsageType)0;

                if (values.Length > 2)
                {
                    type = (string)values[2];
                    var st = type.Split("/");
                    foreach (var sx in st)
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
                else if (values.Length >= 6)
                {
                    // There is I,O,F
                    std = (string)values[5];

                    if (std == "SBML" && values.Length == 7)
                    {
                        sbml = true;
                        std = (string)values[6];
                    }

                    var iof = (string)values[3];

                    i = iof.Contains("I");
                    o = iof.Contains("O");
                    f = iof.Contains("F");

                    s = (string)values[4];
                    r = (s == "R/W" || s == "R/O");
                    w = (s == "R/W");

                }

                items.Add(new HidBatteryUsageInfo()
                {
                    UsageId = (ushort)(int)values[0],
                    UsageName = sname,
                    UsageType = t,
                    Standard = std,
                    Input = i,
                    Output = o,
                    Feature = f,
                    AccessWrite = w,
                    AccessRead = r,
                    SBML = sbml
                });

            }

        }

        protected HidBatteryDevicePageInfo() : base(0x85)
        {
            Parse(0, "Undefined");
            Parse(0x01, "SMBBatteryMode", "CL", "", "N/A", "4.2.1");
            Parse(0x02, "SMBBatteryStatus", "CL", "", "N/A", "4.2.1");
            Parse(0x03, "SMBAlarmWarning", "CL", "", "N/A", "4.2.1");
            Parse(0x04, "SMBChargerMode", "CL", "", "N/A", "4.2.1");
            Parse(0x05, "SMBChargerStatus", "CL", "", "N/A", "4.2.1");
            Parse(0x06, "SMBChargerSpecInfo", "CL", "", "N/A", "4.2.1");
            Parse(0x07, "SMBSelectorState", "CL", "", "N/A", "4.2.1");
            Parse(0x08, "SMBSelectorPresets", "CL", "", "N/A", "4.2.1");
            Parse(0x09, "SMBSelectorInfo", "CL", "", "N/A", "4.2.1");
            Parse("0A-0F", "Reserved");
            Parse(0x10, "OptionalMfgFunction1", "DV", "F", "R/W", "4.2.2");
            Parse(0x11, "OptionalMfgFunction2", "DV", "F", "R/W", "4.2.2");
            Parse(0x12, "OptionalMfgFunction3", "DV", "F", "R/W", "4.2.2");
            Parse(0x13, "OptionalMfgFunction4", "DV", "F", "R/W", "4.2.2");
            Parse(0x14, "OptionalMfgFunction5", "DV", "F", "R/W", "4.2.2");
            Parse(0x15, "ConnectionToSMBus", "DF", "F", "R/W", "4.2.2");
            Parse(0x16, "OutputConnection", "DF", "F", "R/W", "4.2.2");
            Parse(0x17, "ChargerConnection", "DF", "F", "R/W", "4.2.2");
            Parse(0x18, "BatteryInsertion", "DF", "F", "R/W", "4.2.2");
            Parse(0x19, "Usenext", "DF", "F", "R/W", "4.2.2");
            Parse(0x1A, "OKToUse", "DF", "F", "R/W", "4.2.2");
            Parse(0x1B, "BatterySupported", "DF", "F", "R/O", "4.2.2");
            Parse(0x1C, "SelectorRevision", "DF", "F", "R/O", "4.2.2");
            Parse(0x1D, "ChargingIndicator", "DF", "F", "R/O", "4.2.2");
            Parse("1E-27", "Reserved");
            Parse(0x28, "ManufacturerAccess", "DV", "F", "R/W", "4.2.3");
            Parse(0x29, "RemainingCapacityLimit", "DV", "F", "R/W", "SBML", "4.2.3");
            Parse(0x2A, "RemainingTimeLimit", "DV", "F", "R/W", "SBML", "4.2.3");
            Parse(0x2B, "AtRate", "DV", "F", "R/W", "4.2.3");
            Parse(0x2C, "CapacityMode", "DV", "F", "R/W", "SBML", "4.2.3");
            Parse(0x2D, "BroadcastToCharger", "DV", "F", "R/W", "4.2.3");
            Parse(0x2E, "PrimaryBattery", "DV", "F", "R/W", "SBML", "4.2.3");
            Parse(0x2F, "ChargeController", "DV", "F", "R/W", "4.2.3");
            Parse("30-3F", "Reserved");
            Parse(0x40, "TerminateCharge", "DF", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x41, "TerminateDischarge", "DF", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x42, "BelowRemainingCapacityLimit", "DF", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x43, "RemainingTimeLimitExpired", "DF", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x44, "Charging", "DF", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x45, "Discharging", "DV", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x46, "FullyCharged", "DF", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x47, "FullyDischarged", "DV", "IOF", "R/W", "SBML", "4.2.4");
            Parse(0x48, "ConditioningFlag", "DV", "IOF", "R/W", "4.2.4");
            Parse(0x49, "AtRateOK", "DV", "IOF", "R/W", "4.2.4");
            Parse(0x4A, "SMBErrorCode", "DF", "IOF", "R/W", "4.2.4");
            Parse(0x4B, "NeedReplacement", "DF", "IOF", "R/W", "SBML", "4.2.4");
            Parse("4C-5F", "Reserved");
            Parse(0x60, "AtRateTimeToFull", "DV", "IF", "R/O", "4.2.5");
            Parse(0x61, "AtRateTimeToEmpty", "DV", "IF", "R/O", "4.2.5");
            Parse(0x62, "AverageCurrent", "DV", "IF", "R/O", "4.2.5");
            Parse(0x63, "Maxerror", "DV", "IF", "R/O", "4.2.5");
            Parse(0x64, "RelativeStateOfCharge", "DV", "IF", "R/O", "4.2.5");
            Parse(0x65, "AbsoluteStateOfCharge", "DV", "IF", "R/O", "SBML", "4.2.5");
            Parse(0x66, "RemainingCapacity", "DV", "IF", "R/O", "SBML", "4.2.5");
            Parse(0x67, "FullChargeCapacity", "DV", "IF", "R/O", "SBML", "4.2.5");
            Parse(0x68, "RunTimeToEmpty", "DV", "IF", "R/O", "SBML", "4.2.5");
            Parse(0x69, "AverageTimeToEmpty", "DV", "IF", "R/O", "4.2.5");
            Parse(0x6A, "AverageTimeToFull", "DV", "IF", "R/O", "4.2.5");
            Parse(0x6B, "CycleCount", "DV", "IF", "R/O", "SBML", "4.2.5");
            Parse("6C-7F", "Reserved");
            Parse(0x80, "BattPackModelLevel", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x81, "InternalChargeController", "SF", "F", "R/O", "4.2.6");
            Parse(0x82, "PrimaryBatterySupport", "SF", "F", "R/O", "SBML", "4.2.6");
            Parse(0x83, "DesignCapacity", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x84, "SpecificationInfo", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x85, "ManufacturerDate", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x86, "SerialNumber", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x87, "iManufacturerName", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x88, "iDevicename", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x89, "iDeviceChemistry", "SV", "F", "R/O", "SBML", "4.2.6");
            Parse(0x8A, "ManufacturerData", "SV", "F", "R/O", "4.2.6");
            Parse(0x8B, "Rechargable", "SV", "F", "R/O", "SBML", "4.2.7");
            Parse(0x8C, "WarningCapacityLimit", "SV", "F", "R/O", "SBML", "4.2.7");
            Parse(0x8D, "CapacityGranularity1", "SV", "F", "R/O", "SBML", "4.2.7");
            Parse(0x8E, "CapacityGranularity2", "SV", "F", "R/O", "SBML", "4.2.7");
            Parse(0x8F, "iOEMInformation", "SV", "F", "R/O", "SBML", "4.2.7");
            Parse("90-BF", "Reserved");
            Parse(0xC0, "InhibitCharge", "DF", "F", "R/W", "4.2.8");
            Parse(0xC1, "EnablePolling", "DF", "F", "R/W", "4.2.8");
            Parse(0xC2, "ResetToZero", "DF", "F", "R/W", "4.2.8");
            Parse("C3-CF", "Reserved");
            Parse(0xD0, "ACPresent", "DF", "IOF", "R/W", "SBML", "4.2.9");
            Parse(0xD1, "BatteryPresent", "DF", "IOF", "R/W", "SBML", "4.2.9");
            Parse(0xD2, "PowerFail", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xD3, "AlarmInhibited", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xD4, "ThermistorUnderRange", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xD5, "ThermistorHot", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xD6, "ThermistorCold", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xD7, "ThermistorOverRange", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xD8, "VoltageOutOfRange", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xD9, "CurrentOutOfRange", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xDA, "CurrentNotRegulated", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xDB, "VoltageNotRegulated", "DF", "IOF", "R/W", "4.2.9");
            Parse(0xDC, "MasterMode", "DF", "IOF", "R/W", "4.2.9");
            Parse("DD-EF", "Reserved");
            Parse(0xF0, "ChargerSelectorSupport", "SF", "F", "R/O", "4.2.10");
            Parse(0xF1, "ChargerSpec", "SV", "F", "R/O", "4.2.10");
            Parse(0xF2, "Level2", "SF", "F", "R/O", "4.2.10");
            Parse(0xF3, "Level3", "SF", "F", "R/O", "4.2.10");
            Parse("F2-FF", "Reserved");
        }

    }
}
