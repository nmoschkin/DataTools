// ************************************************* ''
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
using System.ComponentModel;

using DataTools.Text;
using DataTools.Win32.Memory;
using DataTools.Win32.Usb.Keyboard;

namespace DataTools.Win32.Usb
{


    /// <summary>
    /// An object that represents a Human Interface Device USB device on the system.
    /// </summary>
    /// <remarks></remarks>
    public class HidDeviceInfo : DeviceInfo, IDisposable
    {
        protected List<HidUsageCollection>? usageCollections;


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

        protected internal IntPtr hHid = (IntPtr)(-1);
        protected bool openWrite = false;
        private bool disposedValue;

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
        /// <param name="createUsageCollection">True to compile the usage collection after retrieving the device capabilities.</param>
        /// <returns></returns>
        public bool PopulateDeviceCaps(bool createUsageCollection = true)
        {
            bool b;
            if (IsHidOpen)
            {
                b = UsbLibHelpers.PopulateDeviceCaps(this, hHid);
            }
            else
            {
                b = UsbLibHelpers.PopulateDeviceCaps(this);
            }

            if (b && createUsageCollection)
            {
                CreateUsageCollection();
            }

            return b;

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
        /// Retrieves the raw handle to the currently open HID device.
        /// </summary>
        /// <returns></returns>
        public IntPtr DangerousGetHidDeviceHandle()
        {
            return hHid;
        }

        /// <summary>
        /// Gets a value that indicates if the HID device is open.
        /// </summary>
        public bool IsHidOpen
        {
            get
            {
                return !hHid.IsInvalidHandle();
            }
        }

        /// <summary>
        /// Gets a value indicating that hid is opened with write access.
        /// </summary>
        public bool IsOpenWrite
        {
            get
            {
                return IsHidOpen && openWrite;
            }
        }

        /// <summary>
        /// Open or reopen the HID device for multiple reads and writes.
        /// </summary>
        /// <param name="write">True to request write access.</param>
        /// <returns>True if the device was successfully opened.</returns>
        /// <remarks>
        /// Use <see cref="DangerousGetHidDeviceHandle"/> to retrieve the handle value.
        /// </remarks>
        public bool OpenHid(bool write = false)
        {
            if (IsHidOpen && !CloseHid()) return false;

            hHid = HidFeatures.OpenHid(this, write);
            var b = IsHidOpen;

            if (b) this.openWrite = write;
            return b;
        }

        /// <summary>
        /// Close the HID device handle.
        /// </summary>
        /// <returns></returns>
        public bool CloseHid()
        {
            if (!IsHidOpen) return false;

            HidFeatures.CloseHid(hHid);

            hHid = (IntPtr)(-1);
            if (openWrite) openWrite = false;

            return true;
        }

        /// <summary>
        /// Retrieve an indexed string resource from the HID device.
        /// </summary>
        /// <param name="index">The index of the string to retrieve.</param>
        /// <param name="result">The result of the call.</param>
        /// <returns>True if successful.</returns>
        public bool HidGetString(int index, out string? result)
        {
            using (var mm = new SafePtr(256))
            {
                var hhid = IsHidOpen ? this.hHid : HidFeatures.OpenHid(this);
                
                if (hhid != IntPtr.Zero)
                {
                    var b = UsbLibHelpers.HidD_GetIndexedString(hhid, index, mm, (int)mm.Length);
                    if (hhid != hHid) HidFeatures.CloseHid(hhid);

                    if (b)
                    {
                        result = mm.ToString();
                        return true;
                    }
                } 
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Returns the raw byte data for a Hid feature code.
        /// </summary>
        /// <param name="reportId">The Hid report ID to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <param name="expectedSize">The expected size, in bytes, of the result.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte reportId, out byte[] result, int expectedSize)
        {
            bool success = false;

            var hhid = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero)
            {
                result = new byte[0];
                return false;
            }

            using (var mm = new SafePtr())
            {
                mm.Alloc(expectedSize + 1);
                mm.ByteAt(0L) = reportId;

                if (!UsbLibHelpers.HidD_GetFeature(hhid, mm, expectedSize))
                {
                    success = false;
                    result = new byte[0];
                }
                else
                {
                    success = true;
                    result = mm.ToByteArray(1L, expectedSize);
                }

                if (!IsHidOpen) HidFeatures.CloseHid(hhid);
            }

            return success;
        }

        /// <summary>
        /// Returns the short value of a Hid report ID.
        /// </summary>
        /// <param name="reportId">The Hid report ID to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte reportId, out short result)
        {
            result = 0;
            
            bool success;

            var hhid = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(3L);
                mm.ByteAt(0L) = reportId;

                if (!UsbLibHelpers.HidD_GetFeature(hhid, mm, 3))
                {
                    success = false;
                }
                else
                {
                    success = true;
                    result = mm.ShortAtAbsolute(1L);
                }
            }

            if (!IsHidOpen) HidFeatures.CloseHid(hhid);
            return success;
        }

        /// <summary>
        /// Returns the integer value of a Hid report ID.
        /// </summary>
        /// <param name="reportId">The Hid report ID to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte reportId, out int result)
        {
            result = 0;
            
            bool success;

            var hhid = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(5L);
                mm.ByteAt(0L) = reportId;

                if (!UsbLibHelpers.HidD_GetFeature(hhid, mm, 5))
                {
                    success = false;
                }
                else
                {
                    success = true;
                    result = mm.IntAtAbsolute(1L);
                }
            }

            if (!IsHidOpen) HidFeatures.CloseHid(hhid);

            return success;
        }

        /// <summary>
        /// Returns the long value of a Hid report ID.
        /// </summary>
        /// <param name="reportId">The Hid report ID to retrieve.</param>
        /// <param name="result">Receives the result of the operation.</param>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public bool HidGetFeature(byte reportId, out long result)
        {
            result = 0;
            bool success;

            var hhid = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero) return false;

            using(var mm = new SafePtr())
            {
                mm.Alloc(9L);
                mm.ByteAt(0L) = reportId;

                if (!UsbLibHelpers.HidD_GetFeature(hhid, mm, 9))
                {
                    success = false;
                }
                else
                {
                    success = true;
                    result = mm.LongAtAbsolute(1L);
                }
            }

            if (!IsHidOpen) HidFeatures.CloseHid(hhid);

            return success;
        }

        /// <summary>
        /// Sets the raw byte value of a Hid report ID.
        /// </summary>
        /// <param name="reportId">The Hid report ID to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte reportId, byte[] value)
        {
            bool success;

            var hhid = IsOpenWrite ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(value.Length + 1);
                mm.FromByteArray(value, 1L);
                mm.ByteAt(0L) = reportId;

                if (!UsbLibHelpers.HidD_SetFeature(hhid, mm, (int)mm.Length))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }
            }
            
            if (!IsOpenWrite) HidFeatures.CloseHid(hhid);
            return success;
        }

        /// <summary>
        /// Sets the short value of a Hid report ID.
        /// </summary>
        /// <param name="reportId">The Hid report ID to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte reportId, short value)
        {
            bool success;

            var hhid = IsOpenWrite ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(3L);
                mm.ByteAt(0L) = reportId;
                mm.ShortAtAbsolute(1L) = value;
                if (!UsbLibHelpers.HidD_SetFeature(hhid, mm, 3))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }

                if (!IsOpenWrite) HidFeatures.CloseHid(hhid);
            }

            return success;
        }

        /// <summary>
        /// Sets the integer value of a Hid report ID.
        /// </summary>
        /// <param name="reportId">The Hid report ID to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte reportId, int value)
        {
            bool success;

            var hhid = IsOpenWrite ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero) return false;

            using (var mm = new SafePtr())
            {
                mm.Alloc(5L);
                mm.ByteAt(0L) = reportId;
                mm.IntAtAbsolute(1L) = value;

                if (!UsbLibHelpers.HidD_SetFeature(hhid, mm, 5))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }

                if (!IsOpenWrite) HidFeatures.CloseHid(hhid);

            }

            return success;
        }

        /// <summary>
        /// Sets the long value of a Hid report ID.
        /// </summary>
        /// <param name="reportId">The Hid report ID to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HidSetFeature(byte reportId, long value)
        {
            bool success;

            var hhid = IsOpenWrite ? hHid : HidFeatures.OpenHid(this);

            if (hhid == IntPtr.Zero) return false;
            
            using (var mm = new SafePtr())
            {
                mm.Alloc(9L);
                mm.ByteAt(0L) = reportId;
                mm.LongAtAbsolute(1L) = value;
                if (!UsbLibHelpers.HidD_SetFeature(hhid, mm, 9))
                {
                    success = false;
                }
                else
                {
                    success = true;
                }

                if (!IsOpenWrite) HidFeatures.CloseHid(hhid);
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
                        var dev = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

                        mm.AllocZero(128L * sizeof(char));

                        UsbLibHelpers.HidD_GetManufacturerString(dev, mm, (int)mm.Length);

                        hidManufacturer = mm.GetString(0L);

                        if (!IsHidOpen) HidFeatures.CloseHid(dev);
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
                        var dev = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

                        mm.AllocZero(128L * sizeof(char));

                        UsbLibHelpers.HidD_GetSerialNumberString(dev, mm, (int)mm.Length);

                        serialNumber = (string)mm;

                        if (!IsHidOpen) HidFeatures.CloseHid(dev);
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
                        var dev = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

                        mm.AllocZero(128L * sizeof(char));
                        var b = UsbLibHelpers.HidD_GetProductString(dev, mm, (int)mm.Length);

                        productString = (string)mm;

                        if (!IsHidOpen) HidFeatures.CloseHid(dev);
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
                        var dev = IsHidOpen ? hHid : HidFeatures.OpenHid(this);

                        mm.AllocZero(512L);

                        UsbLibHelpers.HidD_GetPhysicalDescriptor(dev, mm, (int)mm.Length);

                        physicalDescriptor = (string)mm;

                        if (!IsHidOpen) HidFeatures.CloseHid(dev);
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
        public List<HidUsageCollection>? UsageCollections
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
        public List<HidUsageCollection>? RefreshDynamicValues()
        {
            var retCols = GetFeatureValues(HidUsageType.CP | HidUsageType.CL | HidUsageType.CA, HidUsageType.DV | HidUsageType.DF);

            if (retCols != null)
            {
                var leftovers = GetFeatureValues(HidUsageType.Reserved, HidUsageType.DV);
                if (leftovers != null)
                {
                    foreach (var loitem in leftovers)
                    {
                        retCols.Add(loitem);
                    }
                }
            }

            return retCols;
        }

        /// <summary>
        /// Reads the current value for a usage report.
        /// </summary>
        /// <param name="item">The <see cref="HidUsageInfo"/> object that contains information about the report to retrieve.</param>
        /// <param name="populateItemValue">True to set the <see cref="HidUsageInfo.Value"/> property to the result of the operation.</param>
        /// <returns>The result of the operator or null.</returns>
        protected bool ReadUsageValue(HidUsageInfo item, bool populateItemValue, out HidFeatureValue? res)
        {
            int olen;
            bool b;
            res = null;

            if (hidCaps == null)
            {
                PopulateDeviceCaps();
            }

            if (hidCaps is HidCaps && item.ValueCaps is HidPValueCaps vc)
            {
                //b = HidGetFeature(item.ReportID, out byte[] resb, olen);

                res = UsbLibHelpers.GetScaledValue(this, item.ReportType, vc);

                if (res != null)
                {
                    if (populateItemValue) item.Value = res;

                    return true;
                }

            }

            return false;
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
        public virtual List<HidUsageCollection>? GetFeatureValues(HidUsageType collectionType, HidUsageType usageType, bool includeUnlinked = false)
        {
            var result = new List<HidUsageCollection>();
            
            if (UsageCollections == null) return null;
            
            bool ch = false;

            if (!IsHidOpen)
            {
                if (!OpenHid()) return null;
                ch = true;
            }

            foreach (var enumCol in UsageCollections)
            {

                HidUsageCollection usageCol = enumCol;

                if ((collectionType & usageCol.UsageType) != 0 || (usageCol.UsageType == HidUsageType.Reserved && (collectionType == HidUsageType.Reserved || includeUnlinked)))
                {
                    foreach (var item in usageCol)
                    {
                        if ((usageType & item.UsageType) != 0 || (item.IsButton && item.UsageType == 0))
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

                                if (result.Where((t) => t.UsageId == usageCol.UsageId && t.ReportType == usageCol.ReportType).FirstOrDefault() is HidUsageCollection uc)
                                {
                                    usageCol = uc;
                                }
                                else
                                {
                                    usageCol = usageCol.Clone(usageCol.ReportType);
                                    result.Add(usageCol);
                                }

                                usageCol.Add(item);
                            }
                            else if (item.ValueCaps is HidPValueCaps vc && vc.StringIndex != 0)
                            {
                                if (HidGetString(vc.StringIndex, out string? strres))
                                {
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

                                        if (result.Where((t) => t.UsageId == usageCol.UsageId && t.ReportType == usageCol.ReportType).FirstOrDefault() is HidUsageCollection uc)
                                        {
                                            usageCol = uc;
                                        }
                                        else
                                        {
                                            usageCol = usageCol.Clone(usageCol.ReportType);
                                            result.Add(usageCol);
                                        }

                                        usageCol.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                if (ReadUsageValue(item, false, out HidFeatureValue? res) && item.ValueCaps is HidPValueCaps vc2 && res != null)
                                {
                                    if (item.UsageId == 0x5a && vc2.UsagePage == HidUsagePage.PowerDevice1)
                                    {
                                        item.Value = (AudibleAlarmControlState)(int)res;
                                    }
                                    else if (item.UsageId == 0x58 && vc2.UsagePage == HidUsagePage.PowerDevice1)
                                    {
                                        item.Value = (HidPowerTestState)(int)res;
                                    }
                                    else if (item.UsageId == 0x2c && vc2.UsagePage == HidUsagePage.PowerDevice2)
                                    {
                                        item.Value = (PowerCapacityMode)(int)res;
                                    }
                                    else if (item.UsageId == 0x8b && vc2.UsagePage == HidUsagePage.PowerDevice2)
                                    {
                                        item.IsButton = true;
                                        item.ButtonValue = ((int)res) == 1;
                                    }
                                    else
                                    {
                                        item.Value = (int)res;
                                    }

                                    if (result.Where((t) => t.UsageId == usageCol.UsageId && t.ReportType == usageCol.ReportType).FirstOrDefault() is HidUsageCollection uc)
                                    {
                                        usageCol = uc;
                                    }
                                    else
                                    {
                                        usageCol = usageCol.Clone(usageCol.ReportType);
                                        result.Add(usageCol);
                                    }

                                    usageCol.Add(item);

                                }

                            }

                        }
                    }

                }
            }

            if (ch) CloseHid();

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
        public virtual HidUsageInfo? LookupValue(byte usageId, byte collectionId = 0, bool retrieveValue = false)
        {
            if (UsageCollections == null) return null;

            //var features = deviceInfo.GetFeatureValues(HidUsageType.CL | HidUsageType.CA | HidUsageType.CP, HidUsageType.DV | HidUsageType.SV);

            foreach (var usageCol in UsageCollections)
            {
                if (collectionId == 0 || usageCol.UsageId == collectionId)
                {
                    foreach (var item in usageCol)
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
                                else if (item.ValueCaps is HidPValueCaps vc && vc.StringIndex != 0)
                                {
                                    if (HidGetString(vc.StringIndex, out string? strres))
                                    {
                                        newitem.Value = strres ?? string.Empty;
                                    }
                                }
                                else
                                {
                                    if (ReadUsageValue(item, false, out HidFeatureValue? res) && res != null)
                                    {
                                        newitem.Value = (int)res;
                                    }
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
        /// Create a power device collection from the pre-populated list of collections and usages.
        /// </summary>
        /// <param name="data">The linked usage data.</param>
        /// <param name="currCol">The collection to add to.</param>
        /// <returns>
        /// Either <paramref name="currCol"/> or a new collection.
        /// </returns>
        public List<HidUsageCollection> GetCollection(Dictionary<(HidUsagePage, int), IList<HidPValueCaps>> data, HidReportType reportType, List<HidUsageCollection>? currCol = null)
        {
            var result = currCol ?? new List<HidUsageCollection>();

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

                IList<HidUsageInfo> l;

                l = result.Where((scan) => scan.UsageId == collectionid && scan.ReportType == reportType).FirstOrDefault() ?? (IList<HidUsageInfo>)new List<HidUsageInfo>();
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

                if (newres) result.Add(new HidUsageCollection(bitem, reportType, l));
            }

            return result;
        }



        /// <summary>
        /// Create a power device collection from the pre-populated list of collections and usages.
        /// </summary>
        /// <param name="data">The linked usage data.</param>
        /// <param name="currCol">The collection to add to.</param>
        /// <returns>
        /// Either <paramref name="currCol"/> or a new collection.
        /// </returns>
        public List<HidUsageCollection> GetCollection(Dictionary<(HidUsagePage, int), IList<HidPButtonCaps>> data, HidReportType reportType, List<HidUsageCollection>? currCol = null)
        {
            var result = currCol ?? new List<HidUsageCollection>();

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

                IList<HidUsageInfo> l;

                l = result.Where((scan) => scan.UsageId == collectionid && scan.ReportType == reportType).FirstOrDefault() ?? (IList<HidUsageInfo>)new List<HidUsageInfo>();
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

                if (newres) result.Add(new HidUsageCollection(bitem, reportType, l));
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

        #region Dispose Pattern
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (IsHidOpen) CloseHid();
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~HidDeviceInfo()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }


}