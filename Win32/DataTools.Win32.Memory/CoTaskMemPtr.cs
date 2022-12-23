using DataTools.Memory;
using DataTools.Win32.Memory;

using System;
using System.Runtime.InteropServices;

namespace DataTools
{
    /// <summary>
    /// Wraps a block of COM memory.
    /// </summary>
    public sealed class CoTaskMemPtr : SafePtrBase
    {
        private long size = 0;

        public CoTaskMemPtr() : base(nint.Zero, true, false)
        {
        }

        public CoTaskMemPtr(int size) : base(nint.Zero, true, true)
        {
            Alloc(size);
        }

        public CoTaskMemPtr(nint ptr) : base(ptr, true, false)
        {
            GetAllocatedSize();
        }

        public CoTaskMemPtr(nint ptr, bool fOwn) : base(ptr, fOwn, false)
        {
            GetAllocatedSize();
        }

        public CoTaskMemPtr(nint ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            GetAllocatedSize();
        }

        public override MemoryType MemoryType => MemoryType.CoTaskMem;

        public override bool Alloc(long size)
        {
            if (handle != 0) return ReAlloc(size);

            if (size > int.MaxValue) throw new NotSupportedException("CoTaskMem only supports 32-bit integer buffer lengths.");
            try
            {
                Handle = Marshal.AllocCoTaskMem((int)size);
                this.size = size;
                GetAllocatedSize();
                if (HasGCPressure) GC.AddMemoryPressure(size);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Free()
        {
            if (handle == nint.Zero) return false;
            if (!IsOwner) return false;

            try
            {
                Marshal.FreeCoTaskMem(handle);
                if (HasGCPressure) GC.RemoveMemoryPressure(size);
                size = 0;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override long GetAllocatedSize()
        {
            try
            {
                if (handle == 0) return 0;
                size = Native.HeapSize(ProcessHeap, 0, handle);
            }
            catch
            {
            }

            return size;
        }

        public override bool ReAlloc(long size)
        {
            if (handle == nint.Zero) return Alloc(size);
            else if (size <= 0) return Free();
            else if (this.size == size) return true;

            try
            {
                var oldsize = this.size;

                Handle = Marshal.ReAllocCoTaskMem(handle, (int)size);
                this.size = size;
                GetAllocatedSize();

                if (HasGCPressure)
                {
                    if (oldsize < size)
                    {
                        GC.AddMemoryPressure(size - oldsize);
                    }
                    else if (oldsize > size)
                    {
                        GC.RemoveMemoryPressure(oldsize - size);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override SafePtrBase Clone()
        {
            var cm = new CoTaskMemPtr();
            if (handle == nint.Zero) return cm;

            unsafe
            {
                cm.Alloc(GetAllocatedSize());

                void* ptr1 = (void*)handle;
                void* ptr2 = (void*)cm.handle;

                Buffer.MemoryCopy(ptr1, ptr2, size, size);
                return cm;
            }
        }

        public static implicit operator CoTaskMemPtr(nint handle)
            => new CoTaskMemPtr(handle, true, false);

        public static implicit operator nint(CoTaskMemPtr ptr) => ptr.handle;

        public static explicit operator CoTaskMemPtr(string str)
        {
            var cm = new CoTaskMemPtr(nint.Zero, true, true);
            cm.Alloc(sizeof(char) * (str.Length + 1));
            cm.SetString(0, str);
            return cm;
        }
    }
}