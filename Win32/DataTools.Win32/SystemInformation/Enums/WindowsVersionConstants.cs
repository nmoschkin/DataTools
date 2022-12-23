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
    /// Windows Version Constants - (Maj &lt;&lt; 8) | Min method.
    /// Minimum value is Vista.  Older versions of Windows are not supported.
    /// </summary>
    /// <remarks></remarks>
    public enum WindowsVersionConstants
    {

        /// <summary>
        /// Windows Vista
        /// </summary>
        /// <remarks></remarks>
        WindowsVista = 0x600,

        /// <summary>
        /// Windows 7
        /// </summary>
        /// <remarks></remarks>
        Windows7 = 0x601,

        /// <summary>
        /// Windows 8
        /// </summary>
        /// <remarks></remarks>
        Windows8 = 0x602,

        /// <summary>
        /// Windows 8.1
        /// </summary>
        /// <remarks></remarks>
        Windows81 = 0x603,

        /// <summary>
        /// Windows 10
        /// </summary>
        /// <remarks></remarks>
        Windows10 = 0xA00
    }
}
