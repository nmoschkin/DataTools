using System;

// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: IfDefApi
//         The almighty network interface native API.
//         Some enum documentation comes from the MSDN.
//
// (and an exercise in creative problem solving and data-structure marshaling.)
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using System.ComponentModel;

namespace DataTools.Win32.Network
{
    // From Microsoft:

    //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    //                                                                          ''
    // Media types                                                              ''
    //                                                                          ''
    // These are enumerated values of the ifType object defined in MIB-II's     ''
    // ifTable.  They are registered with IANA which publishes this list        ''
    // periodically, in either the Assigned Numbers RFC, or some derivative     ''
    // of it specific to Internet Network Management number assignments.        ''
    // See ftp:''ftp.isi.edu/mib/ianaiftype.mib                                 ''
    //                                                                          ''
    //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    public enum IFTYPE
    {
        /// <summary>
        /// Minimum IF_TYPE integer value present in this enumeration.
        /// </summary>
        [Description("Minimum IF_TYPE integer value present in this enumeration.")]
        MIN_IF_TYPE = 1,

        /// <summary>
        /// None of the below
        /// </summary>
        [Description("None of the below")]
        OTHER = 1,

        REGULAR_1822 = 2,
        HDH_1822 = 3,
        DDN_X25 = 4,
        RFC877_X25 = 5,

        /// <summary>
        /// Ethernet adapter
        /// </summary>
        [Description("Ethernet adapter")]
        ETHERNET_CSMACD = 6,

        IS088023_CSMACD = 7,
        ISO88024_TOKENBUS = 8,
        ISO88025_TOKENRING = 9,
        ISO88026_MAN = 10,
        STARLAN = 11,
        PROTEON_10MBIT = 12,
        PROTEON_80MBIT = 13,
        HYPERCHANNEL = 14,
        FDDI = 15,
        LAP_B = 16,
        SDLC = 17,

        /// <summary>
        /// DS1-MIB
        /// </summary>
        [Description("DS1-MIB")]
        DS1 = 18,

        /// <summary>
        /// Obsolete; see DS1-MIB
        /// </summary>
        [Description("Obsolete; see DS1-MIB")]
        E1 = 19,

        BASIC_ISDN = 20,
        PRIMARY_ISDN = 21,

        /// <summary>
        /// proprietary serial
        /// </summary>
        [Description("proprietary serial")]
        PROP_POINT2POINT_SERIAL = 22,

        PPP = 23,
        SOFTWARE_LOOPBACK = 24,

        /// <summary>
        /// CLNP over IP
        /// </summary>
        [Description("CLNP over IP")]
        EON = 25,

        ETHERNET_3MBIT = 26,

        /// <summary>
        /// XNS over IP
        /// </summary>
        [Description("XNS over IP")]
        NSIP = 27,

        /// <summary>
        /// Generic Slip
        /// </summary>
        [Description("Generic Slip")]
        SLIP = 28,

        /// <summary>
        /// ULTRA Technologies
        /// </summary>
        [Description("ULTRA Technologies")]
        ULTRA = 29,

        /// <summary>
        /// DS3-MIB
        /// </summary>
        [Description("DS3-MIB")]
        DS3 = 30,

        /// <summary>
        /// SMDS, coffee
        /// </summary>
        [Description("SMDS, coffee")]
        SIP = 31,

        /// <summary>
        /// DTE only
        /// </summary>
        [Description("DTE only")]
        FRAMERELAY = 32,

        RS232 = 33,

        /// <summary>
        /// Parallel port
        /// </summary>
        [Description("Parallel port")]
        PARA = 34,

        ARCNET = 35,
        ARCNET_PLUS = 36,

        /// <summary>
        /// ATM cells
        /// </summary>
        [Description("ATM cells")]
        ATM = 37,

        MIO_X25 = 38,

        /// <summary>
        /// SONET or SDH
        /// </summary>
        [Description("SONET or SDH")]
        SONET = 39,

        X25_PLE = 40,
        ISO88022_LLC = 41,
        LOCALTALK = 42,
        SMDS_DXI = 43,

        /// <summary>
        /// FRNETSERV-MIB
        /// </summary>
        [Description("FRNETSERV-MIB")]
        FRAMERELAY_SERVICE = 44,

        V35 = 45,
        HSSI = 46,
        HIPPI = 47,

        /// <summary>
        /// Generic Modem
        /// </summary>
        [Description("Generic Modem")]
        MODEM = 48,

        /// <summary>
        /// AAL5 over ATM
        /// </summary>
        [Description("AAL5 over ATM")]
        AAL5 = 49,

        SONET_PATH = 50,
        SONET_VT = 51,

        /// <summary>
        /// SMDS InterCarrier Interface
        /// </summary>
        [Description("SMDS InterCarrier Interface")]
        SMDS_ICIP = 52,

        /// <summary>
        /// Proprietary virtual/internal
        /// </summary>
        [Description("Proprietary virtual/internal")]
        PROP_VIRTUAL = 53,

        /// <summary>
        /// Proprietary multiplexing
        /// </summary>
        [Description("Proprietary multiplexing")]
        PROP_MULTIPLEXOR = 54,

        /// <summary>
        /// 100BaseVG
        /// </summary>
        [Description("100BaseVG")]
        IEEE80212 = 55,

        FIBRECHANNEL = 56,
        HIPPIINTERFACE = 57,

        /// <summary>
        /// Obsolete, use 32 or 44
        /// </summary>
        [Description("Obsolete, use 32 or 44")]
        FRAMERELAY_INTERCONNECT = 58,

        /// <summary>
        /// ATM Emulated LAN for 802.3
        /// </summary>
        [Description("ATM Emulated LAN for 802.3")]
        AFLANE_8023 = 59,

        /// <summary>
        /// ATM Emulated LAN for 802.5
        /// </summary>
        [Description("ATM Emulated LAN for 802.5")]
        AFLANE_8025 = 60,

        /// <summary>
        /// ATM Emulated circuit
        /// </summary>
        [Description("ATM Emulated circuit")]
        CCTEMUL = 61,

        /// <summary>
        /// Fast Ethernet (100BaseT)
        /// </summary>
        [Description("Fast Ethernet (100BaseT)")]
        FASTETHER = 62,

        /// <summary>
        /// ISDN and X.25
        /// </summary>
        [Description("ISDN and X.25")]
        ISDN = 63,

        /// <summary>
        /// CCITT V.11/X.21
        /// </summary>
        [Description("CCITT V.11/X.21")]
        V11 = 64,

        /// <summary>
        /// CCITT V.36
        /// </summary>
        [Description("CCITT V.36")]
        V36 = 65,

        /// <summary>
        /// CCITT G703 at 64Kbps
        /// </summary>
        [Description("CCITT G703 at 64Kbps")]
        G703_64K = 66,

        /// <summary>
        /// Obsolete; see DS1-MIB
        /// </summary>
        [Description("Obsolete; see DS1-MIB")]
        G703_2MB = 67,

        /// <summary>
        /// SNA QLLC
        /// </summary>
        [Description("SNA QLLC")]
        QLLC = 68,

        /// <summary>
        /// Fast Ethernet (100BaseFX)
        /// </summary>
        [Description("Fast Ethernet (100BaseFX)")]
        FASTETHER_FX = 69,

        CHANNEL = 70,

        /// <summary>
        /// Radio spread spectrum - WiFi
        /// </summary>
        [Description("Radio spread spectrum - WiFi")]
        IEEE80211 = 71,

        /// <summary>
        /// IBM System 360/370 OEMI Channel
        /// </summary>
        [Description("IBM System 360/370 OEMI Channel")]
        IBM370PARCHAN = 72,

        /// <summary>
        /// IBM Enterprise Systems Connection
        /// </summary>
        [Description("IBM Enterprise Systems Connection")]
        ESCON = 73,

        /// <summary>
        /// Data Link Switching
        /// </summary>
        [Description("Data Link Switching")]
        DLSW = 74,

        /// <summary>
        /// ISDN S/T interface
        /// </summary>
        [Description("ISDN S/T interface")]
        ISDN_S = 75,

        /// <summary>
        /// ISDN U interface
        /// </summary>
        [Description("ISDN U interface")]
        ISDN_U = 76,

        /// <summary>
        /// Link Access Protocol D
        /// </summary>
        [Description("Link Access Protocol D")]
        LAP_D = 77,

        /// <summary>
        /// IP Switching Objects
        /// </summary>
        [Description("IP Switching Objects")]
        IPSWITCH = 78,

        /// <summary>
        /// Remote Source Route Bridging
        /// </summary>
        [Description("Remote Source Route Bridging")]
        RSRB = 79,

        /// <summary>
        /// ATM Logical Port
        /// </summary>
        [Description("ATM Logical Port")]
        ATM_LOGICAL = 80,

        /// <summary>
        /// Digital Signal Level 0
        /// </summary>
        [Description("Digital Signal Level 0")]
        DS0 = 81,

        /// <summary>
        /// Group of ds0s on the same ds1
        /// </summary>
        [Description("Group of ds0s on the same ds1")]
        DS0_BUNDLE = 82,

        /// <summary>
        /// Bisynchronous Protocol
        /// </summary>
        [Description("Bisynchronous Protocol")]
        BSC = 83,

        /// <summary>
        /// Asynchronous Protocol
        /// </summary>
        [Description("Asynchronous Protocol")]
        ASYNC = 84,

        /// <summary>
        /// Combat Net Radio
        /// </summary>
        [Description("Combat Net Radio")]
        CNR = 85,

        /// <summary>
        /// ISO 802.5r DTR
        /// </summary>
        [Description("ISO 802.5r DTR")]
        ISO88025R_DTR = 86,

        /// <summary>
        /// Ext Pos Loc Report Sys
        /// </summary>
        [Description("Ext Pos Loc Report Sys")]
        EPLRS = 87,

        /// <summary>
        /// Appletalk Remote Access Protocol
        /// </summary>
        [Description("Appletalk Remote Access Protocol")]
        ARAP = 88,

        /// <summary>
        /// Proprietary Connectionless Proto
        /// </summary>
        [Description("Proprietary Connectionless Proto")]
        PROP_CNLS = 89,

        /// <summary>
        /// CCITT-ITU X.29 PAD Protocol
        /// </summary>
        [Description("CCITT-ITU X.29 PAD Protocol")]
        HOSTPAD = 90,

        /// <summary>
        /// CCITT-ITU X.3 PAD Facility
        /// </summary>
        [Description("CCITT-ITU X.3 PAD Facility")]
        TERMPAD = 91,

        /// <summary>
        /// Multiproto Interconnect over FR
        /// </summary>
        [Description("Multiproto Interconnect over FR")]
        FRAMERELAY_MPI = 92,

        /// <summary>
        /// CCITT-ITU X213
        /// </summary>
        [Description("CCITT-ITU X213")]
        X213 = 93,

        /// <summary>
        /// Asymmetric Digital Subscrbr Loop
        /// </summary>
        [Description("Asymmetric Digital Subscrbr Loop")]
        ADSL = 94,

        /// <summary>
        /// Rate-Adapt Digital Subscrbr Loop
        /// </summary>
        [Description("Rate-Adapt Digital Subscrbr Loop")]
        RADSL = 95,

        /// <summary>
        /// Symmetric Digital Subscriber Loop
        /// </summary>
        [Description("Symmetric Digital Subscriber Loop")]
        SDSL = 96,

        /// <summary>
        /// Very H-Speed Digital Subscrb Loop
        /// </summary>
        [Description("Very H-Speed Digital Subscrb Loop")]
        VDSL = 97,

        /// <summary>
        /// ISO 802.5 CRFP
        /// </summary>
        [Description("ISO 802.5 CRFP")]
        ISO88025_CRFPRINT = 98,

        /// <summary>
        /// Myricom Myrinet
        /// </summary>
        [Description("Myricom Myrinet")]
        MYRInet = 99,

        /// <summary>
        /// Voice recEive and transMit
        /// </summary>
        [Description("Voice recEive and transMit")]
        VOICE_EM = 100,

        /// <summary>
        /// Voice Foreign Exchange Office
        /// </summary>
        [Description("Voice Foreign Exchange Office")]
        VOICE_FXO = 101,

        /// <summary>
        /// Voice Foreign Exchange Station
        /// </summary>
        [Description("Voice Foreign Exchange Station")]
        VOICE_FXS = 102,

        /// <summary>
        /// Voice encapsulation
        /// </summary>
        [Description("Voice encapsulation")]
        VOICE_ENCAP = 103,

        /// <summary>
        /// Voice over IP encapsulation
        /// </summary>
        [Description("Voice over IP encapsulation")]
        VOICE_OVERIP = 104,

        /// <summary>
        /// ATM DXI
        /// </summary>
        [Description("ATM DXI")]
        ATM_DXI = 105,

        /// <summary>
        /// ATM FUNI
        /// </summary>
        [Description("ATM FUNI")]
        ATM_FUNI = 106,

        /// <summary>
        /// ATM IMA
        /// </summary>
        [Description("ATM IMA")]
        ATM_IMA = 107,

        /// <summary>
        /// PPP Multilink Bundle
        /// </summary>
        [Description("PPP Multilink Bundle")]
        PPPMULTILINKBUNDLE = 108,

        /// <summary>
        /// IBM ipOverCdlc
        /// </summary>
        [Description("IBM ipOverCdlc")]
        IPOVER_CDLC = 109,

        /// <summary>
        /// IBM Common Link Access to Workstn
        /// </summary>
        [Description("IBM Common Link Access to Workstn")]
        IPOVER_CLAW = 110,

        /// <summary>
        /// IBM stackToStack
        /// </summary>
        [Description("IBM stackToStack")]
        STACKTOSTACK = 111,

        /// <summary>
        /// IBM VIPA
        /// </summary>
        [Description("IBM VIPA")]
        VIRTUALIPADDRESS = 112,

        /// <summary>
        /// IBM multi-proto channel support
        /// </summary>
        [Description("IBM multi-proto channel support")]
        MPC = 113,

        /// <summary>
        /// IBM ipOverAtm
        /// </summary>
        [Description("IBM ipOverAtm")]
        IPOVER_ATM = 114,

        /// <summary>
        /// ISO 802.5j Fiber Token Ring
        /// </summary>
        [Description("ISO 802.5j Fiber Token Ring")]
        ISO88025_FIBER = 115,

        /// <summary>
        /// IBM twinaxial data link control
        /// </summary>
        [Description("IBM twinaxial data link control")]
        TDLC = 116,

        GIGABITETHERNET = 117,
        HDLC = 118,
        LAP_F = 119,
        V37 = 120,

        /// <summary>
        /// Multi-Link Protocol
        /// </summary>
        [Description("Multi-Link Protocol")]
        X25_MLP = 121,

        /// <summary>
        /// X.25 Hunt Group
        /// </summary>
        [Description("X.25 Hunt Group")]
        X25_HUNTGROUP = 122,

        TRANSPHDLC = 123,

        /// <summary>
        /// Interleave channel
        /// </summary>
        [Description("Interleave channel")]
        INTERLEAVE = 124,

        /// <summary>
        /// Fast channel
        /// </summary>
        [Description("Fast channel")]
        FAST = 125,

        /// <summary>
        /// IP (for APPN HPR in IP networks)
        /// </summary>
        [Description("IP (for APPN HPR in IP networks)")]
        IP = 126,

        /// <summary>
        /// CATV Mac Layer
        /// </summary>
        [Description("CATV Mac Layer")]
        DOCSCABLE_MACLAYER = 127,

        /// <summary>
        /// CATV Downstream interface
        /// </summary>
        [Description("CATV Downstream interface")]
        DOCSCABLE_DOWNSTREAM = 128,

        /// <summary>
        /// CATV Upstream interface
        /// </summary>
        [Description("CATV Upstream interface")]
        DOCSCABLE_UPSTREAM = 129,

        /// <summary>
        /// Avalon Parallel Processor
        /// </summary>
        [Description("Avalon Parallel Processor")]
        A12MPPSWITCH = 130,

        /// <summary>
        /// Encapsulation interface
        /// </summary>
        [Description("Encapsulation interface")]
        TUNNEL = 131,

        /// <summary>
        /// Coffee pot
        /// </summary>
        [Description("Coffee pot")]
        COFFEE = 132,

        /// <summary>
        /// Circuit Emulation Service
        /// </summary>
        [Description("Circuit Emulation Service")]
        CES = 133,

        /// <summary>
        /// ATM Sub Interface
        /// </summary>
        [Description("ATM Sub Interface")]
        ATM_SUBINTERFACE = 134,

        /// <summary>
        /// Layer 2 Virtual LAN using 802.1Q
        /// </summary>
        [Description("Layer 2 Virtual LAN using 802.1Q")]
        L2_VLAN = 135,

        /// <summary>
        /// Layer 3 Virtual LAN using IP
        /// </summary>
        [Description("Layer 3 Virtual LAN using IP")]
        L3_IPVLAN = 136,

        /// <summary>
        /// Layer 3 Virtual LAN using IPX
        /// </summary>
        [Description("Layer 3 Virtual LAN using IPX")]
        L3_IPXVLAN = 137,

        /// <summary>
        /// IP over Power Lines
        /// </summary>
        [Description("IP over Power Lines")]
        DIGITALPOWERLINE = 138,

        /// <summary>
        /// Multimedia Mail over IP
        /// </summary>
        [Description("Multimedia Mail over IP")]
        MEDIAMAILOVERIP = 139,

        /// <summary>
        /// Dynamic syncronous Transfer Mode
        /// </summary>
        [Description("Dynamic syncronous Transfer Mode")]
        DTM = 140,

        /// <summary>
        /// Data Communications Network
        /// </summary>
        [Description("Data Communications Network")]
        DCN = 141,

        /// <summary>
        /// IP Forwarding Interface
        /// </summary>
        [Description("IP Forwarding Interface")]
        IPFORWARD = 142,

        /// <summary>
        /// Multi-rate Symmetric DSL
        /// </summary>
        [Description("Multi-rate Symmetric DSL")]
        MSDSL = 143,

        /// <summary>
        /// IEEE1394 High Perf Serial Bus
        /// </summary>
        [Description("IEEE1394 High Perf Serial Bus")]
        IEEE1394 = 144,

        IF_GSN = 145,
        DVBRCC_MACLAYER = 146,
        DVBRCC_DOWNSTREAM = 147,
        DVBRCC_UPSTREAM = 148,
        ATM_VIRTUAL = 149,
        MPLS_TUNNEL = 150,
        SRP = 151,
        VOICEOVERATM = 152,
        VOICEOVERFRAMERELAY = 153,
        IDSL = 154,
        COMPOSITELINK = 155,
        SS7_SIGLINK = 156,
        PROP_WIRELESS_P2P = 157,
        FR_FORWARD = 158,
        RFC1483 = 159,
        USB = 160,
        IEEE8023AD_LAG = 161,
        BGP_POLICY_ACCOUNTING = 162,
        FRF16_MFR_BUNDLE = 163,
        H323_GATEKEEPER = 164,
        H323_PROXY = 165,
        MPLS = 166,
        MF_SIGLINK = 167,
        HDSL2 = 168,
        SHDSL = 169,
        DS1_FDL = 170,
        POS = 171,
        DVB_ASI_IN = 172,
        DVB_ASI_OUT = 173,
        PLC = 174,
        NFAS = 175,
        TR008 = 176,
        GR303_RDT = 177,
        GR303_IDT = 178,
        ISUP = 179,
        PROP_DOCS_WIRELESS_MACLAYER = 180,
        PROP_DOCS_WIRELESS_DOWNSTREAM = 181,
        PROP_DOCS_WIRELESS_UPSTREAM = 182,
        HIPERLAN2 = 183,
        PROP_BWA_P2MP = 184,
        SONET_OVERHEAD_CHANNEL = 185,
        DIGITAL_WRAPPER_OVERHEAD_CHANNEL = 186,
        AAL2 = 187,
        RADIO_MAC = 188,
        ATM_RADIO = 189,
        IMT = 190,
        MVL = 191,
        REACH_DSL = 192,
        FR_DLCI_ENDPT = 193,
        ATM_VCI_ENDPT = 194,
        OPTICAL_CHANNEL = 195,
        OPTICAL_TRANSPORT = 196,
        IEEE80216_WMAN = 237,

        /// <summary>
        /// WWAN devices based on GSM technology
        /// </summary>
        [Description("WWAN devices based on GSM technology")]
        WWANPP = 243,

        /// <summary>
        /// WWAN devices based on CDMA technology
        /// </summary>
        [Description("WWAN devices based on CDMA technology")]
        WWANPP2 = 244,

        /// <summary>
        /// Maximum IF_TYPE integer value present in this enumeration.
        /// </summary>
        [Description("Maximum IF_TYPE integer value present in this enumeration.")]
        MAX_IF_TYPE = 244
    }
}