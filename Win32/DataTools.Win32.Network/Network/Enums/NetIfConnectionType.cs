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
    /// Interface connection type.
    /// </summary>
    /// <remarks></remarks>
    public enum NetIfConnectionType
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Dedicated connection.  This is a typical connection.
        /// </summary>
        /// <remarks></remarks>
        Dedicated = 1,

        /// <summary>
        /// Passive
        /// </summary>
        Passive = 2,

        /// <summary>
        /// On demand
        /// </summary>
        Demand = 3,

        /// <summary>
        /// Maximum
        /// </summary>
        Maximum = 4
    }
}