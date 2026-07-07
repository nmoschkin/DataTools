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
        private readonly int err;
        private readonly string msg;

        /// <summary>
        /// Instantiate a new exception with a system error value.
        /// </summary>
        /// <param name="error"></param>
        /// <remarks></remarks>
        public NativeException(int error)
        {
            this.err = error;
            if (error == 0x000000C1)
            {
                // Bad EXE format
                msg = $"p/Invoke Error: Bad EXE file format. File is not a valid Win32 executable. {NativeError.FormatLastError((uint)this.err)}";
            }
            else
            {
                msg = $"p/Invoke Error: {this.err}: {NativeError.FormatLastError((uint)this.err)}";
            }            
        }

        /// <summary>
        /// Instantiate a new exception with the current system error value.
        /// </summary>
        /// <remarks></remarks>
        public NativeException() : this(User32.GetLastError())
        {
        }

        /// <summary>
        /// Instantiate a new exception with the specified system error and additional message.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        public NativeException(int error, string message) : this(error)
        {
            this.msg += $". {message}";
        }

        /// <summary>
        /// Gets the native error code.
        /// </summary>
        public int Code => err;

        /// <summary>
        /// Instantiate a new exception with the current system error and additional message.
        /// </summary>
        /// <param name="message"></param>
        public NativeException(string message) : this()
        {
            this.msg += $". {message}";
        }



        /// <summary>
        /// Returns the error message.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string Message => msg;
    }
}