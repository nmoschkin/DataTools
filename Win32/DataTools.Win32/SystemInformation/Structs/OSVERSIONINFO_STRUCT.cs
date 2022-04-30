// ************************************************* ''
// DataTools C# Native Utility Library For Windows 
//
// Module: SystemInfo
//         Provides basic information about the
//         current computer.
// 
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using System.Runtime.InteropServices;

namespace DataTools.SystemInformation
{
    /// <summary>
    /// Operating system version information
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct OSVERSIONINFO_STRUCT
    {

        /// <summary>
        /// Size of this structure
        /// </summary>
        public int OSVersionInfoSize;

        /// <summary>
        /// Major version
        /// </summary>
        public int MajorVersion;

        /// <summary>
        /// Minor version
        /// </summary>
        public int MinorVersion;

        /// <summary>
        /// Build number
        /// </summary>
        public int BuildNumber;

        /// <summary>
        /// Platform ID
        /// </summary>
        public int PlatformId;

        /// <summary>
        /// CSD Version
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string CSDVersion;
    }
}
