using DataTools.Win32.Disk.Partition;
using DataTools.Win32.Memory;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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
        public static DiskDeviceInfo[] _internalEnumDisks(Guid DiskClass = default)
        {
            var hHeap = new SafePtr();
            // hHeap.IsString = True

            try
            {
                DiskDeviceInfo[] info = null;

                if (DiskClass == Guid.Empty)
                    DiskClass = DevProp.GUID_DEVINTERFACE_DISK;

                var disk = DevProp.INVALID_HANDLE_VALUE;
                var diskNumber = new NativeDisk.STORAGE_DEVICE_NUMBER();

                uint bytesReturned = 0U;

                info = DeviceEnum.EnumerateDevices<DiskDeviceInfo>(DiskClass);

                if (info is null || info.Count() == 0)
                    return Array.Empty<DiskDeviceInfo>();

                foreach (var inf in info)
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

                        disk = IO.CreateFile(inf.DevicePath, IO.GENERIC_READ, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, nint.Zero, IO.OPEN_EXISTING, IO.FILE_ATTRIBUTE_NORMAL, nint.Zero);

                        if (disk != DevProp.INVALID_HANDLE_VALUE)
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

                            User32.CloseHandle(disk);
                        }
                        else
                        {
                            PopulateVolumeInfo(inf);
                        }
                    }
                    else if (inf.Type != StorageType.Optical)
                    {
                        disk = IO.CreateFile(inf.DevicePath, IO.GENERIC_READ, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, nint.Zero, IO.OPEN_EXISTING, IO.FILE_ATTRIBUTE_NORMAL, nint.Zero);

                        if (disk != DevProp.INVALID_HANDLE_VALUE)
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

                            User32.CloseHandle(disk);
                            disk = DevProp.INVALID_HANDLE_VALUE;
                        }
                    }

                    if (inf.FriendlyName == "Microsoft Virtual Disk")
                    {
                        inf.Type = StorageType.Virtual;
                        disk = IO.CreateFile(@"\\.\PhysicalDrive" + inf.PhysicalDevice, IO.GENERIC_READ, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, nint.Zero, IO.OPEN_EXISTING, IO.FILE_ATTRIBUTE_NORMAL, nint.Zero);
                        if (disk == DevProp.INVALID_HANDLE_VALUE)
                            continue;
                        var sdi = default(STORAGE_DEPENDENCY_INFO_V2);
                        var mm = new SafePtr();
                        uint su = 0U;
                        uint r;
                        int sdic = Marshal.SizeOf(sdi);
                        mm.Length = sdic;
                        sdi.Version = STORAGE_DEPENDENCY_INFO_VERSION.STORAGE_DEPENDENCY_INFO_VERSION_2;
                        mm.IntAt(0L) = (int)STORAGE_DEPENDENCY_INFO_VERSION.STORAGE_DEPENDENCY_INFO_VERSION_2;
                        mm.IntAt(1L) = 1;
                        r = VirtDisk.GetStorageDependencyInformation(disk, GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_HOST_VOLUMES | GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_DISK_HANDLE, (uint)mm.Length, mm, ref su);
                        if (su != sdic)
                        {
                            mm.Length = su;
                            r = VirtDisk.GetStorageDependencyInformation(disk, GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_HOST_VOLUMES | GET_STORAGE_DEPENDENCY_FLAG.GET_STORAGE_DEPENDENCY_FLAG_DISK_HANDLE, (uint)mm.Length, mm, ref su);
                        }

                        if (r != 0L)
                        {
                            //Interaction.MsgBox(NativeError.FormatLastError(), Constants.vbExclamation);
                        }
                        else
                        {
                            sdi.NumberEntries = mm.UIntAt(1L);
                            inf.BackingStore = new string[((int)sdi.NumberEntries)];

                            var sne = sdi.NumberEntries;

                            for (long d = 0; d < sne; d++)
                            {
                                sdi.Version2Entries = mm.ToStruct<STORAGE_DEPENDENCY_INFO_TYPE_2>();
                                try
                                {
                                    var relpath = sdi.Version2Entries.DependentVolumeRelativePath.ToString();
                                    inf.BackingStore[(int)d] = Path.GetFullPath(relpath);
                                }
                                catch (Exception ex)
                                {
                                    Console.Error.WriteLine(ex.Message);
                                }
                            }
                        }

                        mm.Dispose();
                        User32.CloseHandle(disk);
                        inf.VirtualDisk = new VirtualDisk(inf, false);
                        disk = DevProp.INVALID_HANDLE_VALUE;
                    }
                }

                if (hHeap is object)
                    hHeap.Dispose();
                return info;
            }
            catch // (Exception ex)
            {
                if (hHeap is object)
                    hHeap.Dispose();
                return null;
            }
        }

        /// <summary>
        /// Populates a DiskDeviceInfo object with extended volume information.
        /// </summary>
        /// <param name="disk">The disk device object to populate.</param>
        /// <param name="handle">Optional handle to an open disk.</param>
        /// <remarks></remarks>
        public static void PopulateVolumeInfo(DiskDeviceInfo disk, nint handle = default)
        {
            int pLen = (IO.MAX_PATH + 1) * 2;

            MemPtr mm1 = new MemPtr();
            MemPtr mm2 = new MemPtr();

            int mc = 0;

            mm1.Alloc(pLen);
            mm2.Alloc(pLen);

            mm1.ZeroMemory(0, pLen);
            mm2.ZeroMemory(0, pLen);

            string pp = new string('\0', 1024);
            IO.GetVolumeNameForVolumeMountPoint(disk.DevicePath + @"\", mm1, 1024U);

            // just get rid of the extra nulls (they like to stick around).

            disk.Type = StorageType.Volume;
            disk.VolumeGuidPath = (string)mm1;
            disk.VolumePaths = NativeDisk.GetVolumePaths((string)mm1);

            mm1.ZeroMemory(0, pLen);

            if (handle == nint.Zero || handle == DevProp.INVALID_HANDLE_VALUE)
            {
                string arglpRootPathName = disk.VolumeGuidPath;
                uint arglpVolumeSerialNumber = disk.SerialNumber;
                var arglpFileSystemFlags = disk.VolumeFlags;

                NativeDisk.GetVolumeInformation(arglpRootPathName, mm1, pLen, ref arglpVolumeSerialNumber, ref mc, ref arglpFileSystemFlags, mm2, pLen);

                disk.VolumeGuidPath = arglpRootPathName;
                disk.SerialNumber = arglpVolumeSerialNumber;
                disk.VolumeFlags = arglpFileSystemFlags;
            }
            else
            {
                uint arglpVolumeSerialNumber1 = disk.SerialNumber;
                var arglpFileSystemFlags1 = disk.VolumeFlags;

                NativeDisk.GetVolumeInformationByHandle(handle, mm1, pLen, ref arglpVolumeSerialNumber1, ref mc, ref arglpFileSystemFlags1, mm2, pLen);

                disk.SerialNumber = arglpVolumeSerialNumber1;
                disk.VolumeFlags = arglpFileSystemFlags1;
            }

            disk.DiskExtents = NativeDisk.GetDiskExtentsFor(disk.DevicePath);
            disk.FriendlyName = (string)mm1;
            disk.FileSystem = (string)mm2;

            var a = default(int);
            var b = default(int);
            var c = default(int);
            var d = default(int);

            string guidpath = disk.VolumeGuidPath;

            IO.GetDiskFreeSpace(guidpath, ref a, ref b, ref c, ref d);

            disk.VolumeGuidPath = guidpath;
            disk.SectorSize = b;

            mm1.Free();
            mm2.Free();
        }
    }
}