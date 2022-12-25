using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTools.Win32.Network
{
    public static class IfTable
    {
        public const int ERROR_INSUFFICIENT_BUFFER = 0x7A;

        [DllImport("Iphlpapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetIfTable(
         nint pIfTable,
         ref int pdwSize,
         bool bOrder
       );

        [DllImport("Iphlpapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetIfTable(
         DataTools.Memory.SafePtr pIfTable,
         ref int pdwSize,
         bool bOrder
       );

        [DllImport("Iphlpapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetIfTable(
         LPMIB_IFTABLE pIfTable,
         ref int pdwSize,
         bool bOrder
       );

        public static List<MIB_IFROW> GetIfTable()
        {
            int x, cb = 0;
            var ret = new LPMIB_IFTABLE();

            x = GetIfTable(nint.Zero, ref cb, false);

            if (x != ERROR_INSUFFICIENT_BUFFER) return null;

            if (cb <= 0) return null;

            if (!ret.Alloc(cb)) return null;

            x = GetIfTable(ret, ref cb, false);

            if (x != 0) return null;

            var l = new List<MIB_IFROW>();

            foreach (var inf in ret)
            {
                l.Add(inf);
            }

            ret.Dispose();

            return l;
        }
    }
}