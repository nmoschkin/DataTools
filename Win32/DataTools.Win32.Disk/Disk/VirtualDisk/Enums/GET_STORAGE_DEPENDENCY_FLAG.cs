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
    /// Flags for GetStorageDependencyInformation
    /// </summary>
    [Flags()]
    internal enum GET_STORAGE_DEPENDENCY_FLAG
    {
        /// <summary>
        /// None
        /// </summary>
        GET_STORAGE_DEPENDENCY_FLAG_NONE = 0,

        /// <summary>
        /// Return information for volumes or disks hosting the volume specified 
        /// If not set, returns info about volumes or disks being hosted by
        /// the volume or disk specified 
        /// </summary>
        GET_STORAGE_DEPENDENCY_FLAG_HOST_VOLUMES = 1,

        /// <summary>
        /// The handle provided is to a disk, not volume or file
        /// </summary>
        GET_STORAGE_DEPENDENCY_FLAG_DISK_HANDLE = 2
    }
}
