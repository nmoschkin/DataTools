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

using DataTools.Win32.Memory;

using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Network
{
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct ADAPTER_UNICAST_ADDRESS
    {
        public IP_ADAPTER_HEADER_UNION Header;
        public LPADAPTER_UNICAST_ADDRESS Next;
        public SOCKET_ADDRESS Address;
        public IpPrefixOrigin PrefixOrigin;
        public IpSuffixOrigin SuffixOrigin;
        public uint ValidLifetime;
        public uint PreferredLifetime;
        public uint LeaseLifetime;

        public IPAddress IPAddress
        {
            get
            {
                if (Address.lpSockaddr.Handle == MemPtr.Empty)
                {
                    return null;
                }
                else
                {
                    return Address.lpSockaddr.IPAddress;
                }
            }
        }

        public override string ToString() => Address.ToString();
    }
}