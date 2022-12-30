// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NativeError
//         GetLastError and related.
//
// Started in 2000 on Windows 98/ME (and then later 2000).
//
// Still kicking in 2014 on Windows 8.1!
// A whole bunch of pInvoke/Const/Declare/Struct and associated utility functions that have been collected over the years.

// Some enum documentation copied from the MSDN (and in some cases, updated).
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Win32.Memory;

using System;

namespace DataTools.Win32
{
    public sealed class NativeError
    {
        /// <summary>
        /// Returns the current last native error.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int Error
        {
            get
            {
                return User32.GetLastError();
            }
        }

        /// <summary>
        /// returns the current last native error formatted message.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Message
        {
            get
            {
                return NativeError.FormatLastError();
            }
        }

        /// <summary>
        /// Format a given system error, or the last system error by default.
        /// </summary>
        /// <param name="syserror">Format code to pass. GetLastError is used as by default.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string FormatLastError(uint syserror = 0U)
        {
            uint err = syserror == 0L ? (uint)User32.GetLastError() : syserror;
            var mm = new SafePtr();
            string s;
            mm.Length = 1026L;
            mm.ZeroMemory();
            User32.FormatMessage(0x1000, IntPtr.Zero, err, 0U, mm.handle, 512U, IntPtr.Zero);
            s = "Error &H" + err.ToString("X8") + ": " + mm.ToString();
            mm.Dispose();
            return s;
        }
    }
}