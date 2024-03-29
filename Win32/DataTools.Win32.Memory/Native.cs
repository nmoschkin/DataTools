﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Memory
{
    [Flags]
    internal enum HeapWalkFlags : short
    {
        /// <summary>
        /// The heap element is an allocated block.
        /// <br /><br />
        /// If PROCESS_HEAP_ENTRY_MOVEABLE is also specified, the Block structure becomes valid. The hMem member of the Block structure contains a handle to the allocated, moveable memory block.
        /// </summary>
        PROCESS_HEAP_ENTRY_BUSY = 0x0004,

        /// <summary>
        /// This value must be used with PROCESS_HEAP_ENTRY_BUSY, indicating that the heap element is an allocated block.
        /// </summary>
        PROCESS_HEAP_ENTRY_DDESHARE = 0x0020,

        /// <summary>
        /// This value must be used with PROCESS_HEAP_ENTRY_BUSY, indicating that the heap element is an allocated block.
        /// <br /><br />
        /// The block was allocated with LMEM_MOVEABLE or GMEM_MOVEABLE, and the Block structure becomes valid. The hMem member of the Block structure contains a handle to the allocated, moveable memory block.
        /// </summary>
        PROCESS_HEAP_ENTRY_MOVEABLE = 0x0010,

        /// <summary>
        /// The heap element is located at the beginning of a region of contiguous virtual memory in use by the heap.
        /// <br /><br />
        /// The lpData member of the structure points to the first virtual address used by the region; the cbData member specifies the total size, in bytes, of the address space that is reserved for this region; and the cbOverhead member specifies the size, in bytes, of the heap control structures that describe the region.
        /// <br /><br />
        /// The Region structure becomes valid. The dwCommittedSize, dwUnCommittedSize, lpFirstBlock, and lpLastBlock members of the structure contain additional information about the region.
        /// </summary>
        PROCESS_HEAP_REGION = 0x0001,

        /// <summary>
        /// The heap element is located in a range of uncommitted memory within the heap region.
        /// <br /><br />
        /// The lpData member points to the beginning of the range of uncommitted memory; the cbData member specifies the size, in bytes, of the range of uncommitted memory; and the cbOverhead member specifies the size, in bytes, of the control structures that describe this uncommitted range.
        /// </summary>
        PROCESS_HEAP_UNCOMMITTED_RANGE = 0x0002,
    }

    [Flags]
    internal enum MemoryTypes
    {
        /// <summary>
        /// Indicates that the memory pages within the region are mapped into the view of an image section.
        ///</summary>
        [Description(" Indicates that the memory pages within the region are mapped into the view of an image section.")]
        MEM_IMAGE = 0x1000000,

        /// <summary>
        /// Indicates that the memory pages within the region are mapped into the view of a section.
        ///</summary>
        [Description(" Indicates that the memory pages within the region are mapped into the view of a section.")]
        MEM_MAPPED = 0x40000,

        /// <summary>
        /// Indicates that the memory pages within the region are private (that is, not shared by other processes
        ///</summary>
        [Description(" Indicates that the memory pages within the region are private (that is, not shared by other processes")]
        MEM_PRIVATE = 0x20000
    }

    [Flags]
    internal enum MemoryStates
    {
        /// <summary>
        /// Indicates committed pages for which physical storage has been allocated, either in memory or in the paging file on disk.
        ///</summary>
        [Description(" Indicates committed pages for which physical storage has been allocated, either in memory or in the paging file on disk.")]
        MEM_COMMIT = 0x1000,

        /// <summary>
        /// Indicates free pages not accessible to the calling process and available to be allocated. For free pages, the information in the AllocationBase, AllocationProtect, Protect, and Type members is undefined.
        ///</summary>
        [Description(" Indicates free pages not accessible to the calling process and available to be allocated. For free pages, the information in the AllocationBase, AllocationProtect, Protect, and Type members is undefined.")]
        MEM_FREE = 0x10000,

        /// <summary>
        /// Indicates reserved pages where a range of the process's virtual address space is reserved without any physical storage
        ///</summary>
        [Description(" Indicates reserved pages where a range of the process's virtual address space is reserved without any physical storage ")]
        MEM_RESERVE = 0x2000
    }

    [Flags]
    internal enum MemoryProtectionFlags
    {
        /// <summary>
        ///Enables execute access to the committed region of pages. An attempt to read from or write to the committed region results in an access violation.
        ///</summary>
        [Description("Enables execute access to the committed region of pages. An attempt to read from or write to the committed region results in an access violation.")]
        PAGE_EXECUTE = 0x10,

        /// <summary>
        ///Enables execute or read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation.
        ///</summary>
        [Description("Enables execute or read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation.")]
        PAGE_EXECUTE_READ = 0x20,

        /// <summary>
        ///Enables execute, read-only, or read/write access to the committed region of pages.
        ///</summary>
        [Description("Enables execute, read-only, or read/write access to the committed region of pages.")]
        PAGE_EXECUTE_READWRITE = 0x40,

        /// <summary>
        ///Enables execute, read-only, or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write page results in a private copy of the page being made for the process. The private page is marked as PAGE_EXECUTE_READWRITE, and the change is written to the new page.
        ///</summary>
        [Description("Enables execute, read-only, or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write page results in a private copy of the page being made for the process. The private page is marked as PAGE_EXECUTE_READWRITE, and the change is written to the new page.")]
        PAGE_EXECUTE_WRITECOPY = 0x80,

        /// <summary>
        ///Disables all access to the committed region of pages. An attempt to read from, write to, or execute the committed region results in an access violation.
        ///</summary>
        [Description("Disables all access to the committed region of pages. An attempt to read from, write to, or execute the committed region results in an access violation.")]
        PAGE_NOACCESS = 0x1,

        /// <summary>
        ///Enables read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation. If Data Execution Prevention is enabled, an attempt to execute code in the committed region results in an access violation.
        ///</summary>
        [Description("Enables read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation. If Data Execution Prevention is enabled, an attempt to execute code in the committed region results in an access violation.")]
        PAGE_READONLY = 0x2,

        /// <summary>
        ///Enables read-only or read/write access to the committed region of pages. If Data Execution Prevention is enabled, attempting to execute code in the committed region results in an access violation.
        ///</summary>
        [Description("Enables read-only or read/write access to the committed region of pages. If Data Execution Prevention is enabled, attempting to execute code in the committed region results in an access violation.")]
        PAGE_READWRITE = 0x4,

        /// <summary>
        ///Enables read-only or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write page results in a private copy of the page being made for the process. The private page is marked as PAGE_READWRITE, and the change is written to the new page. If Data Execution Prevention is enabled, attempting to execute code in the committed region results in an access violation.
        ///</summary>
        [Description("Enables read-only or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write page results in a private copy of the page being made for the process. The private page is marked as PAGE_READWRITE, and the change is written to the new page. If Data Execution Prevention is enabled, attempting to execute code in the committed region results in an access violation.")]
        PAGE_WRITECOPY = 0x8,

        /// <summary>
        ///Pages in the region become guard pages. Any attempt to access a guard page causes the system to raise a STATUS_GUARD_PAGE_VIOLATION exception and turn off the guard page status. Guard pages thus act as a one-time access alarm. For more information, see Creating Guard Pages.
        ///</summary>
        [Description("Pages in the region become guard pages. Any attempt to access a guard page causes the system to raise a STATUS_GUARD_PAGE_VIOLATION exception and turn off the guard page status. Guard pages thus act as a one-time access alarm. For more information, see Creating Guard Pages.")]
        PAGE_GUARD = 0x100,

        /// <summary>
        ///Sets all pages to be non-cachable. Applications should not use this attribute except when explicitly required for a device. Using the interlocked functions with memory that is mapped with SEC_NOCACHE can result in an EXCEPTION_ILLEGAL_INSTRUCTION exception.
        ///</summary>
        [Description("Sets all pages to be non-cachable. Applications should not use this attribute except when explicitly required for a device. Using the interlocked functions with memory that is mapped with SEC_NOCACHE can result in an EXCEPTION_ILLEGAL_INSTRUCTION exception.")]
        PAGE_NOCACHE = 0x200,

        /// <summary>
        ///Sets all pages to be write-combined.
        ///</summary>
        [Description("Sets all pages to be write-combined.")]
        PAGE_WRITECOMBINE = 0x400
    }

    [Flags]
    internal enum VMemFreeFlags
    {
        /// <summary>
        ///Decommits the specified region of committed pages. After the operation, the pages are in the reserved state.
        ///The function does not fail if you attempt to decommit an uncommitted page. This means that you can decommit a range of pages without first determining the current commitment state.
        ///Do not use this value with MEM_RELEASE.
        ///</summary>
        ///<remarks></remarks>
        [Description("Decommits the specified region of committed pages. After the operation, the pages are in the reserved state.")]
        MEM_DECOMMIT = 0x4000,

        /// <summary>
        ///Releases the specified region of pages. After this operation, the pages are in the free state.
        ///</summary>
        ///<remarks></remarks>
        [Description("Releases the specified region of pages. After this operation, the pages are in the free state.")]
        MEM_RELEASE = 0x8000
    }

    [Flags]
    internal enum VMemAllocFlags
    {
        /// <summary>
        ///Allocates memory charges (from the overall size of memory and the paging files on disk) for the specified reserved memory pages. The function also guarantees that when the caller later initially accesses the memory, the contents will be zero. Actual physical pages are not allocated unless/until the virtual addresses are actually accessed.
        ///</summary>
        [Description("Allocates memory charges (from the overall size of memory and the paging files on disk) for the specified reserved memory pages. The function also guarantees that when the caller later initially accesses the memory, the contents will be zero. Actual physical pages are not allocated unless/until the virtual addresses are actually accessed.")]
        MEM_COMMIT = 0x1000,

        /// <summary>
        ///Reserves a range of the process's virtual address space without allocating any actual physical storage in memory or in the paging file on disk.
        ///</summary>
        [Description("Reserves a range of the process's virtual address space without allocating any actual physical storage in memory or in the paging file on disk.")]
        MEM_RESERVE = 0x2000,

        /// <summary>
        ///Indicates that data in the memory range specified by lpAddress and dwSize is no longer of interest. The pages should not be read from or written to the paging file. However, the memory block will be used again later, so it should not be decommitted. This value cannot be used with any other value.
        ///</summary>
        [Description("Indicates that data in the memory range specified by lpAddress and dwSize is no longer of interest. The pages should not be read from or written to the paging file. However, the memory block will be used again later, so it should not be decommitted. This value cannot be used with any other value.")]
        MEM_RESET = 0x80000,

        /// <summary>
        ///MEM_RESET_UNDO should only be called on an address range to which MEM_RESET was successfully applied earlier. It indicates that the data in the specified memory range specified by lpAddress and dwSize is of interest to the caller and attempts to reverse the effects of MEM_RESET. If the function succeeds, that means all data in the specified address range is intact. If the function fails, at least some of the data in the address range has been replaced with zeroes.
        ///</summary>
        [Description("MEM_RESET_UNDO should only be called on an address range to which MEM_RESET was successfully applied earlier. It indicates that the data in the specified memory range specified by lpAddress and dwSize is of interest to the caller and attempts to reverse the effects of MEM_RESET. If the function succeeds, that means all data in the specified address range is intact. If the function fails, at least some of the data in the address range has been replaced with zeroes.")]
        MEM_RESET_UNDO = 0x1000000,

        /// <summary>
        ///Allocates memory using large page support. The size and alignment must be a multiple of the large-page minimum. To obtain this value, use the GetLargePageMinimum function.
        ///</summary>
        [Description("Allocates memory using large page support. The size and alignment must be a multiple of the large-page minimum. To obtain this value, use the GetLargePageMinimum function.")]
        MEM_LARGE_PAGES = 0x20000000,

        /// <summary>
        ///Reserves an address range that can be used to map Address Windowing Extensions (AWE) pages.
        ///This value must be used with MEM_RESERVE and no other values.
        ///</summary>
        [Description("Reserves an address range that can be used to map Address Windowing Extensions (AWE) pages. This value must be used with MEM_RESERVE and no other values.")]
        MEM_PHYSICAL = 0x400000,

        /// <summary>
        ///Allocates memory at the highest possible address. This can be slower than regular allocations, especially when there are many allocations.
        ///</summary>
        [Description("Allocates memory at the highest possible address. This can be slower than regular allocations, especially when there are many allocations.")]
        MEM_TOP_DOWN = 0x100000,

        /// <summary>
        ///Causes the system to track pages that are written to in the allocated region. If you specify this value, you must also specify MEM_RESERVE.
        ///</summary>
        [Description("Causes the system to track pages that are written to in the allocated region. If you specify this value, you must also specify MEM_RESERVE.")]
        MEM_WRITE_WATCH = 0x200000
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryProtectionFlags AllocationProtect;
        public IntPtr RegionSize;
        public MemoryStates State;
        public MemoryProtectionFlags Protect;
        public MemoryTypes Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESS_HEAP_ENTRY_BLOCK
    {
        public IntPtr lpData;
        public uint cbData;
        public byte cbOverhead;
        public byte iRegionIndex;
        public HeapWalkFlags wFlags;
        public IntPtr hMem;
        public uint dwReserved1;
        public uint dwReserved2;
        public uint dwReserved3;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESS_HEAP_ENTRY_REGION
    {
        public IntPtr lpData;
        public uint cbData;
        public byte cbOverhead;
        public byte iRegionIndex;
        public HeapWalkFlags wFlags;
        public uint dwCommittedSize;
        public uint dwUnCommittedSize;
        public IntPtr lpFirstBlock;
        public IntPtr lpLastBlock;
    }

    internal static class Native
    {
        [DllImport("netapi32.dll")]
        public static extern int NetApiBufferAllocate(int byteCount, out IntPtr buffer);

        [DllImport("netapi32.dll")]
        public static extern int NetApiBufferReallocate(IntPtr oldBuffer, int newSize, out IntPtr newBuffer);

        [DllImport("netapi32.dll")]
        public static extern int NetApiBufferSize(IntPtr buffer, out int cbsize);

        [DllImport("netapi32.dll")]
        public static extern int NetApiBufferFree(IntPtr buffer);

        [DllImport("kernel32")]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, IntPtr dwSize, VMemAllocFlags flAllocationType, MemoryProtectionFlags flProtect);

        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, MemoryProtectionFlags flNewProtect, ref MemoryProtectionFlags flOldProtect);

        [DllImport("kernel32")]
        public static extern bool VirtualFree(IntPtr lpAddress, IntPtr dwSize = default, VMemFreeFlags dwFreeType = VMemFreeFlags.MEM_RELEASE);

        [DllImport("kernel32")]
        public static extern IntPtr VirtualQuery(IntPtr lpAddress, ref MEMORY_BASIC_INFORMATION lpBuffer, IntPtr dwLength);

        [DllImport("kernel32")]
        public static extern bool VirtualLock(IntPtr lpAddress, IntPtr dwSize);

        [DllImport("kernel32")]
        public static extern bool VirtualUnlock(IntPtr lpAddress, IntPtr dwSize);

        [DllImport("kernel32")]
        public static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

        [DllImport("kernel32")]
        public static extern IntPtr GetLargePageMinimum();

        [DllImport("kernel32", EntryPoint = "HeapWalk", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern bool HeapWalk(
          IntPtr hHeap,
          WinPtrBase lpEntry
        );

        [DllImport("kernel32", EntryPoint = "HeapCreate", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr HeapCreate(int dwOptions, IntPtr initSize, IntPtr maxSize);

        [DllImport("kernel32", EntryPoint = "HeapDestroy", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern bool HeapDestroy(IntPtr hHeap);

        [DllImport("kernel32", EntryPoint = "GetProcessHeap", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr GetProcessHeap();

        [DllImport("kernel32", EntryPoint = "HeapQueryInformation", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern bool HeapQueryInformation(IntPtr HeapHandle, int HeapInformationClass, ref ulong HeapInformation, IntPtr HeapInformationLength, IntPtr ReturnLength);

        // As per the MSDN manual, we're using ONLY Heap functions, here.

        [DllImport("kernel32", EntryPoint = "HeapAlloc", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwOptions, IntPtr dwBytes);

        [DllImport("kernel32", EntryPoint = "HeapReAlloc", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr HeapReAlloc(IntPtr hHeap, int dwOptions, IntPtr lpMem, IntPtr dwBytes);

        [DllImport("kernel32", EntryPoint = "HeapFree", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern bool HeapFree(IntPtr hHeap, uint dwOptions, IntPtr lpMem);

        [DllImport("kernel32", EntryPoint = "HeapSize", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr HeapSize(IntPtr hHeap, uint dwOptions, IntPtr lpMem);

        [DllImport("kernel32", EntryPoint = "HeapLock", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern bool HeapLock(IntPtr hHeap);

        [DllImport("kernel32", EntryPoint = "HeapUnlock", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern bool HeaUnlock(IntPtr hHeap);

        [DllImport("kernel32", EntryPoint = "HeapValidate", CharSet = CharSet.Unicode, PreserveSig = true, SetLastError = true)]
        public static extern bool HeapValidate(IntPtr hHeap, uint dwOptions, IntPtr lpMem);

        public static unsafe void PlatformZeroMemory(void* dest, long byteCount)
        {
            n_memset(dest, 0, (IntPtr)byteCount);
        }

        // used for specific operating system functions.
        [DllImport("kernel32.dll")]
        public static extern IntPtr LocalFree(IntPtr hMem);

        // used for specific operating system functions.
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        /// <summary>
        ///Native Visual C Runtime memset.
        ///</summary>
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memset", PreserveSig = true)]
        public static extern unsafe void n_memset(void* dest, int c, IntPtr count);

        /// <summary>
        ///Native Visual C Runtime memcpy.
        ///</summary>
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr n_memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        public static unsafe void ZeroMemory(void* handle, long len)
        {
            if (len > 1024)
            {
                PlatformZeroMemory(handle, len);
                return;
            }

            unsafe
            {
                byte* bp1 = (byte*)handle;
                byte* bep = (byte*)handle + len;

                var mc = 0M;

                ((long*)&mc)[1] = 0L;

                if (len >= IntPtr.Size)
                {
                    if (IntPtr.Size == 8)
                    {
                        long* lp1 = (long*)bp1;
                        long* lep = (long*)bep;

                        do
                        {
                            *lp1++ = 0L;
                        } while (lp1 < lep);

                        if (lp1 == lep) return;

                        lp1--;
                        bp1 = (byte*)lp1;
                    }
                    else
                    {
                        int* ip1 = (int*)bp1;
                        int* lep = (int*)bep;

                        do
                        {
                            *ip1++ = 0;
                        } while (ip1 < lep);

                        if (ip1 == lep) return;

                        ip1--;
                        bp1 = (byte*)ip1;
                    }
                }

                do
                {
                    *bp1++ = 0;
                } while (bp1 < bep);
            }
        }

        public static void MemCpy(IntPtr src, IntPtr dest, long len)
        {
            unsafe
            {
                Buffer.MemoryCopy((void*)src, (void*)dest, len, len);
            }
        }

        //        public static unsafe void MemCpy(void* src, void* dest, long len)
        //        {
        //            unsafe
        //            {
        //                //if (len >= 2048)
        //                //{
        //                buffer.MemoryCopy(src, dest, len, len);
        //                return;
        //                //}

        //                byte* bp1 = (byte*)src;
        //                byte* bp2 = (byte*)dest;
        //                byte* bep = (byte*)src + len;

        //#if X64
        //                if (len >= 16)
        //                {
        //                    do
        //                    {
        //                        *(long*)bp2 = *(long*)bp1;
        //                        if (bep - bp1 < 8) break;

        //                        bp1 += 8;
        //                        bp2 += 8;

        //                    } while (bp1 < bep);

        //                    if (bp1 == bep) return;

        //                    len = bep - bp1;
        //                }

        //#else
        //                if (len >= 16)
        //                {
        //                    do
        //                    {
        //                        *(int*)bp2 = *(int*)bp1;
        //                        if (bep - bp1 < 4) break;

        //                        bp1 += 4;
        //                        bp2 += 4;

        //                    } while (bp1 < bep);

        //                    if (bp1 == bep) return;
        //                }

        //#endif
        //                do
        //                {
        //                    *bp2++ = *bp1++;
        //                } while (bp1 < bep);
        //            }
        //        }
    }

    //[StructLayout(LayoutKind.Sequential, Size = 16, Pack = 1)]
    //internal struct Block16
    //{
    //    public ulong val1;
    //    public ulong val2;
    //}

    //[StructLayout(LayoutKind.Sequential, Size = 64, Pack = 1)]
    //internal struct Block64
    //{
    //    public ulong val1;
    //    public ulong val2;
    //    public ulong val3;
    //    public ulong val4;
    //    public ulong val5;
    //    public ulong val6;
    //    public ulong val7;
    //    public ulong val8;
    //}
}