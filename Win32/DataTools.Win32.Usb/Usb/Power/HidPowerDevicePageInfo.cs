
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DataTools.Win32.Usb.Power
{
    /// <summary>
    /// Encapsulates HID Power Device USB Page Info
    /// </summary>
    public class HidPowerDevicePageInfo : HidUsagePageInfo<HidPowerUsageInfo>
    {
        /// <summary>
        /// Get the singleton instance of this class
        /// </summary>
        public static HidPowerDevicePageInfo Instance { get; protected set; }

        static HidPowerDevicePageInfo()
        {
            Instance = new HidPowerDevicePageInfo();
        }

        private HidPowerDevicePageInfo() : base(HidUsagePage.PowerDevice1)
        {
        }


    }
}
