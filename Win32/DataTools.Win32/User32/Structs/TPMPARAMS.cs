// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Native
//         Myriad Windows API Declares
//
// Started in 2000 on Windows 98/ME (and then later 2000).
//
// Still kicking in 2014 on Windows 8.1!
// A whole bunch of pInvoke/Const/Declare/Struct and associated utility functions that have been collected over the years.

// Some enum documentation copied from the MSDN (and in some cases, updated).
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using DataTools.Win32;

namespace DataTools.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct TPMPARAMS
    {
        public int cbSize;
        [MarshalAs(UnmanagedType.Struct)]
        public W32RECT rcExclude;
    }
}
