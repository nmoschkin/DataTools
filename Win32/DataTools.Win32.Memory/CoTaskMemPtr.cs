using System;

namespace DataTools.Win32.Memory
{
    /// <summary>
    /// Wraps a block of COM memory.
    /// </summary>
    public sealed class CoTaskMemPtr : DataTools.Memory.CoTaskMemPtr
    {
        public static readonly nint ProcessHeap = Native.GetProcessHeap();

        private long size = 0;

        public override long GetAllocatedSize()
        {
            try
            {
                if (handle == 0) return 0;
                size = Native.HeapSize(ProcessHeap, 0, handle);
            }
            catch
            {
            }

            return size;
        }
    }
}