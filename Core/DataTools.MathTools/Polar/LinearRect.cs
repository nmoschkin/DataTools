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

using System.Drawing;

namespace DataTools.MathTools.Polar
{
    /// <summary>
    /// A structure that contains a rectangle (Left, Top, Right, Bottom) on a 2D cartesean plane.
    /// </summary>
    public struct LinearRect
    {
        /// <summary>
        /// Left
        /// </summary>
        public double Left;

        /// <summary>
        /// Top
        /// </summary>
        public double Top;

        /// <summary>
        /// Right
        /// </summary>
        public double Right;

        /// <summary>
        /// Bottom
        /// </summary>
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
            Left = left;
            Top = top;
            Right = left + width;
            Bottom = top + height;
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

            Right = leftTop.X + size.Width;
            Bottom = leftTop.Y + size.Height;
        }

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        /// <remarks>
        /// When this property is set, <see cref="Right"/> is recomputed.
        /// </remarks>
        public double Width
        {
            get => Right - Left;
            set
            {
                Right = Left + value;
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
            get => Bottom - Top;
            set
            {
                Bottom = Top + value;
            }
        }

        /// <inheritdoc/>
        public static explicit operator RectangleF(LinearRect rc)
        {
            return new RectangleF((float)rc.Left, (float)rc.Top, (float)rc.Width, (float)rc.Height);
        }

        /// <inheritdoc/>
        public static explicit operator Rectangle(LinearRect rc)
        {
            return new Rectangle((int)rc.Left, (int)rc.Top, (int)rc.Width, (int)rc.Height);
        }

        /// <inheritdoc/>
        public static implicit operator LinearRect(RectangleF rc)
        {
            return new LinearRect(rc.Left, rc.Top, rc.Width, rc.Height);
        }

        /// <inheritdoc/>
        public static implicit operator LinearRect(Rectangle rc)
        {
            return new LinearRect(rc.Left, rc.Top, rc.Width, rc.Height);
        }
    }
}