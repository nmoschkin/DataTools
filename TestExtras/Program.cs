

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
using System.Text.Json;

namespace TestExtras
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //TestBufferList(args);
            // TestParsing(args);
            TestCParse<Marker>(args);
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

            foreach (var s in ltest)
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

        [Flags]
        public enum AccessModifiers
        {
            None = 0x0,
            Private = 0x1,
            Protected = 0x2,
            Internal = 0x4,
            Public = 0x8,
        }

        public class Marker<T> where T : Marker<T>
        {

            public AccessModifiers AccessModifiers { get; set; }

            public bool IsVirtual { get; set; }

            public bool IsAbstract { get; set; }

            public bool IsExtern { get; set; }

            public bool IsReadOnly { get; set; }

            public bool IsOverride { get; set; }

            public bool IsNew { get; set; }

            public bool IsStatic { get; set; }

            public string Namespace { get; set; }

            public string Content { get; set; }

            public string ScanHit { get; set; }

            public string MethodParamsString { get; set; }

            public List<string> MethodParams { get; set; }

            public int StartPos { get; set; }   

            public int EndPos { get; set; }

            public int StartLine { get; set; }

            public int EndLine { get; set; }

            public int StartColumn { get; set; }

            public int EndColumn { get; set; }

            public string Kind { get; set; }

            public string Name { get; set; }

            public string Generics { get; set; }

            public int Level { get; set; }

            public List<T> Markers { get; set; }

            public override string ToString()
            {
                return "(" + Kind + ") " + (Name ?? Content ?? "");
            }

        }

        public class Marker : Marker<Marker>
        {
        }


        public static List<T> TestCParse<T>(string[] args) where T : Marker<T>, new()
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

            var filename = "C:\\Users\\theim\\Desktop\\Projects\\Personal Projects\\Repos\\DataTools\\DataTools.Win32.Memory\\SafePtr.cs";

            var chars = File.ReadAllText(filename).ToCharArray();

            return ParseCSCode<T>(chars);
        }

        public static List<T> ParseCSCode<T>(char[] chars) where T : Marker<T>, new()  
        {

            int i, j, c = chars.Length;

            List<T> markers = new List<T>();
            T currMarker = null;
            var strack = new Stack<string>();
            var stack = new Stack<T>();
            var listStack = new Stack<List<T>>();

            StringBuilder cw = new StringBuilder();

            StringBuilder sb;

            int startPos = 0;
            int scanStartPos = 0;

            int startLine = 0;

            int currLine = 0;

            int currLevel = 0;

            string currNS = "";

            Dictionary<string, Regex> patterns = new Dictionary<string, Regex>();
            
            patterns.Add("Using", new Regex(@"using (.+)\s*;"));
            patterns.Add("Namespace", new Regex(@"namespace (.+)"));
            patterns.Add("This", new Regex(@".*(this)\s*\[.+\].*"));
            patterns.Add("Class", new Regex(@".*class\s+([A-Za-z0-9_@.]+).*"));
            patterns.Add("Interface", new Regex(@".*interface\s+([A-Za-z0-9_@.]+).*")); 
            patterns.Add("Struct", new Regex(@".*struct\s+([A-Za-z0-9_@.]+).*")); 
            patterns.Add("Enum", new Regex(@".*enum\s+([A-Za-z0-9_@.]+).*")); 
            patterns.Add("Record", new Regex(@".*record\s+([A-Za-z0-9_@.]+).*")); 
            patterns.Add("Delegate", new Regex(@".*delegate\s+.+\s+([A-Za-z0-9_@.]+)\(.*\)\s*;")); 
            patterns.Add("Event", new Regex(@".*event\s+.+\s+([A-Za-z0-9_@.]+)\s*"));
            patterns.Add("Const", new Regex(@".*const\s+.+\s+([A-Za-z0-9_@.]+)\s*"));
            patterns.Add("Operator", new Regex(@".*operator\s+(\S+)\(.*\)"));
            patterns.Add("ForLoop", new Regex(@"\s*for\s*\(.*;.*;.*\)"));
            patterns.Add("DoWhile", new Regex(@"\s*while\s*\(.*\)\s*;"));
            patterns.Add("While", new Regex(@"\s*while\s*\(.*\)"));
            patterns.Add("Switch", new Regex(@"\s*switch\s*\(.+\)"));
            patterns.Add("Case", new Regex(@"\s*case\s*\(.+\)\s*:"));
            patterns.Add("UsingBlock", new Regex(@"\s*using\s*\(.*\)"));
            patterns.Add("Lock", new Regex(@"\s*lock\s*\(.*\)"));
            patterns.Add("Unsafe", new Regex(@"\s*unsafe\s*$"));
            patterns.Add("Fixed", new Regex(@"\s*fixed\s*"));
            patterns.Add("ForEach", new Regex(@"\s*foreach\s*\(.*\)"));
            patterns.Add("Do", new Regex(@"\s*do\s*(\(.+\)|$)"));
            patterns.Add("Else", new Regex(@"\s*else\s*.*"));
            patterns.Add("ElseIf", new Regex(@"\s*else if\s*(\(.+\)|$)"));
            patterns.Add("If", new Regex(@"\s*if\s*(\(.+\)|$)"));
            patterns.Add("Get", new Regex(@"\s*get\s*($|\=\>).*"));
            patterns.Add("Set", new Regex(@"\s*set\s*($|\=\>).*"));
            patterns.Add("Add", new Regex(@"\s*add\s*($|\=\>).*"));
            patterns.Add("Remove", new Regex(@"\s*remove\s*($|\=\>).*"));
            patterns.Add("FieldValue", new Regex(@".+\s+([A-Za-z0-9_@.]+)\s*\=.+;$"));
            patterns.Add("Method", new Regex(@".* ([A-Za-z0-9_@.]+).*\s*\(.*\)\s*(;|\=\>|$|\s*where\s*.+:.+)"));
            patterns.Add("EnumValue", new Regex(@"\s*([A-Za-z0-9_@.]+)(\s*=\s*(.+))?[,]?"));
            patterns.Add("Property", new Regex(@".+\s+([A-Za-z0-9_@.]+)\s*($|\=\>).*"));
            patterns.Add("Field", new Regex(@".+\s+([A-Za-z0-9_@.]+)\s*;$"));

            int z = 0;

            Regex genericPatt = new Regex(@".* ([A-Za-z0-9_@.]+)\s*<(.+)>.*");

            bool lwd = false;

            Regex currCons = null;
            Regex currDecons = null;

            string currName = null;
            string currPatt = null;

            Dictionary<string, bool> activas = new Dictionary<string, bool>();

            activas.Add("public", false);
            activas.Add("internal", false);
            activas.Add("protected", false);
            activas.Add("private", true);
            activas.Add("static", false);
            activas.Add("extern", false);
            activas.Add("abstract", false);
            activas.Add("virtual", false);
            activas.Add("override", false);
            activas.Add("new", false);
            activas.Add("readonly", false);

            for (i = 0; i < c; i++)
            {
                if (chars[i] == '\n')
                {
                    currLine++;
                }
                else if (chars[i] == '\'')
                {
                    TextTools.QuoteFromHere(chars, i, ref currLine, out int? spt, out int? ept, quoteChar: '\'', withQuotes: true);
                    i = (int)ept;
                }
                else if (chars[i] == '\"')
                {
                    TextTools.QuoteFromHere(chars, i, ref currLine, out int? spt, out int? ept, withQuotes: true);
                    i = (int)ept;
                }
                else if (chars[i] == ';' || (chars[i] == ',' && currPatt == "Enum"))
                {
                    var lookback = TextTools.OneSpace(new string(chars, scanStartPos, i - scanStartPos + 1).Replace("\r", "").Replace("\n", "").Trim());

                    currMarker = new T
                    {
                        Namespace = currNS,
                        StartPos = startPos,
                        StartLine = startLine,
                        StartColumn = ColumnFromHere(chars, startPos),
                        EndPos = i,
                        EndLine = currLine,
                        EndColumn = ColumnFromHere(chars, i),
                        Content = new string(chars, startPos, i - startPos + 1),
                        ScanHit = lookback
                    };

                    currMarker.IsAbstract = activas["abstract"];
                    currMarker.IsVirtual = activas["virtual"];
                    currMarker.IsStatic = activas["static"];
                    currMarker.IsExtern = activas["extern"];
                    currMarker.IsOverride = activas["override"];
                    currMarker.IsNew = activas["new"];

                    markers.Add(currMarker);
                    ResetActivas(activas);

                    foreach (var kvp in patterns)
                    {
                        var result = kvp.Value.Match(lookback);

                        if (result.Success)
                        {
                            if (kvp.Key == "EnumValue" && currPatt != "Enum") continue;

                            currMarker.Kind = kvp.Key;
                            currMarker.Name = result.Groups[1].Value;

                            break;
                        }
                    }

                    scanStartPos = startPos = i + 1;
                    if (i < c - 1 && chars[i + 1] == '\n')
                    {
                        startLine = currLine + 1;
                    }
                    else
                    {
                        startLine = currLine;
                    }
                }
                else if (chars[i] == '{')
                {
                    ++currLevel;
                    lwd = false;

                    strack.Push(currPatt);

                    var lookback = TextTools.OneSpace(new string(chars, scanStartPos, i - scanStartPos).Replace("\r", "").Replace("\n", "").Trim());
                    Match cons = currCons?.Match(lookback) ?? null;
                    Match ops = patterns["Operator"].Match(lookback);
                    if (cons != null && cons.Success && !ops.Success)
                    {
                        currMarker = new T
                        {
                            StartPos = startPos,
                            Namespace = currNS,
                            StartLine = startLine,
                            StartColumn = ColumnFromHere(chars, startPos),
                            Kind = "Constructor",
                            Name = currName,
                            AccessModifiers = ActivasToAccessModifiers(activas),
                            ScanHit = lookback
                        };

                        currPatt = "Constructor";
                        markers.Add(currMarker);
                    }
                    else
                    {
                        cons = currDecons?.Match(lookback) ?? null;

                        if (cons != null && cons.Success)
                        {
                            currMarker = new T
                            {
                                Namespace = currNS,
                                StartPos = startPos,
                                StartLine = startLine,
                                StartColumn = ColumnFromHere(chars, startPos),
                                Kind = "Destructor",
                                Name = currName,
                                ScanHit = lookback
                            };

                            currPatt = "Destructor";
                            markers.Add(currMarker);
                        }
                        else
                        {
                            foreach (var kvp in patterns)
                            {
                                var result = kvp.Value.Match(lookback);

                                if (result.Success)
                                {
                                    if (kvp.Key == "EnumValue" && currPatt != "Enum") continue;

                                    currMarker = new T
                                    {
                                        Namespace = currNS,
                                        StartPos = startPos,
                                        StartLine = startLine,
                                        StartColumn = ColumnFromHere(chars, startPos),
                                        Kind = kvp.Key,
                                        Name = result.Groups[1].Value,
                                        ScanHit = lookback
                                    };

                                    currPatt = kvp.Key;

                                    if (kvp.Key == "Namespace")
                                    {
                                        currNS = result.Groups[1].Value;
                                    }
                                    else
                                    {
                                        currMarker.AccessModifiers = ActivasToAccessModifiers(activas);

                                        currMarker.IsAbstract = activas["abstract"];
                                        currMarker.IsVirtual = activas["virtual"];
                                        currMarker.IsStatic = activas["static"];
                                        currMarker.IsExtern = activas["extern"];
                                        currMarker.IsOverride = activas["override"];
                                        currMarker.IsNew = activas["new"];


                                        var genScan = genericPatt.Match(lookback);
                                        if (genScan.Success)
                                        {
                                            if (genScan.Groups[1].Value == currMarker.Name)
                                            {
                                                currMarker.Generics = $"<{genScan.Groups[2].Value}>";
                                            }
                                        }
                                    }

                                    markers.Add(currMarker);

                                    if (currPatt == "Class" || currPatt == "Struct" || currPatt == "Record")
                                    {
                                        currName = currMarker.Name;

                                        currCons = new Regex($"^.*{currMarker.Name}\\s*\\(.*\\).*$");
                                        currDecons = new Regex($"^.*\\~{currMarker.Name}\\s*\\(\\)$");
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    ResetActivas(activas);

                    stack.Push(currMarker);
                    listStack.Push(markers);
                    markers = new List<T>();

                    scanStartPos = startPos = i + 1;
                    if (i < c - 1 && chars[i + 1] == '\n')
                    {
                        startLine = currLine + 1;
                    }
                    else
                    {
                        startLine = currLine;
                    }
                }
                else if (chars[i] == '}')
                {
                    if (currPatt == "Enum")
                    {
                        var lookback = TextTools.OneSpace(new string(chars, scanStartPos, i - scanStartPos).Replace("\r", "").Replace("\n", "").Trim());
                        var testEnum = patterns["EnumValue"].Match(lookback);

                        if (testEnum.Success)
                        {
                            currMarker = new T
                            {
                                Namespace = currNS,
                                StartPos = startPos,
                                StartLine = startLine,
                                StartColumn = ColumnFromHere(chars, startPos),
                                EndPos = i - 1,
                                EndLine = currLine,
                                EndColumn = ColumnFromHere(chars, i - 1),
                                Content = new string(chars, startPos, i - startPos),
                                ScanHit = lookback,
                                Name = testEnum.Groups[1].Value
                            };

                            currMarker.Kind = "EnumValue";
                            currMarker.IsAbstract = activas["abstract"];
                            currMarker.IsVirtual = activas["virtual"];
                            currMarker.IsStatic = activas["static"];
                            currMarker.IsExtern = activas["extern"];
                            currMarker.IsOverride = activas["override"];
                            currMarker.IsNew = activas["new"];

                            markers.Add(currMarker);
                            ResetActivas(activas);
                        }
                    }


                    --currLevel;
                    currPatt = strack.Pop();

                    currMarker = stack.Pop();

                    if (currMarker != null)
                    {
                        currMarker.EndPos = i;
                        currMarker.EndLine = currLine;
                        currMarker.EndColumn = ColumnFromHere(chars, i);
                        currMarker.Content = new string(chars, currMarker.StartPos, currMarker.EndPos - currMarker.StartPos + 1);
                    }

                    currMarker.Markers = markers;
                    markers = listStack.Pop();

                    lwd = true;
                    scanStartPos = startPos = i + 1;
                    if (i < c - 1 && chars[i + 1] == '\n')
                    {
                        startLine = currLine + 1;
                    }
                    else
                    {
                        startLine = currLine;
                    }
                    ResetActivas(activas);

                }
                else if ((i < c - 1) && (chars[i] == '/' && chars[i + 1] == '/'))
                {
                    currMarker = new T()
                    {
                        StartColumn = ColumnFromHere(chars, i),
                        StartLine = currLine,
                        StartPos = i,
                        Namespace = currNS
                    };

                    sb = new StringBuilder();

                    sb.Append(chars[i]);
                    sb.Append(chars[i + 1]);
                        
                    bool docs = false;
                    for (j = i + 2; j < c; j++)
                    {
                        if (j == i + 2 && chars[j] == '/')
                        {
                            docs = true;
                        }
                        sb.Append(chars[j]);

                        if (chars[j] == '\n')
                        {
                            currMarker.EndColumn = ColumnFromHere(chars, j - 1);
                            currMarker.EndLine = currLine;
                            currMarker.EndPos = j - 1;
                            currMarker.Content = sb.ToString();
                            currMarker.Kind = docs ? "XMLDoc" : "LineComment";
                                
                            markers.Add(currMarker);

                            currLine++;

                            if (docs)
                            {
                                startPos = scanStartPos = j + 1;
                                startLine = currLine;
                            }
                            break;
                        }

                    }

                    if (j >= c) break;
                    i = j;
                }
                else if ((i < c - 3) && (chars[i] == '/' && chars[i + 1] == '*'))
                {
                    currMarker = new T()
                    {
                        StartColumn = ColumnFromHere(chars, i),
                        StartLine = currLine,
                        StartPos = i,
                        Namespace = currNS

                    };

                    sb = new StringBuilder();

                    sb.Append(chars[i]);
                    sb.Append(chars[i + 1]);

                    for (j = i + 2; j < c; j++)
                    {
                        sb.Append(chars[j]);

                        if (j < c - 1 && chars[j] == '*' && chars[j + 1] == '/')
                        {
                            sb.Append('/');

                            currMarker.EndColumn = ColumnFromHere(chars, j + 1);
                            currMarker.EndLine = currLine;
                            currMarker.EndPos = j + 1;
                            currMarker.Content = sb.ToString();
                            currMarker.Kind = "BlockComment";

                            markers.Add(currMarker);

                            scanStartPos = j + 2;
                            if (i < c - 2 && chars[i + 2] == '\n')
                            {
                                startLine = currLine + 1;
                            }
                            else
                            {
                                startLine = currLine;
                            }
                            break;
                        }
                        else if (chars[j] == '\n')
                        {
                            currLine++;

                            continue;
                        }

                    }

                    if (j >= c) break;
                    i = j;
                }
                else if (char.IsLetter(chars[i]))
                {
                    cw.Append(chars[i]);
                }
                else
                {
                    if (cw.Length > 0)
                    {
                        var cword = cw.ToString();

                        if (activas.ContainsKey(cword))
                        {
                            activas[cword] = true;
                        }
                        cw.Clear();
                    }
                }
            }

            CleanKids<List<T>, T>(markers);

            return markers;
        }

        private static void CleanKids<T, U>(T markers) where T : List<U> where U: Marker<U>, new()
        {
            int c = markers.Count;
            int i;

            for (i = 0; i < c; i++)
            {
                if (markers[i].Markers != null) CleanKids<List<U>, U>(markers[i].Markers);

               
                if (i < c - 1)
                {
                    if (markers[i].Kind == "Do" && markers[i + 1].Kind == "DoWhile")
                    {
                        markers[i].EndPos = markers[i + 1].EndPos;
                        markers[i].EndLine = markers[i + 1].EndLine;
                        markers[i].EndColumn = markers[i + 1].EndColumn;
                        markers[i].Content += markers[i + 1].Content;
                        markers[i].ScanHit += markers[i + 1].ScanHit;
                        markers[i].Kind = "DoWhile";

                        if (markers[i].Markers == null && markers[i + 1].Markers != null)
                        {
                            markers[i].Markers = markers[i + 1].Markers;
                        }
                        else if (markers[i].Markers != null && markers[i + 1].Markers != null)
                        {
                            markers[i].Markers.AddRange(markers[i + 1].Markers);
                        }

                        CleanKids<List<U>, U>(markers[i].Markers);
                        markers.RemoveAt(i + 1);
                        c--;
                    }
                    else if (markers[i].Kind == "XMLDoc" || markers[i].Kind == "LineComment")
                    {
                        int x = i;

                        while (i < c && (markers[i].Kind == "XMLDoc" || markers[i].Kind == "LineComment"))
                        {
                            i++;
                        }

                        if (i < c)
                        {
                            var mknew = new U();

                            mknew.StartPos = markers[x].StartPos;
                            mknew.StartLine = markers[x].StartLine;
                            mknew.StartColumn = markers[x].StartColumn;
                            
                            mknew.Markers = new List<U>();
                            mknew.Content = "";


                            for (int z = x; z <= i; z++)
                            {
                                if (markers[z].Markers != null) CleanKids<List<U>, U>(markers[z].Markers);
                                mknew.Content += markers[z].Content;
                                mknew.Markers.Add(markers[z]);
                            }

                            mknew.EndPos = markers[i].EndPos;
                            mknew.EndLine = markers[i].EndLine;
                            mknew.EndColumn = markers[i].EndColumn;

                            mknew.Kind = markers[i].Kind;
                            mknew.Name = markers[i].Name;
                            mknew.ScanHit = markers[i].ScanHit;
                            mknew.Generics = markers[i].Generics;

                            mknew.AccessModifiers = markers[i].AccessModifiers;
                            mknew.IsAbstract = markers[i].IsAbstract;
                            mknew.IsVirtual = markers[i].IsVirtual;
                            mknew.IsStatic = markers[i].IsStatic;
                            mknew.IsExtern = markers[i].IsExtern;
                            mknew.IsOverride = markers[i].IsOverride;
                            mknew.IsNew = markers[i].IsNew;

                            markers.RemoveRange(x, (i - x) + 1);
                            markers.Insert(x, mknew);
                            c -= (i - x);
                            i = x;
                        }
                    }

                }

                if (i < c)
                {
                    if ((markers[i].Kind == "Method" || markers[i].Kind == "Constructor") && !string.IsNullOrEmpty(markers[i].ScanHit))
                    {
                        var re = new Regex(@".* ([A-Za-z0-9_@.]+)(<(.+)>)?.*\s*\((.*)\)\s+?:?.+?");
                        var re2 = new Regex(@".* ([A-Za-z0-9_@.]+)(<(.+)>)?.*\s*\((.*)\)");

                        var m = re.Match(markers[i].ScanHit);

                        if (m.Success)
                        {
                            markers[i].MethodParamsString = m.Groups[m.Groups.Count - 1].Value;
                            if (!string.IsNullOrEmpty(markers[i].MethodParamsString))
                            {
                                markers[i].MethodParams = new List<string>(TextTools.Split(markers[i].MethodParamsString, ",", trimResults: true));
                            }
                        }
                        else
                        {
                            m = re2.Match(markers[i].ScanHit);

                            if (m.Success)
                            {
                                markers[i].MethodParamsString = m.Groups[m.Groups.Count - 1].Value;
                                if (!string.IsNullOrEmpty(markers[i].MethodParamsString))
                                {
                                    markers[i].MethodParams = new List<string>(TextTools.Split(markers[i].MethodParamsString, ",", trimResults: true));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static AccessModifiers ActivasToAccessModifiers(Dictionary<string, bool> activas)
        {
            var test = new string[] { "public", "private", "internal", "protected" };

            var sb = new StringBuilder();

            foreach (var t in test)
            {
                if (activas[t])
                {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(TextTools.TitleCase(t));
                }

            }

            if (Enum.TryParse<AccessModifiers>(sb.ToString(), out AccessModifiers result))
            {
                return result;
            }

            return AccessModifiers.None;

        }

        public static void ResetActivas(Dictionary<string, bool> activas)
        {
            var keys = activas.Keys;
            foreach (var key in keys)
            {
                activas[key] = false;
            }
        }

        public static void EnsureLevels(List<bool> items, int levels)
        {
            if (levels > items.Count)
            {
                for (int i = items.Count; i < levels; i++)
                {
                    items.Add(false);
                }
            }
        }

        public static int ColumnFromHere(char[] chars, int pos)
        {

            int c = 0;
            int i;

            for (i = pos - 1; i >= 0; i--)
            {

                var ch = chars[i];
                if (ch == '\n') return c;

                c++;
            }

            return pos;
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