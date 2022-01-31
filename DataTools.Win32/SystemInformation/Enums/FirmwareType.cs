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

namespace DataTools.SystemInformation
{
    
    /// <summary>
    /// Computer firmware types
    /// </summary>
    /// <remarks></remarks>
    public enum FirmwareType
    {
        Unknown = 0,
        Bios = 1,
        Uefi = 2,
        Max = 3
    }
}
