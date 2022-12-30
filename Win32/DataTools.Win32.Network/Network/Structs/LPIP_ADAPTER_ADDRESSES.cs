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
using System.Runtime.InteropServices;

namespace DataTools.Win32.Network
{
    /// <summary>
    /// Creatively marshaled pointerized structure for the IP_ADAPTER_ADDRESSES structure.
    /// </summary>
    /// <remarks></remarks>
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct LPIP_ADAPTER_ADDRESSES
    {
        public MemPtr Handle;

        public override string ToString()
        {
            if (Handle.Handle == IntPtr.Zero) return "NULL";
            return Struct.FriendlyName;
        }

        public LPIP_ADAPTER_ADDRESSES Next => Struct.Next;

        public IP_ADAPTER_ADDRESSES Struct => ToAdapterStruct();

        public IP_ADAPTER_ADDRESSES ToAdapterStruct()
        {
            if (Handle == IntPtr.Zero) return default;
            return Handle.ToStruct<IP_ADAPTER_ADDRESSES>();
        }

        public static implicit operator LPIP_ADAPTER_ADDRESSES(IP_ADAPTER_ADDRESSES operand)
        {
            var a = new LPIP_ADAPTER_ADDRESSES();
            a.Handle.FromStruct(operand);
            return a;
        }

        public static implicit operator IP_ADAPTER_ADDRESSES(LPIP_ADAPTER_ADDRESSES operand) => operand.Struct;
    }
}