// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: VDiskDecl
//         Port of virtdisk.h (in its entirety)
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.Runtime.InteropServices;


namespace DataTools.Win32.Disk
{
    // Versioned parameter structure for SetVirtualDiskInformation
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SET_VIRTUAL_DISK_INFO_PARENT_FILE_PATH
    {
        public SET_VIRTUAL_DISK_INFO_VERSION Version;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ParentFilePath;
    }
}
