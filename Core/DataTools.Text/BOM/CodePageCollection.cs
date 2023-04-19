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
    public class CodePageCollection : CollectionBase
    {
        public CodePage[] InnerArray
        {
            get
            {
                return (CodePage[])InnerList.ToArray(typeof(CodePage));
            }
        }

        public void AddRange(CodePage[] sel)
        {
            InnerList.AddRange(sel);
        }

        public int Add(CodePage value)
        {
            return List.Add(value);
        }

        public void InsertRange(int index, CodePage[] sel)
        {
            InnerList.InsertRange(index, sel);
        }

        [Browsable(true)]
        public CodePage this[int index]
        {
            get
            {
                return (CodePage)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        [Browsable(true)]
        public CodePage this[string name]
        {
            get
            {
                name = name.ToLower();
                foreach (CodePage cp in List)
                {
                    if (cp.Name.ToLower() == name)
                        return cp;
                }

                return null;
            }
            set
            {
                int c = 0;
                foreach (CodePage cp in List)
                {
                    if (cp.Name.ToLower() == name)
                    {
                        List[c] = value;
                        break;
                    }
                    c += 1;
                }
            }
        }

        public int IndexOf(CodePage value)
        {
            return List.IndexOf(value);
        } // IndexOf(value)

        public void Insert(int index, CodePage value, string key = "")
        {
            List.Insert(index, value);
        } // Insert

        public new void RemoveAt(int index)
        {
            if (List.Count == 0)
                return;
            List.RemoveAt(index);
        }

        public void Remove(CodePage value)
        {
            List.Remove(value);
        } // Remove

        public bool Contains(CodePage value)
        {
            // If value is not of type CodePage, this will return false.
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
            if (!typeof(CodePage).IsAssignableFrom(value.GetType()))
                throw new ArgumentException("value must be of type Object.", "value");
        } // OnValidate

        internal CodePageCollection() : base()
        {
        }
    }
}