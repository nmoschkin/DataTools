using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTools.Win32.Disk.Partition.Interfaces;
using DataTools.Win32.Disk.Partition.Internals;

namespace DataTools.Win32.Disk.Partition.Gpt
{


    /// <summary>
    /// Represents GPT style partition information.
    /// </summary>
    /// <remarks></remarks>
    public class GptDiskPartitionInfo : DiskPartitionInfo, IGuid
    {
        private GptCodeInfo partInfo;

        /// <summary>
        /// Creates a new instance of this DiskPartitionInfo-derived class and populates it with information from the operating system.
        /// </summary>
        /// <param name="pe"></param>
        /// <remarks></remarks>
        internal GptDiskPartitionInfo(Partitioning.PARTITION_INFORMATION_EX pe) : base(pe)
        {
            partInfo = _partex.Gpt.PartitionCode;
        }

        /// <summary>
        /// Returns the UEFI attributes for the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public GptPartitionAttributes Attributes
        {
            get
            {
                return _partex.Gpt.Attributes;
            }
        }

        /// <summary>
        /// Returns the Partition Guid.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        Guid IGuid.Id
        {
            get
            {
                return _partex.Gpt.PartitionId;
            }
        }

        /// <summary>
        /// Returns detailed partition type information.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override IPartitionType PartitionType => partInfo;
    }
}
