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
    [Flags()]
    public enum CREATE_VIRTUAL_DISK_FLAGS
    {
        CREATE_VIRTUAL_DISK_FLAG_NONE = 0x0,
        CREATE_VIRTUAL_DISK_FLAG_FULL_PHYSICAL_ALLOCATION = 0x1,
        CREATE_VIRTUAL_DISK_FLAG_PREVENT_WRITES_TO_SOURCE_DISK = 0x2,
        CREATE_VIRTUAL_DISK_FLAG_DO_NOT_COPY_METADATA_FROM_PARENT = 0x4
    }
}