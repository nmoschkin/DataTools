using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents color data in HSV format
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct HSVDATA : IEquatable<HSVDATA>, IComparable<HSVDATA>
    {

        /// <summary>
        /// Hue
        /// </summary>
        /// <remarks>
        /// This is a <see cref="DataTools.Graphics.Hue"/> value
        /// </remarks>
        [FieldOffset(0)]
        public Hue Hue;

        /// <summary>
        /// Saturation
        /// </summary>
        [FieldOffset(8)]
        public double Saturation;

        /// <summary>
        /// Value
        /// </summary>
        [FieldOffset(16)]
        public double Value;

        /// <summary>
        /// Create a new HSV structure
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        public HSVDATA(Hue h, double s, double v)
        {
            Hue = h;
            Saturation = s;
            Value = v;
        }

        /// <summary>
        /// Cast to unicolor
        /// </summary>
        /// <param name="h"></param>
        public static explicit operator UniColor(HSVDATA h)
        {
            return ColorMath.HSVToColor(h);
        }

        /// <summary>
        /// Cast from unicolor
        /// </summary>
        /// <param name="c"></param>
        public static explicit operator HSVDATA(UniColor c)
        {
            return ColorMath.ColorToHSV(c);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (Hue, Saturation, Value).GetHashCode();
        }

        /// <inheritdoc/>
        public bool Equals(HSVDATA other)
        {
            return (other.Hue == Hue && other.Saturation == Saturation && other.Value == Value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is HSVDATA h)
            {
                return Equals(h);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Convert to a string with the form 'HSV(h,s,v)'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"HSV({Hue},{Saturation},{Value})";
        }

        /// <inheritdoc/>
        public int CompareTo(HSVDATA other)
        {
            var r = Hue.CompareTo(other.Hue);

            if (r == 0)
            {
                r = Saturation.CompareTo(other.Saturation);

                if (r == 0)
                {
                    r = Value.CompareTo(other.Value);
                }
            }

            return r;
        }

        /// <summary>
        /// Converts negative numbers to absolute values and returns the new structure
        /// </summary>
        /// <returns></returns>
        public HSVDATA Abs()
        {
            return new HSVDATA(Hue, Math.Abs(Saturation), Math.Abs(Value));
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static HSVDATA operator +(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue + v2.Hue, v1.Saturation + v2.Saturation, v1.Value + v2.Value);
        }

        /// <summary>
        /// Subtract
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static HSVDATA operator -(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue - v2.Hue, v1.Saturation - v2.Saturation, v1.Value - v2.Value);
        }

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static HSVDATA operator /(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue / v2.Hue, v1.Saturation / v2.Saturation, v1.Value / v2.Value);
        }

        /// <summary>
        /// Multiply
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static HSVDATA operator *(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue * v2.Hue, v1.Saturation * v2.Saturation, v1.Value * v2.Value);
        }

        /// <inheritdoc/>
        public static HSVDATA operator +(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue + v2, v1.Saturation + v2, v1.Value + v2);
        }

        /// <inheritdoc/>
        public static HSVDATA operator -(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue - v2, v1.Saturation - v2, v1.Value - v2);
        }

        /// <inheritdoc/>
        public static HSVDATA operator /(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue / v2, v1.Saturation / v2, v1.Value / v2);
        }

        /// <inheritdoc/>
        public static HSVDATA operator *(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue * v2, v1.Saturation * v2, v1.Value * v2);
        }

        /// <inheritdoc/>
        public static bool operator ==(HSVDATA v1, HSVDATA v2)
        {
            return v1.Equals(v2);
        }

        /// <inheritdoc/>
        public static bool operator !=(HSVDATA v1, HSVDATA v2)
        {
            return !v1.Equals(v2);
        }

        /// <inheritdoc/>
        public static bool operator >(HSVDATA v1, HSVDATA v2)
        {
            if (v1.Hue == v2.Hue)
            {
                if (v1.Saturation == v2.Saturation)
                {
                    if (v1.Value <= v2.Value)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return v1.Saturation > v2.Saturation;
                }
            }
            else
            {
                return v1.Hue > v2.Hue;
            }
        }

        /// <inheritdoc/>
        public static bool operator >=(HSVDATA v1, HSVDATA v2)
        {
            if (v1.Hue == v2.Hue)
            {
                if (v1.Saturation == v2.Saturation)
                {
                    if (v1.Value < v2.Value)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return v1.Saturation >= v2.Saturation;
                }
            }
            else
            {
                return v1.Hue >= v2.Hue;
            }
        }

        /// <inheritdoc/>
        public static bool operator <(HSVDATA v1, HSVDATA v2)
        {
            if (v1.Hue == v2.Hue)
            {
                if (v1.Saturation == v2.Saturation)
                {
                    if (v1.Value >= v2.Value)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return v1.Saturation < v2.Saturation;
                }
            }
            else
            {
                return v1.Hue < v2.Hue;
            }
        }

        /// <inheritdoc/>
        public static bool operator <=(HSVDATA v1, HSVDATA v2)
        {
            if (v1.Hue == v2.Hue)
            {
                if (v1.Saturation == v2.Saturation)
                {
                    if (v1.Value > v2.Value)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return v1.Saturation <= v2.Saturation;
                }
            }
            else
            {
                return v1.Hue <= v2.Hue;
            }
        }
    }
}