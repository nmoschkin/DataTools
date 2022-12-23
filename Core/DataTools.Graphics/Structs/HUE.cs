using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace DataTools.Graphics
{
    /// <summary>
    /// A structure to contain a hue.
    /// </summary>
    /// <remarks>
    /// A hue is a number that is greater than or equal to 0 and less than 360 where 360 and 0 are the same position.
    /// <br/><br/>
    /// The <see cref="IsGrayScale"/> property (equivalent to <see cref="Double.NaN"/>) indicates a gray-scale color with no hue component (Red, Green, and Blue are all equal).
    /// <br/><br/>
    /// Math operators are overridden to perform hue arithmatic where two numbers are compared and their maximum difference is 180.
    /// <br/><br/>
    /// When performing arithmatic, for instances where both hues are gray-scale, a gray-scale hue is returned. For instances where only one value is gray-scale, the value of the non-gray-scale hue is returned.
    /// <br/><br/>
    /// If a number is given that is out of bounds, it will be arithmatically adjusted to fall within the expected range.
    /// </remarks>
    public struct HUE
    {
        double value;

        /// <summary>
        /// Gets the pure color value for this hue, assuming that Saturation and Value are both 1.
        /// </summary>
        /// <remarks>
        /// In the case of gray-scale colors, the returned color is true gray (#7F7F7F in hexadecimal notation.)
        /// </remarks>
        public UniColor Color
        {
            get => (UniColor)new HSVDATA() { Hue = this, Saturation = 1, Value = 1 };
        }

        /// <summary>
        /// Gets a value indicating that the hue is a grayscale.
        /// </summary>
        public bool IsGrayScale
        {
            get
            {
                return double.IsNaN(value);
            }
        }

        /// <summary>
        /// Gets or sets the hue numeric value.
        /// </summary>
        /// <remarks>
        /// Setting this property to <see cref="double.NaN"/> will cause <see cref="IsGrayScale"/> to be true.<br /><br />
        /// Setting this property to <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/> will cause an <see cref="InvalidCastException"/> to be raised.
        /// </remarks>
        /// <exception cref="InvalidCastException"></exception>
        public double Value
        {
            get => value;
            set
            {
                if (double.IsInfinity(value)) throw new InvalidCastException("Infinity cannot be cast to HUE");

                if (!double.IsNaN(value))
                {
                    while (value >= 360)
                    {
                        value -= 360;
                    }
                    while (value < 0)
                    {
                        value += 360;
                    }
                }

                this.value = value;
            }
        }

        
        /// <summary>
        /// Create a new hue that is either gray-scale or not gray-scale.
        /// </summary>
        /// <param name="isGrayScale">True to set to gray-scale.</param>
        public HUE(bool isGrayScale)
        {
            value = isGrayScale ? double.NaN : 0;
        }

        /// <summary>
        /// Create a new hue with the given color value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <remarks>
        /// Passing <see cref="double.NaN"/> will cause <see cref="IsGrayScale"/> to be true.<br /><br />
        /// Passing <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/> will cause an <see cref="InvalidCastException"/> to be raised.
        /// </remarks>
        public HUE(double value)
        {
            if (double.IsInfinity(value)) throw new InvalidCastException("Infinity cannot be cast to HUE");

            if (!double.IsNaN(value))
            {
                while (value >= 360)
                {
                    value -= 360;
                }
                while (value < 0)
                {
                    value += 360;
                }
            }

            this.value = value;
        }

        /// <summary>
        /// Gets the distance between the current hue and another hue.
        /// </summary>
        /// <param name="other">The hue to compare.</param>
        /// <returns>An absolute value between 0 and 180 inclusive.</returns>
        public double GetDistance(HUE other)
        {
            if (IsGrayScale && other.IsGrayScale) return double.NaN;
            else if (IsGrayScale) return other.value;            
            else if (other.IsGrayScale) return value;

            double t1, t2;

            if (value >= 180)
            {
                t1 = -1 * (360 - value);
            }
            else
            {
                t1 = value;
            }

            if (other.value >= 180)
            {
                t2 = -1 * (360 - other.value);
            }
            else
            {
                t2 = other.value;
            }

            var t3 = (t1 - t2);
            return t3 >= 0 ? t3 : -t3;
        }

        public static HUE operator +(HUE value1, HUE value2)
        {
            if (value1.IsGrayScale && value2.IsGrayScale) return new HUE { value = double.NaN };
            else if (value1.IsGrayScale) return new HUE { value = value2.value };
            else if (value2.IsGrayScale) return new HUE { value = value1.value };

            var v = value1.value + value2.value;
            
            while (v >= 360)
            {
                v -= 360;
            }

            while (v < 0)
            {
                v += 360;
            }

            return new HUE() { value = v };
        }

        public static HUE operator -(HUE value1, HUE value2)
        {
            if (value1.IsGrayScale && value2.IsGrayScale) return new HUE { value = double.NaN };
            else if (value1.IsGrayScale) return new HUE { value = value2.value };
            else if (value2.IsGrayScale) return new HUE { value = value1.value };

            return new HUE() { value = value1.GetDistance(value2.value) };
        }

        public static bool operator ==(HUE value1, HUE value2)
        {
            if (value1.IsGrayScale && value2.IsGrayScale) return true;
            else if (value1.IsGrayScale || value2.IsGrayScale) return false;
            return value1.value == value2.value;
        }

        public static bool operator !=(HUE value1, HUE value2)
        {
            if (value1.IsGrayScale && value2.IsGrayScale) return false;
            else if (value1.IsGrayScale || value2.IsGrayScale) return true;
            return value1.value != value2.value;
        }

        public static bool operator >(HUE value1, HUE value2)
        {
            return value1.value > value2.value;
        }

        public static bool operator <(HUE value1, HUE value2)
        {
            return value1.value < value2.value;
        }

        public static bool operator >=(HUE value1, HUE value2)
        {
            return value1.value >= value2.value;
        }

        public static bool operator <=(HUE value1, HUE value2)
        {
            return value1.value <= value2.value;
        }

        public static implicit operator HUE(double value)
        {
            return new HUE(value);
        }

        public static implicit operator double(HUE value)
        {
            return value.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is HUE value2)
            {
                if (IsGrayScale && value2.IsGrayScale) return true;
                else if (IsGrayScale || value2.IsGrayScale) return false;
                return value == value2.value;
            }
            else if (obj is double d)
                return value.Equals(d);

            else return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
