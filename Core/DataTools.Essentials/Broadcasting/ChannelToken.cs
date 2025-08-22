using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DataTools.Essentials.Broadcasting
{

    /// <summary>
    /// Represents a channel token
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 1)]
    public struct ChannelToken : IEquatable<ChannelToken>, IComparable<ChannelToken>
    {
        [FieldOffset(0)]
        private ulong _l;

        [FieldOffset(8)]
        private ulong _h;

        /// <summary>
        /// Gets or sets the MD5 seed to use when generating tokens
        /// </summary>
        public static Guid RuntimeSeed { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Return an empty channel token
        /// </summary>
        public static readonly ChannelToken Empty = CreateToken("");

        /// <summary>
        /// Create a channel token from the specified string
        /// </summary>
        /// <param name="stringValue">The string value to create the token from</param>
        /// <returns></returns>
        public static ChannelToken CreateToken(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return new ChannelToken();
            }

            var md5 = new HMACMD5(RuntimeSeed.ToByteArray());

            unsafe
            {
                return new ChannelToken(md5.ComputeHash(Encoding.UTF8.GetBytes(stringValue)));                
            }
        }

        /// <summary>
        /// Creates a randomly-generated channel token
        /// </summary>
        /// <returns></returns>
        public static ChannelToken CreateToken()
        {
            return CreateToken(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Create a new channel token from the specified bytes
        /// </summary>
        /// <param name="bytes"></param>
        public ChannelToken(byte[] bytes)
        {
            unsafe
            {
                fixed (void *bs = bytes)
                {
                    fixed (ChannelToken *ts = &this)
                    {
                        *ts = *(ChannelToken*)bs;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the bytes of the structure
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            unsafe
            {
                var bytes = new byte[16];

                fixed (void* bs = bytes)
                {
                    fixed (ChannelToken* ts = &this)
                    {
                        *(ChannelToken*)bs = *ts;
                    }
                }

                return bytes;
            }
        }

        /// <summary>
        /// Convert the token to a <see cref="Guid"/> structure
        /// </summary>
        /// <returns></returns>
        public Guid ToGuid() => new Guid(ToByteArray());

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();

            unsafe
            {
                fixed (void* bs = &this)
                {
                    short* bptr = (short*)bs;

                    for (var i = 0; i < 8; i++)
                    {
                        if (i > 0) sb.Append(':');
                        sb.Append($"{*bptr:X4}");
                        bptr++;

                    }
                }
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is ChannelToken ct)
            {
                return Equals(ct);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Equals(ChannelToken other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public int CompareTo(ChannelToken other)
        {
            return InnerCompare(this, other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unsafe
            {
                int hc = -1;

                fixed (void* bs = &this)
                {
                    int* bptr = (int*)bs;

                    hc ^= *bptr++;
                    hc <<= 3;
                    hc ^= *bptr++ >> 3;
                    hc <<= 3;
                    hc ^= *bptr++ >> 3;
                    hc <<= 3;
                    hc ^= *bptr++ >> 3;

                    return hc;
                }
            }
        }

        /// <summary>
        /// Test equality of two channel tokens
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ChannelToken a, ChannelToken b)
        {
            unsafe
            {
                ulong* aptr = (ulong*)&a;
                ulong* bptr = (ulong*)&b;

                return (*aptr ^ *bptr) == 0 && (*(aptr + 1) ^ *(bptr + 1)) == 0;
            }
        }

        /// <summary>
        /// Test inequality of two channel tokens
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ChannelToken a, ChannelToken b)
        {
            unsafe
            {
                ulong* aptr = (ulong*)&a;
                ulong* bptr = (ulong*)&b;

                return (*aptr ^ *bptr) != 0 || (*(aptr + 1) ^ *(bptr + 1)) != 0;
            }
        }

        /// <summary>
        /// Test if a greater than b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(ChannelToken a, ChannelToken b)
        {
            return InnerCompare(a, b) > 0;
        }

        /// <summary>
        /// Test if a greater than or equal to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >=(ChannelToken a, ChannelToken b)
        {
            return InnerCompare(a, b) >= 0;
        }

        /// <summary>
        /// Test if a less than b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <(ChannelToken a, ChannelToken b)
        {
            return InnerCompare(a, b) < 0;
        }

        /// <summary>
        /// Test if a less than or equal to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <=(ChannelToken a, ChannelToken b)
        {
            return InnerCompare(a, b) <= 0;
        }

        /// <summary>
        /// Bitwise OR
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ChannelToken operator |(ChannelToken a, ChannelToken b)
        {
            unsafe
            {
                ulong* aptr = (ulong*)&a;
                ulong* bptr = (ulong*)&b;

                *aptr |= *bptr;
                *(aptr + 1) |= *(bptr + 1);

                return a;
            }
        }

        /// <summary>
        /// Bitwise AND
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ChannelToken operator &(ChannelToken a, ChannelToken b)
        {
            unsafe
            {
                ulong* aptr = (ulong*)&a;
                ulong* bptr = (ulong*)&b;

                *aptr &= *bptr;
                *(aptr + 1) &= *(bptr + 1);

                return a;
            }
        }

        /// <summary>
        /// Bitwise XOR
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ChannelToken operator ^(ChannelToken a, ChannelToken b)
        {
            unsafe
            {
                ulong* aptr = (ulong*)&a;
                ulong* bptr = (ulong*)&b;

                *aptr ^= *bptr;
                *(aptr + 1) ^= *(bptr + 1);

                return a;
            }
        }


        /// <summary>
        /// Bitwise NOT
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ChannelToken operator ~(ChannelToken val) 
        {
            unsafe
            {
                ulong* aptr = (ulong*)&val;

                *aptr ^= 0xffffffffffffffff;
                *(aptr + 1) ^= 0xffffffffffffffff;

                return val;
            }
        }
       
        /// <summary>
        /// Compare two tokens and return their relationship as a sorting integer (-1,0,1)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int InnerCompare(ChannelToken a, ChannelToken b)
        {
            unsafe
            {
                ushort* aptr = (ushort*)&a;
                ushort* bptr = (ushort*)&b;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;

                if (*aptr < *bptr) return -1;
                else if (*aptr++ > *bptr++) return 1;
            }

            return 0;
        }
    }
}
