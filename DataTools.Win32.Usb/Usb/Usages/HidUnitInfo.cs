
namespace DataTools.Win32
{

    public enum UnitInfoCode
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
    public sealed class HidUnitInfo
    {
        public string Name { get; private set; }
        
        public string PhysicalUnit { get; private set; }
            
        public string HIDUnit { get; private set; }

        public UnitInfoCode Code { get; private set; }
        
        public int Exponent { get; private set; }
        
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

        public static HidUnitInfo? GetByCode(int code)
        {
            return units.Where((o) => o.Code == (UnitInfoCode)code).FirstOrDefault();
        }

        public static HidUnitInfo? GetByCode(UnitInfoCode code)
        {
            return units.Where((o) => o.Code == code).FirstOrDefault();
        }

        public override string ToString()
        {
            return Name;
        }

        static List<HidUnitInfo> units = new List<HidUnitInfo>();

        public static IReadOnlyList<HidUnitInfo> Units { get => units; private set => units = new List<HidUnitInfo>(value); } 

        public static readonly HidUnitInfo ACVoltage = new HidUnitInfo("AC Voltage", "Volt", "Volt", 0x00F0D121, 7, 8);
        public static readonly HidUnitInfo ACCurrent = new HidUnitInfo( "AC Current",  "centiAmp",  "Amp",  0x00100001, - 2, 16);
        public static readonly HidUnitInfo Frequency = new HidUnitInfo( "Frequency",  "Hertz",  "Hertz",  0xF001,  0, 8);
        public static readonly HidUnitInfo DCVoltage = new HidUnitInfo( "DC Voltage",  "centiVolt",  "Volt",  0x00F0D121,  5, 16);
        public static readonly HidUnitInfo Time = new HidUnitInfo( "Time",  "second",  "s",  0x1001,  0, 16);
        public static readonly HidUnitInfo DCCurrent = new HidUnitInfo( "DC Current",  "centiAmp",  "Amp",  0x00100001, - 2, 16);
        public static readonly HidUnitInfo ApparentPower = new HidUnitInfo( "Apparent or Active Power",  "VA or W",  "VA or W",  0xD121,  7, 16);
        public static readonly HidUnitInfo Temperature = new HidUnitInfo( "Temperature",  "°K",  "°K",  0x00010001,  0, 16);
        public static readonly HidUnitInfo BatteryCapacity = new HidUnitInfo( "Battery Capacity",  "AmpSec",  "AmpSec",  0x00101001,  0, 24);
        public static readonly HidUnitInfo None = new HidUnitInfo( "None",  "None",  "None",  0x0,  0, 8);

    }
}
