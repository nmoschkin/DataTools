using DataTools.Graphics;
using DataTools.Graphics.Structs;
using DataTools.Memory;
using DataTools.Memory.StringBlob;
using DataTools.Streams;

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

    public static class Program
    {
        public static void Main(string[] args)
        {
            var booo = new RGBDATA()
            {
                Red = 255,
                Green = 0,
                Blue = 255,
            };

            BSTR bustr = "This is my buster";
            bustr += " Busrrramvvoov";

            Console.WriteLine(bustr);

            foreach (var ch in bustr)
            {
                Console.Write(ch + " ");
            }
            Console.WriteLine();

            var obj1 = new MySampleThing("This is verdna", 7);

            obj1.TheBless = booo;

            var obj2 = new MySampleThing("This is verdna", 7);
            var obj3 = new MySampleThing("This tamburo", 4);
            var obj4 = new MySampleThing("This tamburo", 4);

            object boxed = obj1.TheBless;
            dynamic dyna = boxed;

            string s = boxed.ToString();

            if (dyna is IParseableColorData p)
            {
                dyna = IParseableColorData.Parse(p.GetType(), "RGB(45, 90, 230)");
            }

            RGBDATA u = (RGBDATA)dyna;

            //6Console.WriteLine(s);

            var obarr = new MySampleThing[] { obj1, obj2, obj3, obj4 };

            int x = 0;
            foreach (var ob in obarr)
            {
                Console.WriteLine($"{x++} => {ob}; Hash Code: {(uint)ob.GetHashCode()}, Crc-32: {ob.GetCrc()}");
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                    {
                        Console.WriteLine($"{i}, {j} => {obarr[i].Equals(obarr[j])}");
                    }
                }
            }

            object boxed1 = obj1;
            object boxed2 = obj2;
            object boxed3 = obj3;
            object boxed4 = obj4;

            SafePtr sf = (SafePtr)("CoreTest UNO") + Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            Console.WriteLine("The safe size is: " + sf.Length);
            Console.WriteLine();
            Console.WriteLine("The text stored is:");
            Console.WriteLine(sf.ToString());
            Console.WriteLine();

            Console.WriteLine("The bytes before running zero memory: ");

            var b = (byte[])sf;

            Console.WriteLine($"[{string.Join(", ", b.Select(bb => bb.ToString("X2")))}]");

            Console.WriteLine("Running zero memory...");

            sf.ZeroMemory();

            Console.WriteLine("The bytes after running zero memory: ");

            b = (byte[])sf;

            Console.WriteLine($"[{string.Join(", ", b.Select(bb => bb.ToString("X2")))}]");
        }
    }
}