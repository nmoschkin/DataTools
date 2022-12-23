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

namespace DataTools.SystemInformation
{
    /// <summary>
    /// Processor cache type
    /// </summary>
    public enum ProcessorCacheType
    {

        /// <summary>
        /// Unified
        /// </summary>
        CacheUnified,

        /// <summary>
        /// Instruction
        /// </summary>
        CacheInstruction,

        /// <summary>
        /// Data
        /// </summary>
        CacheData,

        /// <summary>
        /// Trace
        /// </summary>
        CacheTrace
    }
}
