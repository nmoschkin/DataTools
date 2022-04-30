using DataTools.Win32;
using DataTools.Win32.Disk.Partition;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Disk
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DISK_GEOMETRY
    {

        public LARGE_INTEGER Cylinders;
        public MEDIA_TYPE MediaType;
        public uint TracksPerCylinder;
        public uint SectorsPerTrack;
        public uint BytesPerSector;
    }
}
