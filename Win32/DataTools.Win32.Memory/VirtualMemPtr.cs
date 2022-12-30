using DataTools.Memory;
using DataTools.Win32.Memory;

using System;
using System.Runtime.InteropServices;

namespace DataTools
{
    public class VirtualMemPtr : DataTools.Win32.Memory.WinPtrBase
    {
        public VirtualMemPtr() : base(IntPtr.Zero, true, true)
        {
        }

        public VirtualMemPtr(int size) : base(IntPtr.Zero, true, true)
        {
            Alloc(size);
        }

        public override MemoryType MemoryType => MemoryType.Virtual;

        protected override IntPtr Allocate(long size)
        {
            return Native.VirtualAlloc(IntPtr.Zero, (IntPtr)size, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);
        }

        protected override void Deallocate(IntPtr ptr)
        {
            Native.VirtualFree(ptr);
        }

        protected override IntPtr Reallocate(IntPtr oldptr, long newsize)
        {
            var olds = GetNativeSize();

            var cpysize = olds > newsize ? newsize : olds;

            var nhandle = Native.VirtualAlloc(IntPtr.Zero, (IntPtr)newsize, VMemAllocFlags.MEM_COMMIT | VMemAllocFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);

            if (nhandle == IntPtr.Zero) return IntPtr.Zero;

            Native.MemCpy(oldptr, nhandle, cpysize);
            Native.VirtualFree(oldptr);

            return nhandle;
        }

        protected override long GetNativeSize()
        {
            if (handle == IntPtr.Zero) return 0;

            MEMORY_BASIC_INFORMATION m = new MEMORY_BASIC_INFORMATION();

            if (Native.VirtualQuery(handle, ref m, (IntPtr)Marshal.SizeOf(m)) != IntPtr.Zero) return (long)m.RegionSize;

            return 0;
        }

        protected override bool CanGetNativeSize()
        {
            return true;
        }

        protected override SafePtrBase Clone()
        {
            var cm = new VirtualMemPtr();
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
    }
}