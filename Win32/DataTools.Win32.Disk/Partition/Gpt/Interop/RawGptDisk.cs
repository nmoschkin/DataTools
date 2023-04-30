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
using DataTools.Win32.Disk.Partition.Mbr;
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

                // get the disk geometry to retrieve the sector (LBA) size.
                if (!DiskGeometry.GetDiskGeometry(null, hfile, out var outGeo) || outGeo == null)
                {
                    return false;
                }

                var geo = outGeo.Value;

                // sector size (usually 512 bytes)
                long bps = geo.Geometry.BytesPerSector;

                uint br = 0U;

                long lp = 0L;
                long lp2 = 0L;

                var pfsys = new List<string>();

                var ntfssig = BitConverter.ToUInt64(Encoding.UTF8.GetBytes("NTFS    "), 0);

                using (var mm = new SafePtr(bps * 2L))
                {
                    
                    IO.SetFilePointerEx(hfile, 0L, ref lp, IO.FilePointerMoveMethod.Begin);
                    IO.ReadFile(hfile, mm, (uint)(bps * 2L), ref br, IntPtr.Zero);
                    
                    var mbr = mm.ToStruct<MasterBootRecord>();

                    if (mbr.PartTable[0].PartType != 0xee) return false;

                    IO.SetFilePointerEx(hfile, mbr.PartTable[0].StartLBA * bps, ref lp, IO.FilePointerMoveMethod.Begin);
                    IO.ReadFile(hfile, mm, (uint)(bps * 2L), ref br, IntPtr.Zero);

                    RawGptPartitionStruct[] gpp = null;

                    // read the GPT structure header.
                    var gpt = mm.ToStruct<GptDiskHeader>();

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
                                pfsys.Add(FileSystemInfo.ReadFromDisk(hfile, bps, (long)((ulong)bps * gpp[i].StartingLBA)));

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
        /// Contains an entire raw GPT disk layout.
        /// </summary>
        /// <remarks></remarks>
        public struct RAW_GPT_DISK
        {
            /// <summary>
            /// Header
            /// </summary>
            public GptDiskHeader Header;

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
}
