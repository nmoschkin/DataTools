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

using System.Collections;

namespace DataTools.Text.ByteOrderMark
{
    public class CPSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            CodePageElement cp1 = (CodePageElement)x;
            CodePageElement cp2 = (CodePageElement)y;

            return (int)cp1.Code - (int)cp2.Code;
        }
    }
}