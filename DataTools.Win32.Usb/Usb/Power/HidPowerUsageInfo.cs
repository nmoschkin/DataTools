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
        protected string name = "";

        public override string? UsageName 
        {
            get => name;
            set
            {
                name = value ?? "";

                PowerUnitCode puc = PowerUnitCode.None;

                if (name.Contains("Current"))
                {
                    puc = PowerUnitCode.DCCurrent;
                }
                else if (name.Contains("Volt"))
                {
                    puc = PowerUnitCode.DCVoltage;
                }
                else if (name.Contains("Power"))
                {
                    puc = PowerUnitCode.ApparentPower;
                }
                else if (name.Contains("Time"))
                {
                    puc = PowerUnitCode.Time;
                }
                else if (name.Contains("Temperature"))
                {
                    puc = PowerUnitCode.Temperature;
                }
                else if (name.Contains("Freq"))
                {
                    puc = PowerUnitCode.Frequency;
                }
                else if (name.Contains("Capacity"))
                {
                    puc = PowerUnitCode.BatteryCapacity;
                }


                PowerUnit = puc;
            }
        }

        /// <summary>
        /// Returns the HID Power Unit For this usage.
        /// </summary>
        [JsonProperty("powerUnit")]
        public PowerUnitCode PowerUnit { get; set; }


    }
}
