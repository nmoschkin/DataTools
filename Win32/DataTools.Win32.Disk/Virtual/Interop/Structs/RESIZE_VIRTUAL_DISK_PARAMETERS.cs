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
    // Versioned parameter structure for ResizeVirtualDisk
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RESIZE_VIRTUAL_DISK_PARAMETERS
    {
        public RESIZE_VIRTUAL_DISK_VERSION Version;

        [MarshalAs(UnmanagedType.LPWStr)]
        public ulong NewSize;
    }
}