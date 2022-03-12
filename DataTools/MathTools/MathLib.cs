// Basic Math Functions
// Copyright (C) 2022 Nathaniel Moschkin

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

using DataTools.Text;

namespace DataTools.MathTools
{
    public static class MathLib
    {
        // Extended mathematic process library for Visual Basic
        //
        // Duplication is prohibited, as is commercial use without prior written permission.
        // Copyright (C) 2015 Nathaniel Moschkin.  All Rights Reserved.

        public const double MAX_VALUE = 1.7976931348623157E+308d;
        public const double MIN_VALUE = 4.94065645841247E-324d;
        public const int ProgressDiv = 17;
        public static int[] OnBits = new int[32];
        public static byte[,] LShiftTab = new byte[256, 8];
        public static byte[,] RShiftTab = new byte[256, 8];

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
        /// <remarks>The maximum number of iterations required by this algorithm is O^(<paramref name="maxSignificantDigits"/> + <paramref name="maxDenominator"/>).</remarks>
        public static string PrintFraction(double value, int maxSignificantDigits = 7, int maxDenominator = 25, bool addQuasiMark = true)
        {
            double wholePart = 0.0d;
            double workVal;
            double testVal;
            double numerator;
            double denominator;

            int orgden = maxDenominator;

            int currSig;
            int hSigFound = 1;

            int fi = 0;
            var foundFractions = new (int, int)[maxSignificantDigits];

            var lastTest = default(double);

            if (value == 0)
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
                        if (testVal == workVal)
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
                if (maxDenominator - orgden > maxSignificantDigits)
                    return "NaN";

                maxDenominator++;
                goto retry;
            }

            // the best fit will be the last found fraction pair,
            // which will have the closest approximation to the the original number,
            // within the bounds of available significant digits.
            (numerator, denominator) = foundFractions[fi - 1];

            var sb = new StringBuilder();

            // if the higest found fraction was below the maximum number of significant digits,
            // we will optionally add the "~" (quasi) mark to the output string to indicate that
            // this is an approximation (with regard to the number of significant digits).
            if (addQuasiMark == true && hSigFound < maxSignificantDigits && lastTest != value)
            {
                sb.Append("~ ");
            }

            // If we have an integer component, add that to the output string.
            if (numerator == 1 && denominator == 1)
                wholePart = wholePart + 1;

            if (wholePart > 0)
            {
                sb.Append($"{wholePart} ");
            }

            // Finally, add the fraction.
            if (!(numerator == 1 && denominator == 1))
                sb.Append($"{numerator}/{denominator}");

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
        /// Strips the units from a number, returns both, cleaned.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string StripUnit(string text, ref string unit)
        {
            var ch = text.ToCharArray();
            int i;
            int c = ch.Count() - 1;

            unit = null;

            for (i = c; i >= 0; i -= 1)
            {
                if (TextTools.IsNumber(ch[i]))
                {
                    if (i == c)
                        return text.Trim();

                    unit = text.Substring(i + 1).Trim();
                    text = text.Substring(0, i + 1).Trim();

                    break;
                }
            }

            return text;
        }

        /// <summary>
        /// Parse any string into an array of numbers with their optional unit markers.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="returnUnits"></param>
        /// <param name="validunits"></param>
        /// <param name="validSeparators"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double[] ParseArrayOfNumbersWithUnits(string input, [Optional, DefaultParameterValue(null)] ref string[] returnUnits, string[] validunits = null, string validSeparators = ",x;:")
        {
            if (validunits is null)
            {
                validunits = new[] { "in", "mm", "'", "\"", "ft" };
            }

            var vc = validSeparators.ToCharArray();
            int i;
            int c;
            var sb = new System.Text.StringBuilder();
            var retVal = new List<double>();
            var ru = new List<string>();
            string s;
            var exp = new MathExpressionParser();
            input = input.ToLower();
            c = vc.Count();

            for (i = 1; i < c; i++)
                input = input.Replace(vc[i], vc[0]);

            var parse = TextTools.Split(input, vc[0].ToString());

            c = parse.Count();

            string su = null;

            for (i = 0; i < c; i++)
            {
                s = parse[i].Trim();

                s = StripUnit(s, ref su);

                ru.Add(su);

                string argErrorText = null;

                retVal.Add(exp.ParseExpression(s, ErrorText: ref argErrorText));
            }

            returnUnits = ru.ToArray();
            return retVal.ToArray();
        }

        /// <summary>
        /// Strips out everything else from a string and returns only the numbers.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="culture"></param>
        /// <param name="hex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string NumbersOnly(string input, System.Globalization.CultureInfo culture = null, bool hex = false)
        {
            if (culture is null)
            {
                culture = System.Globalization.CultureInfo.CurrentCulture;
            }

            char[] ch;
            var sb = new System.Text.StringBuilder();
            int i;
            string scan;
            int cs = '0';
            for (i = 0; i <= 9; i++)
                sb.Append((char)(cs + i));
            if (hex)
            {
                cs = 'A';
                for (i = 0; i <= 5; i++)
                    sb.Append((char)(cs + i));
                cs = 'a';
                for (i = 0; i <= 5; i++)
                    sb.Append((char)(cs + i));
            }

            sb.Append(culture.NumberFormat.NumberDecimalSeparator);
            sb.Append(culture.NumberFormat.CurrencyDecimalSeparator);
            sb.Append(culture.NumberFormat.PercentDecimalSeparator);
            sb.Append(culture.NumberFormat.NegativeSign);
            sb.Append(culture.NumberFormat.PositiveSign);

            ch = input.ToCharArray();
            scan = sb.ToString();

            cs = ch.Length;

            for (i = 0; i < cs; i++)
            {
                if (scan.Contains(ch[i]))
                {
                    sb.Append(ch[i]);
                }
            }

            return sb.ToString();
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

    }

}