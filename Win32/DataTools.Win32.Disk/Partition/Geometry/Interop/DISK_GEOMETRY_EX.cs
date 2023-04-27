using System.Runtime.InteropServices;

namespace DataTools.Win32.Disk.Partition.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DISK_GEOMETRY_EX
    {
        public DISK_GEOMETRY Geometry;
        public LargeInteger DiskSize;
        public byte Data;
    }
}