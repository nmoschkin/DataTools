// ColorMath class.  Copyright (C) 1999-2022 Nathaniel Moschkin.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

using static DataTools.MathTools.MathLib;

namespace DataTools.Graphics
{
    /// <summary>
    /// Perform calculations on colors
    /// </summary>
    public class ColorMath
    {
        internal const int SF_VALUE = 0x0;
        internal const int SF_SATURATION = 0x1;
        internal const int SF_HUE = 0x2;

        /// <summary>
        /// Reads the Red, Green, Blue values from the <see cref="UniColor"/>
        /// </summary>
        /// <param name="color">The <see cref="UniColor"/> value.</param>
        /// <param name="red">Receives the red byte value.</param>
        /// <param name="green">Receives the green byte value.</param>
        /// <param name="blue">Receives the blue byte value.</param>
        public static void GetRGB(UniColor color, out byte red, out byte green, out byte blue)
        {
            red = color.R;
            green = color.G;
            blue = color.B;
        }

        // Single Convert ColorRef to RGB

        /// <summary>
        /// Converts the <see cref="UniColor"/> to an <see cref="RGBDATA"/> structure
        /// </summary>
        /// <param name="color">The <see cref="UniColor"/> value.</param>
        /// <param name="data">The <see cref="RGBDATA"/></param>
        public static void ColorToRGB(UniColor color, out RGBDATA data)
        {
            data = new RGBDATA()
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B,
            };
        }

        /// <summary>
        /// Converts the <see cref="UniColor"/> to an <see cref="ARGBDATA"/> structure
        /// </summary>
        /// <param name="color"></param>
        /// <param name="data">The <see cref="ARGBDATA"/></param>
        public static void ColorToARGB(UniColor color, out ARGBDATA data)
        {
            data = new ARGBDATA()
            {
                Alpha = color.A,
                Red = color.R,
                Green = color.G,
                Blue = color.B,
            };
        }

        // Single Convert RGB to ColorRef

        /// <summary>
        /// Converts an <see cref="RGBDATA"/> structure to a <see cref="UniColor"/>
        /// </summary>
        /// <param name="data">The <see cref="RGBDATA"/></param>
        /// <returns></returns>
        public static UniColor RGBToColor(RGBDATA data)
        {
            return new UniColor()
            {
                R = data.Red,
                G = data.Green,
                B = data.Blue,
                A = 0xff
            };
        }

        /// <summary>
        /// Converts an <see cref="ARGBDATA"/> structure to a <see cref="UniColor"/>
        /// </summary>
        /// <param name="data">The <see cref="ARGBDATA"/></param>
        /// <returns></returns>
        public static UniColor ARGBToColor(ARGBDATA data)
        {
            return new UniColor()
            {
                A = data.Alpha,
                R = data.Red,
                G = data.Green,
                B = data.Blue,
            };
        }

        // Single Convert ColorRef to RGB-reversed

        /// <summary>
        /// Converts the <see cref="UniColor"/> to a <see cref="BGRDATA"/> structure
        /// </summary>
        /// <param name="color"></param>
        /// <param name="data">The <see cref="BGRDATA"/></param>
        public static void ColorToBGR(UniColor color, out BGRDATA data)
        {
            data = new BGRDATA()
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
        }

        /// <summary>
        /// Converts the <see cref="UniColor"/> to a <see cref="BGRADATA"/> structure
        /// </summary>
        /// <param name="color"></param>
        /// <param name="data">The <see cref="BGRADATA"/></param>
        public static void ColorToBGRA(UniColor color, out BGRADATA data)
        {
            data = new BGRADATA()
            {
                Alpha = color.A,
                Red = color.R,
                Green = color.G,
                Blue = color.B,
            };
        }

        // Single Convert RGB-reversed to ColorRef

        /// <summary>
        /// Converts a <see cref="BGRADATA"/> structure to a <see cref="UniColor"/>
        /// </summary>
        /// <param name="data">The <see cref="BGRADATA"/></param>
        /// <returns></returns>
        public static UniColor BGRAToColor(BGRADATA data)
        {
            return new UniColor()
            {
                A = data.Alpha,
                R = data.Red,
                G = data.Green,
                B = data.Blue
            };
        }

        /// <summary>
        /// Converts a <see cref="BGRDATA"/> structure to a <see cref="UniColor"/>
        /// </summary>
        /// <param name="data">The <see cref="BGRADATA"/></param>
        /// <returns></returns>
        public static UniColor BGRToColor(BGRDATA data)
        {
            return new UniColor()
            {
                R = data.Red,
                G = data.Green,
                B = data.Blue,
                A = 0xff
            };
        }

        /// <summary>
        /// Converts a <see cref="UniColor"/> to a <see cref="HSVDATA"/> structure
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static HSVDATA ColorToHSV(UniColor color)
        {
            ColorToHSV(color, out var hsv);
            return hsv;
        }

        /// <summary>
        /// Converts a <see cref="UniColor"/> to a <see cref="HSVDATA"/> structure
        /// </summary>
        /// <param name="color"></param>
        /// <param name="hsv"></param>
        public static void ColorToHSV(UniColor color, out HSVDATA hsv)
        {
            double s, v;
            Hue h;
            ColorToHSV(color, out h, out s, out v);

            hsv = new HSVDATA
            {
                Hue = h,
                Saturation = s,
                Value = v
            };
        }

        /// <summary>
        /// Calculates Hue, Saturation, Value for the <see cref="UniColor"/>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="hue">This is a <see cref="Hue"/> number.</param>
        /// <param name="saturation"></param>
        /// <param name="value"></param>
        public static void ColorToHSV(UniColor color, out Hue hue, out double saturation, out double value)
        {
            hue = default(double);

            double sat;
            double val;
            double Mn;
            double Mx;
            double r;
            double g;
            double b;
            double chroma;

            r = color.R / 255d;
            g = color.G / 255d;
            b = color.B / 255d;

            Mn = Min(r, g, b);
            Mx = Max(r, g, b);

            chroma = Mx - Mn;
            val = Mx;

            if (chroma == 0)
            {
                hue = new Hue(true);

                switch (val)
                {
                    case var @case when @case <= 0.5d:
                        saturation = 1d;
                        value = 510d * val / 360d;
                        break;

                    case var case1 when case1 <= 1d:
                    default:
                        val = 1d - val;
                        value = 1d;
                        saturation = 720d * val / 360d;
                        break;
                }

                return;
            }

            if (Mx == r)
            {
                hue = (g - b) / chroma % 6d;
            }
            else if (Mx == g)
            {
                hue = (b - r) / chroma + 2d;
            }
            else if (Mx == b)
            {
                hue = (r - g) / chroma + 4d;
            }

            hue *= 60d;
            if (hue < 0d)
                hue = 360d + hue;

            sat = val != 0 ? chroma / val : 0;

            value = val;
            saturation = sat;
        }

        public static UniColor HSVToColor(HSVDATA hsv)
        {
            int c = 0;
            HSVToColorRaw(hsv, ref c);
            return c;
        }

        public static void HSVToColor(HSVDATA hsv, ref UniColor dest)
        {
            // preserve alpha
            var a = dest.A;
            dest.SetValue(HSVToColorRaw(hsv));
            dest.A = a;
        }

        /// <summary>
        /// Convert <see cref="HSVDATA"/> into a raw 32-bit color value.
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns></returns>
        public static int HSVToColorRaw(HSVDATA hsv)
        {
            int c = 0;
            HSVToColorRaw(hsv, ref c);
            return c;
        }

        /// <summary>
        /// Convert <see cref="HSVDATA"/> into a raw 32-bit color value.
        /// </summary>
        /// <param name="hsv"></param>
        /// <param name="retColor"></param>
        public static void HSVToColorRaw(HSVDATA hsv, ref int retColor)
        {
            HSVToColorRaw(hsv.Hue, hsv.Saturation, hsv.Value, ref retColor);
        }

        /// <summary>
        /// Convert the raw Hue, Saturation, and Value into a raw 32-bit ARGB color <see cref="uint"/>
        /// </summary>
        /// <param name="hue">A <see cref="Hue"/> value between 0 and 360.</param>
        /// <param name="saturation">A <see cref="double"/> value from 0 to 1</param>
        /// <param name="value">A <see cref="double"/> value from 0 to 1</param>
        /// <param name="retColor">The returned 32-bit ARGB color <see cref="uint"/> value</param>
        /// <remarks>
        /// I adapted the equation from the one in Wikipedia:<br />
        /// http://en.wikipedia.org/wiki/HSL_and_HSV#Hue_and_chroma<br /><br />
        /// I wish I could offer a better explanation, but they do a better job.
        /// </remarks>
        public static void HSVToColorRaw(Hue hue, double saturation, double value, ref int retColor)
        {
            unchecked
            {
                double a;
                double b;
                double c;

                int ab;
                int bb;
                int cb;

                double chroma;
                double Mx;
                double Mn;

                int j = (int)0xFF000000;

                double n;

                if (hue.IsGrayScale)
                {
                    if (saturation > value)
                    {
                        saturation = 1d;
                        n = value * 360d / 510d;
                    }
                    else
                    {
                        n = 1d - saturation * 360d / 720d;
                    }

                    ab = (int)(n * 255d);

                    retColor = j | (ab << 16) | (ab << 8) | ab;
                    return;
                }

                //while (hue.Value >= 360d)
                //    hue -= 360d;

                chroma = value * saturation;

                Mn = Math.Abs(value - chroma);
                Mx = value;

                n = hue / 60d;

                a = Mx;

                c = Mn;

                b = chroma * (1d - Math.Abs(n % 2d - 1d));
                b += c;

                // fit the color space in to byte space.
                ab = (int)Math.Round(a * 255d);
                bb = (int)Math.Round(b * 255d);
                cb = (int)Math.Round(c * 255d);

                // Get the floored value of n
                n = Math.Floor(n);
                switch (n)
                {
                    case 0d:
                    case 6d: // 0, 360 - Red
                        j = j | ab << 16 | bb << 8 | cb;
                        break;

                    case 1d: // 60 - Yellow
                        j = j | bb << 16 | ab << 8 | cb;
                        break;

                    case 2d: // 120 - Green
                        j = j | cb << 16 | ab << 8 | bb;
                        break;

                    case 3d: // 180 - Cyan
                        j = j | cb << 16 | bb << 8 | ab;
                        break;

                    case 4d: // 240 - Blue
                        j = j | bb << 16 | cb << 8 | ab;
                        break;

                    case 5d: // 300 - Magenta
                        j = j | ab << 16 | cb << 8 | bb;
                        break;
                }

                retColor = j;
                return;
            }
        }

        public static void ColorToCMY(UniColor Color, ref CMYDATA cmy)
        {
            //
            byte r;
            byte g;
            byte b;

            int x;

            GetRGB(Color, out r, out g, out b);

            x = Max(r, g, b);

            r = (byte)Math.Abs(1 - r);
            g = (byte)Math.Abs(1 - g);
            b = (byte)Math.Abs(1 - b);

            cmy.Magenta = r;
            cmy.Yellow = g;
            cmy.Cyan = b;
        }

        public static UniColor CMYToColor(CMYDATA cmy)
        {
            //
            int c;
            int m;
            int y;

            byte r;
            byte g;
            byte b;

            c = cmy.Cyan;
            m = cmy.Magenta;
            y = cmy.Yellow;

            r = (byte)Math.Abs(1 - m);
            g = (byte)Math.Abs(1 - y);
            b = (byte)Math.Abs(1 - c);

            return new UniColor(255, r, g, b);
        }

        public static void GetPercentages(UniColor Color, ref double dpRed, ref double dpGreen, ref double dpBlue)
        {
            double vR;
            double vB;
            double vG;
            var vIn = new byte[3];
            double d;
            GetRGB(Color, out vIn[0], out vIn[1], out vIn[2]);
            vR = vIn[0];
            vG = vIn[1];
            vB = vIn[2];
            if (vR > vG)
                d = vR;
            else
                d = vG;
            if (vB > d)
                d = vB;
            if (d == 0d)
                d = 255.0d;
            dpRed = vR / d;
            dpGreen = vG / d;
            dpBlue = vB / d;
        }

        public static UniColor AbsTone(UniColor Color)
        {
            UniColor AbsToneRet = default;
            var pR = default(double);
            var pB = default(double);
            var pG = default(double);
            double sR;
            double sB;
            double sG;
            GetPercentages(Color, ref pR, ref pG, ref pB);
            sR = pR * 255d;
            sG = pG * 255d;
            sB = pB * 255d;
            AbsToneRet = new UniColor(Color.A, (byte)sR, (byte)sG, (byte)sB);
            return AbsToneRet;
        }

        public static UniColor SetTone(ref RGBDATA rgbData, float pPercent, UniColor Color = default)
        {
            float x;
            if (Color != default)
                ColorToRGB(Color, out rgbData);
            x = Max(rgbData.Red, rgbData.Green, rgbData.Blue);
            if (x == 0f)
            {
                rgbData.Red = 0;
                rgbData.Green = 0;
                rgbData.Blue = 0;
                return UniColor.Empty;
            }

            rgbData.Red = (byte)(rgbData.Red / x * (255f * pPercent));
            rgbData.Green = (byte)(rgbData.Green / x * (255f * pPercent));
            rgbData.Blue = (byte)(rgbData.Blue / x * (255f * pPercent));
            return RGBToColor(rgbData);
        }

        public static UniColor SetTone(ref ARGBDATA argbData, float pPercent, UniColor Color = default)
        {
            float x;
            if (Color != UniColor.Empty)
                ColorToARGB(Color, out argbData);
            x = Max(argbData.Red, argbData.Green, argbData.Blue);
            if (x == 0f)
            {
                argbData.Red = 0;
                argbData.Green = 0;
                argbData.Blue = 0;
                return UniColor.Empty;
            }

            argbData.Red = (byte)(argbData.Red / x * (255f * pPercent));
            argbData.Green = (byte)(argbData.Green / x * (255f * pPercent));
            argbData.Blue = (byte)(argbData.Blue / x * (255f * pPercent));
            return ARGBToColor(argbData);
        }

        public static UniColor GrayTone(UniColor Color)
        {
            UniColor GrayToneRet = default;
            ARGBDATA rgbData;
            int a;
            int b;
            int c;
            byte tone;
            ColorToARGB(Color, out rgbData);
            a = rgbData.Red;
            b = rgbData.Green;
            c = rgbData.Blue;
            tone = (byte)((a + b + c) / 3d);
            GrayToneRet = new UniColor(rgbData.Alpha, tone, tone, tone);
            return GrayToneRet;
        }

        public static UniColor GetAverageColor(UniColor Color1, UniColor Color2)
        {
            var Bits = new RGBDATA[3];
            float df;
            int e;
            float clr2 = Color2.Value;
            byte al = Color1.A;
            byte af = Color2.A;
            ColorToRGB(Color1, out Bits[0]);
            ColorToRGB(Color2, out Bits[1]);
            e = 0;
            if (Math.Round(clr2) != clr2)
            {
                df = Bits[0].Red * clr2;
                e = (int)Math.Round(df, 0);
                Bits[2].Red = (byte)(e & 0xFF);
                df = Bits[0].Green * clr2;
                e = (int)Math.Round(df, 0);
                Bits[2].Green = (byte)(e & 0xFF);
                df = Bits[0].Blue * clr2;
                e = (int)Math.Round(df, 0);
                Bits[2].Blue = (byte)(e & 0xFF);
            }
            else
            {
                df = (Bits[0].Red + (float)Bits[1].Red) / 2f;
                e = (int)Math.Round(df, 0);
                Bits[2].Red = (byte)(e & 0xFF);
                df = (Bits[0].Green + (float)Bits[1].Green) / 2f;
                e = (int)Math.Round(df, 0);
                Bits[2].Green = (byte)(e & 0xFF);
                df = (Bits[0].Blue + (float)Bits[1].Blue) / 2f;
                e = (int)Math.Round(df, 0);
                Bits[2].Blue = (byte)(e & 0xFF);
            }

            // Get the average alpha
            al = (byte)((al + af) / 2d);
            return new UniColor(al, Bits[2].Red, Bits[2].Green, Bits[2].Blue);
        }
    }
}