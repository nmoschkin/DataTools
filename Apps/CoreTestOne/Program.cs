using DataTools.Desktop;
using DataTools.Graphics;
using DataTools.Streams;

using Newtonsoft.Json;

using SkiaSharp;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace CoreTestOne
{
    public class MySampleThing : IEquatable<MySampleThing>
    {
        public string ValueA { get; set; }

        public int ValueI { get; set; }

        public MySampleThing(string valueA, int valueI)
        {
            ValueA = valueA;
            ValueI = valueI;
        }

        public bool Equals(MySampleThing other)
        {
            if (other is null) return false;
            return ValueA == other.ValueA && ValueI == other.ValueI;
        }

        public override bool Equals(object obj)
        {
            if (obj is MySampleThing m) return Equals(m);
            return false;
        }

        public override int GetHashCode()
        {
            return (ValueA, ValueI).GetHashCode();
        }

        public static bool operator !=(MySampleThing a, MySampleThing b)
        {
            if (a is object)
            {
                return !a.Equals(b);
            }
            else if (b is object)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(MySampleThing a, MySampleThing b)
        {
            if (a is object)
            {
                return a.Equals(b);
            }
            else if (b is object)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public uint GetCrc()
        {
            var b = new List<byte>();

            b.AddRange(Encoding.Unicode.GetBytes(ValueA));
            b.AddRange(BitConverter.GetBytes(ValueI));

            return Crc32.Hash(b.ToArray());
        }

        public RGBDATA TheBless { get; set; }

        public static implicit operator string(MySampleThing obj)
        {
            return obj.ValueA;
        }

        public static implicit operator MySampleThing(string obj)
        {
            return new MySampleThing(obj, 0);
        }

        public static implicit operator MySampleThing(int obj)
        {
            return new MySampleThing(obj.ToString(), obj);
        }

        public static implicit operator int(MySampleThing obj)
        {
            return obj.ValueI;
        }

        public override string ToString()
        {
            return $"{ValueI}: {ValueA}";
        }
    }

    public struct RGBQUAD
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;

        public override string ToString()
        {
            return $"BGRA({Blue}, {Green}, {Red}, {Alpha})";
        }
    }

    public class FSConvert : JsonConverter<FoundStruct>
    {
        public override FoundStruct ReadJson(JsonReader reader, Type objectType, FoundStruct existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is string s)
            {
                return FoundStruct.Parse(s);
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, FoundStruct value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    [JsonConverter(typeof(FSConvert))]
    public struct FoundStruct : IEquatable<FoundStruct>, IComparable<FoundStruct>
    {
        public uint Crc;
        public long Size;

        public override string ToString()
        {
            return $"{Crc};{Size}";
        }

        public static FoundStruct Parse(string value)
        {
            var sp = value.Split(";");

            return new FoundStruct()
            {
                Crc = uint.Parse(sp[0]),
                Size = uint.Parse(sp[1])
            };
        }

        public bool Equals(FoundStruct other)
        {
            return Crc == other.Crc && Size == other.Size;
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is FoundStruct fs) return Equals(fs);
            return false;
        }

        public override int GetHashCode()
        {
            return (Crc, Size).GetHashCode();
        }

        public static bool operator ==(FoundStruct a, FoundStruct b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FoundStruct a, FoundStruct b)
        {
            return !a.Equals(b);
        }

        public int CompareTo(FoundStruct other)
        {
            var r = Crc.CompareTo(other.Crc);

            if (r == 0)
            {
                r = Size.CompareTo(other.Size);
            }

            return r;
        }
    }

    public static class Program
    {
        private static Dictionary<FoundStruct, List<string>> MatchedFiles = new Dictionary<FoundStruct, List<string>>();
        private static int max = 100;

        private static void ScanDir(DirectoryObject dir, List<string> fileTypes, List<string> skipDirs)
        {
            foreach (var obj in dir)
            {
                if (obj is FileObject fobj)
                {
                    if (fobj.IsSpecial) continue;

                    if (!fileTypes.Contains(System.IO.Path.GetExtension(fobj.ParsingName.ToLower()))) continue;

                    if (fobj.TryHashCrc32(out var crc))
                    {
                        var fs = new FoundStruct()
                        {
                            Crc = crc,
                            Size = fobj.Size
                        };

                        if (!MatchedFiles.TryGetValue(fs, out var l))
                        {
                            l = new List<string>();
                            MatchedFiles.Add(fs, l);
                        }

                        l.Add(fobj.ParsingName);
                        if (l.Count > 1)
                        {
                            var pdr = fobj.ParsingName;
                            if (pdr.Length > max) pdr = pdr.Substring(0, max - 10) + " ...";

                            Console.WriteLine($"\r\n{System.IO.Path.GetFileName(pdr)}: {crc:X8} {{{l.Count}}}".PadRight(max));
                        }
                    }
                }
                else if (obj is DirectoryObject dobj)
                {
                    if (dobj.IsSpecial) continue;

                    if (skipDirs.Contains(System.IO.Path.GetFileName(dobj.ParsingName.ToLower()))) continue;
                    var pdr = dobj.ParsingName;
                    if (pdr.Length > max) pdr = pdr.Substring(0, max - 10) + " ...";

                    Console.Write($"Scanning Directory {dobj.ParsingName.PadRight(max)}\r");
                    ScanDir(dobj, fileTypes, skipDirs);
                }
            }
        }

        public static void Main(string[] args)
        {
            byte a = 139;
            byte r = 246;
            byte g = 13;
            byte b = 55;

            byte[] color = { a, r, g, b };
            char[] codes = { 'A', 'R', 'G', 'B' };
            var color1 = System.Drawing.Color.FromArgb(a, r, g, b);
            var color2 = System.Windows.Media.Color.FromArgb(a, r, g, b);
            var color3 = new SKColor(r, g, b, a);

            var blist = new List<byte[]>();
            var nlist = new List<string>();

            BGRADATA test1 = new BGRADATA();
            ARGBDATA test2 = new ARGBDATA();
            RGBQUAD test3 = new RGBQUAD();

            UniColor ce1 = UniColor.FromStruct(color1);
            UniColor ce2 = UniColor.FromStruct(color2);
            UniColor ce3 = UniColor.FromStruct(color3);

            using (var sf = new DataTools.Memory.CoTaskMemPtr())
            {
                sf.FromStruct(color1.ToArgb());
                blist.Add(sf.ToByteArray());
                nlist.Add("System.Drawing.Color.ToArgb()");

                test1 = sf.ToStruct<BGRADATA>();
                test2 = sf.ToStruct<ARGBDATA>();
                test3 = sf.ToStruct<RGBQUAD>();
                sf.FromStruct((uint)color3);
                blist.Add(sf.ToByteArray());
                nlist.Add("(uint)SKColor");

                sf.Length = Marshal.SizeOf<System.Drawing.Color>();
                sf.FromStruct<System.Drawing.Color>(color1);
                blist.Add(sf.ToByteArray());
                nlist.Add("System.Drawing.Color : " + Marshal.SizeOf<System.Drawing.Color>().ToString());

                sf.Length = Marshal.SizeOf<System.Windows.Media.Color>();
                sf.FromStruct<System.Windows.Media.Color>(color2);
                blist.Add(sf.ToByteArray());
                nlist.Add("System.Windows.Media.Color : " + Marshal.SizeOf<System.Windows.Media.Color>().ToString());

                sf.Length = Marshal.SizeOf<SKColor>();
                sf.FromStruct<SKColor>(color3);
                blist.Add(sf.ToByteArray());
                nlist.Add("SKColor : " + Marshal.SizeOf<SKColor>().ToString());
            }

            List<int[]> indexes = new List<int[]>();
            int n = 0;
            foreach (var scan in blist)
            {
                int c = scan.Length;
                int[] idr = new int[4];

                for (int i = 0; i < c; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (scan[i] == color[j])
                        {
                            idr[j] = i;
                        }
                    }
                }

                Console.WriteLine(nlist[n++]);
                Console.WriteLine($"A: {idr[0]} {(idr[0] > idr[1] ? " -> " : " <- ")} R: {idr[1]}  {(idr[1] > idr[2] ? " -> " : " <- ")}  G: {idr[2]}  {(idr[2] > idr[3] ? " -> " : " <- ")}  B: {idr[3]}");
            }

            Console.WriteLine("Test 1: " + test1.ToString());
            Console.WriteLine("Test 2: " + test2.ToString());
            Console.WriteLine("Test 3: " + test3.ToString());

            Console.WriteLine($"Original: A: {a}, R: {r}, G: {g}, B: {b}");
        }

        public static void HuntForDuplicateFiles()
        {
            string[] driveList = new string[] { "D:\\", "E:\\", "C:\\Users\\theim\\OneDrive" };

            var fileTypes = new List<string>(new string[] { ".psd", ".xcf", ".ttf", ".exe", ".dll", ".cs", ".jpg", ".png", ".csproj", ".vb", ".py", ".gif", ".pdf", ".zip" });
            var skipDirs = new List<string>(new string[] { ".git", "bin", "obj", "node_modules", "env", "venv", "google", "adobe" });

            Console.WriteLine($"Searching for duplicates in the following drives: [{string.Join(", ", driveList)}]");
            Console.WriteLine();

            Console.WriteLine("Press any key to begin...");
            Console.ReadKey();

            foreach (var drive in driveList)
            {
                var dir = new DirectoryObject(drive, false, true, StandardIcons.Icon16);
                if (dir != null) ScanDir(dir, fileTypes, skipDirs);
            }

            if (MatchedFiles.Count > 0)
            {
                var dict = new SortedDictionary<FoundStruct, List<string>>();

                foreach (var kv in MatchedFiles)
                {
                    if (kv.Value.Count > 1)
                    {
                        dict.Add(kv.Key, kv.Value);
                    }
                }
                if (dict.Count > 0)
                {
                    var fileReport = "E:\\reportRaw.json";
                    var json = JsonConvert.SerializeObject(dict);
                    System.IO.File.WriteAllText(fileReport, json);
                    Console.WriteLine($"Duplicates saved to {fileReport}");
                }
            }

            Console.ReadKey();
            Console.WriteLine("Press any key to exit...");

            //    var booo = new RGBDATA()
            //    {
            //        Red = 255,
            //        Green = 0,
            //        Blue = 255,
            //    };

            //    BSTR bustr = "This is my buster";
            //    bustr += " Busrrramvvoov";

            //    Console.WriteLine(bustr);

            //    foreach (var ch in bustr)
            //    {
            //        Console.Write(ch + " ");
            //    }
            //    Console.WriteLine();

            //    var obj1 = new MySampleThing("This is verdna", 7);

            //    obj1.TheBless = booo;

            //    var obj2 = new MySampleThing("This is verdna", 7);
            //    var obj3 = new MySampleThing("This tamburo", 4);
            //    var obj4 = new MySampleThing("This tamburo", 4);

            //    object boxed = obj1.TheBless;
            //    dynamic dyna = boxed;

            //    string s = boxed.ToString();

            //    if (dyna is IParseableColorData p)
            //    {
            //        dyna = IParseableColorData.Parse(p.GetType(), "RGB(45, 90, 230)");
            //    }

            //    RGBDATA u = (RGBDATA)dyna;

            //    //6Console.WriteLine(s);

            //    var obarr = new MySampleThing[] { obj1, obj2, obj3, obj4 };

            //    int x = 0;
            //    foreach (var ob in obarr)
            //    {
            //        Console.WriteLine($"{x++} => {ob}; Hash Code: {(uint)ob.GetHashCode()}, Crc-32: {ob.GetCrc()}");
            //    }

            //    for (int i = 0; i < 4; i++)
            //    {
            //        for (int j = 0; j < 4; j++)
            //        {
            //            if (i != j)
            //            {
            //                Console.WriteLine($"{i}, {j} => {obarr[i].Equals(obarr[j])}");
            //            }
            //        }
            //    }

            //    object boxed1 = obj1;
            //    object boxed2 = obj2;
            //    object boxed3 = obj3;
            //    object boxed4 = obj4;

            //    SafePtr sf = (SafePtr)("CoreTest UNO") + Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            //    Console.WriteLine("The safe size is: " + sf.Length);
            //    Console.WriteLine();
            //    Console.WriteLine("The text stored is:");
            //    Console.WriteLine(sf.ToString());
            //    Console.WriteLine();

            //    Console.WriteLine("The bytes before running zero memory: ");

            //    var b = (byte[])sf;

            //    Console.WriteLine($"[{string.Join(", ", b.Select(bb => bb.ToString("X2")))}]");

            //    Console.WriteLine("Running zero memory...");

            //    sf.ZeroMemory();

            //    Console.WriteLine("The bytes after running zero memory: ");

            //    b = (byte[])sf;

            //    Console.WriteLine($"[{string.Join(", ", b.Select(bb => bb.ToString("X2")))}]");
        }
    }
}