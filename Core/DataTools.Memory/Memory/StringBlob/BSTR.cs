using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DataTools.Memory.StringBlob
{
    public class BSTR : IEquatable<BSTR>, IEquatable<string>, IReadOnlyList<char>
    {
        private CoTaskMemPtr mem = null;

        internal IntPtr _ptr
        {
            get => mem?.Handle + 4 ?? IntPtr.Zero;
            set
            {
                if (value - 4 == (mem?.Handle ?? IntPtr.Zero)) return;

                if (mem != null)
                {
                    mem.Dispose();
                }

                mem = new CoTaskMemPtr(value - 4);
            }
        }

        public BSTR()
        {
            mem = new CoTaskMemPtr();
            mem.Alloc(4);
            mem.IntAt(0) = 0;
        }

        public int Length
        {
            get => (int)mem.IntAt(0);
        }

        int IReadOnlyCollection<char>.Count => Length;

        public char this[int index] => mem.CharAt(2 + index);

        public BSTR(string s)
        {
            mem = new CoTaskMemPtr(4 + (s.Length * sizeof(char)));
            mem.SetString(4, s);
            mem.IntAt(0) = s.Length;
        }

        public static BSTR operator +(BSTR lhs, string rhs)
        {
            var b = new BSTR();

            var lhl = lhs.Length;
            var rhl = rhs.Length;

            b.mem = new CoTaskMemPtr(4 + (lhl * 2) + (rhl * 2));
            b.mem.IntAt(0) = lhl + rhl;

            lhs.mem.CopyTo(b.mem, 4, 4, lhl * 2);
            b.mem.SetString(4 + (lhl * 2), rhs, false);

            return b;
        }

        public static bool operator ==(BSTR lhs, BSTR rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(BSTR lhs, BSTR rhs)
        {
            return lhs.Equals(rhs);
        }

        public BSTR(IntPtr m)
        {
            mem = new CoTaskMemPtr(m);
        }

        public unsafe BSTR(void* m)
        {
            mem = new CoTaskMemPtr((IntPtr)m, true);
        }

        public override bool Equals(object obj)
        {
            if (obj is BSTR b) return Equals(b);
            else if (obj is string s) return Equals(s);
            return false;
        }

        public bool Equals(BSTR other)
        {
            if (other == null) return false;
            return other.mem.Equals(mem);
        }

        public bool Equals(string other)
        {
            if (other == null) return false;
            return ToString().Equals(other);
        }

        public override int GetHashCode()
        {
            int i = mem.IntAt(0);
            return (i, ToString()).GetHashCode();
        }

        public override string ToString()
        {
            if (mem == null || mem.Length == 0L) return null;
            var i = mem.IntAt(0);
            if (i <= 0) return null;

            return mem.GetString(sizeof(int), i);
        }

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
        {
            var l = Length + 2;

            for (int i = 2; i < l; i++)
            {
                yield return mem.CharAt(i);
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<char>)this).GetEnumerator();
        }

        public static implicit operator string(BSTR val)
        {
            return val.ToString();
        }

        public static implicit operator BSTR(string val)
        {
            return new BSTR(val);
        }

        public static explicit operator IntPtr(BSTR val) => val.mem.DangerousGetHandle();

        public static explicit operator BSTR(IntPtr val) => new BSTR(val);
    }
}