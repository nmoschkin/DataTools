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
    /// Paper nationalities
    /// </summary>
    public enum PaperNationalities
    {

        /// <summary>
        /// American
        /// </summary>
        American,

        /// <summary>
        /// ISO / International Standard
        /// </summary>
        Iso,

        /// <summary>
        /// Japanese
        /// </summary>
        Japanese,

        /// <summary>
        /// German
        /// </summary>
        German,

        /// <summary>
        /// Chinese
        /// </summary>
        Chinese
    }
}
