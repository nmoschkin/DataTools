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
    // Versioned parameter structure for MirrorVirtualDisk
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MIRROR_VIRTUAL_DISK_PARAMETERS
    {
        public MIRROR_VIRTUAL_DISK_VERSION Version;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string MirrorVirtualDiskPath;
    }
}
