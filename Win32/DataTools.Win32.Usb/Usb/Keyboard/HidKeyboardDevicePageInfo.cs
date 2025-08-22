using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Keyboard
{
    /// <summary>
    /// Encapsulates HID Keyboard Device USB Page Info
    /// </summary>
    public class HidKeyboardDevicePageInfo : HidUsagePageInfo<HidKeyboardUsageInfo>
    {
        /// <summary>
        /// Get the singleton instance of this class
        /// </summary>
        public static HidKeyboardDevicePageInfo Instance { get; private set; } = new HidKeyboardDevicePageInfo();

        private HidKeyboardDevicePageInfo() : base(HidUsagePage.KeyboardKeypad)
        {
        }

    }
}
