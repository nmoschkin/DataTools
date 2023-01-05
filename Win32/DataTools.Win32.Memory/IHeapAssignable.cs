using System;

namespace DataTools.Win32.Memory
{
    public interface IHeapAssignable
    {
        /// <summary>
        /// The current heap.
        /// </summary>
        /// <remarks>
        /// This is usually the process heap, but creating independent heaps are possible.
        /// </remarks>
        IntPtr CurrentHeap { get; }

        HeapDestroyBehavior HeapDestroyBehavior { get; }

#if NET6_0_OR_GREATER

        /// <summary>
        /// Set the heap for the object
        /// </summary>
        /// <param name="heap"></param>
        protected internal void AssignHeap(Heap heap);

        protected internal void HeapIsClosing(Heap heap);

#else
        /// <summary>
        /// Set the heap for the object
        /// </summary>
        /// <param name="heap"></param>
        void AssignHeap(Heap heap);

        void HeapIsClosing(Heap heap);

#endif
    }
}