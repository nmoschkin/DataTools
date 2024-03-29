// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NativeShell
//         Wrappers for native and COM shell interfaces.
//
// Some enum documentation copied from the MSDN (and in some cases, updated).
// Some classes and interfaces were ported from the WindowsAPICodePack.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

//using DataTools.Hardware.MessageResources;
//using DataTools.Hardware;
//using DataTools.Hardware.Native;

namespace DataTools.Shell.Native
{
    /// <summary>
    /// Specifies the display types for a property.
    /// </summary>
    public enum PropertyDisplayType
    {
        /// <summary>
        /// The String Display. This is the default if the property doesn't specify a display type.
        /// </summary>
        String = 0,

        /// <summary>
        /// The Number Display.
        /// </summary>
        Number = 1,

        /// <summary>
        /// The Boolean Display.
        /// </summary>
        Boolean = 2,

        /// <summary>
        /// The DateTime Display.
        /// </summary>
        DateTime = 3,

        /// <summary>
        /// The Enumerated Display.
        /// </summary>
        Enumerated = 4
    }
}
