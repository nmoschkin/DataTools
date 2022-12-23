using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Text
{
    public static class StringExtensions
    {

        /// <summary>
        /// Partition a string into an array of strings of the specified length.
        /// </summary>
        /// <param name="s">The string to partition.</param>
        /// <param name="length">The length of the partition.</param>
        /// <returns>An array of string</returns>
        /// <remarks>
        /// The last element may be shorter than the value specified by the length parameter.
        /// </remarks>
        public static string[] Partition(this string s, int length)
        {
            List<string> lw = new List<string>();

            if (s.Length <= length) return new string[] { s };

            int i, c = s.Length;

            for (i = 0; i < c; i += length)
            {
                if (i + length >= s.Length)
                {
                    lw.Add(s.Substring(i));
                }
                else
                {
                    lw.Add(s.Substring(i, length));
                }
            }

            return lw.ToArray();
        }
    
        /// <summary>
        /// Parse a string into an array of strings using the specified parameters
        /// </summary>
        /// <param name="Scan">String to scan</param>
        /// <param name="Separator">Separator string</param>
        /// <param name="SkipQuote">Whether to skip over quote blocks</param>
        /// <param name="Unescape">Whether to unescape quotes.</param>
        /// <param name="QuoteChar">Quote character to use.</param>
        /// <param name="EscapeChar">Escape character to use.</param>
        /// <param name="WithToken">Include the token in the return array.</param>
        /// <param name="WithTokenIn">Attach the token to the beginning of every string separated by a token (except for string 0).  Requires WithToken to also be set to True.</param>
        /// <param name="Unquote">Remove quotes from around characters.</param>
        /// <returns>An array of strings.</returns>
        /// <remarks></remarks>
        public static string[] Split(this string Scan, string Separator, bool SkipQuote = false, bool Unescape = false, char QuoteChar = '"', char EscapeChar = '\\', bool Unquote = false, bool WithToken = false, bool WithTokenIn = false)
        {
            return TextTools.Split(Scan, Separator, SkipQuote, Unescape, QuoteChar, EscapeChar, Unquote, WithToken, WithTokenIn);
        }

        /// <summary>
        /// Print an enumeration value's description.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The contents of the <see cref="DescriptionAttribute"/> or the result of <see cref="Enum.ToString"/>.</returns>
        public static string Print(this Enum value)
        {
            return TextTools.PrintEnumDesc(value);
        }

    }
}
