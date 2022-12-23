using DataTools.Memory;
using DataTools.Win32.Memory;

using System;
using System.Runtime.InteropServices;

namespace DataTools
{
    public class VirtualMemPtr : SafePtrBase
    {
        private long size = 0;

        public VirtualMemPtr() : base(nint.Zero, true, true)
        {
        }

        public VirtualMemPtr(int size) : base(nint.Zero, true, true)
        {
            Alloc(size);
        }

        public override MemoryType MemoryType => MemoryType.Virtual;

        public override bool Alloc(long size)
        {
            long l = 0;
            bool va;

            // While the function doesn't need to call VirtualAlloc, it hasn't necessarily failed, either.
            if (size == l && handle != nint.Zero) return true;

            handle = Native.VirtualAlloc(nint.Zero, (nint)size, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);

            va = handle != nint.Zero;

            size = GetAllocatedSize();

            if (va && HasGCPressure) GC.AddMemoryPressure(size);

            return va;
        }

        public override bool Free()
        {
            long l = 0;
            bool vf;

            // While the function doesn't need to call vf, it hasn't necessarily failed, either.
            if (handle == nint.Zero)
                vf = true;
            else
            {
                // see if we need to tell the garbage collector anything.

                if (HasGCPressure) l = GetAllocatedSize();
                vf = Native.VirtualFree(handle);

                // see if we need to tell the garbage collector anything.
                if (vf)
                {
                    handle = nint.Zero;

                    if (HasGCPressure)
                    {
                        GC.RemoveMemoryPressure(l);
                    }

                    size = 0;
                }
            }

            return vf;
        }

        public override long GetAllocatedSize()
        {
            if (handle == nint.Zero) return 0;

            MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();

            if (Native.VirtualQuery(handle, ref m, (nint)Marshal.SizeOf(m)) != nint.Zero) return (long)m.RegionSize;

            return 0;
        }

        public override bool ReAlloc(long size)
        {
            if (this.size == size)
            {
                return true;
            }
            else if (size == 0)
            {
                return Alloc(size);
            }
            else if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            var olds = size;

            var cpysize = olds > size ? size : olds;

            var nhandle = Native.VirtualAlloc(nint.Zero, (nint)size, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);

            if (nhandle == nint.Zero) return false;

            Native.MemCpy(handle, nhandle, cpysize);
            Native.VirtualFree(handle);

            handle = nhandle;

            if (HasGCPressure)
            {
                if (olds > size)
                {
                    GC.RemoveMemoryPressure(olds - size);
                }
                else
                {
                    GC.AddMemoryPressure(size - olds);
                }
            }

            this.size = size;
            return true;
        }

        protected override SafePtrBase Clone()
        {
            var cm = new VirtualMemPtr();
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
    }
}