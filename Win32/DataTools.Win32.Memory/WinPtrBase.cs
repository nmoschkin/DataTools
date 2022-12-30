using System;
using System.Text;

namespace DataTools.Win32.Memory
{
    /// <summary>
    /// Represents a base class for memory buffers that are uniquely allocated for specific use cases (COM, networking, etc.) in Windows
    /// </summary>
    public abstract class WinPtrBase : DataTools.Memory.SafePtrBase
    {
        /// <summary>
        /// Instantiate and set up a new memory buffer object.
        /// </summary>
        /// <param name="ptr">The initial pointer.</param>
        /// <param name="t">The type of memory being allocated.</param>
        /// <param name="fOwn">Whether we will own the memory pointer specified by <paramref name="ptr"/>.</param>
        /// <param name="gcpressure">True to make the garbage collector aware of memory allocations made by this object.</param>
        protected WinPtrBase(IntPtr ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
        }

        /// <summary>
        /// Instantiate and set up a new memory buffer object.
        /// </summary>
        /// <param name="ptr">The initial pointer.</param>
        /// <param name="size">The size of the buffer.</param>
        /// <param name="t">The type of memory being allocated.</param>
        /// <param name="fOwn">Whether we will own the memory pointer specified by <paramref name="ptr"/>.</param>
        /// <param name="gcpressure">True to make the garbage collector aware of memory allocations made by this object.</param>
        protected unsafe WinPtrBase(void* ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
        {
        }

        protected override void InternalDoZeroMem(IntPtr startptr, long length)
        {
            unsafe
            {
                Native.ZeroMemory((void*)startptr, (IntPtr)length);
            }
        }
    }
}