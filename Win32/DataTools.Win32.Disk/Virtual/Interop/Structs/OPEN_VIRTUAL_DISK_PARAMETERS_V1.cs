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
    // Versioned OpenVirtualDisk parameter structure
    internal struct OPEN_VIRTUAL_DISK_PARAMETERS_V1
    {
        public OPEN_VIRTUAL_DISK_VERSION Version;
        public uint RWDepth;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 20)]
        public byte[] res;
    }
}