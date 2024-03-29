﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTools.Win32
{
    /// <summary>
    /// Helper static class to open files using the Windows shell.
    /// </summary>
    public static class ShellExecuteHelper
    {
        /// <summary>
        /// Execute a shell command on a file.
        /// </summary>
        /// <param name="filename">The filename of the shell command to execute on.</param>
        /// <param name="verb">The verb to execute</param>
        /// <param name="dir">Optional name of the folder.  If this is not explicitly provided, then a directory will not be passed to the call.</param>
        /// <remarks>
        /// If <paramref name="dir"/> is not explicitly provided, then a directory will not be passed to the call.
        /// </remarks>
        public static void Execute(string filename, string verb = "open", string dir = null)
        {
            var shex = new SHELLEXECUTEINFO();
            var proc = Process.GetCurrentProcess();

            shex.cbSize = Marshal.SizeOf(shex);
            shex.nShow = User32.SW_SHOW;
            shex.hInstApp = proc.Handle;
            shex.hWnd = proc.MainWindowHandle;
            shex.lpVerb = verb;

            // Set the parsing name exactly this way.
            shex.lpDirectory = dir;

            shex.lpFile = filename;
            shex.fMask = User32.SEE_MASK_ASYNCOK | User32.SEE_MASK_FLAG_DDEWAIT | User32.SEE_MASK_UNICODE;
            User32.ShellExecuteEx(ref shex);
        }
    }
}