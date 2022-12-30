using DataTools.Memory;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Memory
{
    /// <summary>
    /// Wraps a native Win32 custom heap
    /// </summary>
    public sealed class Heap : SafeHandle
    {
        /// <summary>
        /// Gets the process heap.
        /// </summary>
        public static readonly Heap ProcessHeap = new Heap(Native.GetProcessHeap());

        private readonly List<WeakReference<IHeapAssignable>> createdObjecs = new List<WeakReference<IHeapAssignable>>();

        private IntPtr maxsize = IntPtr.Zero;
        private IntPtr currsize = IntPtr.Zero;

        public bool IsProcessHeap => ProcessHeap.handle == handle;

        public long MaxSize
        {
            get
            {
                GetHeapSize();
                return (long)maxsize;
            }
        }

        public long CurrentSize
        {
            get
            {
                GetHeapSize();
                return (long)currsize;
            }
        }

        public long UsedSpace
        {
            get
            {
                return GetHeapSize();
            }
        }

        public long UnusedSpace
        {
            get
            {
                var sv = GetHeapSize(out var a, out var b);
                return (a + b) - sv;
            }
        }

        public Heap(IntPtr heapPtr, bool fOwn) : base(IntPtr.Zero, fOwn)
        {
            handle = heapPtr;
            GetHeapSize();
        }

        private Heap(IntPtr ptr) : base(IntPtr.Zero, false)
        {
            handle = ptr;
            GetHeapSize();
        }

        public Heap(long size) : base(IntPtr.Zero, true)
        {
            CreateHeap((IntPtr)size, (IntPtr)size);
        }

        public Heap(int size) : base(IntPtr.Zero, true)
        {
            CreateHeap((IntPtr)size, (IntPtr)size);
        }

        public Heap(long size, long maxsize) : base(IntPtr.Zero, true)
        {
            CreateHeap((IntPtr)size, (IntPtr)maxsize);
        }

        public Heap(int size, int maxsize) : base(IntPtr.Zero, true)
        {
            CreateHeap((IntPtr)size, (IntPtr)maxsize);
        }

        /// <summary>
        /// Gets the size of the heap.
        /// </summary>
        /// <returns></returns>
        public long GetHeapSize()
        {
            return GetHeapSize(out _, out _);
        }

        /// <summary>
        /// Gets the size of the heap, and the allocated and unallocated sizes.
        /// </summary>
        /// <param name="committed">The current size of the heap.</param>
        /// <param name="uncommitted">The size of the heap remaining.</param>
        /// <returns>The total amount of memory allocated on the heap.</returns>
        public long GetHeapSize(out long committed, out long uncommitted)
        {
            committed = uncommitted = 0;

            if ((long)handle == 0) return 0;
            using (var sp = new SafePtr())
            {
                int cbsize = Marshal.SizeOf<PROCESS_HEAP_ENTRY_REGION>();

                PROCESS_HEAP_ENTRY_REGION rgn = new PROCESS_HEAP_ENTRY_REGION();

                sp.Alloc(cbsize);

                while (Native.HeapWalk(handle, sp))
                {
                    try
                    {
                        rgn = sp.ToStruct<PROCESS_HEAP_ENTRY_REGION>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return 0;
                    }

                    if ((rgn.wFlags & HeapWalkFlags.PROCESS_HEAP_REGION) != 0)
                    {
                        var reg = sp.ToStruct<PROCESS_HEAP_ENTRY_REGION>();

                        maxsize = (IntPtr)(reg.dwCommittedSize + reg.dwUnCommittedSize);
                        this.currsize = (IntPtr)(reg.dwCommittedSize);

                        committed = (long)(IntPtr)reg.dwCommittedSize;
                        uncommitted = (long)(IntPtr)reg.dwUnCommittedSize;

                        return (long)(IntPtr)reg.cbData;
                    }
                }
            }

            return 0;
        }

        private void CreateHeap(IntPtr size, IntPtr maxsize)
        {
            handle = Native.HeapCreate(0, size, maxsize);
            if ((long)handle == 0)
            {
                this.maxsize = this.currsize = IntPtr.Zero;
            }
            else
            {
                GC.AddMemoryPressure((long)maxsize);

                this.maxsize = maxsize;
                this.currsize = size;

                GetHeapSize();
            }
        }

        private bool DestroyHeap()
        {
            bool b = true;

            if ((long)handle != 0)
            {
                b = Native.HeapDestroy(handle);
                if (b) handle = IntPtr.Zero;
            }

            return b;
        }

        /// <summary>
        /// Create a new <see cref="WinPtrBase"/> <see cref="IHeapAssignable"/> object on the current heap.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreatePtr<T>() where T : WinPtrBase, IHeapAssignable, new()
        {
            var ptr = new T();
            ptr.AssignHeap(this);
            createdObjecs.Add(new WeakReference<IHeapAssignable>(ptr));
            return ptr;
        }

        /// <summary>
        /// Create a new <see cref="WinPtrBase"/> <see cref="IHeapAssignable"/> object on the current heap.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Copy the contents of the source buffer to the new object.</param>
        /// <returns></returns>
        public T CreatePtr<T>(SafePtrBase source) where T : WinPtrBase, IHeapAssignable, new()
        {
            var ptr = new T();
            ptr.AssignHeap(this);
            createdObjecs.Add(new WeakReference<IHeapAssignable>(ptr));

            if (source != null && source.Length > 0)
            {
                ptr.Alloc(source.Length);
                source.CopyTo(ptr, 0L, 0L, source.Length);
            }

            return ptr;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            foreach (var wr in createdObjecs)
            {
                if (wr.TryGetTarget(out var ptr))
                {
                    ptr.HeapIsClosing(this);
                }
            }

            return DestroyHeap();
        }

        public override string ToString()
        {
            var b = 2 * IntPtr.Size;
            var xb = "x" + b.ToString();
            return $"0x{handle.ToString(xb)} [{UsedSpace:#,##0} Used; {UnusedSpace:#,##0} Free]";
        }
    }

    /// <summary>
    /// Actions to be taken by an object created by a <see cref="Heap"/> when it is being destroyed.
    /// </summary>
    public enum HeapDestroyBehavior
    {
        /// <summary>
        /// The contents of the object are destroyed with the heap.
        /// </summary>
        Cascade,

        /// <summary>
        /// The buffer is transferred to the process heap.
        /// </summary>
        TransferOut
    }

    public interface IHeapAssignable
    {
        /// <summary>
        /// The current heap.
        /// </summary>
        /// <remarks>
        /// This is usually the process heap, but creating independent heaps are possible.
        /// </remarks>
        IntPtr CurrentHeap { get; }

        HeapDestroyBehavior HeapDestroyBehavior { get; }

#if NET6_0_OR_GREATER

        /// <summary>
        /// Set the heap for the object
        /// </summary>
        /// <param name="heap"></param>
        protected internal void AssignHeap(Heap heap);

        protected internal void HeapIsClosing(Heap heap);

#else
        /// <summary>
        /// Set the heap for the object
        /// </summary>
        /// <param name="heap"></param>
        void AssignHeap(Heap heap);

        void HeapIsClosing(Heap heap);

#endif
    }

    public class SafePtr : WinPtrBase, IHeapAssignable
    {
        /// <summary>
        /// Gets the pointer to the process heap.
        /// </summary>
        public static readonly IntPtr ProcessHeap = Native.GetProcessHeap();

        private IntPtr currentHeap = ProcessHeap;

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

        protected internal new IntPtr handle
        {
            get => (IntPtr)base.handle;
            protected set => base.handle = value;
        }

        public SafePtr(IntPtr ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
        }

        public SafePtr() : this(IntPtr.Zero, true, true)
        {
        }

        public SafePtr(long size) : this(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        public SafePtr(int size) : this(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        public bool AllocZero(long size)
        {
            var b = Alloc(size);
            if (b) ZeroMemory();
            return b;
        }

        public override MemoryType MemoryType { get; }

        protected override IntPtr Allocate(long size)
        {
            return Native.HeapAlloc(CurrentHeap, 8, (IntPtr)size);
        }

        protected override bool CanGetNativeSize()
        {
            return true;
        }

        protected override SafePtrBase Clone()
        {
            var b = new SafePtr();
            var by = this.ToByteArray();
            b.FromByteArray(by);
            return b;
        }

        protected override void Deallocate(IntPtr ptr)
        {
            Native.HeapFree(CurrentHeap, 0, ptr);
        }

        protected override long GetNativeSize()
        {
            if ((long)handle == 0) return 0;
            return (long)Native.HeapSize(CurrentHeap, 0, handle);
        }

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