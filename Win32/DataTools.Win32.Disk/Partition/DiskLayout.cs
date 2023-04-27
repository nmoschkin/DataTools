using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTools.Win32.Disk.Partition.Gpt;
using DataTools.Win32.Disk.Partition.Mbr;
using DataTools.Win32.Disk.Interop;
using DataTools.Win32.Disk.Partition.Interfaces;
using DataTools.Win32.Disk.Partition.Internals;

namespace DataTools.Win32.Disk.Partition
{
    /// <summary>
    /// Base class for disk device layout information.
    /// </summary>
    /// <remarks></remarks>
    public abstract class DiskLayoutInfo : IDiskLayout
    {
        internal Partitioning.DRIVE_LAYOUT_INFORMATION_EX layout;
        internal IDiskPartition[] partitions;

        /// <summary>
        /// Returns an array of the <see cref="IDiskPartition"/> interfaces referenced by this object
        /// </summary>
        /// <returns></returns>
        public IDiskPartition[] ToArray()
        {
            return partitions?.ToArray() ?? Array.Empty<IDiskPartition>();
        }

        /// <summary>
        /// Populates disk layout information from an open disk handle.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static IDiskLayout CreateLayout(DiskHandle disk)
        {
            Partitioning.PARTITION_INFORMATION_EX[] p;
            p = Partitioning.GetPartitions(null, disk, out var lay);

            if (p is null)
            {
                return null;
            }
            else if (lay.PartitionStyle == PartitionStyle.Gpt)
            {
                return new GptDiskLayoutInfo(lay, p);
            }
            else
            {
                return new MbrDiskLayoutInfo(lay, p);
            }
        }

        /// <summary>
        /// Populates disk layout information from a device path.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static IDiskLayout CreateLayout(string disk)
        {
            Partitioning.PARTITION_INFORMATION_EX[] p;
            p = Partitioning.GetPartitions(disk, null, out var lay);

            if (p is null)
            {
                return null;
            }
            else if (lay.PartitionStyle == PartitionStyle.Gpt)
            {
                return new GptDiskLayoutInfo(lay, p);
            }
            else
            {
                return new MbrDiskLayoutInfo(lay, p);
            }
        }

        /// <summary>
        /// Create a new instance of this DiskLayoutInfo-derived class and initialize it with raw data from the operating system.
        /// </summary>
        /// <param name="li"></param>
        /// <param name="p"></param>
        /// <remarks></remarks>
        internal DiskLayoutInfo(Partitioning.DRIVE_LAYOUT_INFORMATION_EX li, Partitioning.PARTITION_INFORMATION_EX[] p)
        {
            layout = li;

            var pts = new List<IDiskPartition>();

            foreach (var i in p)
            {
                pts.Add(DiskPartitionInfo.CreateInfo(i));
            }

            partitions = pts.ToArray();
        }


        /// <summary>
        /// Returns the partition style of the disk (MBR or GPT).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PartitionStyle PartitionStyle => layout.PartitionStyle;

        /// <summary>
        /// Returns the number of partitions on the disk.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Count => (int)layout.ParititionCount;

        /// <summary>
        /// Returns a specific partition by its index in the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IDiskPartition this[int index]
        {
            get
            {
                return partitions[index];
            }
            internal protected set
            {
                partitions[index] = value;
            }
        }


        /// <summary>
        /// Converts this object into its string representation.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return $"{layout.PartitionStyle} disk, {Count} partitions.";
        }

        /// <inheritdoc/>
        public IEnumerator<IDiskPartition> GetEnumerator()
        {
            foreach (var part in partitions)
            {
                yield return part;
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    }

}
