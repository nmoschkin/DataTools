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
    public struct LPIP_ADAPTER_PREFIX
    {
        public MemPtr Handle;

        public LPIP_ADAPTER_PREFIX[] AddressChain
        {
            get
            {
                if (Handle == IntPtr.Zero)
                    return null;
                int c = 0;
                var mx = this;
                var ac = default(LPIP_ADAPTER_PREFIX[]);
                do
                {
                    Array.Resize(ref ac, c + 1);
                    ac[c] = mx;
                    mx = mx.Next;
                    c += 1;
                }
                while (mx.Handle.Handle != IntPtr.Zero);
                return ac;
            }
        }

        public LPIP_ADAPTER_PREFIX Next
        {
            get
            {
                if (Handle == IntPtr.Zero)
                    return default;
                return Struct.Next;
            }
        }

        public override string ToString()
        {
            if (Handle.Handle == IntPtr.Zero)
                return "NULL";
            return "" + IPAddress.ToString() + " (" + AddressFamily.ToString() + ")";
        }

        public IPAddress IPAddress
        {
            get
            {
                if (Handle == IntPtr.Zero) return null;
                return Struct.Address.lpSockaddr.IPAddress;
            }
        }

        public IP_ADAPTER_PREFIX Struct
        {
            get
            {
                if (Handle == IntPtr.Zero) return default;
                return ToAddress();
            }
        }

        public IP_ADAPTER_PREFIX ToAddress()
        {
            if (Handle == IntPtr.Zero) return default;
            return Handle.ToStruct<IP_ADAPTER_PREFIX>();
        }

        public void Dispose()
        {
            Handle.Free();
        }

        public AddressFamily AddressFamily
        {
            get
            {
                if (Handle == IntPtr.Zero) return default;
                return Struct.Address.lpSockaddr.AddressFamily;
            }
        }

        public static implicit operator LPIP_ADAPTER_PREFIX(IntPtr operand)
        {
            return new LPIP_ADAPTER_PREFIX
            {
                Handle = operand
            };
        }

        public static implicit operator IntPtr(LPIP_ADAPTER_PREFIX operand)
        {
            return operand.Handle;
        }

        public static implicit operator LPIP_ADAPTER_PREFIX(MemPtr operand)
        {
            return new LPIP_ADAPTER_PREFIX
            {
                Handle = operand
            };
        }

        public static implicit operator MemPtr(LPIP_ADAPTER_PREFIX operand)
        {
            return operand.Handle;
        }

        public static implicit operator LPIP_ADAPTER_PREFIX(IP_ADAPTER_PREFIX operand)
        {
            var a = new LPIP_ADAPTER_PREFIX();
            a.Handle.FromStruct(operand);
            return a;
        }

        public static implicit operator IP_ADAPTER_PREFIX(LPIP_ADAPTER_PREFIX operand)
        {
            return operand.ToAddress();
        }
    }
}