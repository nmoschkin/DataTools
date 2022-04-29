using DataTools.Win32.Usb.Globalization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Power
{

    /// <summary>
    /// List of common battery device chemistries.
    /// </summary>
    public struct DeviceChemistry
    {
        public static readonly DeviceChemistry PbAc = new DeviceChemistry(nameof(AppResources.pbAc), AppResources.pbAc);
        public static readonly DeviceChemistry LION = new DeviceChemistry(nameof(AppResources.LION), AppResources.LION);
        public static readonly DeviceChemistry NiCd = new DeviceChemistry(nameof(AppResources.NiCd), AppResources.NiCd);
        public static readonly DeviceChemistry NiMH = new DeviceChemistry(nameof(AppResources.NiMH), AppResources.NiMH);
        public static readonly DeviceChemistry NiZn = new DeviceChemistry(nameof(AppResources.NiZn), AppResources.NiZn);
        public static readonly DeviceChemistry RAM = new DeviceChemistry(nameof(AppResources.RAM), AppResources.RAM);

        private string name;
        private string description;

        /// <summary>
        /// Find a device chemistry description from its short name.
        /// </summary>
        /// <param name="name">The name to look up.</param>
        /// <returns></returns>
        public static DeviceChemistry? FindByName(string name)
        {
            var fis = typeof(DeviceChemistry).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var fi in fis)
            {
                var test = (DeviceChemistry?)fi.GetValue(null);
                if (test is DeviceChemistry dc)
                {
                    if (dc.Name.ToLower() == name.ToLower()) return dc;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return $"{name} ({description})";
        }

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public DeviceChemistry(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}
