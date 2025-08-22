using System.Runtime.InteropServices;

namespace DataTools.Win32.Disk.Partition.Mbr
{
    /// <summary>
    /// MBR Disk Partition
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RawMbrPartitionStruct
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
        /// Partition type code (see <see cref="MbrCodeInfo"/>)
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
        /// Total LBAs in the partitition (partition size / sector size)
        /// </summary>
        public uint TotalLBAs;

        /// <summary>
        /// Gets the size based on 512 sector size
        /// </summary>
        public long Size => (TotalLBAs * 512L);
    }
}