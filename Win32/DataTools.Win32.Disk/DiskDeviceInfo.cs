// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: DiskDeviceInfo derived class for disks and
//         volumes.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Text;
using DataTools.Win32.Disk.Interop;
using DataTools.Win32.Disk.Partition;
using DataTools.Win32.Disk.Partition.Geometry;
using DataTools.Win32.Disk.Partition.Interfaces;
using DataTools.Win32.Disk.Partition.Internals;
using DataTools.Win32.Disk.Virtual;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DataTools.Win32.Disk
{
    /// <summary>
    /// An object that represents a disk or volume device on the system.
    /// </summary>
    /// <remarks></remarks>
    public class DiskDeviceInfo : DeviceInfo
    {
        private int phyiscalDevice;
        private int partitionNumber;
        private StorageType storageType;
        private long size;
        private DeviceCapabilities devCaps;
        private string[] backingStore;
        private bool isVolume;
        private uint serialNumber;
        private string fileSystem;
        private FileSystemFlags volumeFlags;
        private string volumeGuidPath;
        private string[] volumePaths;
        private DiskExtent[] diskExtents;
        private IDiskLayout diskLayout;
        private IDiskPartition partInfo;
        private int sectorSize;
        private VirtualDisk virtualDisk;

        /// <summary>
        /// Enumerate all local volumes (with or without mount points).
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DiskDeviceInfo[] EnumVolumes()
        {
            return DiskEnum.InternalEnumDisks(DevProp.GUID_DEVINTERFACE_VOLUME);
        }

        /// <summary>
        /// Enumerate all physical hard drives and optical drives, and virtual drives.
        /// </summary>
        /// <returns>An array of <see cref="DiskDeviceInfo"/> objects.</returns>
        /// <remarks></remarks>
        public static DiskDeviceInfo[] EnumDisks()
        {
            var ld = new List<DiskDeviceInfo>();

            var hdds = DiskEnum.InternalEnumDisks();
            var opticals = DiskEnum.InternalEnumDisks(DevProp.GUID_DEVINTERFACE_CDROM);

            if (hdds != null && hdds.Length != 0) ld.AddRange(hdds);
            if (opticals != null && opticals.Length != 0) ld.AddRange(opticals);

            ld.Sort((a, b) =>
            {
                return string.Compare(a.DevicePath, b.DevicePath);
            });

            return ld.ToArray();
        }

        /// <summary>
        /// Access the virtual disk object (if any).
        /// </summary>
        /// <value></value>
        /// <returns>An array of <see cref="DiskDeviceInfo"/> objects.</returns>
        /// <remarks></remarks>
        public VirtualDisk VirtualDisk
        {
            get => virtualDisk;
            internal set => virtualDisk = value;
        }

        /// <summary>
        /// The sector size of this volume, in bytes.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int SectorSize
        {
            get => sectorSize;
            internal set => sectorSize = value;
        }

        /// <summary>
        /// Returns the disk layout and partition information on a physical disk where Type is not StorageType.Volume.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IDiskLayout DiskLayout
        {
            get => diskLayout;
            set => diskLayout = value;
        }

        /// <summary>
        /// Returns partition information for a DiskDeviceInfo object whose Type = StorageType.Volume
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IDiskPartition PartitionInfo
        {
            get
            {
                Partitioning.PARTITION_INFORMATION_EX[] p;
                if (Type != StorageType.Volume) return null;

                if (partInfo == null)
                {
                    try
                    {
                        if (!IsVolumeMounted) return null;
                        p = Partitioning.GetPartitions(@"\\.\PhysicalDrive" + PhysicalDevice, null, out _);
                    }
                    catch
                    {
                        return null;
                    }

                    if (p != null)
                    {
                        foreach (var x in p)
                        {
                            if (x.PartitionNumber == PartitionNumber)
                            {
                                partInfo = DiskPartitionInfo.CreateInfo(x);
                                break;
                            }
                        }
                    }
                }

                return partInfo;
            }
            internal set
            {
                partInfo = value;
            }
        }

        /// <summary>
        /// The physical disk drive number.
        /// </summary>
        /// <remarks></remarks>
        public int PhysicalDevice
        {
            get => phyiscalDevice;
            internal set => phyiscalDevice = value;
        }

        /// <summary>
        /// If applicable, the partition number of the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PartitionNumber
        {
            get => partitionNumber;
            internal set => partitionNumber = value;
        }

        /// <summary>
        /// Total capacity of storage device.
        /// </summary>
        /// <remarks></remarks>
        public FriendlySizeLong Size
        {
            get
            {
                if (Type != StorageType.Volume || string.IsNullOrEmpty(VolumeGuidPath))
                {
                    if (size == 0)
                    {
                        diskLayout = DiskLayoutInfo.CreateLayout(DevicePath);
                        if (diskLayout != null)
                        {
                            long newsize = 0L;
                            foreach (var disk in diskLayout)
                            {
                                newsize += disk.Size;
                            }

                            size = newsize;
                        }
                    }

                    return size;
                }

                try
                {
                    if (!IsVolumeMounted)
                        return new FriendlySizeLong(0);

                    IO.GetDiskFreeSpaceEx(VolumeGuidPath, out _, out ulong size, out _);
                    return (FriendlySizeLong)(long)size;
                }
                catch
                {
                    return new FriendlySizeLong(0);
                }
            }

            internal set
            {
                size = value;
            }
        }

        /// <summary>
        /// Available capacity of volume.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public FriendlySizeLong SizeFree
        {
            get
            {
                if (Type != StorageType.Volume || string.IsNullOrEmpty(VolumeGuidPath)) return size;

                try
                {
                    if (!IsVolumeMounted) return 0;
                    IO.GetDiskFreeSpaceEx(VolumeGuidPath, out _, out _, out ulong sizeFree);
                    return sizeFree;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// The total used space of the volume.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public FriendlySizeLong SizeUsed
        {
            get
            {
                if (Type != StorageType.Volume || string.IsNullOrEmpty(VolumeGuidPath))
                    return size;

                try
                {
                    if (!IsVolumeMounted) return new FriendlySizeLong(0);
                    IO.GetDiskFreeSpaceEx(VolumeGuidPath, out _, out ulong size, out ulong free);
                    return size - free;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// TYpe of storage.
        /// </summary>
        /// <remarks></remarks>
        public StorageType Type
        {
            get => storageType;
            internal set => storageType = value;
        }

        /// <summary>
        /// Physical device capabilities.
        /// </summary>
        /// <remarks></remarks>
        public DeviceCapabilities Capabilities
        {
            get => devCaps;
            internal set => devCaps = value;
        }

        /// <summary>
        /// Contains a list of VHD/VHDX files that make up a virtual hard drive.
        /// </summary>
        /// <remarks></remarks>
        public string[] BackingStore
        {
            get => backingStore ?? new string[0];
            internal set => backingStore = value;
        }

        // Volume information

        /// <summary>
        /// Indicates whether or not this structure refers to a volume or a device.
        /// </summary>
        /// <remarks></remarks>
        public bool IsVolume
        {
            get => isVolume;
            internal set => isVolume = value;
        }

        /// <summary>
        /// Returns a value indicating whether this volume is mounted, if it represents a volume of a removeable drive.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsVolumeMounted => GetDriveFlag() != 0L & IO.GetLogicalDrives() != 0L;

        /// <summary>
        /// Returns the current logical drive flag.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private uint GetDriveFlag()
        {
            if (!IsVolume) return 0U;
            if (volumePaths is null || volumePaths.Length == 0) return 0U;

            char ch = '-';
            uint vl = 0U;

            foreach (var vp in volumePaths)
            {
                if (vp.Length <= 3)
                {
                    ch = vp.ToCharArray()[0];
                    break;
                }
            }

            if (ch == '-') return 0U;
            ch = ch.ToString().ToUpper()[0];
            vl = (uint)(ch - 'A');

            return (uint)(1 << (int)vl);
        }

        /// <summary>
        /// The volume serial number.
        /// </summary>
        /// <remarks></remarks>
        public uint SerialNumber
        {
            get => serialNumber;
            internal set => serialNumber = value;
        }

        /// <summary>
        /// The name of the file system for this volume.
        /// </summary>
        /// <remarks></remarks>
        public string FileSystem
        {
            get => fileSystem;
            internal set => fileSystem = value;
        }

        /// <summary>
        /// Volume flags and capabilities.
        /// </summary>
        /// <remarks></remarks>
        public FileSystemFlags VolumeFlags
        {
            get => volumeFlags;
            internal set => volumeFlags = value;
        }

        /// <summary>
        /// The Volume GUID (parsing) path.  This member can be used in a call to CreateFile for DeviceIoControl (for volumes).
        /// </summary>
        /// <remarks></remarks>
        public string VolumeGuidPath
        {
            get => volumeGuidPath;
            internal set => volumeGuidPath = value;
        }

        /// <summary>
        /// A list of all mount-points for the volume
        /// </summary>
        /// <remarks></remarks>
        public string[] VolumePaths
        {
            get => volumePaths;
            internal set => volumePaths = value;
        }

        /// <summary>
        /// Partition locations on the physical disk or disks.
        /// </summary>
        /// <remarks></remarks>
        public DiskExtent[] DiskExtents
        {
            get => diskExtents;
            internal set => diskExtents = value;
        }

        /// <summary>
        /// Print friendly device information, including friendly name, mount points and the device's friendly size.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            string str;

            if (IsVolume)
            {
                if (VolumePaths is object && VolumePaths.Count() > 0)
                {
                    string slist = string.Join(", ", VolumePaths);
                    str = "[" + slist + "] ";
                }
                else
                {
                    str = "";
                }

                str += FriendlyName + " (" + TextTools.PrintFriendlySize(Size) + ")";
            }
            else
            {
                str = "[" + Type.ToString() + "] " + FriendlyName + " (" + TextTools.PrintFriendlySize(Size) + ")";
            }

            return str;
        }
    }
}