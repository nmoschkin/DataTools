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
using System.Text;

namespace DataTools.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct BLUETOOTH_ADDRESS
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] rgBytes;

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (rgBytes is null || rgBytes.Length < 5) return "";

            for (int i = 5; i >= 0; i -= 1)
            {
                if (sb.Length > 0) sb.Append(':');

                sb.Append(rgBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static explicit operator BLUETOOTH_ADDRESS(ulong val1)
        {
            BLUETOOTH_ADDRESS bt;
            bt.rgBytes = BitConverter.GetBytes(val1);
            return bt;
        }

        public static explicit operator ulong(BLUETOOTH_ADDRESS val1)
        {
            return BitConverter.ToUInt64(val1.rgBytes, 0);
        }
    }
}