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
    /// <summary>
    /// Structure that encapsulates the marshaling of a live memory pointer to either a SOCKADDR or a SOCKADDRV6
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct InetSocketPtr : IDisposable
    {
        internal MemPtr Handle;

        public override string ToString()
        {
            if (Handle == MemPtr.Empty) return "NULL";
            return $"{IPAddress} ({AddressFamily})";
        }

        /// <summary>
        /// Gets the resolved <see cref="System.Net.IPAddress"/> for this socket, based on the <see cref="AddressFamily"/>.
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                if (AddressFamily == AddressFamily.AfInet6)
                {
                    return IPAddrV6.Address;
                }
                else
                {
                    return IPAddrV4.Address;
                }
            }
        }

        public Inet4Socket IPAddrV4
        {
            get
            {
                if (AddressFamily == AddressFamily.AfInet6) return default;
                return ToSockAddr();
            }
        }

        public Inet6Socket IPAddrV6
        {
            get
            {
                if (AddressFamily == AddressFamily.AfInet) return default;
                return ToSockAddr6();
            }
        }

        private Inet4Socket ToSockAddr()
        {
            if (Handle == IntPtr.Zero) return default;
            return Handle.ToStruct<Inet4Socket>();
        }

        private Inet6Socket ToSockAddr6()
        {
            if (Handle == IntPtr.Zero) return default;
            return Handle.ToStruct<Inet6Socket>();
        }

        public void Dispose()
        {
            Handle.Free();
        }

        public AddressFamily AddressFamily
        {
            get
            {
                if (Handle == MemPtr.Empty) return AddressFamily.AfUnspecified;
                return ToSockAddr().AddressFamily;
            }
        }
    }
}