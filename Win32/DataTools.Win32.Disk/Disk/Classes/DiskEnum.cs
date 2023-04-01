using DataTools.Shell.Native;
using DataTools.Win32.Disk.Partition;
using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using static DataTools.Win32.DeviceEnum;

namespace DataTools.Win32.Disk
{
    internal static class DiskEnum
    {
        /// <summary>
        /// Enumerate all local physical and virtual disk drives, including optical drives or volumes, depending on the class Id.
        /// </summary>
        /// <param name="DiskClass">The disk-type class Id or interface Id to use.</param>
        /// <returns>An array of DiskDeviceInfo objects.</returns>
        /// <remarks></remarks>
        public static DiskDeviceInfo[] InternalEnumDisks(Guid DiskClass = default)
        {
            using (var hHeap = new SafePtr())
            {
                // hHeap.IsString = True

                try
                {
                    DiskDeviceInfo[] info = null;

                    if (DiskClass == Guid.Empty) DiskClass = DevProp.GUID_DEVINTERFACE_DISK;

                    var diskNumber = new NativeDisk.STORAGE_DEVICE_NUMBER();

                    uint bytesReturned = 0U;

                    info = EnumerateDevices<DiskDeviceInfo>(DiskClass);

                    if (info is null || info.Count() == 0) return Array.Empty<DiskDeviceInfo>();

                    foreach (var inf in info)
                    {
                        try
                        {
                            object caps = GetDeviceProperty(inf, DevProp.DEVPKEY_Device_Capabilities, DevPropTypes.Int32);

                            if (caps != null)
                            {
                                inf.Capabilities = (DeviceCapabilities)(int)caps;
                            }
                            if (inf.Capabilities == DeviceCapabilities.None)
                            {
                                caps = (GetDeviceProperty(inf, DevProp.DEVPKEY_Device_Capabilities, DevPropTypes.Int32, useClassId: true));

                                if (caps != null)
                                {
                                    inf.Capabilities = (DeviceCapabilities)(int)caps;
                                }
                            }

                            if (inf.DeviceClass == DeviceClassEnum.CdRom)
                            {
                                inf.Type = StorageType.Optical;
                            }
                            else if (inf.RemovalPolicy != DeviceRemovalPolicy.ExpectNoRemoval)
                            {
                                // this is a conundrum because these values are not predictable in some cases.
                                // we'll leave it this way, for now.
                                inf.Type = StorageType.Removable;

                                // If inf.Capabilities And DeviceCapabilities.Removable Then
                                // inf.Type = StorageType.RemovableHardDisk
                                // Else
                                // inf.Type = StorageType.Removable
                                // End If
                            }

                            if (DiskClass == DevProp.GUID_DEVINTERFACE_VOLUME || DiskClass == DevProp.GUID_DEVCLASS_VOLUME)
                            {
                                inf.IsVolume = true;
                                var disk = DiskHandle.OpenDisk(inf.DevicePath, false);

                                if (disk != null)
                                {
                                    PopulateVolumeInfo(inf, disk);

                                    hHeap.Length = Marshal.SizeOf(diskNumber);
                                    hHeap.ZeroMemory();

                                    bytesReturned = 0U;

                                    NativeDisk.DeviceIoControl(disk, NativeDisk.IOCTL_STORAGE_GET_DEVICE_NUMBER, default, 0U, hHeap.DangerousGetHandle(), (uint)hHeap.Length, ref bytesReturned, default);

                                    if (bytesReturned > 0L)
                                    {
                                        diskNumber = hHeap.ToStruct<NativeDisk.STORAGE_DEVICE_NUMBER>();
                                        inf.PhysicalDevice = (int)diskNumber.DeviceNumber;
                                        if (diskNumber.PartitionNumber < 1024L)
                                        {
                                            inf.PartitionNumber = (int)diskNumber.PartitionNumber;
                                        }
                                    }

                                    disk.Close();
                                }
                                else
                                {
                                    PopulateVolumeInfo(inf);
                                }
                            }
                            else if (inf.Type != StorageType.Optical)
                            {
                                using (var disk = DiskHandle.OpenDisk(inf.DevicePath))
                                {
                                    hHeap.Length = 128L;
                                    hHeap.ZeroMemory();

                                    NativeDisk.DeviceIoControl(disk, NativeDisk.IOCTL_DISK_GET_LENGTH_INFO, default, 0U, hHeap.DangerousGetHandle(), (uint)hHeap.Length, ref bytesReturned, default);

                                    inf.DiskLayout = DiskLayoutInfo.CreateLayout(disk);
                                    inf.Size = hHeap.LongAt(0L);

                                    hHeap.Length = Marshal.SizeOf(diskNumber);
                                    hHeap.ZeroMemory();

                                    bytesReturned = 0U;

                                    NativeDisk.DeviceIoControl(disk, NativeDisk.IOCTL_STORAGE_GET_DEVICE_NUMBER, default, 0U, hHeap.DangerousGetHandle(), (uint)hHeap.Length, ref bytesReturned, default);

                                    var res = DiskGeometry.GetDiskGeometry(null, disk, out DISK_GEOMETRY_EX? georet);

                                    if (res && georet is DISK_GEOMETRY_EX geometry)
                                    {
                                        inf.SectorSize = (int)geometry.Geometry.BytesPerSector;
                                    }

                                    if (bytesReturned > 0L)
                                    {
                                        diskNumber = hHeap.ToStruct<NativeDisk.STORAGE_DEVICE_NUMBER>();
                                        inf.PhysicalDevice = (int)diskNumber.DeviceNumber;
                                    }
                                }

                                if (inf.DevicePath.Contains("disk&ven_msft&prod_virtual_disk"))
                                {
                                    using (var disk = DiskHandle.OpenDisk(inf.DevicePath, false))
                                    {
                                        if (disk == null) continue;

                                        STORAGE_DEPENDENCY_INFO_V2 sdi;
                                        int sdisize = Marshal.SizeOf<STORAGE_DEPENDENCY_INFO_V2>();

                                        using (var mm = new SafePtr(sdisize))
                                        {
                                            uint realsize = 0U;
                                            HResult r;

                                            sdi.Version = STORAGE_DEPENDENCY_INFO_VERSION.STORAGE_DEPENDENCY_INFO_VERSION_2;

                                            mm.IntAt(0L) = (int)STORAGE_DEPENDENCY_INFO_VERSION.STORAGE_DEPENDENCY_INFO_VERSION_2;
                                            mm.IntAt(1L) = 1;

                                            r = (HResult)VirtDisk.GetStorageDependencyInformation(disk, GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_HOST_VOLUMES | GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_DISK_HANDLE, (uint)mm.Length, mm, out realsize);

                                            if (realsize != sdisize && r != 0)
                                            {
                                                mm.Length = realsize;
                                                r = (HResult)VirtDisk.GetStorageDependencyInformation(disk, GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_HOST_VOLUMES | GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_DISK_HANDLE, (uint)mm.Length, mm, out realsize);
                                            }
                                            if (r == 0)
                                            {
                                                sdi.Version = (STORAGE_DEPENDENCY_INFO_VERSION)mm.IntAt(0);

                                                if (sdi.Version == STORAGE_DEPENDENCY_INFO_VERSION.STORAGE_DEPENDENCY_INFO_VERSION_2)
                                                {
                                                    sdi.NumberEntries = mm.UIntAt(1L);

                                                    var bls = new List<string>();
                                                    var d = sdi.NumberEntries;

                                                    sdi.Version2Entries = mm.ToStructAt<STORAGE_DEPENDENCY_INFO_TYPE_2>(8);
                                                    for (var j = 0; j < d; j++)
                                                    {
                                                        var relpath = sdi.Version2Entries.DependentVolumeRelativePath.ToString();
                                                        if (!string.IsNullOrEmpty(relpath))
                                                        {
                                                            bls.Add(Path.GetFullPath(relpath));
                                                        }
                                                    }

                                                    if (bls.Count > 0)
                                                    {
                                                        inf.BackingStore = bls.ToArray();
                                                        inf.Type = StorageType.Virtual;
                                                        inf.VirtualDisk = new VirtualDisk(inf, false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }

                    return info;
                }
                catch // (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Populates a DiskDeviceInfo object with extended volume information.
        /// </summary>
        /// <param name="disk">The disk device object to populate.</param>
        /// <param name="handle">Optional handle to an open disk.</param>
        /// <remarks></remarks>
        public static void PopulateVolumeInfo(DiskDeviceInfo disk, IntPtr handle = default)
        {
            int pLen = (IO.MAX_PATH + 1) * 2;

            using (var mm1 = new SafePtr(pLen))
            {
                using (var mm2 = new SafePtr(pLen))
                {
                    int mc = 0;

                    mm1.ZeroMemory(0, pLen);
                    mm2.ZeroMemory(0, pLen);

                    string pp = new string('\0', 1024);
                    IO.GetVolumeNameForVolumeMountPoint(disk.DevicePath + @"\", mm1, 1024U);

                    // just get rid of the extra nulls (they like to stick around).

                    disk.Type = StorageType.Volume;
                    disk.VolumeGuidPath = (string)mm1;
                    disk.VolumePaths = NativeDisk.GetVolumePaths((string)mm1);

                    mm1.ZeroMemory(0, pLen);

                    if (handle == IntPtr.Zero || handle == DevProp.INVALID_HANDLE_VALUE)
                    {
                        string vgpath = disk.VolumeGuidPath;
                        uint dsn = disk.SerialNumber;
                        var sysvf = disk.VolumeFlags;

                        NativeDisk.GetVolumeInformation(vgpath, mm1, pLen, ref dsn, ref mc, ref sysvf, mm2, pLen);

                        disk.VolumeGuidPath = vgpath;
                        disk.SerialNumber = dsn;
                        disk.VolumeFlags = sysvf;
                    }
                    else
                    {
                        uint dsn = disk.SerialNumber;
                        var sysvf = disk.VolumeFlags;

                        NativeDisk.GetVolumeInformationByHandle(handle, mm1, pLen, ref dsn, ref mc, ref sysvf, mm2, pLen);

                        disk.SerialNumber = dsn;
                        disk.VolumeFlags = sysvf;
                    }

                    disk.DiskExtents = NativeDisk.GetDiskExtentsFor(disk.DevicePath);
                    disk.FriendlyName = (string)mm1;
                    disk.FileSystem = (string)mm2;

                    string guidpath = disk.VolumeGuidPath;

                    IO.GetDiskFreeSpace(guidpath, out _, out int sectorSize, out _, out _);

                    disk.VolumeGuidPath = guidpath;
                    disk.SectorSize = sectorSize;
                }
            }
        }
    }
}