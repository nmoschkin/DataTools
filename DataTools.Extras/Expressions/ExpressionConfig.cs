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

        public bool IsQuote(char ch)
        {
            return QuoteChar == ch;
        }

        public bool IsQuote(string value, int index)
        {
            if (Prefix != null)
            {
                if ((value.Length - index) > Prefix.Length) return value.IndexOf(QuoteChar, index) == 0;
                else return false;
            }
            else
            {
                return value[0] == QuoteChar;
            }

        }

        public bool IsEscape(char ch)
        {
            return EscapeChar == ch;
        }

        public string GetQuote(string value, int index, out int? startPos, out int? endPos, bool returnInQuotes = false)
        {


            return QuoteFromHere(value, index, out startPos, out endPos, QuoteChar, EscapeChar, returnInQuotes);
        }



    }

    public class ExpressionConfig
    {

        private List<QuoteStringConfig> quotes = new List<QuoteStringConfig>(); 


        public List<QuoteStringConfig> Quotes
        {
            get => Quotes;
        }
        

    }
}
