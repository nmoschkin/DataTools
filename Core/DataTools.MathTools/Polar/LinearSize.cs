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
            Width = width;
            Height = height;
        }
    }
}