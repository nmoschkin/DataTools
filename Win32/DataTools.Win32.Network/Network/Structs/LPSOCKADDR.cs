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
    public struct LPSOCKADDR
    {
        public MemPtr Handle;

        public override string ToString()
        {
            if (Handle.Handle == nint.Zero)
                return "NULL";
            return "" + IPAddress.ToString() + " (" + AddressFamily.ToString() + ")";
        }

        public IPAddress IPAddress
        {
            get
            {
                if (Data is null) return null;
                return new IPAddress(Data);
            }
        }

        public SOCKADDR IPAddrV4
        {
            get
            {
                if (AddressFamily == AddressFamily.AfInet6) return default;
                return ToSockAddr();
            }
        }

        public SOCKADDRV6 IPAddrV6
        {
            get
            {
                if (AddressFamily == AddressFamily.AfInet) return default;
                return ToSockAddr6();
            }
        }

        public SOCKADDR ToSockAddr()
        {
            if (Handle == nint.Zero) return default;
            return Handle.ToStruct<SOCKADDR>();
        }

        public SOCKADDRV6 ToSockAddr6()
        {
            if (Handle == nint.Zero) return default;
            return Handle.ToStruct<SOCKADDRV6>();
        }

        public void Dispose()
        {
            Handle.Free();
        }

        public AddressFamily AddressFamily
        {
            get
            {
                if (Handle.Handle == nint.Zero)
                    return AddressFamily.AfUnspecified;
                return ToSockAddr().AddressFamily;
            }
        }

        public byte[] Data
        {
            get
            {
                switch (AddressFamily)
                {
                    case AddressFamily.AfInet6:
                        {
                            return IPAddrV6.Data;
                        }

                    default:
                        {
                            return IPAddrV4.Data;
                        }
                }
            }
        }

        //public static implicit operator LPSOCKADDR(nint operand)
        //{
        //    var a = new LPSOCKADDR();
        //    a.Handle = operand;
        //    return a;
        //}

        //public static implicit operator nint(LPSOCKADDR operand)
        //{
        //    return operand.Handle.Handle;
        //}

        //public static implicit operator LPSOCKADDR(MemPtr operand)
        //{
        //    var a = new LPSOCKADDR();
        //    a.Handle = operand;
        //    return a;
        //}

        //public static implicit operator MemPtr(LPSOCKADDR operand)
        //{
        //    return operand.Handle;
        //}

        //public static implicit operator LPSOCKADDR(SOCKADDR operand)
        //{
        //    var a = new LPSOCKADDR();
        //    a.Handle.Alloc(Marshal.SizeOf(operand));
        //    Marshal.StructureToPtr(operand, a.Handle.Handle, true);
        //    return a;
        //}

        //public static implicit operator SOCKADDR(LPSOCKADDR operand)
        //{
        //    return operand.ToSockAddr();
        //}

        //public static implicit operator LPSOCKADDR(SOCKADDRV6 operand)
        //{
        //    var a = new LPSOCKADDR();
        //    a.Handle.Alloc(Marshal.SizeOf(operand));
        //    Marshal.StructureToPtr(operand, a.Handle.Handle, true);
        //    return a;
        //}

        //public static implicit operator SOCKADDRV6(LPSOCKADDR operand)
        //{
        //    return operand.ToSockAddr6();
        //}
    }
}