using DataTools.Memory;
using DataTools.Win32.Memory;

using System;

namespace DataTools
{
    internal class NetworkMemPtr : DataTools.Win32.Memory.WinPtrBase
    {
        public NetworkMemPtr() : base(nint.Zero, true, false)
        {
        }

        public NetworkMemPtr(int size) : base(nint.Zero, true, true)
        {
            Alloc(size);
        }

        public NetworkMemPtr(nint ptr) : base(ptr, true, false)
        {
            GetNativeSize();
        }

        public NetworkMemPtr(nint ptr, bool fOwn) : base(ptr, fOwn, false)
        {
            GetNativeSize();
        }

        public NetworkMemPtr(nint ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            GetNativeSize();
        }

        public override MemoryType MemoryType { get; }

        protected override nint Allocate(long size)
        {
            var r = Native.NetApiBufferAllocate((int)size, out var nhandle);
            if (r != 0) return nhandle;
            return 0;
        }

        protected override void Deallocate(nint ptr)
        {
            Native.NetApiBufferFree(ptr);
        }

        public override long GetNativeSize()
        {
            Native.NetApiBufferSize(handle, out int size);
            return size;
        }

        protected override nint Reallocate(nint oldptr, long newsize)
        {
            var r = Native.NetApiBufferReallocate(oldptr, (int)newsize, out var nhandle);

            if (r != 0) return nhandle;
            return 0;
        }

        protected override WinPtrBase Clone()
        {
            var cm = new NetworkMemPtr();
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

        public static implicit operator NetworkMemPtr(nint handle)
            => new NetworkMemPtr(handle, true, false);

        public static implicit operator nint(NetworkMemPtr ptr) => ptr.handle;
    }
}