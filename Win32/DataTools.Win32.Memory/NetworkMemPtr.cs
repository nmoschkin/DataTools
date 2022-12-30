using DataTools.Memory;
using DataTools.Win32.Memory;

using System;

namespace DataTools
{
    internal class NetworkMemPtr : DataTools.Win32.Memory.WinPtrBase
    {
        public NetworkMemPtr() : base(IntPtr.Zero, true, false)
        {
        }

        public NetworkMemPtr(int size) : base(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        public NetworkMemPtr(IntPtr ptr) : base(ptr, true, false)
        {
            GetNativeSize();
        }

        public NetworkMemPtr(IntPtr ptr, bool fOwn) : base(ptr, fOwn, false)
        {
            GetNativeSize();
        }

        public NetworkMemPtr(IntPtr ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            GetNativeSize();
        }

        public override MemoryType MemoryType { get; }

        protected override IntPtr Allocate(long size)
        {
            var r = Native.NetApiBufferAllocate((int)size, out var nhandle);
            if (r != 0) return nhandle;
            return 0;
        }

        protected override void Deallocate(IntPtr ptr)
        {
            Native.NetApiBufferFree(ptr);
        }

        protected override long GetNativeSize()
        {
            Native.NetApiBufferSize(handle, out int size);
            return size;
        }

        protected override bool CanGetNativeSize()
        {
            return true;
        }

        protected override IntPtr Reallocate(IntPtr oldptr, long newsize)
        {
            var r = Native.NetApiBufferReallocate(oldptr, (int)newsize, out var nhandle);

            if (r != 0) return nhandle;
            return 0;
        }

        protected override WinPtrBase Clone()
        {
            var cm = new NetworkMemPtr();
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

        public static implicit operator NetworkMemPtr(IntPtr handle)
            => new NetworkMemPtr(handle, true, false);

        public static implicit operator IntPtr(NetworkMemPtr ptr) => ptr.handle;

        public static explicit operator NetworkMemPtr(DataTools.Memory.MemPtr handle)
            => new NetworkMemPtr(handle, true, false);

        public static explicit operator NetworkMemPtr(DataTools.Win32.Memory.MemPtr handle)
            => new NetworkMemPtr(handle, true, false);

        public static explicit operator DataTools.Memory.MemPtr(NetworkMemPtr ptr) => new DataTools.Memory.MemPtr(ptr.handle);

        public static explicit operator DataTools.Win32.Memory.MemPtr(NetworkMemPtr ptr) => new DataTools.Win32.Memory.MemPtr(ptr.handle);
    }
}