using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Supported HID Report Types
    /// </summary>
    public enum HidReportType
    {
        /// <summary>
        /// Input Report
        /// </summary>
        Input = 0,

        /// <summary>
        /// Output Report
        /// </summary>
        Output = 1,

        /// <summary>
        /// Feature Report
        /// </summary>
        Feature = 2
    }
}
