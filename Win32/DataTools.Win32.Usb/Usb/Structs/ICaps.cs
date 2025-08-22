using System;
using System.Linq;
using System.Text;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Presents a common interface for various flavors of HID device caps
    /// </summary>
    public interface ICaps : ICloneable
    {
        /// <summary>
        /// Report ID
        /// </summary>
        byte ReportID { get; }

        /// <summary>
        /// Usage
        /// </summary>
        ushort Usage { get; }

        /// <summary>
        /// Link collection
        /// </summary>
        ushort LinkCollection { get; }

        /// <summary>
        /// Link usage
        /// </summary>
        ushort LinkUsage { get; }

        /// <summary>
        /// Usage page
        /// </summary>
        HidUsagePage UsagePage { get; }

        /// <summary>
        /// Link usage page
        /// </summary>
        HidUsagePage LinkUsagePage { get; }

        /// <summary>
        /// Is a range
        /// </summary>
        bool IsRange { get; }

        /// <summary>
        /// Usage minimum
        /// </summary>
        ushort UsageMin { get; }

        /// <summary>
        /// Usage maximum
        /// </summary>
        ushort UsageMax { get; }

        /// <summary>
        /// Is a button
        /// </summary>
        bool IsButton { get; }
    }
}