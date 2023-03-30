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
        /// Enumerates all the partitions on a physical device.
        /// </summary>
        /// <param name="devicePath">The disk device path to query.</param>
        /// <param name="hfile">Optional valid disk handle.</param>
        /// <param name="geometry">Receives disk geometry information.</param>
        /// <returns>An array of PARTITION_INFORMATION_EX structures.</returns>
        /// <remarks></remarks>
        public static bool GetDiskGeometry(string devicePath, DiskHandle hfile, out DISK_GEOMETRY_EX? geometry)
        {
            geometry = null;

            if (devicePath != null && hfile == null)
            {
                hfile = DiskHandle.OpenDisk(devicePath);
            }
            else if (hfile == null)
            {
                return false;
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
                    succeed = NativeDisk.DeviceIoControl(hfile, NativeDisk.IOCTL_DISK_GET_DRIVE_GEOMETRY_EX, IntPtr.Zero, 0U, pex, (uint)pex.Length, ref cb, IntPtr.Zero);

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
                return succeed;
            }


        }
    }
}