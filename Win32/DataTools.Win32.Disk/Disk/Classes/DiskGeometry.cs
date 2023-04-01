using DataTools.Win32.Memory;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTools.Win32.Disk
{
    internal static class DiskGeometry
    {
        /// <summary>
        /// Get the disk geometry for the specified device
        /// </summary>
        /// <param name="devicePath">The disk device path to query.</param>
        /// <param name="disk">The valid disk handle.</param>
        /// <param name="geometry">Receives disk geometry information.</param>
        /// <returns>An array of PARTITION_INFORMATION_EX structures.</returns>
        /// <remarks>Use either <paramref name="devicePath"/> or <paramref name="disk"/>, but not both. Set the other to null.</remarks>
        public static bool GetDiskGeometry(string devicePath, DiskHandle disk, out DISK_GEOMETRY_EX? geometry)
        {
            bool owndisk;
            geometry = null;

            if (devicePath != null && disk == null)
            {
                disk = DiskHandle.OpenDisk(devicePath);
                owndisk = true;
            }
            else if (disk == null)
            {
                return false;
            }
            else
            {
                owndisk = false;
            }

            using (var pex = new SafePtr())
            {
                int pexLen = Marshal.SizeOf<DISK_GEOMETRY_EX>();

                uint cb = 0U;
                int sbs = 32768;

                bool succeed = false;

                do
                {
                    pex.ReAlloc(sbs);
                    succeed = NativeDisk.DeviceIoControl(disk, NativeDisk.IOCTL_DISK_GET_DRIVE_GEOMETRY_EX, IntPtr.Zero, 0U, pex, (uint)pex.Length, ref cb, IntPtr.Zero);

                    if (!succeed)
                    {
                        int xErr = User32.GetLastError();

                        if (xErr != NativeDisk.ERROR_MORE_DATA && xErr != NativeDisk.ERROR_INSUFFICIENT_BUFFER)
                        {
                            succeed = false;
                            break;

                            //throw new NativeException();
                        }
                    }

                    sbs *= 2;
                }
                while (!succeed);

                if (sbs == -1)
                {
                    pex.Free();
                    return false;
                }

                if (succeed) geometry = pex.ToStruct<DISK_GEOMETRY_EX>();

                if (owndisk) disk.Close();
                return succeed;
            }
        }
    }
}