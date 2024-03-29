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

namespace DataTools.Win32.Network
{
    /// <summary>
    /// Interface tunnel type.
    /// </summary>
    /// <remarks></remarks>
    public enum TunnelType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Other
        /// </summary>
        Other = 1,

        /// <summary>
        /// Direct
        /// </summary>
        Direct = 2,

        /// <summary>
        /// Ipv6 to Ipv4 tunnel
        /// </summary>
        IPv6ToIPv4 = 11,

        /// <summary>
        /// ISATAP tunnel
        /// </summary>
        ISATAP = 13,

        /// <summary>
        /// Teredo Ipv6 tunnel
        /// </summary>
        Teredo = 14,

        /// <summary>
        /// IPHTTPS tunnel
        /// </summary>
        IPHTTPS = 15
    }
}