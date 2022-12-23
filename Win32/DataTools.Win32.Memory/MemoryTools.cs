using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Memory
{
    public static class MemoryTools
    {

        /// <summary>
        /// Returns true if the handle is one of the invalid handle values (<see cref="nint.Zero"/> or (<see cref="nint"/>)(-1).)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInvalidHandle(this nint value)
        {
            return (value == nint.Zero || value == (nint)(-1));
        }


        /// <summary>
        /// Convert an array of bytes to a structure
        /// </summary>
        /// <param name="input">An arrayou of bytes</param>
        /// <param name="output">A valid structure object (cannot be null)</param>
        /// <param name="startingIndex">the starting index in the array of bytes.</param>
        /// <param name="length">Length in the array of bytes.</param>
        /// <returns>True if successful</returns>
        public static bool BytesToStruct(byte[] input, ref object output, int startingIndex = 0, int length = -1)
        {
            try
            {
                var ptr = Marshal.AllocHGlobal(input.Length);
                if (length == -1)
                    length = input.Length - startingIndex;
                Marshal.Copy(input, startingIndex, ptr, length);
                output = Marshal.PtrToStructure(ptr, output.GetType());
                Marshal.FreeHGlobal(ptr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes a structure to a byte array
        /// </summary>
        /// <param name="input">Structure</param>
        /// <param name="output">Byte array</param>
        /// <returns>True if successful</returns>
        public static bool StructToBytes(object input, ref byte[] output)
        {
            try
            {
                int a = Marshal.SizeOf(input);
                var ptr = Marshal.AllocHGlobal(a);
                output = new byte[a];
                Marshal.StructureToPtr(input, ptr, false);
                Marshal.Copy(ptr, output, 0, a);
                Marshal.FreeHGlobal(ptr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Write a structure to a byte array, and return the byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns><see cref="Byte()"/></returns>
        public static byte[] StructToBytes(object input)
        {
            try
            {
                int a = Marshal.SizeOf(input);
                var ptr = Marshal.AllocHGlobal(a);
                byte[] output = null;
                output = new byte[a];
                Marshal.StructureToPtr(input, ptr, false);
                Marshal.Copy(ptr, output, 0, a);
                Marshal.FreeHGlobal(ptr);
                return output;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// Read a structure from a stream
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> object</param>
        /// <param name="offset">Offset inside the stream to begin reading the struct.</param>
        /// <param name="struct">The output struct.</param>
        /// <returns>True if successful</returns>
        public static bool ReadStruct(Stream stream, int offset, ref object @struct)
        {
            try
            {
                int a = Marshal.SizeOf(@struct);
                byte[] b;
                b = new byte[a];
                stream.Read(b, offset, a);
                return BytesToStruct(b, ref @struct);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Read a structure of type T from a stream
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> object</param>
        /// <param name="struct">The output struct of type T</param>
        /// <returns>True if successful</returns>
        public static bool ReadStruct<T>(Stream stream, ref T @struct) where T : struct
        {
            try
            {
                int a = Marshal.SizeOf<T>();
                byte[] b;
                b = new byte[a];
                stream.Read(b, 0, a);
                var gch = GCHandle.Alloc(b, GCHandleType.Pinned);
                MemPtr mm = gch.AddrOfPinnedObject();
                @struct = mm.ToStruct<T>();
                gch.Free();
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
