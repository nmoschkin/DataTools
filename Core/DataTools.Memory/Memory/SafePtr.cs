using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataTools.Memory
{
    /// <summary>
    /// Safe general use heap memory handler
    /// </summary>
    public class SafePtr : SafePtrBase
    {
        /// <inheritdoc/>
        public override bool IsInvalid => handle == IntPtr.Zero;

        internal virtual new IntPtr handle
        {
            get => base.handle;
            set
            {
                if (base.handle == value) return;
                base.handle = value;
            }
        }

        /// <inheritdoc/>
        public override MemoryType MemoryType => MemoryType.HGlobal;

        /// <inheritdoc/>
        public SafePtr(IntPtr ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
        }

        /// <inheritdoc/>
        public SafePtr(byte[] data) : this(IntPtr.Zero, true, true)
        {            
            FromByteArray(data);
        }

        /// <inheritdoc/>
        public SafePtr(long Length) : this(IntPtr.Zero, true, true)
        {
            Alloc(Length);
        }

        /// <inheritdoc/>
        public SafePtr(int Length) : this((long)Length)
        {
        }

        /// <inheritdoc/>
        public SafePtr() : this(IntPtr.Zero, true, true)
        {
        }

        /// <inheritdoc/>
        public SafePtr(IntPtr ptr, long Length, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            this.Length = Length;
        }

        /// <inheritdoc/>
        public SafePtr(IntPtr ptr, long Length) : this(ptr, Length, true, true)
        {
        }

        /// <inheritdoc/>
        public SafePtr(IntPtr ptr, int Length) : this(ptr, (long)Length, true, true)
        {
        }

        /// <inheritdoc/>
        public SafePtr(IntPtr ptr) : this(ptr, false, false)
        {
        }

        /// <inheritdoc/>
        public SafePtr(IntPtr ptr, bool fOwn) : this(ptr, fOwn, fOwn)
        {
        }

        /// <inheritdoc/>
        public unsafe SafePtr(void* ptr, long Length) : this((IntPtr)ptr, Length, true, true)
        {
        }

        /// <inheritdoc/>
        public unsafe SafePtr(void* ptr) : this((IntPtr)ptr, 0L, true, true)
        {
        }

        /// <inheritdoc/>
        protected override void InternalDoZeroMem(IntPtr startptr, long length)
        {
            unsafe
            {
                Native.ZeroMemory((void*)startptr, length);
            }
        }

        /// <inheritdoc/>
        protected override IntPtr Allocate(long Length)
        {
            return Marshal.AllocHGlobal((int)Length);
        }

        /// <inheritdoc/>
        protected override void Deallocate(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
            this.Length = 0;
        }

        /// <inheritdoc/>
        protected override IntPtr Reallocate(IntPtr oldptr, long newsize)
        {
            return Marshal.ReAllocHGlobal(oldptr, (IntPtr)newsize);
        }

        /// <inheritdoc/>
        protected override long GetNativeSize()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override bool CanGetNativeSize()
        {
            return false;
        }

        /// <inheritdoc/>
        protected override SafePtrBase Clone()
        {
            unsafe
            {
                var l = Length;
                if (l <= 0) return new SafePtr();
                var sp = new SafePtr(l);

                Buffer.MemoryCopy((void*)handle, (void*)sp, l, l);
                return sp;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (handle == IntPtr.Zero) return "";
            return GetString(0);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public static explicit operator byte[](SafePtr val)
        {
            return val.ToByteArray();
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public static explicit operator SafePtr(byte[] val)
        {
            var n = new SafePtr();
            n.FromByteArray(val);
            return n;
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public static explicit operator char[](SafePtr val)
        {
            return val.ToCharArray();
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public static explicit operator SafePtr(char[] val)
        {
            var n = new SafePtr();
            n.FromCharArray(val);
            return n;
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public static explicit operator sbyte[](SafePtr val)
        {
            return val.ToArray<sbyte>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(sbyte[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator short[](SafePtr val)
        {
            return val.ToArray<short>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(short[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator ushort[](SafePtr val)
        {
            return val.ToArray<ushort>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(ushort[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator int[](SafePtr val)
        {
            return val.ToArray<int>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(int[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator uint[](SafePtr val)
        {
            return val.ToArray<uint>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(uint[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator long[](SafePtr val)
        {
            return val.ToArray<long>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(long[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator ulong[](SafePtr val)
        {
            return val.ToArray<ulong>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(ulong[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator float[](SafePtr val)
        {
            return val.ToArray<float>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(float[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator double[](SafePtr val)
        {
            return val.ToArray<double>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(double[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator decimal[](SafePtr val)
        {
            return val.ToArray<decimal>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(decimal[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator DateTime[](SafePtr val)
        {
            return val.ToArray<DateTime>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(DateTime[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        public static explicit operator Guid[](SafePtr val)
        {
            return val.ToArray<Guid>();
        }

        /// <inheritdoc/>
        public static explicit operator SafePtr(Guid[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator string(SafePtr val)
        {
            if (val?.handle == IntPtr.Zero) return null;
            return val.GetString(0);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator SafePtr(string val)
        {
            var op = new SafePtr((val.Length + 1) * sizeof(char));
            op.SetString(0, val);
            return op;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, byte[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length);
            val1.FromByteArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, char[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(char));
            val1.FromCharArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, string val2)
        {
            var c = val1.Length;
            val1.Length += (val2.Length * 2);

            if (val1.CharAtAbsolute(c - sizeof(char)) == 0)
            {
                val1.SetString(c - sizeof(char), val2);
            }
            else
            {
                val1.SetString(c, val2);
            }

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, sbyte[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length);
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, short[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(short));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, ushort[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(ushort));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, int[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(int));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, uint[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(uint));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, long[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(long));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, ulong[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(ulong));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, float[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(float));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, double[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(double));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, decimal[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(decimal));
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, DateTime[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * Marshal.SizeOf<DateTime>());
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, Guid[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * Marshal.SizeOf<Guid>());
            val1.FromArray(val2, c);

            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, short val2)
        {
            val1.handle += val2;
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator -(SafePtr val1, short val2)
        {
            val1.handle -= val2;
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, ushort val2)
        {
            val1.handle += val2;
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator -(SafePtr val1, ushort val2)
        {
            val1.handle -= val2;
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, int val2)
        {
            val1.handle += val2;
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator -(SafePtr val1, int val2)
        {
            val1.handle -= val2;
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, long val2)
        {
            val1.handle = (IntPtr)((long)val1.handle + val2);
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator -(SafePtr val1, long val2)
        {
            val1.handle = (IntPtr)((long)val1.handle - val2);
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, uint val2)
        {
            val1.handle = (IntPtr)((uint)val1.handle + val2);
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator -(SafePtr val1, uint val2)
        {
            val1.handle = (IntPtr)((uint)val1.handle - val2);
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, ulong val2)
        {
            val1.handle = (IntPtr)((ulong)val1.handle + val2);
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator -(SafePtr val1, ulong val2)
        {
            val1.handle = (IntPtr)((ulong)val1.handle - val2);
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator +(SafePtr val1, IntPtr val2)
        {
            val1.handle = (IntPtr)((long)val1.handle + (long)val2);
            return val1;
        }

        /// <inheritdoc/>
        public static SafePtr operator -(SafePtr val1, IntPtr val2)
        {
            val1.handle = (IntPtr)((long)val1.handle - (long)val2);
            return val1;
        }

        /// <inheritdoc/>
        public static bool operator ==(IntPtr val1, SafePtr val2)
        {
            return val1 == (val2?.handle ?? IntPtr.Zero);
        }

        /// <inheritdoc/>
        public static bool operator !=(IntPtr val1, SafePtr val2)
        {
            return val1 != (val2?.handle ?? IntPtr.Zero);
        }

        /// <inheritdoc/>
        public static bool operator ==(SafePtr val2, IntPtr val1)
        {
            return val1 == (val2?.handle ?? IntPtr.Zero);
        }

        /// <inheritdoc/>
        public static bool operator !=(SafePtr val2, IntPtr val1)
        {
            return val1 != (val2?.handle ?? IntPtr.Zero);
        }

        /// <inheritdoc/>
        public static implicit operator IntPtr(SafePtr val)
        {
            return val?.handle ?? IntPtr.Zero;
        }

        /// <inheritdoc/>
        public static implicit operator SafePtr(IntPtr val)
        {
            unsafe
            {
                return new SafePtr
                {
                    handle = (IntPtr)(void*)val
                };
            }
        }

        /// <inheritdoc/>
        public static implicit operator UIntPtr(SafePtr val)
        {
            unsafe
            {
                return (UIntPtr)(void*)val.handle;
            }
        }

        /// <inheritdoc/>
        public static implicit operator SafePtr(UIntPtr val)
        {
            unsafe
            {
                return new SafePtr
                {
                    handle = (IntPtr)(void*)val
                };
            }
        }
    }
}