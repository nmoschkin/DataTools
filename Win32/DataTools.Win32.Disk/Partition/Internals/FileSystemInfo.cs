using DataTools.Win32.Disk.Interop;
using DataTools.Win32.Disk.Partition.Interfaces;
using DataTools.Win32.Memory;
using System;

namespace DataTools.Win32.Disk.Partition.Internals
{
    /// <summary>
    /// File System Utility methods
    /// </summary>
    public static class FileSystemInfo
    {
        /// <summary>
        /// Detect the file system type
        /// </summary>
        /// <param name="deviceInfo">The disk device</param>
        /// <param name="partition">The partition</param>
        /// <returns>A string that indicates the file-system type</returns>
        public static string DetectFileSystem(DiskDeviceInfo deviceInfo, IDiskPartition partition)
        {
            using (var disk = DiskHandle.OpenDisk(deviceInfo))
            {
                return InternalReadFS(deviceInfo, partition, disk);
            }
        }

        /// <summary>
        /// Detect the file system type
        /// </summary>
        /// <param name="deviceInfo">The disk device</param>
        /// <param name="partition">The partition</param>
        /// <param name="disk">The open disk handle (can be null)</param>
        /// <returns>A string that indicates the file-system type</returns>
        internal static string DetectFileSystem(DiskDeviceInfo deviceInfo, IDiskPartition partition, DiskHandle disk)
        {
            if (disk == null)
            {
                using (disk = DiskHandle.OpenDisk(deviceInfo))
                {
                    return InternalReadFS(deviceInfo, partition, disk);
                }
            }
            else
            {
                return InternalReadFS(deviceInfo, partition, disk);
            }
        }

        /// <summary>
        /// Read from disk
        /// </summary>
        /// <param name="disk">The open disk</param>
        /// <param name="bps">Bytes per sector</param>
        /// <param name="offset">Offset in bytes</param>
        /// <returns></returns>
        internal static string ReadFromDisk(DiskHandle disk, long bps, long offset)
        {
            if (disk == null || disk.IsClosed) throw new ArgumentNullException(nameof(disk));

            long fp = 0;
            IO.SetFilePointerEx(disk, offset, ref fp, IO.FilePointerMoveMethod.Begin);

            using (var expt = new SafePtr(bps))
            {
                uint br = 0;

                IO.ReadFile(disk, expt, (uint)bps, ref br, IntPtr.Zero);

                var xf = expt.GetUTF8String(3).Trim();
                if (!string.IsNullOrEmpty(xf))
                {
                    // It's something with a magic string where it's supposed to be (FAT or NTFS)

                    if (xf.StartsWith("MSDOS") || xf == "mkfs.fat") return "FAT32";
                    else if (xf == "NTFS    ") return "NTFS";
                    return "Unknown";
                }
                else
                {
                    // Let's just check one more place...

                    IO.SetFilePointerEx(disk, offset + 1024, ref fp, IO.FilePointerMoveMethod.Begin);

                    expt.Free();
                    expt.AllocZero(1024);

                    IO.ReadFile(disk, expt, 1024, ref br, IntPtr.Zero);

                    var sb = expt.ToStruct<SuperBlock>();

                    if (sb.MagicSignature == SuperBlock.MagicSignatureConstant)
                    {
                        // This is absolutely a Linux ext2/3/4 file system.
                        return "ext4";
                    }
                    else
                    {
                        // No support for detecting other things, just yet.
                        return "Unknown";

                        // TODO: HPFS
                    }
                }
            }
        }

        /// <summary>
        /// Detect the file system type
        /// </summary>
        /// <param name="deviceInfo">The disk device</param>
        /// <param name="partition">The partition</param>
        /// <param name="disk">The open disk handle (cannot be null)</param>
        /// <returns>A string that indicates the file-system type</returns>
        private static string InternalReadFS(DiskDeviceInfo deviceInfo, IDiskPartition partition, DiskHandle disk)
        {
            if (disk == null) throw new ArgumentNullException(nameof(disk));
            var bps = deviceInfo.SectorSize;
            return ReadFromDisk(disk, bps, partition.Offset);
        }
    }
}