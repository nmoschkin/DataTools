// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Printers
//         Windows Printer Api
//
//         Enums are documented in part from the API documentation at MSDN.
//         Other knowledge and references obtained through various sources
//         and all is considered public domain/common knowledge.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using DataTools.Text;
using DataTools.MathTools;
using DataTools.Win32;
using DataTools.Win32.Memory;
using DataTools.Graphics;
using DataTools.MathTools.PolarMath;

namespace DataTools.Hardware.Printers
{
    /// <summary>
    /// Device mode fields flags
    /// </summary>
    [Flags]
    public enum DeviceModeFields : uint
    {

        /// <summary>
        /// Orientation
        /// </summary>
        Orientation = 0x1U,

        /// <summary>
        /// Paper size
        /// </summary>
        PaperSize = 0x2U,

        /// <summary>
        /// Paper length
        /// </summary>
        PaperLength = 0x4U,

        /// <summary>
        /// Paper width
        /// </summary>
        PaperWidth = 0x8U,

        /// <summary>
        /// Scale
        /// </summary>
        Scale = 0x10U,

        /// <summary>
        /// Position
        /// </summary>
        Position = 0x20U,

        /// <summary>
        /// Nup
        /// </summary>
        Nup = 0x40U,

        /// <summary>
        /// Display orientation
        /// </summary>
        DisplayOrientation = 0x80U,

        /// <summary>
        /// Copies
        /// </summary>
        Copies = 0x100U,

        /// <summary>
        /// Default source
        /// </summary>
        DefaultSource = 0x200U,

        /// <summary>
        /// Print quality
        /// </summary>
        PrintQuality = 0x400U,

        /// <summary>
        /// Color printer
        /// </summary>
        Color = 0x800U,

        /// <summary>
        /// Duplex support
        /// </summary>
        Duplex = 0x1000U,

        /// <summary>
        /// YR resolution
        /// </summary>
        YResolution = 0x2000U,

        /// <summary>
        /// TTOption
        /// </summary>
        TTOption = 0x4000U,

        /// <summary>
        /// Collate
        /// </summary>
        Collate = 0x8000U,

        /// <summary>
        /// Form name
        /// </summary>
        FormName = 0x10000U,

        /// <summary>
        /// Log pixels
        /// </summary>
        LogPixels = 0x20000U,

        /// <summary>
        /// Bits per pixel
        /// </summary>
        BitsPerPel = 0x40000U,

        /// <summary>
        /// Width in pixels
        /// </summary>
        PelsWidth = 0x80000U,

        /// <summary>
        /// Height in pixels
        /// </summary>
        PelsHeight = 0x100000U,

        /// <summary>
        /// Display flags
        /// </summary>
        DisplayFlags = 0x200000U,

        /// <summary>
        /// Display frequency
        /// </summary>
        DisplayFrequency = 0x400000U,

        /// <summary>
        /// ICM Method
        /// </summary>
        ICMMethod = 0x800000U,

        /// <summary>
        /// ICM Intent
        /// </summary>
        ICMIntent = 0x1000000U,

        /// <summary>
        /// Media type
        /// </summary>
        MediaType = 0x2000000U,

        /// <summary>
        /// Dither type
        /// </summary>
        DitherType = 0x4000000U,

        /// <summary>
        /// Panning width
        /// </summary>
        PanningWidth = 0x8000000U,

        /// <summary>
        /// Panning height
        /// </summary>
        PanningHeight = 0x10000000U
    }
}
