// Basic Math Functions
// Copyright (C) 2023 Nathaniel Moschkin

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

using DataTools.Text;

namespace DataTools.MathTools
{
    public static class MathLib
    {
        // Copyright (C) 2023 Nathaniel Moschkin.  All Rights Reserved.
        // Apache 2.0 License
        // 
        public static double InchesToMillimeters(double value)
        {
            return value * 25.4d;
        }

        public static double MillimetersToInches(double value)
        {
            return value / 25.4d;
        }

        public static double[] InchesToMillimeters(double[] value)
        {
            int i;
            int c = value.Count();

            for (i = 0; i < c; i++)
                value[i] *= 25.4d;
            return value;
        }

        public static double[] MillimetersToInches(double[] value)
        {
            int i;
            int c = value.Count();
            for (i = 0; i < c; i++)
                value[i] /= 25.4d;
            return value;
        }


        /// <summary>
        /// Prints a fractional number from a double value. (More Consistently Human-Readable Version)
        /// </summary>
        /// <param name="value">The value to convert to a fraction.</param>
        /// <param name="maxSignificantDigits">The maximum number of significant digits the number can be rounded to.</param>
        /// <param name="maxDenominator">The maximum possible value of the denominator.</param>
        /// <param name="addQuasiMark">Set to True to output the '~' symbol for fractions that are found below the maximum significant digit.</param>
        /// <returns>A string representing a whole number with a fraction.</returns>
        /// <remarks>
        /// The maximum number of iterations required by this algorithm is O^(<paramref name="maxSignificantDigits"/> + <paramref name="maxDenominator"/>).
        /// <br /><br />
        /// If a number is too small to fit inside a fraction with <paramref name="maxDenominator"/>, then the function will return 0.
        /// </remarks>
        public static string PrintFraction(double value, int maxSignificantDigits = 7, int maxDenominator = 20, bool addQuasiMark = true)
        {
            double wholePart = 0.0d;
            double workVal;
            double testVal;
            double numerator;
            double denominator;

            int currSig;
            int hSigFound = 1;

            int fi = 0;
            var foundFractions = new (int, int)[maxSignificantDigits];

            var lastTest = default(double);

            if (Math.Round(value, maxSignificantDigits) == 0)
            {
                return "0";
            }

            // test to see if there is a whole-number component, and place it off to the side.
            if ((double)value >= 1.0d)
            {
                wholePart = Math.Floor(value);
                value -= wholePart;

                // if there is no fractional component, there's no reason to go on.
                if (value == 0d)
                    return wholePart.ToString("0");
            }

            if (maxSignificantDigits > 18)
                maxSignificantDigits = 18;
            
            // Go from 1 to the maximum number of significant digits.
            retry:            
            
            for (currSig = 1; currSig <= maxSignificantDigits; currSig++)
            {
                // get the rounded, working value for the test.
                workVal = Math.Round(value, currSig);

                // iterate the numerator to the maximum denominator value.
                for (numerator = 1; numerator < maxDenominator; numerator++)
                {
                    // iterate the denomenator to the maximum denominator value.
                    for (denominator = 1; denominator <= maxDenominator; denominator++)
                    {                        
                        // create the test value.
                        testVal = Math.Round(numerator / denominator, currSig);
                        if (testVal == workVal && testVal != 0)
                        {
                            // at this significant digit, the test and working values
                            // produce the same result, meaning that this is a viable
                            // fraction.  But it may not be the only viable fraction
                            // within the range of possible denominators.

                            // we'll record the highest significant digit for which
                            // a fraction was found, so far.
                            hSigFound = currSig;

                            // add the compatible pair to a list
                            foundFractions[fi++] = (((int)numerator, (int)denominator));
        
                            // break to the next significant digit, there's
                            // no reason to do any more work.
                            lastTest = testVal;
                            goto nextSig;
                        }
                    }
                }

            nextSig:
                ;
            }

            if (fi == 0)
            {
                if (wholePart == 0) return "0";

                numerator = 0;
                denominator = 0;
            }
            else
            {
                // the best fit will be the last found fraction pair,
                // which will have the closest approximation to the the original number,
                // within the bounds of available significant digits.
                (numerator, denominator) = foundFractions[fi - 1];
            }

            var sb = new StringBuilder();

            // if the higest found fraction was below the maximum number of significant digits,
            // we will optionally add the "~" (quasi) mark to the output string to indicate that
            // this is an approximation (with regard to the number of significant digits).
            if (addQuasiMark == true && hSigFound < maxSignificantDigits && lastTest != value)
            {
                sb.Append("~");
            }

            // If we have an integer component, add that to the output string.
            if (numerator == 1 && denominator == 1)
                wholePart = wholePart + 1;

            if (wholePart > 0)
            {
                if (sb.Length != 0) sb.Append(' ');
                sb.Append($"{wholePart}");               
            }

            // Finally, add the fraction.
            if (!(numerator == 1 && denominator == 1) && (numerator != 0 && denominator != 0))
            {
                if (sb.Length != 0) sb.Append(' ');
                sb.Append($"{numerator}/{denominator}");
            }

            // We're done!
            return sb.ToString();
        }

        /// <summary>
        /// Prints a fractional number from a double value (Stern-Brocot Tree Version).
        /// </summary>
        /// <param name="value">The value to convert to a fraction.</param>
        /// <param name="accuracy">The maximum number of significant digits the number can be rounded to.</param>
        /// <param name="addQuasiMark">Set to True to output the '~' symbol for fractions that are found below the maximum significant digit.</param>
        /// <returns>A string representing a whole number with a fraction.</returns>
        /// <remarks>The number of iterations required by this algorithm is greatly influenced by the size of the maximum denominator.</remarks>
        public static string PrintFraction(double value, double accuracy = 0.001, bool addQuasiMark = true)
        {
            if (accuracy <= 0.0 || accuracy >= 1.0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1.");
            }
            double orgVal = value;
            int sign = Math.Sign(value);

            if (sign == -1)
            {
                value = Math.Abs(value);
            }

            // Accuracy is the maximum relative error; convert to absolute maxError
            double maxError = accuracy; // sign == 0 ? accuracy : value * accuracy;
            
            int n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                return $"{sign * n}/1";
            }

            if (1 - maxError < value)
            {
                return $"{sign * (n + 1)}/1";
            }


            // The lower fraction is 0/1
            int lower_n = 0;
            int lower_d = 1;

            // The upper fraction is 1/1
            int upper_n = 1;
            int upper_d = 1;

            while (true)
            {
                // The middle fraction is (lower_n + upper_n) / (lower_d + upper_d)
                int middle_n = lower_n + upper_n;
                int middle_d = lower_d + upper_d;

                if (middle_d * (value + maxError) < middle_n)
                {
                    // real + error < middle : middle is our new upper
                    upper_n = middle_n;
                    upper_d = middle_d;
                }
                else if (middle_n < (value - maxError) * middle_d)
                {
                    // middle < real - error : middle is our new lower
                    lower_n = middle_n;
                    lower_d = middle_d;
                }
                else
                {
                    // Middle is our best fraction

                    var s = "";
                    if (addQuasiMark && ((n * sign) + ((double)middle_n/middle_d) != orgVal))
                    {
                        s = "~ ";
                    }

                    if (n != 0)
                        return $"{s}{n * sign} {middle_n}/{middle_d}";
                    else
                        return $"{s}{middle_n}/{middle_d}";
                }
            }

            // We're done!
        }



        /// <summary>
        /// Find the greatest common factor between a group of integers.
        /// </summary>
        /// <param name="values">The integers to search.</param>
        /// <returns>The greatest common factor of all integers.</returns>
        public static int FindGreatestCommonFactor(params int[] values)
        {
            if (values == null || values.Length == 1) throw new ArgumentOutOfRangeException("Function takes 2 or more numbers.");
            int candidate = values[0];

            int i, c = values.Length;
            int min, max;

            int gcf = -1;

            for (i = 1; i < c; i++)
            {
                min = candidate;
                max = values[i];

                // put the numbers in the right position.
                if (min > max)
                {
                    min = max;
                    max = values[i - 1];
                }

                while (max != min)
                {
                    if (max < min)
                    {
                        min = min - max;
                    }
                    else
                    {
                        max = max - min;
                    }
                }

                candidate = min;

                // it may seem counter-intuitive that we're looking for the lesser value to get the 'greatest' common factor.
                // but it's simply that we are looking for the largest number that will go into all numbers.
                // if the number we just got is less than then number that we already had (the candidate), 
                // that means that new, smaller number becomes the new largest possible value.

                // so, we're looking for 1 result for each pair of numbers.
                // we want that 1 result for each pair to be the biggest number possible (the greatest common factor).
                // BUT, when we're dealing with more than 1 pair, we have to take all the numbers into account,
                // and so we have a set of 'biggest numbers possible'.

                // and we're looking for the smallest number in that set.
                // because THAT will ultimately be the final 'biggest number possible.' 

                if (gcf == -1 || candidate < gcf) gcf = candidate;

                // unless the laws of mathematics change, tomorrow, this can end, here.
                if (gcf == 1) break;
            }

            return gcf;
        }

      
        /// <summary>
        /// Swap nibbles
        /// </summary>
        /// <param name="ByteVal"></param>
        /// <returns></returns>
        public static byte Swan(byte ByteVal)
        {
            return (byte)((ByteVal << 4) | (byte)(ByteVal >> 4));
        }

        /// <summary>
        /// Change the endianness of the given value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static uint Endian(uint Value)
        {
            var b = BitConverter.GetBytes(Value);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
        }

        /// <summary>
        /// Change the endianness of the given value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static int Endian(int Value)
        {
            var b = BitConverter.GetBytes(Value);
            Array.Reverse(b);
            return BitConverter.ToInt32(b, 0);
        }


        public static double Min(params double[] vars)
        {
            double d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return double.NaN;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static double Max(params double[] vars)
        {
            double d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return double.NaN;
            d = vars[0];

            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static float Min(params float[] vars)
        {
            float d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return float.NaN;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static float Max(params float[] vars)
        {
            float d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return float.NaN;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static decimal Min(params decimal[] vars)
        {
            decimal d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return decimal.Zero;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static decimal Max(params decimal[] vars)
        {
            decimal d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return decimal.Zero;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static long Min(params long[] vars)
        {
            long d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0L;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static long Max(params long[] vars)
        {
            long d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0L;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static int Min(params int[] vars)
        {
            int d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static int Max(params int[] vars)
        {
            int d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static short Min(params short[] vars)
        {
            short d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static short Max(params short[] vars)
        {
            short d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static byte Min(params byte[] vars)
        {
            byte d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static byte Max(params byte[] vars)
        {
            byte d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static ulong Min(params ulong[] vars)
        {
            ulong d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0UL;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static ulong Max(params ulong[] vars)
        {
            ulong d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0UL;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static uint Min(params uint[] vars)
        {
            uint d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0U;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static uint Max(params uint[] vars)
        {
            uint d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0U;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static ushort Min(params ushort[] vars)
        {
            ushort d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static ushort Max(params ushort[] vars)
        {
            ushort d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static sbyte Min(params sbyte[] vars)
        {
            sbyte d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Min(d, vars[i]);
            return d;
        }

        public static sbyte Max(params sbyte[] vars)
        {
            sbyte d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0;
            d = vars[0];
            for (i = 1; i < c; i++)
                d = Math.Max(d, vars[i]);
            return d;
        }

        public static double Sum(params double[] vars)
        {
            double d = 0d;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0d;
            d = vars[0];
            for (i = 1; i < c; i++)
                d += vars[i];
            return d;
        }

        public static decimal Sum(params decimal[] vars)
        {
            decimal d = 0m;
            int i;
            int c = vars.Length;
            if (c <= 0)
                return 0m;
            d = vars[0];
            for (i = 1; i < c; i++)
                d += vars[i];
            return d;
        }

    }


}