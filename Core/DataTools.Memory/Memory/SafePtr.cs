using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTools.Memory
{
    public class SafePtr : SafePtrBase
    {
        public override bool IsInvalid => handle == IntPtr.Zero;
        private long size = 0;

        internal virtual new IntPtr handle
        {
            get => base.handle;
            set
            {
                if (base.handle == value) return;
                base.handle = value;
            }
        }

        public override MemoryType MemoryType => MemoryType.HGlobal;

        public SafePtr(IntPtr ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
        }

        public SafePtr(long size) : this(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        public SafePtr(int size) : this((long)size)
        {
        }

        public SafePtr() : this(IntPtr.Zero, true, true)
        {
        }

        public SafePtr(IntPtr ptr, long size, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            this.size = size;
        }

        public SafePtr(IntPtr ptr, long size) : this(ptr, size, true, true)
        {
        }

        public SafePtr(IntPtr ptr, int size) : this(ptr, (long)size, true, true)
        {
        }

        public SafePtr(IntPtr ptr) : this(ptr, false, false)
        {
        }

        public SafePtr(IntPtr ptr, bool fOwn) : this(ptr, fOwn, fOwn)
        {
        }

        public unsafe SafePtr(void* ptr, long size) : this((IntPtr)ptr, size, true, true)
        {
        }

        public unsafe SafePtr(void* ptr) : this((IntPtr)ptr, 0L, true, true)
        {
        }

        protected override void InternalDoZeroMem(IntPtr startptr, long length)
        {
            unsafe
            {
                Native.ZeroMemory((void*)startptr, length);
            }
        }

        protected override IntPtr Allocate(long size)
        {
            return Marshal.AllocHGlobal((int)size);
        }

        protected override void Deallocate(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
            this.size = 0;
        }

        protected override IntPtr Reallocate(IntPtr oldptr, long newsize)
        {
            return Marshal.ReAllocHGlobal(oldptr, (IntPtr)newsize);
        }

        protected override long GetNativeSize()
        {
            throw new NotImplementedException();
        }

        protected override bool CanGetNativeSize()
        {
            return false;
        }

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

        public override string ToString()
        {
            if (handle == IntPtr.Zero) return "";
            return GetString(0);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static explicit operator byte[](SafePtr val)
        {
            return val.ToByteArray();
        }

        public static explicit operator SafePtr(byte[] val)
        {
            var n = new SafePtr();
            n.FromByteArray(val);
            return n;
        }

        public static explicit operator char[](SafePtr val)
        {
            return val.ToCharArray();
        }

        public static explicit operator SafePtr(char[] val)
        {
            var n = new SafePtr();
            n.FromCharArray(val);
            return n;
        }

        public static explicit operator sbyte[](SafePtr val)
        {
            return val.ToArray<sbyte>();
        }

        public static explicit operator SafePtr(sbyte[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator short[](SafePtr val)
        {
            return val.ToArray<short>();
        }

        public static explicit operator SafePtr(short[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator ushort[](SafePtr val)
        {
            return val.ToArray<ushort>();
        }

        public static explicit operator SafePtr(ushort[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator int[](SafePtr val)
        {
            return val.ToArray<int>();
        }

        public static explicit operator SafePtr(int[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator uint[](SafePtr val)
        {
            return val.ToArray<uint>();
        }

        public static explicit operator SafePtr(uint[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator long[](SafePtr val)
        {
            return val.ToArray<long>();
        }

        public static explicit operator SafePtr(long[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator ulong[](SafePtr val)
        {
            return val.ToArray<ulong>();
        }

        public static explicit operator SafePtr(ulong[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator float[](SafePtr val)
        {
            return val.ToArray<float>();
        }

        public static explicit operator SafePtr(float[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator double[](SafePtr val)
        {
            return val.ToArray<double>();
        }

        public static explicit operator SafePtr(double[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator decimal[](SafePtr val)
        {
            return val.ToArray<decimal>();
        }

        public static explicit operator SafePtr(decimal[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator DateTime[](SafePtr val)
        {
            return val.ToArray<DateTime>();
        }

        public static explicit operator SafePtr(DateTime[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator Guid[](SafePtr val)
        {
            return val.ToArray<Guid>();
        }

        public static explicit operator SafePtr(Guid[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator string(SafePtr val)
        {
            if (val?.handle == IntPtr.Zero) return null;
            return val.GetString(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator SafePtr(string val)
        {
            var op = new SafePtr((val.Length + 1) * sizeof(char));
            op.SetString(0, val);
            return op;
        }

        public static SafePtr operator +(SafePtr val1, byte[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length);
            val1.FromByteArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, char[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(char));
            val1.FromCharArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, string val2)
        {
            var c = val1.size;
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

        public static SafePtr operator +(SafePtr val1, sbyte[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length);
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, short[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(short));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ushort[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(ushort));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, int[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(int));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, uint[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(uint));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, long[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(long));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ulong[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(ulong));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, float[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(float));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, double[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(double));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, decimal[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * sizeof(decimal));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, DateTime[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * Marshal.SizeOf<DateTime>());
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, Guid[] val2)
        {
            var c = val1.size;

            val1.Alloc(val1.size + val2.Length * Marshal.SizeOf<Guid>());
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, short val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, short val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ushort val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, ushort val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, int val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, int val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, long val2)
        {
            val1.handle = (IntPtr)((long)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, long val2)
        {
            val1.handle = (IntPtr)((long)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, uint val2)
        {
            val1.handle = (IntPtr)((uint)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, uint val2)
        {
            val1.handle = (IntPtr)((uint)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ulong val2)
        {
            val1.handle = (IntPtr)((ulong)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, ulong val2)
        {
            val1.handle = (IntPtr)((ulong)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, IntPtr val2)
        {
            val1.handle = (IntPtr)((long)val1.handle + (long)val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, IntPtr val2)
        {
            val1.handle = (IntPtr)((long)val1.handle - (long)val2);
            return val1;
        }

        public static bool operator ==(IntPtr val1, SafePtr val2)
        {
            return val1 == (val2?.handle ?? IntPtr.Zero);
        }

        public static bool operator !=(IntPtr val1, SafePtr val2)
        {
            return val1 != (val2?.handle ?? IntPtr.Zero);
        }

        public static bool operator ==(SafePtr val2, IntPtr val1)
        {
            return val1 == (val2?.handle ?? IntPtr.Zero);
        }

        public static bool operator !=(SafePtr val2, IntPtr val1)
        {
            return val1 != (val2?.handle ?? IntPtr.Zero);
        }

        public static implicit operator IntPtr(SafePtr val)
        {
            return val?.handle ?? IntPtr.Zero;
        }

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

        public static implicit operator UIntPtr(SafePtr val)
        {
            unsafe
            {
                return (UIntPtr)(void*)val.handle;
            }
        }

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