// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NetInfoApi
//         Windows Networking Api
//
//         Enums are documented in part from the API documentation at MSDN.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.Runtime.InteropServices;

using DataTools.Text;
using DataTools.Win32;

namespace DataTools.Win32.Network
{
    /// <summary>
    /// Windows API GROUP_INFO_2 structure.  Basic group information and attributes.
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct GroupInfo2
    {

        /// <summary>
        /// Group name
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Name;

        /// <summary>
        /// Comments
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;

        /// <summary>
        /// Group ID
        /// </summary>
        public int GroupId;

        /// <summary>
        /// Attributes
        /// </summary>
        public int Attributes;

        public override string ToString()
        {
            return Name;
        }
    }
}
