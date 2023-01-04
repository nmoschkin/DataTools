namespace DataTools.Win32.Memory
{
    /// <summary>
    /// Actions to be taken by an object created by a <see cref="Heap"/> when it is being destroyed.
    /// </summary>
    public enum HeapDestroyBehavior
    {
        /// <summary>
        /// The contents of the object are destroyed with the heap.
        /// </summary>
        Cascade,

        /// <summary>
        /// The buffer is transferred to the process heap.
        /// </summary>
        TransferOut
    }
}