using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using CoreTestOne.Resources;

using DataTools.Desktop;
using DataTools.Essentials.Broadcasting;
using DataTools.Essentials.Converters.ClassDescriptions;
using DataTools.Essentials.Converters.ClassDescriptions.Framework;
using DataTools.Essentials.Converters.EnumDescriptions.Framework;
using DataTools.Essentials.Observable;
using DataTools.Essentials.Settings;
using DataTools.Graphics;
using DataTools.Streams;
using DataTools.Text;
using DataTools.Win32.Memory;
using DataTools.Windows.Essentials.Settings;
using Newtonsoft.Json;

using SkiaSharp;

using static DataTools.Essentials.SortedLists.BinarySearch;

namespace CoreTestOne
{
    [EnumDescriptionProvider(typeof(DataTools.Essentials.Converters.EnumDescriptions.GlobalizedDescriptionProvider<ExampleEnum>), typeof(AppResources))]
    public enum ExampleEnum
    {
        Gold,
        Silver,
        Blue,
        Green,
        Red,
        Purple
    }

    public class GetButtonEventArgs : EventArgs
    {
        public string ButtonText
        {
            get; set;
        }
    }

    public class SamViewModel : SyncObservableBase
    {
        public event EventHandler<GetButtonEventArgs> GetButtonRequest;

        private string title;

        public SamViewModel() : base(Synchronizer.Default)
        {
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public Task<string> GetButtonText()
        {
            return (sync as Synchronizer).InvokeAsync(() =>
            {
                var s = "";
                var e = new GetButtonEventArgs()
                {
                    ButtonText = s
                };
                GetButtonRequest?.Invoke(this, e);
                return e.ButtonText;
            });
        }
    }

    public class ExternInfo : IEquatable<ExternInfo>
    {
        private static readonly PropertyInfo[] props = typeof(ExternInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        public bool Equals(ExternInfo other)
        {
            var res = true;

            foreach (var prop in props)
            {
                var a = prop.GetValue(this);
                var b = prop.GetValue(other);

                if (a is object && b is object)
                {
                    res = a.Equals(b);
                }
                else if (a is object || b is object)
                {
                    res = false;
                }
            }

            return res;
        }

        public override bool Equals(object obj)
        {
            if (obj is ExternInfo other) return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            var finalhash = -1;

            foreach (var prop in props)
            {
                finalhash ^= prop.GetValue(this)?.GetHashCode() ?? 0;
            }

            return finalhash;
        }

        public string Project
        {
            get; set;
        }

        public string DLLName
        {
            get; set;
        }

        public string EntryPoint
        {
            get; set;
        }

        public string FileName
        {
            get; set;
        }

        public string Namespace
        {
            get; set;
        }

        public string ClassName
        {
            get; set;
        }

        public string FunctionName
        {
            get; set;
        }

        public string Params
        {
            get; set;
        }

        public string ReturnType
        {
            get; set;
        }

        public string FilePath
        {
            get; set;
        }

        public string Visibility
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        public string FullyQualifiedClassName
        {
            get
            {
                if (!string.IsNullOrEmpty(Namespace))
                {
                    return $"{Namespace}.{ClassName}";
                }
                else
                {
                    return $"{ClassName}";
                }
            }
            set
            {
                if (value == null) { return; }
            }
        }

        public string FullyQualifiedName
        {
            get
            {
                if (!string.IsNullOrEmpty(Namespace))
                {
                    return $"{Namespace}.{ClassName}.{FunctionName}";
                }
                else
                {
                    return $"{ClassName}.{FunctionName}";
                }
            }
            set
            {
                if (value == null) { return; }
            }
        }

        public List<string> ReferenceFiles
        {
            get; set;
        }

        public override string ToString()
        {
            return $"{Visibility} {ReturnType} {FunctionName} ({DLLName}.{EntryPoint}) [{FileName}] [Project: {Project}] [References: {ReferenceFiles?.Count ?? 0}]";
        }
    }

    [Globalized(typeof(SampleBaseClass), typeof(AppResources))]
    public class SampleBaseClass : RegistrySerializable, IEquatable<SampleBaseClass>
    {
        public SampleBaseClass() : base("Nathaniel Moschkin", typeof(SampleBaseClass).Assembly)
        {
        }

        public string ValueA
        {
            get; set;
        }

        public int ValueI
        {
            get; set;
        }

        public SampleBaseClass(string valueA, int valueI) : this()
        {
            ValueA = valueA;
            ValueI = valueI;
        }

        public bool Equals(SampleBaseClass other)
        {
            if (other is null) return false;
            return ValueA == other.ValueA && ValueI == other.ValueI;
        }

        public override bool Equals(object obj)
        {
            if (obj is SampleBaseClass m) return Equals(m);
            return false;
        }

        public override int GetHashCode()
        {
            return (ValueA, ValueI).GetHashCode();
        }

        public static bool operator !=(SampleBaseClass a, SampleBaseClass b)
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

        public static bool operator ==(SampleBaseClass a, SampleBaseClass b)
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

        public RGBDATA ColorCode
        {
            get; set;
        }

        public static implicit operator string(SampleBaseClass obj)
        {
            return obj.ValueA;
        }

        public static implicit operator SampleBaseClass(string obj)
        {
            return new SampleBaseClass(obj, 0);
        }

        public static implicit operator SampleBaseClass(int obj)
        {
            return new SampleBaseClass(obj.ToString(), obj);
        }

        public static implicit operator int(SampleBaseClass obj)
        {
            return obj.ValueI;
        }

        public override string ToString()
        {
            return $"{ValueI}: {ValueA}";
        }
    }

    public class DescendantA : SampleBaseClass
    {
        public DescendantA(string valueA, int valueI) : base(valueA, valueI)
        {
            Lanana = DateTime.Now;
            Fanalala = Lanana.TimeOfDay;
        }

        public DateTime Lanana
        {
            get; set;
        }

        public TimeSpan Fanalala
        {
            get; set;
        }
    }

    public class DescendantA1 : DescendantA
    {
        public string SkipNeeds
        {
            get; set;
        }

        public DescendantA1(string skipNeeds, string valueA, int valueI) : base(valueA, valueI)
        {
            SkipNeeds = skipNeeds;
        }
    }

    public class DescendantA2 : DescendantA
    {
        public DescendantA2(short flowers, string valueA, int valueI) : base(valueA, valueI)
        {
            Flowers = flowers;
        }

        public short Flowers
        {
            get; set;
        }

        public DescendantA1 MaybeSkippers { get; set; } = new DescendantA1("Hoyaman", "Singing Loopily", 11);
    }

    public class DescendantB : SampleBaseClass
    {
        public decimal ValueDe
        {
            get; set;
        }

        public DescendantB(decimal valueDe, string valueA, int valueI) : base(valueA, valueI)
        {
            ValueDe = valueDe;
        }

        public override string ToString()
        {
            return ValueDe.ToString() + " @ " + base.ToString();
        }
    }

    public class DescendantB1 : DescendantB
    {
        [TranslationKey("Proptens")]
        public Guid HippieCode { get; set; } = Guid.NewGuid();

        public DescendantB1(decimal valueDe, string valueA, int valueI, Guid? hippieCode = null) : base(valueDe, valueA, valueI)
        {
            HippieCode = hippieCode ?? HippieCode;
        }

        public override string ToString()
        {
            return HippieCode.ToString() + " %% " + base.ToString();
        }
    }

    public class DescendantB1A : DescendantB1
    {
        [JsonProperty("staysNoodles")]
        [DescriptionProvider(typeof(CallbackDescriptionProvider<List<string>>), nameof(GrizzlyJones), typeof(DescendantB1A))]
        public List<string> MarketNoodles
        {
            get; set;
        }

        private static string GrizzlyJones(List<string> pappy, string sisterDaughter)
        {
            return string.Join(", ", pappy);
        }

        public DescendantA2 Ables { get; set; } = new DescendantA2(4, "nininin", 995);

        public DescendantB1A(decimal valueDe, string valueA, int valueI, IEnumerable<string> preNoodles = null, Guid? hippieCode = null) : base(valueDe, valueA, valueI, hippieCode)
        {
            MarketNoodles = new List<string>
            {
                valueDe.ToString(),
                valueA.ToString(),
                valueI.ToString(),
                HippieCode.ToString("d")
            };

            if (preNoodles != null) MarketNoodles.AddRange(preNoodles);
        }

        public override string ToString() => $"{string.Join("!", MarketNoodles)} {base.ToString()}";
    }

    public class DescendantB1B : DescendantB1
    {
        public string Balloons
        {
            get; set;
        }

        public DescendantB1B(string balloons, decimal valueDe, string valueA, int valueI, Guid? hippieCode = null) : base(valueDe, valueA, valueI, hippieCode)
        {
        }

        public override string ToString() => $"{Balloons} float: {base.ToString()}";
    }

    public class DescendantB2B : DescendantB2<bool>
    {
        public double ForTe
        {
            get; set;
        }

        public DescendantB2B(bool aardvulnis, decimal valueDe, string valueA, int valueI) : base(aardvulnis, valueDe, valueA, valueI)
        {
            ForTe = (double)valueI / valueA.Length;
        }

        public override string ToString() => $"ForTe {ForTe} @ {base.ToString()}";
    }

    public abstract class DescendantB2<T> : DescendantB
    {
        public T Aardvulnis
        {
            get; set;
        }

        protected DescendantB2(T aardvulnis, decimal valueDe, string valueA, int valueI) : base(valueDe, valueA, valueI)
        {
            Aardvulnis = aardvulnis;
        }

        public override string ToString() => $"{Aardvulnis} :// {base.ToString()}";
    }

    public class DescendantB2A : DescendantB2<MemoryStream>
    {
        public DescendantB2A(decimal valueDe, string valueA, int valueI) : base(new MemoryStream(), valueDe, valueA, valueI)
        {
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

    public class BCast : Broadcaster<byte[]>
    {
        public BCast(InvocationType invocationType = InvocationType.Synchronous) : base(invocationType, ChannelToken.CreateToken("My Dispatch"), "My Dispatch")
        {
        }

        public void PostBytes(byte[] data, params ChannelToken[] channels)
        {
            TransmitData(data, InvocationType, channels);
        }

    }

    public class Listener : ISubscriber<byte[]>
    {
        private byte[] buffer = new byte[65536];

        public long Packets { get; set; } = 0;
        public string ReceiveString { get; set; } = "Broadcast Received: {0}";
        public long Counter { get; set; } = 0;
        public long LongCounter { get; set; } = 0;

        public void ReceiveData(byte[] value, ISideBandData sideBandData)
        {
            Counter += value.Length;
            Packets++;
            LongCounter++;

            Array.Copy(value, buffer, 65536);
        }
        
        void ISubscriber.ReceiveData(object value, ISideBandData sideBandData)
        {
            ReceiveData((byte[])value, sideBandData);
        }
    }


    public static class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [STAThread]
        public static void Main(string[] args)
        {
            //AllocConsole();

            //var bc = new BCast(InvocationType.Synchronous);

            //var tok1 = ChannelToken.CreateToken("Boogie1");
            //var tok2 = ChannelToken.CreateToken("Boogie2");
            //var tok3 = ChannelToken.CreateToken("Boogie3");

            //var tok4 = tok1 ^ tok3;
            //var tok5 = tok4 ^ tok3;

            //var tok8 = ~tok4 ^ tok1;

            //var t8c = tok8.GetHashCode();

            //var lb = tok5 == tok1;

            //var th1 = tok1.GetHashCode();
            //var th5 = tok5.GetHashCode();

            //var lb2 = th1 == th5;

            //var btest1 = new byte[16];

            //btest1[0] = 5;
            //btest1[1] = 5;
            //btest1[2] = 0xa;
            //btest1[7] = 0xc;
            //btest1[13] = 71;
            //btest1[14] = 15;
            //btest1[15] = 55;

            //var tok6 = new ChannelToken(btest1);

            //Console.WriteLine($"{tok6}");

            //var btest2 = new byte[16];
            
            //btest2[0] = 7;
            //btest2[1] = 9;
            //btest2[2] = 0xd;
            //btest2[7] = 0xf;
            //btest2[13] = 23;
            //btest2[14] = 137;
            //btest2[15] = 45;

            //var sf = new DataTools.Memory.SafePtr();
            //sf.FromByteArray(btest1);
            //sf += btest2;

            //var btest3 = sf.ToByteArray();
            //sf.Free();

            //var tok7 = new ChannelToken(btest1);


            //var tv = tok6.CompareTo(tok7);

            //var ls = new Listener();
            //var ls3 = new Listener();
            //var ls2 = new Listener()
            //{
            //    ReceiveString = "Other Listener {0}"
            //};

            //var sub = bc.Subscribe(ls, tok1);
            //var sub2 = bc.Subscribe(ls2, tok2);
            //var sub3 = bc.Subscribe(ls3, tok3);

            //DateTime n = DateTime.Now;

            //var bre = true;
            //long ssz = 0;
            
            //int plep;

            //Console.WriteLine("Initiating...");
            //Console.WriteLine("");
            //var forseed = (int)DateTime.Now.Ticks & 0x7fffffff;

            //var orig = Console.GetCursorPosition().Top;

            //var th = new Thread(async () =>
            //{
            //    var i = 0;
            //    var rnd = new Random(forseed);

            //    var bsize = 65536;// rnd.Next(4 * 1024 * 1024, 8 * 1024 * 1024);
            //    var buff = new byte[bsize];

            //    while (bre)
            //    {
            //        var nn = DateTime.Now;
            //        var tf = nn - n;

            //        plep = rnd.Next(1, 10);

            //        if (tf.Seconds >= 1)
            //        {
            //            forseed ^= i;
            //            rnd = new Random(forseed);

            //            var bps1 = ls.Counter; // / ls.Counter;
            //            var bps2 = ls2.Counter; // / ls2.Counter;
            //            var bps3 = ls3.Counter; // / ls2.Counter;

            //            var pk1 = ls.Packets;
            //            var pk2 = ls2.Packets;
            //            var pk3 = ls3.Packets;

            //            var l1 = ls.LongCounter;
            //            var l2 = ls2.LongCounter;
            //            var l3 = ls3.LongCounter;


            //            n = nn;
            //            ls.Counter = ls2.Counter = ls3.Counter = 0;
            //            ls.Packets = ls2.Packets = ls3.Packets = 0;

            //            ssz = 0;

            //            Console.SetCursorPosition(0, orig);

            //            Console.WriteLine($"Object 1: {new FriendlySpeedLong(bps1),-9} ({pk1,-7:#,##0} Pkts/s) ({l1,-11:#,##0} Packets Total)        ");
            //            Console.WriteLine($"Object 2: {new FriendlySpeedLong(bps2),-9} ({pk2,-7:#,##0} Pkts/s) ({l2,-11:#,##0} Packets Total)        ");
            //            Console.WriteLine($"Object 3: {new FriendlySpeedLong(bps3),-9} ({pk3,-7:#,##0} Pkts/s) ({l3,-11:#,##0} Packets Total)        ");
            //        }

            //        i++;

            //        _ = Task.Run(() =>
            //        {
            //            var by1 = (byte)rnd.Next(0, 255);
            //            var by2 = (byte)rnd.Next(0, 255);

            //            for (var j = 0; j < bsize; j++)
            //            {
            //                by1 ^= by2;
            //                buff[j] = by1;
            //                by2 ^= (byte)(by1 ^ by2 << 1);
            //            }

            //            ssz += bsize;

            //            if ((plep & 8) != 0)
            //            {
            //                if (plep == 8)
            //                {
            //                    bc.PostBytes(buff, tok1);
            //                }
            //                else 
            //                {
            //                    bc.PostBytes(buff, tok2, tok3);
            //                }
            //            }
            //            else
            //            {
            //                if ((plep & 1) != 0)
            //                {
            //                    bc.PostBytes(buff, tok1);
            //                }

            //                if ((plep & 2) != 0)
            //                {
            //                    bc.PostBytes(buff, tok2);
            //                }

            //                if ((plep & 4) != 0)
            //                {
            //                    bc.PostBytes(buff, tok3);
            //                } 
            //            }

            //        });
                    
            //        await Task.Run(() =>
            //        {
            //            for (var n = 0; n < 64; n++)
            //            {
            //                Thread.Sleep(new TimeSpan(0, 0, 0, 0, 0, 500));
            //            }
            //        });
            //    }

            //    Console.WriteLine("");
            //    Console.WriteLine("Terminated");
            //});

            //th.Priority = ThreadPriority.AboveNormal;
            //th.IsBackground = false;
            //th.Start();

            //while(true)
            //{
            //    if (Console.KeyAvailable)
            //    {
            //        break;
            //    }

            //    Thread.Sleep(0);
            //}

            //bre = false;
            //Environment.Exit(0);




            //var b1 = new DescendantB1(5.4949944444M, "Volleyball", 10, Guid.NewGuid());

            //var a1 = new DescendantA1("Hork filled", "smash", 400);

            //var b2a = new DescendantB2A(69M, "Becknus", 5);

            var b1a = new DescendantB1A(45.5912933M, "Glissful Blissoms", 55, new List<string> { "Glances", "Common Noodles", "Weeds" });

            //var allprovs = ClassInfo.GetDescriptions(b1a);

            //var a = new DescendantA("Becknus", 5)
            //{
            //    ColorCode = new RGBDATA()
            //    {
            //        Red = 255,
            //        Green = 255,
            //        Blue = 0
            //    }
            //};

            //var b2b = new DescendantB2B(true, 551.33M, "thirty two men", 5)
            //{
            //    ColorCode = new RGBDATA()
            //    {
            //        Red = 253,
            //        Green = 135,
            //        Blue = 25
            //    }
            //};

            //DataTools.Essentials.Helpers.ObjectMerge.MergeObjects(b2b, b2a);

            //DataTools.Essentials.Helpers.ObjectMerge.MergeObjects(b1, a1);

            //DataTools.Essentials.Helpers.ObjectMerge.MergeObjects(b2a, b1);

            //DataTools.Essentials.Helpers.ObjectMerge.MergeObjects(a, a1);

            var smo = new SettingsMapOptions();
            var bllb = new DescendantB1A(30.00M, "Hiiyp", 934);
            bllb.Load();

            var rmo = SettingsMapOptions.MapSettings(typeof(DescendantB1A), smo);
            bllb.Ables.ValueA = "Bjorn";
            var tbool = SettingsMapOptions.LookupProperty(bllb, "Ables\\MaybeSkippers\\SkipNeeds", out var found, out var fprop);

            if (tbool)
            {
                var springTest = (string)fprop.GetValue(found);
            }

            bllb.Save();

            var frm = new frmExterns();
            System.Windows.Forms.Application.Run(frm);
        }

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

        private static List<string> MakeNS(string ns)
        {
            var sp = ns.Split('.');

            int i, j;
            int c = sp.Length;
            var sb = new StringBuilder();
            var l = new List<string>();

            for (i = 0; i < c; i++)
            {
                sb.Clear();

                for (j = 0; j <= i; j++)
                {
                    if (j > 0) sb.Append('.');
                    sb.Append(sp[j]);
                }

                l.Add(sb.ToString());
            }

            return l;
        }

        private static void ScanFileForExternReferences(string file, List<ExternInfo> nsexterns, List<ExternInfo> fqnexterns)
        {
            Console.Write($"Scanning for references {Path.GetFileName(file)} ... ".PadRight(80) + "\r");

            List<ExternInfo> wex;

            var lines = File.ReadAllLines(file);
            var txt = string.Join("\r\n", lines);

            var usings = new List<string>();
            var statics = new List<string>();
            var rusing = new Regex(@"using ([A-Za-z0-9_.]+);");
            var rstatic = new Regex(@"using static ([A-Za-z0-9_.]+);");
            var rns = new Regex(@"\s*namespace\s+([A-Za-z0-9_.]+).*");
            var rcls = new Regex(@".* class (\w+).*");

            string currNamespace = null;

            int i, c = lines.Length;

            for (i = 0; i < c; i++)
            {
                var line = lines[i];

                var mns = rns.Match(line);

                if (mns.Success)
                {
                    currNamespace = mns.Groups[1].Value;
                    usings.AddRange(MakeNS(currNamespace));
                }
                else
                {
                    var mcls = rcls.Match(line);

                    if (mcls.Success)
                    {
                        string currClass;

                        currClass = mcls.Groups[1].Value;

                        if (currNamespace != null)
                        {
                            statics.Add($"{currNamespace}.{currClass}");
                        }
                    }
                    else
                    {
                        var musing = rusing.Match(line);

                        if (!musing.Success)
                        {
                            var mstatic = rstatic.Match(line);
                            if (mstatic.Success)
                            {
                                statics.Add(mstatic.Groups[1].Value);
                            }
                        }
                        else
                        {
                            usings.Add(musing.Groups[1].Value);
                        }
                    }
                }
            }

            usings = usings.Distinct().ToList();
            usings.Sort();
            statics = statics.Distinct().ToList();
            statics.Sort();
            var fnonly = Path.GetFileName(file);

            wex = nsexterns;
            int wc = wex.Count;

            foreach (var u in usings)
            {
                var idx = Search<List<ExternInfo>, ExternInfo>((b) =>
                {
                    return string.Compare(u, b.Namespace);
                }, wex, out var ei, first: true);

                if (idx == -1)
                {
                    continue;
                }

                while (idx < wc && wex[idx].Namespace == u)
                {
                    var witem = wex[idx];

                    var funcname = $"{witem.ClassName}.{witem.FunctionName}";

                    if (txt.Contains(funcname))
                    {
                        witem.ReferenceFiles ??= new List<string>();
                        witem.ReferenceFiles.Add(file);
                    }

                    idx++;
                }
            }

            wex = fqnexterns;
            wc = wex.Count;

            foreach (var u in statics)
            {
                var idx = Search<List<ExternInfo>, ExternInfo>((b) =>
                {
                    return string.Compare(u, b.FullyQualifiedClassName);
                }, wex, out var ei, first: true);

                if (idx == -1)
                {
                    continue;
                }
                while (idx < wc && wex[idx].Namespace == u)
                {
                    var witem = wex[idx];

                    var funcname = $"{witem.FunctionName}";

                    if (txt.Contains(funcname))
                    {
                        witem.ReferenceFiles ??= new List<string>();
                        witem.ReferenceFiles.Add(file);
                    }

                    idx++;
                }
            }
        }

        private static List<ExternInfo> ScanFileForExterns(string file, string project = null)
        {
            if (!File.Exists(file)) throw new FileNotFoundException(file);
            Console.Write($"Scanning file {Path.GetFileName(file)} ... ".PadRight(80) + "\r");
            var lines = File.ReadAllLines(file);

            var l = new List<ExternInfo>();

            int i, c = lines.Length;
            string classname = null;
            string classvis = "internal";
            string nns = null;
            var currvis = "private";

            var rcls = new Regex(@".* class (\w+).*");
            var rext = new Regex(@"\s*\[DllImport\(""([a-zA-Z0-9.]+)"".*\].*");
            var rdecl = new Regex(@"\s*\S*\s+static\s+extern\s+([A-Za-z0-9_]+)\s+([A-Za-z0-9_]+)(.+)");
            var rtypes = new Regex(@"\((.+)\)\s*(;)?$");
            var rentry = new Regex(@".*EntryPoint\s*\=\s*""(\w+)"".*");
            var rns = new Regex(@"\s*namespace\s+([A-Za-z0-9_.]+).*");

            for (i = 0; i < c; i++)
            {
                if (lines[i].Contains("namespace"))
                {
                    var m = rns.Match(lines[i]);

                    if (m.Success)
                    {
                        nns = m.Groups[1].Value;
                    }
                }
                else if (lines[i].Contains("class "))
                {
                    var m = rcls.Match(lines[i]);

                    if (m.Success)
                    {
                        classname = m.Groups[1].Value;
                        if (lines[i].Contains("public ")) classvis = "public";
                        else if (lines[i].Contains("private ")) classvis = "private";
                        else classvis = "internal";
                    }
                }
                else
                {
                    var m = rext.Match(lines[i]);

                    if (m.Success)
                    {
                        var me = rentry.Match(lines[i]);
                        var ep = me.Success ? me.Groups[1].Value : null;

                        string dname;
                        string dtype;
                        string paramsig = null;

                        Match dm = null;

                        while (i < c - 1)
                        {
                            i++;
                            dm = rdecl.Match(lines[i]);
                            if (dm.Success) break;
                        }

                        if (dm == null || i == c - 1) break;

                        dtype = dm.Groups[1].Value;
                        dname = dm.Groups[2].Value;

                        if (lines[i].Contains("public ") && classvis == "public")
                        {
                            currvis = "public";
                        }
                        else if (classvis != "public")
                        {
                            if (lines[i].Contains("protected internal ")) currvis = "protected internal";
                            else if (lines[i].Contains("internal ")) currvis = "internal";
                            else if (lines[i].Contains("private ")) currvis = "private";
                            else if (lines[i].Contains("protected ")) currvis = "protected";
                        }

                        var mtypes = rtypes.Match(lines[i]);

                        if (mtypes.Success)
                        {
                            paramsig = mtypes.Groups[1].Value;
                        }

                        ep ??= dname;

                        var cn = new ExternInfo()
                        {
                            Namespace = nns,
                            Params = paramsig,
                            ClassName = classname,
                            EntryPoint = ep,
                            DLLName = m.Groups[1].Value?.ToLower(),
                            FunctionName = dname,
                            ReturnType = dtype,
                            FilePath = file,
                            FileName = Path.GetFileName(file),
                            Project = project,
                            Visibility = currvis
                        };

                        if (!cn.DLLName.EndsWith(".dll")) cn.DLLName += ".dll";

                        Console.WriteLine($"\r\nFound Method: {cn}");
                        l.Add(cn);
                    }
                }
            }

            return l;
        }

        private static List<ExternInfo> ScanDir(string dir, bool refscan = false, List<ExternInfo> nsextern = null, List<ExternInfo> fqnextern = null, string project = null)
        {
            if (!Directory.Exists(dir)) throw new FileNotFoundException(dir);

            var files = Directory.GetFiles(dir, "*.cs");
            var dirs = Directory.GetDirectories(dir);
            List<string> modules = null;
            string[] mods = null;

            project = GetProjectName(dir, out mods) ?? project;

            if (mods != null && mods.Length > 0)
            {
                modules = new List<string>(mods);
            }

            List<ExternInfo> results = refscan ? null : new List<ExternInfo>();

            foreach (var file in files)
            {
                if (refscan)
                {
                    ScanFileForExternReferences(file, nsextern, fqnextern);
                }
                else
                {
                    results.AddRange(ScanFileForExterns(file, project));
                }
            }

            foreach (var d in dirs)
            {
                var dname = Path.GetFileName(d);

                if (modules != null && modules.Contains(dname)) continue;
                if (dname == "bin" || dname == "obj" || dname == "nmdt" || dname == "DataTools5" || dname == "DataTools Suite" || dname.StartsWith(".")) continue;

                if (refscan)
                {
                    ScanDir(d, true, nsextern, fqnextern);
                }
                else
                {
                    results.AddRange(ScanDir(d, project: project));
                }
            }

            return results ?? nsextern;
        }

        public static List<ExternInfo> ScanForExterns(string path)
        {
            var root = path ?? @"E:\Projects\Personal Projects\Repos";
            var results = ScanDir(root);
            results = results.Distinct().ToList();

            var nsexterns = new List<ExternInfo>(results);
            var fqnexterns = new List<ExternInfo>(results);

            nsexterns.Sort((a, b) => string.Compare(a.Namespace, b.Namespace));
            fqnexterns.Sort((a, b) => string.Compare(a.FullyQualifiedClassName, b.FullyQualifiedClassName));

            ScanDir(root, true, nsexterns, fqnexterns);

            var groups = results.GroupBy(x => x.DLLName + "_" + x.EntryPoint).ToList();

            var grp = new Dictionary<string, List<ExternInfo>>();

            foreach (var group in groups)
            {
                var l = group.ToList();
                if (l.Count < 2) continue;
                grp.Add(group.Key, l);
            }

            results.Sort((a, b) =>
            {
                int r = 0;

                if (a.ReferenceFiles == null && b.ReferenceFiles != null)
                {
                    return 1;
                }
                else if (a.ReferenceFiles != null && b.ReferenceFiles == null)
                {
                    return -1;
                }
                else if ((a.ReferenceFiles == null && b.ReferenceFiles == null) ||
                    (a.ReferenceFiles != null && b.ReferenceFiles != null && a.ReferenceFiles.Count == b.ReferenceFiles.Count))
                {
                    r = string.Compare(a.DLLName, b.DLLName);

                    if (r == 0)
                    {
                        r = string.Compare(a.FilePath, b.FilePath);

                        if (r == 0)
                        {
                            r = string.Compare(a.EntryPoint, b.EntryPoint);

                            if (r == 0)
                            {
                                r = string.Compare(a.FunctionName, b.FunctionName);

                                if (r == 0)
                                {
                                    r = string.Compare(a.ClassName, b.ClassName);

                                    if (r == 0)
                                    {
                                        r = string.Compare(a.ReturnType, b.ReturnType);
                                        if (r == 0)
                                        {
                                            r = string.Compare(a.Params, b.Params);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (a.ReferenceFiles != null && b.ReferenceFiles != null)
                {
                    if (a.ReferenceFiles.Count > b.ReferenceFiles.Count)
                    {
                        return -1;
                    }
                    else if (a.ReferenceFiles.Count < b.ReferenceFiles.Count)
                    {
                        return 1;
                    }
                }

                return r;
            });
            var orgres = results;
            var dict = new Dictionary<string, List<ExternInfo>>();

            foreach (var res in results)
            {
                if (!dict.TryGetValue(res.DLLName, out var dll))
                {
                    dll = new List<ExternInfo>();
                    dict.Add(res.DLLName, dll);
                }

                dll.Add(res);
            }
            var opt = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };
            var json = JsonConvert.SerializeObject(dict, opt);

            File.WriteAllText(@"E:\extern_res.json", json);

            json = JsonConvert.SerializeObject(grp, opt);

            File.WriteAllText(@"E:\extern_duplicates.json", json);

            results = results.Where(x => x.ReferenceFiles == null || x.ReferenceFiles.Count == 0).ToList();

            json = JsonConvert.SerializeObject(results, opt);

            File.WriteAllText(@"E:\no_known_refs.json", json);

            return orgres;
        }

        public static void TestHeap()
        {
            Console.WriteLine("Creating heap");
            var h = new Heap(0, 1024 * 1024 * 8);

            var obj = h.CreatePtr<SafePtr>();

            Console.WriteLine("Making heap data");
            obj += "This is some text on a very frigid day at the end of december, on christmas, in fact.";

            Console.WriteLine();
            Console.WriteLine($"Process Heap Address: {SafePtr.ProcessHeap:X16}");
            Console.WriteLine($"Heap Address:         {h.DangerousGetHandle():X16}");
            Console.WriteLine($"Data Heap Address:    {obj.CurrentHeap:X16}");
            Console.WriteLine($"Data Address:         {obj.DangerousGetHandle():X16}");

            Console.WriteLine();
            Console.WriteLine(obj.ToString());
            Console.WriteLine();

            Console.WriteLine("Getting heap size...");
            Console.WriteLine($"Data Size:   {obj.Length} bytes");

            var cb = h.GetHeapSize(out var all, out var unall);

            Console.WriteLine($"Total Size:  {cb:#,##0} bytes");
            Console.WriteLine($"Allocated:   {all:#,##0} bytes");
            Console.WriteLine($"Unallocated: {unall:#,##0} bytes");

            Console.WriteLine("Deleting the heap...");

            h.Dispose();

            Console.WriteLine("After deleting...");

            Console.WriteLine($"Data Heap Address:    {obj.CurrentHeap:X16}");
            Console.WriteLine($"Data Address:         {obj.DangerousGetHandle():X16}");
            Console.WriteLine(obj.ToString());
        }

        public static void ColorInvestigation()
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

        private static string GetProjectName(string dir, out string[] modules)
        {
            var fi = Directory.GetFiles(dir, "*.csproj");
            var projname = default(string);
            var regex = new Regex(@"\s*path\s*\=\s*(.+)");
            var modout = new List<string>();

            if (fi != null && fi.Length > 0)
            {
                projname = Path.GetFileNameWithoutExtension(fi[0]);
            }
            else if (File.Exists(Path.Join(dir, ".gitmodules")))
            {
                var modfile = Path.Join(dir, ".gitmodules");
                var lines = File.ReadAllLines(modfile);

                foreach (var line in lines)
                {
                    var m = regex.Match(line);
                    if (m.Success)
                    {
                        modout.Add(m.Groups[1].Value);
                    }
                }
            }

            modules = modout.ToArray();
            return projname;
        }

        public static void DescribedEnumDemo()
        {
            var eval = new DescribedEnum<ExampleEnum>(ExampleEnum.Silver);

            Console.WriteLine(eval.ToString());
            eval = ExampleEnum.Blue;
            Console.WriteLine(eval.ToString());
            eval = ExampleEnum.Red;
            Console.WriteLine(eval.ToString());
            eval = ExampleEnum.Purple;
            Console.WriteLine(eval.ToString());
            eval = ExampleEnum.Green;
            Console.WriteLine(eval.ToString());
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