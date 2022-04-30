// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: VDiskDecl
//         Port of virtdisk.h (in its entirety)
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''


using System;
using System.Runtime.InteropServices;


namespace DataTools.Win32.Disk
{
    //
    // ResizeVirtualDisk
    //

    // Version definitions
    public enum RESIZE_VIRTUAL_DISK_VERSION
    {
        RESIZE_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
        RESIZE_VIRTUAL_DISK_VERSION_1 = 1
    }
}
