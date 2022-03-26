using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DataTools.Win32.Usb
{
    public class HidPowerDevicePageInfo : HidUsagePageInfo<HidPowerUsageInfo>
    {

        public static HidPowerDevicePageInfo Instance { get; protected set; }

        static HidPowerDevicePageInfo()
        {
            Instance = new HidPowerDevicePageInfo();
        }

        protected HidPowerDevicePageInfo() : base((HidUsagePage)0x84)
        {
        }


    }
}
