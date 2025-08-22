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
    // Parameter structure for GetStorageDependencyInformation
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct STORAGE_DEPENDENCY_INFO_TYPE_1
    {
        public DEPENDENT_DISK_FLAG DependencyTypeFlags;
        public uint ProviderSpecificFlags;
        public VIRTUAL_STORAGE_TYPE VirtualStorageType;
    }
}