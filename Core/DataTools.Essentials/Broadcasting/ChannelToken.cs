using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Represents a channel token
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct ChannelToken : IEquatable<ChannelToken>, IComparable<ChannelToken>
    {
        /// <summary>
        /// Gets or sets the MD5 seed to use when generating tokens
        /// </summary>
        public static Guid RuntimeSeed { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Return an empty channel token
        /// </summary>
        public static readonly ChannelToken Empty = CreateToken("");

        [FieldOffset(0)]
        private Guid guid;

        /// <summary>
        /// Create a channel token from the specified string
        /// </summary>
        /// <param name="stringValue">The string value to create the token from</param>
        /// <returns></returns>
        public static ChannelToken CreateToken(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return new ChannelToken { guid = Guid.Empty };
            }

            var md5 = new HMACMD5(RuntimeSeed.ToByteArray());
            var ct = new ChannelToken();

            ct.guid = new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(stringValue)));

            return ct;
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
        /// Gets the bytes of the structure
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return guid.ToByteArray();
        }

        /// <summary>
        /// Convert the token to a <see cref="Guid"/> structure
        /// </summary>
        /// <returns></returns>
        public Guid ToGuid() => guid;

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            var bytes = GetBytes();
            int i = 0;

            foreach (var b in bytes)
            {
                if (i > 0) sb.Append(" ");
                sb.Append($"{b:X2}");
                i++;
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
            return guid.Equals(other.guid);
        }

        /// <inheritdoc/>
        public int CompareTo(ChannelToken other)
        {
            return InnerCompare(this, other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        /// <summary>
        /// Test equality of two channel tokens
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ChannelToken a, ChannelToken b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Test inequality of two channel tokens
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ChannelToken a, ChannelToken b)
        {
            return !a.Equals(b);
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
        /// Compare two tokens and return their relationship as an integer
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int InnerCompare(ChannelToken a, ChannelToken b)
        {
            unsafe
            {
                long* aptr = (long*)&a;
                long* bptr = (long*)&b;

                for (byte i = 0; i < 2; i++)
                {
                    if (*aptr < *bptr) return -1;
                    else if (*aptr > *bptr) return 1;

                    aptr++;
                    bptr++;
                }
            }

            return 0;
        }
    }
}
