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
    /// <summary>
    /// Only supported for Windows 8/Server 2012 and greater
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CREATE_VIRTUAL_DISK_PARAMETERS_V2
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
        public OPEN_VIRTUAL_DISK_FLAG OpenFlags;
        [MarshalAs(UnmanagedType.Struct)]
        public VIRTUAL_STORAGE_TYPE ParentVirtualStorageType;
        [MarshalAs(UnmanagedType.Struct)]
        public VIRTUAL_STORAGE_TYPE SourceVirtualStorageType;
        [MarshalAs(UnmanagedType.Struct)]
        public Guid ResiliencyGuid;
    }
}
