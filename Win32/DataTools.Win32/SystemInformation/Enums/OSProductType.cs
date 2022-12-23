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

using System.ComponentModel;

namespace DataTools.SystemInformation
{
    /// <summary>
    /// Windows product type information.
    /// </summary>
    /// <remarks></remarks>
    [Flags]
    public enum OSProductType : byte
    {

        /// <summary>
        /// The system is a domain controller and the operating system is Windows Server 2012 R2, Windows Server 2012, Windows Server 2008 R2, Windows Server 2008, Windows Server 2003, or Windows 2000 Server.
        /// </summary>
        [Description("The system is a domain controller and the operating system is Windows Server 2012 R2, Windows Server 2012, Windows Server 2008 R2, Windows Server 2008, Windows Server 2003, or Windows 2000 Server.")]
        NTDomainController = 0x2,

        /// <summary>
        /// The operating system is Windows Server 2012 R2, Windows Server 2012, Windows Server 2008 R2, Windows Server 2008, Windows Server 2003, or Windows 2000 Server.
        /// </summary>
        [Description("The operating system is Windows Server 2012 R2, Windows Server 2012, Windows Server 2008 R2, Windows Server 2008, Windows Server 2003, or Windows 2000 Server.")]
        NTServer = 0x3,

        /// <summary>
        /// The operating system is Windows 8.1, Windows 8, Windows 7, Windows Vista, Windows XP Professional, Windows XP Home Edition, or Windows 2000 Professional.
        /// </summary>
        [Description("The operating system is Windows 8.1, Windows 8, Windows 7, Windows Vista, Windows XP Professional, Windows XP Home Edition, or Windows 2000 Professional.")]
        NTWorkstation = 0x1
    }
}
