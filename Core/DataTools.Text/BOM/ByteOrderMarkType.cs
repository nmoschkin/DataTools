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

namespace DataTools.Text.ByteOrderMark
{
    /// <summary>
    /// The Byte Order Mark (BOM) Type
    /// </summary>
    public enum ByteOrderMarkType : short
    {
        /// <summary>
        /// ASCII
        /// </summary>
        ASCII = 0,

        /// <summary>
        /// Unicode UTF-8 Encoding
        /// </summary>
        UTF8 = 2,
        
        /// <summary>
        /// Unicode UTF-16 Big-Endian Encoding
        /// </summary>
        UTF16BE = 3,

        /// <summary>
        /// Unicode UTF-16 Little-Endian Encoding
        /// </summary>
        UTF16LE = 4,

        /// <summary>
        /// Unicode UTF-32 Big-Endian Encoding
        /// </summary>
        UTF32BE = 5,

        /// <summary>
        /// Unicode UTF-32 Little-Endian Encoding
        /// </summary>
        UTF32LE = 6,

        /// <summary>
        /// Unicode UTF-7a
        /// </summary>
        UTF7a = 7,

        /// <summary>
        /// Unicode UTF-7b
        /// </summary>
        UTF7b = 8,

        /// <summary>
        /// Unicode UTF-7c
        /// </summary>
        UTF7c = 9,

        /// <summary>
        /// Unicode UTF-7d
        /// </summary>
        UTF7d = 10,

        /// <summary>
        /// Unicode UTF-7e
        /// </summary>
        UTF7e = 11,

        /// <summary>
        /// Unicode UTF-1
        /// </summary>
        UTF1 = 12,

        /// <summary>
        /// Unicode UTF-EBCDIC
        /// </summary>
        UTFEBCDIC = 13,

        /// <summary>
        /// SCSU
        /// </summary>
        SCSU = 14,

        /// <summary>
        /// SCSU_BOCU1
        /// </summary>
        SCSU_BOCU1 = 15,

        /// <summary>
        /// SCSU_GB18030
        /// </summary>
        SCSU_GB18030 = 16
    }
}