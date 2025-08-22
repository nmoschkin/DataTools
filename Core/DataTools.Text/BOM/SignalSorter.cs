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

using System.Collections.Generic;

namespace DataTools.Text.ByteOrderMark
{
    public class SignalSorter : IComparer<SignalElement>
    {
        public int Compare(SignalElement x, SignalElement y)
        {
            SignalElement s1 = x;
            SignalElement s2 = y;

            return s1.Code - s2.Code;
        }
    }
}