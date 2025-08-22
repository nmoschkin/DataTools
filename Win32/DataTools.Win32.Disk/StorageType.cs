// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Miscellaneous enums to support devices.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.ComponentModel;
using DataTools.Text;

namespace DataTools.Win32.Disk
{
    /// <summary>
    /// Specifies the storage type of the device.
    /// </summary>
    /// <remarks></remarks>
    public enum StorageType
    {
        /// <summary>
        /// Internal HDD or SDD
        /// </summary>
        HardDisk,

        /// <summary>
        /// Removeable HDD or SDD
        /// </summary>
        RemovableHardDisk,

        /// <summary>
        /// Removeable Drive
        /// </summary>
        /// <remarks>
        /// A drive that is neither HDD/SDD nor Optical (e.g. a thumb drive)
        /// </remarks>
        Removable,

        /// <summary>
        /// Virtual Drive
        /// </summary>
        Virtual,
        
        /// <summary>
        /// Network Server
        /// </summary>
        NetworkServer,

        /// <summary>
        /// Network Share
        /// </summary>
        NetworkShare,

        /// <summary>
        /// Optical Drive
        /// </summary>
        Optical,

        /// <summary>
        /// A disk volume
        /// </summary>
        Volume,

        /// <summary>
        /// A folder
        /// </summary>
        Folder,

        /// <summary>
        /// A file
        /// </summary>
        File
    }
}
