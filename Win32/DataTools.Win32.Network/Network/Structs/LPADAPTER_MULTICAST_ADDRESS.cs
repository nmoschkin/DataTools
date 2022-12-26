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
    public struct LPADAPTER_MULTICAST_ADDRESS
    {
        internal MemPtr Handle;

        public LPADAPTER_MULTICAST_ADDRESS[] AddressChain
        {
            get
            {
                if (Handle == nint.Zero)
                    return null;
                int c = 0;
                var mx = this;
                var ac = default(LPADAPTER_MULTICAST_ADDRESS[]);
                do
                {
                    Array.Resize(ref ac, c + 1);
                    ac[c] = mx;
                    mx = mx.Next;
                    c += 1;
                }
                while (mx.Handle.Handle != nint.Zero);
                return ac;
            }
        }

        public LPADAPTER_MULTICAST_ADDRESS Next
        {
            get
            {
                if (Handle == nint.Zero)
                    return default;
                return Struct.Next;
            }
        }

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
                if (Handle == nint.Zero)
                    return null;
                return Struct.Address.lpSockaddr.IPAddress;
            }
        }

        public ADAPTER_MULTICAST_ADDRESS Struct
        {
            get
            {
                if (Handle == nint.Zero) return default;
                return ToAddress();
            }
        }

        public ADAPTER_MULTICAST_ADDRESS ToAddress()
        {
            if (Handle == nint.Zero) return default;
            return Handle.ToStruct<ADAPTER_MULTICAST_ADDRESS>();
        }

        public void Dispose()
        {
            Handle.Free();
        }

        public AddressFamily AddressFamily
        {
            get
            {
                if (Handle == nint.Zero) return AddressFamily.AfUnspecified;
                return Struct.Address.lpSockaddr.AddressFamily;
            }
        }

        public byte[] Data
        {
            get
            {
                if (Handle == nint.Zero)
                    return null;
                return Struct.Address.lpSockaddr.Data;
            }
        }

        public static implicit operator LPADAPTER_MULTICAST_ADDRESS(nint operand)
        {
            var a = new LPADAPTER_MULTICAST_ADDRESS();
            a.Handle = operand;
            return a;
        }

        public static implicit operator nint(LPADAPTER_MULTICAST_ADDRESS operand)
        {
            return operand.Handle;
        }

        public static implicit operator LPADAPTER_MULTICAST_ADDRESS(MemPtr operand)
        {
            var a = new LPADAPTER_MULTICAST_ADDRESS();
            a.Handle = operand;
            return a;
        }

        public static implicit operator MemPtr(LPADAPTER_MULTICAST_ADDRESS operand)
        {
            return operand.Handle;
        }

        public static implicit operator LPADAPTER_MULTICAST_ADDRESS(ADAPTER_MULTICAST_ADDRESS operand)
        {
            var a = new LPADAPTER_MULTICAST_ADDRESS();
            a.Handle.Alloc(Marshal.SizeOf(operand));
            Marshal.StructureToPtr(operand, a.Handle.Handle, true);
            return a;
        }

        public static implicit operator ADAPTER_MULTICAST_ADDRESS(LPADAPTER_MULTICAST_ADDRESS operand)
        {
            return operand.ToAddress();
        }
    }
}