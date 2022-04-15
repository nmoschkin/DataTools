

using DataTools.Extras.Expressions;
using DataTools.Extras.Conversion;
using DataTools.MathTools;
using DataTools.Text;

using Newtonsoft.Json;

using System.IO;
using System.Text.Json.Nodes;

using static DataTools.MathTools.MathLib;
using DataTools.Extras.AdvancedLists;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace TestExtras
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //TestBufferList(args);
            // TestParsing(args);
            TestCParse(args);
        }

        public static void TestBufferList(string[] args)
        {

            var ltest = new RedBlackCollection<string>();
            int strcount = 64;
            var ch = 'A';

            for (int i = 0; i < strcount; i++)
            {
                if (ch > 'Z') break;
                var s = ch;

                ltest.Add(ch.ToString() + ch.ToString());
                ltest.Add(ch.ToString());
                ltest.Add(ch.ToString() + ch.ToString() + ch.ToString());
                ltest.Add(ch.ToString() + ch.ToString() + ch.ToString() + ch.ToString());

                ch++;
            }

            var bb = ltest.Contains("Z");
            ltest.Remove("Z");

            ltest.Add("Z");
            foreach (var item in ltest) 
            {
                if (!ltest.Contains(item)) throw new KeyNotFoundException();
            }

            var lcomp = new List<string>();

            foreach(var s in ltest)
            {
                lcomp.Add(s);
            }

            var carr = ltest.ToArray();

            var itest = new RedBlackCollection<int?>(new NullableValueTypeComparer<int>(true));

            var r = new Random();

            var testcount = 52000;

            var rand = new Random();
            
            for (int x = 0; x < testcount; x++)
            {
                int z = x;
                itest.Add(rand.Next(testcount));

                z = rand.Next(testcount) - x;
                itest.Add(z);

                z = (x + rand.Next(testcount)) / 2;
                itest.Add(z);

                z = x % rand.Next(1, testcount);
                itest.Add(z);
            }

            var icomp = itest.ToArray();

            for (int x = 1; x < icomp.Length; x++)
            {
                if (icomp[x - 1] < icomp[x]) throw new Exception();
            }


        }

        public class Snippet
        {
            public string Content { get; set; }

            public int StartPos { get; set; }   

            public int EndPos { get; set; }

            public int StartLine { get; set; }

            public int EndLine { get; set; }

            public int StartColumn { get; set; }

            public int EndColumn { get; set; }

            public string Type { get; set; }

            public string Name { get; set; }

            public int Level { get; set; }

            public List<Snippet> Children { get; set; }

            public override string ToString()
            {
                return "(" + Type + ") " + (Name ?? Content ?? "");
            }

        }

        public static List<Snippet> TestCParse(string[] args)
        {

            //var dlg = new OpenFileDialog()
            //{
            //    Filter = "C# Files (*.cs)|*.cs",
            //    InitialDirectory = Environment.CurrentDirectory
            //};

            //if (dlg.ShowDialog() != DialogResult.OK)
            //{
            //    return;
            //}

            //var filename = dlg.FileName;

            var filename = "C:\\Users\\theim\\Desktop\\Projects\\Personal Projects\\Repos\\DataTools\\DataTools\\Text\\BOM.cs";


            var chars = File.ReadAllText(filename).ToCharArray();

            int i, j, c = chars.Length;

            List<Snippet> captured = new List<Snippet>();
            Snippet currSnip = null;

            var stack = new Stack<Snippet>();
            var listStack = new Stack<List<Snippet>>();

            StringBuilder sb;

            int startPos = 0, endPos = 0;

            int startLine = 0, endLine = 0;
            int startCol = 0, endCol = 0;

            int column = 0;
            int currLine = 0;

            int currLevel = 0;

            Dictionary<string, Regex> patterns = new Dictionary<string, Regex>();

            patterns.Add("using", new Regex(@"using (.+);"));
            patterns.Add("namespace", new Regex(@"namespace (.+)"));
            patterns.Add("class", new Regex(@".*class (\w+).*")); 
            patterns.Add("interface", new Regex(@".*interface (\w+).*")); 
            patterns.Add("struct", new Regex(@".*struct (\w+).*")); 
            patterns.Add("enum", new Regex(@".*enum (\w+).*")); 
            patterns.Add("record", new Regex(@".*record (\w+).*")); 
            patterns.Add("delegate", new Regex(@".*delegate .+ (\w+).*;")); 
            patterns.Add("event", new Regex(@".*event .+ (\w+).*;")); 
            patterns.Add("method", new Regex(@".* (\w+).*\s*\(.*\)"));
            patterns.Add("property", new Regex(@".+ (\w+)"));

            for (i = 0; i < c; i++)
            {
                if (chars[i] == ';')
                {
                    var lookback = TextTools.OneSpace(new string(chars, startPos, i - startPos + 1).Replace("\r", "").Replace("\n", "").Trim());

                    foreach (var kvp in patterns)
                    {
                        var result = kvp.Value.Match(lookback);

                        if (result.Success)
                        {
                            currSnip = new Snippet();

                            currSnip.StartPos = startPos;
                            currSnip.StartLine = startLine;
                            currSnip.EndPos = i;
                            currSnip.EndLine = currLine;
                            currSnip.Content = new string(chars, startPos, i - startPos + 1);
                            currSnip.Type = kvp.Key;
                            currSnip.Name = result.Groups[1].Value;
                            captured.Add(currSnip);

                            break;
                        }
                    }

                    startPos = i + 1;
                    startLine = currLine;
                }
                else if (chars[i] == '{')
                {
                    ++currLevel;

                    var lookback = TextTools.OneSpace(new string(chars, startPos, i - startPos).Replace("\r", "").Replace("\n", "").Trim());

                    foreach (var kvp in patterns)
                    {
                        var result = kvp.Value.Match(lookback);

                        if (result.Success)
                        {
                            currSnip = new Snippet();

                            currSnip.StartPos = startPos;
                            currSnip.StartLine = startLine;
                            currSnip.Type = kvp.Key;
                            currSnip.Name = result.Groups[1].Value;

                            captured.Add(currSnip);

                            break;
                        }
                    }

                    stack.Push(currSnip);
                    listStack.Push(captured);

                    captured = new List<Snippet>();

                    startPos = i;
                    startLine = currLine;
                }
                else if (chars[i] == '}')
                {
                    --currLevel;

                    if (currSnip != null)
                    {
                        currSnip.EndPos = i;
                        currSnip.EndLine = currLine;
                        currSnip.Children = captured;
                        currSnip.Content = new string(chars, currSnip.StartPos, currSnip.EndPos - currSnip.StartPos + 1);
                    }

                    currSnip = stack.Pop();

                    if (currSnip != null)
                    {
                        currSnip.Children = captured;
                        captured = listStack.Pop();
                    }
                    else
                    {
                        var cc = captured;
                        captured = listStack.Pop();
                        captured.AddRange(cc);
                    }

                    startPos = i + 1;
                }
                else if (chars[i] == '\n')
                {
                    currLine++;
                    column = 0;
                }
                else if ((i < c - 1) && (chars[i] == '/' && chars[i + 1] == '/'))
                {
                    currSnip = new Snippet()
                    {
                        StartColumn = column,
                        StartLine = currLine,
                        StartPos = i,

                    };

                    sb = new StringBuilder();

                    sb.Append(chars[i]);
                    sb.Append(chars[i + 1]);
                        
                    column += 2;
            
                    for (j = i + 2; j < c; j++)
                    {
                        sb.Append(chars[j]);

                        if (chars[j] == '\n')
                        {
                            currSnip.EndColumn = column;
                            currSnip.EndLine = currLine;
                            currSnip.EndPos = j - 1;
                            currSnip.Content = sb.ToString();
                            currSnip.Type = "linecomment";
                                
                            captured.Add(currSnip);

                            currLine++;
                            column = 0;
                            startPos = j + 1;
                            break;
                        }

                        column++;
                    }

                    if (j >= c) break;
                    i = j;
                }
                else if ((i < c - 3) && (chars[i] == '/' && chars[i + 1] == '*'))
                {
                    currSnip = new Snippet()
                    {
                        StartColumn = column,
                        StartLine = currLine,
                        StartPos = i,

                    };

                    sb = new StringBuilder();

                    sb.Append(chars[i]);
                    sb.Append(chars[i + 1]);

                    column += 2;

                    for (j = i + 2; j < c; j++)
                    {
                        sb.Append(chars[j]);

                        if (j < c - 1 && chars[j] == '*' && chars[j + 1] == '/')
                        {
                            sb.Append('/');
                            currSnip.EndColumn = column + 1;
                            currSnip.EndLine = currLine;
                            currSnip.EndPos = j + 1;
                            currSnip.Content = sb.ToString();
                            currSnip.Type = "blockcomment";

                            captured.Add(currSnip);

                            column += 1;
                            startPos = j + 2;

                            break;
                        }
                        else if (chars[j] == '\n')
                        {
                            currLine++;
                            column = 0;

                            continue;
                        }

                        column++;
                    }

                    if (j >= c) break;
                    i = j;
                }
            }

            return captured;
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
            
            var dstr = "5 m^2 = x ft^2";

            var dstr3 = "abs((19 + 2)^u / 6 * 5 / (4 sqrt (sqrt (v * 5))) + (4 - 6 * 10))";
            //var dstr = "0x4A4e33 = {x:#,##0}";
            var dstr2 = "$x km/h = 45 mi/6 [min]";
            ConversionTool.RoundingDigits = 2;
            //var dstr = "1 hr + 2 days = $x [min]";

            //var lstr = "4 sqrt (9)";
            //var ostr = "sqrt(300)";

            Console.WriteLine($"Input Expression:  {dstr3}");


            var zExp3 = new ExpressionSegment(dstr, "");
            var zExp1 = new ExpressionSegment(dstr3, "");
            var zExp2 = new ExpressionSegment(dstr2, "");

            var expList = new List<ExpressionSegment>();

            expList.Add(zExp3);
            expList.Add(zExp1);
            expList.Add(zExp2);

            foreach (var res in expList)
            {




                res.StorageMode = StorageMode.AsDouble;

                var v = 119.74d;
                var u = 2.234d;

                res["v"] = v;
                res["u"] = u;

                var pairs = res.GetValueUnitPairs();
                var b = res.IsSolvable;
                Console.WriteLine($"\r\nExpression:");
                Console.WriteLine($"-------------");
                Console.WriteLine($"Parsed Expression: {res}");
                Console.WriteLine($"Is Solvable:       {b}");
                Console.WriteLine($"Unit Value Pairs:  {pairs?.Count ?? 0}");

                if (!res.HasUnits)
                {

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
                }

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
                    Console.Write($" ({TextTools.Separate(es.Unit.PluralName)})");
                }
                else
                {
                    Console.Write($" ({TextTools.Separate(es.Unit.Name)})");
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
        bool desc = false;
        public NullableValueTypeComparer(bool descending)
        {
            desc = descending;
        }

        public NullableValueTypeComparer()
        {

        }

        public int Compare(T? x, T? y)
        {
            if (x is int a && y is int b)
            {
                if (desc)
                {
                    return -a.CompareTo(b);
                }
                else
                {
                    return a.CompareTo(b);
                }
            }
            else
            {
                if (x is object && !(y is object)) return 1;
                else if (y is object) return -1;
                return 0;
            }
        }
    }

}