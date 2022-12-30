using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTools.Win32.Usb.Power
{
    /// <summary>
    /// An object that represents a HID power device or battery on the local machine.
    /// </summary>
    public class HidPowerDeviceInfo : HidDeviceInfo
    {
        protected Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>> powerCollections;

        /// <summary>
        /// Create a <see cref="HidPowerDeviceInfo"/> object from an existing <see cref="HidDeviceInfo"/> object.
        /// </summary>
        /// <param name="device">The device information to instantiate from.</param>
        /// <returns>A new <see cref="HidPowerDeviceInfo"/> object.</returns>
        /// <exception cref="ArgumentException">Thrown if the device class is not 'battery.'</exception>
        /// <remarks>
        /// The incoming object must be of device class 'Battery'.
        /// </remarks>
        public static HidPowerDeviceInfo CreateFromHidDevice(HidDeviceInfo device)
        {
            if (device.DeviceClass != DeviceClassEnum.Battery) throw new ArgumentException($"{nameof(device)} must have a device class of {DeviceClassEnum.Battery}");

            var result = device.CopyTo<HidPowerDeviceInfo>();

            result.PopulateDeviceCaps();
            result.CreateUsageCollection();

            return result;
        }

        public override object RetrieveValue(HidUsageInfo item, bool populateItemValue = false)
        {
            var res = base.RetrieveValue(item, populateItemValue);

            if (item.ValueCaps is HidPValueCaps vc2 && res != null)
            {
                if (item.UsageId == (byte)HidPowerUsageCode.AudibleAlarmControl && vc2.UsagePage == HidUsagePage.PowerDevice1)
                {
                    res = (AudibleAlarmControlState)(int)res;
                    if (populateItemValue) item.Value = res;
                }
                else if (item.UsageId == (byte)HidPowerUsageCode.Test && vc2.UsagePage == HidUsagePage.PowerDevice1)
                {
                    res = (HidPowerTestState)(int)res;
                    if (populateItemValue) item.Value = res;
                }
                else if (item.UsageId == (byte)HidBatteryUsageCode.CapacityMode && vc2.UsagePage == HidUsagePage.PowerDevice2)
                {
                    res = (PowerCapacityMode)(int)res;
                    if (populateItemValue) item.Value = res;
                }
                else if (item.UsageId == (byte)HidBatteryUsageCode.Rechargable && vc2.UsagePage == HidUsagePage.PowerDevice2)
                {
                    res = (int)res == 1;

                    if (populateItemValue)
                    {
                        item.IsButton = true;
                        item.ButtonValue = (bool)res;
                    }
                }
                else if (res is string strres && !string.IsNullOrEmpty(strres))
                {
                    if (item.UsageId == (byte)HidBatteryUsageCode.iDeviceChemistery && vc2.UsagePage == HidUsagePage.PowerDevice2 && DeviceChemistry.FindByName(strres) is DeviceChemistry dchem)
                    {
                        res = dchem;
                        if (populateItemValue) item.Value = res;
                    }
                }
            }

            return res;
        }
    }
}