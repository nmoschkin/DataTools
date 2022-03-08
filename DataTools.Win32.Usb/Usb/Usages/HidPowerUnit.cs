
namespace DataTools.Win32
{

    public enum PowerUnitCode
    {
        ACVoltage = 0x00F0D121,
        ACCurrent = 0x00100001,
        Frequency = 0xF001,
        DCVoltage = 0x00F0D121,
        Time = 0x1001,
        DCCurrent = 0x00100001,
        ApparentPower = 0xD121,
        Temperature = 0x00010001,
        BatteryCapacity = 0x00101001,
        None = 0x0
    }


    // 10 0001
    public sealed class HidPowerUnit
    {
        public string Name { get; private set; }
        
        public string PhysicalUnit { get; private set; }
            
        public string HIDUnit { get; private set; }

        public PowerUnitCode Code { get; private set; }
        
        public int Exponent { get; private set; }
        
        public int FieldSize { get; private set; }
     
        private HidPowerUnit(string name, string physUnit, string hidUnit, int unitCode, int exp, int fieldSize)
        {
            Name = name;
            PhysicalUnit = physUnit;
            HIDUnit = hidUnit;
            Code = (PowerUnitCode)unitCode;
            Exponent = exp;
            FieldSize = fieldSize;

            units.Add(units.Where((o) => o.Name == name).FirstOrDefault() ?? this);
        }

        public static HidPowerUnit? GetByCode(int code)
        {
            return units.Where((o) => o.Code == (PowerUnitCode)code).FirstOrDefault();
        }

        public static HidPowerUnit? GetByCode(PowerUnitCode code)
        {
            return units.Where((o) => o.Code == code).FirstOrDefault();
        }

        public override string ToString()
        {
            return Name;
        }

        static List<HidPowerUnit> units = new List<HidPowerUnit>();

        public static IReadOnlyList<HidPowerUnit> Units { get => units; private set => units = new List<HidPowerUnit>(value); } 

        public static readonly HidPowerUnit ACVoltage = new HidPowerUnit("AC Voltage", "Volt", "Volt", 0x00F0D121, 7, 8);
        public static readonly HidPowerUnit ACCurrent = new HidPowerUnit( "AC Current",  "centiAmp",  "Amp",  0x00100001, - 2, 16);
        public static readonly HidPowerUnit Frequency = new HidPowerUnit( "Frequency",  "Hertz",  "Hertz",  0xF001,  0, 8);
        public static readonly HidPowerUnit DCVoltage = new HidPowerUnit( "DC Voltage",  "centiVolt",  "Volt",  0x00F0D121,  5, 16);
        public static readonly HidPowerUnit Time = new HidPowerUnit( "Time",  "second",  "s",  0x1001,  0, 16);
        public static readonly HidPowerUnit DCCurrent = new HidPowerUnit( "DC Current",  "centiAmp",  "Amp",  0x00100001, - 2, 16);
        public static readonly HidPowerUnit ApparentPower = new HidPowerUnit( "Apparent or Active Power",  "VA or W",  "VA or W",  0xD121,  7, 16);
        public static readonly HidPowerUnit Temperature = new HidPowerUnit( "Temperature",  "°K",  "°K",  0x00010001,  0, 16);
        public static readonly HidPowerUnit BatteryCapacity = new HidPowerUnit( "Battery Capacity",  "AmpSec",  "AmpSec",  0x00101001,  0, 24);
        public static readonly HidPowerUnit None = new HidPowerUnit( "None",  "None",  "None",  0x0,  0, 8);

    }
}
