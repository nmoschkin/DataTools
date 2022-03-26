using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public class HidPowerUsageInfo : HidUsageInfo
    {
        /// <summary>
        /// Returns the HID Power Unit For this usage.
        /// </summary>
        [JsonProperty("powerUnit")]
        public virtual UnitInfoCode PowerUnit
        {
            get => base.HidUnit;
            set => base.HidUnit = value;
        }


    }
}
