//using DataTools.Memory;
//using DataTools.Win32.Memory;

//using System;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;

//namespace DataTools.Win32.Memory_old
//{
//    [Obsolete("This class is going away. The various memory types have been split off into their own classes. Use those or for generic purposes use SafePtr.")]
//    public class SafePtr : DataTools.Win32.Memory.SafePtrBase
//    {
//        private static nint procHeap = Native.GetProcessHeap();

//        private nint currentHeap = procHeap;

//        public override bool IsInvalid => handle == (nint)0;

//        private long buffLen;
//        private MemoryType memtype = MemoryType.HGlobal;

//        public override long Length
//        {
//            get => buffLen;
//            set
//            {
//                if (buffLen == value) return;

//                if (value == 0)
//                {
//                    TFree();
//                    return;
//                }
//                else if (handle == nint.Zero || MemoryType == MemoryType.HGlobal)
//                {
//                    ReAlloc(value);
//                }
//            }
//        }

//        public override MemoryType MemoryType => memtype;

//        internal new nint handle
//        {
//            get => base.handle;
//            set
//            {
//                if (base.handle == value) return;
//                base.handle = value;
//            }
//        }

//        public SafePtr(nint ptr, int size, MemoryType t, bool fOwn) : base((nint)0, fOwn, false)
//        {
//            base.handle = ptr;
//            buffLen = size;
//            memtype = t;
//        }

//        public SafePtr(nint ptr, long size) : base(ptr, false, false)
//        {
//            buffLen = size;
//        }

//        public SafePtr(nint ptr, int size) : this(ptr, (long)size)
//        {
//        }

//        public SafePtr(nint ptr) : this(ptr, 0)
//        {
//        }

//        public SafePtr(nint ptr, bool fOwn) : base(ptr, fOwn, false)
//        {
//        }

//        public SafePtr(nint ptr, long size, bool fOwn) : base(ptr, fOwn, false)
//        {
//            buffLen = size;
//        }

//        public SafePtr(nint ptr, int size, bool fOwn) : this(ptr, (long)size, fOwn)
//        {
//        }

//        public unsafe SafePtr(void* ptr, int size) : this((nint)ptr, (long)size, false)
//        {
//        }

//        public unsafe SafePtr(void* ptr, long size) : this((nint)ptr, size, false)
//        {
//        }

//        public unsafe SafePtr(void* ptr) : this((nint)ptr, 0)
//        {
//        }

//        public unsafe SafePtr(void* ptr, bool fOwn) : this((nint)ptr, fOwn)
//        {
//        }

//        public unsafe SafePtr(void* ptr, long size, bool fOwn) : this((nint)ptr, size, fOwn)
//        {
//        }

//        public unsafe SafePtr(void* ptr, int size, bool fOwn) : this((nint)ptr, size, fOwn)
//        {
//        }

//        public SafePtr() : base((nint)0, true, true)
//        {
//        }

//        public SafePtr(long size) : this()
//        {
//            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
//            Alloc(size);
//        }

//        public SafePtr(int size) : this()
//        {
//            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
//            Alloc(size);
//        }

//        public SafePtr(long size, MemoryType t) : this()
//        {
//            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

//            memtype = t;
//            TAlloc(size);
//        }

//        public SafePtr(int size, MemoryType t) : this()
//        {
//            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));

//            memtype = t;
//            if (size > 0) TAlloc(size);
//        }

//        /// <summary>
//        /// Allocate a block of memory on a heap (typically the process heap).
//        /// </summary>
//        /// <param name="size">The size to attempt to allocate</param>
//        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
//        /// <param name="hHeap">
//        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
//        /// If you use an alternate heap handle, you will need to free the memory using the same heap handle or an error will occur.
//        /// </param>
//        /// <param name="zeroMem">Whether or not to zero the contents of the memory on allocation.</param>
//        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
//        /// <remarks></remarks>
//        public bool Alloc(long size, bool addPressure = false, nint? hHeap = null, bool zeroMem = true)
//        {
//            if (handle != nint.Zero)
//            {
//                if (MemoryType == MemoryType.HGlobal)
//                    return ReAlloc(size);
//                else
//                    return false;
//            }

//            long l = buffLen;
//            bool al;

//            if (hHeap == null || (nint)hHeap == nint.Zero)
//                hHeap = currentHeap;

//            // While the function doesn't need to call HeapAlloc, it hasn't necessarily failed, either.
//            if (size == l) return true;

//            if (l > 0)
//            {
//                // we already have a pointer, so we will call realloc, instead.
//                return ReAlloc(size);
//            }

//            handle = Native.HeapAlloc((nint)hHeap, zeroMem ? 8u : 0, (nint)size);
//            al = handle != nint.Zero;

//            // see if we need to tell the garbage collector anything.
//            if (al)
//            {
//                if (addPressure) GC.AddMemoryPressure(size);
//                HasGCPressure = addPressure;

//                if (hHeap != null) currentHeap = (nint)hHeap;
//                memtype = MemoryType.HGlobal;

//                buffLen = (long)Native.HeapSize(currentHeap, 0, handle);
//            }

//            return al;
//        }

//        /// <summary>
//        /// Allocate a block of memory on the process heap.
//        /// </summary>
//        /// <param name="size">The size to attempt to allocate</param>
//        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
//        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
//        /// <remarks></remarks>
//        public bool Alloc(long size, bool addPressure)
//        {
//            return Alloc(size, addPressure, null, true);
//        }

//        /// <summary>
//        /// Allocate a block of memory on the process heap.
//        /// </summary>
//        /// <param name="size">The size to attempt to allocate</param>
//        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
//        /// <remarks></remarks>
//        public override bool Alloc(long size)
//        {
//            return Alloc(size, false, null, true);
//        }

//        /// <summary>
//        /// (Deprecated) Allocate a block of memory and set its contents to zero.
//        /// </summary>
//        /// <param name="size">The size to attempt to allocate</param>
//        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
//        /// <param name="hHeap">
//        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
//        /// If you use an alternate heap handle, you will need to free the memory using the same heap handle or an error will occur.
//        /// </param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public bool AllocZero(long size, bool addPressure = false, nint? hHeap = null)
//        {
//            return Alloc(size, addPressure, hHeap, true);
//        }

//        /// <summary>
//        /// Allocates memory aligned to a particular byte boundary.
//        /// Memory allocated in this way must be freed with AlignedFree()
//        /// </summary>
//        /// <param name="size">Size of the memory to allocate.</param>
//        /// <param name="alignment">The byte alignment of the memory.</param>
//        /// <param name="addPressure">Specify whether or not to add memory pressure to the garbage collector.</param>
//        /// <param name="hHeap">
//        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
//        /// If you use an alternate heap handle, you will need to free the memory using the same heap handle or an error will occur.
//        /// </param>
//        /// <returns></returns>
//        public bool AlignedAlloc(long size, long alignment = 512, bool addPressure = false, nint? hHeap = null)
//        {
//            if (handle != nint.Zero) return false;

//            if (alignment == 0 || (alignment & 1) != 0)
//                return false;

//            if (handle != nint.Zero)
//            {
//                if (!Free())
//                    return false;
//            }

//            long l = size + (alignment - 1) + 8;

//            if (hHeap == null || (nint)hHeap == nint.Zero)
//                hHeap = currentHeap;

//            if (l < 1)
//                return false;

//            nint p = Native.HeapAlloc((nint)hHeap, 8, (nint)l);

//            if (p == nint.Zero) return false;

//            nint p2 = (nint)((long)p + (alignment - 1) + 8);

//            if (p == nint.Zero)
//                return false;

//            p2 = (nint)((long)p2 - p2.ToInt64() % alignment);

//            DataTools.Win32.Memory.MemPtr mm = p2;

//            mm.LongAt(-1) = p.ToInt64();
//            handle = p2;

//            if (addPressure)
//                GC.AddMemoryPressure(l);

//            HasGCPressure = addPressure;

//            memtype = MemoryType.Aligned;
//            if (hHeap != null) currentHeap = (nint)hHeap;

//            buffLen = size;

//            return true;
//        }

//        protected override SafePtr Clone()
//        {
//            var p = new SafePtr();
//            p.memtype = memtype;
//            p.Length = Length;

//            CopyTo(p, 0, 0, Length);
//            return p;
//        }

//        public override long GetAllocatedSize()
//        {
//            return Length;
//        }

//        /// <summary>
//        /// Frees a previously allocated block of aligned memory.
//        /// </summary>
//        /// <param name="removePressure">Specify whether or not to remove memory pressure from the garbage collector.</param>
//        /// <param name="hHeap">
//        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
//        /// If you use an alternate heap handle, you will need to free the memory using the same heap handle or an error will occur.
//        /// </param>
//        /// <returns></returns>
//        public bool AlignedFree()
//        {
//            if (handle == nint.Zero)
//                return true;

//            if (MemoryType != MemoryType.HGlobal && MemoryType != MemoryType.Aligned) return false;

//            nint p = (nint)LongAt(-1);
//            long l = Convert.ToInt64(Native.HeapSize(currentHeap, 0, p));

//            if (Native.HeapFree(currentHeap, 0, p))
//            {
//                if (HasGCPressure) GC.RemoveMemoryPressure(l);

//                handle = nint.Zero;

//                HasGCPressure = false;
//                currentHeap = procHeap;
//                memtype = MemoryType.Invalid;
//                buffLen = 0;

//                return true;
//            }
//            else
//                return false;
//        }

//        /// <summary>
//        /// Reallocate a block of memory to a different size on the task heap.
//        /// </summary>
//        /// <param name="size">The size to attempt to allocate</param>
//        /// <param name="modifyPressure">Whether or not to call GC.AddMemoryPressure or GC.RemoveMemoryPressure.</param>
//        /// <param name="hHeap">
//        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
//        /// </param>
//        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
//        /// <remarks></remarks>
//        public override bool ReAlloc(long size)
//        {
//            if (handle == nint.Zero) return Alloc(size);

//            if (MemoryType != MemoryType.HGlobal) return false;

//            long l = buffLen;
//            bool ra;

//            // While the function doesn't need to call HeapReAlloc, it hasn't necessarily failed, either.
//            if (size == l) return true;

//            if (l <= 0)
//            {
//                // we don't have a pointer yet, so we have to call alloc instead.
//                return Alloc(size);
//            }

//            handle = Native.HeapReAlloc(currentHeap, 8, handle, new nint(size));
//            ra = handle != nint.Zero;

//            // see if we need to tell the garbage collector anything.
//            if (ra && HasGCPressure)
//            {
//                if (size < l)
//                    GC.RemoveMemoryPressure(l - size);
//                else
//                    GC.AddMemoryPressure(size - l);
//            }

//            buffLen = size;
//            return ra;
//        }

//        /// <summary>
//        /// Frees a previously allocated block of memory on the task heap.
//        /// </summary>
//        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
//        /// <param name="removePressure">Whether or not to call GC.RemoveMemoryPressure</param>
//        /// <param name="hHeap">
//        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
//        /// The handle pointed to by the internal pointer must have been previously allocated with the same heap handle.
//        /// </param>
//        /// <remarks></remarks>
//        public override bool Free()
//        {
//            long l = 0;

//            // While the function doesn't need to call HeapFree, it hasn't necessarily failed, either.
//            if (handle == nint.Zero)
//            {
//                return true;
//            }
//            else if (MemoryType != MemoryType.HGlobal)
//            {
//                return TFree();
//            }
//            else
//            {
//                // see if we need to tell the garbage collector anything.
//                if (HasGCPressure) l = buffLen;

//                var res = Native.HeapFree(currentHeap, 0, handle);

//                // see if we need to tell the garbage collector anything.
//                if (res)
//                {
//                    handle = nint.Zero;
//                    memtype = MemoryType.Invalid;

//                    currentHeap = procHeap;

//                    if (HasGCPressure) GC.RemoveMemoryPressure(l);

//                    memtype = MemoryType.Invalid;
//                    HasGCPressure = false;

//                    buffLen = 0;

//                    currentHeap = procHeap;
//                }

//                return res;
//            }
//        }

//        /// <summary>
//        /// Validates whether the pointer referenced by this structure
//        /// points to a valid and accessible block of memory.
//        /// </summary>
//        /// <returns>True if the memory block is valid, or False if the pointer is invalid or zero.</returns>
//        /// <remarks></remarks>
//        public bool Validate()
//        {
//            if (handle == nint.Zero || MemoryType != MemoryType.HGlobal && MemoryType != MemoryType.Aligned)
//            {
//                return false;
//            }

//            return Native.HeapValidate(currentHeap, 0, handle);
//        }

//        /// <summary>
//        /// Frees a previously allocated block of memory on the task heap with LocalFree()
//        /// </summary>
//        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
//        /// <remarks></remarks>
//        public bool LocalFree()
//        {
//            if (handle == nint.Zero)
//                return true;
//            else
//            {
//                Native.LocalFree(handle);

//                handle = nint.Zero;
//                memtype = MemoryType.Invalid;
//                HasGCPressure = false;
//                buffLen = 0;

//                return true;
//            }
//        }

//        /// <summary>
//        /// Frees a previously allocated block of memory on the task heap with GlobalFree()
//        /// </summary>
//        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
//        /// <remarks></remarks>
//        public bool GlobalFree()
//        {
//            if (handle == nint.Zero)
//                return false;
//            else
//            {
//                Native.GlobalFree(handle);

//                handle = nint.Zero;
//                memtype = MemoryType.Invalid;
//                HasGCPressure = false;

//                buffLen = 0;

//                return true;
//            }
//        }

//        // NetApi Memory functions should be used carefully and not within the context
//        // of any scenario when you may accidentally call normal memory management functions
//        // on any region of memory allocated with the network memory functions.
//        // Be mindful of usage.
//        // Some normal functions such as Length and SetLength cannot be used.
//        // Normal allocation and deallocation functions cannot be used, at all.
//        // NetApi memory is not reallocatable.
//        // The size of a NetApi memory buffer cannot be retrieved.

//        /// <summary>
//        /// Allocate a network API compatible memory buffer.
//        /// </summary>
//        /// <param name="size">Size of the buffer to allocate, in bytes.</param>
//        /// <remarks></remarks>
//        public bool NetAlloc(int size)
//        {
//            // just ignore an allocated buffer.
//            if (handle != nint.Zero)
//                return true;

//            int r = Native.NetApiBufferAllocate(size, out base.handle);

//            if (r == 0)
//            {
//                memtype = MemoryType.Network;
//                buffLen = size;
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Free a network API compatible memory buffer previously allocated with NetAlloc.
//        /// </summary>
//        /// <remarks></remarks>
//        public void NetFree()
//        {
//            if (handle == nint.Zero)
//                return;

//            Native.NetApiBufferFree(handle);
//            memtype = MemoryType.Invalid;
//            handle = nint.Zero;
//            buffLen = 0;
//        }

//        // Virtual Memory should be used carefully and not within the context
//        // of any scenario when you may accidentally call normal memory management functions
//        // on any region of memory allocated with the Virtual functions.
//        // Be mindful of usage.
//        // Some normal functions such as Length and SetLength cannot be used (use VirtualLength).
//        // Normal allocation and deallocation functions cannot be used, at all.
//        // Virtual memory is not reallocatable.

//        /// <summary>
//        /// Allocates a region of virtual memory.
//        /// </summary>
//        /// <param name="size">The size of the region of memory to allocate.</param>
//        /// <param name="addPressure">Whether to call GC.AddMemoryPressure</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public bool VirtualAlloc(long size, bool addPressure = true)
//        {
//            long l = 0;
//            bool va;

//            // While the function doesn't need to call VirtualAlloc, it hasn't necessarily failed, either.
//            if (size == l && handle != nint.Zero) return true;

//            handle = Native.VirtualAlloc(nint.Zero, (nint)size, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);

//            va = handle != nint.Zero;

//            buffLen = GetVirtualLength();

//            if (va && addPressure)
//                GC.AddMemoryPressure(buffLen);

//            HasGCPressure = addPressure;

//            return va;
//        }

//        public bool VirtualReAlloc(long size)
//        {
//            if (buffLen == size)
//            {
//                return true;
//            }
//            else if (buffLen == 0)
//            {
//                return VirtualAlloc(size, HasGCPressure);
//            }
//            else if (size <= 0)
//            {
//                throw new ArgumentOutOfRangeException(nameof(size));
//            }

//            var olds = buffLen;

//            var cpysize = olds > size ? size : olds;

//            var nhandle = Native.VirtualAlloc(nint.Zero, (nint)size, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);

//            if (nhandle == nint.Zero) return false;

//            Native.MemCpy(handle, nhandle, cpysize);
//            Native.VirtualFree(handle);

//            handle = nhandle;

//            if (HasGCPressure)
//            {
//                if (olds > size)
//                {
//                    GC.RemoveMemoryPressure(olds - size);
//                }
//                else
//                {
//                    GC.AddMemoryPressure(size - olds);
//                }
//            }

//            buffLen = size;
//            return true;
//        }

//        /// <summary>
//        /// Frees a region of memory previously allocated with VirtualAlloc.
//        /// </summary>
//        /// <param name="removePressure">Whether to call GC.RemoveMemoryPressure</param>
//        /// <returns></returns>
//        /// <remarks></remarks>
//        public bool VirtualFree()
//        {
//            long l = 0;
//            bool vf;

//            // While the function doesn't need to call vf, it hasn't necessarily failed, either.
//            if (handle == nint.Zero)
//                vf = true;
//            else
//            {
//                // see if we need to tell the garbage collector anything.
//                if (HasGCPressure)
//                    l = GetVirtualLength();

//                vf = Native.VirtualFree(handle);

//                // see if we need to tell the garbage collector anything.
//                if (vf)
//                {
//                    handle = nint.Zero;
//                    if (HasGCPressure)
//                        GC.RemoveMemoryPressure(l);

//                    HasGCPressure = false;
//                    memtype = MemoryType.Invalid;

//                    currentHeap = procHeap;
//                    buffLen = 0;
//                }
//            }

//            return vf;
//        }

//        /// <summary>
//        /// Returns the size of a region of virtual memory previously allocated with VirtualAlloc.
//        /// </summary>
//        /// <returns>The size of a virtual memory region or zero.</returns>
//        /// <remarks></remarks>
//        private long GetVirtualLength()
//        {
//            if (handle == nint.Zero)
//                return 0;

//            MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();

//            if (Native.VirtualQuery(handle, ref m, (nint)Marshal.SizeOf(m)) != nint.Zero)
//                return (long)m.RegionSize;

//            return 0;
//        }

//        public void FreeCoTaskMem()
//        {
//            buffLen = 0;
//            currentHeap = procHeap;
//            HasGCPressure = false;
//            Marshal.FreeCoTaskMem(handle);
//            handle = nint.Zero;
//        }

//        public void AllocCoTaskMem(int size)
//        {
//            handle = Marshal.AllocCoTaskMem(size);
//            if (handle != nint.Zero)
//            {
//                buffLen = size;
//                memtype = MemoryType.CoTaskMem;
//                currentHeap = procHeap;
//                HasGCPressure = false;
//            }
//        }

//        private void TAlloc(long size)
//        {
//            switch (MemoryType)
//            {
//                case MemoryType.HGlobal:
//                    Alloc(size, size > 1024);
//                    return;

//                case MemoryType.Aligned:
//                    AlignedAlloc(size, default, size > 1024);
//                    return;

//                case MemoryType.CoTaskMem:

//                    if ((size & 0x7fff_ffff_0000_0000L) != 0L) throw new ArgumentOutOfRangeException(nameof(size), "Size is too big for memory type.");
//                    AllocCoTaskMem((int)size);
//                    return;

//                case MemoryType.Virtual:
//                    VirtualAlloc(size, size > 1024);
//                    return;

//                case MemoryType.Network:
//                    if ((size & 0x7fff_ffff_0000_0000L) != 0L) throw new ArgumentOutOfRangeException(nameof(size), "Size is too big for memory type.");
//                    NetAlloc((int)size);
//                    return;

//                default:
//                    Alloc(size, size > 1024);
//                    return;
//            }
//        }

//        private bool TFree()
//        {
//            switch (MemoryType)
//            {
//                case MemoryType.HGlobal:
//                    return Free();

//                case MemoryType.Aligned:
//                    return AlignedFree();

//                case MemoryType.CoTaskMem:
//                    FreeCoTaskMem();
//                    return true;

//                case MemoryType.Virtual:
//                    return VirtualFree();

//                case MemoryType.Network:
//                    NetFree();
//                    return true;

//                default:
//                    return Free();
//            }
//        }

//        protected override bool ReleaseHandle()
//        {
//            TFree();
//            return true;
//        }

//        public override bool Equals(object obj)
//        {
//            return base.Equals(obj);
//        }

//        public override int GetHashCode()
//        {
//            return base.GetHashCode();
//        }

//        public static explicit operator byte[](SafePtr val)
//        {
//            return val.ToByteArray();
//        }

//        public static explicit operator SafePtr(byte[] val)
//        {
//            var n = new SafePtr();
//            n.FromByteArray(val);
//            return n;
//        }

//        public static explicit operator char[](SafePtr val)
//        {
//            return val.ToCharArray();
//        }

//        public static explicit operator SafePtr(char[] val)
//        {
//            var n = new SafePtr();
//            n.FromCharArray(val);
//            return n;
//        }

//        public static explicit operator sbyte[](SafePtr val)
//        {
//            return val.ToArray<sbyte>();
//        }

//        public static explicit operator SafePtr(sbyte[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator short[](SafePtr val)
//        {
//            return val.ToArray<short>();
//        }

//        public static explicit operator SafePtr(short[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator ushort[](SafePtr val)
//        {
//            return val.ToArray<ushort>();
//        }

//        public static explicit operator SafePtr(ushort[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator int[](SafePtr val)
//        {
//            return val.ToArray<int>();
//        }

//        public static explicit operator SafePtr(int[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator uint[](SafePtr val)
//        {
//            return val.ToArray<uint>();
//        }

//        public static explicit operator SafePtr(uint[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator long[](SafePtr val)
//        {
//            return val.ToArray<long>();
//        }

//        public static explicit operator SafePtr(long[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator ulong[](SafePtr val)
//        {
//            return val.ToArray<ulong>();
//        }

//        public static explicit operator SafePtr(ulong[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator float[](SafePtr val)
//        {
//            return val.ToArray<float>();
//        }

//        public static explicit operator SafePtr(float[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator double[](SafePtr val)
//        {
//            return val.ToArray<double>();
//        }

//        public static explicit operator SafePtr(double[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator decimal[](SafePtr val)
//        {
//            return val.ToArray<decimal>();
//        }

//        public static explicit operator SafePtr(decimal[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator DateTime[](SafePtr val)
//        {
//            return val.ToArray<DateTime>();
//        }

//        public static explicit operator SafePtr(DateTime[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        public static explicit operator Guid[](SafePtr val)
//        {
//            return val.ToArray<Guid>();
//        }

//        public static explicit operator SafePtr(Guid[] val)
//        {
//            var n = new SafePtr();
//            n.FromArray(val);
//            return n;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static explicit operator string(SafePtr val)
//        {
//            if (val?.handle == (nint)0) return null;
//            return val.GetString(0);
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static explicit operator SafePtr(string val)
//        {
//            var op = new SafePtr((val.Length + 1) * sizeof(char));
//            op.SetString(0, val);
//            return op;
//        }

//        public static SafePtr operator +(SafePtr val1, byte[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length);
//            val1.FromByteArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, char[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(char));
//            val1.FromCharArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, string val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(char));
//            val1.FromCharArray(val2.ToCharArray(), c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, sbyte[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length);
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, short[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(short));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, ushort[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(ushort));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, int[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(int));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, uint[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(uint));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, long[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(long));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, ulong[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(ulong));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, float[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(float));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, double[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(double));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, decimal[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * sizeof(decimal));
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, DateTime[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * Marshal.SizeOf<DateTime>());
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, Guid[] val2)
//        {
//            var c = val1.buffLen;

//            val1.Alloc(val1.buffLen + val2.Length * Marshal.SizeOf<Guid>());
//            val1.FromArray(val2, c);

//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, short val2)
//        {
//            val1.handle += val2;
//            return val1;
//        }

//        public static SafePtr operator -(SafePtr val1, short val2)
//        {
//            val1.handle -= val2;
//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, ushort val2)
//        {
//            val1.handle += val2;
//            return val1;
//        }

//        public static SafePtr operator -(SafePtr val1, ushort val2)
//        {
//            val1.handle -= val2;
//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, int val2)
//        {
//            val1.handle += val2;
//            return val1;
//        }

//        public static SafePtr operator -(SafePtr val1, int val2)
//        {
//            val1.handle -= val2;
//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, long val2)
//        {
//            val1.handle = (nint)((long)val1.handle + val2);
//            return val1;
//        }

//        public static SafePtr operator -(SafePtr val1, long val2)
//        {
//            val1.handle = (nint)((long)val1.handle - val2);
//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, uint val2)
//        {
//            val1.handle = (nint)((uint)val1.handle + val2);
//            return val1;
//        }

//        public static SafePtr operator -(SafePtr val1, uint val2)
//        {
//            val1.handle = (nint)((uint)val1.handle - val2);
//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, ulong val2)
//        {
//            val1.handle = (nint)((ulong)val1.handle + val2);
//            return val1;
//        }

//        public static SafePtr operator -(SafePtr val1, ulong val2)
//        {
//            val1.handle = (nint)((ulong)val1.handle - val2);
//            return val1;
//        }

//        public static SafePtr operator +(SafePtr val1, nint val2)
//        {
//            val1.handle = (nint)((long)val1.handle + (long)val2);
//            return val1;
//        }

//        public static SafePtr operator -(SafePtr val1, nint val2)
//        {
//            val1.handle = (nint)((long)val1.handle - (long)val2);
//            return val1;
//        }

//        public static bool operator ==(nint val1, SafePtr val2)
//        {
//            return val1 == (val2?.handle ?? nint.Zero);
//        }

//        public static bool operator !=(nint val1, SafePtr val2)
//        {
//            return val1 != (val2?.handle ?? nint.Zero);
//        }

//        public static bool operator ==(SafePtr val2, nint val1)
//        {
//            return val1 == (val2?.handle ?? nint.Zero);
//        }

//        public static bool operator !=(SafePtr val2, nint val1)
//        {
//            return val1 != (val2?.handle ?? nint.Zero);
//        }

//        public static implicit operator nint(SafePtr val)
//        {
//            return val?.handle ?? nint.Zero;
//        }

//        public static implicit operator SafePtr(nint val)
//        {
//            unsafe
//            {
//                return new SafePtr
//                {
//                    handle = (nint)(void*)val
//                };
//            }
//        }

//        public static implicit operator UIntPtr(SafePtr val)
//        {
//            unsafe
//            {
//                return (UIntPtr)(void*)val.handle;
//            }
//        }

//        public static implicit operator SafePtr(UIntPtr val)
//        {
//            unsafe
//            {
//                return new SafePtr
//                {
//                    handle = (nint)(void*)val
//                };
//            }
//        }
//    }
//}