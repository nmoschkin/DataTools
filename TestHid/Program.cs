


using DataTools.Hardware.Usb;
using DataTools.Win32;
using static DataTools.Win32.IO;
using static DataTools.Win32.User32;

namespace TestHid
{

    public static class Program
    {

        public static void Main(string[] args)
        {


            var hids = HidDeviceInfo.EnumerateHidDevices();


            var battery = hids.Where((e) => e.DeviceClass == DeviceClassEnum.Battery).ToList().First();


            IntPtr hHid = HidFeatures.OpenHid(battery);
            IntPtr ppd = default;
            HidAttributes attr;
            HidCaps caps;

            UsbLibHelpers.HidD_GetPreparsedData(hHid, ref ppd);

            UsbLibHelpers.HidD_GetAttributes(hHid, out attr);

            UsbLibHelpers.HidP_GetCaps(ppd, out caps);



            UsbLibHelpers.HidD_FreePreparsedData(ppd);
            CloseHandle(hHid);

        }
    }
}