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
    public static class RawMbrDisk
    {
        /// <summary>
        /// MBR Disk Partition
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Partition
        {
            /// <summary>
            /// 0x80 for bootable, 0x00 for not bootable, anything else for invalid
            /// </summary>
            public byte Status;

            /// <summary>
            /// Head address of start of partition
            /// </summary>
            public byte StartAddrHead;
            
            /// <summary>
            /// (AddrCylSec &amp; 0x3F) for sector,  (AddrCylSec &amp; 0x3FF) for cylinder
            /// </summary>
            public ushort StartAddrCylSec;

            /// <summary>
            /// Partition type code (see <see cref="PartitionCodeInfo"/>)
            /// </summary>
            public byte PartType;
            
            /// <summary>
            /// Head address of start of partition
            /// </summary>
            public byte EndAddrHead;
            
            /// <summary>
            /// (AddrCylSec &amp; 0x3F) for sector,  (AddrCylSec &amp; 0x3FF) for cylinder
            /// </summary>
            public ushort EndAddrCylSec;
            
            /// <summary>
            /// Linear address of first sector in partition. Multiply by sector size (usually 512) for real offset
            /// </summary>
            public uint StartLBA;
            
            /// <summary>
            /// Linear address of last sector in partition. Multiply by sector size (usually 512) for real offset
            /// </summary>
            public uint EndLBA;
        }

        /// <summary>
        /// Master Boot Record structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MBR
        {
            /// <summary>
            /// Byte Code
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 440)]
            public byte[] Code;

            /// <summary>
            /// Optional
            /// </summary>
            public uint DiskSig; //This is optional
            
            /// <summary>
            /// Reserved
            /// </summary>
            public ushort Reserved; //Usually 0x0000

            /// <summary>
            /// Partition Table
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Partition[] PartTable;

            /// <summary>
            /// 0x55 0xAA for bootable
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] BootSignature; //0x55 0xAA for bootable
        }

        /// <summary>
        /// Entire MBR Disk Structure
        /// </summary>
        public struct RAW_MBR_INFO
        {
            /// <summary>
            /// The Master Boot Record
            /// </summary>
            public MBR Mbr;

            /// <summary>
            /// Sector size
            /// </summary>
            public uint SectorSize;

            /// <summary>
            /// Extended Partitions
            /// </summary>
            public Partition[] Extended;

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
        /// <param name="rawMbrInfo">Receives the drive layout information.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public static bool ReadRawMbrDisk(string devicePath, int sectorSize, out RAW_MBR_INFO? rawMbrInfo)
        {
            MBR mbrInfo;
            RAW_MBR_INFO rawInfo = default;

            rawMbrInfo = null;
            int pc = 0;

            List<Partition> exParts = new List<Partition>();

            // Demand Administrator for accessing a raw disk.
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            // Dim principalPerm As New PrincipalPermission(Nothing, "Administrators")
            // principalPerm.Demand()

            using (var hfile = DiskHandle.OpenDisk(devicePath))
            {
                uint bps = (uint)Marshal.SizeOf<MBR>();
                uint br = 0;

                using (var mm = new SafePtr(bps))
                {
                    var p = mm;

                    var res = IO.ReadFile(hfile, mm, bps, ref br, IntPtr.Zero);
                    
                    if (!res || br == 0)
                    {
                        throw new NativeException();
                    }

                    mbrInfo = mm.ToStruct<MBR>();

                    rawInfo.Mbr = mbrInfo;
                    rawInfo.SectorSize = (uint)sectorSize;

                    foreach (var part in mbrInfo.PartTable)
                    {
                        if (part.PartType != 0) pc++;
                        var partTypes = PartitionCodeInfo.FindByCode(part.PartType);

                        if (part.PartType == 0x05 || part.PartType == 0x0f)
                        {
                            long ebrStart = part.StartLBA * sectorSize;
                            long newFace = 0;
                            IO.SetFilePointerEx(hfile, ebrStart, ref newFace, IO.FilePointerMoveMethod.Begin);

                            mm.ZeroMemory();
                            res = IO.ReadFile(hfile, mm, bps, ref br, IntPtr.Zero);

                            if (!res || br == 0)
                            {
                                throw new NativeException();
                            }

                            MBR ebrInfo = default;

                            do
                            {
                                ebrInfo = mm.ToStruct<MBR>();
                                exParts.Add(ebrInfo.PartTable[0]);

                                if (ebrInfo.PartTable[1].StartLBA != 0)
                                {
                                    var nextEbrStart = ebrStart + (sectorSize * ebrInfo.PartTable[1].StartLBA);
                                    IO.SetFilePointerEx(hfile, nextEbrStart, ref newFace, IO.FilePointerMoveMethod.Begin);

                                    mm.ZeroMemory();
                                    res = IO.ReadFile(hfile, mm, bps, ref br, IntPtr.Zero);
                                }
                            } while (ebrInfo.PartTable[1].StartLBA != 0);

                            // Extended partition
                            // Let's read this info, too.
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