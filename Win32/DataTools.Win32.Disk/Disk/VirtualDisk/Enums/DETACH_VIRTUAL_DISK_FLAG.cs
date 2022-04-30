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
    // DetachVirtualDisk
    //

    // Flags for DetachVirtualDisk
    [Flags()]
    public enum DETACH_VIRTUAL_DISK_FLAG
    {
        DETACH_VIRTUAL_DISK_FLAG_NONE = 0
    }
}
