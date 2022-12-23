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
    // Flags for CompactVirtualDisk
    public enum COMPACT_VIRTUAL_DISK_FLAG
    {
        COMPACT_VIRTUAL_DISK_FLAG_NONE = 0,
        COMPACT_VIRTUAL_DISK_FLAG_NO_ZERO_SCAN = 1,
        COMPACT_VIRTUAL_DISK_FLAG_NO_BLOCK_MOVES = 2
    }
}
