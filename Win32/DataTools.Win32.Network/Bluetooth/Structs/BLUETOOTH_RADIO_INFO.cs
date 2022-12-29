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
    internal struct BLUETOOTH_RADIO_INFO
    {
        /// <summary>
        /// Size, In bytes, Of this entire data Structure
        /// </summary>
        public ulong dwSize;

        /// <summary>
        /// Address Of the local radio
        /// </summary>
        public BLUETOOTH_ADDRESS address;

        /// <summary>
        /// Name of device
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BluetoothInternals.BLUETOOTH_MAX_NAME_SIZE)]
        public string szName;

        /// <summary>
        /// Class of device for the local radio
        /// </summary>
        public uint ulClassofDevice;

        /// <summary>
        /// lmpSubversion, manufacturer specifc.
        /// </summary>
        public ushort lmpSubVersion;

        /// <summary>
        /// Manufacturer Of the radio, BTH_MFG_Xxx value.  For the most up to date
        /// list, goto the Bluetooth specification website And get the Bluetooth
        /// assigned numbers document.
        /// </summary>
        public BTH_MFG_INFO manufacturer;
    }
}