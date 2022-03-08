using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Usb.Power
{
    /// <summary>
    /// An object that represents a HID power device or battery on the local machine.
    /// </summary>
    public class HidPowerDeviceInfo : HidDeviceInfo
    {

        protected Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? powerCollections;

        public static HidPowerDeviceInfo CreateFromHidDevice(HidDeviceInfo device)
        {
            var result = device.CopyTo<HidPowerDeviceInfo>();

            result.PopulateDeviceCaps();
            result.CreatePowerCollection();

            return result;
        }

        public void CreatePowerCollection()
        {
            var cd = GetCollection(LinkedFeatureValues);
            cd = GetCollection(LinkedInputValues, cd);
            cd = GetCollection(LinkedOutputValues, cd);

            PowerCollections = cd;
        }
       
        /// <summary>
        /// Power Usage Collections For PowerDevice1 and PowerDevice2
        /// </summary>
        public Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? PowerCollections
        {
            get => powerCollections;
            protected internal set => powerCollections = value;
        }

        /// <summary>
        /// Create a collection from the linked list.
        /// </summary>
        /// <param name="data">The linked usage data.</param>
        /// <param name="currDict">The dictionary to add to.</param>
        /// <returns>
        /// Either <paramref name="currDict"/> or a new dictionary.
        /// </returns>
        protected internal Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>> GetCollection(Dictionary<int, IList<HidPValueCaps>> data, Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? currDict = null)
        {
            var result = currDict ?? new Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>();

            var bref = HidBatteryDevicePageInfo.Instance;
            var pref = HidPowerDevicePageInfo.Instance;

            foreach (var kv in data)
            {
                var valcap = kv.Key;
                var list = kv.Value;

                var testVal = list.FirstOrDefault();
                
                if (testVal.UsagePage == HidUsagePage.PowerDevice1)
                {
                    var bitem = bref.Where((x) => x.UsageId == valcap).FirstOrDefault();
                    if (bitem == null) continue;

                    var l = new List<HidPowerUsageInfo>();

                    foreach (var item in list)
                    {
                        var bitem2 = bref.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                        if (bitem2 == null) continue;

                        l.Add((HidPowerUsageInfo)bitem2.Clone());
                    }

                    result.Add((HidPowerUsageInfo)bitem.Clone(), l);
                }
                else if (testVal.UsagePage == HidUsagePage.PowerDevice2)
                {
                    var pitem = pref.Where((x) => x.UsageId == valcap).FirstOrDefault();
                    if (pitem == null) continue;

                    var l = new List<HidPowerUsageInfo>();

                    foreach (var item in list)
                    {
                        var pitem2 = pref.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                        if (pitem2 == null) continue;

                        l.Add((HidPowerUsageInfo)pitem2.Clone());
                    }

                    result.Add((HidPowerUsageInfo)pitem.Clone(), l);
                }
            }

            return result;
        }


    }
}
