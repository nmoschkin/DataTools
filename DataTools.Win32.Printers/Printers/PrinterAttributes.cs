// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Printers
//         Windows Printer Api
//
//         Enums are documented in part from the API documentation at MSDN.
//         Other knowledge and references obtained through various sources
//         and all is considered public domain/common knowledge.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''


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
    /// Printer attributes flags
    /// </summary>
    [Flags]
    public enum PrinterAttributes : uint
    {

        /// <summary>
        /// Queued printer
        /// </summary>
        Queued = 0x1U,

        /// <summary>
        /// Direct printer
        /// </summary>
        Direct = 0x2U,

        /// <summary>
        /// Is default printer
        /// </summary>
        Default = 0x4U,

        /// <summary>
        /// Is shared printer
        /// </summary>
        Shared = 0x8U,

        /// <summary>
        /// Is network printer
        /// </summary>
        Network = 0x10U,

        /// <summary>
        /// Is hidden
        /// </summary>
        Hidden = 0x20U,

        /// <summary>
        /// Is a local printer
        /// </summary>
        Local = 0x40U,

        /// <summary>
        /// Enable DevQ
        /// </summary>
        EnableDevQ = 0x80U,

        /// <summary>
        /// Keep printed jobs
        /// </summary>
        KeepPrintedJobs = 0x100U,

        /// <summary>
        /// Do complete first
        /// </summary>
        DoCompleteFirst = 0x200U,

        /// <summary>
        /// Work offline
        /// </summary>
        WorkOffline = 0x400U,

        /// <summary>
        /// Enable BIDI
        /// </summary>
        EnableBIDI = 0x800U,

        /// <summary>
        /// Raw mode only
        /// </summary>
        RawOnly = 0x1000U,

        /// <summary>
        /// Published
        /// </summary>
        Published = 0x2000U,
        Reserved1 = 0x4000U,
        Reserved2 = 0x8000U,
        Reserved3 = 0x10000U
    }
}
