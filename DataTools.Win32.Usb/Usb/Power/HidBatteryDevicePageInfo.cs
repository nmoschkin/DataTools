
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Power
{
    public class HidBatteryDevicePageInfo : HidUsagePageInfo<HidBatteryUsageInfo>
    {
        public static HidBatteryDevicePageInfo Instance { get; protected set; }

        static HidBatteryDevicePageInfo()
        {
            Instance = new HidBatteryDevicePageInfo();
        }


        protected HidBatteryDevicePageInfo() : base((HidUsagePage)0x85)
        {
        }

    }
}
