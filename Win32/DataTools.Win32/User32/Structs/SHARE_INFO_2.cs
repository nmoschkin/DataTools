// ************************************************* ''
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
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''



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
    // Share folder information structure
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct SHARE_INFO_2
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string shi2_netname;
        public uint shi2_type;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string shi2_remark;
        public uint shi2_permissions;
        public uint shi2_max_users;
        public uint shi2_current_uses;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string shi2_path;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string shi2_passwd;
    }
}
