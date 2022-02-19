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

namespace DataTools.Win32
{
    public class HidUsageInfo
    {
        public ushort UsageId { get; set; }
        public string UsageName { get; set; }
        public HidUsageType UsageType { get; set; }
        public bool Input { get; set; }
        public bool Output { get; set; }
        public bool Feature { get; set; }
        public string Standard { get; set; }

        public bool AccessRead { get; set; }

        public bool AccessWrite { get; set; }


        public override string ToString()
        {
            var text = UsageName;

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
    }
}
