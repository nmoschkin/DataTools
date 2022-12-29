using System;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct HSVDATA : IEquatable<HSVDATA>, IComparable<HSVDATA>
    {
        [FieldOffset(0)]
        public Hue Hue;

        [FieldOffset(8)]
        public double Saturation;

        [FieldOffset(16)]
        public double Value;

        public HSVDATA(Hue h, double s, double v)
        {
            Hue = h;
            Saturation = s;
            Value = v;
        }

        public static explicit operator UniColor(HSVDATA h)
        {
            return ColorMath.HSVToColor(h);
        }

        public static explicit operator HSVDATA(UniColor c)
        {
            return ColorMath.ColorToHSV(c);
        }

        public override int GetHashCode()
        {
            return (Hue, Saturation, Value).GetHashCode();
        }

        public bool Equals(HSVDATA other)
        {
            return (other.Hue == Hue && other.Saturation == Saturation && other.Value == Value);
        }

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

        public override string ToString()
        {
            return $"HSV({Hue}, {Saturation}, {Value})";
        }

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

        public HSVDATA Abs()
        {
            return new HSVDATA(Math.Abs(Hue), Math.Abs(Saturation), Math.Abs(Value));
        }

        public static HSVDATA operator +(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue + v2.Hue, v1.Saturation + v2.Saturation, v1.Value + v2.Value);
        }

        public static HSVDATA operator -(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue - v2.Hue, v1.Saturation - v2.Saturation, v1.Value - v2.Value);
        }

        public static HSVDATA operator /(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue / v2.Hue, v1.Saturation / v2.Saturation, v1.Value / v2.Value);
        }

        public static HSVDATA operator *(HSVDATA v1, HSVDATA v2)
        {
            return new HSVDATA(v1.Hue * v2.Hue, v1.Saturation * v2.Saturation, v1.Value * v2.Value);
        }

        public static HSVDATA operator +(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue + v2, v1.Saturation + v2, v1.Value + v2);
        }

        public static HSVDATA operator -(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue - v2, v1.Saturation - v2, v1.Value - v2);
        }

        public static HSVDATA operator /(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue / v2, v1.Saturation / v2, v1.Value / v2);
        }

        public static HSVDATA operator *(HSVDATA v1, double v2)
        {
            return new HSVDATA(v1.Hue * v2, v1.Saturation * v2, v1.Value * v2);
        }

        public static bool operator ==(HSVDATA v1, HSVDATA v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(HSVDATA v1, HSVDATA v2)
        {
            return !v1.Equals(v2);
        }

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