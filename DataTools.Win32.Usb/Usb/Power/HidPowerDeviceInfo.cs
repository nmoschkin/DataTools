using DataTools.Win32.Memory;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// An object that represents a HID power device or battery on the local machine.
    /// </summary>
    public class HidPowerDeviceInfo : HidDeviceInfo
    {

        protected Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? powerCollections;

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

    }
}
