using DataTools.MathTools.Polar;

using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Unified size structure for WinForms, Win32 API, and SkiaSharp.
    /// </summary>
    /// <remarks>
    /// Platform-specific libraries within this library family will implement extensions to convert to platform-specific objects.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct UniSize
    {
        private double width;
        private double height;

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public double Width
        {
            get => width;
            set => width = value;
        }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public double Height
        {
            get => height;
            set => height = value;
        }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public double CX
        {
            get => width;
            set => width = value;
        }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public double CY
        {
            get => height;
            set => height = value;
        }

        /// <summary>
        /// Initialize a new <see cref="UniSize"/> structure
        /// </summary>
        /// <param name="p"></param>
        public UniSize(Size p)
        {
            width = p.Width;
            height = p.Height;
        }

        /// <summary>
        /// Initialize a new <see cref="UniSize"/> structure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UniSize(int x, int y)
        {
            width = x;
            height = y;
        }

        /// <summary>
        /// Initialize a new <see cref="UniSize"/> structure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UniSize(float x, float y)
        {
            width = x;
            height = y;
        }

        /// <summary>
        /// Initialize a new <see cref="UniSize"/> structure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UniSize(double x, double y)
        {
            width = x;
            height = y;
        }

        /// <summary>
        /// Gets the bytes of this structure as two 64-bit floating point numbers.
        /// </summary>
        /// <returns></returns>
        public byte[] GetDoubleBytes()
        {
            int st = sizeof(double);
            byte[] ret = new byte[2 * st];

            Array.Copy(BitConverter.GetBytes(width), 0, ret, 0, st);
            Array.Copy(BitConverter.GetBytes(height), 0, ret, st, st);

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

            Array.Copy(BitConverter.GetBytes((float)width), 0, ret, 0, st);
            Array.Copy(BitConverter.GetBytes((float)height), 0, ret, st, st);

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

            Array.Copy(BitConverter.GetBytes((int)width), 0, ret, 0, st);
            Array.Copy(BitConverter.GetBytes((int)height), 0, ret, st, st);

            return ret;
        }

        /// <summary>
        /// Create a new <see cref="UniSize"/> structure from bytes.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <param name="isIntBytes">True if the bytes represent 2 32-bit integers.</param>
        /// <returns></returns>
        public static UniSize FromBytes(byte[] bytes, bool isIntBytes)
        {
            var up = new UniSize();

            if (isIntBytes && bytes.Length >= sizeof(int) * 2)
            {
                up.width = BitConverter.ToInt32(bytes, 0);
                up.height = BitConverter.ToInt32(bytes, sizeof(int));

                return up;
            }
            else if (bytes.Length >= sizeof(float) * 2)
            {
                if (bytes.Length >= (sizeof(double) * 2))
                {
                    up.width = BitConverter.ToDouble(bytes, 0);
                    up.height = BitConverter.ToDouble(bytes, sizeof(double));

                    return up;
                }
                else
                {
                    up.width = BitConverter.ToSingle(bytes, 0);
                    up.height = BitConverter.ToSingle(bytes, sizeof(float));

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
        /// Create a new <see cref="UniSize"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static unsafe UniSize FromPointer(int* ptr)
        {
            var up = new UniSize();

            up.width = *ptr;
            up.height = *(ptr + sizeof(int));

            return up;
        }

        /// <summary>
        /// Create a new <see cref="UniSize"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static UniSize FromInt32Pointer(IntPtr ptr)
        {
            unsafe
            {
                return FromPointer((int*)ptr);
            }
        }

        /// <summary>
        /// Create a new <see cref="UniSize"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static unsafe UniSize FromPointer(double* ptr)
        {
            var up = new UniSize();

            up.width = *ptr;
            up.height = *(ptr + sizeof(double));

            return up;
        }

        /// <summary>
        /// Create a new <see cref="UniSize"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static UniSize FromDoublePointer(IntPtr ptr)
        {
            unsafe
            {
                return FromPointer((double*)ptr);
            }
        }

        /// <summary>
        /// Create a new <see cref="UniSize"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static unsafe UniSize FromPointer(float* ptr)
        {
            var up = new UniSize();

            up.width = *ptr;
            up.height = *(ptr + sizeof(float));

            return up;
        }

        /// <summary>
        /// Create a new <see cref="UniSize"/> from a pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static UniSize FromFloatPointer(IntPtr ptr)
        {
            unsafe
            {
                return FromPointer((float*)ptr);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}, {1}", Width, Height);
        }

        /// <inheritdoc/>
        public static explicit operator SizeF(UniSize operand)
        {
            return new SizeF((float)operand.Width, (float)operand.Height);
        }

        /// <inheritdoc/>
        public static implicit operator UniSize(SizeF operand)
        {
            return new UniSize(operand.Width, operand.Height);
        }

        /// <inheritdoc/>
        public static explicit operator Size(UniSize operand)
        {
            return new Size((int)operand.Width, (int)operand.Height);
        }

        /// <inheritdoc/>
        public static implicit operator UniSize(Size operand)
        {
            return new UniSize(operand);
        }

        /// <inheritdoc/>
        public static implicit operator (double, double)(UniSize operand)
        {
            return (operand.width, operand.height);
        }

        /// <inheritdoc/>
        public static implicit operator UniSize((double, double) operand)
        {
            return new UniSize(operand.Item1, operand.Item2);
        }

        /// <inheritdoc/>
        public static explicit operator (float, float)(UniSize operand)
        {
            return ((float)operand.width, (float)operand.height);
        }

        /// <inheritdoc/>
        public static implicit operator UniSize((float, float) operand)
        {
            return new UniSize(operand.Item1, operand.Item2);
        }

        /// <inheritdoc/>
        public static explicit operator (int, int)(UniSize operand)
        {
            return ((int)operand.width, (int)operand.height);
        }

        /// <inheritdoc/>
        public static implicit operator UniSize((int, int) operand)
        {
            return new UniSize(operand.Item1, operand.Item2);
        }

        /// <inheritdoc/>
        public static implicit operator LinearSize(UniSize size)
        {
            return new LinearSize(size.width, size.height);
        }

        /// <inheritdoc/>
        public static implicit operator UniSize(LinearSize coordinates)
        {
            return new UniSize(coordinates.Width, coordinates.Height);
        }
    }
}