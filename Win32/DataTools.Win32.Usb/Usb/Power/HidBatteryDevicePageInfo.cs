
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Power
{
    /// <summary>
    /// Encapsulates HID Battery Device USB Page Info
    /// </summary>
    public class HidBatteryDevicePageInfo : HidUsagePageInfo<HidBatteryUsageInfo>
    {
        /// <summary>
        /// Get the singleton instance of this class
        /// </summary>
        public static HidBatteryDevicePageInfo Instance { get; protected set; }

        static HidBatteryDevicePageInfo()
        {
            Instance = new HidBatteryDevicePageInfo();
        }

        private HidBatteryDevicePageInfo() : base(HidUsagePage.PowerDevice2)
        {
        }

    }
}
