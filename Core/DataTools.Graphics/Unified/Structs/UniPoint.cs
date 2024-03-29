using DataTools.MathTools.Polar;

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Unified point structure for WinForms, Win32 API, and SkiaSharp.
    /// </summary>
    /// <remarks>
    /// Platform-specific libraries within this library family will implement extensions to convert to platform-specific objects.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct UniPoint
    {
        private double _x;
        private double _y;

        /// <summary>
        /// The X (horizontal) coordinate
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// The Y (vertical) coordinate
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        /// <summary>
        /// Initialize a new <see cref="UniPoint"/> structure
        /// </summary>
        /// <param name="p"></param>
        public UniPoint(Point p)
        {
            _x = p.X;
            _y = p.Y;
        }

        /// <summary>
        /// Initialize a new <see cref="UniPoint"/> structure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UniPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Initialize a new <see cref="UniPoint"/> structure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UniPoint(float x, float y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Initialize a new <see cref="UniPoint"/> structure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UniPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Gets the bytes of this structure as two 64-bit floating point numbers.
        /// </summary>
        /// <returns></returns>
        public byte[] GetDoubleBytes()
        {
            int st = sizeof(double);
            byte[] ret = new byte[2 * st];

            Array.Copy(BitConverter.GetBytes(_x), 0, ret, 0, st);
            Array.Copy(BitConverter.GetBytes(_y), 0, ret, st, st);

            return ret;
        }

        /// <summary>
        /// Gets the bytes of this structure as two 32-bit floating point numbers.
        /// </summary>
        /// <returns></returns>
        public byte[] GetFloatBytes()
        {
            int st = sizeof(float);
            byte[] ret = new byte[2 * st];

            Array.Copy(BitConverter.GetBytes((float)_x), 0, ret, 0, st);
            Array.Copy(BitConverter.GetBytes((float)_y), 0, ret, st, st);

            return ret;
        }

        /// <summary>
        /// Gets the bytes of this structure as two 32-bit integers.
        /// </summary>
        /// <returns></returns>
        public byte[] GetIntBytes()
        {
            int st = sizeof(int);
            byte[] ret = new byte[2 * st];

            Array.Copy(BitConverter.GetBytes((int)_x), 0, ret, 0, st);
            Array.Copy(BitConverter.GetBytes((int)_y), 0, ret, st, st);

            return ret;
        }

        /// <summary>
        /// Create a new <see cref="UniPoint"/> structure from bytes.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <param name="isIntBytes">True if the bytes represent 2 32-bit integers.</param>
        /// <returns></returns>
        public static UniPoint FromBytes(byte[] bytes, bool isIntBytes)
        {
            var up = new UniPoint();

            if (isIntBytes && bytes.Length >= sizeof(int) * 2)
            {
                up._x = BitConverter.ToInt32(bytes, 0);
                up._y = BitConverter.ToInt32(bytes, sizeof(int));

                return up;
            }
            else if (bytes.Length >= sizeof(float) * 2)
            {
                if (bytes.Length >= (sizeof(double) * 2))
                {
                    up._x = BitConverter.ToDouble(bytes, 0);
                    up._y = BitConverter.ToDouble(bytes, sizeof(double));

                    return up;
                }
                else
                {
                    up._x = BitConverter.ToSingle(bytes, 0);
                    up._y = BitConverter.ToSingle(bytes, sizeof(float));

                    return up;
                }
            }
            else
            {
                throw new ArgumentException("Not enough bytes", nameof(bytes));
            }
        }

        /// <summary>
        /// Copy the structure to the pointer.
        /// </summary>
        /// <param name="ptr"></param>
        public unsafe void CopyTo(int* ptr)
        {
            (int, int) b = ((int, int))this;
            *ptr = b.Item1;
            *(ptr + sizeof(int)) = b.Item2;
        }

        /// <summary>
        /// Copy the structure to the pointer.
        /// </summary>
        /// <param name="ptr"></param>
        public void CopyToInt32Pointer(IntPtr ptr)
        {
            unsafe
            {
                CopyTo((int*)ptr);
            }
        }

        /// <summary>
        /// Copy the structure to the pointer.
        /// </summary>
        /// <param name="ptr"></param>
        public unsafe void CopyTo(double* ptr)
        {
            (double, double) b = ((double, double))this;
            *ptr = b.Item1;
            *(ptr + sizeof(double)) = b.Item2;
        }

        /// <summary>
        /// Copy the structure to the pointer.
        /// </summary>
        /// <param name="ptr"></param>
        public void CopyToDoublePointer(IntPtr ptr)
        {
            unsafe
            {
                CopyTo((double*)ptr);
            }
        }

        /// <summary>
        /// Copy the structure to the pointer.
        /// </summary>
        /// <param name="ptr"></param>
        public unsafe void CopyTo(float* ptr)
        {
            (float, float) b = ((float, float))this;
            *ptr = b.Item1;
            *(ptr + sizeof(float)) = b.Item2;
        }

        /// <summary>
        /// Copy the structure to the pointer.
        /// </summary>
        /// <param name="ptr"></param>
        public void CopyToFloatPointer(IntPtr ptr)
        {
            unsafe
            {
                CopyTo((float*)ptr);
            }
        }

        /// <summary>
        /// Create a new <see cref="UniPoint"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static unsafe UniPoint FromPointer(int* ptr)
        {
            var up = new UniPoint();

            up._x = *ptr;
            up._y = *(ptr + sizeof(int));

            return up;
        }

        /// <summary>
        /// Create a new <see cref="UniPoint"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static UniPoint FromInt32Pointer(IntPtr ptr)
        {
            unsafe
            {
                return FromPointer((int*)ptr);
            }
        }

        /// <summary>
        /// Create a new <see cref="UniPoint"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static unsafe UniPoint FromPointer(double* ptr)
        {
            var up = new UniPoint();

            up._x = *ptr;
            up._y = *(ptr + sizeof(double));

            return up;
        }

        /// <summary>
        /// Create a new <see cref="UniPoint"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static UniPoint FromDoublePointer(IntPtr ptr)
        {
            unsafe
            {
                return FromPointer((double*)ptr);
            }
        }

        /// <summary>
        /// Create a new <see cref="UniPoint"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static unsafe UniPoint FromPointer(float* ptr)
        {
            var up = new UniPoint();

            up._x = *ptr;
            up._y = *(ptr + sizeof(float));

            return up;
        }

        /// <summary>
        /// Create a new <see cref="UniPoint"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static UniPoint FromFloatPointer(IntPtr ptr)
        {
            unsafe
            {
                return FromPointer((float*)ptr);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}, {1}", X, Y);
        }

        /// <inheritdoc/>
        public static explicit operator PointF(UniPoint operand)
        {
            return new PointF((float)operand.X, (float)operand.Y);
        }

        /// <inheritdoc/>
        public static implicit operator UniPoint(PointF operand)
        {
            return new UniPoint(operand.X, operand.Y);
        }

        /// <inheritdoc/>
        public static explicit operator Point(UniPoint operand)
        {
            return new Point((int)operand.X, (int)operand.Y);
        }

        /// <inheritdoc/>
        public static implicit operator UniPoint(Point operand)
        {
            return new UniPoint(operand);
        }

        /// <inheritdoc/>
        public static implicit operator (double, double)(UniPoint operand)
        {
            return (operand._x, operand._y);
        }

        /// <inheritdoc/>
        public static implicit operator UniPoint((double, double) operand)
        {
            return new UniPoint(operand.Item1, operand.Item2);
        }

        /// <inheritdoc/>
        public static explicit operator (float, float)(UniPoint operand)
        {
            return ((float)operand._x, (float)operand._y);
        }

        /// <inheritdoc/>
        public static implicit operator UniPoint((float, float) operand)
        {
            return new UniPoint(operand.Item1, operand.Item2);
        }

        /// <inheritdoc/>
        public static explicit operator (int, int)(UniPoint operand)
        {
            return ((int)operand._x, (int)operand._y);
        }

        /// <inheritdoc/>
        public static implicit operator UniPoint((int, int) operand)
        {
            return new UniPoint(operand.Item1, operand.Item2);
        }

        /// <inheritdoc/>
        public static implicit operator LinearCoordinates(UniPoint point)
        {
            return new LinearCoordinates(point._x, point._y);
        }

        /// <inheritdoc/>
        public static implicit operator UniPoint(LinearCoordinates coordinates)
        {
            return new UniPoint(coordinates.X, coordinates.Y);
        }
    }
}