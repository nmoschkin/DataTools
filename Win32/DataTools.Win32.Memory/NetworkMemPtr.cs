using DataTools.Memory;
using DataTools.Win32.Memory;

using System;

namespace DataTools
{
    internal class NetworkMemPtr : SafePtrBase
    {
        private long size = 0;

        public NetworkMemPtr() : base(nint.Zero, true, false)
        {
        }

        public NetworkMemPtr(int size) : base(nint.Zero, true, true)
        {
            Alloc(size);
        }

        public NetworkMemPtr(nint ptr) : base(ptr, true, false)
        {
            GetAllocatedSize();
        }

        public NetworkMemPtr(nint ptr, bool fOwn) : base(ptr, fOwn, false)
        {
            GetAllocatedSize();
        }

        public NetworkMemPtr(nint ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
            GetAllocatedSize();
        }

        public override MemoryType MemoryType { get; }

        public override bool Alloc(long size)
        {
            if (handle != 0) return ReAlloc(size);

            if (size > int.MaxValue) throw new NotSupportedException("CoTaskMem only supports 32-bit integer buffer lengths.");

            if (handle != nint.Zero) return ReAlloc(size);

            int r = Native.NetApiBufferAllocate((int)size, out base.handle);

            if (r == 0)
            {
                GetAllocatedSize();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Free()
        {
            try
            {
                if (handle == nint.Zero) return false;

                Native.NetApiBufferFree(handle);
                handle = nint.Zero;
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

                Native.NetApiBufferSize(handle, out int size);
                this.size = size;
            }
            catch
            {
            }

            return this.size;
        }

        public override bool ReAlloc(long size)
        {
            if (handle == nint.Zero) return Alloc(size);
            else if (size <= 0) return Free();
            else if (this.size == size) return true;

            var r = Native.NetApiBufferReallocate(handle, (int)size, out var nhandle);

            if (r != 0) return false;

            var oldsize = this.size;

            handle = nhandle;

            if (HasGCPressure)
            {
                if (oldsize > size)
                {
                    GC.RemoveMemoryPressure(oldsize - size);
                }
                else
                {
                    GC.AddMemoryPressure(size - oldsize);
                }
            }

            GetAllocatedSize();
            return true;
        }

        protected override SafePtrBase Clone()
        {
            var cm = new NetworkMemPtr();
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

        public static implicit operator NetworkMemPtr(nint handle)
            => new NetworkMemPtr(handle, true, false);

        public static implicit operator nint(NetworkMemPtr ptr) => ptr.handle;
    }
}