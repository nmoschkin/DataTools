
/************************************************** 
 * DataTools C# Utility Library (Universal)
 *
 * Module: PolarMath
 *         Structures for containing and converting 
 *         to and from polar coordinates.
 *
 * Copyright (C) 2011-2022 Nathan Moschkin
 * All Rights Reserved
 * 
 * Licensed Under the MIT License   
 **************************************************/


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace DataTools.MathTools.PolarMath
{

    /// <summary>
    /// Flags to be used for formatting the display of a <see cref="PolarCoordinates"/> structure utilizing the <see cref="PolarCoordinates.ToString(PolarCoordinatesFormattingFlags, int)"/> function.
    /// </summary>
    public enum PolarCoordinatesFormattingFlags
    {
        
        /// <summary>
        /// Basic formatting
        /// </summary>
        Default = 0x0,

        /// <summary>
        /// Use the degrees symbol
        /// </summary>
        UseDegreeSymbol = 0x1,

        /// <summary>
        /// Use the polar symbol
        /// </summary>
        UsePolarSymbol = 0x2,

        /// <summary>
        /// Use the pi symbol
        /// </summary>
        UsePiSymbol = 0x4,

        /// <summary>
        /// Use the radians indicator
        /// </summary>
        UseRadianIndicator = 0x8,

        /// <summary>
        /// Use parentheses
        /// </summary>
        UseParenthesis = 0x10,

        /// <summary>
        /// Use brackets
        /// </summary>
        UseBrackets = 0x20,

        /// <summary>
        /// Display the value in radians
        /// </summary>
        DisplayInRadians = 0x40,

        /// <summary>
        /// Display the value in standard notation
        /// </summary>
        Standard = 0x1,

        /// <summary>
        /// Display the value in formatted radians
        /// </summary>
        Radians = 0x4 | 0x8 | 0x40,

        /// <summary>
        /// Display the value in standard scientific notation
        /// </summary>
        StandardScientific = 0x2,

        /// <summary>
        /// Display the value in scientific radians notation
        /// </summary>
        RadiansScientific = 0x2 | 0x4 | 0x8 | 0x40
    }

    /// <summary>
    /// A structure that contains linear (X, Y) coordinates on a 2D cartesean plane.
    /// </summary>
    public struct LinearCoordinates
    {
        /// <summary>
        /// Represents the X (horizontal) coordinate
        /// </summary>
        public double X;

        /// <summary>
        /// Represents the Y (vertical) coordinate
        /// </summary>
        public double Y;

        /// <summary>
        /// Create a new instance of the <see cref="LinearCoordinates"/> structure with the specified X and Y coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public LinearCoordinates(double x, double y)
        {
            X = x;
            Y = y;
        }
    }


    /// <summary>
    /// A structure that contains linear (Width, Height) sizes on a 2D cartesean plane.
    /// </summary>
    public struct LinearSize
    {
        /// <summary>
        /// Represents the horizontal extent
        /// </summary>
        public double Width;

        /// <summary>
        /// Represents the vertical extent
        /// </summary>
        public double Height;

        /// <summary>
        /// Create a new instance of the <see cref="LinearSize"/> structure with the specified width and height.
        /// </summary>
        /// <param name="width">The width, or horizontal extent value.</param>
        /// <param name="height">The height, or vertical extent value.</param>
        public LinearSize(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }
    }

    /// <summary>
    /// A structure that contains a rectangle (Left, Top, Right, Bottom) on a 2D cartesean plane.
    /// </summary>
    public struct LinearRect
    {
        public double Left;
        public double Top;
        public double Right;
        public double Bottom;

        /// <summary>
        /// Creates a new rectangle from the specified location and size.
        /// </summary>
        /// <param name="left">The left-most coordinate.</param>
        /// <param name="top">The top-most coordinate.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public LinearRect(double left, double top, double width, double height)
        {
            this.Left = left;
            this.Top = top;
            this.Right = (left + width);
            this.Bottom = (top + height);
        }

        /// <summary>
        /// Creates a new rectangle from the specified location and size.
        /// </summary>
        /// <param name="leftTop">The location.</param>
        /// <param name="size">The size.</param>
        public LinearRect(LinearCoordinates leftTop, LinearSize size)
        {
            Left = leftTop.X;
            Top = leftTop.Y;

            Right = (leftTop.X + size.Width);
            Bottom = (leftTop.Y + size.Height);
        }

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        /// <remarks>
        /// When this property is set, <see cref="Right"/> is recomputed.
        /// </remarks>
        public double Width
        {
            get => (Right - Left);
            set
            {
                Right = (Left + value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        /// <remarks>
        /// When this property is set, <see cref="Bottom"/> is recomputed.
        /// </remarks>
        public double Height
        {
            get => (Bottom - Top);
            set
            {
                Bottom = (Top + value);
            }
        }

        public static explicit operator RectangleF(LinearRect rc)
        {
            return new RectangleF((float)rc.Left, (float)rc.Top, (float)rc.Width, (float)rc.Height);
        }

        public static explicit operator Rectangle(LinearRect rc)
        {
            return new Rectangle((int)rc.Left, (int)rc.Top, (int)rc.Width, (int)rc.Height);
        }

        public static implicit operator LinearRect(RectangleF rc)
        {
            return new LinearRect(rc.Left, rc.Top, rc.Width, rc.Height);
        }

        public static implicit operator LinearRect(Rectangle rc)
        {
            return new LinearRect(rc.Left, rc.Top, rc.Width, rc.Height);
        }


    }

    /// <summary>
    /// A structure that contains a set of polar coordinates (arc sweep and radius) on a 2D plane.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PolarCoordinates //: IFormattable
    {
        public const string PolarSymbol = "φ";
        public const string PiSymbol = "π";
        public const string DegreeSymbol = "°";
        public const double RadianConst = 180.0d / 3.1415926535897931d;

        /// <summary>
        /// The angle (arc sweep) of the coordinate in degrees.
        /// </summary>
        public double Arc { get; set; }

        /// <summary>
        /// The radius value of the coordinate.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Create a new instance of the <see cref="PolarCoordinates"/> structure.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="arc">The angle (arc sweep) in degrees.</param>
        public PolarCoordinates(double radius, double arc)
        {
            Radius = radius;
            Arc = arc;
        }

        /// <summary>
        /// Create a new instance of the <see cref="PolarCoordinates"/> structure from an existing instance.
        /// </summary>
        /// <param name="p"></param>
        public PolarCoordinates(PolarCoordinates p)
        {
            Radius = p.Radius;
            Arc = p.Arc;
        }


        //public string ToString(string format, IFormatProvider formatProvider)
        //{

        //}

        /// <summary>
        /// Print the current values of the structure according to the specified formatting parameters.
        /// </summary>
        /// <param name="formatting">The formatting parameters.</param>
        /// <param name="precision">The maximum precision of the outputted numerical values.</param>
        /// <returns></returns>
        public string ToString(PolarCoordinatesFormattingFlags formatting, int precision = 2)
        {
            string s = "";
            
            double a = Arc;
            double r = Radius;
        
            if ((formatting & PolarCoordinatesFormattingFlags.UseParenthesis) == PolarCoordinatesFormattingFlags.UseParenthesis)
            {
                s += "(";
            }

            if ((formatting & PolarCoordinatesFormattingFlags.UseBrackets) == PolarCoordinatesFormattingFlags.UseBrackets)
            {
                s += "{";
            }

            s += Math.Round(r, precision).ToString() + ", ";
            if ((formatting & PolarCoordinatesFormattingFlags.DisplayInRadians) == PolarCoordinatesFormattingFlags.DisplayInRadians)
            {
                a *= RadianConst;
            }

            s += Math.Round(a, precision).ToString();
            if ((formatting & PolarCoordinatesFormattingFlags.UsePiSymbol) == PolarCoordinatesFormattingFlags.UsePiSymbol)
            {
                s += PiSymbol;
            }

            if ((formatting & PolarCoordinatesFormattingFlags.UseDegreeSymbol) == PolarCoordinatesFormattingFlags.UseDegreeSymbol)
            {
                s += DegreeSymbol;
            }

            if ((formatting & PolarCoordinatesFormattingFlags.UsePolarSymbol) == PolarCoordinatesFormattingFlags.UsePolarSymbol)
            {
                s += PolarSymbol;
            }

            if ((formatting & PolarCoordinatesFormattingFlags.UseRadianIndicator) == PolarCoordinatesFormattingFlags.UseRadianIndicator)
            {
                s += " rad";
            }

            if ((formatting & PolarCoordinatesFormattingFlags.UseBrackets) == PolarCoordinatesFormattingFlags.UseBrackets)
            {
                s += "}";
            }

            if ((formatting & PolarCoordinatesFormattingFlags.UseParenthesis) == PolarCoordinatesFormattingFlags.UseParenthesis)
            {
                s += ")";
            }

            return s;

        }

        public override string ToString()
        {
            return ToString(PolarCoordinatesFormattingFlags.Standard);
        }

        /// <summary>
        /// Converts the specified <see cref="PolarCoordinates"/> structure into its <see cref="LinearCoordinates"/> equivalent.
        /// </summary>
        /// <param name="p">The structure to convert.</param>
        /// <returns>A new instance of the <see cref="LinearCoordinates"/> structure.</returns>
        /// <remarks>
        /// Origin is presumed to be (0, 0), therefore the X and Y values of the returned structure may be negative.
        /// </remarks>
        public static LinearCoordinates ToLinearCoordinates(PolarCoordinates p)
        {
            return ToLinearCoordinates(p.Radius, p.Arc);
        }

        /// <summary>
        /// Converts the specified polar coordinates into a <see cref="LinearCoordinates"/> structure.
        /// </summary>
        /// <param name="a">The angle (arc sweep) of the polar coordinates.</param>
        /// <param name="r">The radius (size) of the polar coordinates.</param>
        /// <returns>A new instance of the <see cref="LinearCoordinates"/> structure.</returns>
        /// <remarks>
        /// Origin is presumed to be (0, 0), therefore the X and Y values of the returned structure may be negative.
        /// </remarks>
        public static LinearCoordinates ToLinearCoordinates(double r, double a)
        {
            double x;
            double y;
            
            a /= RadianConst;

            x = r * Math.Sin(a);
            y = r * Math.Cos(a);

            return new LinearCoordinates(x, -y);
        }

        /// <summary>
        /// Converts the specified <see cref="PolarCoordinates"/> structure into its <see cref="LinearCoordinates"/> equivalent, relative to the specified rectangle.
        /// </summary>
        /// <param name="p">The structure to convert.</param>
        /// <param name="rect">The reference rectangle to use.</param>
        /// <returns>A new instance of the <see cref="LinearCoordinates"/> structure.</returns>
        /// <remarks>
        /// Origin is computed from <paramref name="rect"/>. X and Y coordinates may be negative numbers if values within the rectangle are negative.
        /// </remarks>
        public static LinearCoordinates ToLinearCoordinates(PolarCoordinates p, LinearRect rect)
        {
            var pt = ToLinearCoordinates(p.Radius, p.Arc);

            double x;
            double y;

            x = (rect.Width / 2d) + pt.X;
            y = (rect.Height / 2d) + pt.Y;

            return new LinearCoordinates(x + rect.Left, y + rect.Top);
        }

        /// <summary>
        /// Given the specified <see cref="LinearCoordinates"/>, return a new <see cref="PolarCoordinates"/> structure.
        /// </summary>
        /// <param name="point">The linear coordinates.</param>
        /// <returns>A new <see cref="PolarCoordinates"/> structure.</returns>
        /// <remarks>
        /// Origin is presumed to be (0, 0), therefore, the X and Y values of <paramref name="point"/> may be negative.
        /// </remarks>
        public static PolarCoordinates ToPolarCoordinates(LinearCoordinates point)
        {
            return ToPolarCoordinates(point.X, point.Y);
        }

        /// <summary>
        /// Given the x and y coordinates, return a new <see cref="PolarCoordinates"/> structure.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>A new <see cref="PolarCoordinates"/> structure.</returns>
        /// <remarks>
        /// Origin is presumed to be (0, 0), therefore, <paramref name="x"/> and <paramref name="y"/> may be negative.
        /// </remarks>
        public static PolarCoordinates ToPolarCoordinates(double x, double y)
        {
            double r;
            double a;

            r = Math.Sqrt((x * x) + (y * y));

            // screen coordinates are funny, had to reverse this.
            a = Math.Atan(x / y);
            a *= RadianConst;

            if (!(x >= 0 && y < 0))
            {
                if (x < 0 && y < 0)
                {
                    a = 360 - a;
                }
                else if (x >= 0 && y >= 0)
                {
                    a = 180 - a;
                }
                else if (x < 0 && y >= 0)
                {
                    a = 90 + (90 - a);
                }
            }

            if (a < 0.0d)
                a = 360.0d - a;

            if (a > 360.0d)
                a = a - 360.0d;

            return new PolarCoordinates(r, a);
        }

        /// <summary>
        /// Tests whether or not the specified <see cref="LinearCoordinates"/> are within the specified radius.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <param name="rad">The radius to test.</param>
        /// <returns>True if the linear coordinates <paramref name="pt"/> are within <paramref name="rad" />.</returns>
        public static bool InWheel(LinearCoordinates pt, double rad)
        {
            pt.X -= rad;
            pt.Y -= rad;
            var p = ToPolarCoordinates(pt);
            return p.Radius <= rad;
        }

        public static explicit operator PolarCoordinates(LinearCoordinates operand)
        {
            return ToPolarCoordinates(operand);
        }
    }

}