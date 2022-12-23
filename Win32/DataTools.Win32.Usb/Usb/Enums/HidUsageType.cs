// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: UsbHid
//         HID-related structures, enums and functions.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Converters;

using Newtonsoft.Json;

using System.ComponentModel;

namespace DataTools.Win32.Usb
{
    [Flags]
    [JsonConverter(typeof(EnumToStringConverter<HidUsageType>))]
    public enum HidUsageType
    {
        /// <summary>
        /// Reserved
        /// </summary>
        [Description("Reserved")]
        Reserved = 0,

        /// <summary>
        /// Linear Control
        /// </summary>
        [Description("Linear Control")]
        LC = 0x1,

        /// <summary>
        /// On/Off Control
        /// </summary>
        [Description("On/Off Control")]
        OOC = 0x2,

        /// <summary>
        /// Momentary Control
        /// </summary>
        [Description("Momentary Control")]
        MC = 0x4,

        /// <summary>
        /// One Shot Control
        /// </summary>
        [Description("One Shot Control")]
        OSC = 0x8,

        /// <summary>
        /// Re-trigger Control
        /// </summary>
        [Description("Re-trigger Control")]
        RTC = 0x10,

        /// <summary>
        /// Sel Selector Array Contained in a Named Array
        /// </summary>
        [Description("Sel Selector Array Contained in a Named Array")]
        Sel = 0x20,

        /// <summary>
        /// Static Value
        /// </summary>
        /// <remarks>
        /// Constant, Variable, Absolute A read-only multiple-bit value.
        /// </remarks>
        [Description("Static Value")]
        SV = 0x40,

        /// <summary>
        /// Static Flag
        /// </summary>
        /// <remarks>
        /// Constant, Variable, Absolute A read-only single-bit value.
        /// </remarks>
        [Description("Static Flag")]
        SF = 0x80,

        /// <summary>
        /// Dynamic Value
        /// </summary>
        /// <remarks>
        /// Data, Variable, Absolute A read/write multiple-bit value.
        /// </remarks>
        [Description("Dynamic Value")]
        DV = 0x100,

        /// <summary>
        /// Dynamic Flag
        /// </summary>
        /// <remarks>
        /// Data, Variable, Absolute A read/write single-bit value.
        /// </remarks>
        [Description("Dynamic Flag")]
        DF = 0x200,

        /// <summary>
        /// Named Array
        /// </summary>
        /// <remarks>
        /// Logical A collection that encompasses an array definition, naming the array set or the field created by the array.
        /// </remarks>
        [Description("Named Array")]
        NAry = 0x400,

        /// <summary>
        /// Application Collection
        /// </summary>
        /// <remarks>
        /// Application Applies a name to a top level collection which the operating system uses to identify a device and possibly remap to a legacy API.
        /// </remarks>
        [Description("Application Collection")]
        CA = 0x800,

        /// <summary>
        /// Logical Collection
        /// </summary>
        /// <remarks>
        /// Logical A logical collection of items.
        /// </remarks>
        [Description("Logical Collection")]
        CL = 0x1000,

        /// <summary>
        /// Physical Collection
        /// </summary>
        /// <remarks>
        /// Physical A physical collection of items.
        /// </remarks>
        [Description("Physical Collection")]
        CP = 0x2000,

        /// <summary>
        /// Usage Switch
        /// </summary>
        /// <remarks>
        /// Logical Modifies the purpose or function of the Usages (controls) that it contains.
        /// </remarks>
        [Description("Usage Switch")]
        US = 0x4000,

        /// <summary>
        /// Usage Modifier
        /// </summary>
        /// <remarks>
        /// Logical Modifies the purpose or function of the Usages (controls) that contains it.
        /// </remarks>
        [Description("Usage Modifier")]
        UM = 0x8000,

        /// <summary>
        /// Item
        /// </summary>
        [Description("Item")]
        Item = 0x10000
    }
}