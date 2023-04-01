using System;

namespace DataTools.Win32.Memory
{
    /// <summary>
    /// Represents a memory object that can be assigned to a custom heap
    /// </summary>
    public interface IHeapAssignable
    {
        /// <summary>
        /// The current heap.
        /// </summary>
        /// <remarks>
        /// This is usually the process heap, but creating independent heaps are possible.
        /// </remarks>
        IntPtr CurrentHeap { get; }

        /// <summary>
        /// Gets behavior for this object when the heap is destroyed before the object
        /// </summary>
        HeapDestroyBehavior HeapDestroyBehavior { get; }

#if NET6_0_OR_GREATER

        /// <summary>
        /// Set the heap for the object
        /// </summary>
        /// <param name="heap"></param>
        protected internal void AssignHeap(Heap heap);

        /// <summary>
        /// Inform the heap-assignable object that the heap is going to be destroyed
        /// </summary>
        /// <param name="heap">The heap sending the message</param>
        protected internal void HeapIsClosing(Heap heap);

#else
        /// <summary>
        /// Set the heap for the object
        /// </summary>
        /// <param name="heap"></param>
        void AssignHeap(Heap heap);

        /// <summary>
        /// Inform the heap-assignable object that the heap is going to be destroyed
        /// </summary>
        /// <param name="heap">The heap sending the message</param>
        void HeapIsClosing(Heap heap);

#endif
    }
}