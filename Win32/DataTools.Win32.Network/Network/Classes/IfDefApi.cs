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

using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Network
{
    public static class IfDefApi
    {
        // A lot of creative marshaling is used here.

        public const int MAX_ADAPTER_ADDRESS_LENGTH = 8;
        public const int MAX_DHCPV6_DUID_LENGTH = 130;

        [DllImport("Iphlpapi.dll")]
        private static extern ADAPTER_ENUM_RESULT GetAdaptersAddresses(AfENUM Family, GAA_FLAGS Flags, IntPtr Reserved, LPIP_ADAPTER_ADDRESSES Addresses, ref uint SizePointer);

        /// <summary>
        /// Retrieves a linked, unmanaged structure array of IP_ADAPTER_ADDRESSES, enumerating all network interfaces on the system.
        /// This function is internal to the managed API in this assembly and is not intended to be used independently from it.
        /// The results of this function are abstracted into the managed <see cref="AdaptersCollection" /> class. Use that, instead.
        /// </summary>
        /// <param name="origPtr">Receives the memory pointer for the memory allocated to retrieve the information from the system.</param>
        /// <param name="noRelease">Specifies that the memory will not be released after usage (this is a typical scenario).</param>
        /// <returns>A linked, unmanaged structure array of IP_ADAPTER_ADDRESSES.</returns>
        /// <remarks></remarks>
        internal static IP_ADAPTER_ADDRESSES[] GetAdapters(ref MemPtr origPtr, bool noRelease = true)
        {
            var lpadapt = new LPIP_ADAPTER_ADDRESSES();
            IP_ADAPTER_ADDRESSES adapt;

            uint cblen = 0;

            var res = GetAdaptersAddresses(AfENUM.AfUnspecified, GAA_FLAGS.GAA_FLAG_INCLUDE_GATEWAYS | GAA_FLAGS.GAA_FLAG_INCLUDE_WINS_INFO | GAA_FLAGS.GAA_FLAG_INCLUDE_ALL_COMPARTMENTS | GAA_FLAGS.GAA_FLAG_INCLUDE_ALL_INTERFACES, IntPtr.Zero, lpadapt, ref cblen);

            // we have a buffer overflow?  We need to get more memory.
            if (res == ADAPTER_ENUM_RESULT.ERROR_BUFFER_OVERFLOW)
            {
                lpadapt.Handle.Alloc(cblen, noRelease);
                res = GetAdaptersAddresses(AfENUM.AfUnspecified, GAA_FLAGS.GAA_FLAG_INCLUDE_GATEWAYS | GAA_FLAGS.GAA_FLAG_INCLUDE_WINS_INFO, IntPtr.Zero, lpadapt, ref cblen);
            }

            if (res != ADAPTER_ENUM_RESULT.NO_ERROR)
            {
                throw new NativeException();
            }

            origPtr = lpadapt.Handle;
            var adapters = new List<IP_ADAPTER_ADDRESSES>();

            adapt = lpadapt;

            do
            {
                if (string.IsNullOrEmpty(adapt.Description) || adapt.FirstDnsServerAddress.Handle == MemPtr.Empty)
                {
                    adapt = adapt.Next;

                    if (adapt.Next.Handle == MemPtr.Empty)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                adapters.Add(adapt);
                adapt = adapt.Next;
            }
            while (adapt.NetworkGuid != Guid.Empty);

            // there is currently no reason for this function to free this pointer,
            // but we reserve the right to do so, in the future.
            if (!noRelease)
                origPtr.Free();

            return adapters.ToArray();
        }
    }
}