﻿// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: DiskDeviceInfo derived class for disks and
//         volumes.
// 
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using System;
using System.ComponentModel;
using System.Linq;
using DataTools.Text;
using static DataTools.Text.TextTools;
using DataTools.Win32;
using DataTools.Win32.Disk.Partition;
using DataTools.Win32.Disk.Partition.Gpt;

namespace DataTools.Win32.Disk
{

    
    /// <summary>
    /// An object that represents a disk or volume device on the system.
    /// </summary>
    /// <remarks></remarks>
    public class DiskDeviceInfo : DeviceInfo
    {
        protected int _PhysicalDevice;
        protected int _PartitionNumber;
        protected StorageType _Type;
        protected long _Size;
        protected DeviceCapabilities _Capabilities;
        protected string[] _BackingStore;
        protected bool _IsVolume;
        protected uint _SerialNumber;
        protected string _FileSystem;
        protected FileSystemFlags _VolumeFlags;
        protected string _VolumeGuidPath;
        protected string[] _VolumePaths;
        protected DiskExtent[] _DiskExtents;
        protected IDiskLayout _DiskLayout;
        protected IDiskPartition _PartInfo;
        protected int _SectorSize;
        protected VirtualDisk _VirtualDrive;



        /// <summary>
        /// Enumerate all local volumes (with or without mount points).
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DiskDeviceInfo[] EnumVolumes()
        {
            return DiskEnum._internalEnumDisks(DevProp.GUID_DEVINTERFACE_VOLUME);
        }

        /// <summary>
        /// Enumerate all physical hard drives and optical drives, and virtual drives.
        /// </summary>
        /// <returns>An array of <see cref="DiskDeviceInfo"/> objects.</returns>
        /// <remarks></remarks>
        public static DiskDeviceInfo[] EnumDisks()
        {
            var d = DiskEnum._internalEnumDisks();
            var e = DiskEnum._internalEnumDisks(DevProp.GUID_DEVINTERFACE_CDROM);
            int c = d.Count();
            if (e is null || e.Count() == 0)
                return d;
            try
            {
                foreach (var x in e)
                {
                    try
                    {
                        Array.Resize(ref d, c + 1);
                        d[c] = x;
                        c += 1;
                    }
                    catch
                    {
                        e = null;
                        return d;
                    }
                }

                e = null;
                return d;
            }
            catch
            {
            }

            return new DiskDeviceInfo[0];
        }



        /// <summary>
        /// Access the virtual disk object (if any).
        /// </summary>
        /// <value></value>
        /// <returns>An array of <see cref="DiskDeviceInfo"/> objects.</returns>
        /// <remarks></remarks>
        public VirtualDisk VirtualDisk
        {
            get
            {
                return _VirtualDrive;
            }

            internal set
            {
                _VirtualDrive = value;
            }
        }

        /// <summary>
        /// The sector size of this volume, in bytes.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int SectorSize
        {
            get
            {
                return _SectorSize;
            }

            internal set
            {
                _SectorSize = value;
            }
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
            get
            {
                return _DiskLayout;
            }

            internal set
            {
                _DiskLayout = value;
            }
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

                if (Type != StorageType.Volume)
                    return null;

                if (_PartInfo == null)
                {
                    try
                    {
                        if (!IsVolumeMounted)
                            return null;

                        Partitioning.DRIVE_LAYOUT_INFORMATION_EX arglayInfo = new Partitioning.DRIVE_LAYOUT_INFORMATION_EX();

                        p = Partitioning.GetPartitions(@"\\.\PhysicalDrive" + PhysicalDevice, IntPtr.Zero, layInfo: ref arglayInfo);
                        
                    }
                    catch
                    {
                        return null;
                    }

                    if (p is object)
                    {
                        foreach (var x in p)
                        {
                            if (x.PartitionNumber == PartitionNumber)
                            {
                                _PartInfo = DiskPartitionInfo.CreateInfo(x);
                                break;
                            }
                        }
                    }
                }

                return _PartInfo;
            }

            internal set
            {
                _PartInfo = value;
            }
        }

        /// <summary>
        /// The physical disk drive number.
        /// </summary>
        /// <remarks></remarks>
        public int PhysicalDevice
        {
            get
            {
                return _PhysicalDevice;
            }

            internal set
            {
                _PhysicalDevice = value;
            }
        }

        /// <summary>
        /// If applicable, the partition number of the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PartitionNumber
        {
            get
            {
                return _PartitionNumber;
            }

            internal set
            {
                _PartitionNumber = value;
            }
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
                    if (_Size == 0)
                    {
                        _DiskLayout = DiskLayoutInfo.CreateLayout(DevicePath);
                        if (_DiskLayout != null)
                        {
                            long newsize = 0L;
                            foreach (var disk in _DiskLayout)
                            {
                                newsize += disk.Size;
                            }

                            _Size = newsize;
                        }
                    }

                    return _Size;
                    
                }
                var a = default(ulong);
                var b = default(ulong);
                var c = default(ulong);
                try
                {
                    if (!IsVolumeMounted)
                        return new FriendlySizeLong(0);

                    IO.GetDiskFreeSpaceEx(VolumeGuidPath, ref a, ref b, ref c);
                }
                catch
                {
                    return new FriendlySizeLong(0);
                }

                return b;
            }

            internal set
            {
                _Size = value;
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
                if (Type != StorageType.Volume || string.IsNullOrEmpty(VolumeGuidPath))
                    return _Size;
                var a = default(ulong);
                var b = default(ulong);
                var c = default(ulong);
                try
                {
                    if (!IsVolumeMounted)
                        return new FriendlySizeLong(0);

                    IO.GetDiskFreeSpaceEx(VolumeGuidPath, ref a, ref b, ref c);
                }
                catch
                {
                    return new FriendlySizeLong(0);
                }

                return c;
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
                    return _Size;
                var a = default(ulong);
                var b = default(ulong);
                var c = default(ulong);
                try
                {
                    if (!IsVolumeMounted)
                        return new FriendlySizeLong(0);

                    IO.GetDiskFreeSpaceEx(VolumeGuidPath, ref a, ref b, ref c);

                    return b - c;
                }
                catch
                {
                    return new FriendlySizeLong(0);
                }
            }
        }

        /// <summary>
        /// TYpe of storage.
        /// </summary>
        /// <remarks></remarks>
        public StorageType Type
        {
            get
            {
                return _Type;
            }

            internal set
            {
                _Type = value;
            }
        }

        /// <summary>
        /// Physical device capabilities.
        /// </summary>
        /// <remarks></remarks>
        public DeviceCapabilities Capabilities
        {
            get
            {
                return _Capabilities;
            }

            internal set
            {
                _Capabilities = value;
            }
        }
        /// <summary>
        /// Contains a list of VHD/VHDX files that make up a virtual hard drive.
        /// </summary>
        /// <remarks></remarks>
        public string[] BackingStore
        {
            get
            {
                return _BackingStore ?? new string[0];
            }

            internal set
            {
                _BackingStore = value;
            }
        }
        // Volume information

        /// <summary>
        /// Indicates whether or not this structure refers to a volume or a device.
        /// </summary>
        /// <remarks></remarks>
        public bool IsVolume
        {
            get
            {
                return _IsVolume;
            }

            internal set
            {
                _IsVolume = value;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this volume is mounted, if it represents a volume of a removeable drive.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsVolumeMounted
        {
            get
            {
                return GetDriveFlag() != 0L & IO.GetLogicalDrives() != 0L;
            }
        }

        /// <summary>
        /// Returns the current logical drive flag.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private uint GetDriveFlag()
        {
            if (!IsVolume)
                return 0U;
            if (_VolumePaths is null || _VolumePaths.Length == 0)
                return 0U;
            char ch = '-';
            uint vl = 0U;
            foreach (var vp in _VolumePaths)
            {
                if (vp.Length <= 3)
                {
                    ch = vp.ToCharArray()[0];
                    break;
                }
            }

            if (ch == '-')
                return 0U;
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
            get => _SerialNumber;
            internal set
            {
                _SerialNumber = value;
            }
        }

        /// <summary>
        /// The name of the file system for this volume.
        /// </summary>
        /// <remarks></remarks>
        public string FileSystem
        {
            get => _FileSystem;
            internal set
            {
                _FileSystem = value;
            }
        }

        /// <summary>
        /// Volume flags and capabilities.
        /// </summary>
        /// <remarks></remarks>
        public FileSystemFlags VolumeFlags
        {
            get => _VolumeFlags;
            internal set
            {
                _VolumeFlags = value;
            }
        }

        /// <summary>
        /// The Volume GUID (parsing) path.  This member can be used in a call to CreateFile for DeviceIoControl (for volumes).
        /// </summary>
        /// <remarks></remarks>
        public string VolumeGuidPath
        {
            get => _VolumeGuidPath;
            set
            {
                _VolumeGuidPath = value;
            }
        }
        /// <summary>
        /// A list of all mount-points for the volume
        /// </summary>
        /// <remarks></remarks>
        public string[] VolumePaths
        {
            get => _VolumePaths;
            set
            {
                _VolumePaths = value;
            }
        }

        /// <summary>
        /// Partition locations on the physical disk or disks.
        /// </summary>
        /// <remarks></remarks>
        public DiskExtent[] DiskExtents
        {
            get => _DiskExtents;
            internal set
            {
                _DiskExtents = value;
            }
        }

        /// <summary>
        /// Print friendly device information, including friendly name, mount points and the device's friendly size.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            string str = "";

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