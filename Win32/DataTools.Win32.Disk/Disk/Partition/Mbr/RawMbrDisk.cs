using DataTools.Win32.Memory;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace DataTools.Win32.Disk.Partition.Mbr
{
    public static class RawMbrDisk
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Partition
        {
            public byte status; //0x80 for bootable, 0x00 for not bootable, anything else for invalid
            public byte StartAddrHead; //head address of start of partition
            public ushort StartAddrCylSec; //(AddrCylSec & 0x3F) for sector,  (AddrCylSec & 0x3FF) for cylendar
            public byte PartType;
            public byte EndAddrHead; //head address of start of partition
            public ushort EndAddrCylSec; //(AddrCylSec & 0x3F) for sector,  (AddrCylSec & 0x3FF) for cylendar
            public uint StartLBA; //linear address of first sector in partition. Multiply by sector size (usually 512) for real offset
            public uint EndLBA;   //linear address of last sector in partition. Multiply by sector size (usually 512) for real offset
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MBR
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 440)]
            public byte[] Code;

            public uint DiskSig; //This is optional
            public ushort Reserved; //Usually 0x0000

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Partition[] PartTable;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] BootSignature; //0x55 0xAA for bootable
        }

        public struct RAW_MBR_INFO
        {
            public MBR Mbr;
            public uint SectorSize;

            public Partition[] Extended;
            public int PrimaryPartitionCount;
            public int ExtendedPartitionCount;
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
            RAW_MBR_INFO rawInfo = new RAW_MBR_INFO();

            rawMbrInfo = null;
            int pc = 0;

            // Demand Administrator for accessing a raw disk.
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            // Dim principalPerm As New PrincipalPermission(Nothing, "Administrators")
            // principalPerm.Demand()

            var hfile = IO.CreateFile(devicePath, IO.GENERIC_READ | IO.GENERIC_WRITE, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, nint.Zero, IO.OPEN_EXISTING, IO.FILE_FLAG_NO_BUFFERING | IO.FILE_FLAG_RANDOM_ACCESS, nint.Zero);
            if (hfile == IO.INVALID_HANDLE_VALUE)
                return false;

            uint bps = (uint)Marshal.SizeOf<MBR>();
            uint br = 0;
            var mm = new SafePtr(bps);
            var p = mm;

            var res = IO.ReadFile(hfile, mm, bps, ref br, nint.Zero);
            if (!res || br == 0)
            {
                throw new NativeException();
            }

            mbrInfo = mm.ToStruct<MBR>();

            rawInfo.Mbr = mbrInfo;
            rawInfo.SectorSize = (uint)sectorSize;

            List<Partition> exParts = new List<Partition>();

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
                    res = IO.ReadFile(hfile, mm, bps, ref br, nint.Zero);

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
                            res = IO.ReadFile(hfile, mm, bps, ref br, nint.Zero);
                        }
                    } while (ebrInfo.PartTable[1].StartLBA != 0);

                    // Extended partition
                    // Let's read this info, too.
                }
            }

            User32.CloseHandle(hfile);
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