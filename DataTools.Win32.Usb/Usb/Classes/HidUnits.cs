// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: UsbHid
//         HID-related structures, enums and functions.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DataTools.Win32
{
    public class HidUnits
    {
        private static List<HidUnit> _units = new List<HidUnit>();

        public static List<HidUnit> units
        {
            get
            {
                return _units;
            }
        }

        static HidUnits()
        {
            _units.Add(new HidUnit("AC Voltage", "Volt", "Volt", 0xF0D121, 7, 8));
            _units.Add(new HidUnit("AC Current", "centiAmp", "Amp", 0x100001, -2, 16));
            _units.Add(new HidUnit("Frequency", "Hertz", "Hertz", 0xF001, 0, 8));
            _units.Add(new HidUnit("DC Voltage", "centiVolt", "Volt", 0xF0D121, 5, 16));
            _units.Add(new HidUnit("Time", "second", "s", 0x1001, 0, 16));
            _units.Add(new HidUnit("DC Current", "centiAmp", "Amp", 0x100001, -2, 16));
            _units.Add(new HidUnit("Apparent or Active Power", "VA or W", "VA or W", 0xD121, 7, 16));
            _units.Add(new HidUnit("Temperature", "°K", "°K", 0x10001, 0, 16));
            _units.Add(new HidUnit("Battery Capacity", "AmpSec", "AmpSec", 0x101001, 0, 24));
            _units.Add(new HidUnit("None", "None", "None", 0x0, 0, 8));
        }

        public static HidUnit ByUnitCode(int code)
        {
            foreach (var hid in _units)
            {
                if (hid.HIDUnitCode == code)
                    return hid;
            }

            return null;
        }

        private HidUnits()
        {
        }
    }
}
