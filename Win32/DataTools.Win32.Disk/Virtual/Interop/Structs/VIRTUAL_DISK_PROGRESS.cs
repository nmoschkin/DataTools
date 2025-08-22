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

namespace DataTools.Win32.Disk.Virtual
{
    // NTDDI_VERSION >= NTDDI_WIN8

    //
    // GetVirtualDiskOperationProgress
    //

    internal struct VIRTUAL_DISK_PROGRESS
    {
        public uint OperationStatus;
        public ulong CurrentValue;
        public ulong CompletionValue;
    }
}