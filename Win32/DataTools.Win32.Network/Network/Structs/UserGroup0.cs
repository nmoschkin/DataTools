// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NetInfoApi
//         Windows Networking Api
//
//         Enums are documented in part from the API documentation at MSDN.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''


using System;
using System.Runtime.InteropServices;

using DataTools.Text;
using DataTools.Win32;

namespace DataTools.Win32.Network
{
    /// <summary>
    /// UserGroup0 structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UserGroup0
    {

        /// <summary>
        /// User group name
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Name;
    }
}
