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
        /// Gets the pointer to the process heap.
        /// </summary>
        public static readonly nint ProcessHeap = Native.GetProcessHeap();

        private nint currentHeap = ProcessHeap;

        /// <summary>
        /// The current heap.
        /// </summary>
        /// <remarks>
        /// This is usually the process heap, but creating independent heaps are possible.
        /// </remarks>
        public virtual nint CurrentHeap
        {
            get
            {
                if (currentHeap == nint.Zero)
                {
                    currentHeap = ProcessHeap;
                }

                return currentHeap;
            }
            protected set
            {
                if (value == nint.Zero)
                {
                    currentHeap = ProcessHeap;
                }
                else if (currentHeap == value)
                {
                    return;
                }
                else
                {
                    currentHeap = value;
                }
            }
        }

        /// <summary>
        /// Instantiate and set up a new memory buffer object.
        /// </summary>
        /// <param name="ptr">The initial pointer.</param>
        /// <param name="t">The type of memory being allocated.</param>
        /// <param name="fOwn">Whether we will own the memory pointer specified by <paramref name="ptr"/>.</param>
        /// <param name="gcpressure">True to make the garbage collector aware of memory allocations made by this object.</param>
        protected WinPtrBase(nint ptr, bool fOwn, bool gcpressure) : base(ptr, fOwn, gcpressure)
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
    }
}