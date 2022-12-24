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

namespace DataTools.MathTools.Polar
{
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
}