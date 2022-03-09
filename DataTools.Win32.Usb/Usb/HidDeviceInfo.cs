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

using DataTools.Text;
using DataTools.Win32.Memory;


namespace DataTools.Win32.Usb
{


    /// <summary>
    /// An object that represents a Human Interface Device USB device on the system.
    /// </summary>
    /// <remarks></remarks>
    public class HidDeviceInfo : DeviceInfo
    {
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
                if (hidManufacturer is null)
                {

                    using (var mm = new SafePtr())
                    {
                        var dev = HidFeatures.OpenHid(this);

                        mm.AllocZero(512L);

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

                        mm.AllocZero(512L);

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
                if (productString is null)
                {
                    using (var mm = new SafePtr())
                    {
                        var dev = HidFeatures.OpenHid(this);

                        mm.AllocZero(512L);

                        UsbLibHelpers.HidD_GetProductString(dev, mm, (int)mm.Length);

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