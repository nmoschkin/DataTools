using DataTools.Memory;
using DataTools.Win32.Memory;

using System;
using System.Runtime.InteropServices;

namespace DataTools
{
    public class VirtualMemPtr : DataTools.Win32.Memory.WinPtrBase
    {
        public VirtualMemPtr() : base(nint.Zero, true, true)
        {
        }

        public VirtualMemPtr(int size) : base(nint.Zero, true, true)
        {
            Alloc(size);
        }

        public override MemoryType MemoryType => MemoryType.Virtual;

        protected override nint Allocate(long size)
        {
            return Native.VirtualAlloc(nint.Zero, (nint)size, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);
        }

        protected override void Deallocate(nint ptr)
        {
            Native.VirtualFree(ptr);
        }

        protected override nint Reallocate(nint oldptr, long newsize)
        {
            var olds = GetAllocatedSize();

            var cpysize = olds > newsize ? newsize : olds;

            var nhandle = Native.VirtualAlloc(nint.Zero, (nint)newsize, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);

            if (nhandle == nint.Zero) return 0;

            Native.MemCpy(oldptr, nhandle, cpysize);
            Native.VirtualFree(oldptr);

            return nhandle;
        }

        public override long GetAllocatedSize()
        {
            if (handle == nint.Zero) return 0;

            MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();

            if (Native.VirtualQuery(handle, ref m, (nint)Marshal.SizeOf(m)) != nint.Zero) return (long)m.RegionSize;

            return 0;
        }

        protected override DataTools.Win32.Memory.WinPtrBase Clone()
        {
            var cm = new VirtualMemPtr();
            if (handle == nint.Zero) return cm;

            unsafe
            {
                var size = GetAllocatedSize();
                cm.Alloc(size);

                void* ptr1 = (void*)handle;
                void* ptr2 = (void*)cm.handle;
                Buffer.MemoryCopy(ptr1, ptr2, size, size);
                return cm;
            }
        }
    }
}