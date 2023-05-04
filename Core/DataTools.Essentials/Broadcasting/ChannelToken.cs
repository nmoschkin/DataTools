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
    public struct ChannelToken : IEquatable<ChannelToken>
    {
        private static readonly Guid runtimeSeed = Guid.NewGuid();

        [FieldOffset(0)]
        private Guid guid;

        /// <summary>
        /// Create a channel token from the specified string
        /// </summary>
        /// <param name="stringValue">The string value to create the token from</param>
        /// <returns></returns>
        public static ChannelToken CreateToken(string stringValue)
        {
            HMACMD5 hMACMD5 = new HMACMD5(runtimeSeed.ToByteArray());
            var ct = new ChannelToken();

            ct.guid = new Guid(hMACMD5.ComputeHash(Encoding.UTF8.GetBytes(stringValue)));
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


    }
}
