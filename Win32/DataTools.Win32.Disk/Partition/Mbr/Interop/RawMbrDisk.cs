using DataTools.Win32.Disk.Interop;
using DataTools.Win32.Disk.Partition.Geometry;
using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace DataTools.Win32.Disk.Partition.Mbr
{
    /// <summary>
    /// Raw MBR-format disk reading tools
    /// </summary>
    internal static class RawMbrDisk
    {

        /// <summary>
        /// Entire MBR Disk Structure
        /// </summary>
        public struct RAW_MBR_INFO
        {
            /// <summary>
            /// The Master Boot Record
            /// </summary>
            public MasterBootRecord Mbr;

            /// <summary>
            /// Sector size
            /// </summary>
            public uint SectorSize;

            /// <summary>
            /// Extended Partitions
            /// </summary>
            public RawMbrPartitionStruct[] Extended;

            /// <summary>
            /// Primary Partition Count
            /// </summary>
            public int PrimaryPartitionCount;

            /// <summary>
            /// Extended Partition Count
            /// </summary>
            public int ExtendedPartitionCount;

            /// <summary>
            /// Total Partition Count
            /// </summary>
            public int PartitionCount;
        }

        /// <summary>
        /// Retrieve the partition table of a MBR-layout disk, manually.
        /// Must be Administrator.
        /// </summary>
        /// <param name="devicePath">Device Path of Disk Device.</param>
        /// <param name="sectorSize">The known sector size of the RAW MBR disk</param>
        /// <param name="rawMbrInfo">Receives the drive layout information.</param>
        /// <returns>True if successful.</returns>
        /// <remarks>
        /// Sector size must be correctly specified, even if it is 512 (and especially if it is not.)<br/>
        /// See <see cref="DiskGeometry.GetDiskGeometry(string, DiskHandle, out DISK_GEOMETRY_EX?)"/>.
        /// </remarks>
        public static bool ReadRawMbrDisk(string devicePath, int sectorSize, out RAW_MBR_INFO? rawMbrInfo)
        {
            MasterBootRecord mbrInfo;
            RAW_MBR_INFO rawInfo = default;

            rawMbrInfo = null;
            int pc = 0;

            List<RawMbrPartitionStruct> exParts = new List<RawMbrPartitionStruct>();

            // Demand Administrator for accessing a raw disk.
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            // Dim principalPerm As New PrincipalPermission(Nothing, "Administrators")
            // principalPerm.Demand()

            using (var hfile = DiskHandle.OpenDisk(devicePath))
            {
                uint bps = (uint)sectorSize;
                uint br = 0;

                using (var mm = new SafePtr(bps))
                {
                    var p = mm;

                    var res = IO.ReadFile(hfile, mm, bps, ref br, IntPtr.Zero);
                    
                    if (!res || br == 0)
                    {
                        throw new NativeException();
                    }

                    mbrInfo = mm.ToStruct<MasterBootRecord>();

                    rawInfo.Mbr = mbrInfo;
                    rawInfo.SectorSize = (uint)sectorSize;

                    foreach (var part in mbrInfo.PartTable)
                    {
                        if (part.PartType != 0) pc++;
                        var partTypes = MbrCodeInfo.FindByCode(part.PartType);

                        if (part.PartType == 0x05 || part.PartType == 0x0f)
                        {
                            // Ah, ye olde extended partition

                            long ebrStart = part.StartLBA * sectorSize;
                            long newFace = 0;
                            IO.SetFilePointerEx(hfile, ebrStart, ref newFace, IO.FilePointerMoveMethod.Begin);

                            mm.ZeroMemory();
                            res = IO.ReadFile(hfile, mm, bps, ref br, IntPtr.Zero);

                            if (!res || br == 0)
                            {
                                throw new NativeException();
                            }

                            MasterBootRecord ebrInfo = default;

                            do
                            {
                                ebrInfo = mm.ToStruct<MasterBootRecord>();
                                exParts.Add(ebrInfo.PartTable[0]);

                                if (ebrInfo.PartTable[1].StartLBA != 0)
                                {
                                    var nextEbrStart = ebrStart + (sectorSize * ebrInfo.PartTable[1].StartLBA);
                                    IO.SetFilePointerEx(hfile, nextEbrStart, ref newFace, IO.FilePointerMoveMethod.Begin);

                                    mm.ZeroMemory();
                                    res = IO.ReadFile(hfile, mm, bps, ref br, IntPtr.Zero);
                                }
                            } while (ebrInfo.PartTable[1].StartLBA != 0);
                        }
                    }
                }

                rawInfo.Extended = exParts.ToArray();

                rawInfo.PrimaryPartitionCount = pc;
                rawInfo.ExtendedPartitionCount = exParts.Count;

                if (exParts.Count > 0) pc -= 1;
                rawInfo.PartitionCount = pc + exParts.Count;

                rawMbrInfo = rawInfo;
                // we have succeeded.
                return true;
            }

        }
    }


}