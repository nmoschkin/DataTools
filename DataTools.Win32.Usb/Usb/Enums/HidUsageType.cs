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
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DataTools.Win32
{
    [Flags]
    public enum HidUsageType
    {
        /// <summary>
        /// Reserved
        /// </summary>
        [Description("Reserved")]
        Reserved = 0,

        /// <summary>
        /// Application Collection
        /// </summary>
        [Description("Application Collection")]
        CA = 0x1,

        /// <summary>
        /// Logical Collection
        /// </summary>
        [Description("Logical Collection")]
        CL = 0x2,

        /// <summary>
        /// Physical Collection
        /// </summary>
        [Description("Physical Collection")]
        CP = 0x4,

        /// <summary>
        /// Dynamic Flag
        /// </summary>
        [Description("Dynamic Flag")]
        DF = 0x8,

        /// <summary>
        /// Dynamic Value
        /// </summary>
        [Description("Dynamic Value")]
        DV = 0x10,

        /// <summary>
        /// Static Flag
        /// </summary>
        [Description("Static Flag")]
        SF = 0x20,

        /// <summary>
        /// Static Value
        /// </summary>
        [Description("Static Value")]
        SV = 0x40,

        /// <summary>
        /// Item
        /// </summary>
        [Description("Item")]
        Item = 0x80
    }
}
