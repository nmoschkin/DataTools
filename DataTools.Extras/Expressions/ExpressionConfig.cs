using System;
using System.Collections.Generic;
using System.Text;

using static DataTools.Text.TextTools;

namespace DataTools.Extras.Expressions
{
    public class QuoteStringConfig
    {
        public static readonly QuoteStringConfig SingleQuotes = new QuoteStringConfig('\'', '\\', name: "Single Quotes");

        public static readonly QuoteStringConfig DoubleQuotes = new QuoteStringConfig('"', '\\', name: "Double Quotes");

        public char QuoteChar { get; } = '"';

        public char EscapeChar { get; } = '\\';
        
        public string Prefix { get; } 

        public string Suffix { get; } 

        public int TypeCode { get; }

        public string Name { get; }

        public QuoteStringConfig(char quoteChar, char escChar, string prefix = null, string suffix = null, string name = null, int typeCode = 0)
        {
            QuoteChar = quoteChar;
            EscapeChar = escChar;
            Prefix = prefix ?? string.Empty;
            Suffix = suffix ?? string.Empty;
            TypeCode = typeCode;
            Name = name;
        }

        //public bool IsQuote(char ch)
        //{
        //    return QuoteChar == ch;
        //}

        //public bool IsQuote(string value, int index)
        //{
        //    if (Prefix != null)
        //    {
        //        if ((value.Length - index) > Prefix.Length) return value.IndexOf(QuoteChar, index) == 0;
        //        else return false;
        //    }
        //    else
        //    {
        //        return value[0] == QuoteChar;
        //    }

        //}

        //public bool IsEscape(char ch)
        //{
        //    return EscapeChar == ch;
        //}

        //public string GetQuote(string value, int index, out int? startPos, out int? endPos, bool returnInQuotes = false)
        //{
        //    return QuoteFromHere(value, index, out startPos, out endPos, QuoteChar, EscapeChar, returnInQuotes);
        //}



    }

    public class ExpressionConfig
    {
        private List<QuoteStringConfig> quotes = new List<QuoteStringConfig>();

        private List<string> keywords = new List<string>();

        private List<(string, string)> blockDelimiters = new List<(string, string)>();

        private string expressionTerminator = string.Empty;

        public string ExpressionEnd
        {
            get => expressionTerminator;
            set => expressionTerminator = value;
        }

        public List<(string, string)> BlockDelimiters
        {
            get => blockDelimiters;
            set
            {
                blockDelimiters = value;
            }
        }

        public List<QuoteStringConfig> Quotes
        {
            get => quotes;
            set
            {
                quotes = value;
            }
        }

        public List<string> Keywords
        {
            get => keywords;
            set
            {
                keywords = value;
            }
        }

        public ExpressionConfig(IEnumerable<QuoteStringConfig> quotes, IEnumerable<(string, string)> blocks, IEnumerable<string> keywords, string terminator)
        {
            this.quotes.AddRange(quotes);
            this.keywords.AddRange(keywords);
            this.expressionTerminator = terminator;
            blockDelimiters.AddRange(blocks);
        }

        public ExpressionConfig()
        {
            quotes.Add(QuoteStringConfig.SingleQuotes);
            quotes.Add(QuoteStringConfig.DoubleQuotes);

            blockDelimiters.Add(("{", "}"));

            keywords.AddRange(new[] { "public", "private", "new", "lock", "try", "catch", "finally", "fixed", "unsafe", "using", "static", "internal", "extern", "protected", "abstract", "class", "interface", "struct", "record", "namespace", "sealed" });

        }

    }
}
