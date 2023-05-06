using System.Runtime.InteropServices;

namespace DataTools.Win32.Disk.Partition.Mbr
{
    /// <summary>
    /// Master Boot Record structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MasterBootRecord
    {
        /// <summary>
        /// Byte Code
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 440)]
        public byte[] BootstrapByteCode;

        /// <summary>
        /// Optional
        /// </summary>
        public uint DiskSignature; //This is optional

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved; //Usually 0x0000

        /// <summary>
        /// Partition Table
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public RawMbrPartitionStruct[] PartTable;

        /// <summary>
        /// 0x55 0xAA for bootable
        /// </summary>
        public ushort BootSignature; // 0x55 0xAA for bootable

        ///// <summary>
        ///// True if the reserved word indicates copy protection
        ///// </summary>
        //public bool IsCopyProtected => Reserved == 0x5a5a;
    }


}