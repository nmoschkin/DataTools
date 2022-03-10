using System;
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
            result.CreatePowerCollection();

            return result;
        }

        /// <summary>
        /// Create the power usage page collections.
        /// </summary>
        public void CreatePowerCollection()
        {
            var cd = GetCollection(LinkedFeatureValues, HidReportType.Feature);
            cd = GetCollection(LinkedInputValues, HidReportType.Input, cd);
            cd = GetCollection(LinkedOutputValues, HidReportType.Output, cd);

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
        /// Retrieve dynamic values live from the device.
        /// </summary>
        /// <returns></returns>
        public Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? RefreshDynamicValues()
        {
            return GetFeatureValues(HidUsageType.CP | HidUsageType.CL, HidUsageType.DV);
        }

        /// <summary>
        /// Retrieve values live from the device by collection and usage type.
        /// </summary>
        /// <param name="collectionType">The collection type or types to return.</param>
        /// <param name="usageType">The usage types to return.</param>
        /// <returns><see cref="Dictionary{HidPowerUsageInfo, List{HidPowerUsageInfo}}"/> of live values organized by usage collection.</returns>
        /// <remarks>
        /// The <paramref name="collectionType"/> and <paramref name="usageType"/> properties can be OR'd to retrieve more than one kind of value set.
        /// </remarks>
        public Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? GetFeatureValues(HidUsageType collectionType, HidUsageType usageType)
        {
            var result = new Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>();
            if (PowerCollections == null) return null;

            foreach(var kvp in PowerCollections)
            {
                if ((collectionType & kvp.Key.UsageType) == kvp.Key.UsageType && kvp.Key.ReportType == HidReportType.Feature)
                {
                    foreach (var item in kvp.Value)
                    {
                        if ((usageType & item.UsageType) == item.UsageType)
                        {
                            int res = 0;
                            var b = HidGetFeature(item.ReportID, out res);
                            if (b)
                            {
                                item.Value = res;

                                if (!result.TryGetValue(kvp.Key, out List<HidPowerUsageInfo>? col))
                                {
                                    col = new List<HidPowerUsageInfo>();
                                    result.Add(kvp.Key, col);
                                }

                                col.Add(item);
                            }

                        }
                    }

                }
            }

            return result;
        }


        /// <summary>
        /// Create a power device collection from the pre-populated linked list of collections and usages.
        /// </summary>
        /// <param name="data">The linked usage data.</param>
        /// <param name="currDict">The dictionary to add to.</param>
        /// <returns>
        /// Either <paramref name="currDict"/> or a new dictionary.
        /// </returns>
        public Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>> GetCollection(Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> data, HidReportType reportType, Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? currDict = null)
        {
            var result = currDict ?? new Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>();

            var bref = HidBatteryDevicePageInfo.Instance;
            var pref = HidPowerDevicePageInfo.Instance;

            foreach (var kv in data)
            {
                var page = kv.Key.Item1;
                var valcap = kv.Key.Item2;
                var list = kv.Value;
                
                if (page == HidUsagePage.PowerDevice2)
                {
                    var bitem = bref.Where((x) => x.UsageId == valcap && x.UsageName != "Reserved").FirstOrDefault();
                    if (bitem == null) continue;

                    var l = new List<HidPowerUsageInfo>();

                    foreach (var item in list)
                    {
                        var bitem2 = bref.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                        if (bitem2 == null) continue;
                        var newItem = (HidPowerUsageInfo)bitem2.Clone(reportType);
                        newItem.ReportID = item.ReportID;
                        l.Add(newItem);
                    }

                    result.Add((HidPowerUsageInfo)bitem.Clone(reportType), l);
                }
                else if (page == HidUsagePage.PowerDevice1)
                {
                    var pitem = pref.Where((x) => x.UsageId == valcap && x.UsageName != "Reserved").FirstOrDefault();
                    if (pitem == null) continue;

                    var l = new List<HidPowerUsageInfo>();

                    foreach (var item in list)
                    {
                        var pitem2 = pref.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                        if (pitem2 == null) continue;

                        var newItem = (HidPowerUsageInfo)pitem2.Clone(reportType);
                        newItem.ReportID = item.ReportID;
                        l.Add(newItem);
                    }

                    result.Add((HidPowerUsageInfo)pitem.Clone(reportType), l);
                }
            }

            return result;
        }


    }
}
