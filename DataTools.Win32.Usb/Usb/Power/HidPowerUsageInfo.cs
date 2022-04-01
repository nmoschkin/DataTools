
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Power
{
    public class HidPowerUsageInfo : HidUsageInfo
    {
        /// <summary>
        /// Returns the HID Power Unit For this usage.
        /// </summary>
        [JsonProperty("powerUnit")]
        public virtual UnitInfoCode PowerUnit
        {
            get => HidUnit;
            set => HidUnit = value;
        }


    }
}
