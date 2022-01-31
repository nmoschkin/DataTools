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
    // NTDDI_VERSION >= NTDDI_WIN8

    //
    // MirrorVirtualDisk
    //

    // Version definitions
    public enum MIRROR_VIRTUAL_DISK_VERSION
    {
        MIRROR_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
        MIRROR_VIRTUAL_DISK_VERSION_1 = 1
    }
}
