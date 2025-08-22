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

using System.Runtime.InteropServices;

namespace DataTools.Win32.Disk.Virtual
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct GET_VIRTUAL_DISK_INFO_LOGICAL_SECTOR_SIZE
    {
        public GET_VIRTUAL_DISK_INFO_VERSION Version;
        public uint LogicalSectorSize;
        public uint PhysicalSectorSize;

        [MarshalAs(UnmanagedType.Bool)]
        public bool IsRemote;
    }
}