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
using System.Collections;

namespace DataTools.Text.ByteOrderMark
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CodePage : CollectionBase
    {
        private string _Name;
        private char[] mCA;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public void Sort()
        {
            InnerList.Sort(new CPSorter());
        }

        public void AddCode(string code)
        {
            CodePageElement cp = CodePageElement.Parse(code);
            Add(cp);
            mCA = null;
        }

        public char[] CodeArray
        {
            get
            {
                if (mCA == null)
                    mCA = GetCodeArray();
                return mCA;
            }
        }

        public string ToUnicodeString(byte[] subject)
        {
            if (mCA == null)
                mCA = GetCodeArray();
            int i;
            int c = subject.Length - 1;

            char[] ch;

            ch = new char[c + 1];

            for (i = 0; i <= c; i++)
                ch[i] = mCA[subject[i]];

            return ch.ToString();
        }

        public byte[] FromUnicodeString(string subject)
        {
            if (mCA == null)
                mCA = GetCodeArray();
            int i;
            int c = subject.Length - 1;

            byte[] ch;

            ch = new byte[c + 1];

            for (i = 0; i <= c; i++)
                ch[i] = (byte)Array.IndexOf(mCA, subject[i]);

            return ch;
        }

        public char[] GetCodeArray()
        {
            Sort();
            CodePageElement objCP;
            char[] codes;
            int i;

            objCP = (CodePageElement)List[List.Count - 1];
            i = (int)objCP.Code;
            codes = new char[i - 1 + 1];

            foreach (CodePageElement objC in List)
                codes[objC.Code] = objC.Unicode;

            return codes;
        }

        public CodePageElement[] InnerArray
        {
            get
            {
                return (CodePageElement[])InnerList.ToArray(typeof(CodePageElement));
            }
        }

        public void AddRange(CodePageElement[] sel)
        {
            InnerList.AddRange(sel);
        }

        public void InsertRange(int index, CodePageElement[] sel)
        {
            InnerList.InsertRange(index, sel);
        }

        public CodePageElement this[int index]
        {
            get
            {
                return (CodePageElement)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public int IndexOf(CodePageElement value)
        {
            return List.IndexOf(value);
        } // IndexOf(value)

        public void Insert(int index, CodePageElement value)
        {
            List.Insert(index, value);
        } // Insert

        public int Add(CodePageElement value)
        {
            return List.Add(value);
        }

        public new void RemoveAt(int index)
        {
            if (List.Count == 0)
                return;
            List.RemoveAt(index);
        }

        public void Remove(CodePageElement value)
        {
            List.Remove(value);
        } // Remove

        public bool Contains(CodePageElement value)
        {
            // If value is not of type CodePageElement, this will return false.
            return List.Contains(value);
        } // Contains

        protected override void OnInsert(int index, object value)
        {
        } // OnInsert

        protected override void OnRemove(int index, object value)
        {
        } // OnRemove

        protected override void OnSet(int index, object oldValue, object newValue)
        {
        } // OnSet

        protected override void OnValidate(object value)
        {
            if (!typeof(CodePageElement).IsAssignableFrom(value.GetType()))
                throw new ArgumentException("value must be of type Object.", "value");
        } // OnValidate

        public CodePage() : base()
        {
        }
    }
}