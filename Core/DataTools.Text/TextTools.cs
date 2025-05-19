// *************************************************
// DataTools C# Utility Library For Windows
//
// Module: TextTools
//         Text processing and managling library.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataTools.Text
{
    /// <summary>
    /// Match condition flags.
    /// </summary>
    /// <remarks></remarks>
    [Flags]
    public enum MatchCondition
    {
        /// <summary>
        /// The match must be exact
        /// </summary>
        /// <remarks></remarks>
        Exact = 0x0,

        /// <summary>
        /// The match must be exact up until the length of the
        /// requested expression (if it is shorter than the matched index)
        /// </summary>
        /// <remarks></remarks>
        FirstOfSearch = 0x1,

        /// <summary>
        /// The match must be exact up until the length of the
        /// matched index (if it is shorter than the search expression)
        /// </summary>
        /// <remarks></remarks>
        FirstOfMatch = 0x2,

        /// <summary>
        /// Instead of returning the index matched, return the string
        /// </summary>
        /// <remarks></remarks>
        ReturnString = 0x4
    }

    /// <summary>
    /// Generic sort order.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Sort ascending.
        /// </summary>
        Ascending = 0x0,

        /// <summary>
        /// Sort descending.
        /// </summary>
        Descending = 0x1
    }

    /// <summary>
    /// Flags for use with NoSpace
    /// </summary>
    /// <remarks></remarks>
    [Flags]
    public enum NoSpaceModifiers
    {
        /// <summary>
        /// No modifiers.
        /// </summary>
        None = 0,

        /// <summary>
        /// Trim space and convert character before to lowercase.
        /// </summary>
        BeforeToLower = 1,

        /// <summary>
        /// Trim space and convert character before to uppercase.
        /// </summary>
        BeforeToUpper = 2,

        /// <summary>
        /// Trim space and convert character after to lowercase.
        /// </summary>
        AfterToLower = 4,

        /// <summary>
        /// Trim space and convert character after to uppercase.
        /// </summary>
        AfterToUpper = 8,

        /// <summary>
        /// Convert the first character to lowercase.
        /// </summary>
        FirstToLower = 16,

        /// <summary>
        /// Convert the first character to uppercase.
        /// </summary>
        FirstToUpper = 32
    }

    /// <summary>
    /// Good all-purpose text mangling library.
    /// </summary>
    public static class TextTools
    {
        /// <summary>
        /// All allowed mathematical characters.
        /// </summary>
        /// <remarks></remarks>
        public const string MathChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.()+-\\=/*^%";

        /// <summary>
        /// All canonical letters and numbers.
        /// </summary>
        /// <remarks></remarks>
        public const string AlphaNumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// All canonical letters.
        /// </summary>
        /// <remarks></remarks>
        public const string AlphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// All standard non-alphanumeric characters.
        /// </summary>
        /// <remarks></remarks>
        public const string NonAlphas = "-._~:/?#[]@!$&'()*+,;=";

        /// <summary>
        /// Characters allowed in a url.
        /// </summary>
        /// <remarks></remarks>
        public const string UrlAllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~:/?#[]@!$&'()*+,;=";

        /// <summary>
        /// Compares a string to many other strings.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="comp"></param>
        /// <returns>True if found</returns>
        public static bool MultiComp(string v, string[] comp)
        {
            foreach (string x in comp)
            {
                if (x == v) return true;
            }

            return false;
        }

        /// <summary>
        /// Compares a character to many other characters.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="comp"></param>
        /// <returns>True if found</returns>
        public static bool MultiComp(char v, char[] comp)
        {
            foreach (char x in comp)
            {
                if (x == v) return true;
            }

            return false;
        }

        /// <summary>
        /// Trim all public, writable, instance string properties in a class object.
        /// </summary>
        /// <param name="obj">Object to scan and trim</param>
        /// <param name="trimChars">Optional trim characters</param>
        public static void ClassTrimTool(object obj, char[] trimChars = null)
        {
            if (obj == null || obj.GetType().IsClass == false) throw new ArgumentException();

            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            foreach (var prop in props)
            {
                string val;

                if (prop.PropertyType == typeof(string) && prop.SetMethod != null)
                {
                    val = (string)prop.GetValue(obj);
                    if (val == null) continue;

                    if (trimChars != null)
                    {
                        val = val.Trim(trimChars);
                    }
                    else
                    {
                        val = val.Trim();
                    }

                    prop.SetValue(obj, val);
                }
            }
        }

        /// <summary>
        /// Parse a string into an array of strings using the specified parameters
        /// </summary>
        /// <param name="scan">String to scan</param>
        /// <param name="separator">Separator string</param>
        /// <param name="skipQuote">Whether to skip over quote blocks</param>
        /// <param name="unescape">Whether to unescape quotes.</param>
        /// <param name="quoteChar">Quote character to use.</param>
        /// <param name="escChar">Escape character to use.</param>
        /// <param name="withToken">Include the token in the return array.</param>
        /// <param name="withTokenIn">Attach the token to the beginning of every string separated by a token (except for string 0).  Requires WithToken to also be set to True.</param>
        /// <param name="unquote">Remove quotes from around characters.</param>
        /// <param name="skipAllQuotes">Skips over all standard quotes regardless of the value of <paramref name="quoteChar"/>, including interpolated, single-quote, and literal. This is the default behavior.</param>
        /// <param name="trimResults">Trim all results. Not enabled by default.</param>
        /// <returns>An array of strings.</returns>
        /// <remarks>
        /// <paramref name="skipAllQuotes"/> is true by default, as one of the bonuses this function has over <see cref="string.Split(char[])"/> is the ability to do that.
        /// </remarks>
        public static string[] Split(string scan, string separator, bool skipQuote = false, bool unescape = false, char quoteChar = '"', char escChar = '\\', bool unquote = false, bool withToken = false, bool withTokenIn = false, bool skipAllQuotes = true, bool trimResults = false, bool skipPairs = true)
        {
            int i;
            int line = 0;

            List<string> sOut = new List<string>();

            var sb = new StringBuilder();

            int c, e;

            char[] chrs = scan.ToCharArray();
            char[] sep = separator.ToCharArray();

            c = chrs.Length;
            e = sep.Length;

            for (i = 0; i < c; i++)
            {
                if (skipQuote && (chrs[i] == quoteChar))
                {
                    var txt = QuoteFromHere(chrs, i, ref line, out int? sqStart, out int? sqEnd, quoteChar: quoteChar, escChar: escChar, withQuotes: true);
                    if (sqEnd != null)
                    {
                        if (unquote)
                        {
                            sb.Append(txt.Substring(1, txt.Length - 2));
                        }
                        else
                        {
                            sb.Append(txt);
                        }
                        
                        i = (int)sqEnd;
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((skipAllQuotes) && (chrs[i] == '\"'))
                {
                    var txt = QuoteFromHere(chrs, i, ref line, out int? sqStart, out int? sqEnd, quoteChar: '\"', withQuotes: true);
                    if (sqEnd != null)
                    {
                        if (unquote)
                        {
                            sb.Append(txt.Substring(1, txt.Length - 2));
                        }
                        else
                        {
                            sb.Append(txt);
                        }

                        i = (int)sqEnd;
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((skipAllQuotes) && (chrs[i] == '\''))
                {
                    var txt = QuoteFromHere(chrs, i, ref line, out int? sqStart, out int? sqEnd, quoteChar: '\'', withQuotes: true);
                    if (sqEnd != null)
                    {
                        if (unquote)
                        {
                            sb.Append(txt.Substring(1, txt.Length - 2));
                        }
                        else
                        {
                            sb.Append(txt);
                        }

                        i = (int)sqEnd;
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((skipPairs) && (chrs[i] == '('))
                {
                    var txt = TextBetween(chrs, i, ref line, '(', ')', out int? sqStart, out int? sqEnd, withDelimiters: true);
                    if (sqEnd != null)
                    {
                        sb.Append(txt);
                        i = (int)sqEnd;
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((skipPairs) && (chrs[i] == '{'))
                {
                    var txt = TextBetween(chrs, i, ref line, '{', '}', out int? sqStart, out int? sqEnd, withDelimiters: true);
                    if (sqEnd != null)
                    {
                        sb.Append(txt);
                        i = (int)sqEnd;
                    }
                    else
                    {
                        break;
                    }
                }
                else if ((skipPairs) && (chrs[i] == '['))
                {
                    var txt = TextBetween(chrs, i, ref line, '[', ']', out int? sqStart, out int? sqEnd, withDelimiters: true);
                    if (sqEnd != null)
                    {
                        sb.Append(txt);
                        i = (int)sqEnd;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (chrs[i] == sep[0])
                    {
                        var x = 0;
                        var y = i;

                        while (y < c && x < e && chrs[y] == sep[x])
                        {
                            y++;
                            x++;
                        }

                        if (x == e)
                        {
                            if (withTokenIn) sb.Append(sep);
                            sOut.Add(sb.ToString());
                            if (withToken) sOut.Add(separator);
                            sb.Clear();

                            i = y - 1;
                        }
                    }

                    else
                    {
                        sb.Append(chrs[i]);
                    }
                }
            }

            if (sb.Length > 0)
            {
                sOut.Add(sb.ToString());
            }

            return sOut.ToArray();
        }

        /// <summary>
        /// Determine if a string consists only of numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsAllNumbers(string value)
        {
            char[] ch = value.ToCharArray();
            int i, c = ch.Length - 1;

            for (i = 0; i < c; i++)
            {
                if ((ch[i] < '0') || (ch[i] > '9'))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two array of bytes for equality.
        /// </summary>
        /// <param name="a">First array</param>
        /// <param name="b">Second array</param>
        /// <param name="result">Relative disposition</param>
        /// <returns>True if equal</returns>
        /// <remarks></remarks>
        public static bool CompareBytes(byte[] a, byte[] b, ref int result)
        {
            if (a.Length != b.Length)
                return false;

            if ((a == null) && (b == null))
                return true;

            if (((a != null) && (b == null)) || ((b != null) && (a == null)))
                return false;

            int i = 0;
            int c = a.Length - 1;

            for (i = 0; i <= c; i++)
            {
                if (a[i] != b[i])
                {
                    if (a[i] > b[i])
                        result = 1;
                    else
                        result = -1;
                    return false;
                }
            }

            result = 0;
            return true;
        }

        /// <summary>
        /// Removes all spaces from a string using default modifiers.
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string NoSpace(string subject)
        {
            return StripChars(subject, " " + (char)9);
        }

        /// <summary>
        /// Remove all spaces from a string and alters the output results according to NoSpaceModifiers
        /// </summary>
        /// <param name="subject">String to alter.</param>
        /// <param name="modifiers">Modifiers.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string NoSpace(string subject, NoSpaceModifiers modifiers)
        {
            char[] ch = null;
            int i = 0;
            int j = subject.Length - 1;

            int e = 0;
            string exclusions = " " + (char)9;
            bool ws = false;

            ch = new char[j + 1];

            for (i = 0; i <= j; i++)
            {
                if ((i == 0) && (modifiers & (NoSpaceModifiers.FirstToLower | NoSpaceModifiers.FirstToUpper)) != 0 && (exclusions.IndexOf(subject[i]) == -1))
                {
                    switch (modifiers)
                    {
                        case NoSpaceModifiers.FirstToUpper:
                            ch[e] = char.ToUpper(subject[i]);

                            break;

                        case NoSpaceModifiers.FirstToLower:
                            ch[e] = char.ToLower(subject[i]);

                            break;
                    }
                }

                if (exclusions.IndexOf(subject[i]) != -1)
                {
                    if (modifiers != NoSpaceModifiers.None)
                    {
                        if (i > 0)
                        {
                            if ((modifiers & NoSpaceModifiers.BeforeToLower) != 0)
                            {
                                ch[e - 1] = char.ToLower(ch[e - 1]);
                            }
                            else if ((modifiers & NoSpaceModifiers.BeforeToUpper) != 0)
                            {
                                ch[e - 1] = char.ToUpper(ch[e - 1]);
                            }
                        }
                    }
                    ws = true;
                }
                else if (ws)
                {
                    if (modifiers != NoSpaceModifiers.None)
                    {
                        if ((modifiers & NoSpaceModifiers.AfterToLower) != 0)
                        {
                            ch[e] = char.ToLower(subject[i]);
                        }
                        else if ((modifiers & NoSpaceModifiers.AfterToUpper) != 0)
                        {
                            ch[e] = char.ToUpper(subject[i]);
                        }
                        else
                        {
                            ch[e] = subject[i];
                            e += 1;
                        }
                    }
                    ws = false;
                }
                else
                {
                    ch[e] = subject[i];
                    e += 1;
                }
            }

            return new string(ch);
        }

        /// <summary>
        /// Counts occurrences of 'character'
        /// </summary>
        /// <param name="subject">The string to count.</param>
        /// <param name="character">The character to count.</param>
        /// <returns>The number of occurrences of 'character' in 'subject'</returns>
        /// <remarks></remarks>
        public static int CountChar(string subject, char character)
        {
            char[] ch = subject.ToCharArray();
            int c = ch.Length;
            int d = 0;

            for (int i = 0; i < c; i++)
            {
                if ((ch[i] == character))
                    d++;
            }

            return d;
        }

        /// <summary>
        /// Exclude a set of characters from a string.
        /// </summary>
        /// <param name="subject">The text to search.</param>
        /// <param name="exclusions">The characters to remove.</param>
        /// <returns>Processed text.</returns>
        /// <remarks></remarks>
        public static string StripChars(string subject, string exclusions)
        {
            string ch = "";
            int i = 0;
            int j = subject.Length - 1;

            for (i = 0; i <= j; i++)
            {
                if (exclusions.IndexOf(subject.Substring(i, 1)) != -1)
                    continue;
                ch += subject.Substring(i, 1);
            }

            return ch;
        }

        /// <summary>
        /// Finds and returns the first occurrence of text between startString and stopString
        /// </summary>
        /// <param name="subject">The text to process.</param>
        /// <param name="startString">The starting string to scan.</param>
        /// <param name="stopString">The ending string to scan.</param>
        /// <returns>The first occurrence of text between two seperator strings.</returns>
        /// <remarks>This version does not check for quoted text.</remarks>
        public static string TextBetween(string subject, string startString, string stopString)
        {
            return TextBetween(subject, 0, startString, stopString, out _, out _);
        }

        /// <summary>
        /// Test to see if successive characters constitute a string we are looking for.
        /// </summary>
        /// <param name="goalStr">The string we are searching for.</param>
        /// <param name="currChar">The next character to test.</param>
        /// <param name="currStr">The current string that has been assembled so far.</param>
        /// <param name="currPos">The current position in the string that we have assembled so far.</param>
        /// <returns>An integer value indicating the state of the search:<br />
        /// <br />0 - The string is not matched, and the string cannot be built from the combination of characters that have been passed.
        /// <br />1 - The string is matched, but not yet complete.
        /// <br />2 - The string is completely matched.
        /// </returns>
        /// <remarks>
        /// If the <paramref name="currChar"/>, <paramref name="currStr"/>, or <paramref name="currPos"/> values seem unrelated to <paramref name="goalStr"/>,
        /// an <see cref="ArgumentOutOfRangeException"/> will be thrown.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">currPos cannot be greater than or equal to the length of goalStr.</exception>
        /// <exception cref="ArgumentOutOfRangeException">currPos cannot be greater than the length of currStr.</exception>
        /// <exception cref="ArgumentOutOfRangeException">currStr does not represent a portion of goalStr.</exception>
        public static int RunBy(string goalStr, char currChar, ref string currStr, ref int currPos)
        {
            if (currPos >= goalStr.Length) throw new ArgumentOutOfRangeException("currPos cannot be greater than or equal to the length of goalStr.");

            if (string.IsNullOrEmpty(currStr))
            {
                currStr = "";
                currPos = 0;
            }

            if (currPos > currStr.Length || currPos < 0) throw new ArgumentOutOfRangeException("currPos cannot be greater than the length of currStr.");

            if (currStr.Length > 0 && !goalStr.StartsWith(currStr))
            {
                throw new ArgumentOutOfRangeException("currStr does not represent a portion of goalStr.");
            }

            if (currChar != goalStr[currPos])
            {
                currStr = null;
                currPos = 0;

                return 0;
            }
            else
            {
                currPos++;
                currStr += currChar;

                if (currPos == goalStr.Length)
                {
                    currPos = 0;
                    currStr = "";

                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Finds the text between two strings.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="startPos">The position of the string to start at.</param>
        /// <param name="start">The start string.</param>
        /// <param name="stop">The stop character.</param>
        /// <param name="idxStart">The returned start index of the text between (excluding the start character.)</param>
        /// <param name="idxStop">The returned stop index of the text between (excluding the stop character.)</param>
        /// <param name="withDelimiters">Include the <paramref name="start"/> and <paramref name="stop"/> delimiters.</param>
        /// <param name="escChar">The escape character to use for inside quoted strings (default is '\').</param>
        /// <param name="throwException">True to throw a <see cref="SyntaxErrorException"/> if the block cannot be completed.</param>
        /// <returns>The matched string optionally including the <paramref name="start"/> and <paramref name="stop"/> delimiter strings.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the index is less than 2 positions before the last position in the string.</exception>
        /// <exception cref="SyntaxErrorException">If the block is not terminated.</exception>
        public static string TextBetween(string value, int startPos, string start, string stop, out int? idxStart, out int? idxStop, bool withDelimiters = false, char escChar = '\\', bool throwException = false, bool noQuoteCheck = false)
        {
            int i = 0;
            return TextBetween(value.ToCharArray(), startPos, ref i, start, stop, out idxStart, out idxStop, withDelimiters, escChar, throwException, noQuoteCheck);
        }

        /// <summary>
        /// Finds the text between two strings.
        /// </summary>
        /// <param name="chars">The string to search.</param>
        /// <param name="startPos">The position of the string to start at.</param>
        /// <param name="line">Tracks the current line number.</param>
        /// <param name="start">The start string.</param>
        /// <param name="stop">The stop character.</param>
        /// <param name="idxStart">The returned start index of the text between (excluding the start character.)</param>
        /// <param name="idxStop">The returned stop index of the text between (excluding the stop character.)</param>
        /// <param name="withDelimiters">Include the <paramref name="start"/> and <paramref name="stop"/> delimiters.</param>
        /// <param name="escChar">The escape character to use for inside quoted strings (default is '\').</param>
        /// <param name="throwException">True to throw a <see cref="SyntaxErrorException"/> if the block cannot be completed.</param>
        /// <returns>The matched string optionally including the <paramref name="start"/> and <paramref name="stop"/> delimiter strings.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the index is less than 2 positions before the last position in the string.</exception>
        /// <exception cref="SyntaxErrorException">If the block is not terminated.</exception>
        public static string TextBetween(char[] chars, int startPos, ref int line, string start, string stop, out int? idxStart, out int? idxStop, bool withDelimiters = false, char escChar = '\\', bool throwException = false, bool noQuoteCheck = false)
        {
            idxStart = null;
            idxStop = null;

            bool inSpan = false;

            int rb1Pos = 0;
            string rb1Str = "";
            int rb2Pos = 0;
            string rb2Str = "";

            int s1test, s2test;

            int level = 0;
            StringBuilder sb = new StringBuilder();
            sb.Capacity = chars.Length;

            if (startPos < 0 || startPos >= chars.Length - ((start.Length + stop.Length) + 1)) throw new ArgumentOutOfRangeException(nameof(startPos));

            for (int i = startPos; i < chars.Length; i++)
            {
                char c = chars[i];

                if (c == '\n')
                {
                    line++;
                }
                else if (!noQuoteCheck && (c == '"' || c == '\''))
                {
                    var qs = QuoteFromHere(chars, i, ref line, out int? qstart, out int? qstop, quoteChar: c, escChar: escChar, withQuotes: true, throwException: throwException);

                    if (!string.IsNullOrEmpty(qs))
                    {
                        sb.Append(qs);
                    }
                    if (qstart != null && qstop != null)
                    {
                        i = qstop.Value;
                        continue;
                    }
                    else if (qstart != null && qstop == null)
                    {
                        break;
                    }
                }
                else
                {
                    s1test = RunBy(start, c, ref rb1Str, ref rb1Pos);

                    if (s1test == 2)
                    {
                        if (!inSpan)
                        {
                            sb.Append(start);
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        if (level == 0)
                        {
                            idxStart = i - (start.Length - 1);
                            inSpan = true;
                        }

                        level++;
                        continue;
                    }
                    else
                    {
                        s2test = RunBy(stop, c, ref rb2Str, ref rb2Pos);

                        if (s2test == 2)
                        {
                            level--;
                            if (level == 0)
                            {
                                idxStop = i;
                                //if (withDelimiters)
                                //{
                                //    idxStop = i;
                                //}
                                //else
                                //{
                                //    idxStop = i - stop.Length;
                                //}

                                sb.Append(c);
                                break;
                            }
                        }
                    }
                }

                if (inSpan)
                {
                    sb.Append(c);
                }
            }

            if (level != 0 && throwException)
            {
                throw new SyntaxErrorException($"Block at position {idxStart} does not have an ending string '{stop}'.");
            }

            if (level == 0 && !withDelimiters)
            {
                var strTemp = sb.ToString().Substring(start.Length, sb.Length - (stop.Length + start.Length));
                return strTemp;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Finds the text between two characters.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="startPos">The position of the string to start at.</param>
        /// <param name="start">The start character.</param>
        /// <param name="stop">The stop character.</param>
        /// <param name="idxStart">The returned start index of the text between (excluding the start character.)</param>
        /// <param name="idxStop">The returned stop index of the text between (excluding the stop character.)</param>
        /// <param name="withDelimiters">Include the <paramref name="start"/> and <paramref name="stop"/> delimiters.</param>
        /// <param name="escChar">The escape character to use for inside quoted strings (default is '\').</param>
        /// <param name="throwException">True to throw a <see cref="SyntaxErrorException"/> if the block cannot be completed.</param>
        /// <returns>The matched string optionally including the <paramref name="start"/> and <paramref name="stop"/> delimiter characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the index is less than 2 positions before the last position in the string.</exception>
        /// <exception cref="SyntaxErrorException">If the block is not terminated.</exception>
        public static string TextBetween(string value, int startPos, char start, char stop, out int? idxStart, out int? idxStop, bool withDelimiters = false, char escChar = '\\', bool throwException = false)
        {
            int i = 0;
            return TextBetween(value.ToCharArray(), startPos, ref i, start, stop, out idxStart, out idxStop, withDelimiters, escChar, throwException);
        }

        /// <summary>
        /// Finds the text between two characters.
        /// </summary>
        /// <param name="chars">The string to search.</param>
        /// <param name="startPos">The position of the string to start at.</param>
        /// <param name="start">The start character.</param>
        /// <param name="stop">The stop character.</param>
        /// <param name="idxStart">The returned start index of the text between (excluding the start character.)</param>
        /// <param name="idxStop">The returned stop index of the text between (excluding the stop character.)</param>
        /// <param name="withDelimiters">Include the <paramref name="start"/> and <paramref name="stop"/> delimiters.</param>
        /// <param name="escChar">The escape character to use for inside quoted strings (default is '\').</param>
        /// <param name="throwException">True to throw a <see cref="SyntaxErrorException"/> if the block cannot be completed.</param>
        /// <returns>The matched string optionally including the <paramref name="start"/> and <paramref name="stop"/> delimiter characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the index is less than 2 positions before the last position in the string.</exception>
        /// <exception cref="SyntaxErrorException">If the block is not terminated.</exception>
        public static string TextBetween(char[] chars, int startPos, ref int line, char start, char stop, out int? idxStart, out int? idxStop, bool withDelimiters = false, char escChar = '\\', bool throwException = false)
        {
            idxStart = null;
            idxStop = null;

            bool inSpan = false;

            int level = 0;
            StringBuilder sb = new StringBuilder();
            sb.Capacity = chars.Length;

            if (startPos < 0 || startPos > chars.Length - 2) throw new ArgumentOutOfRangeException(nameof(startPos));

            for (int i = startPos; i < chars.Length; i++)
            {
                char c = chars[i];

                if (c == '\n')
                {
                    line++;
                }
                else if (c == '"' || c == '\'')
                {
                    var qs = QuoteFromHere(chars, i, ref line, out int? qstart, out int? qstop, quoteChar: c, escChar: escChar, withQuotes: true, throwException: throwException);

                    if (!string.IsNullOrEmpty(qs))
                    {
                        sb.Append(qs);
                    }

                    if (qstart != null && qstop != null)
                    {
                        i = qstop.Value;
                        continue;
                    }
                    else if (qstart != null && qstop == null)
                    {
                        break;
                    }
                }
                else
                {
                    if (c == start)
                    {
                        if (level == 0)
                        {
                            idxStart = i;
                            inSpan = true;
                            level++;
                            continue;
                        }

                        level++;
                    }
                    else if (c == stop)
                    {
                        level--;
                        if (level == 0)
                        {
                            idxStop = i;
                            break;
                        }
                    }
                }

                if (inSpan)
                {
                    sb.Append(c);
                }
            }

            if (level != 0 && throwException)
            {
                throw new SyntaxErrorException($"Block at position {idxStart} does not have an ending character '{stop}'.");
            }

            if (withDelimiters)
            {
                sb.Insert(0, start);
                sb.Append(stop);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Function to retrieve a quote from a string of data.
        /// </summary>
        /// <param name="input">The string characters to scan.</param>
        /// <param name="index">The position to begin scanning.</param>
        /// <param name="startPos">
        /// The returned start position of the quoted string.<br/><br />
        /// if <paramref name="withQuotes"/> is true, this will be the position of the quote character. Otherwise, it will be the position immediately after the quote character.
        /// </param>
        /// <param name="endPos">
        /// The returned end position of the quoted string.<br/><br />
        /// If <paramref name="withQuotes"/> is true, this will be the position of the quote character. Otherwise, it will be the position immediately before the quote character.
        /// </param>
        /// <param name="interpolationChar">The character that denotes the start of an interpolated string (usually '$')</param>
        /// <param name="literalChar">>The character that denotes the start of a literal string (usually '@')</param>
        /// <param name="interpolationBegin">The character the signals the start of code inside of an interpolated string.</param>
        /// <param name="interpolationEnd">The character the signals the end of code inside of an interpolated string.</param>
        /// <param name="quoteChar">The quote character to use.</param>
        /// <param name="escChar">The escape character to use.</param>
        /// <param name="withQuotes">Return the string in quotes.</param>
        /// <param name="throwException">True to throw a <see cref="SyntaxErrorException"/> if an unterminated quoted string is found.</param>
        /// <returns>The requested string, or an empty string if no string is found.</returns>
        /// <remarks>
        /// The quote character must be: exactly one before, exactly at, or anywhere after the location specified by 'index'.<br /><br />
        /// Any text outside of the first discovered quoted string is discarded.<br /><br />
        /// If <paramref name="withQuotes"/> is true, then the escape characters are returned, verbatim. Otherwise, they are discarded.
        /// </remarks>
        /// <exception cref="SyntaxErrorException">If <paramref name="throwException"/> is true, then this error is thrown if the quoted string is unterminated.</exception>
        ///
        public static string QuoteFromHere(char[] input, int index, ref int line, out int? startPos, out int? endPos, char interpolationChar = '$', char literalChar = '@', char interpolationBegin = '{', char interpolationEnd = '}', char quoteChar = '\"', char escChar = '\\', bool withQuotes = false, bool throwException = false)
        {
            int i = index, c = input.Length;
            var sb = new StringBuilder();

            startPos = null;
            endPos = null;

            while (i < c && input[i] != quoteChar)
            {
                i++;
            }

            if (i >= c)
            {
                return null;
            }

            if (input[i] == quoteChar)
            {
                startPos = i;

                if (withQuotes)
                {
                    sb.Append(input[i]);
                }
                else
                {
                    startPos++;
                }

                if (i > c - 1)
                {
                    if (throwException)
                        throw new SyntaxErrorException("Quote at end of file.");
                    else return null;
                }

                bool inLiteral = false;
                bool inInterpolation = false;
                int interpolationLevel = 0;

                List<bool> quoteOpen = null;
                List<bool> interpolationLiteral = null;

                if (i > 0 && input[i - 1] == literalChar) inLiteral = true;
                if (i > 0 && input[i - 1] == interpolationChar)
                {
                    inInterpolation = true;
                    quoteOpen = new List<bool>();
                    interpolationLiteral = new List<bool>();
                    quoteOpen.Add(true);
                    interpolationLiteral.Add(false);
                }

                for (int j = i + 1; j < c; j++)
                {
                    sb.Append(input[j]);

                    if (inInterpolation && input[j] == interpolationBegin)
                    {
                        interpolationLevel++;
                        quoteOpen.Add(false);
                        interpolationLiteral.Add(false);
                    }
                    else if (inInterpolation && input[j] == interpolationEnd)
                    {
                        if (!quoteOpen[interpolationLevel])
                        {
                            quoteOpen.RemoveAt(interpolationLevel);
                            interpolationLiteral.RemoveAt(interpolationLevel);

                            interpolationLevel--;
                        }
                    }
                    else if (input[j] == '\n')
                    {
                        int oline = line;

                        line++;
                        j++;
                        if (j < c) sb.Append(input[j]);

                        continue;
                    }
                    if (!inLiteral && input[j] == escChar)
                    {
                        if (inInterpolation)
                        {
                            if (quoteOpen[interpolationLevel])
                            {
                                j++;
                                if (j < c) sb.Append(input[j]);

                                continue;
                            }
                        }
                        else
                        {
                            j++;
                            if (j < c) sb.Append(input[j]);

                            continue;
                        }
                    }
                    else if (input[j] == quoteChar)
                    {
                        if (inInterpolation)
                        {
                            if (interpolationLiteral[interpolationLevel] && j < c - 1)
                            {
                                if (input[j + 1] == quoteChar)
                                {
                                    j++;
                                    if (j < c) sb.Append(input[j]);
                                    continue;
                                }
                            }
                            else
                            {
                                quoteOpen[interpolationLevel] = !quoteOpen[interpolationLevel];
                                if (quoteOpen[0] == false)
                                {
                                    i = j;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (inLiteral)
                            {
                                if (j < c - 1)
                                {
                                    if (input[j + 1] == quoteChar)
                                    {
                                        j++;
                                        if (j < c) sb.Append(input[j]);
                                        continue;
                                    }
                                }

                                i = j;
                                break;
                            }
                            else
                            {
                                i = j;
                                break;
                            }
                        }
                    }
                }

                if (i < c)
                {
                    if (!withQuotes)
                    {
                        endPos = i - 1;
                        sb.Remove(sb.Length - 1, 1);
                    }
                    else
                    {
                        endPos = i;
                    }

                    return sb.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Function to retrieve a quote from a string of data.
        /// </summary>
        /// <param name="value">The string to scan.</param>
        /// <param name="index">The position to begin scanning.</param>
        /// <param name="startPos">
        /// The returned start position of the quoted string.<br/><br />
        /// if <paramref name="withQuotes"/> is true, this will be the position of the quote character. Otherwise, it will be the position immediately after the quote character.
        /// </param>
        /// <param name="endPos">
        /// The returned end position of the quoted string.<br/><br />
        /// If <paramref name="withQuotes"/> is true, this will be the position of the quote character. Otherwise, it will be the position immediately before the quote character.
        /// </param>
        /// <param name="quoteChar">The quote character to use.</param>
        /// <param name="escChar">The escape character to use.</param>
        /// <param name="withQuotes">Return the string in quotes.</param>
        /// <param name="throwException">True to throw a <see cref="SyntaxErrorException"/> if an unterminated quoted string is found.</param>
        /// <returns>The requested string, or an empty string if no string is found.</returns>
        /// <remarks>
        /// The quote character must be: exactly one before, exactly at, or anywhere after the location specified by 'index'.<br /><br />
        /// Any text outside of the first discovered quoted string is discarded.<br /><br />
        /// If <paramref name="withQuotes"/> is true, then the escape characters are returned, verbatim. Otherwise, they are discarded.
        /// </remarks>
        /// <exception cref="SyntaxErrorException">If <paramref name="throwException"/> is true, then this error is thrown if the quoted string is unterminated.</exception>
        ///
        public static string QuoteFromHere(string value, int index, out int? startPos, out int? endPos, char quoteChar = '\"', char escChar = '\\', bool withQuotes = false, bool throwException = false)
        {
            int l = 0;
            return QuoteFromHere(value.ToCharArray(), index, ref l, out startPos, out endPos, '$', '@', '{', '}', quoteChar, escChar, withQuotes, throwException);
        }

        /// <summary>
        /// Determines if something really is a number. Supports C-style hex strings.
        /// </summary>
        /// <param name="subject">The subject to test.</param>
        /// <param name="noTrim">Whether to skip tripping white space around the text.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNumber(object o, bool noTrim = false)
        {
            if (o is char ch)
            {
                return char.IsNumber(ch);
            }
            if (o is string subject)
            {
                if (!noTrim)
                    subject = subject.Trim();

                double d = 0.0;
                ulong v;

                bool b = double.TryParse(subject, out d);

                if (b)
                    return true;

                if ((subject.IndexOf("0x") == 0) || (subject.IndexOf("&H") == 0))
                {
                    return ulong.TryParse(subject.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out v);
                }
                return false;
            }
            else
            {
                return o.GetType().IsPrimitive;
            }
        }

        /// <summary>
        /// Gets a clean filename extension from a string.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string CleanExtension(string s)
        {
            int i = s.LastIndexOf(".");
            if (i == -1)
                return "." + s.Trim().ToLower();

            return s.Substring(i).ToLower();
        }

        /// <summary>
        /// Tightens up a text string by removing extra "stuff" for parsing (possibly in a lexer).
        /// </summary>
        /// <param name="Input">The unput to process.</param>
        /// <param name="RemoveOperatorGaps">Specify whether to remove the gaps between equals signs and values.</param>
        /// <param name="RemoveComments">Specifies whether to remove comments.</param>
        /// <param name="CommentChars">Comment characters to use to discern comments.</param>
        /// <returns>Processed text.</returns>
        /// <remarks></remarks>
        public static string TightenText(string Input, bool RemoveOperatorGaps = true, bool RemoveComments = true, string CommentChars = "'")
        {
            // ERROR: Not supported in C#: OnErrorStatement

            // as efficiently as possible
            char[] c = null;
            int a = 0;
            int b = 0;

            int i = 0;
            int j = 0;

            char[] cmt = CommentChars.ToCharArray();

            char[] ops = "=".ToCharArray();

            char[] d = null;
            int p = 0;

            int spc = 0;

            byte t = 0;
            byte f = 0;

            c = Input.ToCharArray();
            b = c.Length - 1;

            d = new char[b + 1];

            for (a = 0; a <= b; a++)
            {
                if ((RemoveComments == true))
                {
                    j = cmt.Length - 1;

                    for (i = 0; i <= j; i++)
                    {
                        if ((c[a] == cmt[i]))
                            break;
                    }
                }

                if ((t == 0) & (c[a] == ' '))
                {
                    continue;
                }
                else if ((t == 0))
                {
                    t = 1;
                }

                if ((c[a] == ' '))
                {
                    spc += 1;
                }
                else if ((spc > 0) && (f == 0))
                {
                    d[p] = ' ';
                    d[p + 1] = c[a];
                    p += 2;
                    spc = 0;
                }
                else
                {
                    d[p] = c[a];
                    if (f == 1)
                    {
                        f = 0;
                        spc = 0;
                    }
                    p++;
                }

                if ((RemoveOperatorGaps))
                {
                    j = ops.Length - 1;

                    for (i = 0; i <= j; i++)
                    {
                        if ((c[a] == ops[i]))
                        {
                            f = 1;
                            if ((a > 0))
                            {
                                if (d[p - 2] == ' ')
                                {
                                    d[p - 2] = c[a];
                                    p -= 1;

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            Array.Resize(ref d, p);
            return new string(d);
        }

        public static string JustNumbers(string value, bool justDigits = false)
        {
            if (justDigits)
            {
                return JustDigits(value);
            }
            else
            {
                return JustNumbers(value, out _);
            }
        }

        /// <summary>
        /// Returns a string suitable for parsing by Val() or <see cref="FVal" />.
        /// The default behavior processes the string exactly as the Val function looks at it, but it is customizable.
        /// </summary>
        /// <param name="value">String to process.</param>
        /// <param name="values">Receives the string values of all discovered individual numbers based upon the selected configuration.</param>
        /// <param name="justFirst">Whether to process just the first discovered word.</param>
        /// <param name="allInOne">Whether to merge all word blocks together before processing.</param>
        /// <param name="maxSkip">The maximum number of words to skip in search of a number.</param>
        /// <param name="skipChars">Specific characters to ignore or step over in search of a number (default is common currency).</param>
        /// <returns>A result ready to be parsed by a numeric parser.</returns>
        /// <remarks></remarks>
        public static string JustNumbers(string value, out string[] values, bool justFirst = true, bool allInOne = true, int maxSkip = 0, string skipChars = "$£€#")
        {
            char[] sn = value.ToCharArray();
            char[] sc = null;

            int i = 0;
            int c = sn.Length - 1;

            int e = 0;

            int skip = -1;

            int d = 0;
            bool t = false;

            string firstScan = "&0123456789+-. ";
            string scan = " 1234567890-+.eEHhOoxX";

            sc = new char[c + 1];

            for (i = 0; i <= c; i++)
            {
                if (!t)
                {
                    if (firstScan.IndexOf(sn[i]) >= 0)
                    {
                        t = true;
                    }
                    else
                    {
                        if (justFirst)
                        {
                            if (maxSkip > -1 && skip > maxSkip)
                            {
                                values = null;
                                return "";
                            }
                            else
                            {
                                skip += 1;
                            }
                        }
                    }
                }

                if (t)
                {
                    if (scan.IndexOf(sn[i]) >= 0)
                    {
                        if (sn[i] == ' ')
                        {
                            if (justFirst && d != 0)
                            {
                                break;
                            }

                            d += 1;
                            t = false;

                            if (!allInOne)
                            {
                                sc[e] = sn[i];
                                e += 1;
                            }
                        }
                        else
                        {
                            sc[e] = sn[i];
                            e += 1;
                        }
                    }
                }
                else if (justFirst && !string.IsNullOrEmpty(skipChars) && skipChars.IndexOf(sn[i]) == -1)
                {
                    values = null;
                    return "";
                }
            }

            if (e == 0)
            {
                values = null;
                return "";
            }

            if (e < c)
            {
                Array.Resize(ref sc, e + 1);
            }

            values = Split(new string(sc), " ");
            return new string(sc);
        }

        public static string JustDigits(string value)
        {
            var chars = value.ToCharArray();
            var sb = new StringBuilder();

            foreach (var ch in chars)
            {
                if (char.IsDigit(ch))
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// A better Val() function.  Will parse hexadecimal with 0x or &amp;H markers or binary digits with a 'b' marker.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>An numeric primitive (either a Long or a Double).</returns>
        /// <remarks></remarks>
        public static double? FVal(string value)
        {
            double o;

            value = value.Trim();
            if (value.Contains(" "))
            {
                var sp = value.Split(' ');
                value = sp[0];
            }

            if (value.Length < 2)
            {
                if (IsNumber(value) == false)
                    return double.NaN;

                if (double.TryParse(value, out o))
                    return o;
            }

            if ((value.Substring(0, 2) == "&H") || (value.Substring(0, 2) == "0x"))
            {
                value = value.Substring(2);
                o = (double)long.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier);
                return o;
            }
            else if (value.ToLower()[0] == 'b')
            {
                int i;
                int c;
                char[] ch;
                long v = 0;

                ch = value.Substring(1).ToCharArray();
                c = ch.Length - 1;

                for (i = 0; i <= c; i++)
                {
                    if (ch[i] == '1')
                    {
                        v += 1;
                    }
                    else if (ch[i] != '0')
                    {
                        return v;
                    }
                    v <<= 1;
                }

                return (v);
            }

            if (IsNumber(value) == false)
                return double.NaN;

            if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture.NumberFormat, out o))
                return (double)o;

            return null;
        }

        /// <summary>
        /// Escape text for use in a CSV file.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string TextEscapeCSV(string s)
        {
            char[] b = null;
            int i = 0;

            if (s == null) return "";

            StringBuilder sb = new StringBuilder();

            b = s.ToCharArray();

            for (i = 0; i <= b.Length - 1; i++)
            {
                switch (b[i])
                {
                    case '\"':
                        sb.Append("\"\"");
                        break;

                    default:
                        sb.Append(b[i]);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escape text for use in a Json file.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string TextEscapeJson(string s)
        {
            char[] b;
            int i;

            StringBuilder sOut = new StringBuilder();

            if (s == null) return "";

            b = s.ToCharArray();

            for (i = 0; i < b.Length; i++)
            {
                if (i < b.Length - 1)
                {
                    if ((b[i] == '\\') && (b[i + 1] == '\"'))
                    {
                        sOut.Append("\\\"");
                        i++;
                        continue;
                    }
                }

                if (MultiComp(b[i], ("\\\"/").ToCharArray()))
                {
                    sOut.Append("\\");
                }

                sOut.Append(b[i]);
            }

            return sOut.ToString();
        }

        /// <summary>
        /// Replaces all instances of 'search' with 'replace in 'subj'
        /// </summary>
        /// <param name="subj">The string to scan.</param>
        /// <param name="search">The text for which to scan.</param>
        /// <param name="replace">The replacement text.</param>
        /// <returns></returns>
        public static string SearchReplace(string subj, string search, string replace)
        {
            int i;

            do
            {
                i = subj.IndexOf(search);
                if (i != -1) subj = subj.Replace(search, replace);
            } while (i != -1);

            return subj;
        }

        /// <summary>
        /// Removes all blank lines from a string.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string NoNullLines(string i)
        {
            string[] st = null;
            int l = 0;

            StringBuilder sOut = new StringBuilder();

            i = SearchReplace(i, "\r\n", "\r");
            i = SearchReplace(i, "\n", "\r");

            st = Split(i, "\r");

            for (l = 0; l <= st.Length - 1; l++)
            {
                if ((!string.IsNullOrEmpty(st[l])))
                {
                    if ((sOut.Length != 0))
                        sOut.Append("\r\n");
                    sOut.Append(st[l]);
                }
            }

            return sOut.ToString();
        }

        /// <summary>
        /// Clear all null characters from a string
        /// </summary>
        /// <param name="input">String to process.</param>
        /// <returns>Processed text.</returns>
        /// <remarks></remarks>
        public static string RemoveNulls(string input)
        {
            return input.Trim((char)0);
        }

        /// <summary>
        /// Reduces extraneous spacing, and ensures only one space exists at any given place.
        /// </summary>
        /// <param name="input">The string to process.</param>
        /// <param name="spaceChars">The characters to interpret as space characters.</param>
        /// <param name="PreserveQuotedText">Whether to preserve multiple spaces within quoted text.</param>
        /// <param name="quoteChar">The quote character to use for determining the location of quoted text.</param>
        /// <param name="escapeChar">The escape character to use to recognize the escaping of quote characters.</param>
        /// <param name="Quick">Whether to perform a quick search and replace.  If this parameter is set to true, all other optional parameters are ignored.</param>
        /// <returns>Processed text.</returns>
        /// <remarks></remarks>
        public static string OneSpace(string input, string spaceChars = " ", bool PreserveQuotedText = true, char quoteChar = '\"', char escapeChar = '\\', bool Quick = true)
        {
            int a = 0;
            int b = 0;

            StringBuilder varOut = new StringBuilder();
            bool isP = false;
            bool iQ = false;

            char[] ch = input.ToCharArray();

            if (Quick)
            {
                if (input.IndexOf("  ") == -1)
                {
                    return input;
                }

                while (!(input.IndexOf("  ") == -1))
                {
                    input = input.Replace("  ", " ");
                }

                return input;
            }

            b = ch.Length - 1;

            for (a = 0; a <= b; a++)
            {
                if ((iQ == true))
                {
                    varOut.Append(ch[a]);
                    if ((ch[a] == quoteChar))
                    {
                        if (a > 0)
                        {
                            if (ch[a - 1] == escapeChar)
                            {
                                continue;
                            }
                        }
                        iQ = false;
                    }
                }
                else
                {
                    if ((spaceChars.IndexOf(ch[a]) != -1) & (isP == false))
                    {
                        isP = true;
                        varOut.Append(ch[a]);
                    }
                    else if ((spaceChars.IndexOf(ch[a]) == -1))
                    {
                        varOut.Append(ch[a]);
                        if ((isP == true))
                            isP = false;
                        if ((ch[a] == quoteChar) & (PreserveQuotedText == true))
                        {
                            iQ = true;
                        }
                    }
                }
            }

            return varOut.ToString();
        }

        public static string Bracket(string szText, ref int startIndex, ref int newIndex, ref string ErrorText)
        {
            return Bracket(szText, ref startIndex, ref newIndex, "()", ref ErrorText);
        }

        public static string Bracket(string szText, ref int startIndex, ref int newIndex)
        {
            string _d = null;
            return Bracket(szText, ref startIndex, ref newIndex, "()", ref _d);
        }

        /// <summary>
        /// Get all text within the first occurance of a specified bracket set.  Discards text outside.
        /// </summary>
        /// <param name="szText">String to scan.</param>
        /// <param name="startIndex">Index in the string at which to start scanning.</param>
        /// <param name="newIndex">Receives the index of the first character after the closing bracket.</param>
        /// <param name="BracketPair">Bracket pair (must consist of exactly 2 characters, for other division pairs, use TextBetween.)</param>
        /// <param name="ErrorText"></param>
        /// <returns>The text inside the first complete bracket, excluding the outer-most pair.</returns>
        /// <remarks></remarks>
        public static string Bracket(string szText, ref int startIndex, ref int newIndex, string BracketPair, ref string ErrorText)
        {
            // returns the text inside the first complete bracket, excluding the outer-most pair. newIndex is set to the first character after the closing bracket

            char[] ch = szText.ToCharArray();
            char[] bp = BracketPair.ToCharArray();
            int i = 0;
            int c = ch.Length - 1;

            string sOut = "";
            int n = 0;
            int bc = 0;

            char open;
            char close;

            if (BracketPair.Length != 2)
            {
                ErrorText = "Invalid bracket pair string. The bracket pair string must consist of exactly one open character and exactly one close character.";
                return null;
            }

            open = bp[0];
            close = bp[1];

            try
            {
                newIndex = startIndex;

                if (string.IsNullOrEmpty(szText))
                    return "";
                i = szText.IndexOf(open, startIndex);

                if (i == -1)
                {
                    // there are no brackets, so we shall assume that entire string from startIndex to finish is a "bracket",
                    // we'll return that text and set newIndex to the end of the line

                    newIndex = 0;
                    i = szText.IndexOf(close);

                    if (i != -1)
                    {
                        ErrorText = "Syntax error.  Unexpected closing bracket at column " + i + 1;
                        return null;
                    }

                    newIndex = ch.Length;
                    return szText.Substring(startIndex);
                }

                for (n = i; n <= c; n++)
                {
                    sOut += ch[n];

                    if ((ch[n] == open))
                    {
                        bc += 1;
                    }
                    else if ((ch[n] == close))
                    {
                        bc -= 1;
                        if ((bc < 0))
                        {
                            ErrorText = "Syntax error.  Unexpected closing bracket at column " + i + 1;
                            return null;
                        }
                        else if (bc == 0)
                        {
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }

                if (bc != 0)
                {
                    ErrorText = "Syntax error.  Unmatched closing bracket at column " + i + 1;
                    return null;
                }

                sOut = sOut.Substring(1, sOut.Length - 2);

                newIndex = n + 1;
                startIndex = i;

                return sOut;
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Get all text within the first occurance of a specified bracket set.  Discards text outside.
        /// </summary>
        /// <param name="szText">String to scan.</param>
        /// <param name="BracketPair">Bracket pair (must consist of exactly 2 characters, for other division pairs, use TextBetween.)</param>
        /// <returns>The text inside the first complete bracket, excluding the outer-most pair.</returns>
        /// <remarks></remarks>
        public static string Bracket(string szText, string BracketPair = "()")
        {
            string et = null;
            int i = 0, j = 0;
            return Bracket(szText, ref i, ref j, BracketPair, ref et);
        }

        /// <summary>
        /// Space out operators in preparation for mathematical parsing.
        /// </summary>
        /// <param name="value">The text to process.</param>
        /// <param name="Operators">The list of operator characters to use.</param>
        /// <param name="SepChars">The list of separator characters to use (default is all white space characters in the current culture).</param>
        /// <param name="StickyCharsLeft">Character operators that should stick to the text on their right if it is adjacent (not separated by a separator character).</param>
        /// <param name="StickyCharsRight">Character operators that should stick to the text on their left if it is adjacent (not separated by a separator character).</param>
        /// <param name="NoStickyChars">Characters that under no circumstances should stick to adjacent characters.</param>
        /// <returns>Processed text.</returns>
        /// <remarks></remarks>
        public static string SpaceOperators(string value, string Operators = "/\\&^%*-+^!?|", string SepChars = null, string StickyCharsLeft = "", string StickyCharsRight = "!+-?", string NoStickyChars = "")
        {
            int i = 0;
            int c = 0;

            string s = null;
            char ch = '\0';
            bool inq = false;
            bool inqs = false;

            char[] sp = null;

            if (SepChars == null)
            {
                SepChars = "";
                for (i = 0; i <= 255; i++)
                {
                    ch = (char)i;
                    if (char.IsWhiteSpace(ch))
                    {
                        SepChars += ch;
                    }
                }
            }

            if (string.IsNullOrEmpty(SepChars))
                return value;

            sp = value.ToCharArray();
            c = sp.Length - 1;
            s = "";

            if (StickyCharsLeft == null)
                StickyCharsLeft = "";
            if (StickyCharsRight == null)
                StickyCharsRight = "";
            if (NoStickyChars == null)
                NoStickyChars = "";

            for (i = 0; i <= c; i++)
            {
                if ((sp[i] == '\"'))
                {
                    if (!(i > 0 && inq && sp[i - 1] == '\\'))
                        inq = !inq;
                }
                else if ((sp[i] == '\''))
                {
                    if (!(i > 0 && inq && sp[i - 1] == '\\'))
                        inqs = !inqs;
                }

                if (!inq && !inqs && Operators.IndexOf(sp[i]) != -1)
                {
                    if ((i > 0) && (Operators.IndexOf(sp[i - 1]) != -1) && (NoStickyChars.IndexOf(sp[i]) == -1) && (NoStickyChars.IndexOf(sp[i - 1]) == -1))
                    {
                        if ((StickyCharsRight.IndexOf(sp[i - 1]) <= -1))
                        {
                            s = s.Substring(0, s.Length - 1);
                        }

                        s += sp[i];
                    }
                    else
                    {
                        if (StickyCharsLeft.IndexOf(sp[i]) == -1 || (NoStickyChars.IndexOf(sp[i]) != -1))
                        {
                            s += " ";
                        }

                        s += sp[i];
                    }

                    if (StickyCharsRight.IndexOf(sp[i]) == -1 || (NoStickyChars.IndexOf(sp[i]) != -1))
                    {
                        s += " ";
                    }
                }
                else
                {
                    s += sp[i];
                }
            }

            return (OneSpace(s, SepChars)).Trim();
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
        /// Returns all words in a string as an array of strings.
        /// </summary>
        /// <param name="Text">Text to split.</param>
        /// <param name="SepChars">Separator characters to use.</param>
        /// <param name="AdditionalSepChars">Any additional separator characters to use.</param>
        /// <param name="SkipQuotes">Skip over quoted text.</param>
        /// <param name="Unescape">Unescape quoted text.</param>
        /// <param name="qc">Quote character to use.</param>
        /// <param name="ec">Escape character to use.</param>
        /// <returns>Array of strings.</returns>
        /// <remarks></remarks>
        public static string[] Words(string Text, string SepChars = null, string AdditionalSepChars = null, bool SkipQuotes = false, bool Unescape = false, char qc = '\"', char ec = '\\')
        {
            int i = 0;
            int c = 0;

            int n = 0;

            string s = null;
            char ch = '\0';

            string[] stout = null;
            string[] stwork = null;
            string[] stwork2 = null;

            char[] sep = null;

            if (SepChars == null)
            {
                SepChars = "";
                for (i = 0; i <= 255; i++)
                {
                    ch = (char)(i);
                    if (char.IsWhiteSpace(ch))
                    {
                        //if (ch != '\r' && ch != '\n')
                        SepChars += ch;
                    }
                }
            }

            if (string.IsNullOrEmpty(SepChars))
                return new string[] { Text };

            s = SepChars;
            if (AdditionalSepChars != null) s += AdditionalSepChars;

            sep = s.ToCharArray();
            ch = sep[0];

            s = Text;
            s = OneSpace(s, "" + ch, SkipQuotes).Trim();

            stout = Split(s, "" + ch, SkipQuotes, Unescape, qc, ec);

            if (stout == null)
                return new string[] { s };

            c = stout.Length - 1;

            if (SepChars.Length == 1)
            {
                return stout;
            }

            SepChars = SepChars.Substring(1);

            for (i = 0; i <= c; i++)
            {
                stwork = Words(stout[i], SepChars, null, SkipQuotes, Unescape, qc, ec);
                if (stwork2 == null)
                {
                    stwork2 = stwork;
                    continue;
                }

                n = stwork2.Length;
                Array.Resize(ref stwork2, n + stwork.Length);
                Array.Copy(stwork, 0, stwork2, n, stwork.Length);
            }

            return stwork2;
        }

        /// <summary>
        /// Wrap the input text by the given columns.
        /// </summary>
        /// <param name="szText">Text to wrap.</param>
        /// <param name="Cols">Maximum character columns.</param>
        /// <returns>Wrapped text.</returns>
        /// <remarks></remarks>
        public static string Wrap(string szText, int Cols = 60)
        {
            string[] st = null;
            int xTot = 0;
            string sOut = "";
            int i = 0;
            int j = 0;

            if (szText == null) return null;

            if ((szText.Length <= Cols))
                return szText;

            st = Words(szText);

            for (i = 0; i < st.Length; i++)
            {
                if ((st[i].Length >= Cols))
                {
                    if (sOut.Substring(sOut.Length - 1, 1) == " ")
                        sOut = sOut.Substring(0, sOut.Length - 1);

                    sOut += "\r\n";
                    j = 0;

                    while ((j < st[i].Length))
                    {
                        try
                        {
                            sOut += st[i].Substring(j, Cols);
                        }
                        catch (Exception)
                        {
                            sOut += st[i].Substring(j);
                        }

                        j += Cols;

                        if (sOut.Substring(sOut.Length - 1, 1) == " ")
                            sOut = sOut.Substring(0, sOut.Length - 1);

                        sOut += "\r\n";
                    }
                    xTot = 0;

                    continue;
                }
                else if ((xTot + st[i].Length) > Cols)
                {
                    if (sOut.Substring(sOut.Length - 1, 1) == " ")
                        sOut = sOut.Substring(0, sOut.Length - 1);

                    sOut += "\r\n";
                    xTot = 0;
                }

                xTot += st[i].Length;
                sOut += st[i];

                if ((i < (st.Length - 1)))
                {
                    xTot++;
                    sOut += " ";
                }
            }

            return sOut;
        }

        /// <summary>
        /// Returns all lines in a string.
        /// </summary>
        /// <param name="szText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string[] GetLines(string szText)
        {
            return szText.Replace("\r", "").Replace("\n", "\0").Split('\0');
        }

        /// <summary>
        /// Return a padded hexadecimal value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="width"></param>
        /// <param name="prefix"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string PadHex(object value, int width = 8, string prefix = "", bool lowercase = false)
        {
            string s;
            s = ((ulong)value).ToString("X");

            if ((lowercase))
                s = s.ToLower();

            if ((width - s.Length > 0))
            {
                return prefix + new string('0', width - s.Length) + s;
            }
            else
            {
                return prefix + s;
            }
        }

        /// <summary>
        /// Determines of a number can be parsed in hexadecimal
        /// (quick version, does not accept &amp;H or 0x, use IsHex() to parse those strings).
        /// </summary>
        /// <param name="input">Input string to scan.</param>
        /// <param name="value">Optionally receives the value of the input string.</param>
        /// <returns>True if the string can be parsed as a hexadecimal number.</returns>
        /// <remarks></remarks>
        public static bool IsHexQ(string input, ref int value)
        {
            string hx = "0123456789ABCDEFabcdef";
            int i = 0;
            int c = input.Length - 1;

            for (i = 0; i <= c; i++)
            {
                if (hx.IndexOf(input[i]) == -1)
                {
                    return false;
                }
            }

            value = int.Parse(input, System.Globalization.NumberStyles.HexNumber);
            return true;
        }

        /// <summary>
        /// Determines of a number can be parsed in hexadecimal
        /// (quick version, does not accept &amp;H or 0x, use IsHex() to parse those strings).
        /// </summary>
        /// <param name="input">Input string to scan.</param>
        /// <returns>True if the string can be parsed as a hexadecimal number.</returns>
        /// <remarks></remarks>
        public static bool IsHexQ(string input)
        {
            int i = 0;
            return IsHexQ(input, ref i);
        }

        /// <summary>
        /// Returns true if the value in a hexadecimal number. Accepts &amp;H and 0x prefixes.
        /// </summary>
        /// <param name="hin">String to scan</param>
        /// <param name="value">Optionally receives the parsed value.</param>
        /// <returns>True if the string can be parsed as hex.</returns>
        /// <remarks></remarks>
        public static bool IsHex(string hin, ref int value)
        {
            char[] b = null;
            int i = 0;

            bool c = true;

            b = hin.ToCharArray();

            if (hin.IndexOf("&H") == -1 && hin.IndexOf("0x") == -1)
                return IsHexQ(hin, ref value);

            hin = hin.Replace("&H", "");
            hin = hin.Replace("0x", "");

            for (i = 0; i <= b.Length - 1; i++)
            {
                c = "ABCDEFabcdef0123456789".Contains(b[i]);
                if (!c) break;
            }

            if (c)
            {
                value = (int)FVal("&H" + hin);
            }

            return c;
        }

        /// <summary>
        /// Returns true if the value in a hexadecimal number. Accepts &amp;H and 0x prefixes.
        /// </summary>
        /// <param name="hin">String to scan</param>
        /// <returns>True if the string can be parsed as hex.</returns>
        /// <remarks></remarks>
        public static bool IsHex(string hin)
        {
            int i = 0;
            return IsHex(hin, ref i);
        }

        /// <summary>
        /// Removes comments from a line of text.
        /// </summary>
        /// <param name="input">Text to parse.</param>
        /// <param name="commentchar">Comment marker</param>
        /// <returns>A string with no comments.</returns>
        /// <remarks></remarks>
        public static string NoComment(string input, string commentchar = "//")
        {
            int a = 0;
            int b = 0;

            string varOut = "";

            bool isP = false;
            bool iQ = false;

            b = input.Length - 1;

            for (a = 0; a <= b; a++)
            {
                if ((iQ == true))
                {
                    varOut += input[a];
                    if ((input[a] == '\"'))
                    {
                        iQ = false;
                    }
                }
                else
                {
                    if ((a < (b - commentchar.Length)))
                    {
                        if ((input.Substring(a, commentchar.Length) == commentchar))
                        {
                            break;
                        }
                    }
                    varOut += input[a];

                    if ((isP == true))
                        isP = false;
                    if ((input[a] == '\"'))
                    {
                        iQ = true;
                    }
                }
            }

            return varOut;
        }

        /// <summary>
        /// Separate and clean up a string.
        /// </summary>
        /// <param name="value">String to separate.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Separate(string value) =>
            Separate(value, ' ', true, true, true);

        /// <summary>
        /// Separate and clean up a string.
        /// </summary>
        /// <param name="value">String to separate.</param>
        /// <param name="sepChar">The new separator character.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Separate(string value, char sepChar) =>
            Separate(value, sepChar, true, true, true);

        /// <summary>
        /// Separate and clean up a string.
        /// </summary>
        /// <param name="value">String to separate.</param>
        /// <param name="sepChar">The new separator character.</param>
        /// <param name="sepCamel">Separate by camelCase or PascalCase.</param>
        /// <param name="capitalize">Capitalize all separated words.</param>
        /// <param name="sepBy">Separate by the characters specified to detect separate words..</param>
        /// <param name="sepByChars">Use these characters to detect separate words.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Separate(string value, char sepChar, bool sepCamel, bool capitalize, bool sepBy, string sepByChars = " _-") =>
            Separate(value, sepChar, sepCamel, capitalize, sepBy, sepByChars.ToCharArray());

        /// <summary>
        /// Separate and clean up a string.
        /// </summary>
        /// <param name="value">String to separate.</param>
        /// <param name="sepChar">The new separator character.</param>
        /// <param name="sepCamel">Separate by camelCase or PascalCase.</param>
        /// <param name="capitalize">Capitalize all separated words.</param>
        /// <param name="sepBy">Separate by the characters specified to detect separate words..</param>
        /// <param name="sepByChars">Use these characters to detect separate words.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Separate(string value, char sepChar, bool sepCamel, bool capitalize, bool sepBy, char[] sepByChars)
        {
            if (value == null || (sepBy && sepByChars == null)) throw new ArgumentNullException();

            char[] ch = value.ToCharArray();
            List<char> seps = new List<char>(sepByChars);

            value = value.Trim();

            int i;
            int c = ch.Length;

            bool lastWasSeperator = false;
            bool lastWasCapital = false;
            bool lastWasChar = false;

            StringBuilder sb = new StringBuilder(c * 2);

            for (i = 0; i < c; i++)
            {
                if (sepBy && seps.Contains(ch[i]))
                {
                    if (!lastWasSeperator && lastWasChar)
                    {
                        lastWasSeperator = true;
                        sb.Append(sepChar);
                    }

                    continue;
                }
                else if (ch[i] == sepChar)
                {
                    if (!lastWasSeperator && lastWasChar)
                    {
                        lastWasSeperator = true;
                        sb.Append(sepChar);
                    }

                    continue;
                }

                if ((lastWasSeperator || i == 0) && capitalize)
                {
                    lastWasCapital = true;
                    sb.Append(char.ToUpper(ch[i]));
                }
                else
                {
                    if (sepCamel && char.IsUpper(ch[i]) && !lastWasSeperator)
                    {
                        if (!lastWasCapital) sb.Append(sepChar);
                        else if (i < c - 1 && char.IsLower(ch[i + 1]))
                        {
                            sb.Append(sepChar);
                        }
                        lastWasCapital = true;
                    }
                    else
                    {
                        lastWasCapital = false;
                    }

                    sb.Append(ch[i]);
                }

                lastWasSeperator = false;
                lastWasChar = true;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert spaced or underscored lines to camelCase.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="stripSpaces">Specify whether or not to remove space characters.</param>
        /// <returns></returns>
        /// <remarks>The function is an alias for <see cref="PascalCase(string, bool, bool)"/></remarks>
        public static string CamelCase(string input, bool stripSpaces = true)
        {
            return PascalCase(input, stripSpaces, true);
        }

        /// <summary>
        /// Convert spaced or underscored lines to PascalCase or camelCase.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="stripSpaces">Specify whether or not to remove space characters.</param>
        /// <param name="camel">Convert to camelCase</param>
        /// <returns></returns>
        /// <remarks>The function is an alias for <see cref="PascalCase(string, bool, bool)"/></remarks>
        public static string TitleCase(string input, bool stripSpaces = true, bool camel = false)
        {
            return PascalCase(input, stripSpaces, camel);
        }

        /// <summary>
        /// Convert spaced or underscored lines to PascalCase or camelCase.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="stripSpaces">Specify whether or not to remove space characters.</param>
        /// <param name="camel">Convert to camelCase</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string PascalCase(string input, bool stripSpaces = true, bool camel = false)
        {
            int a = 0;
            int b = 0;

            StringBuilder varOut = new StringBuilder();
            bool nextCap = false;
            bool iQ = false;

            if (input == null)
                return "";

            input = SearchReplace(input, "_", " ");

            b = input.Length - 1;
            for (a = 0; a <= b; a++)
            {
                if ((iQ == true))
                {
                    varOut.Append(input[a]);
                    if (input[a] == '\"')
                    {
                        iQ = false;
                    }
                }
                else
                {
                    if (!char.IsLetter(input[a]))
                    {
                        nextCap = true;
                        varOut.Append(input[a]);
                    }
                    else if (char.IsLetter(input[a]))
                    {
                        if (a == 0 || nextCap)
                        {
                            if (a > 0 && char.IsLetter(input[a - 1]) && input[a - 1] == char.ToLower(input[a - 1]) && input[a] == char.ToUpper(input[a]))
                            {
                                varOut.Append(char.ToUpper(input[a]));
                            }
                            else if (a > 0 && input[a - 1] == '\'')
                            {
                                varOut.Append(char.ToLower(input[a]));
                            }                                
                            else
                            {
                                varOut.Append(char.ToUpper(input[a]));
                            }
                        }
                        else
                        {
                            varOut.Append(char.ToLower(input[a]));
                        }

                        if (nextCap)
                            nextCap = false;

                        if (input[a] == '\"')
                        {
                            iQ = true;
                        }
                    }
                }
            }

            if (camel)
            {
                varOut[0] = char.ToLower(varOut[0]);
            }

            if (stripSpaces)
                return varOut.ToString().Replace(" ", "");
            else
                return varOut.ToString();
        }

        /// <summary>
        /// Converts an IP address string into a 32 bit signed integer.
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns></returns>
        public static int IPInt(string ipaddr)
        {
            try
            {
                int output = 0;
                var toks = ipaddr.Split(".");

                if (toks.Length != 4) return 0;

                for (int i = 3; i >= 0; i--)
                {
                    output = (output << 8) | int.Parse(toks[i]);
                }

                return output;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts a signed 32 bit integer into an IP address string.
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns></returns>

        public static string IPStr(int ipaddr)
        {
            string output = "";

            for (int i = 3; i >= 0; i--)
            {
                if (output != "") output += ".";
                output += ((ipaddr >> (8 * i)) & 0xff).ToString();
            }

            return output;
        }

        /// <summary>
        /// Concats all strings in a string array into one string.
        /// </summary>
        /// <param name="Text">Array to combine.</param>
        /// <returns>A string.</returns>
        /// <remarks></remarks>
        public static string Stream(string[] Text)
        {
            long i = 0;
            long b = 0;

            StringBuilder sb = new StringBuilder();

            i = -1L;
            i = Text.Length - 1;
            for (b = 0; b < i; b++)
            {
                if (sb.Length != 0)
                    sb.Append("\r\n");
                sb.Append(Text[b]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Filters text using odd pairs of characters, when the beginning and end bounds have different constituents.
        /// </summary>
        /// <param name="Text">The text to filter.</param>
        /// <param name="FilterPair">Exactly 2 characters that represent the pair to filter.</param>
        /// <param name="Escape">Escape character to use.</param>
        /// <param name="FirstIsFilter">Receives a value indicating that the first character of the input text was an opening filter character.</param>
        /// <returns>Filtered text.</returns>
        /// <remarks></remarks>
        public static string[] Filter(string Text, ref bool FirstIsFilter, string FilterPair = "\"", string Escape = "\\")
        {
            string[] lnOut = null;

            int i = 0;
            int j = 0;
            int m = 0;

            char[] c = null;
            char[] fp = null;
            bool e = false;

            // ERROR: Not supported in C#: OnErrorStatement

            if ((FilterPair.Length == 0))
                FilterPair = "\"";

            if ((FilterPair.Length == 1))
            {
                FilterPair = FilterPair + FilterPair;
            }

            fp = FilterPair.ToCharArray();

            c = Text.ToCharArray();
            j = -1;
            j = c.Length - 1;

            m = -1;

            for (i = 0; i <= j; i++)
            {
                if ((e == false))
                {
                    if ((c[i] == fp[0]))
                    {
                        if ((m == -1))
                        {
                            FirstIsFilter = true;
                        }
                        m += 1;
                        e = true;
                        Array.Resize(ref lnOut, m + 1);
                        lnOut[m] = "" + c[i];
                    }
                    else
                    {
                        if ((m == -1))
                        {
                            FirstIsFilter = false;
                            m += 1;
                            Array.Resize(ref lnOut, m + 1);
                        }

                        lnOut[m] += c[i];
                    }
                }
                else
                {
                    if ((c[i] == fp[1]))
                    {
                        if ((i > 0))
                        {
                            if ((!string.IsNullOrEmpty(Escape)) & (Escape != null))
                            {
                                if ((c[i - 1] == Escape[0]))
                                {
                                    if ((lnOut[m].Length > 1))
                                    {
                                        lnOut[m] = lnOut[m].Substring(0, lnOut[m].Length - 1);
                                    }
                                }
                            }
                        }

                        lnOut[m] += c[i];

                        m += 1;
                        e = false;
                        Array.Resize(ref lnOut, m + 1);
                    }
                    else
                    {
                        lnOut[m] += c[i];
                    }
                }
            }

            return lnOut;
        }

        public static long ExaBiConst = 1024L * 1024 * 1024 * 1024 * 1024 * 1024;
        public static long PetaBiConst = 1024L * 1024 * 1024 * 1024 * 1024;
        public static long TeraBiConst = 1024L * 1024 * 1024 * 1024;
        public static long GigaBiConst = 1024L * 1024 * 1024;
        public static long MegaBiConst = 1024L * 1024;
        public static long KiloBiConst = 1024L;

        public static long ExaConst = 1000L * 1000 * 1000 * 1000 * 1000 * 1000;
        public static long PetaConst = 1000L * 1000 * 1000 * 1000 * 1000;
        public static long TeraConst = 1000L * 1000 * 1000 * 1000;
        public static long GigaConst = 1000L * 1000 * 1000;
        public static long MegaConst = 1000L * 1000;
        public static long KiloConst = 1000L;

        public static readonly string[] BinSpeedBytes = { "EiB/s", "PiB/s", "TiB/s", "GiB/s", "MiB/s", "KiB/s", "B/s" };
        public static readonly string[] BinSpeedBits = { "Eib/s", "Pib/s", "Tib/s", "Gib/s", "Mib/s", "Kib/s", "b/s" };

        public static readonly string[] BinSizeBytes = { "EiB", "PiB", "TiB", "GiB", "MiB", "KiB", "B" };
        public static readonly string[] BinSizeBits = { "Eib", "Pib", "Tib", "Gib", "Mib", "Kib", "b" };

        public static readonly string[] DecSpeedBytes = { "EB/s", "PB/s", "TB/s", "GB/s", "MB/s", "KB/s", "B/s" };
        public static readonly string[] DecSpeedBits = { "Eb/s", "Pb/s", "Tb/s", "Gb/s", "Mb/s", "Kb/s", "b/s" };

        public static readonly string[] DecSizeBytes = { "EB", "PB", "TB", "GB", "MB", "KB", "B" };
        public static readonly string[] DecSizeBits = { "Eb", "Pb", "Tb", "Gb", "Mb", "Kb", "b" };

        public static readonly long[] BinValues = { ExaBiConst, PetaBiConst, TeraBiConst, GigaBiConst, MegaBiConst, KiloBiConst };
        public static readonly long[] DecValues = { ExaConst, PetaConst, TeraConst, GigaConst, MegaConst, KiloConst };

        /// <summary>
        /// Prints a very large value for a size or transmission rate in a human-friendly format
        /// </summary>
        /// <param name="value">The value to print</param>
        /// <param name="numFmt">The optional number format.</param>
        /// <param name="speed">True for speed, false for size.</param>
        /// <param name="binary">True to use the 1024 method, false to use the 1000 method.</param>
        /// <param name="displayBinary">If <paramref name="binary"/> is true, then display the *bibits as opposed to the *gabits format.</param>
        /// <returns></returns>
        /// <remarks>
        /// Supports either bits or bytes, size or speed, binary or decimal.
        /// </remarks>
        public static string PrintFriendlyAmount(ulong value, string numFmt = null, bool speed = false, bool binary = true, bool displayBinary = false, bool bits = false, int rounding = 2)
        {
            double fs = value;
            double amt = value;

            int i;

            string[] names;

            if (binary && displayBinary)
            {
                if (speed)
                {
                    if (bits)
                    {
                        names = BinSpeedBits;
                    }
                    else
                    {
                        names = BinSpeedBytes;
                    }
                }
                else
                {
                    if (bits)
                    {
                        names = BinSizeBits;
                    }
                    else
                    {
                        names = BinSizeBytes;
                    }
                }
            }
            else
            {
                if (speed)
                {
                    if (bits)
                    {
                        names = DecSpeedBits;
                    }
                    else
                    {
                        names = DecSpeedBytes;
                    }
                }
                else
                {
                    if (bits)
                    {
                        names = DecSizeBits;
                    }
                    else
                    {
                        names = DecSizeBytes;
                    }
                }
            }

            if (binary)
            {
                for (i = 0; i < 6; i++)
                {
                    if (amt >= BinValues[i])
                    {
                        fs = amt / BinValues[i];
                        break;
                    }
                }
            }
            else
            {
                for (i = 0; i < 6; i++)
                {
                    if (amt >= DecValues[i])
                    {
                        fs = amt / DecValues[i];
                        break;
                    }
                }
            }

            if (numFmt != null)
            {
                return Math.Round(fs, rounding).ToString(numFmt) + " " + names[i];
            }
            else
            {
                return Math.Round(fs, rounding) + " " + names[i];
            }
        }

        /// <summary>
        /// Prints a number value as a friendly byte size in TiB, GiB, MiB, KiB or B.
        /// </summary>
        /// <param name="size">The size to format.</param>
        /// <param name="format">Optional numeric format for the resulting value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string PrintFriendlySize(double size, string format = null, bool binary = false, int round = 2)
        {
            return PrintFriendlyAmount((ulong)size, format, false, binary, binary, rounding: 2);
        }

        /// <summary>
        /// Prints a number value as a friendly byte speed in TiB, GiB, MiB, KiB or B.
        /// </summary>
        /// <param name="speed">The speed to format.</param>
        /// <param name="format">Optional numeric format for the resulting value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string PrintFriendlySpeed(ulong speed, string format = null, bool binary = false)
        {
            return PrintFriendlyAmount(speed, format, true, binary);
        }

        /// <summary>
        /// Encode a string for passing in a URL.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string UrlEncode(string input)
        {
            char[] chrs = input.ToCharArray();
            byte[] asc = null;
            string sOut = "";

            int i = 0;
            int c = 0;

            c = chrs.Length - 1;

            for (i = 0; i <= c; i++)
            {
                if (UrlAllowedChars.IndexOf(chrs[i]) != -1)
                {
                    sOut += chrs[i];
                }
                else
                {
                    asc = System.Text.UnicodeEncoding.Unicode.GetBytes("" + chrs[i]);
                    foreach (byte b in asc)
                    {
                        if (b != 0)
                            sOut += "%" + b.ToString("X2");
                    }
                }
            }

            return sOut;
        }

        /// <summary>
        /// Decode a string from a URL.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string UrlDecode(string input)
        {
            if (input.IndexOf("%") == -1)
                return input;

            string[] parse = Split(input, "%", withToken: true, withTokenIn: true);

            StringBuilder asc = new StringBuilder();

            int hv = 0;
            char[] str = null;

            foreach (string lv in parse)
            {
                str = lv.ToCharArray();
                if ((str.Length == 3) && (str[0] == '%') && (IsHexQ("" + str[1], ref hv)))
                {
                    asc.Append((char)hv);
                }
                else
                {
                    asc.Append(str);
                }
            }

            return asc.ToString();
        }

        /// <summary>
        /// Print enumeration or flags descriptions.
        /// </summary>
        /// <typeparam name="T">The Enum type</typeparam>
        /// <param name="val">The value to print</param>
        /// <returns>An enum description or a comma-separated list of flag descriptions.</returns>
        /// <remarks>
        /// This function utilizes <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// </remarks>
        public static string PrintEnumDesc<T>(T val) where T : Enum
        {
            return PrintEnumDesc(typeof(T), val);
        }

        /// <summary>
        /// Print enumeration or flags descriptions.
        /// </summary>
        /// <param name="t">The Enum type</param>
        /// <param name="val">The value to print</param>
        /// <returns>An enum description or a comma-separated list of flag descriptions.</returns>
        /// <remarks>
        /// This function utilizes <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// </remarks>
        public static string PrintEnumDesc(Type t, object val)
        {
            var fs = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            var hfta = t.GetCustomAttribute(typeof(FlagsAttribute));

            bool flags = false;
            if (hfta != null) flags = true;

            if (val is Enum enVal)
            {
                if (flags)
                {
                    var sb = new StringBuilder();
                    var p = false;

                    foreach (var f in fs)
                    {
                        var x = f.GetValue(null);

                        if (x.GetHashCode() == 0) continue;

                        if (enVal.HasFlag((Enum)x))
                        {
                            if (p) sb.Append(", ");
                            p = true;

                            var desc = (DescriptionAttribute)f.GetCustomAttribute(typeof(DescriptionAttribute));
                            if (desc != null)
                            {
                                sb.Append(desc.Description);
                            }
                            else
                            {
                                sb.Append(x.ToString());
                            }
                        }
                    }

                    return sb.ToString();
                }
                else
                {
                    foreach (var f in fs)
                    {
                        var x = f.GetValue(null);

                        if (x.Equals(enVal))
                        {
                            var desc = (DescriptionAttribute)f.GetCustomAttribute(typeof(DescriptionAttribute));
                            if (desc != null)
                            {
                                return desc.Description;
                            }
                            else
                            {
                                return x.ToString();
                            }
                        }
                    }
                }

                return enVal.ToString();
            }

            return val?.ToString();
        }
    }
}