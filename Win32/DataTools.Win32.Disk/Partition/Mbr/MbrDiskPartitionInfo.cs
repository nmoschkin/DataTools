﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTools.Win32.Disk.Partition.Interfaces;
using DataTools.Win32.Disk.Partition.Internals;

namespace DataTools.Win32.Disk.Partition.Mbr
{

    /// <summary>
    /// Represents MBR style partition information.
    /// </summary>
    /// <remarks></remarks>
    public class MbrDiskPartitionInfo : DiskPartitionInfo
    {

        private MbrCodeInfo partInfo;

        /// <summary>
        /// Creates a new instance of this DiskPartitionInfo-derived class and populates it with information from the operating system.
        /// </summary>
        /// <param name="pe"></param>
        /// <remarks></remarks>
        internal MbrDiskPartitionInfo(Partitioning.PARTITION_INFORMATION_EX pe) : base(pe)
        {
            partInfo = MbrCodeInfo.FindByCode(_partex.Mbr.PartitionType).FirstOrDefault();
        }

        /// <summary>
        /// Return detailed information about the partition type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MbrCodeInfo PartitionTypeInfo
        {
            get
            {
                return _partex.Mbr.PartitionCode;
            }
        }        

        /// <summary>
        /// Indicates that the partition is bootable.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Bootable
        {
            get
            {
                return _partex.Mbr.BootIndicator;
            }
        }

        /// <summary>
        /// The partition is a recognized partition type by the operating system.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Recognized
        {
            get
            {
                return _partex.Mbr.RecognizedPartition;
            }
        }

        /// <summary>
        /// Total number of hidden sectors on this partition.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public uint HiddenSectors
        {
            get
            {
                return _partex.Mbr.HiddenSectors;
            }
        }

        /// <inheritdoc/>
        public override IPartitionType PartitionType => partInfo;

    }
}
