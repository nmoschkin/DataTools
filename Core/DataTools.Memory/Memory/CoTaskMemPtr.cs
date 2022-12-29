using System;
using System.Runtime.InteropServices;

namespace DataTools.Memory
{
    /// <summary>
    /// Wraps a block of COM memory.
    /// </summary>
    public class CoTaskMemPtr : SafePtrBase
    {
        private long size;

        public CoTaskMemPtr() : base(nint.Zero, true, false)
        {
        }

        public CoTaskMemPtr(int size) : base(nint.Zero, true, true)
        {
            Alloc(size);
        }

        public CoTaskMemPtr(nint ptr) : base(ptr, true, false)
        {
            GetNativeSize();
        }

        public CoTaskMemPtr(nint ptr, bool fOwn) : base(ptr, fOwn, false)
        {
            GetNativeSize();
        }

        public CoTaskMemPtr(nint ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            GetNativeSize();
        }

        protected override void InternalDoZeroMem(nint startptr, long length)
        {
            unsafe
            {
                Native.ZeroMemory((void*)startptr, length);
            }
        }

        public override MemoryType MemoryType => MemoryType.CoTaskMem;

        protected override long GetNativeSize()
        {
            return GetLogicalSize();
        }

        protected override bool CanGetNativeSize()
        {
            return false;
        }

        protected override nint Allocate(long size)
        {
            var r = Marshal.AllocCoTaskMem((int)size);
            if (r != 0) this.size = size;
            return r;
        }

        protected override void Deallocate(nint ptr)
        {
            Marshal.FreeCoTaskMem(ptr);
        }

        protected override nint Reallocate(nint oldptr, long newsize)
        {
            var r = Marshal.ReAllocCoTaskMem(oldptr, (int)newsize);
            if (r != 0) size = newsize;
            return r;
        }

        protected override SafePtrBase Clone()
        {
            var cm = new CoTaskMemPtr();
            if (handle == nint.Zero) return cm;

            unsafe
            {
                var size = GetNativeSize();
                cm.Alloc(size);

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