// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: UsbHid
//         HID-related structures, enums and functions.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Essentials.Helpers;
using DataTools.Text;

using Newtonsoft.Json;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Describes a usage or capability within a HID Usage Page
    /// </summary>
    public class HidUsageInfo : ICloneable, IComparable<HidUsageInfo>
    {
        private WeakReference<HidUsageCollection?>? wfparent = null;

        protected string name = "";

        public virtual HidUsageCollection? Parent
        {
            get
            {
                if (wfparent != null && wfparent.TryGetTarget(out HidUsageCollection? target) && target is HidUsageCollection)
                {
                    return target;
                }
                else
                {
                    wfparent = null;
                }

                return null;
            }
            protected internal set
            {
                if (value == null)
                {
                    wfparent = null;
                }
                else
                {
                    wfparent = new WeakReference<HidUsageCollection?>(value);
                }
            }
        }

        /// <summary>
        /// The Usage ID
        /// </summary>
        [JsonProperty("usageId")]
        public virtual ushort UsageId { get; set; }

        /// <summary>
        /// The Usage Name
        /// </summary>
        [JsonProperty("usageName")]
        public virtual string? UsageName
        {
            get => name;
            set
            {
                name = value ?? "";

                UnitInfoCode puc = UnitInfoCode.None;

                if (name.Contains("Current"))
                {
                    puc = UnitInfoCode.DCCurrent;
                }
                else if (name.Contains("Volt"))
                {
                    puc = UnitInfoCode.DCVoltage;
                }
                else if (name.Contains("Power"))
                {
                    puc = UnitInfoCode.ApparentPower;
                }
                else if (name.Contains("Time"))
                {
                    puc = UnitInfoCode.Time;
                }
                else if (name.Contains("Temperature"))
                {
                    puc = UnitInfoCode.Temperature;
                }
                else if (name.Contains("Freq"))
                {
                    puc = UnitInfoCode.Frequency;
                }
                else if (name.Contains("Capacity"))
                {
                    puc = UnitInfoCode.BatteryCapacity;
                }

                HidUnit = puc;
            }
        }

        /// <summary>
        /// The Usage Type
        /// </summary>
        [JsonProperty("usageType")]
        public virtual HidUsageType UsageType { get; set; }

        /// <summary>
        /// Yes if this report is a button
        /// </summary>
        [JsonIgnore]
        public virtual bool IsButton { get; set; }

        /// <summary>
        /// Usage Type Description
        /// </summary>
        [JsonIgnore]
        public virtual string UsageTypeDescription
        {
            get => TextTools.PrintEnumDesc(UsageType);
        }

        /// <summary>
        /// Supports Input
        /// </summary>
        [JsonProperty("input")]
        public virtual bool Input { get; set; }

        /// <summary>
        /// Supports Output
        /// </summary>
        [JsonProperty("output")]
        public virtual bool Output { get; set; }

        /// <summary>
        /// Supports Feature
        /// </summary>
        [JsonProperty("feature")]
        public virtual bool Feature { get; set; }

        /// <summary>
        /// HID Standard Publication Version
        /// </summary>
        [JsonProperty("standard")]
        public virtual string? Standard { get; set; }

        /// <summary>
        /// Can Read
        /// </summary>
        [JsonProperty("accessRead")]
        public virtual bool AccessRead { get; set; }

        /// <summary>
        /// Can Write
        /// </summary>
        [JsonProperty("accessWrite")]
        public virtual bool AccessWrite { get; set; }

        /// <summary>
        /// Gets or Sets the discovered ReportID
        /// </summary>
        [JsonProperty("reportID")]
        public virtual byte ReportID { get; set; }

        [JsonProperty("reservedRanges")]
        public virtual List<int[]>? ReservedRanges { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        [JsonIgnore]
        public virtual object? Value { get; set; }

        /// <summary>
        /// Button Value
        /// </summary>
        [JsonIgnore]
        public virtual bool ButtonValue
        {
            get => Value != null && (int)Value is int i && i != 0;
            set => Value = value ? 1 : 0;
        }

        [JsonIgnore]
        public virtual HidPValueCaps? ValueCaps { get; set; }

        [JsonIgnore]
        public virtual HidPButtonCaps? ButtonCaps { get; set; }

        /// <summary>
        /// Report Type
        /// </summary>
        [JsonIgnore]
        public virtual HidReportType ReportType { get; set; }

        /// <summary>
        /// Print the current value.
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public virtual string PrintValue
        {
            get
            {
                if (Value == null)
                {
                    return "Not Set";
                }
                else if (Value is string s)
                {
                    return s;
                }
                else if (Value is HidFeatureValue fv)
                {
                    return fv.Value.ToString();
                }
                else if (IsButton)
                {
                    return ButtonValue.ToString();
                }
                else
                {
                    return Value?.ToString() ?? "";
                }
            }
        }

        public override string ToString()
        {
            var text = UsageName ?? "";

            if (!string.IsNullOrEmpty(text))
            {
                text = TextTools.Separate(text);
            }

            var iof = "";
            var rw = "";
            var ut = UsageType.ToString();

            if (ut == text) ut = "";
            else
            {
                ut = TextTools.PrintEnumDesc(UsageType);
            }

            if (AccessWrite) rw = "Read/Write";
            else if (AccessRead) rw = "Read Only";

            if (Input) iof += "Input";

            if (Output)
            {
                if (iof != "") iof += ", ";
                iof += "Output";
            }

            if (Feature)
            {
                if (iof != "") iof += ", ";
                iof += "Feature";
            }

            if (ut != "") text += " - " + ut;
            if (iof != "") text += " - " + iof;
            if (rw != "") text += " - " + rw;

            return text;
        }

        /// <summary>
        /// Returns the HID Power Unit For this usage.
        /// </summary>
        [JsonProperty("hidUnit")]
        public UnitInfoCode HidUnit { get; set; }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public HidUsageInfo()
        {
        }

        public HidUsageInfo(HidUsageCollection parent)
        {
            Parent = parent;
        }

        public T CloneInto<T>() where T : HidUsageInfo, new()
        {
            T ret = new T();

            ObjectMerge.MergeObjects(this, ret);
            return ret;
        }

        public virtual HidUsageInfo? Clone(HidReportType reportType, bool isButton = false, HidUsageCollection? parent = null)
        {
            var ret = MemberwiseClone();
            HidUsageInfo? rpt = (HidUsageInfo)ret;

            if (rpt != null)
            {
                rpt.IsButton = isButton;
                rpt.ReportType = reportType;
                rpt.Parent = parent;
            }

            return rpt;
        }

        public int CompareTo(HidUsageInfo? other)
        {
            if (other == null) return 1;

            if (other.UsageId > UsageId) return -1;
            if (other.UsageId < UsageId) return 1;

            if (other.ReportID > ReportID) return -1;
            if (other.ReportID < ReportID) return 1;

            return string.Compare(ToString(), other.ToString());
        }
    }
}