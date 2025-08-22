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
    // NTDDI_VERSION >= NTDDI_WIN8

    //
    // MirrorVirtualDisk
    //

    // Version definitions
    internal enum MIRROR_VIRTUAL_DISK_VERSION
    {
        MIRROR_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
        MIRROR_VIRTUAL_DISK_VERSION_1 = 1
    }
}
