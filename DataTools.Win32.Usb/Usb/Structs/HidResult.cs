using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    internal struct HidResult
    {
        private int value;

        // FACILITY_HID_ERROR_CODE defined in ntstatus.h
        public const int FACILITY_HID_ERROR_CODE = 0x11;

        static int HIDP_ERROR_CODES(int SEV, int CODE)
        {
            return ((SEV) << 28) | (FACILITY_HID_ERROR_CODE << 16) | (CODE);
        }

        public HidResult(int value)
        {
            this.value = value;
        }

        public static readonly HidResult HIDP_STATUS_SUCCESS = HIDP_ERROR_CODES(0x0,0);
        public static readonly HidResult HIDP_STATUS_NULL = HIDP_ERROR_CODES(0x8,1);
        public static readonly HidResult HIDP_STATUS_INVALID_PREPARSED_DATA = HIDP_ERROR_CODES(0xC,1);
        public static readonly HidResult HIDP_STATUS_INVALID_REPORT_TYPE = HIDP_ERROR_CODES(0xC,2);
        public static readonly HidResult HIDP_STATUS_INVALID_REPORT_LENGTH = HIDP_ERROR_CODES(0xC,3);
        public static readonly HidResult HIDP_STATUS_USAGE_NOT_FOUND = HIDP_ERROR_CODES(0xC,4);
        public static readonly HidResult HIDP_STATUS_VALUE_OUT_OF_RANGE = HIDP_ERROR_CODES(0xC,5);
        public static readonly HidResult HIDP_STATUS_BAD_LOG_PHY_VALUES = HIDP_ERROR_CODES(0xC,6);
        public static readonly HidResult HIDP_STATUS_BUFFER_TOO_SMALL = HIDP_ERROR_CODES(0xC,7);
        public static readonly HidResult HIDP_STATUS_INTERNAL_ERROR = HIDP_ERROR_CODES(0xC,8);
        public static readonly HidResult HIDP_STATUS_I8042_TRANS_UNKNOWN = HIDP_ERROR_CODES(0xC,9);
        public static readonly HidResult HIDP_STATUS_INCOMPATIBLE_REPORT_ID = HIDP_ERROR_CODES(0xC,0xA);
        public static readonly HidResult HIDP_STATUS_NOT_VALUE_ARRAY = HIDP_ERROR_CODES(0xC,0xB);
        public static readonly HidResult HIDP_STATUS_IS_VALUE_ARRAY = HIDP_ERROR_CODES(0xC,0xC);
        public static readonly HidResult HIDP_STATUS_DATA_INDEX_NOT_FOUND = HIDP_ERROR_CODES(0xC,0xD);
        public static readonly HidResult HIDP_STATUS_DATA_INDEX_OUT_OF_RANGE = HIDP_ERROR_CODES(0xC,0xE);
        public static readonly HidResult HIDP_STATUS_BUTTON_NOT_PRESSED = HIDP_ERROR_CODES(0xC,0xF);
        public static readonly HidResult HIDP_STATUS_REPORT_DOES_NOT_EXIST = HIDP_ERROR_CODES(0xC,0x10);
        public static readonly HidResult HIDP_STATUS_NOT_IMPLEMENTED = HIDP_ERROR_CODES(0xC,0x20);
        public static readonly HidResult HIDP_STATUS_NOT_BUTTON_ARRAY = HIDP_ERROR_CODES(0xC,0x21);

        static Dictionary<int, string> ERROR_NAMES = new Dictionary<int, string>();

        static HidResult()
        {
            var fi = typeof(HidResult).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            foreach (var f in fi)
            {
                if (f.FieldType != typeof(HidResult)) continue;

                HidResult? res = (HidResult?)f.GetValue(null);

                if (res != null)
                {
                    ERROR_NAMES.Add((int)(res.Value), f.Name);
                }
            }

        }

        public static implicit operator int(HidResult value)
        {
            return value.value; 
        }

        public static implicit operator HidResult(int value)
        {
            return new HidResult()
            {
                value = value
            };
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is int i)
            {
                return i == value;
            }
            else if (obj is HidResult hr)
            {
                return hr.value == value;   
            }
            else return false;
        }


        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            
            if (ERROR_NAMES.TryGetValue(value, out string? name))
            {
                return name + " (0x" + value.ToString("x8") + ")"; 
            }
            else
            {
                return "0x" + value.ToString("x8");
            }
           
        }

    }
}
