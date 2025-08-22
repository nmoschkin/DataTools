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
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DataTools.Text.ByteOrderMark
{
    [Description("Describes a control signal character.")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct SignalElement
    {
        private ControlCodes _Code;
        private ControlLayers _Layer;
        private EscapeSequence _Sequence;
        private char _Symbol;
        private char _Escape;
        private string _Descrption;
        private string _Acronym;
        private string _Name;

        public ControlCodes Code
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

        public ControlLayers Layer
        {
            get
            {
                return _Layer;
            }
            internal set
            {
                _Layer = value;
            }
        }

        public EscapeSequence Sequence
        {
            get
            {
                return _Sequence;
            }
            internal set
            {
                _Sequence = value;
            }
        }

        public char Symbol
        {
            get
            {
                return _Symbol;
            }
            internal set
            {
                _Symbol = value;
            }
        }

        public char Escape
        {
            get
            {
                return _Escape;
            }
            internal set
            {
                _Escape = value;
            }
        }

        public string Descrption
        {
            get
            {
                return _Descrption;
            }
            internal set
            {
                _Descrption = value;
            }
        }

        public string Acronym
        {
            get
            {
                return _Acronym;
            }
            internal set
            {
                _Acronym = value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            internal set
            {
                _Name = value;
            }
        }

        public override string ToString()
        {
            return Code.ToString();
        }

        public static SignalElement Parse(string str)
        {
            SignalElement cc = new SignalElement();
            string[] s = TextTools.Split(str, "|");
            string[] sd;

            sd = TextTools.Split(str, "\t");

            switch (sd.Length)
            {
                case 8:
                    {
                        // C0 code
                        // Signals.AddSignal("Seq	Dec	Hex	Acro	Symb	Name	C	Description")

                        cc.Layer = ControlLayers.C0;

                        if (sd[0][0] == '^')
                            cc.Sequence = new EscapeSequence(CPSignalEscapes.Ctrl, sd[0][1]);
                        else
                            cc.Sequence = new EscapeSequence(CPSignalEscapes.None, sd[0][0]);

                        cc.Code = (ControlCodes)Enum.Parse(typeof(ControlCodes), sd[3]);
                        cc.Acronym = cc.ToString();
                        if (sd[6] != "")
                            cc.Escape = sd[6][1];
                        cc.Descrption = sd[7].Replace("\n", "\r\n");
                        cc.Name = sd[5];
                        cc.Symbol = sd[4][0];
                        break;
                    }

                case 6:
                    {
                        // C1 code
                        // Signals.AddSignal("Seq	Dec	Hex	Acro	Name	Description")

                        cc.Layer = ControlLayers.C1;
                        cc.Sequence = new EscapeSequence(CPSignalEscapes.Esc, sd[0][0]);
                        cc.Code = (ControlCodes)Enum.Parse(typeof(ControlCodes), sd[3]);
                        cc.Acronym = cc.ToString();
                        cc.Descrption = sd[5].Replace("\n", "\r\n");
                        cc.Name = sd[4];
                        break;
                    }

                case 5:
                    {
                        cc.Layer = ControlLayers.C1;
                        cc.Sequence = new EscapeSequence(CPSignalEscapes.Esc, sd[0][0]);
                        cc.Code = (ControlCodes)Enum.Parse(typeof(ControlCodes), sd[3]);
                        cc.Acronym = cc.ToString();
                        // cc.Descrption = SearchReplace(sd(4), "\n", vbCrLf)
                        cc.Name = sd[4];
                        break;
                    }
            }

            return cc;
        }

        public static implicit operator ControlCodes(SignalElement operand)
        {
            return operand.Code;
        }

        public static explicit operator SignalElement(ControlCodes operand)
        {
            SignalElement cc = new SignalElement();
            cc.Code = operand;
            cc.Acronym = operand.ToString();

            if ((int)cc.Code < 0x80)
                cc.Layer = ControlLayers.C0;
            else
                cc.Layer = ControlLayers.C1;

            return cc;
        }
    }
}