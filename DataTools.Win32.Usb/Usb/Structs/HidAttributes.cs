using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32
{
    public struct HidAttributes
    {
        public uint Size; // = sizeof (struct _HIDD_ATTRIBUTES)

        //
        // Vendor ids of this hid device
        //
        public ushort VendorID;
        public ushort ProductID;
        public ushort VersionNumber;

        //
        // Additional fields will be added to the end of this structure.
        //
    }
}
