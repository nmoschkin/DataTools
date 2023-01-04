using DataTools.Memory;

using System;
using System.Collections.Generic;
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
}