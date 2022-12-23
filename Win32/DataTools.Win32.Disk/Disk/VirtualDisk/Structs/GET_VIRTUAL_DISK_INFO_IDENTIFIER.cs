// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: VDiskDecl
//         Port of virtdisk.h (in its entirety)
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.Runtime.InteropServices;


namespace DataTools.Win32.Disk
{
    //

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct GET_VIRTUAL_DISK_INFO_IDENTIFIER
    {
        public GET_VIRTUAL_DISK_INFO_VERSION Version;
        [MarshalAs(UnmanagedType.Struct)]
        public Guid Identifier;
    }
}
