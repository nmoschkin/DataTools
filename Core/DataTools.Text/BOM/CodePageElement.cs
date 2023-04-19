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

using System;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DataTools.Text.ByteOrderMark
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct CodePageElement
    {
        private CPCharTypes _Type;
        private SignalElement _Signal;
        private uint _Code;
        private char _Unicode;
        private char _Ascii;
        private string _Description;

        public CPCharTypes Type
        {
            get
            {
                return _Type;
            }
            internal set
            {
                _Type = value;
            }
        }

        public SignalElement Signal
        {
            get
            {
                return _Signal;
            }
            internal set
            {
                _Signal = value;
            }
        }

        public uint Code
        {
            get
            {
                return _Code;
            }
            internal set
            {
                _Code = value;
            }
        }

        public char Unicode
        {
            get
            {
                return _Unicode;
            }
            internal set
            {
                _Unicode = value;
            }
        }

        public char Ascii
        {
            get
            {
                return _Ascii;
            }
            internal set
            {
                _Ascii = value;
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            internal set
            {
                _Description = value;
            }
        }

        public static CodePageElement Parse(string str)
        {
            CodePageElement cp = new CodePageElement();
            string[] s = TextTools.Split(str, "|");
            string[] sd;

            uint val;

            ControlCodes cc = ControlCodes.NUL;

            cp.Type = (CPCharTypes)Enum.Parse(typeof(CPCharTypes), s[0]);

            if (s[1].Trim() != "")
            {
                if (uint.TryParse(s[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out val))
                {
                    cp.Unicode = (char)val;
                }
            }

            if (s.Length == 3)
            {
                if (uint.TryParse(s[2], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out val))
                {
                    cp.Code = (char)val;
                }
                return cp;
            }

            if (uint.TryParse(s[3], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out val))
            {
                cp.Code = (char)val;
            }

            if (cp.Type == CPCharTypes.Undefined)
                return cp;

            if (s[2].IndexOf(",") > -1)
            {
                sd = TextTools.Split(s[2], ",");
                cp.Description = sd[0];
                cp.Signal = new SignalElement();

                if (Enum.TryParse(sd[1], out cc))
                {
                    cp.Signal = CPGlobal.Signals[cc];
                    cp.Ascii = (char)cp.Signal.Code;
                }
                else if (sd[1].Length > 2 && sd[1].Substring(0, 2) == "&H")
                    cp.Ascii = (char)ushort.Parse(s[1].Substring(2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                else
                    cp.Ascii = sd[1][0];
            }
            else if (Enum.TryParse(s[2], out cc))
            {
                cp.Signal = new SignalElement();

                cp.Signal = CPGlobal.Signals[cc];
                cp.Ascii = (char)cp.Signal.Code;
            }
            else if (s[2].Length > 2 && s[2].Substring(0, 2) == "&H")
                cp.Ascii = (char)ushort.Parse(s[2].Substring(2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
            else
                cp.Ascii = s[2][0];

            return cp;
        }
    }
}