using System;
using System.Linq;
using System.Text;

namespace DataTools.Win32.Usb
{
    public interface ICaps : ICloneable
    {
        byte ReportID { get; }

        ushort Usage { get; }

        ushort LinkCollection { get; }

        ushort LinkUsage { get; }

        HidUsagePage UsagePage { get; }

        HidUsagePage LinkUsagePage { get; }

        bool IsRange { get; }

        ushort UsageMin { get; }

        ushort UsageMax { get; }

        bool IsButton { get; }
    }
}