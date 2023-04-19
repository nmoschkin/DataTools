// *************************************************
// DataTools C# Native Utility Library For Windows
//
// Module: Byte Order Marker Library
//         For Mulitple Character Encodings
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DataTools.Text.ByteOrderMark
{
    [Description("Contains an escape sequence")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct EscapeSequence
    {
        public CPSignalEscapes Control;
        public char Character;

        public EscapeSequence(CPSignalEscapes ctrl, char ch)
        {
            Control = ctrl;
            Character = ch;
        }

        public override string ToString()
        {
            if (Control.ToString() != "None")
                return Control.ToString() + "+" + Character;
            else
                return Character.ToString();
        }
    }
}