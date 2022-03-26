﻿// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: HidDeviceInfo
//         USB HID Device derived class.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using System;

using DataTools.Text;
using DataTools.Win32.Memory;
using DataTools.Win32.Usb.Keyboard;

namespace DataTools.Win32.Usb
{


    /// <summary>
    /// An object that represents a Human Interface Device USB device on the system.
    /// </summary>
    /// <remarks></remarks>
    public class HidDeviceInfo : DeviceInfo
    {
        protected Dictionary<HidUsageInfo, List<HidUsageInfo>>? usageCollections;
               

        protected HidUsagePage hidPage;

        protected string serialNumber = "";
        protected string productString = "";
        protected string physicalDescriptor = "";
        protected string hidManufacturer = "";

        protected HidPValueCaps[]? featureValCaps;
        protected HidPValueCaps[]? inputValCaps;
        protected HidPValueCaps[]? outputValCaps;

        protected HidPButtonCaps[]? featureBtnCaps;
        protected HidPButtonCaps[]? inputBtnCaps;
        protected HidPButtonCaps[]? outputBtnCaps;

        protected HidCaps? hidCaps;

        protected Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> featureValMap = new Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>();
        protected Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> inputValMap = new Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>();
        protected Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> outputValMap = new Dictionary<(HidUsagePage, int), IList<HidPValueCaps>>();

        protected Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> featureBtnMap = new Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>();
        protected Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> inputBtnMap = new Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>();
        protected Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> outputBtnMap = new Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>>();


        /// <summary>
        /// Enumarate all HID class devices on the local machine.
        /// </summary>
        /// <param name="populateDevCaps"><see cref="true"/> to enumerate all device capabilities for each device.</param>
        /// <returns></returns>
        public static HidDeviceInfo[] EnumerateHidDevices(bool populateDevCaps = false)
        {
            var result = DeviceEnum.EnumerateDevices<HidDeviceInfo>(DevProp.GUID_DEVINTERFACE_HID);

            if (populateDevCaps)
            {
                foreach (var device in result)
                {
                    device.PopulateDeviceCaps();
                    device.CreateUsageCollection();
                }
            }

            return result;
        }
        
        /// <summary>
        /// Populate All Device Capabilities
        /// </summary>
        /// <returns></returns>
        public bool PopulateDeviceCaps()
        {
            return UsbLibHelpers.PopulateDeviceCaps(this);
        }

        /// <summary>
        /// Create the usage page collections.
        /// </summary>
        public void CreateUsageCollection()
        {
            var cd = GetCollection(LinkedFeatureValues, HidReportType.Feature);
            cd = GetCollection(LinkedInputValues, HidReportType.Input, cd);
            cd = GetCollection(LinkedOutputValues, HidReportType.Output, cd);
            cd = GetCollection(LinkedFeatureButtons, HidReportType.Feature, cd);
            cd = GetCollection(LinkedInputButtons, HidReportType.Input, cd);
            cd = GetCollection(LinkedOutputButtons, HidReportType.Output, cd);

            UsageCollections = cd;
        }


        /// <summary>
        /// Returns the raw byte data for a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <param name="expectedSize">The expected size, in bytes, of the result.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte featureCode, out byte[] result, int expectedSize)
        {
            bool success = false;

            var hfile = HidFeatures.OpenHid(this);

            if (hfile == IntPtr.Zero)
            {
                result = new byte[0];
                return false;
            }

            using (var mm = new SafePtr())
            {
                mm.Alloc(expectedSize + 1);
                mm.ByteAt(0L) = featureCode;

                if (!UsbLibHelpers.HidD_GetFeature(hfile, mm, expectedSize))
                {
                    success = false;
                    result = new byte[0];
                }
                else
                {
                    success = true;
                    result = mm.ToByteArray(1L, expectedSize);
                }

                HidFeatures.CloseHid(hfile);
            }

            return success;
        }

        /// <summary>
        /// Returns the short value of a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte featureCode, out short result)
        {
            result = 0;
            
            bool success;

            var hfile = HidFeatures.OpenHid(this);
            
            if (hfile == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(3L);
                mm.ByteAt(0L) = featureCode;

                if (!UsbLibHelpers.HidD_GetFeature(hfile, mm, 3))
                {
                    success = false;
                }
                else
                {
                    success = true;
                    result = mm.ShortAtAbsolute(1L);
                }
            }

            HidFeatures.CloseHid(hfile);
            return success;
        }

        /// <summary>
        /// Returns the integer value of a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte featureCode, out int result)
        {
            result = 0;
            
            bool success;
            
            var hfile = HidFeatures.OpenHid(this);
            
            if (hfile == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(5L);
                mm.ByteAt(0L) = featureCode;

                if (!UsbLibHelpers.HidD_GetFeature(hfile, mm, 5))
                {
                    success = false;
                }
                else
                {
                    success = true;
                    result = mm.IntAtAbsolute(1L);
                }
            }

            HidFeatures.CloseHid(hfile);

            return success;
        }

        /// <summary>
        /// Returns the long value of a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte featureCode, out long result)
        {
            result = 0;
            bool success;

            var hfile = HidFeatures.OpenHid(this);

            if (hfile == IntPtr.Zero) return false;

            using(var mm = new SafePtr())
            {
                mm.Alloc(9L);
                mm.ByteAt(0L) = featureCode;

                if (!UsbLibHelpers.HidD_GetFeature(hfile, mm, 9))
                {
                    success = false;
                }
                else
                {
                    success = true;
                    result = mm.LongAtAbsolute(1L);
                }
            }

            HidFeatures.CloseHid(hfile);

            return success;
        }

        /// <summary>
        /// Sets the raw byte value of a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte featureCode, byte[] value)
        {
            bool success;

            var hfile = HidFeatures.OpenHid(this);

            if (hfile == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(value.Length + 1);
                mm.FromByteArray(value, 1L);
                mm.ByteAt(0L) = featureCode;

                if (!UsbLibHelpers.HidD_SetFeature(hfile, mm, (int)mm.Length))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }
            }
            
            HidFeatures.CloseHid(hfile);
            return success;
        }

        /// <summary>
        /// Sets the short value of a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte featureCode, short value)
        {
            bool success;
            var hfile = HidFeatures.OpenHid(this);

            if (hfile == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(3L);
                mm.ByteAt(0L) = featureCode;
                mm.ShortAtAbsolute(1L) = value;
                if (!UsbLibHelpers.HidD_SetFeature(hfile, mm, 3))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }

                HidFeatures.CloseHid(hfile);
            }

            return success;
        }

        /// <summary>
        /// Sets the integer value of a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte featureCode, int value)
        {
            bool success;
            
            var hfile = HidFeatures.OpenHid(this);

            if (hfile == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(5L);
                mm.ByteAt(0L) = featureCode;
                mm.IntAtAbsolute(1L) = value;

                if (!UsbLibHelpers.HidD_SetFeature(hfile, mm, 5))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }

                HidFeatures.CloseHid(hfile);

            }

            return success;
        }

        /// <summary>
        /// Sets the long value of a Hid feature code.
        /// </summary>
        /// <param name="featureCode">The Hid feature code to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte featureCode, long value)
        {
            bool success;
            
            var hfile = HidFeatures.OpenHid(this);

            if (hfile == IntPtr.Zero) return false;
            
            using (var mm = new SafePtr())
            {
                mm.Alloc(9L);
                mm.ByteAt(0L) = featureCode;
                mm.LongAtAbsolute(1L) = value;
                if (!UsbLibHelpers.HidD_SetFeature(hfile, mm, 9))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }

                HidFeatures.CloseHid(hfile);
            }

            return success;
        }

        
        /// <summary>
        /// Returns the HID device manufacturer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string HidManufacturer
        {
            get
            {
                if (string.IsNullOrEmpty(hidManufacturer))
                {

                    using (var mm = new SafePtr())
                    {
                        var dev = HidFeatures.OpenHid(this);

                        mm.AllocZero(128L * sizeof(char));

                        UsbLibHelpers.HidD_GetManufacturerString(dev, mm, (int)mm.Length);

                        hidManufacturer = mm.GetString(0L);

                        HidFeatures.CloseHid(dev);
                    }

                }

                return hidManufacturer;
            }

            internal set
            {
                hidManufacturer = value;
            }
        }

        /// <summary>
        /// Returns the HID device serial number.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SerialNumber
        {
            get
            {
                if (string.IsNullOrEmpty(serialNumber))
                {
                    using (var mm = new SafePtr())
                    {
                        var dev = HidFeatures.OpenHid(this);

                        mm.AllocZero(128L * sizeof(char));

                        UsbLibHelpers.HidD_GetSerialNumberString(dev, mm, (int)mm.Length);

                        serialNumber = (string)mm;

                        HidFeatures.CloseHid(dev);
                    }
                }

                return serialNumber;
            }

            internal set
            {
                serialNumber = value;
            }
        }

        /// <summary>
        /// Returns the HID device product descriptor.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ProductString
        {
            get
            {
                if (string.IsNullOrEmpty(productString))
                {
                    using (var mm = new SafePtr())
                    {
                        var dev = HidFeatures.OpenHid(this);

                        mm.AllocZero(128L * sizeof(char));
                        var b = UsbLibHelpers.HidD_GetProductString(dev, mm, (int)mm.Length);

                        productString = (string)mm;

                        HidFeatures.CloseHid(dev);
                    }
                }

                return productString;
            }

            internal set
            {
                productString = value;
            }
        }

        /// <summary>
        /// Returns the ProductString if available, or the FriendlyName, otherwise.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string FriendlyName
        {
            get
            {
                if (string.IsNullOrEmpty(ProductString) == false)
                    return ProductString;
                else
                    return base.FriendlyName;
            }

            internal set
            {
                base.FriendlyName = value;
            }
        }

        /// <summary>
        /// Returns the HID device physical descriptor.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PhysicalDescriptor
        {
            get
            {
                if (physicalDescriptor is null)
                {
                    using (var mm = new SafePtr())
                    {
                        var dev = HidFeatures.OpenHid(this);

                        mm.AllocZero(512L);

                        UsbLibHelpers.HidD_GetPhysicalDescriptor(dev, mm, (int)mm.Length);

                        physicalDescriptor = (string)mm;

                        HidFeatures.CloseHid(dev);
                    }
                }

                return physicalDescriptor;
            }

            internal set
            {
                physicalDescriptor = value;
            }
        }

        /// <summary>
        /// Returns the HID usage description.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string HidUsageDescription
        {
            get
            {
                return DevEnumHelpers.GetEnumDescription(hidPage);
            }
        }

        /// <summary>
        /// Returns the HID usage page type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public HidUsagePage HidUsagePage
        {
            get
            {
                return hidPage;
            }
        }

        /// <summary>
        /// Device HID Capabilities
        /// </summary>
        public HidCaps HidCaps
        {
            get => hidCaps ?? default;
            internal set => hidCaps = value;
        }

        /// <summary>
        /// HID Feature Value Capabilities
        /// </summary>
        public HidPValueCaps[] FeatureValueCaps
        {
            get => featureValCaps ?? new HidPValueCaps[0];
            internal set => featureValCaps = value;  
        }

        /// <summary>
        /// HID Input Value Capabilities
        /// </summary>
        public HidPValueCaps[] InputValueCaps
        {
            get => inputValCaps ?? new HidPValueCaps[0];
            internal set => inputValCaps = value;
        }

        /// <summary>
        /// HID Output Value Capabilities
        /// </summary>
        public HidPValueCaps[] OutputValueCaps
        {
            get => outputValCaps ?? new HidPValueCaps[0];
            internal set => outputValCaps = value;

        }


        /// <summary>
        /// HID Feature Button Capabilities
        /// </summary>
        public HidPButtonCaps[] FeatureButtonCaps
        {
            get => featureBtnCaps ?? new HidPButtonCaps[0];
            internal set => featureBtnCaps = value;
        }

        /// <summary>
        /// HID Feature Input Capabilities
        /// </summary>
        public HidPButtonCaps[] InputButtonCaps
        {
            get => inputBtnCaps ?? new HidPButtonCaps[0];
            internal set => inputBtnCaps = value;
        }

        /// <summary>
        /// HID Output Button Capabilities
        /// </summary>
        public HidPButtonCaps[] OutputButtonCaps
        {
            get => outputBtnCaps ?? new HidPButtonCaps[0];
            internal set => outputBtnCaps = value;

        }

        /// <summary>
        /// Usage Linked Feature Values
        /// </summary>
        public Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> LinkedFeatureValues
        {
            get => featureValMap;
            internal set => featureValMap = value;
        }

        /// <summary>
        /// Usage Linked Input Values
        /// </summary>
        public Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> LinkedInputValues
        {
            get => inputValMap;
            internal set => inputValMap = value;
        }

        /// <summary>
        /// Usage Linked Output Values
        /// </summary>
        public Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> LinkedOutputValues
        {
            get => outputValMap;
            internal set => outputValMap = value;
        }


        /// <summary>
        /// Usage Linked Feature Buttons
        /// </summary>
        public Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> LinkedFeatureButtons
        {
            get => featureBtnMap;
            internal set => featureBtnMap = value;
        }

        /// <summary>
        /// Usage Linked Input Buttons
        /// </summary>
        public Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> LinkedInputButtons
        {
            get => inputBtnMap;
            internal set => inputBtnMap = value;
        }

        /// <summary>
        /// Usage Linked Output Buttons
        /// </summary>
        public Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> LinkedOutputButtons
        {
            get => outputBtnMap;
            internal set => outputBtnMap = value;
        }


        /// <summary>
        /// Usage Collections for this <see cref="HidUsagePage"/>.
        /// </summary>
        public Dictionary<HidUsageInfo, List<HidUsageInfo>>? UsageCollections
        {
            get => usageCollections;
            protected internal set => usageCollections = value;
        }


        protected override void ParseHw()
        {
            base.ParseHw();
            string[] v;

            // this is how we determine the HID class of the device. I've found this to be a very reliable method.
            foreach (string hw in _HardwareIds)
            {
                int i = hw.IndexOf("HID_DEVICE_UP:");
                if (i >= 0)
                {
                    v = TextTools.Split(hw.Substring(i), ":");
                    if (v.Length > 1)
                    {
                        ushort hp;
                        if (ushort.TryParse(v[1].Replace("_U", ""), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture.NumberFormat, out hp))
                        {
                            hidPage = (HidUsagePage)(hp);
                            if ((int)hidPage > 0xFF)
                            {
                                hidPage = HidUsagePage.Reserved;
                                if (v.Length > 2)
                                {
                                    if (ushort.TryParse(v[1].Replace("_U", ""), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture.NumberFormat, out hp))
                                    {
                                        hidPage = (HidUsagePage)(hp);
                                        if ((int)hidPage > 0xFF)
                                            hidPage = HidUsagePage.Reserved;
                                    }
                                }

                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Retrieve dynamic values live from the device.
        /// </summary>
        /// <returns></returns>
        public Dictionary<HidUsageInfo, List<HidUsageInfo>>? RefreshDynamicValues()
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
        /// <returns><see cref="Dictionary{HidUsageInfo, List{HidUsageInfo}}"/> of live values organized by usage collection.</returns>
        /// <remarks>
        /// The <paramref name="collectionType"/> and <paramref name="usageType"/> properties can be OR'd to retrieve more than one kind of value set.
        /// </remarks>
        public Dictionary<HidUsageInfo, List<HidUsageInfo>>? GetFeatureValues(HidUsageType collectionType, HidUsageType usageType, bool includeUnlinked = false)
        {
            var result = new Dictionary<HidUsageInfo, List<HidUsageInfo>>();
            if (UsageCollections == null) return null;

            foreach (var kvp in UsageCollections)
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

                                if (!result.TryGetValue(kvp.Key, out List<HidUsageInfo>? col))
                                {
                                    col = new List<HidUsageInfo>();
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

                                                if (!result.TryGetValue(kvp.Key, out List<HidUsageInfo>? col))
                                                {
                                                    col = new List<HidUsageInfo>();
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

                                if (b && item.ValueCaps is HidPValueCaps vc2)
                                {
                                    if (item.UsageId == 0x5a && vc2.UsagePage == HidUsagePage.PowerDevice1)
                                    {
                                        item.Value = (AudibleAlarmControlState)res;
                                    }
                                    else if (item.UsageId == 0x58 && vc2.UsagePage == HidUsagePage.PowerDevice1)
                                    {
                                        item.Value = (HidPowerTestState)res;
                                    }
                                    else
                                    {
                                        item.Value = res;
                                    }

                                    if (!result.TryGetValue(kvp.Key, out List<HidUsageInfo>? col))
                                    {
                                        col = new List<HidUsageInfo>();
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
        /// <param name="retrieveValue">True to call the device and populate the <see cref="HidUsageInfo.Value"/> property with a real-time value.</param>
        /// <returns>A HID Usage Page or null.</returns>
        /// <remarks>
        /// Several reported usages can have identical <paramref name="usageId"/>'s. Only the first found is returned if <paramref name="collectionId"/> is not specified.
        /// </remarks>
        public HidUsageInfo? LookupValue(byte usageId, byte collectionId = 0, bool retrieveValue = false)
        {
            if (UsageCollections == null) return null;

            //var features = deviceInfo.GetFeatureValues(HidUsageType.CL | HidUsageType.CA | HidUsageType.CP, HidUsageType.DV | HidUsageType.SV);

            foreach (var kvp in UsageCollections)
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
        public Dictionary<HidUsageInfo, List<HidUsageInfo>> GetCollection(Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> data, HidReportType reportType, Dictionary<HidUsageInfo, List<HidUsageInfo>>? currDict = null)
        {
            var result = currDict ?? new Dictionary<HidUsageInfo, List<HidUsageInfo>>();

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
                else if (page == HidUsagePage.PowerDevice2)
                {
                    bitem = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }
                else if (page == HidUsagePage.KeyboardKeypad)
                {
                    bitem = HidKeyboardDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }
                else
                {
                    bitem = HidUsagePageInfo.CreatePage(page).Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }

                if (bitem == null) continue;

                List<HidUsageInfo> l;

                l = result.Where((scan) => scan.Key.UsageId == collectionid && scan.Key.ReportType == reportType).Select((scan2) => scan2.Value).FirstOrDefault() ?? new List<HidUsageInfo>();
                var newres = l.Count == 0;

                foreach (var item in list)
                {
                    HidUsageInfo? bitem2;

                    if (item.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        bitem2 = HidPowerDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else if (item.UsagePage == HidUsagePage.PowerDevice2)
                    {
                        bitem2 = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else if (item.UsagePage == HidUsagePage.KeyboardKeypad)
                    {
                        bitem2 = HidKeyboardDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else
                    {
                        bitem2 = HidUsagePageInfo.CreatePage(item.UsagePage).Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }

                    if (bitem2 == null) continue;

                    if (l.Count((scan3) => scan3.UsageId == bitem2.UsageId) == 0)
                    {
                        var newItem = bitem2.Clone(reportType);

                        newItem.ReportID = item.ReportID;
                        newItem.ValueCaps = item;

                        newItem.IsButton = false;

                        l.Add(newItem);

                    }

                }

                if (newres) result.Add(bitem.Clone(reportType), l);
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
        public Dictionary<HidUsageInfo, List<HidUsageInfo>> GetCollection(Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> data, HidReportType reportType, Dictionary<HidUsageInfo, List<HidUsageInfo>>? currDict = null)
        {
            var result = currDict ?? new Dictionary<HidUsageInfo, List<HidUsageInfo>>();

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
                else if (page == HidUsagePage.PowerDevice2)
                {
                    bitem = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }
                else if (page == HidUsagePage.KeyboardKeypad)
                {
                    bitem = HidKeyboardDevicePageInfo.Instance.Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }
                else
                {
                    bitem = HidUsagePageInfo.CreatePage(page).Where((x) => x.UsageId == collectionid && x.UsageName != "Reserved").FirstOrDefault();
                }


                if (bitem == null) continue;

                List<HidUsageInfo> l;

                l = result.Where((scan) => scan.Key.UsageId == collectionid && scan.Key.ReportType == reportType).Select((scan2) => scan2.Value).FirstOrDefault() ?? new List<HidUsageInfo>();
                var newres = l.Count == 0;

                foreach (var item in list)
                {
                    HidUsageInfo? bitem2;

                    if (item.UsagePage == HidUsagePage.PowerDevice1)
                    {
                        bitem2 = HidPowerDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else if (item.UsagePage == HidUsagePage.PowerDevice2)
                    {
                        bitem2 = HidBatteryDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else if (item.UsagePage == HidUsagePage.KeyboardKeypad)
                    {
                        bitem2 = HidKeyboardDevicePageInfo.Instance.Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }
                    else
                    {
                        bitem2 = HidUsagePageInfo.CreatePage(item.UsagePage).Where((x) => x.UsageId == item.Usage).FirstOrDefault();
                    }

                    if (bitem2 == null) continue;

                    if (l.Count((scan3) => scan3.UsageId == bitem2.UsageId) == 0)
                    {
                        var newItem = bitem2.Clone(reportType);

                        newItem.ReportID = item.ReportID;
                        newItem.ButtonCaps = item;
                        newItem.IsButton = true;
                        l.Add(newItem);

                    }

                }

                if (newres) result.Add(bitem.Clone(reportType, true), l);
            }

            return result;
        }


        public override string UIDescription
        {
            get
            {
                if (string.IsNullOrEmpty(ProductString) == false)
                {
                    return ProductString;
                }
                else if (string.IsNullOrEmpty(Description) == false)
                {
                    return Description;
                }
                else if (string.IsNullOrEmpty(FriendlyName) == false)
                {
                    return FriendlyName;
                }
                else
                {
                    return ToString();
                }
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(FriendlyName))
            {
                return Description + " (" + HidUsageDescription + ")";
            }
            else
            {
                return FriendlyName + ", " + Description + " (" + HidUsageDescription + ")";
            }
        }
    }

    
}