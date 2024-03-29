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


using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

using DataTools.Win32;

namespace DataTools.Win32.Network
{
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct IP_ADAPTER_PREFIX
    {
        public IP_ADAPTER_HEADER_UNION Header;
        public LPIP_ADAPTER_PREFIX Next;
        public SOCKET_ADDRESS Address;
        public uint Prefixlength;

        public override string ToString()
        {
            string ToStringRet = default;
            ToStringRet = Address.ToString();
            return ToStringRet;
        }
    }
}
