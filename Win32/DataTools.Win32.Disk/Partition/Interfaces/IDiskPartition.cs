﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Disk.Partition.Interfaces
{

    /// <summary>
    /// Represents a partition on a disk device.
    /// </summary>
    /// <remarks></remarks>
    public interface IDiskPartition
    {

        /// <summary>
        /// Returns the logical partition number of the partition on the disk.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        int PartitionNumber { get; }

        /// <summary>
        /// Returns the starting offset, in bytes, of the partition relative to the beginning of the disk.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        long Offset { get; }

        /// <summary>
        /// Returns the size, in bytes, of the partition.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        long Size { get; }

        /// <summary>
        /// Get information about the partition type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IPartitionType PartitionType { get; }

        /// <summary>
        /// Returns the style of the disk or partition (MBR or GPT).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        PartitionStyle PartitionStyle { get; }
    }




}
