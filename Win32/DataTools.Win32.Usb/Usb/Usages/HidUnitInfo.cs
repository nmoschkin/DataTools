using System.Collections.Generic;
using System.Linq;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// HID Unit Info Codes
    /// </summary>
    public enum UnitInfoCode
    {
        /// <summary>
        /// AC Voltage
        /// </summary>
        ACVoltage = 0x00F0D121,
        
        /// <summary>
        /// AC Current
        /// </summary>
        ACCurrent = 0x00100001,

        /// <summary>
        /// Frequency
        /// </summary>
        Frequency = 0xF001,

        /// <summary>
        /// DC Voltage
        /// </summary>
        DCVoltage = 0x00F0D121,

        /// <summary>
        /// Time
        /// </summary>
        Time = 0x1001,

        /// <summary>
        /// DC Current
        /// </summary>
        DCCurrent = 0x00100001,

        /// <summary>
        /// Apparent Power
        /// </summary>
        ApparentPower = 0xD121,

        /// <summary>
        /// Temperature
        /// </summary>
        Temperature = 0x00010001,

        /// <summary>
        /// Battery Capacity
        /// </summary>
        BatteryCapacity = 0x00101001,

        /// <summary>
        /// None
        /// </summary>
        None = 0x0
    }

    /// <summary>
    /// HID Unit Info
    /// </summary>
    public sealed class HidUnitInfo
    {
        private static List<HidUnitInfo> units = new List<HidUnitInfo>();

        /// <summary>
        /// Name of the unit
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Physical unit
        /// </summary>
        public string PhysicalUnit { get; private set; }

        /// <summary>
        /// HID unit
        /// </summary>
        public string HIDUnit { get; private set; }

        /// <summary>
        /// Unit code
        /// </summary>
        public UnitInfoCode Code { get; private set; }

        /// <summary>
        /// Exponent
        /// </summary>
        public int Exponent { get; private set; }

        /// <summary>
        /// Field size
        /// </summary>
        public int FieldSize { get; private set; }

        private HidUnitInfo(string name, string physUnit, string hidUnit, int unitCode, int exp, int fieldSize)
        {
            Name = name;
            PhysicalUnit = physUnit;
            HIDUnit = hidUnit;
            Code = (UnitInfoCode)unitCode;
            Exponent = exp;
            FieldSize = fieldSize;

            units.Add(units.Where((o) => o.Name == name).FirstOrDefault() ?? this);
        }

        /// <summary>
        /// Get unit by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static HidUnitInfo GetByCode(int code)
        {
            return units.Where((o) => o.Code == (UnitInfoCode)code).FirstOrDefault();
        }

        /// <summary>
        /// Get unit by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static HidUnitInfo GetByCode(UnitInfoCode code)
        {
            return units.Where((o) => o.Code == code).FirstOrDefault();
        }

        /// <summary>
        /// Return the name of the unit
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        public static IReadOnlyList<HidUnitInfo> Units { get => units; private set => units = new List<HidUnitInfo>(value); }

        /// <summary>
        /// AC Voltage
        /// </summary>
        public static readonly HidUnitInfo ACVoltage = new HidUnitInfo("AC Voltage", "Volt", "Volt", 0x00F0D121, 7, 8);

        /// <summary>
        /// AC Current
        /// </summary>
        public static readonly HidUnitInfo ACCurrent = new HidUnitInfo("AC Current", "centiAmp", "Amp", 0x00100001, -2, 16);

        /// <summary>
        /// Frequency
        /// </summary>
        public static readonly HidUnitInfo Frequency = new HidUnitInfo("Frequency", "Hertz", "Hertz", 0xF001, 0, 8);
        
        /// <summary>
        /// DC Voltage
        /// </summary>
        public static readonly HidUnitInfo DCVoltage = new HidUnitInfo("DC Voltage", "centiVolt", "Volt", 0x00F0D121, 5, 16);
        
        /// <summary>
        /// Time
        /// </summary>
        public static readonly HidUnitInfo Time = new HidUnitInfo("Time", "second", "s", 0x1001, 0, 16);
        
        /// <summary>
        /// DC Current
        /// </summary>        
        public static readonly HidUnitInfo DCCurrent = new HidUnitInfo("DC Current", "centiAmp", "Amp", 0x00100001, -2, 16);
        
        /// <summary>
        /// Apparent Power
        /// </summary>
        public static readonly HidUnitInfo ApparentPower = new HidUnitInfo("Apparent or Active Power", "VA or W", "VA or W", 0xD121, 7, 16);
        
        /// <summary>
        /// Temperature
        /// </summary>
        public static readonly HidUnitInfo Temperature = new HidUnitInfo("Temperature", "°K", "°K", 0x00010001, 0, 16);
        
        /// <summary>
        /// Battery Capacity
        /// </summary>
        public static readonly HidUnitInfo BatteryCapacity = new HidUnitInfo("Battery Capacity", "AmpSec", "AmpSec", 0x00101001, 0, 24);

        /// <summary>
        /// None
        /// </summary>
        public static readonly HidUnitInfo None = new HidUnitInfo("None", "None", "None", 0x0, 0, 8);
    }
}