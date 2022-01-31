using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static DataTools.Win32.User32;

namespace DataTools.Desktop.Tools
{
    public static class WindowHelper
    {


        public static System.Drawing.Icon GetWindowIcon(IntPtr hwnd, int iconType = 0)
        {
            IntPtr hIcon = SendMessage(hwnd, WM_GETICON, iconType, 96);
            if (hIcon != IntPtr.Zero)
            {
                var ico = (Icon)Icon.FromHandle(hIcon).Clone();
                return ico;
            }

            if (IntPtr.Size == 8)
            {
                hIcon = GetClassLongPtr(hwnd, GCLP_HICON);
            }
            else
            {
                hIcon = GetClassLong(hwnd, GCLP_HICON);
            }

            if (hIcon != IntPtr.Zero)
            {
                var ico = (Icon)Icon.FromHandle(hIcon).Clone();
                return ico;
            }

            return null;

        }

        /// <summary>
        /// Returns an array of all current top-level windows HWND pointers on the current desktop.
        /// </summary>
        /// <returns>Array of HWNDs as IntPtr</returns>
        public static IntPtr[] GetCurrentDesktopWindows()
        {
            return GetDesktopWindows(IntPtr.Zero);
        }

        /// <summary>
        /// Returns an array of all current top-level windows HWND pointers on the current desktop.
        /// </summary>
        /// <returns>Array of HWNDs as IntPtr</returns>
        public static IntPtr[] GetDesktopWindows(IntPtr hDesk)
        {
            var l = new List<IntPtr>();
            EnumDesktopWindows(hDesk, new EnumWindowsProc((hwnd, lParam) =>
            {
                l.Add(hwnd);
                return true;
            }), IntPtr.Zero);
            return l.ToArray();
        }

        public static IEnumerable<Process> GetDesktopProcesses(IntPtr hDesk = default)
        {
            var l = new List<Process>();
            var hwnd = GetDesktopWindows(hDesk);
            int pid = 0;
            foreach (var h in hwnd)
            {
                GetWindowThreadProcessId(h, ref pid);
                l.Add(Process.GetProcessById(pid));
            }

            l.Sort(new Comparison<Process>((x, y) => { try { return string.Compare(x.ProcessName, y.ProcessName); } catch { return 0; } }));
            var t = new List<Process>();
            Process c = null;
            foreach (var p in l)
            {
                if (c is null || c.Id != p.Id)
                {
                    c = p;
                    t.Add(c);
                }
            }

            return t;
        }

        public static Process GetWindowProcess(IntPtr hwnd)
        {
            int pid = 0;
            GetWindowThreadProcessId(hwnd, ref pid);
            return Process.GetProcessById(pid);
        }

        public static string GetWindowName(IntPtr Hwnd)
        {
            // This function gets the name of a window from its handle
            StringBuilder Title = new StringBuilder(256);
            GetWindowText(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }





    }
}
