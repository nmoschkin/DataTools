using System.Runtime.InteropServices;

namespace DataTools.Win32.Disk.Partition.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DISK_GEOMETRY
    {
        public LargeInteger Cylinders;
        public MediaType MediaType;
        public uint TracksPerCylinder;
        public uint SectorsPerTrack;
        public uint BytesPerSector;
    }
}