// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Printers
//         Windows Printer Api
//
//         Enums are documented in part from the API documentation at MSDN.
//         Other knowledge and references obtained through various sources
//         and all is considered public domain/common knowledge.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using DataTools.Text;
using DataTools.MathTools;
using DataTools.Win32;
using DataTools.Win32.Memory;
using DataTools.Graphics;
using DataTools.MathTools.PolarMath;

namespace DataTools.Hardware.Printers
{
    
    

    /// <summary>
    /// Information about a job in the print queue
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class JobInfo : SafeHandle
    {
        private MemPtr _ptr;
        private MemPtr _str;

        internal JobInfo(IntPtr ptr) : base(IntPtr.Zero, true)
        {
            _ptr = ptr;
            _str = ptr + 4;
            handle = ptr;
        }

        public override bool IsInvalid
        {
            get
            {
                return _ptr.Handle == IntPtr.Zero;
            }
        }

        protected override bool ReleaseHandle()
        {
            try
            {
                if (_ptr.Handle != IntPtr.Zero)
                    _ptr.Free();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Job Id
        /// </summary>
        /// <returns></returns>
        public uint JobId
        {
            get
            {
                return _ptr.UIntAt(0L);
            }

            set
            {
                _ptr.UIntAt(0L) = value;
            }
        }

        /// <summary>
        /// The name of the printer printing this job
        /// </summary>
        /// <returns></returns>
        public string PrinterName
        {
            get
            {
                return _str.GetStringIndirect(0 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(0 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// The name of the computer that owns this job
        /// </summary>
        /// <returns></returns>
        public string MachineName
        {
            get
            {
                return _str.GetStringIndirect(1 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(1 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// The username of the user printing this job
        /// </summary>
        /// <returns></returns>
        public string UserName
        {
            get
            {
                return _str.GetStringIndirect(2 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(2 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// The name of the document being printed
        /// </summary>
        /// <returns></returns>
        public string Document
        {
            get
            {
                return _str.GetStringIndirect(3 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(3 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Notification name
        /// </summary>
        /// <returns></returns>
        public string NotifyName
        {
            get
            {
                return _str.GetStringIndirect(4 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(4 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Data type
        /// </summary>
        /// <returns></returns>
        public string Datatype
        {
            get
            {
                return _str.GetStringIndirect(5 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(5 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Print processor
        /// </summary>
        /// <returns></returns>
        public string PrintProcessor
        {
            get
            {
                return _str.GetStringIndirect(6 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(6 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Parameters
        /// </summary>
        /// <returns></returns>
        public string Parameters
        {
            get
            {
                return _str.GetStringIndirect(7 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(7 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Driver name
        /// </summary>
        /// <returns></returns>
        public string DriverName
        {
            get
            {
                return _str.GetStringIndirect(8 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(8 * IntPtr.Size, value);
            }
        }

        internal IntPtr DevMode
        {
            get
            {
                return IntPtr.Size == 8 ? (IntPtr)_str.LongAt(9L) : (IntPtr)_str.IntAt(9L);
            }

            set
            {
                if (IntPtr.Size == 4)
                {
                    _str.IntAt(9L) = (int)value;
                }
                else
                {
                    _str.LongAt(9L) = (long)value;
                }
            }
        }

        /// <summary>
        /// Status message
        /// </summary>
        /// <returns></returns>
        public string StatusMessage
        {
            get
            {
                return _str.GetStringIndirect(10 * IntPtr.Size);
            }

            set
            {
                _str.SetStringIndirect(10 * IntPtr.Size, value);
            }
        }

        internal IntPtr SecurityDescriptor
        {
            get
            {
                return IntPtr.Size == 8 ? (IntPtr)_str.LongAt(11L) : (IntPtr)_str.IntAt(11L);
            }

            set
            {
                if (IntPtr.Size == 4)
                {
                    _str.IntAt(11L) = (int)value;
                }
                else
                {
                    _str.LongAt(11L) = (long)value;
                }
            }
        }

        /// <summary>
        /// Status code
        /// </summary>
        /// <returns></returns>
        public uint StatusCode
        {
            get
            {
                int i = 4 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 4 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// Print queue priority
        /// </summary>
        /// <returns></returns>
        public uint Priority
        {
            get
            {
                int i = 8 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 8 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// Print queue position
        /// </summary>
        /// <returns></returns>
        public uint Position
        {
            get
            {
                int i = 12 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 12 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// Job start time
        /// </summary>
        /// <returns></returns>
        public FriendlyUnixTime StartTime
        {
            get
            {
                int i = 16 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 16 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// Job run unti time
        /// </summary>
        /// <returns></returns>
        public FriendlyUnixTime UntilTime
        {
            get
            {
                int i = 20 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 20 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// Total pages in job
        /// </summary>
        /// <returns></returns>
        public uint TotalPages
        {
            get
            {
                int i = 24 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 24 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// The job size
        /// </summary>
        /// <returns></returns>
        public uint Size
        {
            get
            {
                int i = 28 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 28 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// The time the job was submitted
        /// </summary>
        /// <returns></returns>
        public DateTime Submitted
        {
            get
            {
                int i = 32 + 12 * IntPtr.Size;
                return (DateTime)(_ptr.ToStructAt<SYSTEMTIME>(i));
            }

            set
            {
                int i = 32 + 12 * IntPtr.Size;
                _ptr.FromStructAt(i, (SYSTEMTIME)value);
            }
        }

        /// <summary>
        /// Elapsed time (in seconds)
        /// </summary>
        /// <returns></returns>
        public uint Time
        {
            get
            {
                int i = 48 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 48 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }

        /// <summary>
        /// Page finished printing
        /// </summary>
        /// <returns></returns>
        public uint PagePrinted
        {
            get
            {
                int i = 52 + 12 * IntPtr.Size;
                return _ptr.UIntAtAbsolute(i);
            }

            set
            {
                int i = 52 + 12 * IntPtr.Size;
                _ptr.UIntAtAbsolute(i) = value;
            }
        }
    }
}
