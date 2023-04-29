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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using DataTools.Text;
using DataTools.Win32.Disk.Interop;
using DataTools.Win32.Disk.Partition.Geometry;
using DataTools.Win32.Disk.Partition.Internals;
using DataTools.Win32.Memory;

namespace DataTools.Win32.Disk.Partition.Gpt
{
    /// <summary>
    /// Raw disk access.  Currently not exposed.
    /// </summary>
    /// <remarks></remarks>
    [SecurityCritical()]
    internal static class RawGptDisk
    {

        /// <summary>
        /// Retrieve the partition table of a GPT-layout disk, manually.
        /// Must be Administrator.
        /// </summary>
        /// <param name="devicePath">Device Path of Disk Device.</param>
        /// <param name="gptInfo">Receives the drive layout information.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public static bool ReadRawGptDisk(string devicePath, out RAW_GPT_DISK? gptInfo)
        {
            gptInfo = null;

            // Demand Administrator for accessing a raw disk.
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            // Dim principalPerm As New PrincipalPermission(Nothing, "Administrators")
            // principalPerm.Demand()


            using (var hfile = DiskHandle.OpenDisk(devicePath))
            {

                DISK_GEOMETRY_EX? outGeo = default;

                // get the disk geometry to retrieve the sector (LBA) size.
                if (!DiskGeometry.GetDiskGeometry(null, hfile, out outGeo))
                {
                    return false;
                }

                if (outGeo == null)
                {
                    return false;
                }

                var geo = (DISK_GEOMETRY_EX)outGeo;

                // sector size (usually 512 bytes)
                long bps = geo.Geometry.BytesPerSector;
                uint br = 0U;
                long lp = 0L;
                long lp2 = 0L;
                var pfsys = new List<string>();

                var ntfs = BitConverter.ToUInt64(Encoding.UTF8.GetBytes("NTFS    "), 0);

                using (var mm = new SafePtr(bps * 2L))
                {
                    IO.SetFilePointerEx(hfile, 0L, ref lp, IO.FilePointerMoveMethod.Begin);
                    IO.ReadFile(hfile, mm, (uint)(bps * 2L), ref br, IntPtr.Zero);
                    var mbr = new RAW_MBR();
                    var gpt = new RAW_GPT_HEADER();
                    RawGptPartitionStruct[] gpp = null;

                    // read the master boot record.
                    mbr = mm.ToStructAt<RAW_MBR>(446L);

                    // read the GPT structure header.
                    gpt = mm.ToStructAt<RAW_GPT_HEADER>(bps);

                    // check the partition header CRC.
                    if (gpt.IsValid)
                    {
                        long lr = br;

                        // seek to the LBA of the partition information.
                        IO.SetFilePointerEx(hfile, (long)((ulong)bps * gpt.PartitionEntryLBA), ref lr, IO.FilePointerMoveMethod.Begin);
                        br = (uint)lr;

                        // calculate the size of the partition table buffer.
                        lp = gpt.NumberOfPartitions * gpt.PartitionEntryLength;

                        // byte align to the sector size.
                        if (lp % bps != 0L)
                        {
                            lp += bps - lp % bps;
                        }

                        // bump up the memory pointer.
                        mm.AllocZero(lp);

                        // read the partition information into the pointer.
                        IO.ReadFile(hfile, mm, (uint)lp, ref br, IntPtr.Zero);

                        // check the partition table CRC.
                        if (mm.CalculateCrc32() == gpt.PartitionArrayCRC32)
                        {
                            // disk is valid.

                            lp = (uint)Marshal.SizeOf<RawGptPartitionStruct>();
                            br = 0U;

                            int i;
                            int c = (int)gpt.NumberOfPartitions;

                            gpp = new RawGptPartitionStruct[c + 1];

                            // populate the drive information.
                            for (i = 0; i < c; i++)
                            {
                                gpp[i] = mm.ToStructAt<RawGptPartitionStruct>(lp2);

                                // break on empty GUID, we are past the last partition.
                                if (gpp[i].PartitionTypeGuid == Guid.Empty) break;


                                // Some code to try to quickly determine the partition's file system ...

                                IO.SetFilePointerEx(hfile, (long)((ulong)bps * gpp[i].StartingLBA), ref lr, IO.FilePointerMoveMethod.Begin);
                                br = (uint)lr;

                                using (var expt = new SafePtr(bps))
                                {
                                    IO.ReadFile(hfile, expt, (uint)bps, ref br, IntPtr.Zero);

                                    var xf = expt.GetUTF8String(3).Trim();
                                    if (!string.IsNullOrEmpty(xf))
                                    {
                                        // It's something with a magic string where it's supposed to be (FAT or NTFS)
                                        pfsys.Add(xf);
                                    }
                                    else
                                    {
                                        // Let's just check one more place...

                                        IO.SetFilePointerEx(hfile, (long)((ulong)bps * gpp[i].StartingLBA) + 1024, ref lr, IO.FilePointerMoveMethod.Begin);

                                        expt.Free();
                                        expt.AllocZero(1024);

                                        IO.ReadFile(hfile, expt, 1024, ref br, IntPtr.Zero);

                                        var sb = expt.ToStruct<SuperBlock>();


                                        if (sb.s_magic == SuperBlock.MagicSignature)
                                        {
                                            // This is absolutely a Linux ext2/3/4 file system.
                                            pfsys.Add("ext4");
                                        }
                                        else
                                        {
                                            // No support for detecting other things, just yet.
                                            pfsys.Add("Unknown");

                                            // TODO: HPFS
                                        }
                                    }
                                }

                                lp2 += lp;
                            }

                            // trim off excess records from the array.
                            if (i < c)
                            {
                                if (i == 0)
                                {
                                    gpp = Array.Empty<RawGptPartitionStruct>();
                                }
                                else
                                {
                                    Array.Resize(ref gpp, i);
                                }
                            }
                        }
                    }

                    // if gpp is null then some error occurred somewhere and we did not succeed.
                    if (gpp == null) return false;

                    // create a new RAW_GPT_DISK structure.                    
                    gptInfo = new RAW_GPT_DISK()
                    {
                        Header = gpt,
                        Partitions = gpp,
                        PartitionFileSystems = pfsys.ToArray()
                    };

                    // we have succeeded.
                    return true;
                }
            }
        }

        /// <summary>
        /// Raw disk Master Boot Record entry.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RAW_MBR
        {
            /// <summary>
            /// Indicate bootability
            /// </summary>
            public byte BootIndicator;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 3)]
            private byte[] _startingChs;
            /// <summary>
            /// Partition type
            /// </summary>
            public byte PartitionType;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 3)]
            private byte[] _endingChs;
            /// <summary>
            /// Starting LBA
            /// </summary>
            public uint StartingLBA;

            /// <summary>
            /// Size in LBA
            /// </summary>
            public uint SizeInLBA;

            /// <summary>
            /// Starting CHS
            /// </summary>
            public int StartingCHS
            {
                get
                {
                    using (var mm = new SafePtr(4L))
                    {
                        Marshal.Copy(_startingChs, 0, mm, 3);
                        return mm.IntAt(0L);
                    }
                }
            }

            /// <summary>
            /// Ending CHS
            /// </summary>
            public int EndingCHS
            {
                get
                {
                    using (var mm = new SafePtr(4L))
                    {
                        Marshal.Copy(_endingChs, 0, mm, 3);
                        return mm.IntAt(0L);
                    }
                }
            }
        }

        /// <summary>
        /// Raw disk GPT partition table header.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RAW_GPT_HEADER
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

        /// <summary>
        /// Contains an entire raw GPT disk layout.
        /// </summary>
        /// <remarks></remarks>
        public struct RAW_GPT_DISK
        {
            /// <summary>
            /// Header
            /// </summary>
            public RAW_GPT_HEADER Header;

            /// <summary>
            /// Partitions
            /// </summary>
            public RawGptPartitionStruct[] Partitions;

            /// <summary>
            /// Partition File Systems
            /// </summary>
            public string[] PartitionFileSystems;
        }
    }

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
