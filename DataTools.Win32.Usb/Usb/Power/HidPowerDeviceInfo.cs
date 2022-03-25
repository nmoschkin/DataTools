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
            cd = GetCollection(LinkedFeatureButtons, HidReportType.Feature, cd);
            cd = GetCollection(LinkedInputButtons, HidReportType.Input, cd);
            cd = GetCollection(LinkedOutputButtons, HidReportType.Output, cd);

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
            var retDict = GetFeatureValues(HidUsageType.CP | HidUsageType.CL | HidUsageType.CA, HidUsageType.DV | HidUsageType.DF);

            if (retDict != null)
            {
                var leftovers = GetFeatureValues(HidUsageType.Reserved, HidUsageType.DV);
                if (leftovers != null)
                {
                    foreach (var kvp in leftovers)
                    {
                        retDict.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return retDict;
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
        public Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? GetFeatureValues(HidUsageType collectionType, HidUsageType usageType, bool includeUnlinked = false)
        {
            var result = new Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>();
            if (PowerCollections == null) return null;

            foreach(var kvp in PowerCollections)
            {
                if ((collectionType & kvp.Key.UsageType) != 0 || (kvp.Key.UsageType == HidUsageType.Reserved && (collectionType == HidUsageType.Reserved || includeUnlinked)))
                {
                    foreach (var item in kvp.Value)
                    {
                        if ((usageType & item.UsageType) != 0)
                        {                           

                            if (item.IsButton && item.ButtonCaps != null)
                            {
                                var btncaps = UsbLibHelpers.GetButtonStatesRaw(this, item.ReportType, item.ButtonCaps.Value);
                                item.ButtonValue = false;

                                foreach (var pair in btncaps)
                                {
                                    if (/*pair.Item1 == kvp.Key.UsageId &&*/ pair.Item2 == item.UsageId)
                                    {
                                        item.ButtonValue = true;
                                        break;
                                    }
                                }

                                if (!result.TryGetValue(kvp.Key, out List<HidPowerUsageInfo>? col))
                                {
                                    col = new List<HidPowerUsageInfo>();
                                    result.Add(kvp.Key, col);
                                }

                                col.Add(item);
                            }
                            else if (item.ValueCaps is HidPValueCaps vc && vc.StringIndex != 0)
                            {
                                using (var mm = new SafePtr(256))
                                {
                                    var hhid = HidFeatures.OpenHid(this);
                                    if (hhid != IntPtr.Zero)
                                    {
                                        var b = UsbLibHelpers.HidD_GetIndexedString(hhid, vc.StringIndex, mm, (int)mm.Length);
                                        HidFeatures.CloseHid(hhid);

                                        if (b)
                                        {
                                            var strres = mm.ToString();
                                            if (!string.IsNullOrEmpty(strres))
                                            {

                                                if (item.UsageId == 0x89 && vc.UsagePage == HidUsagePage.PowerDevice2 && DeviceChemistry.FindByName(strres) is DeviceChemistry dchem)
                                                {
                                                    item.Value = dchem;
                                                }
                                                else
                                                {
                                                    item.Value = strres;
                                                }

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
                            else
                            {
                                var b = HidGetFeature(item.ReportID, out int res);

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
            }


            return result;
        }


        /// <summary>
        /// Looks up a specific HID Usage value by usageId and collectionId.
        /// </summary>
        /// <param name="usageId">The code to look up.</param>
        /// <param name="collectionId">Specify a collectionId to ensure that the usage is a member of this collection.</param>
        /// <param name="retrieveValue">True to call the device and populate the <see cref="HidPowerUsageInfo.Value"/> property with a real-time value.</param>
        /// <returns>A HID Usage Page or null.</returns>
        /// <remarks>
        /// Several reported usages can have identical <paramref name="usageId"/>'s. Only the first found is returned if <paramref name="collectionId"/> is not specified.
        /// </remarks>
        public HidPowerUsageInfo? LookupValue(byte usageId, byte collectionId = 0, bool retrieveValue = false)
        {
            if (PowerCollections == null) return null;  

            //var features = deviceInfo.GetFeatureValues(HidUsageType.CL | HidUsageType.CA | HidUsageType.CP, HidUsageType.DV | HidUsageType.SV);

            foreach (var kvp in PowerCollections)
            {
                if (collectionId == 0 || kvp.Key.UsageId == collectionId)
                {
                    foreach (var item in kvp.Value)
                    {
                        if (item.UsageId == usageId)
                        {
                            var newitem = item;
                            if (retrieveValue)
                            {
                                if (item.IsButton && item.ButtonCaps != null)
                                {
                                    var btncaps = UsbLibHelpers.GetButtonStatesRaw(this, item.ReportType, item.ButtonCaps.Value);

                                    newitem.ButtonValue = false;

                                    foreach (var pair in btncaps)
                                    {
                                        if (/*pair.Item1 == kvp.Key.UsageId &&*/ pair.Item2 == item.UsageId)
                                        {
                                            newitem.ButtonValue = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    var b = HidGetFeature(item.ReportID, out int res);

                                    if (b)
                                        newitem.Value = res;
                                }
                            }

                            return newitem;
                        }
                    }

                }

            }

            return null;
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
                var collectionid = kv.Key.Item2;
                var list = kv.Value;

                HidUsageInfo? bitem;

                if (page == HidUsagePage.PowerDevice1)
                {
                    bitem = HidPowerDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }
                else
                {
                    bitem = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }

                if (bitem == null) continue;

                List<HidPowerUsageInfo> l;

                l = result.Where((scan) => scan.Key.UsageId == collectionid && scan.Key.ReportType == reportType).Select((scan2) => scan2.Value).FirstOrDefault() ?? new List<HidPowerUsageInfo>();
                var newres = l.Count == 0;

                foreach (var item in list)
                {
                    HidUsageInfo? bitem2;

                    if (item.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        bitem2 = HidPowerDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else
                    {
                        bitem2 = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }

                    if (bitem2 == null) continue;

                    if (l.Count((scan3) => scan3.UsageId == bitem2.UsageId) == 0)
                    {
                        var newItem = (HidPowerUsageInfo)bitem2.Clone(reportType);

                        newItem.ReportID = item.ReportID;
                        newItem.ValueCaps = item;

                        newItem.IsButton = false;

                        l.Add(newItem);

                    }

                }

                if (newres) result.Add((HidPowerUsageInfo)bitem.Clone(reportType), l);
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
        public Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>> GetCollection(Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> data, HidReportType reportType, Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>? currDict = null)
        {
            var result = currDict ?? new Dictionary<HidPowerUsageInfo, List<HidPowerUsageInfo>>();

            foreach (var kv in data)
            {
                var page = kv.Key.Item1;
                var collectionid = kv.Key.Item2;
                var list = kv.Value;

                HidUsageInfo? bitem;

                if (page == HidUsagePage.PowerDevice1)
                {
                    bitem = HidPowerDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }
                else
                {
                    bitem = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }

                if (bitem == null) continue;

                List<HidPowerUsageInfo> l;

                l = result.Where((scan) => scan.Key.UsageId == collectionid && scan.Key.ReportType == reportType).Select((scan2) => scan2.Value).FirstOrDefault() ?? new List<HidPowerUsageInfo>();
                var newres = l.Count == 0;

                foreach (var item in list)
                {
                    HidUsageInfo? bitem2;

                    if (item.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        bitem2 = HidPowerDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else
                    {
                        bitem2 = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }

                    if (bitem2 == null) continue;

                    if (l.Count((scan3) => scan3.UsageId == bitem2.UsageId) == 0)
                    {
                        var newItem = (HidPowerUsageInfo)bitem2.Clone(reportType);

                        newItem.ReportID = item.ReportID;
                        newItem.ButtonCaps = item;
                        newItem.IsButton = true;
                        l.Add(newItem);

                    }

                }

                if (newres) result.Add((HidPowerUsageInfo)bitem.Clone(reportType, true), l);
            }

            return result;
        }

    }
}
