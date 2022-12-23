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
using DataTools.MathTools.PolarMath;

namespace DataTools.Hardware.Printers
{
    /// <summary>
    /// Encapsulates a system paper type
    /// </summary>
    public class SystemPaperType : IEquatable<SystemPaperType>, IPaperType
    {
        private LinearSize _Size;
        private string _Description;
        private bool _IsTransverse;
        private bool _IsRotated;
        private bool _IsPostcard;
        private PaperNationalities _Nationality = PaperNationalities.Iso;

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Equals(IPaperType other)
        {
            if (other.IsLandscape != (IsTransverse | IsEnvelope))
                return false;
            if (other.IsMetric != IsMetric)
                return false;
            if (other.IsMetric)
            {
                if (other.Size.Equals(SizeMillimeters))
                    return true;
            }
            else if (other.Size.Equals(SizeInches))
                return true;
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Equals(SystemPaperType other)
        {
            return _Size.Equals(other._Size);
        }

        /// <summary>
        /// Returns the (apparent) national or international origin of the paper size.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PaperNationalities Nationality
        {
            get
            {
                return _Nationality;
            }

            internal set
            {
                _Nationality = value;
            }
        }

        /// <summary>
        /// True if this is a postcard layout.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsPostcard
        {
            get
            {
                return _IsPostcard;
            }

            internal set
            {
                _IsPostcard = value;
            }
        }

        /// <summary>
        /// True for transverse/landscape.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsTransverse
        {
            get
            {
                return _IsTransverse;
            }

            internal set
            {
                _IsTransverse = value;
            }
        }

        /// <summary>
        /// Returns true for rotated layout.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsRotated
        {
            get
            {
                return _IsRotated;
            }

            internal set
            {
                _IsRotated = value;
            }
        }

        private bool _IsEnvelope;

        /// <summary>
        /// Returns true if this paper type is an envelope.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsEnvelope
        {
            get
            {
                return _IsEnvelope;
            }

            internal set
            {
                _IsEnvelope = value;
            }
        }

        /// <summary>
        /// Returns true if it is transverse or envelope paper.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsLandscape
        {
            get
            {
                return _IsTransverse | _IsEnvelope;
            }

            internal set
            {
                _IsTransverse = value;
            }
        }

        /// <summary>
        /// Returns a description of the paper.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Description
        {
            get
            {
                return _Description;
            }

            internal set
            {
                _Description = value;
            }
        }

        private string _Name;

        /// <summary>
        /// The name of the paper type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Name
        {
            get
            {
                return _Name;
            }

            internal set
            {
                _Name = value;
            }
        }

        private int _Code;

        /// <summary>
        /// The Windows paper type code.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Code
        {
            get
            {
                return _Code;
            }

            internal set
            {
                _Code = value;
            }
        }

        /// <summary>
        /// The size of the paper, in inches.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public LinearSize SizeInches
        {
            get
            {
                return _Size;
            }

            internal set
            {
                _Size = value;
            }
        }

        /// <summary>
        /// The size of the paper, in millimeters.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public LinearSize SizeMillimeters
        {
            get
            {
                return InchesToMillimeters(_Size);
            }

            internal set
            {
                _Size = MillimetersToInches(value);
            }
        }

        /// <summary>
        /// Returns the IPaperType IsMetric value.  If this value is true, millimeters are used instead of inches to measure the paper.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsMetric
        {
            get
            {
                return _Nationality != PaperNationalities.American;
            }

            set
            {
                if (value)
                {
                    _Nationality = PaperNationalities.Iso;
                }
                else
                {
                    _Nationality = PaperNationalities.American;
                }
            }
        }

        /// <summary>
        /// Returns the IPaperType size, which is different according
        /// to the IPaperType.IsMetric value.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public UniSize Size
        {
            get
            {
                if (_Nationality == PaperNationalities.American)
                {
                    return (SizeInches.Width, SizeInches.Height);
                }
                else
                {
                    return (SizeMillimeters.Width, SizeMillimeters.Height);
                }
            }

            internal set
            {
                if (_Nationality == PaperNationalities.American)
                {
                    SizeInches = new LinearSize(value.Width, value.Height);
                }
                else
                {
                    SizeMillimeters = new LinearSize(value.Width, value.Height);
                }
            }
        }

        /// <summary>
        /// Convert inches to millimeters
        /// </summary>
        /// <param name="size">A <see cref="LinearSize"/> structure</param>
        /// <returns></returns>
        public static LinearSize InchesToMillimeters(LinearSize size)
        {
            return new LinearSize(size.Width * 25.4d, size.Height * 25.4d);
        }

        /// <summary>
        /// Convert millimeters to inches
        /// </summary>
        /// <param name="size">A <see cref="LinearSize"/> structure</param>
        /// <returns></returns>
        public static LinearSize MillimetersToInches(LinearSize size)
        {
            return new LinearSize(size.Width / 25.4d, size.Height / 25.4d);
        }


        /// <summary>
        /// Returns the <see cref="Description"/> property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _Description;
        }

        internal SystemPaperType()
        {
        }


        /// <summary>
        /// Explicit cast to <see cref="String"/>
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static explicit operator string(SystemPaperType operand)
        {
            return operand.Name;
        }

        /// <summary>
        /// Explicit cast to <see cref="SystemPaperType"/>
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static explicit operator SystemPaperType(string operand)
        {
            foreach (var t in SystemPaperTypes.PaperTypes)
            {
                if ((t.Name.ToLower() ?? "") == (operand.ToLower() ?? ""))
                    return t;
            }

            return null;
        }

        /// <summary>
        /// Explicit cast to integer
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>

        public static explicit operator int(SystemPaperType operand)
        {
            return operand.Code;
        }

        /// <summary>
        /// Explicit cast to <see cref="SystemPaperType"/>
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static explicit operator SystemPaperType(int operand)
        {
            foreach (var t in SystemPaperTypes.PaperTypes)
            {
                if (t.Code == operand)
                    return t;
            }

            return null;
        }

        /// <summary>
        /// Explicit cast to unsigned integer
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static explicit operator uint(SystemPaperType operand)
        {
            return (uint)operand.Code;
        }

        /// <summary>
        /// Explicit cast to <see cref="SystemPaperType"/>
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static explicit operator SystemPaperType(uint operand)
        {
            foreach (var t in SystemPaperTypes.PaperTypes)
            {
                if ((long)t.Code == operand)
                    return t;
            }

            return null;
        }
    }
}
