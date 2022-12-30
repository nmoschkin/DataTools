// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Printers
//         Windows Printer Api
//
//         Enums are documented in part from the API documentation at MSDN.
//         Other knowledge and references obtained through various sources
//         and all is considered public domain/common knowledge.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using DataTools.Text;
using DataTools.MathTools;
using DataTools.Win32;
using DataTools.Win32.Memory;
using DataTools.Graphics;
using DataTools.MathTools.Polar;

namespace DataTools.Hardware.Printers
{
    
    
    /// <summary>
    /// A collection of printers available to the machine
    /// </summary>
    public class PrinterObjects : ObservableCollection<PrinterObject>
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct PRINTER_INFO_4
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pPrinterName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pServerName;
            public uint Attributes;
        }

        private PrinterObjects()
        {
        }

        private static PrinterObjects _printers;

        static PrinterObjects()
        {
            _printers = new PrinterObjects();
            Refresh();
        }

        /// <summary>
        /// The collection of printers
        /// </summary>
        /// <returns></returns>
        public static PrinterObjects Printers
        {
            get
            {
                return _printers;
            }
        }

        /// <summary>
        /// Refresh the available printers
        /// </summary>
        /// <returns></returns>
        public static bool Refresh()
        {
            var mm = new MemPtr();
            MemPtr ap;
            uint cb = 0U;
            uint rc = 0U;
            var pif = new PRINTER_INFO_4();
            var sp = new List<string>();
            int ps = Marshal.SizeOf(pif);
            string ts;
            PrinterModule.EnumPrinters(PrinterModule.PRINTER_ENUM_NAME, "", 4U, IntPtr.Zero, 0U, ref cb, ref rc);
            if (cb > 0L)
            {
                mm.Alloc(cb);
                ap = mm;
                PrinterModule.EnumPrinters(PrinterModule.PRINTER_ENUM_NAME, "", 4U, mm, cb, ref cb, ref rc);
                cb = 0U;
                for (int u = 1; u <= rc; u++)
                {
                    ts = ap.GetStringIndirect(cb);
                    sp.Add(ts);
                    ap = ap + ps;
                }

                mm.Free();
            }
            else
            {
                return false;
            }

            PrinterObject po = null;
            _printers.Clear();
            foreach (var s in sp)
            {
                try
                {
                    // MsgBox("Attempting to get highly detailed printer information for '" & s & "'")
                    if ((s.Trim() ?? "") != (s ?? ""))
                    {
                        // MsgBox("Printer name has extra space characters after name! '" & s & "'", MsgBoxStyle.Exclamation)
                    }

                    po = PrinterObject.GetPrinterInfoObject(s);
                }
                catch
                {
                    po = null;
                }

                if (po is object)
                {
                    _printers.Add(po);
                    po = null;
                }
                else
                {
                    // MsgBox("For some reason, the printer that the system just reported would not return a useful Object." & vbCrLf & "Printer: " & s)
                }
            }

            return _printers.Count > 0;
        }
    }
}
