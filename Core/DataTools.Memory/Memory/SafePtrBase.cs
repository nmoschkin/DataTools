using DataTools.Streams;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTools.Memory
{
    /// <summary>
    /// Represents a base class for memory buffers that are uniquely allocated for specific use cases (COM, networking, etc.)
    /// </summary>
    public abstract class SafePtrBase : SafeHandle, ICloneable, IEquatable<SafePtrBase>
    {
        #region Constructors

        /// <summary>
        /// Instantiate and set up a new memory buffer object.
        /// </summary>
        /// <param name="ptr">The initial pointer.</param>
        /// <param name="t">The type of memory being allocated.</param>
        /// <param name="fOwn">Whether we will own the memory pointer specified by <paramref name="ptr"/>.</param>
        /// <param name="gcpressure">True to make the garbage collector aware of memory allocations made by this object.</param>
        protected SafePtrBase(nint ptr, bool fOwn, bool gcpressure) : base(0, fOwn)
        {
            handle = ptr;
            IsOwner = fOwn;
            HasGCPressure = gcpressure;
        }

        /// <summary>
        /// Instantiate and set up a new memory buffer object.
        /// </summary>
        /// <param name="ptr">The initial pointer.</param>
        /// <param name="size">The size of the buffer.</param>
        /// <param name="t">The type of memory being allocated.</param>
        /// <param name="fOwn">Whether we will own the memory pointer specified by <paramref name="ptr"/>.</param>
        /// <param name="gcpressure">True to make the garbage collector aware of memory allocations made by this object.</param>
        protected unsafe SafePtrBase(void* ptr, bool fOwn, bool gcpressure) : base(0, fOwn)
        {
            handle = (nint)ptr;
            IsOwner = fOwn;
            HasGCPressure = gcpressure;
        }

        #endregion Constructors

        #region Public Properties

        public override bool IsInvalid => handle == (nint)0;

        /// <summary>
        /// Gets or sets the length of the buffer.
        /// </summary>
        /// <remarks>
        /// If this object does not own the underlying pointer, then setting this value has an unknown effect.
        /// </remarks>
        public virtual long Length
        {
            get => GetLogicalSize();
            set
            {
                ReAlloc(value);
            }
        }

        /// <summary>
        /// Gets the allocated memory type.
        /// </summary>
        public abstract MemoryType MemoryType { get; }

        /// <summary>
        /// True if this is the owner of the underlying memory.
        /// </summary>
        public bool IsOwner { get; }

        /// <summary>
        /// True if allocated memory is known to the garbage collector.
        /// </summary>
        public bool HasGCPressure { get; protected set; }

        /// <summary>
        /// This is a new handle value that can be reached by other members of this assembly.
        /// </summary>
        protected internal nint Handle
        {
            get => base.handle;
            set => base.handle = value;
        }

        #endregion Public Properties

        #region xxxxAt Methods

        /// <summary>
        /// Gets a reference to the value at the specified byte-sized index in the memory block.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte ByteAt(long index)
        {
            unsafe
            {
                return ref *(byte*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified byte-sized index in the memory block.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref sbyte SByteAt(long index)
        {
            unsafe
            {
                return ref *(sbyte*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref char CharAt(long index)
        {
            unsafe
            {
                return ref *(char*)((long)handle + index * sizeof(char));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref char CharAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(char*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref short ShortAt(long index)
        {
            unsafe
            {
                return ref *(short*)((long)handle + index * sizeof(short));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref short ShortAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(short*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ushort UShortAt(long index)
        {
            unsafe
            {
                return ref *(ushort*)((long)handle + index * sizeof(ushort));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ushort UShortAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(ushort*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref int IntAt(long index)
        {
            unsafe
            {
                return ref *(int*)((long)handle + index * sizeof(int));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref int IntAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(int*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref uint UIntAt(long index)
        {
            unsafe
            {
                return ref *(uint*)((long)handle + index * sizeof(uint));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref uint UIntAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(uint*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref long LongAt(long index)
        {
            unsafe
            {
                return ref *(long*)((long)handle + index * sizeof(long));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref long LongAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(long*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ulong ULongAt(long index)
        {
            unsafe
            {
                return ref *(ulong*)((long)handle + index * sizeof(ulong));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ulong ULongAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(ulong*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float FloatAt(long index)
        {
            unsafe
            {
                return ref *(float*)((long)handle + index * sizeof(float));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float FloatAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(float*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float SingleAt(long index) => ref FloatAt(index);

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float SingleAtAbsolute(long index) => ref FloatAtAbsolute(index);

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref double DoubleAt(long index)
        {
            unsafe
            {
                return ref *(double*)((long)handle + index * sizeof(double));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref double DoubleAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(double*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref decimal DecimalAt(long index)
        {
            unsafe
            {
                return ref *(decimal*)((long)handle + index * sizeof(decimal));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref decimal DecimalAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(decimal*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Guid GuidAt(long index)
        {
            unsafe
            {
                return ref *(Guid*)((long)handle + index * sizeof(Guid));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Guid GuidAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(Guid*)((long)handle + index);
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified size-relative index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// The absolute index is computed as sizeof(T) * <paramref name="index"/>.<br/><br/>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref DateTime DateTimeAt(long index)
        {
            unsafe
            {
                return ref *(DateTime*)((long)handle + index * sizeof(DateTime));
            }
        }

        /// <summary>
        /// Gets a reference to the value at the specified absolute index in the memory block.
        /// </summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>A ref value that can be read or assigned to.</returns>
        /// <remarks>
        /// This is an unchecked method. Invoking this method on an unallocated pointer will result in an <see cref="AccessViolationException"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref DateTime DateTimeAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(DateTime*)((long)handle + index);
            }
        }

        #endregion xxxxAt Methods

        #region Structures and Arrays

        /// <summary>
        /// Append the contents of the specified structure to the end of the block of memory.
        /// </summary>
        /// <typeparam name="T">The type of structure to append.</typeparam>
        /// <param name="value">The structure to append.</param>
        /// <remarks>
        /// This method attempts to automatically extend the allocation of the memory block.
        /// </remarks>
        public virtual void Append<T>(T value) where T : struct
        {
            FromStructAt(Length, value);
        }

        /// <summary>
        /// Append the contents of the specified buffer to the end of the block of memory.
        /// </summary>
        /// <param name="buffer">The memory location to copy from.</param>
        /// <param name="size">The length of bytes to copy.</param>
        /// <remarks>
        /// This method attempts to automatically extend the allocation of the memory block.
        /// </remarks>
        public virtual void Append(nint buffer, int size)
        {
            unsafe
            {
                Append((void*)buffer, size);
            }
        }

        /// <summary>
        /// Append the contents of the specified buffer to the end of the block of memory.
        /// </summary>
        /// <param name="buffer">The memory location to copy from.</param>
        /// <param name="size">The length of bytes to copy.</param>
        /// <remarks>
        /// This method attempts to automatically extend the allocation of the memory block.
        /// </remarks>
        public virtual unsafe void Append(void* buffer, int size)
        {
            long c = Length;

            ReAlloc(Length + size);
            Buffer.MemoryCopy(buffer, (void*)((long)handle + c), size, size);
        }

        /// <summary>
        /// Converts the contents of an unmanaged pointer into a structure.
        /// </summary>
        /// <typeparam name="T">The type of structure requested.</typeparam>
        /// <returns>New instance of T.</returns>
        /// <remarks></remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T ToStruct<T>() where T : struct
        {
            return (T)Marshal.PtrToStructure(handle, typeof(T));
        }

        /// <summary>
        /// Sets the contents of a structure into an unmanaged pointer.
        /// </summary>
        /// <typeparam name="T">The type of structure to set.</typeparam>
        /// <param name="val">The structure to set.</param>
        /// <remarks></remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void FromStruct<T>(T val) where T : struct
        {
            int cb = Marshal.SizeOf<T>();
            if (cb > Length) ReAlloc(cb);

            Marshal.StructureToPtr(val, handle, false);
        }

        /// <summary>
        /// Converts the contents of an unmanaged pointer at the specified byte index into a structure.
        /// </summary>
        /// <typeparam name="T">The type of structure requested.</typeparam>
        /// <param name="byteIndex">The byte index relative to the pointer at which to begin copying.</param>
        /// <returns>New instance of T.</returns>
        /// <remarks></remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T ToStructAt<T>(long byteIndex) where T : struct
        {
            return (T)Marshal.PtrToStructure((nint)((long)handle + byteIndex), typeof(T));
        }

        /// <summary>
        /// Sets the contents of a structure into a memory buffer at the specified byte index.
        /// </summary>
        /// <typeparam name="T">The type of structure to set.</typeparam>
        /// <param name="byteIndex">The byte index relative to the pointer at which to begin copying.</param>
        /// <param name="val">The structure to set.</param>
        /// <remarks></remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void FromStructAt<T>(long byteIndex, T val) where T : struct
        {
            int cb = Marshal.SizeOf(val);

            if (byteIndex + cb > Length) ReAlloc(byteIndex + cb);

            Marshal.StructureToPtr(val, (nint)((long)handle + byteIndex), false);
        }

        /// <summary>
        /// Gets a byte array from the memory buffer.
        /// </summary>
        /// <param name="index">The starting absolute index to begin copying.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToByteArray(long index = 0, int length = 0)
        {
            long len = length;
            long size = Length;

            if (len == 0) len = size - index;
            if (size - index < length) len = size - index;

            if (len > int.MaxValue) len = int.MaxValue;

            byte[] output = new byte[len];

            unsafe
            {
                void* ptr1 = (void*)((long)handle + index);
                fixed (void* ptr2 = output)
                {
                    Buffer.MemoryCopy(ptr1, ptr2, len, len);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets a character array from the memory buffer.
        /// </summary>
        /// <param name="index">The starting absolute index to begin copying.</param>
        /// <param name="length">The number of UTF-16 characters to copy.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char[] ToCharArray(long index = 0, int length = 0)
        {
            long len = length * sizeof(char);
            long size = Length;

            if (len == 0) len = size - index;
            if (size - index < length) len = size - index;

            if (len > int.MaxValue) len = int.MaxValue;

            if (len % sizeof(char) != 0)
            {
                len -= len % sizeof(char);
            }

            char[] output = new char[len / sizeof(char)];

            unsafe
            {
                void* ptr1 = (void*)((long)handle + index);
                fixed (void* ptr2 = output)
                {
                    Buffer.MemoryCopy(ptr1, ptr2, len, len);
                }
            }

            return output;
        }

        /// <summary>
        /// Copies the contents of this memory buffer into an array of structs.
        /// </summary>
        /// <param name="index">The starting absolute index to begin copying.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray<T>(long index = 0, int length = 0) where T : struct
        {
            unsafe
            {
                int tlen = typeof(T) == typeof(char) ? sizeof(char) : Marshal.SizeOf<T>();

                long len = length * tlen;
                long size = Length;

                if (len == 0) len = size - index;
                if (size - index < length) len = size - index;

                if (len > int.MaxValue) len = int.MaxValue;

                if (len % tlen != 0)
                {
                    len -= len % tlen;
                }

                T[] output = new T[len / tlen];

                GCHandle gch = GCHandle.Alloc(output, GCHandleType.Pinned);

                unsafe
                {
                    void* ptr1 = (void*)((long)handle + index);
                    void* ptr2 = (void*)gch.AddrOfPinnedObject();

                    Buffer.MemoryCopy(ptr1, ptr2, len, len);
                }

                gch.Free();
                return output;
            }
        }

        /// <summary>
        /// Copy a whole byte array into the memory buffer at the specified index.
        /// </summary>
        /// <param name="value">The byte array to copy.</param>
        /// <param name="index">The starting index in the memory buffer to start placing copied values.</param>
        public void FromByteArray(byte[] value, long index = 0)
        {
            if (Length < value.Length + index)
            {
                ReAlloc(value.Length + index);
            }
            unsafe
            {
                var vl = value.Length;
                fixed (void* ptr = value)
                {
                    Buffer.MemoryCopy(ptr, (void*)((long)handle + index), vl, vl);
                }
            }
        }

        /// <summary>
        /// Copy a whole character array into the memory buffer at the specified index.
        /// </summary>
        /// <param name="value">The character array to copy.</param>
        /// <param name="index">The starting index in the memory buffer to start placing copied values.</param>
        public void FromCharArray(char[] value, long index = 0)
        {
            if (Length < value.Length * 2 + index)
            {
                ReAlloc(value.Length * 2 + index);
            }
            unsafe
            {
                var vl = value.Length * 2;
                fixed (void* ptr = value)
                {
                    Buffer.MemoryCopy(ptr, (void*)((long)handle + index), vl, vl);
                }
            }
        }

        /// <summary>
        /// Copy a whole struct array into the memory buffer at the specified index.
        /// </summary>
        /// <param name="value">The struct array to copy.</param>
        /// <param name="index">The starting index in the memory buffer to start placing copied elements.</param>
        public void FromArray<T>(T[] value, long index = 0) where T : struct
        {
            var cb = Marshal.SizeOf<T>();

            if (Length < value.Length * cb + index)
            {
                ReAlloc(value.Length * cb + index);
            }
            unsafe
            {
                var vl = value.Length * cb;
                GCHandle gch = GCHandle.Alloc(value, GCHandleType.Pinned);
                Buffer.MemoryCopy((void*)gch.AddrOfPinnedObject(), (void*)((long)handle + index), vl, vl);
                gch.Free();
            }
        }

        #endregion Structures and Arrays

        #region Strings

        /// <summary>
        /// Gets a UTF-16 string at the specified index in the memory buffer.
        /// </summary>
        /// <param name="index">The byte index of the string to retrieve.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(long index)
        {
            unsafe
            {
                return new string((char*)((long)handle + index));
            }
        }

        /// <summary>
        /// Gets a UTF-16 string at the specified index in the memory buffer with the specified character length.
        /// </summary>
        /// <param name="index">The byte index of the string to retrieve.</param>
        /// <param name="length">The number of UTF-16 characters to copy.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(long index, int length)
        {
            unsafe
            {
                return new string((char*)((long)handle + index), 0, length);
            }
        }

        /// <summary>
        /// Copy a string as UTF-16 characters into the memory buffer at the specified byte index.
        /// </summary>
        /// <param name="index">The byte index in the buffer to copy the string into.</param>
        /// <param name="value">The string to copy into the buffer.</param>
        /// <param name="addNull">True to add a null termination character to the end of the string.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetString(long index, string value, bool addNull = true)
        {
            unsafe
            {
                EncodeUTF16Ptr((char*)((long)handle + index), value, addNull);
            }
        }

        /// <summary>
        /// Copy a string as UTF-16 characters into memory specified by a pointer located at the specified byte index in the memory buffer.
        /// </summary>
        /// <param name="index">The byte index in the buffer that contains the pointer to the memory block that will receive the string.</param>
        /// <param name="value">The string to copy into the buffer.</param>
        /// <param name="addNull">True to add a null termination character to the end of the string.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStringIndirect(long index, string value, bool addNull = true)
        {
            unsafe
            {
                char* ptr = (char*)*(nint*)((long)handle + index);
                EncodeUTF16Ptr(ptr, value, addNull);
            }
        }

        /// <summary>
        /// Gets a string from the memory pointer located at the specified byte index in the memory buffer.
        /// </summary>
        /// <param name="index">The byte index in the buffer that contains the pointer to the memory block that contains the string.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetStringIndirect(long index)
        {
            unsafe
            {
                char* ptr = (char*)*(nint*)((long)handle + index);
                return new string(ptr);
            }
        }

        /// <summary>
        /// Gets a string encoded as UTF-8 from the specified byte index in the memory buffer.
        /// </summary>
        /// <param name="index">The index in the memory buffer that contains the UTF-8-encoded string.</param>
        /// <returns>A <see cref="string"/> that has been decoded from UTF-8.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetUTF8String(long index)
        {
            unsafe
            {
                return DecodeUTF8Ptr((byte*)((long)handle + index));
            }
        }

        /// <summary>
        /// Gets a string encoded as UTF-8 from the memory pointer located at the specified byte index in the memory buffer.
        /// </summary>
        /// <param name="index">The byte index in the buffer that contains the pointer to the memory block that contains the UTF-8-encoded string.</param>
        /// <returns>A <see cref="string"/> that has been decoded from UTF-8.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetUTF8StringIndirect(long index)
        {
            unsafe
            {
                return DecodeUTF8Ptr((byte*)*(nint*)((long)handle + index));
            }
        }

        /// <summary>
        /// Decode a null-terminated UTF-8 byte array into a string.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe string DecodeUTF8Ptr(byte* ptr)
        {
            byte* b2 = ptr;

            while (*b2 != 0) b2++;
            if (ptr == b2) return "";

            return Encoding.UTF8.GetString(ptr, (int)(b2 - ptr));
        }

        /// <summary>
        /// Copy a string as UTF-8 characters into the memory buffer at the specified byte index.
        /// </summary>
        /// <param name="index">The byte index in the buffer to copy the string into.</param>
        /// <param name="value">The string to copy into the buffer.</param>
        /// <param name="addNull">True to add a null termination character to the end of the string.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUTF8String(long index, string value, bool addNull = true)
        {
            unsafe
            {
                EncodeUTF8Ptr((byte*)((long)handle + index), value, addNull);
            }
        }

        /// <summary>
        /// Copy a string as UTF-8 characters into memory specified by a pointer located at the specified byte index in the memory buffer.
        /// </summary>
        /// <param name="index">The byte index in the buffer that contains the pointer to the memory block that will receive the string.</param>
        /// <param name="value">The string to copy into the buffer.</param>
        /// <param name="addNull">True to add a null termination character to the end of the string.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUTF8StringIndirect(long index, string value, bool addNull = true)
        {
            unsafe
            {
                EncodeUTF8Ptr((byte*)*(nint*)((long)handle + index), value, addNull);
            }
        }

        /// <summary>
        /// Encode a string into a null-terminated UTF-8 byte array at the specified memory pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="value"></param>
        /// <param name="addNull"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void EncodeUTF8Ptr(byte* ptr, string value, bool addNull)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            int slen = data.Length;

            byte* b1 = ptr;

            fixed (byte* ptr2 = data)
            {
                var b2 = ptr2;

                for (int i = 0; i < slen; i++)
                {
                    *b1++ = *b2++;
                }
            }

            if (addNull) *b1++ = 0;
        }

        /// <summary>
        /// Encode a string into a null-terminated UTF-16 character array at the specified memory pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="value"></param>
        /// <param name="addNull"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void EncodeUTF16Ptr(char* ptr, string value, bool addNull)
        {
            int slen = value.Length;

            char* b1 = ptr;
            var buffer = Encoding.Unicode.GetBytes(value);

            fixed (void* ptr2 = buffer)
            {
                char* b2 = (char*)ptr2;

                for (int i = 0; i < slen; i++)
                {
                    *b1++ = *b2++;
                }
            }

            if (addNull) *b1++ = '\x0';
        }

        /// <summary>
        /// Returns the string array at the byteIndex.
        /// </summary>
        /// <param name="byteIndex">Index at which to start copying.</param>
        /// <returns></returns>
        /// <remarks>
        /// The array is assumed to consist of null-terminated UTF-16 character sequences<br/>
        /// with the entire array terminated with a double null UTF-16 character sequence.
        /// </remarks>
        public string[] GetStringArray(long byteIndex)
        {
            unsafe
            {
                if (handle == nint.Zero) return null;

                string s = null;

                char* cp = (char*)((ulong)handle + (ulong)byteIndex);
                char* ap = cp;

                int x = 0;

                List<string> o = new List<string>();

                while (true)
                {
                    if (*ap == (char)0)
                    {
                        if (x != 0)
                        {
                            s = new string(cp, 0, x);
                            o.Add(s);

                            s = null;
                            x = 0;

                            ap++;
                            cp = ap;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        x++;
                        ap++;
                    }
                }

                return o.ToArray();
            }
        }

        /// <summary>
        /// Returns the string array at the byteIndex.
        /// </summary>
        /// <param name="byteIndex">Index at which to start copying.</param>
        /// <returns></returns>
        /// <remarks>
        /// The array is assumed to consist of null-terminated UTF-8 byte sequences<br/>
        /// with the entire array terminated with a double null UTF-8 byte sequence.
        /// </remarks>
        public string[] GetUTF8StringArray(long byteIndex)
        {
            unsafe
            {
                if (handle == nint.Zero) return null;

                string s = null;

                byte* cp = (byte*)((ulong)handle + (ulong)byteIndex);
                byte* ap = cp;

                int x = 0;

                List<string> o = new List<string>();

                while (true)
                {
                    if (*ap == (byte)0)
                    {
                        if (x != 0)
                        {
                            s = Encoding.UTF8.GetString(cp, x);
                            o.Add(s);

                            s = null;
                            x = 0;

                            ap++;
                            cp = ap;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        x++;
                        ap++;
                    }
                }

                return o.ToArray();
            }
        }

        #endregion Strings

        #region Editing

        /// <summary>
        /// Reverses the entire memory pointer.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public virtual bool Reverse()
        {
            if (handle == nint.Zero || Length == 0)
                return false;

            long l = Length;

            unsafe
            {
                byte* b1, b2;

                b1 = (byte*)handle;
                b2 = (byte*)((long)handle + (l - 1));

                byte x;

                for (long i = 0; i < l; i++)
                {
                    x = *b1;

                    *b1 = *b2;
                    *b2 = x;

                    b1++;
                    b2--;
                }
            }

            return true;
        }

        /// <summary>
        /// Slides a block of memory toward the beginning or toward the end of the memory buffer,
        /// moving the memory around it to the other side.
        /// </summary>
        /// <param name="index">The index of the first byte in the affected block.</param>
        /// <param name="length">The length of the block.</param>
        /// <param name="offset">
        /// The offset amount of the slide.  If the amount is negative,
        /// the block slides toward the beginning of the memory buffer.
        /// If it is positive, it slides to the right.
        /// </param>
        /// <remarks></remarks>
        public virtual void Slide(long index, long length, long offset)
        {
            if (offset == 0)
                return;
            long hl = Length;
            if (hl <= 0)
                return;

            if (0 > index + length + offset || index + length + offset > hl)
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.AllocatedPtrBase.Slide().");
            }

            nint p1;
            nint p2;

            p1 = (nint)((long)handle + index);
            p2 = (nint)((long)handle + index + offset);

            long a = Math.Abs(offset);
            MemPtr m = new MemPtr(length);
            MemPtr n = new MemPtr(a);

            Native.MemCpy(p1, m.handle, length);
            Native.MemCpy(p2, n.handle, a);

            p1 = (nint)((long)handle + index + offset + length);
            Native.MemCpy(n.handle, p1, a);
            Native.MemCpy(m.handle, p2, length);

            m.Free();
            n.Free();
        }

        /// <summary>
        /// Pulls the data in from the specified index.
        /// </summary>
        /// <param name="index">The index where contraction begins. The contraction starts at this position.</param>
        /// <param name="amount">Number of bytes to pull in.</param>
        /// <param name="removePressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual long PullIn(long index, long amount, bool removePressure = false)
        {
            long hl = Length;
            if (Length == 0 || 0 > index || index >= hl - 1)
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.MemPtr.PullIn().");
            }

            long a = index + amount;
            long b = Length - a;
            Slide(a, b, -amount);
            ReAlloc(hl - amount);
            return Length;
        }

        /// <summary>
        /// Extend the buffer from the specified index.
        /// </summary>
        /// <param name="index">The index where expansion begins. The expansion starts at this position.</param>
        /// <param name="amount">Number of bytes to push out.</param>
        /// <param name="addPressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual long PushOut(long index, long amount, byte[] bytes = null, bool addPressure = false)
        {
            long hl = Length;
            if (hl <= 0)
            {
                Alloc(amount);
                return amount;
            }

            if (0 > index || index > hl - 1)
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.MemPtr.PushOut().");
            }

            long ol = Length - index;
            ReAlloc(hl + amount);
            Slide(index, ol, amount);

            if (bytes != null && bytes.Length <= amount)
            {
                FromByteArray(bytes, index);
            }
            else
            {
                unsafe
                {
                    Native.ZeroMemory((void*)((long)handle + index), amount);
                }
            }

            return Length;
        }

        /// <summary>
        /// Slides a block of memory as Unicode characters toward the beginning or toward the end of the buffer.
        /// </summary>
        /// <param name="index">The character index preceding the first character in the affected block.</param>
        /// <param name="length">The length of the block, in characters.</param>
        /// <param name="offset">The offset amount of the slide, in characters.  If the amount is negative, the block slides to the left, if it is positive it slides to the right.</param>
        /// <remarks></remarks>
        public virtual void SlideChar(long index, long length, long offset)
        {
            Slide(index << 1, length << 1, offset << 1);
        }

        /// <summary>
        /// Pulls the data in from the specified character index.
        /// </summary>
        /// <param name="index">The index where contraction begins. The contraction starts at this position.</param>
        /// <param name="amount">Number of characters to pull in.</param>
        /// <param name="removePressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual long PullInChar(long index, long amount, bool removePressure = false)
        {
            return PullIn(index << 1, amount * 1);
        }

        /// <summary>
        /// Extend the buffer from the specified character index.
        /// </summary>
        /// <param name="index">The index where expansion begins. The expansion starts at this position.</param>
        /// <param name="amount">Number of characters to push out.</param>
        /// <param name="addPressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual long PushOutChar(long index, long amount, char[] chars = null, bool addPressure = false)
        {
            return PushOut(index << 1, amount * 1, Encoding.Unicode.GetBytes(chars));
        }

        /// <summary>
        /// Parts the string in both directions from index.
        /// </summary>
        /// <param name="index">The index from which to expand.</param>
        /// <param name="amount">The amount of expansion, in both directions, so the total expansion will be amount * 1.</param>
        /// <param name="addPressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual void Part(long index, long amount, bool addPressure = false)
        {
            if (handle == nint.Zero)
            {
                Alloc(amount);
                return;
            }

            long l = Length;
            if (l <= 0)
                return;

            long ol = l - index;
            ReAlloc(l + amount * 1);

            Slide(index, ol, amount);
            Slide(index + amount + 1, ol, amount);
        }

        /// <summary>
        /// Inserts the specified bytes at the specified index.
        /// </summary>
        /// <param name="index">Index at which to insert.</param>
        /// <param name="value">Byte array to insert</param>
        /// <param name="addPressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual void Insert(long index, byte[] value, bool addPressure = false)
        {
            PushOut(index, value.Length, value);
        }

        /// <summary>
        /// Inserts the specified characters at the specified character index.
        /// </summary>
        /// <param name="index">Index at which to insert.</param>
        /// <param name="value">Character array to insert</param>
        /// <param name="addPressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual void Insert(long index, char[] value, bool addPressure = false)
        {
            PushOutChar(index, value.Length, value);
        }

        /// <summary>
        /// Delete the memory from the specified index.  Calls PullIn.
        /// </summary>
        /// <param name="index">Index to start the delete.</param>
        /// <param name="amount">Amount of bytes to delete</param>
        /// <param name="removePressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual void Delete(long index, long amount, bool removePressure = false)
        {
            PullIn(index, amount);
        }

        /// <summary>
        /// Delete the memory from the specified character index.  Calls PullIn.
        /// </summary>
        /// <param name="index">Index to start the delete.</param>
        /// <param name="amount">Amount of characters to delete</param>
        /// <param name="removePressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual void DeleteChar(long index, long amount, bool removePressure = false)
        {
            PullInChar(index, amount);
        }

        /// <summary>
        /// Consumes the buffer in both directions from specified index.
        /// </summary>
        /// <param name="index">Index at which consuming begins.</param>
        /// <param name="amount">Amount of contraction, in both directions, so the total contraction will be amount * 1.</param>
        /// <param name="removePressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual void Consume(long index, long amount, bool removePressure = false)
        {
            long hl = Length;
            if (hl <= 0 || amount > index || index >= hl - amount + 1)
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.Heap:Consume.");
            }

            index -= amount + 1;
            PullIn(index, amount);
            index += amount + 1;
            PullIn(index, amount);
        }

        /// <summary>
        /// Consumes the buffer in both directions from specified character index.
        /// </summary>
        /// <param name="index">Index at which consuming begins.</param>
        /// <param name="amount">Amount of contraction, in both directions, so the total contraction will be amount * 1.</param>
        /// <param name="removePressure">Specify whether to notify the garbage collector.</param>
        /// <remarks></remarks>
        public virtual void ConsumeChar(long index, long amount, bool removePressure = false)
        {
            long hl = Length;
            if (hl <= 0 || amount > index || index >= Convert.ToInt64(hl >> 1) - (amount + 1))
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.Heap:Consume.");
            }

            index -= amount + 1 << 1;
            PullIn(index, amount);
            index += amount + 1 << 1;
            PullIn(index, amount);
        }

        #endregion Editing

        #region Memory Allocation

        /// <summary>
        /// Returns the native size of the allocated buffer handled by this object, if available.
        /// </summary>
        /// <returns></returns>
        protected abstract long GetNativeSize();

        /// <summary>
        /// Returns true if there is a native implementation to retrieve the size or if the known logical size should be used instead.
        /// </summary>
        /// <returns></returns>
        protected abstract bool CanGetNativeSize();

        private long logSize;

        /// <summary>
        /// Gets the logical size of the buffer.
        /// </summary>
        /// <returns></returns>
        protected long GetLogicalSize()
        {
            if (handle == 0) return 0;
            return logSize;
        }

        /// <summary>
        /// Set the logical size for the buffer. This updates the last known state of the object.
        /// </summary>
        /// <param name="logicalSize"></param>
        /// <remarks>
        /// This is usually managed by the base class. Only use if you really know what you're doing!
        /// </remarks>
        protected void DangerousSetLogicalSize(long logicalSize)
        {
            if (handle == 0) logSize = 0;
            else logSize = logicalSize;
        }

        /// <summary>
        /// Allocate a block of memory.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        /// <remarks>
        /// If memory is already allocated, this method will forward the call to <see cref="ReAlloc(long)"/>, instead.
        /// <br /><br />
        /// If the requested size is the same as the current size, no action will be taken, and this method will return true.
        /// </remarks>
        public bool Alloc(long size)
        {
            // TODO: Logical Size!

            if (handle != 0) return ReAlloc(size);
            if (size > int.MaxValue) throw new NotSupportedException("CoTaskMem only supports 32-bit integer buffer lengths.");

            try
            {
                handle = Allocate(size);
                if (handle != 0)
                {
                    if (CanGetNativeSize())
                    {
                        size = GetNativeSize();
                    }

                    DangerousSetLogicalSize(size);
                    if (HasGCPressure) GC.AddMemoryPressure(size);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Provide the low-level ability to allocate a new memory block.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        /// <remarks>
        /// Overrides of this method should provide only the functionality necessary<br />
        /// to allocate a memory block and return the new pointer.
        /// </remarks>
        protected abstract nint Allocate(long size);

        /// <summary>
        /// Provide the low-level ability to re-allocate an existing memory block to change its size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        /// <remarks>
        /// Overrides of this method should provide only the functionality necessary<br />
        /// to reallocate a memory block and return the new pointer.
        /// </remarks>
        protected abstract nint Reallocate(nint oldptr, long newsize);

        /// <summary>
        /// Provide the low-level ability to free an allocated memory block.
        /// </summary>
        /// <param name="ptr"></param>
        /// <remarks>
        /// Overrides of this method should provide only the functionality necessary<br />
        /// to free the memory block.
        /// </remarks>
        protected abstract void Deallocate(nint ptr);

        /// <summary>
        /// Frees the block of memory.
        /// </summary>
        /// <remarks>
        /// Returns false only in the event of catastrophic failure.
        /// </remarks>
        public bool Free()
        {
            if (handle == 0) return true;
            if (!IsOwner) return true;

            try
            {
                var size = CanGetNativeSize() ? GetNativeSize() : GetLogicalSize();
                Deallocate(handle);
                if (HasGCPressure) GC.RemoveMemoryPressure(size);
                DangerousSetLogicalSize(0);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                handle = 0;
            }
        }

        /// <summary>
        /// Reallocates a block of memory in order to change its size. The old contents are copied to the new pointer.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        /// <remarks>
        /// If the requested size is the same as the current size, no action will be taken, and this method will return true.
        /// <br /><br />
        /// If there is no current allocated memory, then this call has the same effect as a call to <see cref="Alloc(long)"/>.
        /// <br /><br />
        /// Reallocation preserves the current block's contents.
        /// <br /><br />
        /// Old pointer references to this object's handle will be invalid after this call.
        /// </remarks>
        public bool ReAlloc(long size)
        {
            if (handle == nint.Zero) return Alloc(size);
            else if (size <= 0) return Free();

            try
            {
                var oldsize = CanGetNativeSize() ? GetNativeSize() : GetLogicalSize();
                var newptr = Reallocate(handle, (int)size);

                if (newptr != 0)
                {
                    handle = newptr;

                    // This may be the culprit
                    //if (CanGetNativeSize()) size = GetNativeSize();

                    if (HasGCPressure)
                    {
                        if (oldsize < size)
                        {
                            GC.AddMemoryPressure(size - oldsize);
                        }
                        else if (oldsize > size)
                        {
                            GC.RemoveMemoryPressure(oldsize - size);
                        }
                    }

                    DangerousSetLogicalSize(size);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Set all bytes in the memory buffer to zero.
        /// </summary>
        public void ZeroMemory(long index = -1, long length = -1)
        {
            unsafe
            {
                var loglen = CanGetNativeSize() ? GetNativeSize() : GetLogicalSize();

                byte* p = (byte*)handle;
                if (index != -1) p += index;

                if (p == null || loglen == 0) return;
                long len;

                if (length < 0 || length > loglen)
                {
                    len = loglen;
                }
                else
                {
                    len = length;
                }

                if (index > 0)
                {
                    if (index + len > loglen)
                    {
                        len -= (index + len) - loglen;
                    }
                }

                Native.ZeroMemory(p, len);
            }
        }

        protected override bool ReleaseHandle()
        {
            return Free();
        }

        #endregion Memory Allocation

        #region Object

        /// <summary>
        /// Copies the memory contents from this buffer to another buffer.
        /// </summary>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="srcidx">The start index in the source buffer to start reading.</param>
        /// <param name="destidx">The start index in the destination buffer to start writing.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <returns></returns>
        public bool CopyTo(SafePtrBase dest, int srcidx, int destidx, int length)
            => CopyTo(dest, (nint)srcidx, (nint)destidx, (nint)length);

        /// <summary>
        /// Copies the memory contents from this buffer to another buffer.
        /// </summary>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="srcidx">The start index in the source buffer to start reading.</param>
        /// <param name="destidx">The start index in the destination buffer to start writing.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <returns></returns>
        public bool CopyTo(SafePtrBase dest, long srcidx, long destidx, long length)
            => CopyTo(dest, (nint)srcidx, (nint)destidx, (nint)length);

        /// <summary>
        /// Copies the memory contents from this buffer to another buffer.
        /// </summary>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="srcidx">The start index in the source buffer to start reading.</param>
        /// <param name="destidx">The start index in the destination buffer to start writing.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <returns></returns>
        public virtual bool CopyTo(SafePtrBase dest, nint srcidx, nint destidx, nint length)
        {
            unsafe
            {
                if (dest.Length < (destidx + length)) return false;

                void* ptr1 = (void*)(handle + srcidx);
                void* ptr2 = (void*)(dest.handle + destidx);

                Buffer.MemoryCopy(ptr1, ptr2, length, length);

                return true;
            }
        }

        /// <summary>
        /// Calculate the Crc-32 of the contents up to the known length of the memory block.
        /// </summary>
        /// <returns></returns>
        public virtual uint CalculateCrc32()
        {
            long c = Length;
            if (handle == nint.Zero || c <= 0) return 0;

            unsafe
            {
                return Crc32.Hash((byte*)handle, c);
            }
        }

        public override string ToString()
        {
            if (handle == (nint)0) return "";
            return GetString(0);
        }

        /// <summary>
        /// Determine if this object is equal to another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the two buffers, or pointers were determined to be identical. Otherwise false.</returns>
        /// <remarks>
        /// If the other object is derived from <see cref="SafePtrBase"/>, then the two buffers are tested<br />
        /// for equality based on the ISO-3309 CRC-32 hash of their respective contents.<br />
        /// <br />
        /// If the other object is of type <see cref="nint"/>, then the pointer references are compared.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj is SafePtrBase ab)
            {
                return Equals(ab);
            }
            else if (obj is nint ip)
            {
                return ip == base.handle;
            }
            else if (obj is UIntPtr uip)
            {
                return uip == (UIntPtr)base.handle;
            }
            else if (obj is MemPtr mp)
            {
                return mp.handle == base.handle;
            }
            else if (obj is SafePtr sp)
            {
                return sp.handle == base.handle;
            }

            return false;
        }

        /// <summary>
        /// Test two buffers for equality based on the ISO-3309 CRC-32 hash of their respective contents.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the two buffers were determined to be identical. Otherwise false.</returns>
        public virtual bool Equals(SafePtrBase obj)
        {
            if (obj is SafePtrBase other)
            {
                return (Length == other.Length && CalculateCrc32() == other.CalculateCrc32());
            }

            return false;
        }

        /// <summary>
        /// Returns a ISO-3309 CRC-32 hash of the contents of the buffer within the known size.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// If the size is not known, the result will be -1.
        /// </remarks>
        public override int GetHashCode()
        {
            unsafe
            {
                if (handle == 0) return 0;
                return (int)Crc32.Hash((byte*)handle, Length);
            }
        }

        #endregion Object

        #region ICloneable

        object ICloneable.Clone()
        {
            return Clone();
        }

        protected abstract SafePtrBase Clone();

        #endregion ICloneable

        #region Explicit Cast Operators

        public static explicit operator byte[](SafePtrBase val)
        {
            return val.ToByteArray();
        }

        public static explicit operator char[](SafePtrBase val)
        {
            return val.ToCharArray();
        }

        public static explicit operator sbyte[](SafePtrBase val)
        {
            return val.ToArray<sbyte>();
        }

        public static explicit operator short[](SafePtrBase val)
        {
            return val.ToArray<short>();
        }

        public static explicit operator ushort[](SafePtrBase val)
        {
            return val.ToArray<ushort>();
        }

        public static explicit operator int[](SafePtrBase val)
        {
            return val.ToArray<int>();
        }

        public static explicit operator uint[](SafePtrBase val)
        {
            return val.ToArray<uint>();
        }

        public static explicit operator long[](SafePtrBase val)
        {
            return val.ToArray<long>();
        }

        public static explicit operator ulong[](SafePtrBase val)
        {
            return val.ToArray<ulong>();
        }

        public static explicit operator float[](SafePtrBase val)
        {
            return val.ToArray<float>();
        }

        public static explicit operator double[](SafePtrBase val)
        {
            return val.ToArray<double>();
        }

        public static explicit operator decimal[](SafePtrBase val)
        {
            return val.ToArray<decimal>();
        }

        public static explicit operator DateTime[](SafePtrBase val)
        {
            return val.ToArray<DateTime>();
        }

        public static explicit operator Guid[](SafePtrBase val)
        {
            return val.ToArray<Guid>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator string(SafePtrBase val)
        {
            if (val?.handle == (nint)0) return null;
            return val.GetString(0);
        }

        #endregion Explicit Cast Operators

        #region Implicit Cast Operators

        public static implicit operator nint(SafePtrBase val) => val.handle;

        public static unsafe implicit operator void*(SafePtrBase val) => (void*)val.handle;

        public static implicit operator UIntPtr(SafePtrBase val) => (UIntPtr)val.handle;

        #endregion Implicit Cast Operators

        #region Concatenating Operators

        public static SafePtrBase operator +(SafePtrBase val1, byte[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length);
            val1.FromByteArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, char[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(char));
            val1.FromCharArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, string val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(char));
            val1.FromCharArray(val2.ToCharArray(), c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, sbyte[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length);
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, short[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(short));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, ushort[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(ushort));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, int[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(int));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, uint[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(uint));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, long[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(long));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, ulong[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(ulong));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, float[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(float));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, double[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(double));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, decimal[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * sizeof(decimal));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, DateTime[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * Marshal.SizeOf<DateTime>());
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, Guid[] val2)
        {
            var c = val1.Length;

            val1.Alloc(val1.Length + val2.Length * Marshal.SizeOf<Guid>());
            val1.FromArray(val2, c);

            return val1;
        }

        #endregion Concatenating Operators

        #region Pointer Arithmatic Operators

        public static SafePtrBase operator +(SafePtrBase val1, short val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtrBase operator -(SafePtrBase val1, short val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, ushort val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtrBase operator -(SafePtrBase val1, ushort val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, int val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtrBase operator -(SafePtrBase val1, int val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, long val2)
        {
            val1.handle = (nint)((long)val1.handle + val2);
            return val1;
        }

        public static SafePtrBase operator -(SafePtrBase val1, long val2)
        {
            val1.handle = (nint)((long)val1.handle - val2);
            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, uint val2)
        {
            val1.handle = (nint)((uint)val1.handle + val2);
            return val1;
        }

        public static SafePtrBase operator -(SafePtrBase val1, uint val2)
        {
            val1.handle = (nint)((uint)val1.handle - val2);
            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, ulong val2)
        {
            val1.handle = (nint)((ulong)val1.handle + val2);
            return val1;
        }

        public static SafePtrBase operator -(SafePtrBase val1, ulong val2)
        {
            val1.handle = (nint)((ulong)val1.handle - val2);
            return val1;
        }

        public static SafePtrBase operator +(SafePtrBase val1, nint val2)
        {
            val1.handle = (nint)((long)val1.handle + (long)val2);
            return val1;
        }

        public static SafePtrBase operator -(SafePtrBase val1, nint val2)
        {
            val1.handle = (nint)((long)val1.handle - (long)val2);
            return val1;
        }

        #endregion Pointer Arithmatic Operators

        #region Equality Operators

        public static bool operator ==(nint val1, SafePtrBase val2)
        {
            return val1 == (val2?.handle ?? nint.Zero);
        }

        public static bool operator !=(nint val1, SafePtrBase val2)
        {
            return val1 != (val2?.handle ?? nint.Zero);
        }

        public static bool operator ==(SafePtrBase val2, nint val1)
        {
            return val1 == (val2?.handle ?? nint.Zero);
        }

        public static bool operator !=(SafePtrBase val2, nint val1)
        {
            return val1 != (val2?.handle ?? nint.Zero);
        }

        #endregion Equality Operators
    }
}