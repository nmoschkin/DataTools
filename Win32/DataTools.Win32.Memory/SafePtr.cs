using DataTools.Memory;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Memory
{
    /// <summary>
    /// SafePtr based on Win32
    /// </summary>
    public class SafePtr : WinPtrBase, IHeapAssignable
    {
        /// <summary>
        /// Gets the pointer to the process heap.
        /// </summary>
        public static readonly IntPtr ProcessHeap = Native.GetProcessHeap();

        private IntPtr currentHeap = ProcessHeap;

        /// <summary>
        /// Gets the <see cref="DataTools.Win32.Memory.HeapDestroyBehavior"/> for this instance.
        /// </summary>
        public virtual HeapDestroyBehavior HeapDestroyBehavior { get; protected set; } = HeapDestroyBehavior.TransferOut;

        void IHeapAssignable.AssignHeap(Heap heap)
        {
            CurrentHeap = heap.DangerousGetHandle();
        }

        void IHeapAssignable.HeapIsClosing(DataTools.Win32.Memory.Heap heap)
        {
            if (heap.DangerousGetHandle() == currentHeap)
            {
                if (HeapDestroyBehavior == HeapDestroyBehavior.Cascade)
                {
                    Free();
                    return;
                }

                if ((long)handle != 0)
                {
                    try
                    {
                        unsafe
                        {
                            var l = (IntPtr)Length;
                            void* ptr = (void*)Native.HeapAlloc(ProcessHeap, 0, l);

                            if (ptr != null)
                            {
                                Buffer.MemoryCopy((void*)handle, ptr, (long)l, (long)l);
                            }

                            Native.HeapFree(currentHeap, 0, handle);
                            currentHeap = ProcessHeap;
                            handle = (IntPtr)ptr;
                        }
                    }
                    catch
                    {
                        try
                        {
                            Free();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The current heap.
        /// </summary>
        /// <remarks>
        /// This is usually the process heap, but creating independent heaps are possible.
        /// </remarks>
        public virtual IntPtr CurrentHeap
        {
            get => currentHeap;
            protected set
            {
                if (handle != IntPtr.Zero) throw new InvalidOperationException("Can't change heap when the buffer is allocated!");

                if (currentHeap == value)
                {
                    return;
                }
                else if (value == IntPtr.Zero)
                {
                    currentHeap = ProcessHeap;
                }
                else
                {
                    currentHeap = value;
                }
            }
        }

        /// <summary>
        /// Open access to the internal for the handle
        /// </summary>
        protected internal new IntPtr handle
        {
            get => (IntPtr)base.handle;
            protected set => base.handle = value;
        }

        /// <summary>
        /// Create a new safe pointer from the specified pointer
        /// </summary>
        /// <param name="ptr">The pointer to access</param>
        /// <param name="fOwn">True if this object owns this pointer</param>
        /// <param name="gcpressure">True to inform garbage collection of allocations and releases</param>
        public SafePtr(IntPtr ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
        }

        /// <summary>
        /// Create a new, unallocated safe pointer 
        /// </summary>
        public SafePtr() : this(IntPtr.Zero, true, true)
        {
        }

        /// <summary>
        /// Create a new safe pointer allocated on the process heap
        /// </summary>
        /// <param name="size">The amount of memory to allocate</param>
        public SafePtr(long size) : this(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        /// <summary>
        /// Create a new safe pointer allocated on the process heap
        /// </summary>
        /// <param name="size">The amount of memory to allocate</param>
        public SafePtr(int size) : this(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        /// <summary>
        /// Create a new safe pointer from byte data
        /// </summary>
        /// <param name="data">The data to initialize the buffer with</param>
        public SafePtr(byte[] data) : this(IntPtr.Zero, true, true)
        {
            FromByteArray(data);
        }

        /// <summary>
        /// Allocate or reallocate the buffer and zero out the memory after allocation
        /// </summary>
        /// <param name="size">The new size of the buffer</param>
        /// <returns></returns>
        public bool AllocZero(long size)
        {
            var b = Alloc(size);
            if (b) ZeroMemory();
            return b;
        }

        /// <inheritdoc/>
        public override MemoryType MemoryType { get; }

        /// <inheritdoc/>
        protected override IntPtr Allocate(long size)
        {
            return Native.HeapAlloc(CurrentHeap, 8, (IntPtr)size);
        }

        /// <inheritdoc/>
        protected override bool CanGetNativeSize()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override SafePtrBase Clone()
        {
            var b = new SafePtr();
            var by = this.ToByteArray();
            b.FromByteArray(by);
            return b;
        }

        /// <inheritdoc/>
        protected override void Deallocate(IntPtr ptr)
        {
            Native.HeapFree(CurrentHeap, 0, ptr);
        }

        /// <inheritdoc/>
        protected override long GetNativeSize()
        {
            if ((long)handle == 0) return 0;
            return (long)Native.HeapSize(CurrentHeap, 0, handle);
        }

        /// <inheritdoc/>
        protected override IntPtr Reallocate(IntPtr oldptr, long newsize)
        {
            return Native.HeapReAlloc(CurrentHeap, 0, handle, (IntPtr)newsize);
        }

        #region Cast Operators

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
            if (val?.handle == (IntPtr)0) return null;
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
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length);
            val1.FromByteArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, char[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(char));
            val1.FromCharArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, string val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(char));
            val1.FromCharArray(val2.ToCharArray(), c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, sbyte[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length);
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, short[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(short));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ushort[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(ushort));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, int[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(int));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, uint[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(uint));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, long[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(long));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ulong[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(ulong));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, float[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(float));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, double[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(double));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, decimal[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(decimal));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, DateTime[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * Marshal.SizeOf<DateTime>());
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, Guid[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * Marshal.SizeOf<Guid>());
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

        public override bool Equals(object obj)
        {
            if (obj is SafePtr b)
            {
                return b.handle == handle;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        #endregion Cast Operators
    }
}