using System;

// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: IfDefApi
//         The almighty network interface native API.
//         Some enum documentation comes from the MSDN.
//
// (and an exercise in creative problem solving and data-structure marshaling.)
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Network
{
    /// <summary>
    /// Represents an IPv6 socket address.
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct Inet6Socket
    {
        /// <summary>
        /// Address family.
        /// </summary>
        /// <remarks></remarks>
        public readonly AddressFamily AddressFamily;

        /// <summary>
        /// Address port.
        /// </summary>
        /// <remarks></remarks>
        public readonly ushort Port;

        /// <summary>
        /// Flow Info
        /// </summary>
        public readonly int FlowInfo;

        /// <summary>
        /// Address data.
        /// </summary>
        /// <remarks></remarks>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        private byte[] Data;

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        /// <remarks></remarks>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private byte[] Zero;

        /// <summary>
        /// Gets the IP address for this structure from the Data buffer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IPAddress Address
        {
            get
            {
                return new IPAddress(Data);
            }
        }

        public override string ToString()
        {
            if (Data is null) return "NULL";
            return "" + new IPAddress(Data).ToString() + " (" + AddressFamily.ToString() + ")";
        }
    }
}