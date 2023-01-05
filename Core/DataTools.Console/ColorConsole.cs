using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DataTools.Console
{
    /// <summary>
    /// Colorful Console Printing Tool
    /// </summary>
    public static class ColorConsole
    {
        private static Dictionary<TableContext, List<int>> tableCache = new Dictionary<TableContext, List<int>>();

        /// <summary>
        /// Gets or sets the global table cache.
        /// </summary>
        public static Dictionary<TableContext, List<int>> TableCache
        {
            get => tableCache;
            set
            {
                tableCache = value;
            }
        }

        /// <summary>
        /// Write text to the console in the specified color, preserving the previous color in the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="color">The color to write the text in.</param>
        public static void WriteColor(string text, ConsoleColor color)
        {
            var oldColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.Write(text);
            System.Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// Write a line to the console in the specified color, preserving the previous color in the console.
        /// </summary>
        /// <param name="text">The line to write.</param>
        /// <param name="color">The color to write the line in.</param>
        public static void WriteColorLine(string text, ConsoleColor color)
        {
            var oldColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// Get or create a table context list.
        /// </summary>
        /// <param name="context">The <see cref="TableContext"/> token.</param>
        /// <returns>A new list for column sizes.</returns>
        public static List<int> GetCreateTableContext(TableContext context)
        {
            if (tableCache.ContainsKey(context))
            {
                return tableCache[context];
            }
            else
            {
                var n = new List<int>();
                tableCache.Add(context, n);

                return n;
            }
        }

        /// <summary>
        /// Auto-generate a table context from the given strings.
        /// </summary>
        /// <param name="columns">The column lengths.</param>
        /// <returns></returns>
        public static List<int> AutoContext(params string[] columns)
        {
            var l = new List<int>();

            foreach (var c in columns)
            {
                l.Add(c.Length + 1);
            }

            return l;
        }

        /// <summary>
        /// Write a fixed row of elements to the console.
        /// </summary>
        /// <param name="context">The <see cref="TableContext"/></param>
        /// <param name="columns">The data</param>
        public static void WriteRow(TableContext context, params string[] columns)
        {
            List<int> cols;
            int c = columns.Length;

            if (!tableCache.ContainsKey(context))
            {
                cols = AutoContext(columns);
            }
            else
            {
                cols = new List<int>(tableCache[context]);

                if (cols.Count < c)
                {
                    var col2 = AutoContext(columns);

                    for (int j = 0; j < cols.Count; j++)
                    {
                        col2[j] = cols[j];
                    }

                    cols = col2;
                }
            }

            for (int i = 0; i < c; i++)
            {
                Write(FixedText(columns[i], cols[i]));
            }
        }

        /// <summary>
        /// Write color markup text to the console.
        /// </summary>
        /// <param name="text"></param>
        public static void Write(string text)
        {
            var stext = new StringBuilder();
            var sb = new StringBuilder();

            int i, c = text.Length;

            var oldColor = System.Console.ForegroundColor;

            for (i = 0; i < c; i++)
            {
                if (text[i] == '{')
                {
                    if (stext.Length != 0)
                    {
                        System.Console.Write(stext.ToString());
                        stext.Clear();
                    }

                    i++;
                    while (text[i] != '}')
                    {
                        sb.Append(text[i++]);
                    }

                    var color = sb.ToString();
                    if (color == "Reset")
                    {
                        System.Console.ResetColor();
                    }
                    else
                    {
                        var val = (ConsoleColor?)typeof(ConsoleColor).GetField(color)?.GetValue(null);

                        if (val is ConsoleColor cc)
                        {
                            System.Console.ForegroundColor = cc;
                        }
                    }
                    sb.Clear();
                }
                else
                {
                    stext.Append(text[i]);
                }
            }

            if (stext.Length > 0) System.Console.Write(stext);
            System.Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// Write a line of color markup text to the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public static void WriteLine(string text)
        {
            Write(text + "\r\n");
        }

        /// <summary>
        /// Write for printing to the console to the edge of the screen buffer into the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to write to.</param>
        /// <param name="text">The text to append.</param>
        public static void WriteToEdge(this StringBuilder sb, string text)
        {
            int c = ColorStrLen(text);
            int d = System.Console.WindowWidth - 2;

            if (c >= d)
            {
                if (c > d) text = text.Substring(0, d);
                sb.Append(text);

                return;
            }

            sb.Append(text);
            sb.Append(new string(' ', d - c));
        }

        /// <summary>
        /// Write a line for printing to the console to the edge of the screen buffer into the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to write to.</param>
        /// <param name="text">The text to append.</param>
        public static void WriteToEdgeLine(this StringBuilder sb, string text)
        {
            int c = ColorStrLen(text);
            int d = System.Console.WindowWidth - 2;

            if (c >= d)
            {
                if (c > d) text = text.Substring(0, d);
                sb.Append(text);

                return;
            }

            sb.Append(text);
            sb.AppendLine(new string(' ', d - c));
        }

        /// <summary>
        /// Gets the actual length of a string, excluding any color markups.
        /// </summary>
        /// <param name="str">The string to get the size for.</param>
        /// <returns></returns>
        public static int ColorStrLen(string str)
        {
            int i, c = str.Length;
            int d = 0;

            for (i = 0; i < c; i++)
            {
                if (str[i] == '{')
                {
                    while (str[i] != '}')
                    {
                        i++;
                    }
                }
                else
                {
                    d++;
                }
            }

            return d;
        }

        /// <summary>
        /// Prints text of a fixed length.
        /// </summary>
        /// <param name="str">The text to print.</param>
        /// <param name="length">The length of the output string.</param>
        /// <param name="truncate">Truncate a long string to the specified length.</param>
        /// <returns>A new string whose length will be <paramref name="length"/>.</returns>
        /// <remarks>
        /// This function works best if you know about how long to expect your strings are going to be, with regularity.<br />
        /// <br />
        /// If the input <paramref name="str"/> is longer than <paramref name="length"/>, and <paramref name="truncate"/> is not set to <see cref="true"/>, then the string will not be truncated, and will be returned unaltered.
        /// </remarks>
        public static string FixedText(string str, int length, bool truncate = false)
        {
            int c = ColorStrLen(str);
            if (c > length)
            {
                if (truncate)
                {
                    return str.Substring(0, length);
                }
                else
                {
                    return str;
                }
            }

            return str + new string(' ', length - c);
        }
    }

    /// <summary>
    /// Token to use when formatting tables.
    /// </summary>
    public struct TableContext : IEquatable<TableContext>, IComparable<TableContext>
    {
        /// <summary>
        /// Represents an invalid table context.
        /// </summary>
        public static readonly TableContext Invalid = new TableContext()
        {
            Guid = Guid.Empty
        };

        private Guid Guid;

        /// <summary>
        /// Create a new <see cref="TableContext"/> GUID token.
        /// </summary>
        /// <returns>A new token.</returns>
        public static TableContext Create()
        {
            return new TableContext()
            {
                Guid = Guid.NewGuid()
            };
        }

#if NET5_0_OR_GREATER
        public override bool Equals([NotNullWhen(true)] object obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is TableContext rc)
            {
                return Equals(rc);
            }
            else if (obj is Guid g)
            {
                return Guid.Equals(g);
            }
            return base.Equals(obj);
        }

        public bool Equals(TableContext obj)
        {
            return obj.Guid == Guid;
        }

        public int CompareTo(TableContext other)
        {
            return Guid.CompareTo(other.Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return Guid.ToString("d");
        }

        public static bool operator ==(TableContext left, TableContext right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TableContext left, TableContext right)
        {
            return !left.Equals(right);
        }
    }
}