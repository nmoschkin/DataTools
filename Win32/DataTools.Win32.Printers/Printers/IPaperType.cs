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
    /// IPaperType interface
    /// </summary>
    public interface IPaperType
    {

        /// <summary>
        /// Paper type name
        /// </summary>
        /// <returns></returns>
        string Name { get; }

        /// <summary>
        /// True if orientation is landscape
        /// </summary>
        /// <returns></returns>
        bool IsLandscape { get; }

        /// <summary>
        /// If true, size is in metric units (millimeters).
        /// If false, size is in inches.
        /// </summary>
        /// <returns></returns>
        bool IsMetric { get; }

        /// <summary>
        /// Paper size
        /// </summary>
        /// <returns></returns>
        UniSize Size { get; }

        /// <summary>
        /// Compare one paper type to another paper type for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Equals(IPaperType other);
    }
}
