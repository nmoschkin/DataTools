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
    
    // This is TTF file header
    [StructLayout(LayoutKind.Sequential)]
    public struct TT_OFFSET_TABLE
    {
        public ushort uMajorVersion;
        public ushort uMinorVersion;
        public ushort uNumOfTables;
        public ushort uSearchRange;
        public ushort uEntrySelector;
        public ushort uRangeShift;
    }
}
