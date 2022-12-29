// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Printers
//         Windows Printer Api
//
//         Enums are documented in part from the API documentation at MSDN.
//         Other knowledge and references obtained through various sources
//         and all is considered public domain/common knowledge.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


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
using DataTools.MathTools.Polar;

namespace DataTools.Hardware.Printers
{
    public class DeviceMode : CriticalFinalizerObject, IDisposable, IEquatable<DeviceMode>, ICloneable
    {
        internal MemPtr _ptr;
        private bool _own = true;

        internal DeviceMode(nint ptr, bool fOwn)
        {
            _ptr = ptr;
            _own = fOwn;
        }

        /// <summary>
        /// Device name
        /// </summary>
        /// <returns></returns>
        public string DeviceName
        {
            get
            {
                return _ptr.GetString(0L, 32).Trim('\0');
            }

            set
            {
                if (value.Length > 32)
                    value = value.Substring(0, 32);
                _ptr.SetString(0L, value);
            }
        }

        /// <summary>
        /// Spec version
        /// </summary>
        /// <returns></returns>
        public ushort SpecVersion
        {
            get
            {
                return _ptr.UShortAtAbsolute(64L);
            }

            set
            {
                _ptr.UShortAtAbsolute(64L) = value;
            }
        }

        /// <summary>
        /// Driver version
        /// </summary>
        /// <returns></returns>
        public ushort DriverVersion
        {
            get
            {
                return _ptr.UShortAtAbsolute(66L);
            }

            set
            {
                _ptr.UShortAtAbsolute(66L) = value;
            }
        }

        /// <summary>
        /// Size
        /// </summary>
        /// <returns></returns>
        public ushort Size
        {
            get
            {
                return _ptr.UShortAtAbsolute(68L);
            }

            set
            {
                _ptr.UShortAtAbsolute(68L) = value;
            }
        }

        /// <summary>
        /// Driver Extra
        /// </summary>
        /// <returns></returns>
        public ushort DriverExtra
        {
            get
            {
                return _ptr.UShortAtAbsolute(70L);
            }

            set
            {
                _ptr.UShortAtAbsolute(70L) = value;
            }
        }

        /// <summary>
        /// Device mode fields
        /// </summary>
        /// <returns></returns>
        public DeviceModeFields Fields
        {
            get
            {
                return (DeviceModeFields)(_ptr.UIntAtAbsolute(72L));
            }

            set
            {
                _ptr.UIntAtAbsolute(72L) = (uint)value;
            }
        }

        // union

        // struct

        public short Orientation
        {
            get
            {
                return _ptr.ShortAtAbsolute(76L);
            }

            set
            {
                _ptr.ShortAtAbsolute(76L) = value;
            }
        }

        public SystemPaperType PaperSize
        {
            get
            {
                return (SystemPaperType)_ptr.ShortAtAbsolute(78L);
            }

            set
            {
                _ptr.ShortAtAbsolute(78L) = (short)value.Code;
            }
        }

        public short PaperSizeCode
        {
            get
            {
                return _ptr.ShortAtAbsolute(78L);
            }

            set
            {
                _ptr.ShortAtAbsolute(78L) = value;
            }
        }

        public short PaperLength
        {
            get
            {
                return _ptr.ShortAtAbsolute(80L);
            }

            set
            {
                _ptr.ShortAtAbsolute(80L) = value;
            }
        }

        public short PaperWidth
        {
            get
            {
                return _ptr.ShortAtAbsolute(82L);
            }

            set
            {
                _ptr.ShortAtAbsolute(82L) = value;
            }
        }

        public short Scale
        {
            get
            {
                return _ptr.ShortAtAbsolute(84L);
            }

            set
            {
                _ptr.ShortAtAbsolute(84L) = value;
            }
        }

        public short Copies
        {
            get
            {
                return _ptr.ShortAtAbsolute(86L);
            }

            set
            {
                _ptr.ShortAtAbsolute(86L) = value;
            }
        }

        public short DefaultSource
        {
            get
            {
                return _ptr.ShortAtAbsolute(88L);
            }

            set
            {
                _ptr.ShortAtAbsolute(88L) = value;
            }
        }

        public short PrintQuality
        {
            get
            {
                return _ptr.ShortAtAbsolute(90L);
            }

            set
            {
                _ptr.ShortAtAbsolute(90L) = value;
            }
        }

        // struct

        public System.Drawing.Point Position
        {
            get
            {
                return new System.Drawing.Point(_ptr.IntAtAbsolute(76L), _ptr.IntAtAbsolute(80L));
            }

            set
            {
                _ptr.IntAtAbsolute(76L) = value.X;
                _ptr.IntAtAbsolute(80L) = value.Y;
            }
        }

        public uint DisplayOrientation
        {
            get
            {
                return _ptr.UIntAtAbsolute(84L);
            }

            set
            {
                _ptr.UIntAtAbsolute(84L) = value;
            }
        }

        public uint DisplayFixedOutput
        {
            get
            {
                return _ptr.UIntAtAbsolute(88L);
            }

            set
            {
                _ptr.UIntAtAbsolute(88L) = value;
            }
        }

        // end union

        public short Color
        {
            get
            {
                return _ptr.ShortAtAbsolute(92L);
            }

            set
            {
                _ptr.ShortAtAbsolute(92L) = value;
            }
        }

        public short Duplex
        {
            get
            {
                return _ptr.ShortAtAbsolute(94L);
            }

            set
            {
                _ptr.ShortAtAbsolute(94L) = value;
            }
        }

        public short YResolution
        {
            get
            {
                return _ptr.ShortAtAbsolute(96L);
            }

            set
            {
                _ptr.ShortAtAbsolute(96L) = value;
            }
        }

        public short TTOption
        {
            get
            {
                return _ptr.ShortAtAbsolute(98L);
            }

            set
            {
                _ptr.ShortAtAbsolute(98L) = value;
            }
        }

        public short Collate
        {
            get
            {
                return _ptr.ShortAtAbsolute(100L);
            }

            set
            {
                _ptr.ShortAtAbsolute(100L) = value;
            }
        }

        public string FormName
        {
            get
            {
                return _ptr.GetString(102L, 32).Trim('\0');
            }

            set
            {
                if (value.Length > 32)
                    value = value.Substring(0, 32);
                _ptr.SetString(102L, value);
            }
        }

        public ushort LogPixels
        {
            get
            {
                return _ptr.UShortAtAbsolute(168L);
            }

            set
            {
                _ptr.UShortAtAbsolute(168L) = value;
            }
        }

        public uint BitsPerPel
        {
            get
            {
                return _ptr.UIntAtAbsolute(170L);
            }

            set
            {
                _ptr.UIntAtAbsolute(170L) = value;
            }
        }

        public uint PelsWidth
        {
            get
            {
                return _ptr.UIntAtAbsolute(174L);
            }

            set
            {
                _ptr.UIntAtAbsolute(174L) = value;
            }
        }

        public uint PelsHeight
        {
            get
            {
                return _ptr.UIntAtAbsolute(178L);
            }

            set
            {
                _ptr.UIntAtAbsolute(178L) = value;
            }
        }

        // union

        public uint DisplayFlags
        {
            get
            {
                return _ptr.UIntAtAbsolute(182L);
            }

            set
            {
                _ptr.UIntAtAbsolute(182L) = value;
            }
        }

        public uint Nup
        {
            get
            {
                return _ptr.UIntAtAbsolute(182L);
            }

            set
            {
                _ptr.UIntAtAbsolute(182L) = value;
            }
        }

        // end union

        public uint DisplayFrequency
        {
            get
            {
                return _ptr.UIntAtAbsolute(186L);
            }

            set
            {
                _ptr.UIntAtAbsolute(186L) = value;
            }
        }

        public uint ICMMethod
        {
            get
            {
                return _ptr.UIntAtAbsolute(190L);
            }

            set
            {
                _ptr.UIntAtAbsolute(190L) = value;
            }
        }

        public uint ICMIntent
        {
            get
            {
                return _ptr.UIntAtAbsolute(194L);
            }

            set
            {
                _ptr.UIntAtAbsolute(194L) = value;
            }
        }

        public uint MediaType
        {
            get
            {
                return _ptr.UIntAtAbsolute(198L);
            }

            set
            {
                _ptr.UIntAtAbsolute(198L) = value;
            }
        }

        public uint DitherType
        {
            get
            {
                return _ptr.UIntAtAbsolute(202L);
            }

            set
            {
                _ptr.UIntAtAbsolute(202L) = value;
            }
        }

        public uint Reserved1
        {
            get
            {
                return _ptr.UIntAtAbsolute(206L);
            }

            set
            {
                _ptr.UIntAtAbsolute(206L) = value;
            }
        }

        public uint Reserved2
        {
            get
            {
                return _ptr.UIntAtAbsolute(210L);
            }

            set
            {
                _ptr.UIntAtAbsolute(210L) = value;
            }
        }

        public uint PanningWidth
        {
            get
            {
                return _ptr.UIntAtAbsolute(214L);
            }

            set
            {
                _ptr.UIntAtAbsolute(214L) = value;
            }
        }

        public uint PanningHeight
        {
            get
            {
                return _ptr.UIntAtAbsolute(218L);
            }

            set
            {
                _ptr.UIntAtAbsolute(218L) = value;
            }
        }

        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                if (_own)
                    _ptr.Free();
            }

            disposedValue = true;
        }

        ~DeviceMode()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Equals(DeviceMode other)
        {
            var pi = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            object o1;
            object o2;
            foreach (var pe in pi)
            {
                o1 = pe.GetValue(this);
                o2 = pe.GetValue(other);
                if (o1.Equals(o2) == false)
                    return false;
            }

            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
