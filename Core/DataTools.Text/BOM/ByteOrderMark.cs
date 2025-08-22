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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;

using System.Security.Authentication;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DataTools.Text.ByteOrderMark
{

    /// <summary>
    /// Represents a sequence of characters that instructs the software how to read text in a file or string
    /// </summary>
    public struct ByteOrderMark
    {
        internal ByteOrderMarkType _Type;
        internal byte[] _BOM;

        private static bool mInit;
        private bool myInit;

        /// <summary>
        /// The UTF-8 BOM
        /// </summary>
        public static ByteOrderMark UTF8;
        
        /// <summary>
        /// The UTF-16 Big-Endian BOM
        /// </summary>
        public static ByteOrderMark UTF16BE;

        /// <summary>
        /// The UTF-16 Little-Endian BOM
        /// </summary>
        public static ByteOrderMark UTF16LE;

        /// <summary>
        /// The UTF-32 Big-Endian BOM
        /// </summary>
        public static ByteOrderMark UTF32BE;

        /// <summary>
        /// The UTF-32 Little-Endian BOM
        /// </summary>
        public static ByteOrderMark UTF32LE;
        
        /// <summary>
        /// The UTF-7a BOM
        /// </summary>
        public static ByteOrderMark UTF7a;

        /// <summary>
        /// The UTF-7b BOM
        /// </summary>
        public static ByteOrderMark UTF7b;

        /// <summary>
        /// The UTF-7c BOM
        /// </summary>
        public static ByteOrderMark UTF7c;

        /// <summary>
        /// The UTF-7d BOM
        /// </summary>
        public static ByteOrderMark UTF7d;

        /// <summary>
        /// The UTF-7e BOM
        /// </summary>
        public static ByteOrderMark UTF7e;

        /// <summary>
        /// The UTF-1 BOM
        /// </summary>
        public static ByteOrderMark UTF1;

        /// <summary>
        /// The UTF-EBCDIC BOM
        /// </summary>
        public static ByteOrderMark UTFEBCDIC;

        /// <summary>
        /// The SCSU BOM
        /// </summary>
        public static ByteOrderMark SCSU;

        /// <summary>
        /// The SCSU_BOCU1 BOM
        /// </summary>
        public static ByteOrderMark SCSU_BOCU1;

        /// <summary>
        /// The SCSU_GB18030 BOM
        /// </summary>
        public static ByteOrderMark SCSU_GB18030;

        static ByteOrderMark()
        {
            if (mInit)
                return;
            mInit = true;

            UTF8 = new ByteOrderMark(new byte[] { 0xEF, 0xBB, 0xBF }, "UTF8");
            UTF16BE = new ByteOrderMark(new byte[] { 0xFE, 0xFF }, "UTF16BE");

            // Windows default UTF16LEWindows")
            UTF16LE = new ByteOrderMark(new byte[] { 0xFF, 0xFE }, "UTF16LE");

            UTF32BE = new ByteOrderMark(new byte[] { 0x0, 0x0, 0xFE, 0xFF }, "UTF32BE");
            UTF32LE = new ByteOrderMark(new byte[] { 0xFF, 0xFE, 0x0, 0x0 }, "UTF32LE");
            UTF7a = new ByteOrderMark(new byte[] { 0x2B, 0x2F, 0x76, 0x38 }, "UTF7a");
            UTF7b = new ByteOrderMark(new byte[] { 0x2B, 0x2F, 0x76, 0x39 }, "UTF7b");
            UTF7c = new ByteOrderMark(new byte[] { 0x2B, 0x2F, 0x76, 0x2B }, "UTF7c");
            UTF7d = new ByteOrderMark(new byte[] { 0x2B, 0x2F, 0x76, 0x2F }, "UTF7d");
            UTF7e = new ByteOrderMark(new byte[] { 0x2B, 0x2F, 0x76, 0x38, 0x2D }, "UTF7e");
            UTF1 = new ByteOrderMark(new byte[] { 0xF7, 0x64, 0x4 }, "UTF1");
            UTFEBCDIC = new ByteOrderMark(new byte[] { 0xDD, 0x73, 0x66, 0x73 }, "UTFEBCDIC");
            SCSU = new ByteOrderMark(new byte[] { 0xE, 0xFE, 0xFF }, "SCSU");
            SCSU_BOCU1 = new ByteOrderMark(new byte[] { 0xFB, 0xEE, 0x28 }, "SCSU_BOCU1");
            SCSU_GB18030 = new ByteOrderMark(new byte[] { 0x84, 0x31, 0x95, 0x33 }, "SCSU_GB18030");
        }

        /// <summary>
        /// Create a BOM structure from another BOM structure
        /// </summary>
        /// <param name="bom"></param>
        public ByteOrderMark(ByteOrderMark bom)
        {
            var bl = bom._BOM.Length;
            _BOM = new byte[bl];

            Buffer.BlockCopy(bom._BOM, 0, _BOM, 0, bl);

            _Type = bom._Type;
            myInit = true;
        }

        private ByteOrderMark(byte[] bytes, string typeName)
        {
            _BOM = new byte[bytes.Length];
            bytes.CopyTo(_BOM, 0);
            Enum.TryParse(typeName, out _Type);
            myInit = true;
        }

        private ByteOrderMark(byte[] bytes, ByteOrderMarkType type)
        {
            _BOM = new byte[bytes.Length];
            bytes.CopyTo(_BOM, 0);
            _Type = type;
            myInit = true;
        }

        public ByteOrderMarkType Type
        {
            get
            {
                if (!myInit)
                {
                    _Type = ByteOrderMarkType.UTF16LE;
                    _BOM = UTF16LE;
                    myInit = true;
                }
                return _Type;
            }
            set
            {
                SetBOM(value);
            }
        }

        public byte[] Bytes
        {
            get
            {
                if (!myInit)
                {
                    _Type = ByteOrderMarkType.UTF16LE;
                    _BOM = UTF16LE;
                    myInit = true;
                }
                return _BOM;
            }
        }

        public void SetBOM(ByteOrderMarkType type)
        {
            if (!myInit)
                myInit = true;
            _BOM = null;
            _Type = type;
            if (type == ByteOrderMarkType.ASCII)
                return;
            _BOM = GetBOM(type);
        }

        public static bool ByteEquals(byte[] operand1, ByteOrderMark operand2)
        {
            int i;
            int c = operand2._BOM.Length - 1;

            if (operand1.Length < operand2._BOM.Length)
                return false;

            for (i = 0; i <= c; i++)
            {
                if (operand1[i] != operand2._BOM[i])
                    return false;
            }

            return true;
        }

        public static byte[] StripBOM(byte[] value)
        {
            ByteOrderMark a = new ByteOrderMark();
            return StripBOM(value, ref a);
        }

        public static byte[] StripBOM(byte[] value, ref ByteOrderMark BOM)
        {
            ByteOrderMark b = Parse(value);
            byte[] retVal;

            if (b._BOM != null && b._BOM.Length > 0)
            {
                byte[] by;
                int c = value.Length;

                c -= b._BOM.Length;
                by = new byte[c - 1 + 1];

                Array.Copy(value, b._BOM.Length, by, 0, c);
                retVal = by;
            }
            else
            {
                BOM._Type = ByteOrderMarkType.ASCII;
                return value;
            }

            BOM._BOM = new byte[b._BOM.Length - 1 + 1];

            b._BOM.CopyTo(BOM._BOM, 0);

            BOM._Type = b.Type;

            return retVal;
        }

        public static byte[] GetBOM(string type)
        {
            ByteOrderMark b = new ByteOrderMark();

            b._Type = (ByteOrderMarkType)Enum.Parse(typeof(ByteOrderMarkType), type, false);
            return GetBOM(b._Type);
        }

        public static byte[] GetBOM(ByteOrderMarkType type)
        {
            MemberInfo[] m;
            FieldInfo fi;
            byte[] by = null;
            ByteOrderMark b = new ByteOrderMark();

            m = typeof(ByteOrderMark).GetMembers();

            foreach (var mt in m)
            {
                if (mt.MemberType == MemberTypes.Field)
                {
                    if (mt.Name == type.ToString())
                    {
                        fi = (FieldInfo)mt;
                        b = (ByteOrderMark)fi.GetValue(b);
                        by = b._BOM;

                        break;
                    }
                }
            }

            return by;
        }

        public override string ToString()
        {
            int i;
            int c;

            string s = "";

            c = _BOM.Length - 1;
            for (i = 0; i <= c; i++)
            {
                if (i != 0)
                    s += ", ";
                s += TextTools.PadHex(_BOM[i], 2, "0x", true);
            }

            return Type.ToString() + " [" + s + "]";
        }

        public byte[] Encode(byte[] value)
        {
            byte[] b;

            byte[] d;

            b = new byte[_BOM.Length + 1];
            Array.Copy(_BOM, b, _BOM.Length);

            switch (_Type)
            {
                case ByteOrderMarkType.ASCII:
                    {
                        d = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, value);
                        break;
                    }

                case ByteOrderMarkType.UTF16BE:
                    {
                        d = Encoding.Convert(Encoding.Unicode, Encoding.BigEndianUnicode, value);
                        break;
                    }

                case ByteOrderMarkType.UTF16LE:
                    {
                        d = value;
                        break;
                    }

                default:
                    {
                        d = value;
                        break;
                    }
            }

            b = new byte[_BOM.Length + 1];
            Array.Copy(_BOM, b, _BOM.Length);

            if (value == null || value.Length == 0)
                return b;
            b.Concat(d);

            return b;
        }

        public byte[] Encode(string value)
        {
            return Encode(Encoding.Unicode.GetBytes(value));
        }

        public static ByteOrderMark Parse(byte[] value)
        {
            ByteOrderMark b;
            MemberInfo[] m;
            FieldInfo fi;
            byte[] by;
            int i;
            int c;

            bool y = false;

            b = new ByteOrderMark();
            b.Type = ByteOrderMarkType.ASCII;

            m = typeof(ByteOrderMark).GetMembers();

            foreach (var mt in m)
            {
                if (mt.MemberType == MemberTypes.Field)
                {
                    if (mt.Name != "BOM" & mt.Name != "Type")
                    {
                        fi = (FieldInfo)mt;
                        if (fi.FieldType != typeof(ByteOrderMark))
                            continue;

                        by = ((ByteOrderMark)fi.GetValue(b))._BOM;

                        if (value.Length < by.Length)
                            continue;
                        c = by.Length - 1;

                        y = true;
                        for (i = 0; i <= c; i++)
                        {
                            if (value[i] != by[i])
                            {
                                y = false;
                                break;
                            }
                        }

                        if (y)
                        {
                            if (b._Type == ByteOrderMarkType.ASCII || by.Length > b._BOM.Length)
                            {
                                b._Type = (ByteOrderMarkType)Enum.Parse(typeof(ByteOrderMarkType), fi.Name, false);
                                if (b._BOM != null && b._BOM.Length > 0)
                                    b._BOM = null;
                                b._BOM = new byte[by.Length - 1 + 1];
                                Array.Copy(by, b._BOM, by.Length);
                            }

                            y = false;
                        }
                    }
                }
            }

            return b;
        }

        public static implicit operator string(ByteOrderMark operand)
        {
            return Encoding.ASCII.GetString(operand._BOM);
        }

        public static explicit operator ByteOrderMark(string operand)
        {
            ByteOrderMark b = Parse(Encoding.ASCII.GetBytes(operand));
            return b;
        }

        public static implicit operator byte[](ByteOrderMark operand)
        {
            byte[] b;
            b = new byte[operand._BOM.Length - 1 + 1];
            operand._BOM.CopyTo(b, 0);
            return b;
        }

        public static explicit operator ByteOrderMark(byte[] operand)
        {
            ByteOrderMark b = Parse(operand);
            return b;
        }

        public static implicit operator int[](ByteOrderMark operand)
        {
            int[] b = new int[operand._BOM.Length];
            for (var i = 0; i < operand._BOM.Length; i++)
            {
                b[i] = operand._BOM[i];
            }

            return b;
        }

        public static explicit operator ByteOrderMark(int[] operand)
        {
            byte[] b = new byte[operand.Length];

            for (var i = 0; i < operand.Length; i++)
            {
                b[i] = (byte)operand[i];
            }

            return new ByteOrderMark((ByteOrderMark)b);
        }

        public static bool operator ==(ByteOrderMark operand1, ByteOrderMark operand2)
        {
            return operand1.Equals(operand2);
        }

        public static bool operator !=(ByteOrderMark operand1, ByteOrderMark operand2)
        {
            return !operand1.Equals(operand2);
        }

        public static bool operator ==(ByteOrderMark operand1, byte[] operand2)
        {
            return ByteEquals(operand2, operand1);
        }

        public static bool operator !=(ByteOrderMark operand1, byte[] operand2)
        {
            return !ByteEquals(operand2, operand1);
        }

        public static bool operator ==(byte[] operand1, ByteOrderMark operand2)
        {
            return ByteEquals(operand1, operand2);
        }

        public static bool operator !=(byte[] operand1, ByteOrderMark operand2)
        {
            return !ByteEquals(operand1, operand2);
        }

        public override bool Equals(object obj)
        {
            if (obj is ByteOrderMark b)
            {
                return _BOM.SequenceEqual(b._BOM);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _BOM.GetHashCode();
        }
    }
}