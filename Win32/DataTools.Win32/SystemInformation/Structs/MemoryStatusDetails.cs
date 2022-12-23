// *************************************************
// DataTools C# Native Utility Library For Windows 
//
// Module: SystemInfo
//         Provides basic information about the
//         current computer.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************

using System.Runtime.InteropServices;

using DataTools.Text;

namespace DataTools.SystemInformation
{
    /// <summary>
    /// MEMORYSTATUSEX structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryStatusDetails
    {

        /// <summary>
        /// Length of this structure
        /// </summary>
        internal int dwLength;

        /// <summary>
        /// Memory load
        /// </summary>
        public FriendlySizeInteger MemoryLoad;

        /// <summary>
        /// Total physical memory on the machine
        /// </summary>
        public FriendlySizeLong TotalPhysicalMemory;

        /// <summary>
        /// Total available memroy on the machine
        /// </summary>
        public FriendlySizeLong AvailPhysicalMemory;

        /// <summary>
        /// Total paging file capacity
        /// </summary>
        public FriendlySizeLong TotalPageFile;

        /// <summary>
        /// Available paging file capacity
        /// </summary>
        public FriendlySizeLong AvailPageFile;

        /// <summary>
        /// Total virtual memory
        /// </summary>
        public FriendlySizeLong TotalVirtualMemory;

        /// <summary>
        /// Available virtual memory
        /// </summary>
        public FriendlySizeLong AvailVirtualMemory;

        /// <summary>
        /// Available extended virtual memory
        /// </summary>
        public FriendlySizeLong AvailExtendedVirtualMemory;
    }
}
