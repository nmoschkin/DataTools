using System;
using System.Linq;

// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: DevProp
//         Native Device Properites.
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
    internal struct DEVPROPKEY : IEquatable<DEVPROPKEY>
    {
        [MarshalAs(UnmanagedType.Struct)]
        public Guid fmtid;

        public uint pid;

        public DEVPROPKEY(uint ui1, ushort s1, short s2, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, uint pid)
        {
            fmtid = new Guid((int)ui1, (short)s1, s2, b1, b2, b3, b4, b5, b6, b7, b8);
            this.pid = pid;
        }

        public override string ToString()
        {
            return DevProp.GetKeyName(this);
        }

        public bool Equals(DEVPROPKEY obj)
        {
            return obj.fmtid == fmtid && obj.pid == pid;
        }

        public static bool operator ==(DEVPROPKEY operand1, DEVPROPKEY operand2)
        {
            return operand1.Equals(operand2);
        }

        public static bool operator !=(DEVPROPKEY operand1, DEVPROPKEY operand2)
        {
            return !operand1.Equals(operand2);
        }

        public override bool Equals(object obj)
        {
            if (obj is DEVPROPKEY d) return Equals(d);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}