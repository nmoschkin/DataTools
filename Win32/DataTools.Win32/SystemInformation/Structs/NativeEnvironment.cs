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

namespace DataTools.SystemInformation
{
    /// <summary>
    /// System information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NativeEnvironment
    {

        /// <summary>
        /// Processor Architecture Type enumeration value.
        /// </summary>
        public readonly ArchitectureType ProcessorArchitecture;

        /// <summary>
        /// Reserved
        /// </summary>
        internal readonly short Reserved;

        /// <summary>
        /// System memory page size
        /// </summary>
        public readonly int PageSize;

        /// <summary>
        /// Minimum allowed application memory address
        /// </summary>
        public readonly nint MinimumApplicationAddress;

        /// <summary>
        /// Maximum allowed application memory address
        /// </summary>
        public readonly nint MaximumApplicationAddress;

        /// <summary>
        /// Active processor mask
        /// </summary>
        public readonly int ActiveProcessorMask;

        internal int nop;

        /// <summary>
        /// Number of processors on the local machine
        /// </summary>
        public int NumberOfProcessors
        {
            get => nop;
        }

        /// <summary>
        /// Processor type
        /// </summary>
        public readonly int ProcessorType;

        /// <summary>
        /// Allocation granularity
        /// </summary>
        public readonly int AllocationGranularity;

        /// <summary>
        /// Processor level
        /// </summary>
        public readonly short ProcessorLevel;

        /// <summary>
        /// Processor revision
        /// </summary>
        public readonly short ProcessorRevision;
    }
}
