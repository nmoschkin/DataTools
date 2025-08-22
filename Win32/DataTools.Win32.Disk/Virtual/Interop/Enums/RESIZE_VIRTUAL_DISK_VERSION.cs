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

namespace DataTools.Win32.Disk.Virtual
{
    //
    // ResizeVirtualDisk
    //

    // Version definitions
    internal enum RESIZE_VIRTUAL_DISK_VERSION
    {
        RESIZE_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
        RESIZE_VIRTUAL_DISK_VERSION_1 = 1
    }
}
