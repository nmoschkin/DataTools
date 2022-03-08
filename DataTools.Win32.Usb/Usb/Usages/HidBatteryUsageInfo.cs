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
    public class HidBatteryUsageInfo : HidPowerUsageInfo
    {

        /// <summary>
        /// Supports Smart Battery Level
        /// </summary>
        public bool SBML { get; internal protected set; }


        public override string ToString()
        {
            return base.ToString() + (SBML ? " - SBML" : "");
        }

    }
}
