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
    // Versioned parameter structure for SetVirtualDiskInformation
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SET_VIRTUAL_DISK_INFO_VHD_SECTOR_SIZE
    {
        public SET_VIRTUAL_DISK_INFO_VERSION Version;
        public uint VhdPhysicalSectorSize;
    }
}
