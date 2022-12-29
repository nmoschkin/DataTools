using DataTools.Streams;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTools.Memory
{
    public class SafePtr : SafePtrBase
    {
        public override bool IsInvalid => handle == 0;
        private long size = 0;

        internal virtual new nint handle
        {
            get => base.handle;
            set
            {
                if (base.handle == value) return;
                base.handle = value;
            }
        }

        public override MemoryType MemoryType => MemoryType.HGlobal;

        public SafePtr(nint ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
        }

        public SafePtr(long size) : this(0, true, true)
        {
            Alloc(size);
        }

        public SafePtr(int size) : this((long)size)
        {
        }

        public SafePtr() : this(0, true, true)
        {
        }

        public SafePtr(nint ptr, long size, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            this.size = size;
        }

        public SafePtr(nint ptr, long size) : this(ptr, size, true, true)
        {
        }

        public SafePtr(nint ptr, int size) : this(ptr, (long)size, true, true)
        {
        }

        public SafePtr(nint ptr) : this(ptr, false, false)
        {
        }

        public SafePtr(nint ptr, bool fOwn) : this(ptr, fOwn, fOwn)
        {
        }

        public unsafe SafePtr(void* ptr, long size) : this((nint)ptr, size, true, true)
        {
        }

        public unsafe SafePtr(void* ptr) : this((nint)ptr, 0L, true, true)
        {
        }

        /// <summary>
        /// Allocate a block of memory on a heap (typically the process heap).
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
        /// <param name="hHeap">
        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
        /// If you use an alternate heap handle, you will need to free the memory using the same heap handle or an error will occur.
        /// </param>
        /// <param name="zeroMem">Whether or not to zero the contents of the memory on allocation.</param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public virtual bool Alloc(long size, bool? addPressure = null, bool zeroMem = true)
        {
            var ap = addPressure ?? HasGCPressure;

            bool al;

            handle = Marshal.AllocHGlobal((nint)size);
            al = handle != nint.Zero;

            // see if we need to tell the garbage collector anything.
            if (al && ap) GC.AddMemoryPressure(size);
            this.size = size;

            if (zeroMem)
            {
                ZeroMemory(0, size);
            }

            return al;
        }

        /// <summary>
        /// Allocate a block of memory on the process heap.
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public virtual bool Alloc(long size, bool addPressure)
        {
            return Alloc(size, addPressure, true);
        }

        /// <summary>
        /// Allocate a block of memory on the process heap.
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public override bool Alloc(long size)
        {
            return Alloc(size, false, true);
        }

        public override bool ReAlloc(long size)
        {
            return ReAlloc(size, HasGCPressure);
        }

        /// <summary>
        /// Reallocate a block of memory to a different size on the task heap.
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <param name="modifyPressure">Whether or not to call GC.AddMemoryPressure or GC.RemoveMemoryPressure.</param>
        /// <param name="hHeap">
        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
        /// </param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public virtual bool ReAlloc(long size, bool? modifyPressure = null)
        {
            var ap = modifyPressure ?? HasGCPressure;

            if (handle == nint.Zero) return Alloc(size, modifyPressure);
            long oldsize = this.size;

            bool ra;

            // While the function doesn't need to call HeapReAlloc, it hasn't necessarily failed, either.

            handle = Marshal.ReAllocHGlobal(handle, new nint(size));

            ra = handle != nint.Zero;

            // see if we need to tell the garbage collector anything.
            if (ra && ap && oldsize > 0)
            {
                if (size < oldsize)
                    GC.RemoveMemoryPressure(oldsize - size);
                else
                    GC.AddMemoryPressure(size - oldsize);
            }

            this.size = size;
            return ra;
        }

        public override bool Free()
        {
            return Free(HasGCPressure);
        }

        /// <summary>
        /// Frees a previously allocated block of memory on the task heap.
        /// </summary>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <param name="removePressure">Whether or not to call GC.RemoveMemoryPressure</param>
        /// <param name="hHeap">
        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
        /// The handle pointed to by the internal pointer must have been previously allocated with the same heap handle.
        /// </param>
        /// <remarks></remarks>
        public virtual bool Free(bool? removePressure = null)
        {
            var ap = removePressure ?? HasGCPressure;

            long oldsize = size;
            // While the function doesn't need to call HeapFree, it hasn't necessarily failed, either.
            if (handle == nint.Zero)
                return true;
            else
            {
                // see if we need to tell the garbage collector anything.
                long l = oldsize;
                Marshal.FreeHGlobal(handle);

                // see if we need to tell the garbage collector anything.
                handle = nint.Zero;
                if (ap && oldsize > 0) GC.RemoveMemoryPressure(oldsize);
                size = 0;
            }
            return true;
        }

        public override long GetAllocatedSize()
        {
            return size;
        }

        protected virtual void TAlloc(long size)
        {
            switch (MemoryType)
            {
                case MemoryType.HGlobal:
                    Alloc(size, size > 1024);
                    return;

                default:
                    Alloc(size, size > 1024);
                    return;
            }
        }

        protected virtual void TFree()
        {
            switch (MemoryType)
            {
                case MemoryType.HGlobal:
                    Free();
                    return;

                default:
                    Free();
                    return;
            }
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

        protected override bool ReleaseHandle()
        {
            TFree();
            return true;
        }

        public override string ToString()
        {
            if (handle == 0) return "";
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
            if (val?.handle == 0) return null;
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
            val1.handle = (nint)(val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, long val2)
        {
            val1.handle = (nint)(val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, uint val2)
        {
            val1.handle = (nint)((uint)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, uint val2)
        {
            val1.handle = (nint)((uint)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ulong val2)
        {
            val1.handle = (nint)((ulong)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, ulong val2)
        {
            val1.handle = (nint)((ulong)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, nint val2)
        {
            val1.handle = (nint)(val1.handle + (long)val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, nint val2)
        {
            val1.handle = (nint)(val1.handle - (long)val2);
            return val1;
        }

        public static bool operator ==(nint val1, SafePtr val2)
        {
            return val1 == (val2?.handle ?? nint.Zero);
        }

        public static bool operator !=(nint val1, SafePtr val2)
        {
            return val1 != (val2?.handle ?? nint.Zero);
        }

        public static bool operator ==(SafePtr val2, nint val1)
        {
            return val1 == (val2?.handle ?? nint.Zero);
        }

        public static bool operator !=(SafePtr val2, nint val1)
        {
            return val1 != (val2?.handle ?? nint.Zero);
        }

        public static implicit operator nint(SafePtr val)
        {
            return val?.handle ?? nint.Zero;
        }

        public static implicit operator SafePtr(nint val)
        {
            unsafe
            {
                return new SafePtr
                {
                    handle = (nint)(void*)val
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
                    handle = (nint)(void*)val
                };
            }
        }
    }
}