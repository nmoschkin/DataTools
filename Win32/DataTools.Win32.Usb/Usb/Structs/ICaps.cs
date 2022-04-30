using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public interface ICaps : ICloneable
    {
        public byte ReportID { get; }

        public ushort Usage { get; }

        public ushort LinkCollection { get; }

        public ushort LinkUsage { get; }

        public HidUsagePage UsagePage { get; }

        public HidUsagePage LinkUsagePage { get; }

        public bool IsRange { get; }

        public ushort UsageMin { get; }

        public ushort UsageMax { get; }

        public bool IsButton { get; }
    }
}
