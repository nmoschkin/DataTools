// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: UsbHid
//         HID-related structures, enums and functions.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using DataTools.Text;

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Describes a usage or capability within a HID Usage Page
    /// </summary>
    public class HidUsageInfo : ICloneable, IComparable<HidUsageInfo>
    {
        /// <summary>
        /// The Usage ID
        /// </summary>
        public virtual ushort UsageId { get; set; }

        /// <summary>
        /// The Usage Name
        /// </summary>
        public virtual string? UsageName { get; set; }
        
        /// <summary>
        /// The Usage Type
        /// </summary>
        public virtual HidUsageType UsageType { get; set; }


        /// <summary>
        /// Yes if this report is a button
        /// </summary>
        public virtual bool IsButton { get; set; }

        /// <summary>
        /// Usage Type Description
        /// </summary>
        public virtual string UsageTypeDescription
        {
            get => TextTools.PrintEnumDesc(UsageType);
        }
        
        /// <summary>
        /// Supports Input
        /// </summary>
        public virtual bool Input { get; set; }
        
        /// <summary>
        /// Supports Output
        /// </summary>
        public virtual bool Output { get; set; }
        
        /// <summary>
        /// Supports Feature
        /// </summary>
        public virtual bool Feature { get; set; }
        
        /// <summary>
        /// HID Standard Publication Version
        /// </summary>
        public virtual string? Standard { get; set; }

        /// <summary>
        /// Can Read
        /// </summary>
        public virtual bool AccessRead { get; set; }

        /// <summary>
        /// Can Write
        /// </summary>
        public virtual bool AccessWrite { get; set; }


        /// <summary>
        /// Gets or Sets the discovered ReportID
        /// </summary>
        public virtual byte ReportID { get; set; }


        /// <summary>
        /// The value
        /// </summary>
        public virtual object? Value { get; set; }

        /// <summary>
        /// Button Value
        /// </summary>
        public virtual bool ButtonValue
        {
            get => Value != null && Value is int i && i != 0;
            set => Value = value ? 1 : 0;
        }

        public virtual HidPValueCaps? ValueCaps { get; set; }

        public virtual HidPButtonCaps? ButtonCaps { get; set; }

        /// <summary>
        /// Print the current value.
        /// </summary>
        /// <returns></returns>
        public virtual string PrintValue()
        {
            if (Value == null)
            {
                return "Not Set";
            }
            else if (Value is string s)
            {
                return s;
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

        /// <summary>
        /// Report Type
        /// </summary>
        public virtual HidReportType ReportType { get; set; }

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

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual object Clone(HidReportType reportType, bool isButton = false)
        {
            var ret = MemberwiseClone();
            HidUsageInfo? rpt = (HidUsageInfo)ret;

            if (rpt != null)
            {
                rpt.IsButton = isButton;
                rpt.ReportType = reportType;
            }

            return ret;
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
