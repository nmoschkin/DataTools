﻿using DataTools.Standard.Memory.NativeLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DataTools.Streams;

namespace DataTools.Standard.Memory
{
    public class SafePtr : SafeHandle
    {
        public override bool IsInvalid => handle == (IntPtr)0;

        public virtual long Size { get; protected set; }

        public virtual long Length
        {
            get => Size;
            set
            {
                if (Size == value) return;

                if (value == 0)
                {
                    TFree();
                    return;
                }                
                else if (handle == IntPtr.Zero || MemoryType == MemoryType.Aligned || MemoryType == MemoryType.HGlobal)
                {
                    ReAlloc(value);
                }
            }
        }

        public virtual MemoryType MemoryType { get; protected set; }

        public virtual bool IsOwner { get; protected set; }

        public virtual bool HasGCPressure { get; protected set; }

        internal virtual new IntPtr handle
        {
            get => base.handle;
            set
            {
                if (base.handle == value) return;
                base.handle = value;
            }
        }

        public SafePtr(IntPtr ptr, int size, MemoryType t, bool fOwn) : base((IntPtr)0, fOwn)
        {
            handle = ptr;
            Size = size;
            MemoryType = t;
            IsOwner = fOwn;
        }

        public SafePtr(IntPtr ptr, long size) : base((IntPtr)0, false)
        {
            handle = ptr;
            Size = size;
        }

        public SafePtr(IntPtr ptr, int size) : base((IntPtr)0, false)
        {
            handle = ptr;
            Size = size;
        }

        public SafePtr(IntPtr ptr) : base((IntPtr)0, false)
        {
            handle = ptr;
        }

        public SafePtr(IntPtr ptr, bool fOwn) : base((IntPtr)0, fOwn)
        {
            handle = ptr;
            IsOwner = fOwn;
        }

        public SafePtr(IntPtr ptr, long size, bool fOwn) : base((IntPtr)0, fOwn)
        {
            handle = ptr;
            Size = size;
            IsOwner = fOwn;
        }

        public SafePtr(IntPtr ptr, int size, bool fOwn) : base((IntPtr)0, fOwn)
        {
            handle = ptr;
            Size = size;
            IsOwner = fOwn;
        }

        public unsafe SafePtr(void* ptr, int size) : base((IntPtr)0, false)
        {
            handle = (IntPtr)ptr;
            Size = size;
        }

        public unsafe SafePtr(void* ptr, long size) : base((IntPtr)0, false)
        {
            handle = (IntPtr)ptr;
            Size = size;
        }

        public unsafe SafePtr(void* ptr) : base((IntPtr)0, false)
        {
            handle = (IntPtr)ptr;
        }

        public unsafe SafePtr(void* ptr, bool fOwn) : base((IntPtr)0, fOwn)
        {
            handle = (IntPtr)ptr;
            IsOwner = fOwn;
        }

        public unsafe SafePtr(void* ptr, long size, bool fOwn) : base((IntPtr)0, fOwn)
        {
            handle = (IntPtr)ptr;
            Size = size;
            IsOwner = fOwn;
        }

        public unsafe SafePtr(void* ptr, int size, bool fOwn) : base((IntPtr)0, fOwn)
        {
            handle = (IntPtr)ptr;
            Size = size;
            IsOwner = fOwn;
        }

        public SafePtr() : base((IntPtr)0, true)
        {
            IsOwner = true;
        }

        public SafePtr(long size) : this()
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            Alloc(size);
        }

        public SafePtr(int size) : this()
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            Alloc(size);
        }

        public SafePtr(long size, MemoryType t) : this()
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

            MemoryType = t;
            TAlloc(size);
        }

        public SafePtr(int size, MemoryType t) : this()
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

            MemoryType = t;
            TAlloc(size);
        }

        public uint CalculateCrc32()
        {
            long c = Size;
            if (handle == IntPtr.Zero || c <= 0) return 0;

            unsafe
            {
                return Crc32.Calculate((byte*)handle, c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte ByteAt(long index)
        {
            unsafe
            {
                return ref *(byte*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref sbyte SByteAt(long index)
        {
            unsafe
            {
                return ref *(sbyte*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref char CharAt(long index)
        {
            unsafe
            {
                return ref *(char*)((long)handle + (index * sizeof(char)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref char CharAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(char*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref short ShortAt(long index)
        {
            unsafe
            {
                return ref *(short*)((long)handle + (index * sizeof(short)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref short ShortAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(short*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ushort UShortAt(long index)
        {
            unsafe
            {
                return ref *(ushort*)((long)handle + (index * sizeof(ushort)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ushort UShortAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(ushort*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref int IntAt(long index)
        {
            unsafe
            {
                return ref *(int*)((long)handle + (index * sizeof(int)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref int IntAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(int*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref uint UIntAt(long index)
        {
            unsafe
            {
                return ref *(uint*)((long)handle + (index * sizeof(uint)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref uint UIntAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(uint*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref long LongAt(long index)
        {
            unsafe
            {
                return ref *(long*)((long)handle + (index * sizeof(long)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref long LongAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(long*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ulong ULongAt(long index)
        {
            unsafe
            {
                return ref *(ulong*)((long)handle + (index * sizeof(ulong)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ulong ULongAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(ulong*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float FloatAt(long index)
        {
            unsafe
            {
                return ref *(float*)((long)handle + (index * sizeof(float)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float FloatAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(float*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float SingleAt(long index) => ref FloatAt(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float SingleAtAbsolute(long index) => ref FloatAtAbsolute(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref double DoubleAt(long index)
        {
            unsafe
            {
                return ref *(double*)((long)handle + (index * sizeof(double)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref double DoubleAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(double*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref decimal DecimalAt(long index)
        {
            unsafe
            {
                return ref *(decimal*)((long)handle + (index * sizeof(decimal)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref decimal DecimalAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(decimal*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Guid GuidAt(long index)
        {
            unsafe
            {
                return ref *(Guid*)((long)handle + (index * sizeof(Guid)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Guid GuidAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(Guid*)((long)handle + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref DateTime DateTimeAt(long index)
        {
            unsafe
            {
                return ref *(DateTime*)((long)handle + (index * sizeof(DateTime)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref DateTime DateTimeAtAbsolute(long index)
        {
            unsafe
            {
                return ref *(DateTime*)((long)handle + index);
            }
        }

        public void Append<T>(T value) where T: struct
        {
            FromStructAt(Size, value);
        }

        public void Append(IntPtr buffer, int buffLen)
        {
            unsafe
            {
                Append((void*)buffer, buffLen);
            }
        }

        public unsafe void Append(void *buffer, int buffLen)
        {
            long c = Length;

            Length += buffLen;
            Buffer.MemoryCopy(buffer, (void*)((long)handle + c), buffLen, buffLen);
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
            int cb = Marshal.SizeOf(val);

            if (cb > Size) ReAlloc(cb);

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
            return (T)Marshal.PtrToStructure((IntPtr)((long)handle + byteIndex), typeof(T));
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

            if (byteIndex + cb > Size) ReAlloc(byteIndex + cb);

            Marshal.StructureToPtr(val, (IntPtr)((long)handle + byteIndex), false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToByteArray(long index = 0, int length = 0)
        {
            long len = length;
            long size = Size;

            if (len == 0) len = (size - index);
            if (size - index < length) len = size - index;

            if (len > int.MaxValue) len = int.MaxValue;

            byte[] output = new byte[len];

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char[] ToCharArray(long index = 0, int length = 0)
        {
            long len = length * sizeof(char);
            long size = Size;

            if (len == 0) len = (size - index);
            if (size - index < length) len = size - index;

            if (len > int.MaxValue) len = int.MaxValue;

            if (len % sizeof(char) != 0)
            {
                len -= len % sizeof(char);
            }

            char[] output = new char[len / sizeof(char)];

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray<T>(long index = 0, int length = 0) where T : struct
        {
            unsafe
            {
                int tlen = typeof(T) == typeof(char) ? sizeof(char) : Marshal.SizeOf<T>();

                long len = length * tlen;
                long size = Size;

                if (len == 0) len = (size - index);
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

        public void FromByteArray(byte[] value, long index = 0)
        {
            if (Size < value.Length + index)
            {
                ReAlloc(value.Length + index);
            }
            unsafe
            {
                var vl = value.Length;
                GCHandle gch = GCHandle.Alloc(value, GCHandleType.Pinned);
                Buffer.MemoryCopy((void*)gch.AddrOfPinnedObject(), (void*)((long)handle + index), vl, vl);
                gch.Free();
            }
        }

        public void FromCharArray(char[] value, long index = 0)
        {
            if (Size < (value.Length * 2) + index)
            {
                ReAlloc((value.Length * 2) + index);
            }
            unsafe
            {
                var vl = value.Length * 2;
                GCHandle gch = GCHandle.Alloc(value, GCHandleType.Pinned);
                Buffer.MemoryCopy((void*)gch.AddrOfPinnedObject(), (void*)((long)handle + index), vl, vl);
                gch.Free();
            }
        }

        public void FromArray<T>(T[] value, long index = 0) where T : struct
        {
            var cb = Marshal.SizeOf<T>();

            if (Size < (value.Length * cb) + index)
            {
                ReAlloc((value.Length * cb) + index);
            }
            unsafe
            {
                var vl = value.Length * cb;
                GCHandle gch = GCHandle.Alloc(value, GCHandleType.Pinned);
                Buffer.MemoryCopy((void*)gch.AddrOfPinnedObject(), (void*)((long)handle + index), vl, vl);
                gch.Free();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(long index)
        {
            unsafe
            {
                return new string((char*)((long)handle + index));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(long index, int length)
        {
            unsafe
            {
                return new string((char*)((long)handle + index), 0, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetString(long index, string value, bool addNull = true)
        {
            unsafe
            {
                internalSetString((char*)((long)handle + index), value, addNull);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStringIndirect(long index, string value, bool addNull = true)
        {
            unsafe
            {
                char* ptr = (char*)*(IntPtr*)((long)handle + index);
                internalSetString(ptr, value, addNull);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetStringIndirect(long index)
        {
            unsafe
            {
                char* ptr = (char*)*(IntPtr*)((long)handle + index);
                return new string(ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetUTF8String(long index)
        {
            unsafe
            {
                return internalGetUTF8String((byte*)((long)handle + index));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetUTF8StringIndirect(long index)
        {
            unsafe
            {
                return internalGetUTF8String((byte*)*(IntPtr*)((long)handle + index));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe string internalGetUTF8String(byte* ptr)
        {
            byte* b2 = ptr;

            while (*b2 != 0) b2++;
            if (ptr == b2) return "";

            return Encoding.UTF8.GetString(ptr, (int)(b2 - ptr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUTF8String(long index, string value, bool addNull = true)
        {
            unsafe
            {
                internalSetUTF8String((byte*)((long)handle + index), value, addNull);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUTF8StringIndirect(long index, string value, bool addNull = true)
        {
            unsafe
            {
                internalSetUTF8String((byte*)*(IntPtr*)((long)handle + index), value, addNull);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void internalSetUTF8String(byte* ptr, string value, bool addNull)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            int slen = data.Length;

            GCHandle gch = GCHandle.Alloc(data, GCHandleType.Pinned);

            byte* b1 = ptr;
            byte* b2 = (byte*)(gch.AddrOfPinnedObject());

            for (int i = 0; i < slen; i++)
            {
                *b1++ = *b2++;
            }

            if (addNull) *b1++ = 0;
            gch.Free();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe void internalSetString(char* ptr, string value, bool addNull)
        {
            int slen = value.Length;
            GCHandle gch = GCHandle.Alloc(Encoding.Unicode.GetBytes(value), GCHandleType.Pinned);

            char* b1 = ptr;
            char* b2 = (char*)(gch.AddrOfPinnedObject());

            for (int i = 0; i < slen; i++)
            {
                *b1++ = *b2++;
            }

            if (addNull) *b1++ = '\x0';
            gch.Free();
        }

        /// <summary>
        /// Returns the string array at the byteIndex.
        /// </summary>
        /// <param name="byteIndex">Index at which to start copying.</param>
        /// <returns></returns>
        public string[] GetStringArray(long byteIndex)
        {
            unsafe
            {
                if (handle == null) return null;

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

        #region Editing

        /// <summary>
        /// Reverses the entire memory pointer.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// <remarks></remarks>
        public virtual bool Reverse()
        {
            if (handle == IntPtr.Zero || Size == 0)
                return false;

            long l = Size;

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
            long hl = this.Length;
            if (hl <= 0)
                return;

            if (0 > (index + length + offset) || (index + length + offset) > hl)
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.SafePtr.Slide().");
            }

            IntPtr p1;
            IntPtr p2;

            p1 = (IntPtr)((long)handle + index);
            p2 = (IntPtr)((long)handle + index + offset);

            long a = Math.Abs(offset);
            MemPtr m = new MemPtr(length);
            MemPtr n = new MemPtr(a);

            Native.MemCpy(p1, m.Handle, length);
            Native.MemCpy(p2, n.Handle, a);

            p1 = (IntPtr)((long)handle + index + offset + length);
            Native.MemCpy(n.Handle, p1, a);
            Native.MemCpy(m.Handle, p2, length);

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
            long hl = Size;
            if (Size == 0 || 0 > index || index >= (hl - 1))
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.MemPtr.PullIn().");
            }

            long a = index + amount;
            long b = Size - a;
            Slide(a, b, -amount);
            ReAlloc(hl - amount);
            return Size;
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
            long hl = this.Length;
            if (hl <= 0)
            {
                Alloc(amount);
                return amount;
            }

            if (0 > index || index > (hl - 1))
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.MemPtr.PushOut().");
            }

            long ol = Size - index;
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

            return Size;
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
            if (handle == IntPtr.Zero)
            {
                Alloc(amount);
                return;
            }

            long l = Size;
            if (l <= 0)
                return;

            long ol = l - index;
            ReAlloc(l + (amount * 1));

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
            long hl = Size;
            if (hl <= 0 || amount > index || index >= ((hl - amount) + 1))
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.Heap:Consume.");
            }

            index -= (amount + 1);
            PullIn(index, amount);
            index += (amount + 1);
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
            long hl = Size;
            if (hl <= 0 || amount > index || index >= (System.Convert.ToInt64(hl >> 1) - (amount + 1)))
            {
                throw new IndexOutOfRangeException("Index out of bounds DataTools.Memory.Heap:Consume.");
            }

            index -= (amount + 1) << 1;
            PullIn(index, amount);
            index += (amount + 1) << 1;
            PullIn(index, amount);
        }

        #endregion Editing


        /// <summary>
        /// Set all bytes in the buffer to zero at the optional index with the optional length.
        /// </summary>
        /// <param name="index">Start position of the buffer to zero, default is 0.</param>
        /// <param name="length">Size of the buffer to zero.  Default is to the end of the buffer.</param>
        /// <remarks></remarks>
        public void ZeroMemory(long index = 0, long length = 0)
        {
            long size = Size;
            if (size <= 0) return;

            long idx = index == -1 ? 0 : index;
            long len = length == -1 ? size - idx : length;

            // The length cannot be greater than the buffer length.
            if (len <= 0 || (idx + len) > size)
                return;

            unsafe
            {
                byte* bp1 = (byte*)((long)handle + idx);
                byte* bep = (byte*)((long)handle + idx + len);

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

            // Native.n_memset(handle, 0, (IntPtr)len);
        }

        /// <summary>
        /// Allocate a block of memory on a heap (typically the process heap).  
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
        /// <param name="hHeap">
        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
        /// If you use an alternate heap handle, you will need to free the memory using the same heap handle or an error will occur.
        /// </param>
        /// <param name="zeroMem">Whether or not to zero the contents of the memory on allocation.</param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public virtual bool Alloc(long size, bool addPressure = false, bool zeroMem = true)
        {
            bool al;

            handle = Marshal.AllocHGlobal((IntPtr)size);
            al = handle != IntPtr.Zero;

            // see if we need to tell the garbage collector anything.
            if (al && addPressure)
                GC.AddMemoryPressure(size);

            if (zeroMem)
            {
                ZeroMemory(0, size);
            }

            Size = size;
            return al;
        }

        /// <summary>
        /// Allocate a block of memory on the process heap.  
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public virtual bool Alloc(long size, bool addPressure)
        {
            return Alloc(size, addPressure, true);
        }


        /// <summary>
        /// Allocate a block of memory on the process heap.  
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public virtual bool Alloc(long size)
        {
            return Alloc(size, false, true);
        }

        /// <summary>
        /// (Deprecated) Allocate a block of memory and set its contents to zero.
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <param name="addPressure">Whether or not to call GC.AddMemoryPressure</param>
        /// <param name="hHeap">
        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
        /// If you use an alternate heap handle, you will need to free the memory using the same heap handle or an error will occur.
        /// </param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual bool AllocZero(long size, bool addPressure = false)
        {
            return Alloc(size, addPressure, true);
        }


        /// <summary>
        /// Reallocate a block of memory to a different size on the task heap.
        /// </summary>
        /// <param name="size">The size to attempt to allocate</param>
        /// <param name="modifyPressure">Whether or not to call GC.AddMemoryPressure or GC.RemoveMemoryPressure.</param>
        /// <param name="hHeap">
        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
        /// </param>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <remarks></remarks>
        public virtual bool ReAlloc(long size, bool modifyPressure = false)
        {
            if (handle == IntPtr.Zero) return Alloc(size, modifyPressure);
            long oldsize = Size;

            bool ra;

            // While the function doesn't need to call HeapReAlloc, it hasn't necessarily failed, either.

            handle = Marshal.ReAllocHGlobal(handle, new IntPtr(size));

            ra = handle != IntPtr.Zero;

            // see if we need to tell the garbage collector anything.
            if (ra && modifyPressure && oldsize > 0)
            {
                if (size < oldsize)
                    GC.RemoveMemoryPressure(oldsize - size);
                else
                    GC.AddMemoryPressure(size - oldsize);
            }

            Size = size;
            return ra;
        }

        /// <summary>
        /// Frees a previously allocated block of memory on the task heap.
        /// </summary>
        /// <returns>True if successful. If False, call GetLastError or FormatLastError to find out more information.</returns>
        /// <param name="removePressure">Whether or not to call GC.RemoveMemoryPressure</param>
        /// <param name="hHeap">
        /// Optional handle to an alternate heap.  The process heap is used if this is set to null.
        /// The handle pointed to by the internal pointer must have been previously allocated with the same heap handle.
        /// </param>
        /// <remarks></remarks>
        public virtual bool Free(bool removePressure = false)
        {
            long oldsize = Size;
            // While the function doesn't need to call HeapFree, it hasn't necessarily failed, either.
            if (handle == IntPtr.Zero)
                return true;
            else
            {
                // see if we need to tell the garbage collector anything.
                long l = oldsize;
                Marshal.FreeHGlobal(handle);

                // see if we need to tell the garbage collector anything.
                handle = IntPtr.Zero;
                if (removePressure && oldsize > 0) GC.RemoveMemoryPressure(oldsize);
                Size = 0;
            }
            return true;
        }


        protected virtual void TAlloc(long size)
        {
            switch (MemoryType)
            {
                case MemoryType.HGlobal:
                    Alloc(size, (size > 1024));
                    return;

                default:
                    Alloc(size, (size > 1024));
                    return;
            }
        }

        protected virtual void TFree()
        {
            switch (MemoryType)
            {
                case MemoryType.HGlobal:
                    Free();
                    return;

                default:
                    Free();
                    return;
            }
        }

        public virtual void ZeroMemory()
        {
            unsafe
            {
                void* p = (void*)handle;
                if (p == null || Size == 0) return;

                Native.ZeroMemory(p, Size);
            }
        }

        protected override bool ReleaseHandle()
        {
            TFree();
            return true;
        }

        public override string ToString()
        {
            if (handle == (IntPtr)0) return "";
            return GetString(0);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static explicit operator byte[](SafePtr val)
        {
            return val.ToByteArray();
        }

        public static explicit operator SafePtr(byte[] val)
        {
            var n = new SafePtr();
            n.FromByteArray(val);
            return n;
        }

        public static explicit operator char[](SafePtr val)
        {
            return val.ToCharArray();
        }

        public static explicit operator SafePtr(char[] val)
        {
            var n = new SafePtr();
            n.FromCharArray(val);
            return n;
        }

        public static explicit operator sbyte[](SafePtr val)
        {
            return val.ToArray<sbyte>();
        }

        public static explicit operator SafePtr(sbyte[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator short[](SafePtr val)
        {
            return val.ToArray<short>();
        }

        public static explicit operator SafePtr(short[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator ushort[](SafePtr val)
        {
            return val.ToArray<ushort>();
        }

        public static explicit operator SafePtr(ushort[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator int[](SafePtr val)
        {
            return val.ToArray<int>();
        }

        public static explicit operator SafePtr(int[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator uint[](SafePtr val)
        {
            return val.ToArray<uint>();
        }

        public static explicit operator SafePtr(uint[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator long[](SafePtr val)
        {
            return val.ToArray<long>();
        }

        public static explicit operator SafePtr(long[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator ulong[](SafePtr val)
        {
            return val.ToArray<ulong>();
        }

        public static explicit operator SafePtr(ulong[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator float[](SafePtr val)
        {
            return val.ToArray<float>();
        }

        public static explicit operator SafePtr(float[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator double[](SafePtr val)
        {
            return val.ToArray<double>();
        }

        public static explicit operator SafePtr(double[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator decimal[](SafePtr val)
        {
            return val.ToArray<decimal>();
        }

        public static explicit operator SafePtr(decimal[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator DateTime[](SafePtr val)
        {
            return val.ToArray<DateTime>();
        }

        public static explicit operator SafePtr(DateTime[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        public static explicit operator Guid[](SafePtr val)
        {
            return val.ToArray<Guid>();
        }

        public static explicit operator SafePtr(Guid[] val)
        {
            var n = new SafePtr();
            n.FromArray(val);
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator string(SafePtr val)
        {
            if (val?.handle == (IntPtr)0) return null;
            return val.GetString(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator SafePtr(string val)
        {
            var op = new SafePtr((val.Length + 1) * sizeof(char));
            op.SetString(0, val);
            return op;
        }

        public static SafePtr operator +(SafePtr val1, byte[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length);
            val1.FromByteArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, char[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(char));
            val1.FromCharArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, string val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(char));
            val1.FromCharArray(val2.ToCharArray(), c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, sbyte[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length);
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, short[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(short));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ushort[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(ushort));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, int[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(int));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, uint[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(uint));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, long[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(long));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ulong[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(ulong));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, float[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(float));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, double[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(double));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, decimal[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * sizeof(decimal));
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, DateTime[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * Marshal.SizeOf<DateTime>());
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, Guid[] val2)
        {
            var c = val1.Size;

            val1.Alloc(val1.Size + val2.Length * Marshal.SizeOf<Guid>());
            val1.FromArray(val2, c);

            return val1;
        }

        public static SafePtr operator +(SafePtr val1, short val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, short val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ushort val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, ushort val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, int val2)
        {
            val1.handle += val2;
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, int val2)
        {
            val1.handle -= val2;
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, long val2)
        {
            val1.handle = (IntPtr)((long)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, long val2)
        {
            val1.handle = (IntPtr)((long)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, uint val2)
        {
            val1.handle = (IntPtr)((uint)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, uint val2)
        {
            val1.handle = (IntPtr)((uint)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, ulong val2)
        {
            val1.handle = (IntPtr)((ulong)val1.handle + val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, ulong val2)
        {
            val1.handle = (IntPtr)((ulong)val1.handle - val2);
            return val1;
        }

        public static SafePtr operator +(SafePtr val1, IntPtr val2)
        {
            val1.handle = (IntPtr)((long)val1.handle + (long)val2);
            return val1;
        }

        public static SafePtr operator -(SafePtr val1, IntPtr val2)
        {
            val1.handle = (IntPtr)((long)val1.handle - (long)val2);
            return val1;
        }

        public static bool operator ==(IntPtr val1, SafePtr val2)
        {
            return (val1 == (val2?.handle ?? IntPtr.Zero));
        }

        public static bool operator !=(IntPtr val1, SafePtr val2)
        {
            return (val1 != (val2?.handle ?? IntPtr.Zero));
        }

        public static bool operator ==(SafePtr val2, IntPtr val1)
        {
            return (val1 == (val2?.handle ?? IntPtr.Zero));
        }

        public static bool operator !=(SafePtr val2, IntPtr val1)
        {
            return val1 != (val2?.handle ?? IntPtr.Zero);
        }

        public static implicit operator IntPtr(SafePtr val)
        {
            return val?.handle ?? IntPtr.Zero;
        }

        public static implicit operator SafePtr(IntPtr val)
        {
            unsafe
            {
                return new SafePtr
                {
                    handle = (IntPtr)(void*)val
                };
            }
        }

        public static implicit operator UIntPtr(SafePtr val)
        {
            unsafe
            {
                return (UIntPtr)(void*)val.handle;
            }
        }

        public static implicit operator SafePtr(UIntPtr val)
        {
            unsafe
            {
                return new SafePtr
                {
                    handle = (IntPtr)(void*)val
                };
            }
        }
    }
}