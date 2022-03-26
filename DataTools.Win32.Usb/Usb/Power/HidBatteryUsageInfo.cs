using DataTools.Win32.Usb.Usb.Power;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Describes a usage or capability within a HID Battery (0x85) Usage Page
    /// </summary>
    /// <remarks>
    /// Extends <see cref="HidUsageInfo"/> to add the <see cref="SBML"/> (Smart Battery Level) attribute.
    /// </remarks>
    public class HidBatteryUsageInfo : HidUsageInfo
    {

        /// <summary>
        /// Supports Smart Battery Level
        /// </summary>
        [JsonProperty("smbl")]
        public bool SBML { get; internal protected set; }


        public override string ToString()
        {
            return base.ToString() + (SBML ? " - SBML" : "");
        }

    }
}
