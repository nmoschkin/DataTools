// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: VirtualDisk
//         Create, Mount, and Unmount .vhd and .vhdx
//         Virtual Disks.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using DataTools.Shell.Native;
using DataTools.Text;
using DataTools.Win32.Memory;

namespace DataTools.Win32.Disk.Virtual
{

    /// <summary>
    /// Virtual storage type.
    /// </summary>
    /// <remarks></remarks>
    public enum VirtualStorageType : int
    {

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks></remarks>
        Unknown = 0,

        /// <summary>
        /// CD-ROM/DVD-ROM ISO image file.
        /// </summary>
        /// <remarks></remarks>
        ISO = 1,

        /// <summary>
        /// Virtual drive (Windows 7)
        /// </summary>
        /// <remarks></remarks>
        VHD = 2,

        /// <summary>
        /// Virtual drive (Windows 8+)
        /// </summary>
        /// <remarks></remarks>
        VHDX = 3
    }

    /// <summary>
    /// Encapsulates a virtual disk device (iso, vhd, or vhdx).
    /// </summary>
    /// <remarks></remarks>
    public sealed class VirtualDisk : SafeHandle
    {
        internal DiskDeviceInfo diskInfo;
        private string imageFile;

        /// <summary>
        /// Initialize the virtual disk object with an existing virtual disk.
        /// </summary>
        /// <param name="imageFile">The disk image file to load.</param>
        /// <remarks></remarks>
        public VirtualDisk(string imageFile) : this(imageFile, true, false)
        {
        }

        /// <summary>
        /// Initialize the virtual disk object with an existing virtual disk, and optionally open the disk.
        /// </summary>
        /// <param name="imageFile">The disk image file to load.</param>
        /// <param name="openDisk">Whether or not to open the disk.</param>
        /// <remarks></remarks>
        public VirtualDisk(string imageFile, bool openDisk) : this(imageFile, openDisk, false)
        {
        }

        /// <summary>
        /// Initialize the virtual disk object with an existing virtual disk, and optionally open the disk.
        /// </summary>
        /// <param name="imageFile">The disk image file to load.</param>
        /// <param name="openDisk">Whether or not to open the disk.</param>
        /// <param name="openReadOnly">Whether or not to open the disk read-only.</param>
        /// <remarks></remarks>
        public VirtualDisk(string imageFile, bool openDisk, bool openReadOnly) : this()
        {
            this.imageFile = imageFile;
            if (openDisk) Open(openReadOnly);
        }

        /// <summary>
        /// Initializes a new instance of a virtual drive with the specified DiskDeviceInfo object.
        /// </summary>
        /// <param name="inf">DiskDeviceInfo object.</param>
        /// <param name="open">Whether or not to open the drive.</param>
        /// <remarks></remarks>
        internal VirtualDisk(DiskDeviceInfo inf, bool open) : this(inf.BackingStore[0], open)
        {
            diskInfo = inf;
        }

        private VirtualDisk() : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// Returns the backing store paths.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] BackingStore => diskInfo?.BackingStore;

        /// <summary>
        /// Returns the DeviceCapabilities enumeration.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DeviceCapabilities Capabilities => diskInfo?.Capabilities ?? DeviceCapabilities.None;

        /// <summary>
        /// Returns the device path.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DevicePath => diskInfo?.DevicePath;

        /// <summary>
        /// Returns the device type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DeviceType DeviceType => DeviceType.Disk;

        /// <summary>
        /// Returns the device friendly name.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string FriendlyName => diskInfo?.FriendlyName;

        /// <summary>
        /// Returns the unique Guid identifier of the virtual drive.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid Identifier
        {
            get
            {
                var info = new GET_VIRTUAL_DISK_INFO_SIZE();
                uint iSize;
                uint sizeUsed = 0;

                using (var mm = new SafePtr())
                {

                    info.Version = GET_VIRTUAL_DISK_INFO_VERSION.GET_VIRTUAL_DISK_INFO_IDENTIFIER;
                    iSize = (uint)Marshal.SizeOf<GET_VIRTUAL_DISK_INFO_SIZE>();

                    mm.FromStruct(info);
                    HResult r = (HResult)VDiskNative.GetVirtualDiskInformation(handle, ref iSize, mm, ref sizeUsed);

                    if (r == HResult.Ok)
                    {
                        return mm.GuidAtAbsolute(8L);
                    }
                    else
                    {
                        return Guid.Empty;
                    }

                }
            }
        }

        /// <summary>
        /// Gets or sets the VHD/VHDX/ISO image file for the current object.
        /// If set, any current image is closed.
        /// </summary>
        /// <returns></returns>
        public string ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                if (IsOpen) ReleaseHandle();
                imageFile = value;
            }
        }

        /// <summary>
        /// Returns the hardware instance id.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string InstanceId => diskInfo?.InstanceId;

        /// <inheritdoc/>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// Indicates whether the virtual drive is open.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsOpen => handle != IntPtr.Zero;

        /// <summary>
        /// Indicates whether the virtual drive is mounted.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Mounted => !string.IsNullOrEmpty(DevicePath);

        /// <summary>
        /// Returns the PhysicalDriveX number.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int PhysicalDevice => diskInfo.PhysicalDevice;

        /// <summary>
        /// Returns the physical hardware path of the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PhysicalPath
        {
            get
            {
                uint szpath = IO.MAX_PATH;
                using (var mm = new SafePtr(szpath * 2))
                {
                    var r = VDiskNative.GetVirtualDiskPhysicalPath(handle, ref szpath, mm);
                    return mm.ToString();
                }
            }
        }

        /// <summary>
        /// Returns the actual physical size of the the virtual drive, as stored on the primary media.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public long PhysicalSize => (long)GetSizeInfo().PhysicalSize;

        /// <summary>
        /// Returns the virtual size of the drive.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public long Size => (long)GetSizeInfo().VirtualSize;

        /// <summary>
        /// Miscellaneous data
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Returns the storage device type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public StorageType Type => StorageType.Virtual;

        /// <summary>
        /// Creates a new virtual disk device file.
        /// </summary>
        /// <param name="imageFile">Full path to the destination image, including file extension (.vhd or .vhdx)</param>
        /// <param name="diskSize">The virtual size of the new disk.</param>
        /// <param name="fixedSize">Indicates whether or not the size is fixed or virtual.</param>
        /// <param name="id">Receives the Guid for the new drive.</param>
        /// <param name="resiliencyId">The Resiliancy Guid.</param>
        /// <param name="blockSize">The transfer block size.</param>
        /// <param name="sectorSize">The virtual sector size.</param>
        /// <param name="sourcePath">The cloning source (if any).</param>
        /// <param name="sourceDiskType">The cloning source disk type (if applicable).</param>
        /// <returns>A new VirtualDisk object.</returns>
        /// <remarks></remarks>
        public static VirtualDisk Create(string imageFile, long diskSize, bool fixedSize, ref Guid id, ref Guid resiliencyId, int blockSize = 2097152, int sectorSize = 512, string sourcePath = null, VirtualStorageType sourceDiskType = VirtualStorageType.Unknown)
        {
            string ext = Path.GetExtension(imageFile).ToLower();

            VIRTUAL_STORAGE_TYPE vst;

            var r = default(uint);
            IntPtr handleNew = IntPtr.Zero;

            vst.VendorId = VDiskNative.VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT;

            switch (ext ?? "")
            {
                case ".vhd":

                    var cp1 = new CREATE_VIRTUAL_DISK_PARAMETERS_V1
                    {
                        BlockSizeInBytes = (uint)blockSize,
                        Version = CREATE_VIRTUAL_DISK_VERSION.CREATE_VIRTUAL_DISK_VERSION_1,
                        MaximumSize = (ulong)diskSize,
                        UniqueId = id,
                        SourcePath = sourcePath,
                        SectorSizeInBytes = 512
                    };
                    vst.DeviceId = VDiskNative.VIRTUAL_STORAGE_TYPE_DEVICE_VHD;

                    r = VDiskNative.CreateVirtualDisk(vst, imageFile, VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ALL, IntPtr.Zero, fixedSize ? CREATE_VIRTUAL_DISK_FLAGS.CREATE_VIRTUAL_DISK_FLAG_FULL_PHYSICAL_ALLOCATION : CREATE_VIRTUAL_DISK_FLAGS.CREATE_VIRTUAL_DISK_FLAG_NONE, 0U, cp1, IntPtr.Zero, out handleNew);

                    break;

                case ".vhdx":

                    var cp2 = new CREATE_VIRTUAL_DISK_PARAMETERS_V2
                    {
                        BlockSizeInBytes = (uint)blockSize,
                        Version = CREATE_VIRTUAL_DISK_VERSION.CREATE_VIRTUAL_DISK_VERSION_2,
                        MaximumSize = (ulong)diskSize,
                        UniqueId = id,
                        SourcePath = sourcePath
                    };

                    if (sourcePath != null)
                    {
                        cp2.SourceVirtualStorageType.DeviceId = (uint)sourceDiskType;
                        cp2.SourceVirtualStorageType.VendorId = VDiskNative.VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT;
                    }

                    cp2.SectorSizeInBytes = sectorSize;

                    if (resiliencyId != Guid.Empty)
                    {
                        cp2.ResiliencyGuid = resiliencyId;
                        // Else
                        // cp2.ResiliencyGuid = Guid.NewGuid
                        // resiliencyId = cp2.ResiliencyGuid
                    }

                    cp2.OpenFlags = OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE;
                    vst.DeviceId = VDiskNative.VIRTUAL_STORAGE_TYPE_DEVICE_VHDX;

                    r = VDiskNative.CreateVirtualDisk(vst, imageFile, VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_NONE, IntPtr.Zero, fixedSize ? CREATE_VIRTUAL_DISK_FLAGS.CREATE_VIRTUAL_DISK_FLAG_FULL_PHYSICAL_ALLOCATION : CREATE_VIRTUAL_DISK_FLAGS.CREATE_VIRTUAL_DISK_FLAG_NONE, 0U, cp2, IntPtr.Zero, out handleNew);
                    break;
            }

            if (r != 0L)
            {
                return null;
            }
            else
            {
                return new VirtualDisk
                {
                    handle = handleNew,
                    imageFile = imageFile
                };
            }

        }

        /// <summary>
        /// Mount the virtual drive, permanently.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// <remarks>
        /// The virtual drive will stay mounted beyond the lifetime of this instance.
        /// </remarks>
        public bool Mount()
        {
            return Mount(true);
        }

        /// <summary>
        /// Mount the virtual drive.
        /// </summary>
        /// <param name="makePermanent">True to make the mount permanent.</param>
        /// <returns>True if successful.</returns>
        /// <remarks>
        /// If <paramref name="makePermanent"/> is set to true, then the virtual drive will stay mounted beyond the lifetime of this instance.
        /// </remarks>
        public bool Mount(bool makePermanent)
        {
            if (handle == IntPtr.Zero || Mounted) return false;

            using (var mm = new SafePtr(8L))
            {
                mm.ByteAt(0L) = 1;
                uint r = VDiskNative.AttachVirtualDisk(handle, IntPtr.Zero, makePermanent ? ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_PERMANENT_LIFETIME : ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_NONE, 0U, mm, IntPtr.Zero);

                return r == 0L;
            }
        }

        /// <summary>
        /// Open the virtual disk.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Open() => Open(false);

        /// <summary>
        /// Open the virtual disk.
        /// </summary>
        /// <param name="openReadOnly">Open as read-only.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Open(bool openReadOnly) => Open(imageFile, openReadOnly);

        /// <summary>
        /// Open the specified virtual disk image file.
        /// </summary>
        /// <param name="imageFile">The vhd or vhdx file to open.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Open(string imageFile) => Open(imageFile, false);

        /// <summary>
        /// Open the specified disk image file.
        /// </summary>
        /// <param name="imageFile">The vhd or vhdx file to open.</param>
        /// <param name="openReadOnly">Open as read-only.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Open(string imageFile, bool openReadOnly)
        {
            if (handle != IntPtr.Zero) ReleaseHandle();

            string ext = Path.GetExtension(imageFile).ToLower();
            VIRTUAL_STORAGE_TYPE vst;

            HResult r;

            var accessMode = VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_GET_INFO | VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_DETACH;

            if (!openReadOnly)
            {
                accessMode |= VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ATTACH_RW;
            }
            else
            {
                accessMode |= VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ATTACH_RO;
            }

            var vdp2 = new OPEN_VIRTUAL_DISK_PARAMETERS_V2
            {
                Version = OPEN_VIRTUAL_DISK_VERSION.OPEN_VIRTUAL_DISK_VERSION_2,
                ResiliencyGuid = Guid.NewGuid(),
                ReadOnly = false,
                GetInfoOnly = false
            };

            var vdp1 = new OPEN_VIRTUAL_DISK_PARAMETERS_V1
            {
                RWDepth = 1U,
                Version = OPEN_VIRTUAL_DISK_VERSION.OPEN_VIRTUAL_DISK_VERSION_1
            };

            vst.VendorId = VDiskNative.VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT;

            switch (ext.ToLower() ?? "")
            {
                case ".vhd":
                    vst.DeviceId = VDiskNative.VIRTUAL_STORAGE_TYPE_DEVICE_VHD;
                    r = (HResult)VDiskNative.OpenVirtualDisk(vst, imageFile, accessMode, OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE, vdp1, out handle);
                    break;

                case ".vhdx":
                    vst.DeviceId = VDiskNative.VIRTUAL_STORAGE_TYPE_DEVICE_VHDX;
                    r = (HResult)VDiskNative.OpenVirtualDisk(vst, imageFile, accessMode, OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE, vdp2, out handle);
                    break;

                default:
                    return false;
            }

            if (r == HResult.Ok)
            {
                this.imageFile = imageFile;
            }

            return r == 0L;
        }

        /// <summary>
        /// Displays drive information.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return $"{TextTools.PrintFriendlySize(Size)} Virtual Drive [{(Mounted ? "Attached" : "Not Attached")}]";
        }

        /// <summary>
        /// Dismount the drive.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Unmount()
        {
            if (!Mounted) return false;

            uint r = VDiskNative.DetachVirtualDisk(handle, DETACH_VIRTUAL_DISK_FLAG.DETACH_VIRTUAL_DISK_FLAG_NONE, 0U);
            return r == 0L;
        }
        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            if (IsInvalid) return true;
            var r = User32.CloseHandle(handle);
            if (r) handle = IntPtr.Zero;
            return r;
        }

        private GET_VIRTUAL_DISK_INFO_SIZE GetSizeInfo()
        {
            if (handle == IntPtr.Zero) return default;

            GET_VIRTUAL_DISK_INFO_SIZE info;
            uint iSize;
            uint sizeUsed = 0;

            using (var mm = new SafePtr())
            {
                info = new GET_VIRTUAL_DISK_INFO_SIZE();
                info.Version = GET_VIRTUAL_DISK_INFO_VERSION.GET_VIRTUAL_DISK_INFO_SIZE;

                iSize = (uint)Marshal.SizeOf<GET_VIRTUAL_DISK_INFO_SIZE>();

                mm.FromStruct(info);

                uint r = VDiskNative.GetVirtualDiskInformation(handle, ref iSize, mm, ref sizeUsed);
                info = mm.ToStruct<GET_VIRTUAL_DISK_INFO_SIZE>();

                return info;
            }
        }
    }
}