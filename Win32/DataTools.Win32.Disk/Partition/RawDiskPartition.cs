using DataTools.Text;
using DataTools.Win32.Disk.Partition.Gpt;
using DataTools.Win32.Disk.Partition.Interfaces;
using DataTools.Win32.Disk.Partition.Mbr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Disk.Partition
{
    /// <summary>
    /// Read and describe raw disk partitions without OS intercession
    /// </summary>
    /// <remarks>
    /// This tool requires elevated privileges to use
    /// </remarks>
    public abstract class RawDiskPartition : IDiskPartition
    {

        /// <summary>
        /// Detect the partition style from the disk
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public static PartitionStyle DetectPartitionStyle(DiskDeviceInfo deviceInfo)
        {
            var res = RawGptDisk.ReadRawGptDisk(deviceInfo.DevicePath, out _);
            if (res) return PartitionStyle.Gpt;
            res = RawMbrDisk.ReadRawMbrDisk(deviceInfo.DevicePath, deviceInfo.SectorSize, out _);
            if (res) return PartitionStyle.Mbr;
            return PartitionStyle.Raw;
        }

        /// <summary>
        /// Read partitions from the physical drive pointed to by the <see cref="DiskDeviceInfo"/> object.
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IList<RawDiskPartition> ReadPartitions(DiskDeviceInfo deviceInfo)
        {
            if (deviceInfo.IsVolume) throw new InvalidOperationException("Cannot read a volume");
            PartitionStyle style;
            
            if (deviceInfo.DiskLayout == null)
            {
                style = DetectPartitionStyle(deviceInfo);
            }
            else
            {
                style = deviceInfo.DiskLayout.PartitionStyle;
            }

            if (style == PartitionStyle.Raw)
            {
                throw new InvalidOperationException("Disk does not appear to be initialized");
            }
            else if (style == PartitionStyle.Mbr)
            {
                return RawMbrPartition.ReadPartitions(deviceInfo).Select(x => x as RawDiskPartition).ToList();
            }
            else
            {
                return RawGptPartition.ReadPartitions(deviceInfo).Select(x => x as RawDiskPartition).ToList();
            }
        }

        /// <summary>
        /// Sector Size
        /// </summary>
        protected readonly int sectorSize;

        internal RawDiskPartition(int sectorSize) 
        {
            this.sectorSize = sectorSize;
        }

        /// <summary>
        /// Size of a disk sector, in bytes (usually 512)
        /// </summary>
        public int SectorSize => sectorSize;

        /// <inheritdoc/>
        public abstract int PartitionNumber { get; }
        /// <inheritdoc/>
        public abstract long Offset { get; }
        /// <inheritdoc/>
        public abstract long Size { get; }

        /// <summary>
        /// The starting LBA
        /// </summary>
        public abstract long StartingLBA { get; }
        
        /// <summary>
        /// The ending LBA
        /// </summary>
        public abstract long EndingLBA { get; }

        /// <inheritdoc/>
        public abstract IPartitionType PartitionType { get; }
        /// <inheritdoc/>
        public abstract PartitionStyle PartitionStyle { get; }

        public override string ToString()
        {
            return $"{(FriendlySizeLong)Size} {PartitionStyle} [{PartitionType}]";
        }
    }

    /// <summary>
    /// MBR Partition kind constants
    /// </summary>
    public enum MbrPartitionKind
    {
        
        /// <summary>
        /// Primary Partition
        /// </summary>
        Primary,

        /// <summary>
        /// Extended Partition
        /// </summary>
        Extended,
    }

    /// <summary>
    /// Raw MBR Partition Information
    /// </summary>
    public class RawMbrPartition : RawDiskPartition
    {
        /// <summary>
        /// The raw MBR partition structure source
        /// </summary>
        protected readonly RawMbrDisk.Partition source;

        private MbrCodeInfo cInfo;
        private int partNo;
        private long offset;
        private long size;
        private long slba;
        private long elba;
        private MbrPartitionKind partitionKind;

        /// <summary>
        /// Read partitions from the physical drive pointed to by the <see cref="DiskDeviceInfo"/> object.
        /// </summary>
        /// <param name="deviceInfo">The disk device to read the partition for.</param>
        /// <returns>A list of partitions</returns>
        new public static IList<RawMbrPartition> ReadPartitions(DiskDeviceInfo deviceInfo)
        {
            var ldisks = new List<RawMbrPartition>();

            var res = RawMbrDisk.ReadRawMbrDisk(deviceInfo.DevicePath, deviceInfo.SectorSize, out var mbrInfo);
            int i = 1;

            if (res && mbrInfo is RawMbrDisk.RAW_MBR_INFO dsk)
            {
                foreach (var part in dsk.Mbr.PartTable)
                {
                    if (part.StartLBA == 0) break;
                    ldisks.Add(
                        new RawMbrPartition(i, part, deviceInfo.SectorSize)
                        );
                    i++;
                }

                foreach (var part in dsk.Extended)
                {
                    ldisks.Add(
                        new RawMbrPartition(i, part, deviceInfo.SectorSize, MbrPartitionKind.Extended)
                        );
                    i++;
                }
            }

            return ldisks;
        }

        internal RawMbrPartition(int partNo, RawMbrDisk.Partition partition, int sectorSize = 512, MbrPartitionKind partitionKind = MbrPartitionKind.Primary) : base(sectorSize)
        {
            this.source = partition;
            this.partNo = partNo;
            this.partitionKind = partitionKind;
            this.offset = (long)source.StartLBA * sectorSize;
            this.size = source.Size;
            this.slba = (long)source.StartLBA;
            this.elba = (long)source.EndLBA;
            this.cInfo = MbrCodeInfo.FindByCode(source.PartType)?.FirstOrDefault();
        }

        /// <summary>
        /// True if this partition is an MBR extended partition entry
        /// </summary>
        public MbrPartitionKind PartitionKind => partitionKind;

        /// <inheritdoc/>
        public override int PartitionNumber => partNo;

        /// <inheritdoc/>
        public override long Offset => offset;

        /// <inheritdoc/>
        public override long Size => size;

        /// <inheritdoc/>
        public override long StartingLBA => slba;

        /// <inheritdoc/>
        public override long EndingLBA => elba;

        /// <inheritdoc/>
        public override IPartitionType PartitionType => cInfo;

        /// <inheritdoc/>
        public override PartitionStyle PartitionStyle => PartitionStyle.Mbr;

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString() + " [" + PartitionKind.ToString() + "]";
        }
    }

    /// <summary>
    /// Raw GPT Partition Information
    /// </summary>
    public class RawGptPartition : RawDiskPartition
    {
        /// <summary>
        /// The raw GPT partition structure source
        /// </summary>
        protected readonly RawGptDisk.RAW_GPT_PARTITION source;

        private int partNo;
        private long offset;
        private long size;
        private long slba;
        private long elba;
        private string fileSystem;

        /// <summary>
        /// Read partitions from the physical drive pointed to by the <see cref="DiskDeviceInfo"/> object.
        /// </summary>
        /// <param name="deviceInfo">The disk device to read the partition for.</param>
        /// <returns></returns>
        new public static IList<RawGptPartition> ReadPartitions(DiskDeviceInfo deviceInfo)
        {
            var ldisks = new List<RawGptPartition>();

            var res = RawGptDisk.ReadRawGptDisk(deviceInfo.DevicePath, out var gptInfo);
            int i = 1;

            if (res && gptInfo is RawGptDisk.RAW_GPT_DISK dsk)
            {
                foreach (var part in dsk.Partitions)
                {
                    ldisks.Add(
                        new RawGptPartition(i, part, deviceInfo.SectorSize, dsk.PartitionFileSystems[i - 1])
                        );
                    i++;
                }
            }

            return ldisks;
        }

        internal RawGptPartition(int partNo, RawGptDisk.RAW_GPT_PARTITION partition, int sectorSize = 512, string fileSystem = "Unknown") : base(sectorSize)
        {
            this.source = partition;
            this.partNo = partNo;
            this.offset = (long)source.StartingLBA * sectorSize;
            this.size = (long)source.Size; // (long)(source.EndingLBA - source.StartingLBA) * sectorSize;
            this.slba = (long)source.StartingLBA;
            this.elba = (long)source.EndingLBA;

            if (fileSystem == "MSDOS5.0") fileSystem = "FAT32";
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// The name of the identified file system moniker.
        /// </summary>
        public string FileSystem => fileSystem; 

        /// <inheritdoc/>
        public override int PartitionNumber => partNo;

        /// <inheritdoc/>
        public override long Offset => offset;

        /// <inheritdoc/>
        public override long Size => size;

        /// <inheritdoc/>
        public override long StartingLBA => slba;

        /// <inheritdoc/>
        public override long EndingLBA => elba;

        /// <inheritdoc/>
        public override IPartitionType PartitionType => source.PartitionCode;

        /// <inheritdoc/>
        public override PartitionStyle PartitionStyle => PartitionStyle.Gpt;

        /// <inheritdoc/>
        public override string ToString()
        {
            var fs = fileSystem;

            if (!string.IsNullOrEmpty(fs) && fs != "Unknown")
            {
                return base.ToString() + " [" + fs + "]";
            }
            else
            {
                return base.ToString();
            }
            
            
        }
    }
}
