using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{

    /// <summary>
    /// List of common battery device chemistries.
    /// </summary>
    public struct DeviceChemistry
    {
        public static DeviceChemistry PbAc = new DeviceChemistry("PbAc", "Lead Acid");
        public static DeviceChemistry LION = new DeviceChemistry("LION", "Lithium Ion");
        public static DeviceChemistry NiCd = new DeviceChemistry("NiCd", "Nickel Cadmium");
        public static DeviceChemistry NiMH = new DeviceChemistry("NiMH", "Nickel Metal Hydride");
        public static DeviceChemistry NiZn = new DeviceChemistry("NiZn", "Nickel Zinc");
        public static DeviceChemistry RAM = new DeviceChemistry("RAM", "Rechargeable Alkaline-Manganese");

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
