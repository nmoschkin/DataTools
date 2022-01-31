﻿// Basic Math Functions
// Copyright (C) 2015 Nathaniel Moschkin

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
        /// Prints a fractional number from a decimal value.
        /// </summary>
        /// <param name="value">The value to convert to a fraction.</param>
        /// <param name="maxSignificantDigits">The maximum number of significant digits the number can be rounded to.</param>
        /// <param name="maxDenominator">The maximum possible value of the denominator.</param>
        /// <param name="addQuasiMark">Set to True to output the '~' symbol for fractions that are found below the maximum significant digit.</param>
        /// <returns>A string representing a whole number with a fraction.</returns>
        /// <remarks>The number of iterations required by this algorithm is greatly influenced by the size of the maximum denominator.</remarks>
        public static string PrintFraction(decimal value, int maxSignificantDigits = 7, int maxDenominator = 25, bool addQuasiMark = true)
        {
            decimal wholePart = 0.0m;
            decimal workVal;
            decimal testVal;
            decimal numerator;
            decimal denominator;
            int currSig;
            int hSigFound = 1;
            var foundFractions = new List<int[]>();
            string output = "";
            var lastTest = default(decimal);
            if (value == decimal.Zero)
            {
                return "0";
            }

            // test to see if there is a whole-number component, and place it off to the side.
            if ((double)value >= 1.0d)
            {
                wholePart = Math.Floor(value);
                value -= wholePart;

                // if there is no fractional component, there's no reason to go on.
                if (value == 0m)
                    return wholePart.ToString("0");
            }

            if (maxSignificantDigits > 28)
                maxSignificantDigits = 28;
            // Go from 1 to the maximum number of significant digits.
            
            for (currSig = 1; currSig <= maxSignificantDigits; currSig++)
            {

                // get the rounded, working value for the test.
                workVal = Math.Round(value, currSig);

                // iterate the numerator to the maximum denominator value.
                for (numerator = 1m; numerator <= maxDenominator; numerator++)
                {

                    // iterate the denomenator to the maximum denominator value.
                    for (denominator = 1m; denominator <= maxDenominator; denominator++)
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
                            foundFractions.Add(new int[] { (int)numerator, (int)denominator });

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


            // the best fit will be the last found fraction pair,
            // which will have the closest approximation to the the original number,
            // within the bounds of available significant digits.
            numerator = foundFractions[foundFractions.Count - 1][0];
            denominator = foundFractions[foundFractions.Count - 1][1];

            // if the higest found fraction was below the maximum number of significant digits,
            // we will optionally add the "~" (quasi) mark to the output string to indicate that
            // this is an approximation (with regard to the number of significant digits).
            if (addQuasiMark == true && hSigFound < maxSignificantDigits && lastTest != value)
            {
                output += "~ ";
            }

            // If we have an integer component, add that to the output string.
            if (numerator == 1m && denominator == 1m)
                wholePart = wholePart + 1m;
            if (wholePart > 0m)
            {
                output += wholePart + " ";
            }

            // Finally, add the fraction.
            if (!(numerator == 1m && denominator == 1m))
                output += string.Format("{0}/{1}", numerator.ToString("0"), denominator.ToString("0"));

            // We're done!
            return output;
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
            int min, max, mod;

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

                // find the remaineder.
                mod = max % min;


                while (mod != 0)
                {
                    // just keep taking the remainder until it's zero, the last number we used is the candidate.
                    max = min;
                    min = mod;
                    mod = max % min;
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