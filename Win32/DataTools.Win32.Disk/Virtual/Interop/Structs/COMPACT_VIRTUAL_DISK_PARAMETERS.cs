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

namespace DataTools.Win32.Disk.Virtual
{
    // Versioned structure for CompactVirtualDisk
    internal struct COMPACT_VIRTUAL_DISK_PARAMETERS
    {
        public COMPACT_VIRTUAL_DISK_VERSION Version;
        public uint Reserved;
    }
}