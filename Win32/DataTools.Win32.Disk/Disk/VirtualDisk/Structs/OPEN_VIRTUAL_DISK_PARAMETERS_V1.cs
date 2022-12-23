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
    // Versioned OpenVirtualDisk parameter structure
    public struct OPEN_VIRTUAL_DISK_PARAMETERS_V1
    {
        public OPEN_VIRTUAL_DISK_VERSION Version;
        public uint RWDepth;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 20)]
        public byte[] res;
    }
}
