using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DataTools.Win32.Usb.Usb.Power
{
    public enum PowerDeviceUsage : ushort
    {
        [Description("Undefined")]
        Undefined = 0x00,

        [Description("IName")]
        iName = 0x01,

        [Description("Present Status")]
        PresentStatus = 0x02,

        [Description("Changed Status")]
        ChangedStatus = 0x03,

        [Description("UPS")]
        UPS = 0x04,

        [Description("Power Supply")]
        PowerSupply = 0x05,

        [Description("Battery System")]
        BatterySystem = 0x10,

        [Description("Battery System ID")]
        BatterySystemID = 0x11,

        [Description("Battery")]
        Battery = 0x12,

        [Description("Battery ID")]
        BatteryID = 0x13,

        [Description("Charger")]
        Charger = 0x14,

        [Description("Charger ID")]
        ChargerID = 0x15,

        [Description("Power Converter")]
        PowerConverter = 0x16,

        [Description("Power Converter ID")]
        PowerConverterID = 0x17,

        [Description("Outlet System")]
        OutletSystem = 0x18,

        [Description("Outlet System ID")]
        OutletSystemID = 0x19,

        [Description("Input")]
        Input = 0x1a,

        [Description("Input ID")]
        InputID = 0x1b,

        [Description("Output")]
        Output = 0x1c,

        [Description("Output ID")]
        OutputID = 0x1d,

        [Description("Flow")]
        Flow = 0x1e,

        [Description("Flow ID")]
        FlowID = 0x1f,

        [Description("Outlet")]
        Outlet = 0x20,

        [Description("Outlet ID")]
        OutletID = 0x21,

        [Description("Gang")]
        Gang = 0x22,

        [Description("Gang ID")]
        GangID = 0x23,

        [Description("Power Summary")]
        PowerSummary = 0x24,

        [Description("Power Summary ID")]
        PowerSummaryID = 0x25,

        [Description("Voltage")]
        Voltage = 0x30,

        [Description("Current")]
        Current = 0x31,

        [Description("Frequency")]
        Frequency = 0x32,

        [Description("Apparent Power")]
        ApparentPower = 0x33,

        [Description("Active Power")]
        ActivePower = 0x34,

        [Description("Percent Load")]
        PercentLoad = 0x35,

        [Description("Temperature")]
        Temperature = 0x36,

        [Description("Humidity")]
        Humidity = 0x37,

        [Description("Bad Count")]
        BadCount = 0x38,

        [Description("Config Voltage")]
        ConfigVoltage = 0x40,

        [Description("Config Current")]
        ConfigCurrent = 0x41,

        [Description("Config Frequency")]
        ConfigFrequency = 0x42,

        [Description("Config Apparent Power")]
        ConfigApparentPower = 0x43,

        [Description("Config Active Power")]
        ConfigActivePower = 0x44,

        [Description("Config Percent Load")]
        ConfigPercentLoad = 0x45,

        [Description("Config Temperature")]
        ConfigTemperature = 0x46,

        [Description("Config Humidity")]
        ConfigHumidity = 0x47,

        [Description("Switch On Control")]
        SwitchOnControl = 0x50,

        [Description("Switch Off Control")]
        SwitchOffControl = 0x51,

        [Description("Toggle Control")]
        ToggleControl = 0x52,

        [Description("Low Voltage Transfer")]
        LowVoltageTransfer = 0x53,

        [Description("High Voltage Transfer")]
        HighVoltageTransfer = 0x54,

        [Description("Delay Before Reboot")]
        DelayBeforeReboot = 0x55,

        [Description("Delay Before Startup")]
        DelayBeforeStartup = 0x56,

        [Description("Delay Before Shutdown")]
        DelayBeforeShutdown = 0x57,

        [Description("Test")]
        Test = 0x58,

        [Description("Module Reset")]
        ModuleReset = 0x59,

        [Description("Audible Alarm Control")]
        AudibleAlarmControl = 0x5a,

        [Description("Present")]
        Present = 0x60,

        [Description("Good")]
        Good = 0x61,

        [Description("Internal Failure")]
        InternalFailure = 0x62,

        [Description("Voltage Out Of Range")]
        VoltageOutOfRange = 0x63,

        [Description("Frequency Out Of Range")]
        FrequencyOutOfRange = 0x64,

        [Description("Overload")]
        Overload = 0x65,

        [Description("Over Charged")]
        OverCharged = 0x66,

        [Description("Over Temperature")]
        OverTemperature = 0x67,

        [Description("Shutdown Requested")]
        ShutdownRequested = 0x68,

        [Description("Shutdown Imminent")]
        ShutdownImminent = 0x69,

        [Description("Switch On/Off")]
        SwitchOnOff = 0x6b,

        [Description("Switchable")]
        Switchable = 0x6c,

        [Description("Used")]
        Used = 0x6d,

        [Description("Boost")]
        Boost = 0x6e,

        [Description("Buck")]
        Buck = 0x6f,

        [Description("Initialized")]
        Initialized = 0x70,

        [Description("Tested")]
        Tested = 0x71,

        [Description("Awaiting Power")]
        AwaitingPower = 0x72,

        [Description("Communication Lost")]
        CommunicationLost = 0x73,

        [Description("IManufacturer")]
        iManufacturer = 0xfd,

        [Description("IProduct")]
        iProduct = 0xfe,

        [Description("Iserial Number")]
        iserialNumber = 0xff,

    }


    public enum BatteryDeviceUsage : ushort
    {
        [Description("Undefined")]
        Undefined = 0x00,

        [Description("SMBBattery Mode")]
        SMBBatteryMode = 0x01,

        [Description("SMBBattery Status")]
        SMBBatteryStatus = 0x02,

        [Description("SMBAlarm Warning")]
        SMBAlarmWarning = 0x03,

        [Description("SMBCharger Mode")]
        SMBChargerMode = 0x04,

        [Description("SMBCharger Status")]
        SMBChargerStatus = 0x05,

        [Description("SMBCharger Spec Info")]
        SMBChargerSpecInfo = 0x06,

        [Description("SMBSelector State")]
        SMBSelectorState = 0x07,

        [Description("SMBSelector Presets")]
        SMBSelectorPresets = 0x08,

        [Description("SMBSelector Info")]
        SMBSelectorInfo = 0x09,

        [Description("Optional Mfg Function1")]
        OptionalMfgFunction1 = 0x10,

        [Description("Optional Mfg Function2")]
        OptionalMfgFunction2 = 0x11,

        [Description("Optional Mfg Function3")]
        OptionalMfgFunction3 = 0x12,

        [Description("Optional Mfg Function4")]
        OptionalMfgFunction4 = 0x13,

        [Description("Optional Mfg Function5")]
        OptionalMfgFunction5 = 0x14,

        [Description("Connection To SMBus")]
        ConnectionToSMBus = 0x15,

        [Description("Output Connection")]
        OutputConnection = 0x16,

        [Description("Charger Connection")]
        ChargerConnection = 0x17,

        [Description("Battery Insertion")]
        BatteryInsertion = 0x18,

        [Description("Usenext")]
        Usenext = 0x19,

        [Description("OKTo Use")]
        OKToUse = 0x1a,

        [Description("Battery Supported")]
        BatterySupported = 0x1b,

        [Description("Selector Revision")]
        SelectorRevision = 0x1c,

        [Description("Charging Indicator")]
        ChargingIndicator = 0x1d,

        [Description("Manufacturer Access")]
        ManufacturerAccess = 0x28,

        [Description("Remaining Capacity Limit")]
        RemainingCapacityLimit = 0x29,

        [Description("Remaining Time Limit")]
        RemainingTimeLimit = 0x2a,

        [Description("At Rate")]
        AtRate = 0x2b,

        [Description("Capacity Mode")]
        CapacityMode = 0x2c,

        [Description("Broadcast To Charger")]
        BroadcastToCharger = 0x2d,

        [Description("Primary Battery")]
        PrimaryBattery = 0x2e,

        [Description("Charge Controller")]
        ChargeController = 0x2f,

        [Description("Terminate Charge")]
        TerminateCharge = 0x40,

        [Description("Terminate Discharge")]
        TerminateDischarge = 0x41,

        [Description("Below Remaining Capacity Limit")]
        BelowRemainingCapacityLimit = 0x42,

        [Description("Remaining Time Limit Expired")]
        RemainingTimeLimitExpired = 0x43,

        [Description("Charging")]
        Charging = 0x44,

        [Description("Discharging")]
        Discharging = 0x45,

        [Description("Fully Charged")]
        FullyCharged = 0x46,

        [Description("Fully Discharged")]
        FullyDischarged = 0x47,

        [Description("Conditioning Flag")]
        ConditioningFlag = 0x48,

        [Description("At Rate OK")]
        AtRateOK = 0x49,

        [Description("SMBError Code")]
        SMBErrorCode = 0x4a,

        [Description("Need Replacement")]
        NeedReplacement = 0x4b,

        [Description("At Rate Time To Full")]
        AtRateTimeToFull = 0x60,

        [Description("At Rate Time To Empty")]
        AtRateTimeToEmpty = 0x61,

        [Description("Average Current")]
        AverageCurrent = 0x62,

        [Description("Maxerror")]
        Maxerror = 0x63,

        [Description("Relative State Of Charge")]
        RelativeStateOfCharge = 0x64,

        [Description("Absolute State Of Charge")]
        AbsoluteStateOfCharge = 0x65,

        [Description("Remaining Capacity")]
        RemainingCapacity = 0x66,

        [Description("Full Charge Capacity")]
        FullChargeCapacity = 0x67,

        [Description("Run Time To Empty")]
        RunTimeToEmpty = 0x68,

        [Description("Average Time To Empty")]
        AverageTimeToEmpty = 0x69,

        [Description("Average Time To Full")]
        AverageTimeToFull = 0x6a,

        [Description("Cycle Count")]
        CycleCount = 0x6b,

        [Description("Batt Pack Model Level")]
        BattPackModelLevel = 0x80,

        [Description("Internal Charge Controller")]
        InternalChargeController = 0x81,

        [Description("Primary Battery Support")]
        PrimaryBatterySupport = 0x82,

        [Description("Design Capacity")]
        DesignCapacity = 0x83,

        [Description("Specification Info")]
        SpecificationInfo = 0x84,

        [Description("Manufacturer Date")]
        ManufacturerDate = 0x85,

        [Description("Serial Number")]
        SerialNumber = 0x86,

        [Description("IManufacturer Name")]
        iManufacturerName = 0x87,

        [Description("IDevicename")]
        iDevicename = 0x88,

        [Description("IDevice Chemistery")]
        iDeviceChemistery = 0x89,

        [Description("Manufacturer Data")]
        ManufacturerData = 0x8a,

        [Description("Rechargable")]
        Rechargable = 0x8b,

        [Description("Warning Capacity Limit")]
        WarningCapacityLimit = 0x8c,

        [Description("Capacity Granularity1")]
        CapacityGranularity1 = 0x8d,

        [Description("Capacity Granularity2")]
        CapacityGranularity2 = 0x8e,

        [Description("IOEMInformation")]
        iOEMInformation = 0x8f,

        [Description("Inhibit Charge")]
        InhibitCharge = 0xc0,

        [Description("Enable Polling")]
        EnablePolling = 0xc1,

        [Description("Reset To Zero")]
        ResetToZero = 0xc2,

        [Description("ACPresent")]
        ACPresent = 0xd0,

        [Description("Battery Present")]
        BatteryPresent = 0xd1,

        [Description("Power Fail")]
        PowerFail = 0xd2,

        [Description("Alarm Inhibited")]
        AlarmInhibited = 0xd3,

        [Description("Thermistor Under Range")]
        ThermistorUnderRange = 0xd4,

        [Description("Thermistor Hot")]
        ThermistorHot = 0xd5,

        [Description("Thermistor Cold")]
        ThermistorCold = 0xd6,

        [Description("Thermistor Over Range")]
        ThermistorOverRange = 0xd7,

        [Description("Voltage Out Of Range")]
        VoltageOutOfRange = 0xd8,

        [Description("Current Out Of Range")]
        CurrentOutOfRange = 0xd9,

        [Description("Current Not Regulated")]
        CurrentNotRegulated = 0xda,

        [Description("Voltage Not Regulated")]
        VoltageNotRegulated = 0xdb,

        [Description("Master Mode")]
        MasterMode = 0xdc,

        [Description("Charger Selector Support")]
        ChargerSelectorSupport = 0xf0,

        [Description("Charger Spec")]
        ChargerSpec = 0xf1,

        [Description("Level2")]
        Level2 = 0xf2,

        [Description("Level3")]
        Level3 = 0xf3,

    }

}
