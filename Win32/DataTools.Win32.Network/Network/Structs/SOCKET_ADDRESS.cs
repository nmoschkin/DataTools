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
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct SOCKET_ADDRESS
    {
        public InetSocketPtr lpSockaddr;
        public int iSockaddrLength;

        public override string ToString()
        {
            if (lpSockaddr.Handle.Handle == IntPtr.Zero) return "NULL";
            return lpSockaddr.ToString();
        }
    }

    public class SocketAddress
    {
        protected SOCKET_ADDRESS sockaddr;

        public AddressFamily AddressFamily => sockaddr.lpSockaddr.AddressFamily;

        public uint Port { get; }

        public IPAddress IPAddress { get; }

        internal SocketAddress(SOCKET_ADDRESS sockaddr)
        {
            this.sockaddr = sockaddr;
            if (AddressFamily == AddressFamily.AfInet6)
            {
                var v6 = sockaddr.lpSockaddr.IPAddrV6;
                Port = v6.Port;
                IPAddress = v6.Address;
            }
            else
            {
                var v4 = sockaddr.lpSockaddr.IPAddrV4;
                Port = v4.Port;
                IPAddress = v4.Address;
            }
        }

        public override string ToString()
        {
            return sockaddr.lpSockaddr.ToString();
        }
    }
}