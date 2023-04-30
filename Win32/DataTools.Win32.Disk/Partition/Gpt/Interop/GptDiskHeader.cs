// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: DiskApi
//         Native Disk Serivces.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.Runtime.InteropServices;
using DataTools.Win32.Memory;

namespace DataTools.Win32.Disk.Partition.Gpt
{
    /// <summary>
    /// Raw disk GPT partition table header.
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GptDiskHeader
    {
        /// <summary>
        /// GPT Partition Signature Constant
        /// </summary>
        public const long GPTSignature = 0x5452415020494645;

        /// <summary>
        /// Signature
        /// </summary>
        public ulong Signature;
        /// <summary>
        /// Revision
        /// </summary>
        public uint Revision;

        /// <summary>
        /// Header Size
        /// </summary>
        public uint HeaderSize;

        /// <summary>
        /// Header CRC-32
        /// </summary>
        public uint HeaderCRC32;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved;

        /// <summary>
        /// My LBA
        /// </summary>
        public ulong MyLBA;

        /// <summary>
        /// Alternate LBA
        /// </summary>
        public ulong AlternateLBA;

        /// <summary>
        /// First Usable LBA
        /// </summary>
        public ulong FirstUsableLBA;

        /// <summary>
        /// Max Usable LBA
        /// </summary>
        public ulong MaxUsableLBA;

        /// <summary>
        /// Disk Guid
        /// </summary>
        public Guid DiskGuid;

        /// <summary>
        /// Partition Entry LBA
        /// </summary>
        public ulong PartitionEntryLBA;

        /// <summary>
        /// Number Of Partitions
        /// </summary>
        public uint NumberOfPartitions;

        /// <summary>
        /// Partition Entry Length
        /// </summary>
        public uint PartitionEntryLength;

        /// <summary>
        /// Partition Array CRC-32
        /// </summary>
        public uint PartitionArrayCRC32;


        /// <summary>
        /// True if this structure contains a CRC-32 valid GPT partition header.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsValid => Validate();

        /// <summary>
        /// Validate the header and CRC-32 of this structure.
        /// </summary>
        /// <returns>True if the structure is valid.</returns>
        /// <remarks></remarks>
        public bool Validate()
        {
            using (var mm = new SafePtr())
            {
                mm.FromStruct(this);
                mm.UIntAt(4L) = 0U;

                // validate the crc and the signature moniker 
                return HeaderCRC32 == mm.CalculateCrc32() && Signature == GPTSignature;
            }
        }
    }
}
