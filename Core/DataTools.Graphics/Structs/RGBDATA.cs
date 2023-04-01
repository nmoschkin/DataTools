using DataTools.Graphics.Structs;

using System;
using System.Text.RegularExpressions;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents 3 bytes of Red, Green, Blue color data.
    /// </summary>
    public struct RGBDATA : IParseableColorData<RGBDATA>
    {
        /// <summary>
        /// Blue Channel
        /// </summary>
        public byte Blue;

        /// <summary>
        /// Green Channel
        /// </summary>
        public byte Green;

        /// <summary>
        /// Red Channel
        /// </summary>
        public byte Red;

        /// <summary>
        /// Get RGB data from a color
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RGBDATA FromColor(UniColor value)
        {
            ColorMath.ColorToRGB(value, out var d);
            return d;
        }

        /// <summary>
        /// Parse a string into RGB data
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static RGBDATA Parse(string value)
        {
            value = value.ToUpper().Trim();

            var rx = new Regex(@"RGB\s*\((\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)");
            var mg = rx.Match(value);

            if (mg.Success)
            {
                return new RGBDATA()
                {
                    Red = byte.Parse(mg.Groups[1].Value),
                    Green = byte.Parse(mg.Groups[2].Value),
                    Blue = byte.Parse(mg.Groups[3].Value)
                };
            }
            else
            {
                rx = new Regex(@"([A-Fa-f0-9]+){2}([A-Fa-f0-9]+){2}([A-Fa-f0-9]+){2}");
                mg = rx.Match(value);

                if (mg.Success)
                {
                    return new RGBDATA()
                    {
                        Red = byte.Parse(mg.Groups[1].Value, System.Globalization.NumberStyles.HexNumber),
                        Green = byte.Parse(mg.Groups[2].Value, System.Globalization.NumberStyles.HexNumber),
                        Blue = byte.Parse(mg.Groups[3].Value, System.Globalization.NumberStyles.HexNumber)
                    };
                }

                throw new FormatException();
            }
        }

        /// <summary>
        /// Create a color from RGB data
        /// </summary>
        /// <returns></returns>
        public UniColor CreateColor()
        {
            return new UniColor()
            {
                R = Red,
                G = Green,
                B = Blue,
                A = 255
            };
        }

        /// <inheritdoc/>
        public bool Equals(RGBDATA other)
        {
            return Red == other.Red && Green == other.Green && Blue == other.Blue;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"RGB({Red}, {Green}, {Blue})";
        }

        /// <inheritdoc/>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "x")
            {
                return $"#{Red:x2}{Green:x2}{Blue:x2}";
            }
            else if (format == "X")
            {
                return $"#{Red:X2}{Green:X2}{Blue:X2}";
            }
            else
            {
                return ToString();
            }
        }
    }
}