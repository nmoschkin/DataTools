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
 * Licensed Under the Apache 2.0 License
 **************************************************/

using System;
using System.Runtime.InteropServices;

namespace DataTools.MathTools.Polar
{
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

            s += System.Math.Round(r, precision).ToString() + ", ";
            if ((formatting & PolarCoordinatesFormattingFlags.DisplayInRadians) == PolarCoordinatesFormattingFlags.DisplayInRadians)
            {
                a *= RadianConst;
            }

            s += System.Math.Round(a, precision).ToString();
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

            x = r * System.Math.Sin(a);
            y = r * System.Math.Cos(a);

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

            x = rect.Width / 2d + pt.X;
            y = rect.Height / 2d + pt.Y;

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

            r = System.Math.Sqrt(x * x + y * y);

            // screen coordinates are funny, had to reverse this.
            a = System.Math.Atan(x / y);
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