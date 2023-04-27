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
    // SetVirtualDiskInformation
    //

    // Version definitions
    internal enum SET_VIRTUAL_DISK_INFO_VERSION
    {
        SET_VIRTUAL_DISK_INFO_UNSPECIFIED = 0,
        SET_VIRTUAL_DISK_INFO_PARENT_PATH = 1,
        SET_VIRTUAL_DISK_INFO_IDENTIFIER = 2,
        SET_VIRTUAL_DISK_INFO_PARENT_PATH_WITH_DEPTH = 3,
        SET_VIRTUAL_DISK_INFO_PHYSICAL_SECTOR_SIZE = 4,
        SET_VIRTUAL_DISK_INFO_VIRTUAL_DISK_ID = 5
    }
}
