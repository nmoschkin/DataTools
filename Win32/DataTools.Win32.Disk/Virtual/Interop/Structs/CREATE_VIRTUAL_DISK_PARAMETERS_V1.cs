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
    /// <summary>
    /// Only supported on Windows 7 and greater
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CREATE_VIRTUAL_DISK_PARAMETERS_V1
    {
        public CREATE_VIRTUAL_DISK_VERSION Version;

        [MarshalAs(UnmanagedType.Struct)]
        public Guid UniqueId;

        public ulong MaximumSize;
        public uint BlockSizeInBytes;
        public int SectorSizeInBytes;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string ParentPath;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string SourcePath;
    }
}