using System;

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

namespace DataTools.Win32
{
    /// <summary>
    /// Throw an exception based on a native Windows system error.
    /// </summary>
    /// <remarks></remarks>
    public sealed class NativeException : Exception
    {
        private int _Err;

        /// <summary>
        /// Instantiate a new exception with a system error value.
        /// </summary>
        /// <param name="err"></param>
        /// <remarks></remarks>
        public NativeException(int err)
        {
            _Err = err;
        }

        /// <summary>
        /// Instantiate a new exception with the current system error value.
        /// </summary>
        /// <remarks></remarks>
        public NativeException()
        {
            _Err = User32.GetLastError();
        }

        /// <summary>
        /// Returns the error message.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string Message
        {
            get
            {
                return "p/Invoke Error: " + _Err + ": " + NativeError.FormatLastError((uint)_Err);
            }
        }
    }
}