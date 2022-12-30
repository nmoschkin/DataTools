using System;

namespace DataTools.Win32.Memory
{
    /// <summary>
    /// Wraps a block of COM memory.
    /// </summary>
    public sealed class CoTaskMemPtr : DataTools.Memory.CoTaskMemPtr
    {
        public static readonly IntPtr ProcessHeap = Native.GetProcessHeap();

        protected override long GetNativeSize()
        {
            return (long)Native.HeapSize(ProcessHeap, 0, handle);
        }

        protected override bool CanGetNativeSize()
        {
            return true;
        }

        protected override void InternalDoZeroMem(IntPtr startptr, long length)
        {
            unsafe
            {
                Native.ZeroMemory((void*)startptr, length);
            }
        }
    }
}