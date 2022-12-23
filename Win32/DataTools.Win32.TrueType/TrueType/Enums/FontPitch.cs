// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: TrueType.
//         Code to read TrueType font file information
//         Adapted from the CodeProject article: http://www.codeproject.com/Articles/2293/Retrieving-font-name-from-TTF-file?msg=4714219#xx4714219xx
//
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using DataTools.Win32;

namespace DataTools.Desktop
{
    public enum FontPitch
    {
        Default = FontPitchAndFamily.DEFAULT_PITCH,
        Variable = FontPitchAndFamily.VARIABLE_PITCH,
        Fixed = FontPitchAndFamily.FIXED_PITCH
    }
}
