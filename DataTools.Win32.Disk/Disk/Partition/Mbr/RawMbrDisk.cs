using DataTools.Win32;
using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Disk.Partition.Mbr
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

        /// <summary>
        /// Retrieve the partition table of a GPT-layout disk, manually.
        /// Must be Administrator.
        /// </summary>
        /// <param name="inf">DiskDeviceInfo object to the physical disk to read.</param>
        /// <param name="gptInfo">Receives the drive layout information.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public static bool ReadRawMbrDisk(string devicePath, ref MBR mbrInfo)
        {


            // Demand Administrator for accessing a raw disk.
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            // Dim principalPerm As New PrincipalPermission(Nothing, "Administrators")
            // principalPerm.Demand()


            var hfile = IO.CreateFile(devicePath, IO.GENERIC_READ | IO.GENERIC_WRITE, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, IntPtr.Zero, IO.OPEN_EXISTING, IO.FILE_FLAG_NO_BUFFERING | IO.FILE_FLAG_RANDOM_ACCESS, IntPtr.Zero);
            if (hfile == IO.INVALID_HANDLE_VALUE)
                return false;

            uint bps = (uint)Marshal.SizeOf<MBR>();
            uint br = 0;
            var mm = new SafePtr(bps);
			var p = mm;

            var res = IO.ReadFile(hfile, mm.DangerousGetHandle(), bps, ref br, IntPtr.Zero);
			if (!res || br == 0)
            {
				throw new NativeException();
            }

			mbrInfo = mm.ToStruct<MBR>();

			//mbrInfo.Code = p.ToByteArray(0, 440);			
			//p += 440;

			//mbrInfo.DiskSig = p.UIntAt(0);
			//p += 4;
			
			//mbrInfo.Reserved = p.UShortAt(0);
			//p += 2;

			//mbrInfo.PartTable = new Partition[4];

			//var psize = Marshal.SizeOf<Partition>();

			//for (var i = 0; i < 4; i++)
            //         {
			//	mbrInfo.PartTable[i] = p.ToStruct<Partition>();
			//	p += psize;
            //         }

			//mbrInfo.BootSignature = p.ToByteArray(0, 2); //0x55 0xAA for bootable

			//mbrInfo.status = mm.At(); 
			//mbrInfo.StartAddrHead = mm.At();
			//mbrInfo.StartAddrCylSec = mm.At();
			//mbrInfo.PartType = mm.At();
			//mbrInfo.EndAddrHead = mm.At();
			//mbrInfo.EndAddrCylSec = mm.At();
			//mbrInfo.StartLBA = mm.At();
			//mbrInfo.EndLBA = mm.At();  

			User32.CloseHandle(hfile);

            // we have succeeded.
            return true;
        }


    }
}
