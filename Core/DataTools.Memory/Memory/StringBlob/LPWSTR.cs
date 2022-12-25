using System;
using System.Text;

namespace DataTools.Memory.StringBlob
{
    public class LPWSTR
    {
        private CoTaskMemPtr mem = null;

        internal nint _ptr
        {
            get => (mem?.DangerousGetHandle()) ?? nint.Zero;
            set
            {
                if (value == (mem?.DangerousGetHandle() ?? nint.Zero)) return;

                if (mem != null)
                {
                    mem.Dispose();
                }

                mem = new CoTaskMemPtr(value);
            }
        }

        public LPWSTR()
        {
            mem = new CoTaskMemPtr();
        }

        public int Length
        {
            get => (int)mem.Length - sizeof(char);
        }

        public LPWSTR(string s)
        {
            mem = new CoTaskMemPtr((s.Length + 1) * sizeof(char));
            mem.SetString(0, s);
        }

        public LPWSTR(nint m)
        {
            mem = new CoTaskMemPtr(m);
        }

        public unsafe LPWSTR(void* m)
        {
            mem = new CoTaskMemPtr((nint)m);
        }

        public override string ToString()
        {
            return mem?.ToString();
        }

        public static explicit operator string(LPWSTR val)
        {
            return val.mem.ToString();
        }

        public static explicit operator LPWSTR(string val)
        {
            return new LPWSTR(val);
        }
    }
}