using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using static System.Math;
using System.Runtime.InteropServices;
using DataTools.MathTools;
using DataTools.Text;
using static DataTools.Text.TextTools;


namespace DataTools.Extras.Conversion
{
    /// <summary>
    /// General, all-purpose unit conversion class.
    /// </summary>
    /// <remarks></remarks>
    public class MetricTool
    {


        public static readonly string[] Prefixes = new string[] { "", "deci", "centi", "milli", "micro", "nano", "pico", "femto", "atto", "zepto", "yocto", "deca", "hecto", "kilo", "mega", "giga", "tera", "peta", "exa", "zetta", "yotta", "kibi", "mebi", "gibi", "tebi", "pebi", "exbi", "zebi", "yobi" };
        public static readonly string[] ShortPrefixes = new string[] { "", "d", "c", "m", "μ", "n", "p", "f", "a", "z", "y", "da", "h", "k", "M", "G", "T", "P", "E", "Z", "Y", "Ki", "Mi", "Gi", "Ti", "Pi", "Ei", "Zi", "Yi" };
        public static readonly double[] Multipliers = new double[] { Pow(10d, 0d), Pow(10d, -1), Pow(10d, -2), Pow(10d, -3), Pow(10d, -6), Pow(10d, -9), Pow(10d, -12), Pow(10d, -15), Pow(10d, -18), Pow(10d, -21), Pow(10d, -24), Pow(10d, 1d), Pow(10d, 2d), Pow(10d, 3d), Pow(10d, 6d), Pow(10d, 9d), Pow(10d, 12d), Pow(10d, 15d), Pow(10d, 18d), Pow(10d, 21d), Pow(10d, 24d), Pow(2d, 10d), Pow(2d, 20d), Pow(2d, 30d), Pow(2d, 40d), Pow(2d, 50d), Pow(2d, 60d), Pow(2d, 70d), Pow(2d, 80d) };

        private static int roundingDigits = 4;

        private MetricInfo info = new MetricInfo();
        private string convert;
        private bool longFormat = true;
        private MetricInfo[] convIn;
        private MetricInfo[] convOut;
        private UnitCollection myUnits;

        private static UnitCollection units = new UnitCollection();

        [Browsable(true)]
        public static UnitCollection Units
        {
            get
            {
                return units;
            }
        }

        [Browsable(true)]
        public MetricInfo[] ConversionQuery
        {
            get
            {
                return convIn;
            }
            set
            {
                convIn = value;
            }
        }

        [Browsable(true)]
        public MetricInfo[] ConversionResult
        {
            get
            {
                return convOut;
            }
            set
            {
                convOut = value;
            }
        }

        [Browsable(true)]
        public bool LongFormat
        {
            get
            {
                return longFormat;
            }
            set
            {
                longFormat = value;

                MetricInfo[] argInputInfo = null;
                MetricInfo[] argOutputInfo = null;

                Convert(Query, out argInputInfo, out argOutputInfo);

                Format = Format;
            }
        }

        [Browsable(true)]
        public static int RoundingDigits
        {
            get
            {
                return roundingDigits;
            }
            set
            {
                roundingDigits = value;
            }
        }

        [Browsable(true)]
        public UnitCollection AvailableUnits
        {
            get
            {
                if (myUnits is null)
                    return units;
                return myUnits;
            }
            set
            {
                myUnits = value;
            }
        }

        [Browsable(true)]
        public string Format
        {
            get
            {
                if (longFormat == true)
                {
                    return info.Format;
                }
                else
                {
                    return info.ShortFormat;
                }
            }

            set
            {
                Parse(value, ref info);
            }
        }

        public MetricInfo ApplyEquation(MetricUnit u, ref string ErrorText, params double[] vars)
        {
            int i;
            int c;
            int j;

            string s = u.Equation;
            string v;

            MetricInfo objInfo = null;

            c = vars.Length;

            var loopTo = c;

            for (i = 0; i <= loopTo; i++)
            {
                v = "$" + (i + 1);
                j = s.IndexOf(v);
                if (j == -1)
                {
                    ErrorText = "Missing variable " + v + " in equation, or too many parameters passed.";
                    return null;
                }

                while (j != -1)
                    s = s.Replace(v, "" + vars[i]);
            }

            j = s.IndexOf("$");
            if (j != -1)
            {
                ErrorText = "Too few parameters passed.";
                return null;
            }

            var p = new MathExpressionParser();
            if (p.ParseOperations(s) == false)
            {
                ErrorText = p.ErrorText;
                return null;
            }

            s = "" + p.Value + u.Name;
            Parse(s, ref objInfo);
            return objInfo;
        }

        /// <summary>
        /// Discerns whether or not a value is an x per y (or x/y) value and returns the parsed contents.
        /// </summary>
        /// <param name="value">Value string to analyze.</param>
        /// <param name="MustMeasure">Imperitive measurement units array.  The string must match this many units of these exact types to parse correctly.</param>
        /// <returns>Parsed MetricInfo array.</returns>
        /// <remarks></remarks>
        public MetricInfo[] IsPer(string value, MetricInfo[] MustMeasure = null, bool parseMath = true)
        {
            string[] s;
            int i = 0;
            MetricInfo[] m = null;
            bool boo = false;
            string[] perScan;
            if (parseMath)
            {
                perScan = new[] { "per" };
            }
            else
            {
                perScan = new[] { "per", "/" };
            }

            foreach (var pp in perScan)
            {
                if (value.IndexOf(pp) != -1)
                {
                    s = Split(value, pp);
                    m = new MetricInfo[s.Length];
                    var loopTo = s.Length - 1;
                    for (i = 0; i <= loopTo; i++)
                    {
                        m[i] = new MetricInfo();
                        s[i] = s[i].Trim();
                        if (i != 0)
                        {
                            if (double.IsNaN(FVal(s[i]) ?? double.NaN))
                                s[i] = "1 " + s[i];
                        }

                        if (MustMeasure is null)
                        {
                            Parse(s[i], ref m[i]);
                        }
                        else if (i <= MustMeasure.Length - 1)
                            Parse(s[i], ref m[i], MustMeasure[i].Measures);
                    }

                    boo = true;
                }
            }

            if (!boo)
            {
                m = new[] { new MetricInfo() };
                if (MustMeasure is null)
                {
                    Parse(value, ref m[0]);
                }
                else if (i <= MustMeasure.Length - 1)
                    Parse(value, ref m[0], MustMeasure[i].Measures);
            }

            return m;
        }

        public bool ComparePer(MetricInfo[] m1, MetricInfo[] m2)
        {
            int i;
            if (m1.Length != m2.Length)
                return false;
            var loopTo = m1.Length - 1;
            for (i = 0; i <= loopTo; i++)
            {
                if ((m1[i].Measures ?? "") != (m2[i].Measures ?? ""))
                {
                    convert = "Cannot convert between " + Separate(m1[i].Measures) + " and " + Separate(m2[i].Measures) + ".";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get or set the conversion query.
        /// </summary>
        /// <returns></returns>
        public string Query
        {
            get
            {
                return convert;
            }

            set
            {                
                MetricInfo[] argInputInfo = null;
                MetricInfo[] argOutputInfo = null;
                Convert(value, out argInputInfo, out argOutputInfo);
            }
        }

        /// <summary>
        /// Returns the value in the current unit.
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public double Value
        {
            get
            {
                return info.Value;
            }
        }

        /// <summary>
        /// Returns the <see cref="MetricInfo"/> object.
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public MetricInfo Info
        {
            get
            {
                return info;
            }

            set
            {
                info = value;
            }
        }

        public double Convert(MetricInfo inputValue, string outputType, ref MetricInfo outputValue)
        {
            string s = inputValue.BaseValue + inputValue.BaseUnit + "=" + outputType;
            Convert(s, out _, out _);
            outputValue = (MetricInfo)convOut[0].Clone();
            return convOut[0].Value;
        }

        public double Convert(MetricInfo inputValue, MetricUnit outputType, ref MetricInfo outputValue)
        {
            string s = inputValue.BaseValue + inputValue.BaseUnit + "=" + outputType.Name;
            Convert(s, out _, out _);
            outputValue = (MetricInfo)convOut[0].Clone();
            return convOut[0].Value;
        }

        public double Convert(MetricInfo inputValue, MetricUnit outputType)
        {
            string s = inputValue.BaseValue + inputValue.BaseUnit + "=" + outputType.Name;
            Convert(s, out _, out _);
            return convOut[0].Value;
        }

        public double Convert(double inputValue, MetricUnit outputType)
        {
            string s = inputValue + "=" + outputType.Name;
            Convert(s, out _, out _);
            return convOut[0].Value;
        }

        public double Convert(string inputValue, string outputType)
        {
            string s;
            s = inputValue + "=" + outputType;
            Convert(s, out _, out _);
            return convOut[0].Value;
        }

        public double Convert(double inputValue, string inputType, string outputType = "px")
        {
            string s;
            s = "" + inputValue + inputType + "=" + outputType;
            Convert(s, out _, out _);
            return convOut[0].Value;
        }

        /// <summary>
        /// Convert a series of unit representations into another form.
        /// For exmaple: 1 liter = gallons - or - 100 km/h = mi/h.  The answers will appear in the <see cref="MetricTool.Query">Query</see> variable.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="inputInfo">The parsed detailed input value/unit array.</param>
        /// <param name="outputInfo">The detailed output value/unit array.</param>
        /// <param name="parseMath">Specifies that the value string contains a mathematical equation.</param>
        /// <remarks></remarks>
        public void Convert(string value, out MetricInfo[] inputInfo, out MetricInfo[] outputInfo, bool parseMath = true)
        {
            string[] s = null;
            int i;

            double v = 0.0d;
            double[] vv;
            
            inputInfo = outputInfo = null;
                
            MetricInfo[] i1;
            MetricInfo[] i2;

            if (string.IsNullOrEmpty(value))
                return;
            i = value.ToLower().IndexOf("convert ");
            if (i != -1)
            {
                value = value.Substring(i + 8).Trim();
            }

            if (value.IndexOf("=") == -1)
            {
                value = value.Replace(" to ", "=");
                value = value.Replace(" TO ", "=");
                value = value.Replace(" To ", "=");
                value = value.Replace(" is ", "=");
                value = value.Replace(" IS ", "=");
                value = value.Replace(" Is ", "=");
                value = value.Replace(" and ", "=");
                value = value.Replace(" AND ", "=");
                value = value.Replace(" And ", "=");
                value = value.Replace(" what ", "=");
                value = value.Replace(" WHAT ", "=");
                value = value.Replace(" What ", "=");
                value = value.Replace(" how many ", "=");
                value = value.Replace(" how much ", "=");
                value = value.Replace(" HOW MANY ", "=");
                value = value.Replace(" HOW MUCH ", "=");
                value = value.Replace(" How Many ", "=");
                value = value.Replace(" How Much ", "=");
                value = value.Replace("to to", "=");
                value = value.Replace("to to", "=");
            }

            value = OneSpace(value);
            if (value.IndexOf("=") != -1)
            {
                s = Split(value, "=");
            }
            else if (value.IndexOf(",") != -1)
            {
                s = Split(value, ",");
            }

            if (s is null)
                return;
            if (s.Length != 2)
                return;
            s[0] = s[0].Trim();
            if (!IsNumber(s[1]))
                s[1] = "1 " + s[1].Trim();
            i1 = IsPer(s[0], parseMath: parseMath);
            i2 = IsPer(s[1], i1, parseMath);
            if (ComparePer(i1, i2) == false)
                return;

            // If i2.Length >= 2 Then
            // For i = 1 To i2.Length - 1
            // Parse("" & i1(i).Value & i2(i).PluralName, i2(i))
            // Next
            // End If

            if (i1.Length == 1)
            {
                v = i1[0].BaseValue;
                {
                    var withBlock = i2[0].Unit;
                    if ((withBlock.Modifies ?? "") == (i1[0].BaseUnit ?? "") || (withBlock.Name ?? "") == (i1[0].BaseUnit ?? ""))
                    {
                        if ((withBlock.Name ?? "") == (i1[0].BaseUnit ?? ""))
                        {
                            v = i1[0].BaseValue;
                        }
                        else if (withBlock.OffsetFirst)
                        {
                            v /= withBlock.Multiplier;
                            v -= withBlock.Offset;
                        }
                        else
                        {
                            v -= withBlock.Offset;
                            v /= withBlock.Multiplier;
                        }
                    }

                    v /= i2[0].Multiplier;
                }

                v = Round(v, roundingDigits);
                Parse(v + " " + i2[0].Name, ref i2[0], i1[0].Measures, parseMath);
                if (longFormat)
                {
                    convert = "" + i1[0].Format + " is " + i2[0].Format;
                }
                else
                {
                    convert = "" + i1[0].ShortFormat + " is " + i2[0].ShortFormat;
                }
            }
            else
            {
                vv = new double[i1.Length];
                var loopTo = i1.Length - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    vv[i] = i1[i].BaseValue;
                    {
                        var withBlock1 = i2[i].Unit;
                        if ((withBlock1.Modifies ?? "") == (i1[i].BaseUnit ?? "") || (withBlock1.Name ?? "") == (i1[i].BaseUnit ?? ""))
                        {
                            if ((withBlock1.Name ?? "") == (i1[i].BaseUnit ?? ""))
                            {
                                vv[i] = i1[i].BaseValue;
                            }
                            else if (withBlock1.OffsetFirst)
                            {
                                vv[i] /= withBlock1.Multiplier;
                                vv[i] -= withBlock1.Offset;
                            }
                            else
                            {
                                vv[i] -= withBlock1.Offset;
                                vv[i] /= withBlock1.Multiplier;
                            }
                        }

                        vv[i] /= i2[i].Multiplier;
                    }

                    if (Round(vv[i], roundingDigits) != 0d)
                        vv[i] = Round(vv[i], roundingDigits);
                    if (i == 0)
                    {
                        v = vv[i];
                    }
                    else
                    {
                        // i2(i).Value = 1
                        // If (i1(i).Value <> 0) Then vv(i) /= i1(i).Value
                        v /= vv[i];
                    }
                }

                v = Round(v, roundingDigits);
                convert = i1[0].Value.ToString("#,##0.####") + " ";
                var loopTo1 = i1.Length - 1;
                for (i = 0; i <= loopTo1; i++)
                {
                    if (longFormat)
                    {
                        if (i != 0)
                            convert += " per " + i1[i].Value.ToString("#,##0.####") + " ";
                        if (i1[i].Value != 1d)
                        {
                            convert += i1[i].PluralName;
                        }
                        else
                        {
                            convert += i1[i].Name;
                        }
                    }
                    else
                    {
                        if (i != 0)
                            convert += "/";
                        convert += i1[i].ShortName;
                    }
                }

                convert += " is ";
                convert += v.ToString("#,##0.####") + " ";
                i2[0].Value = v;
                var loopTo2 = i2.Length - 1;
                for (i = 0; i <= loopTo2; i++)
                {
                    if (longFormat)
                    {
                        if (i != 0)
                            convert += " per " + i2[i].Value.ToString("#,##0.####") + " ";
                        if (i2[i].Value != 1d)
                        {
                            convert += i2[i].PluralName;
                        }
                        else
                        {
                            convert += i2[i].Name;
                        }
                    }
                    else
                    {
                        if (i != 0)
                            convert += "/";
                        convert += i2[i].ShortName;
                    }
                }
            }

            ConversionQuery = i1;
            ConversionResult = i2;
            inputInfo = (MetricInfo[])i1.Clone();
            outputInfo = (MetricInfo[])i2.Clone();
        }

        public static void WordsTest(string Example)
        {
            string[] s;
            s = Words(Example);
            Console.WriteLine("There are " + s.Length + " words, starting with " + s[0]);
        }

        public static double GetMultiplier(string prefix)
        {
            int i;
            int c;

            c = Prefixes.Length;


            if (prefix.Length <= 2)
            {
                for (i = 0; i < c; i++)
                {
                    if ((prefix ?? "") == (ShortPrefixes[i] ?? ""))
                    {
                        return Multipliers[i];
                    }
                }
            }

            prefix = prefix.ToLower();

            for (i = 0; i < c; i++)
            {
                if ((prefix ?? "") == (Prefixes[i] ?? ""))
                {
                    return Multipliers[i];
                }
            }

            // it's always safe to return 1
            return 1d;
        }

        public static double Parse(string value, ref MetricInfo info, string MustMeasure = "", bool parseMath = true)
        {
            MathExpressionParser mMath = default;
            MetricUnit[] arrUnits;

            bool markFail = false;

            char[] valueChars;

            int charLen;

            string[] arrStr;

            string textVar;

            double retVal;

            string procStr;
            string postProcStr;

            int n1;
            int n2 = 0;

            int i;
            int a, b, c;

            int x;
            int y;

            if (parseMath)
                mMath = new MathExpressionParser();
            
            if (MustMeasure is object)
                MustMeasure = MustMeasure.ToLower();
            
            if (info is null || info.Name is null)
            {
                info = new MetricInfo();
                if (string.IsNullOrEmpty(MustMeasure) == false)
                {
                    Parse(value, ref info, parseMath: parseMath);
                }
            }

            value = value.Trim();
            valueChars = value.ToCharArray();
            charLen = valueChars.Length - 1;

            if (string.IsNullOrEmpty(value))
                return 0d;

            if (parseMath == false)
            {
                while (!IsNumber(valueChars[charLen]))
                {
                    charLen -= 1;
                    if (charLen < 0)
                        break;
                }
            }
            else
            {
                while (!IsNumber(valueChars[charLen]) || (valueChars[charLen] == ')'))
                {
                    charLen -= 1;
                    if (charLen < 0)
                        break;
                }
            }

            if (charLen < 0)
                return default;
            
            procStr = value.Substring(charLen + 1).Trim();
            
            value = value.Substring(0, charLen + 1);
            
            i = 0;

            if (parseMath)
            {
                mMath.ParseOperations(value);
                retVal = mMath.Value;
            }
            else if (IsHex(value, ref i))
            {
                retVal = i;
            }
            else
            {
                retVal = FVal(value) ?? double.NaN;
            }

            if (procStr.Length > 1)
            {
                if (procStr.Substring(procStr.Length - 1) == ".")
                    procStr = procStr.Substring(0, procStr.Length - 1);
            }

            retVal = Round(retVal, roundingDigits);
            postProcStr = procStr.ToLower();
            
            info.Value = retVal;

            if (!string.IsNullOrEmpty(MustMeasure))
            {
                arrUnits = GetUnitsArray(MustMeasure);
            }
            else
            {
                arrUnits = Units.ToArray();
            }

            if (arrUnits is null)
                return double.NaN;
            
            b = arrUnits.Length;
            
            for (n1 = 0; n1 < b; n1++)
            {
                if (string.IsNullOrEmpty(arrUnits[n1].Prefix))
                {
                    arrStr = new string[1];
                    arrStr[0] = "";
                }
                else
                {
                    arrStr = Split(arrUnits[n1].Prefix, ",");
                }

                a = ShortPrefixes.Length;

                for (n2 = 0; n2 < a; n2++)
                {
                    y = arrStr.Length;
                    
                    for (x = 0; x < y; x++)
                    {
                        arrStr[x] = arrStr[x].Trim();

                        textVar = ShortPrefixes[n2] + arrStr[x];

                        if ((procStr ?? "") == (textVar ?? "") | (procStr ?? "") == (textVar + "s" ?? ""))
                        {
                            if (string.IsNullOrEmpty(MustMeasure) | (MustMeasure ?? "") == (arrUnits[n1].Measures.ToLower() ?? ""))
                            {
                                info.ShortName = ShortPrefixes[n2] + arrStr[0];
                                info.Name = Prefixes[n2] + arrUnits[n1].Name;
                            
                                break;
                            }
                            // we found it!
                        }
                    }

                    if (x < y)
                        break;
                }

                if (n2 < ShortPrefixes.Length)
                    break;

                c = Prefixes.Length;

                for (n2 = 0; n2 < c; n2++)
                {
                    textVar = Prefixes[n2] + arrUnits[n1].Name.ToLower();
                    if ((postProcStr ?? "") == (textVar ?? "") || (postProcStr ?? "") == (textVar + "s" ?? ""))
                    {
                        if (string.IsNullOrEmpty(MustMeasure) | (MustMeasure ?? "") == (arrUnits[n1].Measures.ToLower() ?? ""))
                        {
                            info.Name = textVar;
                            info.ShortName = ShortPrefixes[n2] + arrStr[0];
                            break;
                            // we found it!
                        }
                    }

                    textVar = Prefixes[n2] + arrUnits[n1].PluralName.ToLower();
                    if ((postProcStr ?? "") == (textVar ?? "") || (postProcStr ?? "") == (textVar + "s" ?? ""))
                    {
                        if (string.IsNullOrEmpty(MustMeasure) | (MustMeasure ?? "") == (arrUnits[n1].Measures.ToLower() ?? ""))
                        {
                            info.PluralName = textVar;
                            info.ShortName = ShortPrefixes[n2] + arrStr[0];
                            info.Name = Prefixes[n2] + arrUnits[n1].Name.ToLower();
                            break;
                            // we found it!
                        }
                    }
                }

                if (n2 < Prefixes.Length)
                    break;
            }

            if (n1 < Units.Count & n2 < Prefixes.Length)
            {
                info.Unit = arrUnits[n1];
                info.Multiplier = Multipliers[n2];
                info.BaseValue = info.Value * info.Multiplier;
                info.BaseUnit = arrUnits[n1].Name;
                info.Measures = arrUnits[n1].Measures;
                info.Format = "" + info.Value.ToString("#,##0.##") + " ";

                info.Name = TitleCase(info.Name);

                if (info.Value != 1d & !string.IsNullOrEmpty(arrUnits[n1].PluralName))
                {
                    info.Format += TitleCase(Prefixes[n2] + arrUnits[n1].PluralName.ToLower());
                }
                else
                {
                    info.Format += TitleCase(info.Name);
                }

                info.ShortFormat = "" + info.Value.ToString("#,##0.##") + " " + info.ShortName;

                if (!string.IsNullOrEmpty(arrUnits[n1].Modifies))
                {
                    if (arrUnits[n1].OffsetFirst == true)
                    {
                        info.BaseValue += arrUnits[n1].Offset;
                        info.BaseValue *= arrUnits[n1].Multiplier;
                    }
                    else
                    {
                        info.BaseValue *= arrUnits[n1].Multiplier;
                        info.BaseValue += arrUnits[n1].Offset;
                    }

                    info.BaseUnit = arrUnits[n1].Modifies;
                }

                if (!string.IsNullOrEmpty(arrUnits[n1].PluralName))
                {
                    if (string.IsNullOrEmpty(Prefixes[n2]))
                    {
                        info.PluralName = TitleCase(arrUnits[n1].PluralName);
                    }
                    else
                    {
                        info.PluralName = TitleCase(Prefixes[n2] + arrUnits[n1].PluralName.ToLower());
                    }
                }
            }

            if (markFail)
                return double.NaN;
            return info.Value;
        }

        public MetricTool(bool WithOwnUnits = false)
        {
            if (WithOwnUnits)
                myUnits = GetUnits();

            info.BaseValue = 0d;
            info.Multiplier = 1d;
            info.Value = 0d;
        }

        static MetricTool()
        {
            SortCategories(true);
        }

        private static void SortCategories(bool BaseUnitsFirst = false)
        {
            // ' Sorts all categories alphabetically, optionally putting all base units first

            var c1 = new UnitCollection();
            MetricUnit objUnit;
            string[] s1;
            string[] s2;
            int i;
            int c;
            int j;
            int d;
            s1 = GetCategories();
            c = s1.Length - 1;

            // ' each category starts with its base unit, and then modifying units come next alphabetically, by category alphabetically
            if (BaseUnitsFirst == false)
            {
                var loopTo = c;
                for (i = 0; i <= loopTo; i++)
                {
                    s2 = GetUnitNames(s1[i]);
                    d = s2.Length - 1;
                    var loopTo1 = d;
                    for (j = 0; j <= loopTo1; j++)
                    {
                        objUnit = FindUnit(s2[j], s1[i]);
                        if (objUnit.IsBase == true)
                        {
                            c1.Add(objUnit);
                            break;
                        }
                    }

                    d = s2.Length - 1;
                    var loopTo2 = d;
                    for (j = 0; j <= loopTo2; j++)
                    {
                        objUnit = FindUnit(s2[j], s1[i]);
                        if (objUnit.IsBase == false)
                            c1.Add(objUnit);
                    }
                }

                units = c1;
                return;
            }
            else
            {
                // ' this is the alternative way, listing all base units by category alphabetically, first, then all other units by category alphabetically
                var loopTo3 = c;
                for (i = 0; i <= loopTo3; i++)
                {
                    s2 = GetUnitNames(s1[i]);
                    d = s2.Length - 1;
                    var loopTo4 = d;
                    for (j = 0; j <= loopTo4; j++)
                    {
                        objUnit = FindUnit(s2[j], s1[i]);
                        if (objUnit.IsBase == true)
                        {
                            c1.Add(objUnit);
                            break;
                        }
                    }
                }

                var loopTo5 = c;
                for (i = 0; i <= loopTo5; i++)
                {
                    s2 = GetUnitNames(s1[i]);
                    d = s2.Length - 1;
                    var loopTo6 = d;
                    for (j = 0; j <= loopTo6; j++)
                    {
                        objUnit = FindUnit(s2[j], s1[i]);
                        if (objUnit.IsBase == false)
                            c1.Add(objUnit);
                    }
                }

                units = c1;
                return;
            }
        }


        public static void AddUnit(MetricUnit unit)
        {
            units.Add(unit);
        }

        public static MetricUnit CreateUnit(string measures = "", string name = "", string pluralName = "", string prefix = "", string modifies = "", double multiplier = 0.0d, double offset = 0.0d, bool offsetFirst = false, bool isBase = false)
        {
            var b = new MetricUnit();

            b.Measures = measures;
            b.IsBase = isBase;
            b.Name = name;
            b.PluralName = pluralName;
            b.Prefix = prefix;
            b.Modifies = modifies;
            b.Multiplier = multiplier;
            b.Offset = offset;
            b.OffsetFirst = offsetFirst;

            units.Add(b);

            return b;
        }

        public static string[] GetCategories()
        {
            string[] s = null;

            int c = -1;

            foreach (MetricUnit u in units)
            {
                if (c == -1)
                {
                    s = new string[1];
                    s[0] = TitleCase(u.Measures);
                    c = 1;
                }
                else if (s.Contains(TitleCase(u.Measures)) == false)
                {
                    Array.Resize(ref s, c + 1);
                    s[c] = TitleCase(u.Measures);
                    c += 1;
                }
            }

            Array.Sort(s);
            return s;
        }

        [Description("Get all unit names for a category.")]
        public static string[] GetUnitNames(string Category, bool ExcludeBaseUnit = false)
        {
            string[] c = null;
            int n = 0;
            foreach (MetricUnit u in units)
            {
                if ((u.Measures.ToLower() ?? "") == (Category.ToLower() ?? "") & (u.IsBase == false | ExcludeBaseUnit == false))
                {
                    Array.Resize(ref c, n + 1);
                    c[n] = TitleCase(u.Name);
                    n += 1;
                }
            }

            Array.Sort(c);
            return c;
        }

        [Description("Get all base unit names.")]
        public static string[] GetBaseUnitNames()
        {
            string[] c = null;
            int n = 0;
            foreach (MetricUnit u in units)
            {
                if (u.IsBase == true)
                {
                    Array.Resize(ref c, n + 1);
                    c[n] = TitleCase(u.Name);
                    n += 1;
                }
            }

            Array.Sort(c);
            return c;
        }

        public static bool HasCategory(string Category)
        {
            foreach (MetricUnit u in units)
            {
                if ((u.Measures.ToLower() ?? "") == (Category.ToLower() ?? ""))
                    return true;
            }

            return false;
        }

        [Description("Get all base units for all categories.")]
        public static UnitCollection GetBaseUnits()
        {
            var c = new UnitCollection();
            foreach (MetricUnit u in units)
            {
                if (u.IsBase == true)
                    c.Add((MetricUnit)u.Clone());
            }

            return c;
        }

        [Description("Get all units for a category.")]
        public static UnitCollection GetUnits(string Category = "")
        {
            var uc = new UnitCollection();
            MetricUnit[] a;
            int i;
            int c;
            a = units.ToArray();
            c = a.Length - 1;
            Category = TitleCase(Category);
            var loopTo = c;
            for (i = 0; i <= loopTo; i++)
            {
                if ((a[i].Measures ?? "") == (Category ?? "") | string.IsNullOrEmpty(Category))
                {
                    uc.Add((MetricUnit)a[i].Clone());
                }
            }

            return uc;
        }

        [Description("Get all units for a category as an array.")]
        public static MetricUnit[] GetUnitsArray(string Category = "")
        {
            MetricUnit[] a;
            int i;
            int c;
            MetricUnit[] b = null;
            int n = 0;
            a = units.ToArray();
            c = a.Length - 1;
            Category = Category.ToLower();
            var loopTo = c;
            for (i = 0; i <= loopTo; i++)
            {
                if (NoSpace(a[i].Measures.ToLower()) == Category | string.IsNullOrEmpty(Category))
                {
                    Array.Resize(ref b, n + 1);
                    b[n] = (MetricUnit)a[i].Clone();
                    n += 1;
                }
            }

            return b;
        }

        [Description("Get the base unit for a specific category.")]
        public static MetricUnit GetBaseUnit(string Category)
        {
            foreach (MetricUnit u in units)
            {
                if (u.IsBase == true & (u.Measures.ToLower() ?? "") == (Category.ToLower() ?? ""))
                    return (MetricUnit)u.Clone();
            }

            return null;
        }

        [Description("Find an exact unit.")]
        public static MetricUnit FindUnit(string Unit, string MustMeasure = "")
        {
            string[] s;
            int i;
            int c;
            foreach (MetricUnit u in units)
            {
                if ((u.Measures.ToLower() ?? "") == (MustMeasure.ToLower() ?? "") | string.IsNullOrEmpty(MustMeasure))
                {
                    if ((u.Name.ToLower() ?? "") == (Unit.ToLower() ?? ""))
                        return (MetricUnit)u.Clone();
                    if ((u.PluralName.ToLower() ?? "") == (Unit.ToLower() ?? ""))
                        return (MetricUnit)u.Clone();
                    s = Split(u.Prefix, ",");
                    c = s.Length - 1;
                    var loopTo = c;
                    for (i = 0; i <= loopTo; i++)
                    {
                        if ((s[i] ?? "") == (Unit ?? ""))
                            return (MetricUnit)u.Clone();
                    }
                }
            }

            return null;
        }
    }
}
