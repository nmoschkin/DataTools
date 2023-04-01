using DataTools.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Disk
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DISK_GEOMETRY_EX
    {
        public DISK_GEOMETRY Geometry;
        public LargeInteger DiskSize;
        public byte Data;
    }
}
