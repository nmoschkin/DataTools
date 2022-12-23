// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Miscellaneous enums to support devices.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.ComponentModel;
using DataTools.Text;
using DataTools.Win32;

namespace DataTools.Win32
{
    /// <summary>
    /// Specifies the type of the device.
    /// </summary>
    /// <remarks></remarks>
    public enum DeviceType
    {
        Disk,
        Network,
        Usb,
        Volume
    }
}
