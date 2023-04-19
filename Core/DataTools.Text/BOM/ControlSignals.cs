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
using System.ComponentModel;

namespace DataTools.Text.ByteOrderMark
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ControlSignals : List<SignalElement>
    {
        public new void Sort()
        {
            Sort(new SignalSorter());
        }

        public void AddSignal(string str)
        {
            SignalElement cc = SignalElement.Parse(str);
            Add(cc);
        }

        public SignalElement[] InnerArray
        {
            get
            {
                return ToArray();
            }
        }

        public void AddRange(SignalElement[] sel)
        {
            base.AddRange(sel);
        }

        public void InsertRange(int index, SignalElement[] sel)
        {
            base.InsertRange(index, sel);
        }

        public SignalElement this[ControlCodes index]
        {
            get
            {
                foreach (SignalElement cc in this)
                {
                    if (cc.Code == index)
                        return cc;
                }
                return default;
            }
            set
            {
                this[index] = value;
            }
        }

        public new SignalElement this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value;
            }
        }

        public new int IndexOf(SignalElement value)
        {
            return base.IndexOf(value);
        } // IndexOf(value)

        public new bool Contains(SignalElement value)
        {
            // If value is not of type SignalElement, this will return false.
            return base.Contains(value);
        } // Contains

        public ControlSignals()
        {
        }
    }
}