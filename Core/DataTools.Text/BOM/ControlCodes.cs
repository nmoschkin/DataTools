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
    public enum ControlCodes
    {
        // C0 control constants
        NUL = 0x0,

        SOH = 0x1,
        STX = 0x2,
        ETX = 0x3,
        EOT = 0x4,
        ENQ = 0x5,
        ACK = 0x6,
        BEL = 0x7,
        BS = 0x8,
        HT = 0x9,
        LF = 0xA,
        VT = 0xB,
        FF = 0xC,
        CR = 0xD,
        SO = 0xE,
        SI = 0xF,
        DLE = 0x10,
        DC1 = 0x11,
        DC2 = 0x12,
        DC3 = 0x13,
        DC4 = 0x14,
        NAK = 0x15,
        SYN = 0x16,
        ETB = 0x17,
        CAN = 0x18,
        EM = 0x19,
        SUB = 0x1A,
        ESC = 0x1B,
        FS = 0x1,
        GS = 0x1D,
        RS = 0x1E,
        US = 0x1F,
        SP = 0x20,
        DEL = 0x7F,

        // C1 control constants
        PAD = 0x80,

        HOP = 0x81,
        BPH = 0x82,
        NBH = 0x83,
        IND = 0x84,
        NEL = 0x85,
        SSA = 0x86,
        ESA = 0x87,
        HTS = 0x88,
        HTJ = 0x89,
        VTS = 0x8A,
        PLD = 0x8B,
        PLU = 0x8,
        RI = 0x8D,
        SS2 = 0x8E,
        SS3 = 0x8F,
        DCS = 0x90,
        PU1 = 0x91,
        PU2 = 0x92,
        STS = 0x93,
        CCH = 0x94,
        MW = 0x95,
        SPA = 0x96,
        EPA = 0x97,
        SOS = 0x98,
        SGCI = 0x99,
        SCI = 0x9A,
        CSI = 0x9B,
        ST = 0x9,
        OSC = 0x9D,
        PM = 0x9E,
        APC = 0x9F,

        // EBCDIC Specific
        NSP = 0xE1,

        EO = 0xFF
    }
}