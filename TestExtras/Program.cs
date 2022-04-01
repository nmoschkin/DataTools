

using DataTools.Extras.Expressions;
using DataTools.Extras.Conversion;
using DataTools.MathTools;
using DataTools.Text;

using Newtonsoft.Json;

using System.IO;
using System.Text.Json.Nodes;

using static DataTools.MathTools.MathLib;

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TestExtras
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            TestParsing(args);
        }


        public static void TestParsing(string[] args)
        {

            //var u1 = MetricTool.GetUnitByName("Fahrenheit");
            var jcfg = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            var tss = "Hey roberts roberts gemini falcon \"Hot extra sauce\" derecho derecho agnes";

            var tb = TextTools.TextBetween(tss, 0, "roberts", "derecho", out int? is1, out int? is2);



            Console.WriteLine($"\"{tb}\"");

            tss = "geez[[flankly schmidt]]";
            tb = TextTools.TextBetween(tss, 0, "[[", "]]", out is1, out is2);

            Console.WriteLine($"\"{tb}\"");
            tb = TextTools.TextBetween(tss, 0, '[', ']', out is1, out is2);

            Console.WriteLine($"\"{tb}\"");





            //MetricTool.GetBaseValue(39d, u1, out double? baseValue, out MetricUnit baseUnit);
            //if (baseValue != null) 
            //{
            //    var u2 = MetricTool.GetUnitByName("Celsius");
            //    MetricTool.GetDerivedValue((double)baseValue, u2, out double? cTemp);
            //}

            //var dstr = "(10^2) mi / [min] = $x mi/h";
            //var dstr = "floor(29.9)";
            //var dstr = "ceil(19.3)";
            //var dstr = "tanh(v)";

            //var dstr = "max(v, 44 * 0.32, 95 / 5)";
            var dstr = "abs((19 + 2)^u / 6 * 5 / (4 sqrt (sqrt (v * 5))) + (4 - 6 * 10))";
            //var dstr = "0x4A4e33 = {x:#,##0}";
            // var dstr = "45 mi/h = $x km/h";
            ConversionTool.RoundingDigits = 4;
            //var dstr = "1 hr + 2 days = $x [min]";

            //var lstr = "4 sqrt (9)";
            //var ostr = "sqrt(300)";

            Console.WriteLine($"Input Expression:  {dstr}");
           
            var res = new ExpressionSegment(dstr, "");
            res.StorageMode = StorageMode.AsDouble;

            var v = 119.74d;
            var u = 2.234d;

            res["v"] = v;
            res["u"] = u;

            var pairs = res.GetValueUnitPairs();
            var b = res.IsSolvable;
            Console.WriteLine($"Parsed Expression: {res}");
            Console.WriteLine($"Is Solvable:       {b}");
            Console.WriteLine($"Unit Value Pairs:  {pairs?.Count ?? 0}");
            Console.WriteLine();
            Console.WriteLine("Variables:\r\n" + JsonConvert.SerializeObject(res.Variables, jcfg));
            Console.WriteLine();

            var t1 = DateTime.Now;
            var execRes = res.Execute();
            var t2 = DateTime.Now;

            var ts = t2 - t1;
            Console.WriteLine($"Computed Result: {execRes}");
            //Console.WriteLine($"{Math.Tanh(v)}");

            t1 = DateTime.Now;
            var expVal = Math.Abs(Math.Pow(19d + 2d, u) / 6d * 5d / (4d * (Math.Sqrt(Math.Sqrt(v * 5)))) + (4 - 6 * 10));
            t2 = DateTime.Now;

            var ts2 = t2 - t1;

            Console.WriteLine($"Expected Result: {expVal}");
            Console.WriteLine($"Parser Run Time: {ts}");
            Console.WriteLine($"Normal Run Time: {ts2}");
            Console.WriteLine();

            Console.WriteLine("Object Graph:");
            Console.WriteLine();

            PrintExpression(res);


            if (res.HasUnits)
            {
                Console.WriteLine();
                Console.WriteLine("Base Unit Object Graph:");
                Console.WriteLine();

                PrintExpression(res.Clone(true));

            }

            if (res.IsEquation)
            {
                Console.WriteLine();
                Console.WriteLine("Solved Object Graph:");
                Console.WriteLine();

                PrintExpression(res.Solve());

            }

        }

        public static void PrintExpression(ExpressionSegment es, int level = 0, bool[] keepPattern = null)
        {
            string space = "";
            ExpressionSegment pa = null;

            if (keepPattern == null)
            {
                keepPattern = new bool[level];
            }
            int i;
            for (i = 0; i < level; i++)
            {
                space += new string(' ', 4);
                space += (i == level - 1) ? (es.IsComposite ? "+" : "-") : keepPattern[i] ? "|" : "";
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{space}");

            if (es.Position != Position.Expression && level == 1)
            {
                Console.Write($" {es.Position}:");
            }

            Console.Write($" {es.PartType}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($": ");


            Console.ResetColor();

            switch (es.PartType)
            {
                case PartType.Operator:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;

                case PartType.Literal:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case PartType.Variable:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                case PartType.Unit:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;

                default:

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

            }

            Console.Write($"{es}");

            if ((es.PartType == PartType.Unit))
            {
                if (!string.IsNullOrEmpty(es.Unit.PluralName))
                {
                    Console.Write($" ({es.Unit.PluralName})");
                }
                else
                {
                    Console.Write($" ({es.Unit.Name})");
                }
            }
            
            else if ((es.PartType & PartType.Variable) == PartType.Variable && es.Value != null && es[es.Value.ToString()] != null) 
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" (");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(es[es.Value?.ToString()]);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(")");
            }
            
            Console.ResetColor();
            Console.Write("\r\n");
            var litems = es.Components;
            var litem = ((IList<ExpressionSegment>)litems).LastOrDefault();
            i = 0;

            Array.Resize(ref keepPattern, level + 1);

            foreach (var item in litems)
            {
                keepPattern[level] = !object.Equals(litem, item);
                PrintExpression(item, level + 1, keepPattern);                
            }
        }

        public static void TestRandom(string[] args)
        {
            var rnd = new Random();

            bool verbose = true;

            double d;
            string f;

            DateTime d1, d2;
            TimeSpan tsA, tsB;

            int i;

            int mfaster = 0;
            int sfaster = 0;
            
            int notfounds = 0;
            
            TimeSpan? bestA = null, bestB = null, worstA = null, worstB = null;

            f = PrintFraction(0.00909, 4, 20);
            int msd;
            int msdmin = 4;
            int msdmax = 4;

            int vcur = 0;
            int maxden = 25;

            for (msd = msdmin; msd <= msdmax; msd++)
            {
                double prec = Math.Pow(10, -msd);
                int c = (int)Math.Pow(10, msd);
                int vmax = (int)Math.Pow(10, msd - 2);

                TimeSpan[] medianA = new TimeSpan[c];
                TimeSpan[] medianB = new TimeSpan[c];

                Console.CursorVisible = false;

                for (i = 0; i < c; i++)
                {
                    d = Math.Round(rnd.NextDouble(), 5);

                    if (verbose)
                    {
                        Console.WriteLine();

                        Console.WriteLine("------------------------");
                        Console.WriteLine($"Start Number: {d}");
                    }

                    d1 = DateTime.Now;
                    f = PrintFraction(d, msd, maxden);
                    d2 = DateTime.Now;

                    tsB = d2 - d1;
                    medianB[i] = tsB;
                    if (bestB == null || bestB > tsB) bestB = tsB;
                    if (worstB == null || worstB < tsB) worstB = tsB;

                    if (f == "NaN") notfounds++;

                    if (verbose)
                    {
                        Console.WriteLine($"");
                        Console.WriteLine($"My Algorithm");
                        Console.WriteLine($"");
                        Console.WriteLine($"Fraction:     {f}");
                        Console.WriteLine($"Time:         {tsB}");
                    }

                    d1 = DateTime.Now;
                    f = PrintFraction(d, prec);
                    d2 = DateTime.Now;
                    tsA = d2 - d1;
                    medianA[i] = tsA;
                    if (bestA == null || bestA > tsA) bestA = tsA;
                    if (worstA == null || worstA < tsA) worstA = tsA;

                    if (verbose)
                    {
                        Console.WriteLine($"");
                        Console.WriteLine($"Stern-Brocot Algorithm");
                        Console.WriteLine($"");
                        Console.WriteLine($"Fraction:     {f}");
                        Console.WriteLine($"Time:         {tsA}");
                        Console.WriteLine("------------------------");
                    }

                    if (tsA > tsB) ++mfaster;
                    else if (tsA < tsB) ++sfaster;

                    if (!verbose)
                    {
                        ++vcur;
                        if (vcur == vmax)
                        {
                            vcur = 0;
                            Console.Write($"{i + 1:#,##0} of {c:#,##0} Processed....        \r");
                        }
                    }
                }

                Array.Sort(medianA);
                Array.Sort(medianB);

                tsA = medianA[i / 2];
                tsB = medianB[i / 2];

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Count: {i:#,##0}");
                Console.WriteLine();
                Console.WriteLine($"S/B Precision:    {prec} ");
                Console.WriteLine($"S/B Faster:       {sfaster:#,##0} times.");
                Console.WriteLine();
                Console.WriteLine($"M Significant Digits: {msd}");
                Console.WriteLine($"M Max Denominator:    {maxden}");
                Console.WriteLine($"M Faster:             {mfaster:#,##0} times.");
                Console.WriteLine();
                Console.WriteLine($"S/B Times - Best:   {bestA} ");
                Console.WriteLine($"S/B Times - Median: {tsA}");
                Console.WriteLine($"S/B Times - Worst:  {worstA} ");
                Console.WriteLine();
                Console.WriteLine($"M Times - Best:     {bestB} ");
                Console.WriteLine($"M Times - Median:   {tsB}");
                Console.WriteLine($"M Times - Worst:    {worstB} ");
                Console.WriteLine();
                Console.WriteLine();

            }

            Console.CursorVisible = true;

        }


        public static string RandomString(int lenMin = 1, int lenMax = 10)
        {
            if (lenMin < 1 || lenMax < 1) throw new ArgumentOutOfRangeException();

            var rnd = new Random();

            char ch;
            int p;
            var strLen = rnd.Next(lenMin, lenMax);
            
            var sb = new StringBuilder();

            for (int i = 0; i < strLen; i++)
            {
                p = rnd.Next(1, 52);
                if (p > 26)
                {
                    p -= 26;
                    p--;

                    ch = 'A';
                    ch += (char)p;
                }
                else
                {
                    p--;
                    ch = 'a';
                    ch += (char)p;
                }

                sb.Append(ch);
            }

            return sb.ToString();

        }
    }

    public class NullableValueTypeComparer<T> : IComparer<T?> where T: struct, IComparable<T>
    {
        public int Compare(T? x, T? y)
        {
            if (x is int a && y is int b)
            {
                return a.CompareTo(b);
            }
            else
            {
                if (x is object && !(y is object)) return -1;
                else if (y is object) return 1;
                return 0;
            }
        }
    }

}