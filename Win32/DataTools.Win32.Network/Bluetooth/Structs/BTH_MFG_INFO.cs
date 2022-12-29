using System;

// *************************************************
// DataTools C# Native Utility Library For Windows
//
// Module: Bluetooth API
//         Complete Translation of
//         BluetoothAPI.h
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using System.Runtime.InteropServices;

namespace DataTools.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct BTH_MFG_INFO
    {
        private static Dictionary<ushort, string> Manufacturers = new Dictionary<ushort, string>();
        private ushort _val;

        public ushort Value
        {
            get
            {
                return _val;
            }
        }

        public string Name
        {
            get
            {
                return ToString();
            }
        }

        internal BTH_MFG_INFO(string name, ushort value)
        {
            _val = value;
            if (!Manufacturers.ContainsKey(value))
                Manufacturers.Add(value, name);
        }

        public override string ToString()
        {
            if (Manufacturers.ContainsKey(_val))
                return string.Format("{0}: {1}", _val, Manufacturers[_val]);
            else
                return _val.ToString();
        }

        public static implicit operator ushort(BTH_MFG_INFO val1)
        {
            return val1._val;
        }

        public static explicit operator BTH_MFG_INFO(ushort val1)
        {
            BTH_MFG_INFO b;
            b._val = val1;
            return b;
        }
    }
}