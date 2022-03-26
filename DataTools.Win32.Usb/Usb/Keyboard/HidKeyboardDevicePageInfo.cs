using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Keyboard
{
    public class HidKeyboardDevicePageInfo : HidUsagePageInfo<HidKeyboardUsageInfo>
    {
        public static HidKeyboardDevicePageInfo Instance { get; private set; } = new HidKeyboardDevicePageInfo(0x10);

        protected HidKeyboardDevicePageInfo(int pageId) : base(pageId)
        {
        }

    }
}
