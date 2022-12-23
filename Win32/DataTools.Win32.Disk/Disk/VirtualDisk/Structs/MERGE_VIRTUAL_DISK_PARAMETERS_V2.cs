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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MERGE_VIRTUAL_DISK_PARAMETERS_V2
    {
        public MERGE_VIRTUAL_DISK_VERSION Version;
        public uint MergeSourceDepth;
        public uint MergeTargetDepth;
    }
}
