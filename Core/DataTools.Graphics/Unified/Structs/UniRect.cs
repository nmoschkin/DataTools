using DataTools.MathTools.Polar;

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Unified rectangle structure for WinForms, Win32 API, and SkiaSharp.
    /// </summary>
    /// <remarks>
    /// Platform-specific libraries within this library family will implement extensions to convert to platform-specific objects.
    /// <br /><br />
    /// The internal structure of this object is 4 sequential <see cref="double"/> values.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct UniRect
    {
        private double left;
        private double top;
        private double width;
        private double height;

        /// <summary>
        /// The leftmost extent of the rectangle (reckoned from 0)
        /// </summary>
        public double Left
        {
            get => left;
            set => left = value;
        }

        /// <summary>
        /// The topmost extent of the rectangle (reckoned from 0)
        /// </summary>
        public double Top
        {
            get => top;
            set => top = value;
        }

        /// <summary>
        /// The width of the rectangle
        /// </summary>
        public double Width
        {
            get => width;
            set => width = value;
        }

        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public double Height
        {
            get => height;
            set => height = value;
        }

        /// <summary>
        /// The rightmost extent of the rectangle (reckoned from 0)
        /// </summary>
        public double Right
        {
            get => width - left - 1d;
            set => width = value - left + 1d;
        }

        /// <summary>
        /// The bottommost extent of the rectangle (reckoned from 0)
        /// </summary>
        public double Bottom
        {
            get => height - top - 1d;
            set => height = value - top + 1d;
        }

        /// <summary>
        /// Synonym for <see cref="Left"/>
        /// </summary>
        public double X => left;

        /// <summary>
        /// Synonym for <see cref="Top"/>
        /// </summary>
        public double Y => top;

        /// <summary>
        /// Synonym for <see cref="Width"/>
        /// </summary>
        public double CX => width;

        /// <summary>
        /// Synonym for <see cref="Height"/>
        /// </summary>
        public double CY => height;

        /// <summary>
        /// Create a new rectangle with the specified geometry
        /// </summary>
        /// <param name="x">The top-left X coordinate in pixels reckoned from zero</param>
        /// <param name="x">The top-left Y coordinate in pixels reckoned from zero</param>
        /// <param name="width">The width in pixels</param>
        /// <param name="width">The height in pixels</param>
        public UniRect(double x, double y, double width, double height)
        {
            left = x;
            top = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Create a new rectangle from the specified location and size.
        /// </summary>
        /// <param name="location">The location point</param>
        /// <param name="size">The size structure</param>
        public UniRect(Point location, Size size)
        {
            left = location.X;
            top = location.Y;
            width = size.Width;
            height = size.Height;
        }

        /// <summary>
        /// Create a new rectangle from the specified location and size.
        /// </summary>
        /// <param name="location">The location point</param>
        /// <param name="size">The size structure</param>
        public UniRect(PointF location, SizeF size)
        {
            left = location.X;
            top = location.Y;
            width = size.Width;
            height = size.Height;
        }

        /// <summary>
        /// Create a new rectangle based on another rectangle
        /// </summary>
        /// <param name="rc">The rectangle to duplicate</param>
        public UniRect(Rectangle rc)
        {
            left = rc.Left;
            top = rc.Top;
            width = rc.Width;
            height = rc.Height;
        }

        /// <summary>
        /// Create a new rectangle based on another rectangle
        /// </summary>
        /// <param name="rc">The rectangle to duplicate</param>
        public UniRect(RectangleF rc)
        {
            left = rc.Left;
            top = rc.Top;
            width = rc.Width;
            height = rc.Height;
        }

        /// <summary>
        /// Create a new rectangle based on another rectangle
        /// </summary>
        /// <param name="rc">The rectangle to duplicate</param>
        public UniRect(LinearRect rc)
        {
            left = rc.Left;
            top = rc.Top;
            width = rc.Width;
            height = rc.Height;
        }

        public static UniRect FromInts(int[] ints)
        {
            return new UniRect(ints[0], ints[1], ints[2], ints[3]);
        }

        public static unsafe UniRect FromIntsPointer(void* ptr)
        {
            int* ints = (int*)ptr;
            return new UniRect(ints[0], ints[1], ints[2], ints[3]);
        }

        public static UniRect FromIntsPointer(IntPtr ptr)
        {
            unsafe
            {
                int* ints = (int*)ptr;
                return new UniRect(ints[0], ints[1], ints[2], ints[3]);
            }
        }

        public static UniRect FromDoubles(double[] doubles)
        {
            return new UniRect(doubles[0], doubles[1], doubles[2], doubles[3]);
        }

        public static UniRect FromDoublesPointer(IntPtr ptr)
        {
            unsafe
            {
                double* doubles = (double*)ptr;
                return new UniRect(doubles[0], doubles[1], doubles[2], doubles[3]);
            }
        }

        public static unsafe UniRect FromDoublesPointer(void* ptr)
        {
            double* doubles = (double*)ptr;
            return new UniRect(doubles[0], doubles[1], doubles[2], doubles[3]);
        }

        public static UniRect FromFloats(float[] floats)
        {
            return new UniRect(floats[0], floats[1], floats[2], floats[3]);
        }

        public static UniRect FromFloatsPointer(IntPtr ptr)
        {
            unsafe
            {
                float* floats = (float*)ptr;
                return new UniRect(floats[0], floats[1], floats[2], floats[3]);
            }
        }

        public static unsafe UniRect FromFloatsPointer(void* ptr)
        {
            float* floats = (float*)ptr;
            return new UniRect(floats[0], floats[1], floats[2], floats[3]);
        }

        /// <summary>
        /// Reports the position and dimensions of the rectangle as a human-readable string expression
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}; {2}x{3}", left, top, width, height);
        }

        public static explicit operator RectangleF(UniRect operand)
        {
            return new RectangleF((float)operand.left, (float)operand.top, (float)operand.width, (float)operand.height);
        }

        public static implicit operator UniRect(RectangleF operand)
        {
            return new UniRect(operand);
        }

        public static explicit operator Rectangle(UniRect operand)
        {
            return new Rectangle((int)operand.left, (int)operand.top, (int)operand.width, (int)operand.height);
        }

        public static implicit operator UniRect(Rectangle operand)
        {
            return new UniRect(operand);
        }

        public static implicit operator double[](UniRect operand)
        {
            return new[] { operand.Left, operand.Top, operand.Right, operand.Bottom };
        }

        public static implicit operator LinearRect(UniRect operand)
        {
            return new LinearRect(operand.Left, operand.Top, operand.Width, operand.Height);
        }

        public static implicit operator UniRect(LinearRect operand)
        {
            return new UniRect(operand);
        }
    }
}