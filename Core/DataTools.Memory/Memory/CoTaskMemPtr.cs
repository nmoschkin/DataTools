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

        public CoTaskMemPtr() : base(IntPtr.Zero, true, false)
        {
        }

        public CoTaskMemPtr(int size) : base(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        public CoTaskMemPtr(IntPtr ptr) : base(ptr, true, false)
        {
            GetNativeSize();
        }

        public CoTaskMemPtr(IntPtr ptr, bool fOwn) : base(ptr, fOwn, false)
        {
            GetNativeSize();
        }

        public CoTaskMemPtr(IntPtr ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            GetNativeSize();
        }

        protected override void InternalDoZeroMem(IntPtr startptr, long length)
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

        protected override IntPtr Allocate(long size)
        {
            var r = Marshal.AllocCoTaskMem((int)size);
            if (r != IntPtr.Zero) this.size = size;
            return r;
        }

        protected override void Deallocate(IntPtr ptr)
        {
            Marshal.FreeCoTaskMem(ptr);
        }

        protected override IntPtr Reallocate(IntPtr oldptr, long newsize)
        {
            var r = Marshal.ReAllocCoTaskMem(oldptr, (int)newsize);
            if (r != IntPtr.Zero) size = newsize;
            return r;
        }

        protected override SafePtrBase Clone()
        {
            var cm = new CoTaskMemPtr();
            if (handle == IntPtr.Zero) return cm;

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

        public static implicit operator CoTaskMemPtr(IntPtr handle)
            => new CoTaskMemPtr(handle, true, false);

        public static implicit operator IntPtr(CoTaskMemPtr ptr) => ptr.handle;

        public static explicit operator CoTaskMemPtr(string str)
        {
            var cm = new CoTaskMemPtr(IntPtr.Zero, true, true);
            cm.Alloc(sizeof(char) * (str.Length + 1));
            cm.SetString(0, str);
            return cm;
        }
    }
}