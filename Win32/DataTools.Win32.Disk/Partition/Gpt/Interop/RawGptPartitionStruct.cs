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

namespace DataTools.Win32.Disk.Partition.Gpt
{
    /// <summary>
    /// Raw GPT partition information.
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RawGptPartitionStruct
    {
        /// <summary>
        /// Partition Type Guid
        /// </summary>
        public Guid PartitionTypeGuid;

        /// <summary>
        /// Unique Partition Guid
        /// </summary>
        public Guid UniquePartitionGuid;

        /// <summary>
        /// Starting LBA
        /// </summary>
        public ulong StartingLBA;

        /// <summary>
        /// Ending LBA
        /// </summary>
        public ulong EndingLBA;

        /// <summary>
        /// Partition Attributes
        /// </summary>
        public GptPartitionAttributes Attributes;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 36)]
        private char[] _Name;

        /// <summary>
        /// Get the starting byte offset on disk of this partition
        /// </summary>
        /// <param name="sectorSize"></param>
        /// <returns></returns>
        public ulong GetByteOffset(ulong sectorSize = 512)
        {
            return StartingLBA * sectorSize;
        }

        /// <summary>
        /// Returns the size of the partition in bytes
        /// </summary>
        public ulong Size
        {
            get
            {
                return (EndingLBA - StartingLBA) * 512;
            }
        }

        /// <summary>
        /// Returns the name of this partition (if any).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Name
        {
            get
            {
                return new string(_Name).Trim('\0');
            }
        }

        /// <summary>
        /// Retrieve the partition code information for this partition type (if any).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public GptCodeInfo PartitionCode
        {
            get
            {
                return GptCodeInfo.FindByCode(PartitionTypeGuid);
            }
        }

        /// <summary>
        /// Converts this object into its string representation.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            string ToStringRet = default;
            if (!string.IsNullOrEmpty(Name))
            {
                ToStringRet = Name;
            }
            else if (PartitionCode is object)
            {
                ToStringRet = PartitionCode.ToString();
            }
            else
            {
                ToStringRet = UniquePartitionGuid.ToString("B");
            }

            return ToStringRet;
        }
    }


}
