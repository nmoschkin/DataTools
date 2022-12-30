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

using DataTools.Graphics;
using DataTools.MathTools.Polar;
using DataTools.Win32;
using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace DataTools.Hardware.Printers
{
    /// <summary>
    /// Encapsulates a printer queue on the system.
    /// </summary>
    /// <remarks></remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PrinterObject : CriticalFinalizerObject, IDisposable, IEquatable<PrinterObject>
    {
        internal MemPtr _ptr;

        // This is a scratch-pad memory pointer for various getting and setting functions
        // so that we don't have to keep allocating and deallocating resources.
        private MemPtr _mm = new MemPtr(16L);

        private DeviceMode _DevMode;

        public bool Equals(PrinterObject other)
        {
            return (PrinterName ?? "") == (other.PrinterName ?? "");
        }

        private PrinterObject()
        {
        }

        internal PrinterObject(IntPtr ptr, bool fOwn)
        {
            _fOwn = fOwn;
            _ptr = ptr;
            MemPtr mm = ptr;
            if (IntPtr.Size == 4)
            {
                _DevMode = new DeviceMode((IntPtr)mm.IntAt(7L), false);
            }
            // MsgBox("Got a pointer! x86 mode.")
            else
            {
                _DevMode = new DeviceMode((IntPtr)mm.LongAt(7L), false);
                // MsgBox("Got a pointer! x64 mode.")
                // If printer._DevMode IsNot Nothing Then MsgBox("DevMode retrieval successful, devmode reports device name as '" & printer._DevMode.DeviceName & "'")
            }
        }

        private bool _fOwn = true;

        public void Dispose()
        {
            if (_fOwn)
                _ptr.Free();
            _mm.Free();
            GC.SuppressFinalize(this);
        }

        public PrinterObject(string printerName)
        {
            internalPopulatePrinter(printerName, this);
        }

        ~PrinterObject()
        {
            if (_fOwn)
                _ptr.Free();
            _mm.Free();
        }

        public override string ToString()
        {
            return PrinterName;
        }

        /// <summary>
        /// Get the printable area of the page
        /// </summary>
        /// <param name="printer">The printer</param>
        /// <param name="paper">Paper type</param>
        /// <param name="resolution">Resolution</param>
        /// <param name="orientation">Orientation</param>
        /// <returns></returns>
        public static UniRect GetPrintableArea(PrinterObject printer, SystemPaperType paper, UniSize resolution, int orientation = 0)
        {
            var rc = new UniRect();
            DeviceMode dev = (DeviceMode)printer._DevMode.Clone();
            dev.Fields = DeviceModeFields.Orientation | DeviceModeFields.PaperSize | DeviceModeFields.YResolution;
            dev.YResolution = (short)resolution.CY;
            dev.PrintQuality = (short)resolution.CX;
            dev.PaperSize = paper;
            dev.Orientation = (short)orientation;
            var hdc = User32.CreateDC(null, printer.PrinterName, IntPtr.Zero, printer._DevMode._ptr);
            if (hdc != IntPtr.Zero)
            {
                int CX = PrinterModule.GetDeviceCaps(hdc, PrinterModule.PHYSICALWIDTH);
                int CY = PrinterModule.GetDeviceCaps(hdc, PrinterModule.PHYSICALHEIGHT);
                int marginX = PrinterModule.GetDeviceCaps(hdc, PrinterModule.PHYSICALOFFSETX);
                int marginY = PrinterModule.GetDeviceCaps(hdc, PrinterModule.PHYSICALOFFSETY);
                User32.DeleteDC(hdc);
                rc.Left = marginX / resolution.CX;
                rc.Top = marginY / resolution.CY;
                rc.Width = (CX - marginX * 2) / resolution.CX;
                rc.Height = (CY - marginY * 2) / resolution.CY;
            }

            return rc;
        }

        /// <summary>
        /// Get <see cref="PrinterObject"/> by name
        /// </summary>
        /// <param name="name">The name of the printer</param>
        /// <returns></returns>
        public static PrinterObject GetPrinterInfoObject(string name)
        {
            PrinterObject po = null;
            internalGetPrinter(name, ref po);
            return po;
        }

        [DllImport("winspool.drv", EntryPoint = "GetDefaultPrinterW", CharSet = CharSet.Unicode)]
        private static extern bool GetDefaultPrinter(IntPtr pszBuffer, ref uint pcchBuffer);

        /// <summary>
        /// Returns the default printer for the system.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static PrinterObject GetDefaultPrinter()
        {
            return new PrinterObject(DefaultPrinterName);
        }

        /// <summary>
        /// Returns the name of the default printer
        /// </summary>
        /// <returns></returns>
        public static string DefaultPrinterName
        {
            get
            {
                string DefaultPrinterNameRet = default;
                uint l = 0U;
                MemPtr mm = new MemPtr();
                GetDefaultPrinter(IntPtr.Zero, ref l);
                if (l <= 0L)
                    return null;
                l = (uint)(l * 2L + 2L);
                mm.Alloc(l);
                if (GetDefaultPrinter(mm, ref l))
                {
                    DefaultPrinterNameRet = (string)mm;
                }
                else
                {
                    DefaultPrinterNameRet = null;
                }

                mm.Free();
                return DefaultPrinterNameRet;
            }
        }

        private static void internalPopulatePrinter(string name, PrinterObject printer)
        {
            internalGetPrinter(name, ref printer);
        }

        private static void internalGetPrinter(string name, ref PrinterObject printer)
        {
            // MsgBox("We are in internalGetPrinter for " & name)

            var mm = new MemPtr();
            uint cb = 0U;
            var hprinter = IntPtr.Zero;

            if (string.IsNullOrEmpty(name))
            {
                // Interaction.MsgBox("Got null printer name.");
                printer = null;
                return;
            }

            if (!PrinterModule.OpenPrinter(name, ref hprinter, IntPtr.Zero))
            {
                // Interaction.MsgBox("OpenPrinter failed, last Win32 Error: " + NativeError.FormatLastError((uint)Marshal.GetLastWin32Error()));
                return;
            }

            // MsgBox("Open Printer for '" & name & "' succeeded...")
            try
            {
                PrinterModule.GetPrinter(hprinter, 2U, IntPtr.Zero, 0U, ref cb);
                mm.Alloc(cb);
                PrinterModule.GetPrinter(hprinter, 2U, mm, cb, ref cb);
                if (printer is null)
                {
                    printer = new PrinterObject();
                    printer._ptr = mm;
                }
                else
                {
                    if (printer._ptr.Handle != IntPtr.Zero)
                    {
                        try
                        {
                            printer._ptr.Free();
                        }
                        catch
                        {
                        }
                    }
                    // we will be holding on to this.
                    printer._ptr = mm;
                }

                if (IntPtr.Size == 4)
                {
                    printer._DevMode = new DeviceMode((IntPtr)mm.IntAt(7L), false);
                }
                // MsgBox("Got a pointer! x86 mode.")
                else
                {
                    printer._DevMode = new DeviceMode((IntPtr)mm.LongAt(7L), false);
                    // MsgBox("Got a pointer! x64 mode.")
                    // If printer._DevMode IsNot Nothing Then MsgBox("DevMode retrieval successful, devmode reports device name as '" & printer._DevMode.DeviceName & "'")
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\sbslog.log", ex.Message + "\r\n" + ex.StackTrace + "\r\n" + "\r\n");
            }

            PrinterModule.ClosePrinter(hprinter);
            internalPopulateDevCaps(ref printer);
        }

        private static void internalPopulateDevCaps(ref PrinterObject printer)
        {
            try
            {
                var mm = new MemPtr();

                // Get the supported resolutions.

                // MsgBox("Polling printer for available print quality resolution.")

                uint[] res;
                uint l = PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_ENUMRESOLUTIONS, IntPtr.Zero, IntPtr.Zero);
                if (printer.PrinterName.Contains("HP LaserJet"))
                {
                    if (l == 0xFFFFFFFFU)
                    {
                        // MsgBox("Attempt to get resolutions failed miserably, let's try it without the port name...")
                        l = PrinterModule.DeviceCapabilities(printer.PrinterName, null, PrinterModule.DC_ENUMRESOLUTIONS, IntPtr.Zero, IntPtr.Zero);
                        if (l == 0xFFFFFFFFU)
                        {
                            // MsgBox("That still failed.  We are going to give the LaserJet practical resolutions, 600x600 and 1200x1200 so that it won't barf")
                            var nRes = new List<LinearSize>();
                            nRes.AddRange(new[] { new LinearSize(600d, 600d), new LinearSize(1200d, 1200d) });
                            printer._Resolutions = nRes;
                        }
                    }

                    if (l > 0L & l != 0xFFFFFFFFU)
                    {
                        // MsgBox("HP LaserJet SAYS it has " & l & " resolutions.")
                        res = null;
                        mm = new MemPtr(l * 8L);
                        l = PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_ENUMRESOLUTIONS, mm, IntPtr.Zero);

                        // MsgBox("Retrieved printer resolutions, RetVal=" & l)

                        try
                        {
                            // MsgBox("Casting memory into UInteger() array")
                            res = mm.ToArray<uint>();
                        }
                        catch
                        {
                            // Interaction.MsgBox("Getting Resolutions bounced!", MsgBoxStyle.Exclamation);
                        }

                        mm.Free();
                        if (res is object)
                        {
                            string stm = "";
                            foreach (var rn in res)
                            {
                                if (!string.IsNullOrEmpty(stm))
                                    stm += ", ";
                                stm += rn.ToString();
                            }

                            // MsgBox("Resolution raw data for LaserJet: " & stm)

                            var supRes = new List<LinearSize>();

                            // MsgBox("Res count should be divisible by two, is it? Count: " & res.Count)

                            var resLen = res.Length;

                            for (int i = 0; i < resLen; i += 2)
                            {
                                if (res.Length % 2 != 0 && i == res.Length - 1)
                                {
                                    supRes.Add(new LinearSize(res[i], res[i]));
                                }
                                else
                                {
                                    supRes.Add(new LinearSize(res[i], res[i + 1]));
                                }
                            }

                            // MsgBox("Finally, we're going to report that the printer has " & supRes.Count & " resolutions.")
                            printer.Resolutions = supRes;
                        }
                    }
                }
                else if (l > 0L & l != 0xFFFFFFFFU)
                {
                    res = null;
                    mm = new MemPtr(l * 8L);
                    l = PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_ENUMRESOLUTIONS, mm, IntPtr.Zero);
                    try
                    {
                        res = mm.ToArray<uint>();
                    }
                    catch // (Exception ex)
                    {
                        // Interaction.MsgBox("Getting Resolutions bounced!", MsgBoxStyle.Exclamation);
                    }

                    mm.Free();
                    var supRes = new List<LinearSize>();
                    if (res is object)
                    {
                        var resLen = res.Length;

                        for (int i = 0; i < resLen; i += 2)
                        {
                            if (res.Length % 2 != 0 && i == res.Length - 1)
                            {
                                supRes.Add(new LinearSize(res[i], res[i]));
                            }
                            else
                            {
                                supRes.Add(new LinearSize(res[i], res[i + 1]));
                            }
                        }

                        printer.Resolutions = supRes;
                    }
                    else
                    {
                        var nRes = new List<LinearSize>();
                        nRes.AddRange(new[] { new LinearSize(300d, 300d), new LinearSize(600d, 600d), new LinearSize(1200d, 1200d) });
                        printer._Resolutions = nRes;
                    }
                }

                // MsgBox("Found " & printer.Resolutions.Count & " resolutions.")

                // MsgBox("Getting paper class sizes")
                // Get the supported paper types.
                l = PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_PAPERS, IntPtr.Zero, IntPtr.Zero);

                // supported paper types are short ints:
                if (l > 0L)
                {
                    mm = new MemPtr(l * 2L);
                    l = PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_PAPERS, mm, IntPtr.Zero);
                    printer._PaperSizes = SystemPaperTypes.TypeListFromCodes(mm.ToArray<short>());
                    mm.Free();
                }

                // MsgBox("Retrieved " & l & " supported paper class sizes.")

                // MsgBox("Looking for the printer trays.")
                // get the names of the supported paper bins.
                l = PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_BINNAMES, IntPtr.Zero, IntPtr.Zero);
                if (l > 0L)
                {
                    mm = new MemPtr(l * 24L * 2L);
                    mm.ZeroMemory();

                    PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_BINNAMES, mm, IntPtr.Zero);
                    printer._Bins.Clear();

                    string srs;
                    int p;

                    for (long i = 0L; i < l; i++)
                    {
                        // some p.o.s. printers make it hard.
                        srs = null;
                        srs = mm.GetString(i * 24L * 2L, 24);
                        if (srs is object)
                        {
                            for (p = 0; p <= 23; p++)
                            {
                                if (srs[p] == '\0')
                                    break;
                            }

                            if (p < 24 && p != 0)
                            {
                                srs = srs.Substring(0, p);
                            }
                            else if (p == 0)
                            {
                                srs = "Unnamed Tray (#" + (i + 1L) + ")";
                            }

                            // MsgBox("Adding printer bin/tray " & srs)
                            printer._Bins.Add(srs);
                        }
                    }

                    mm.Free();
                }

                l = PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_COLORDEVICE, IntPtr.Zero, IntPtr.Zero);
                printer.IsColorPrinter = l == 0 ? false : true;
                printer._LandscapeRotation = (int)PrinterModule.DeviceCapabilities(printer.PrinterName, printer.PortName, PrinterModule.DC_ORIENTATION, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\sbslog.log", ex.Message + "\r\n" + ex.StackTrace + "\r\n" + "\r\n");
            }
        }

        /// <summary>
        /// Select a printer into this object by printer name.
        /// </summary>
        /// <param name="printerName"></param>
        /// <remarks></remarks>
        public void SelectPrinter(string printerName)
        {
            var argprinter = this;
            internalGetPrinter(printerName, ref argprinter);
        }

        /// <summary>
        /// Returns true if this printer is the Windows system default printer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsDefault
        {
            get
            {
                return (PrinterName ?? "") == (DefaultPrinterName ?? "");
            }
        }

        private int _LandscapeRotation;

        /// <summary>
        /// Returns the relationship between portrait and landscape orientations for a device, <br/>
        /// in terms of the number of degrees that portrait orientation is rotated counterclockwise <br/>
        /// to produce landscape orientation.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int LandscapeRotation
        {
            get
            {
                return _LandscapeRotation;
            }

            internal set
            {
                _LandscapeRotation = value;
            }
        }

        private bool _IsColor;

        /// <summary>
        /// Returns true if the printer is capable of color.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsColorPrinter
        {
            get
            {
                return _IsColor;
            }

            internal set
            {
                _IsColor = value;
            }
        }

        /// <summary>
        /// Reports the highest resolution that this printer is capable of printing in.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public LinearSize HighestResolution
        {
            get
            {
                var hsize = new LinearSize();
                foreach (var r in _Resolutions)
                {
                    if (r.Width > hsize.Width & r.Height > hsize.Height)
                    {
                        hsize = r;
                    }
                }

                return hsize;
            }
        }

        private List<LinearSize> _Resolutions;

        /// <summary>
        /// Gets a list of all supported resolutions.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ICollection<LinearSize> Resolutions
        {
            get
            {
                return _Resolutions;
            }

            internal set
            {
                _Resolutions = (List<LinearSize>)value;
            }
        }

        private List<SystemPaperType> _PaperSizes;

        /// <summary>
        /// Gets a list of all paper sizes supported by this printer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ICollection<SystemPaperType> PaperSizes
        {
            get
            {
                return _PaperSizes;
            }

            internal set
            {
                _PaperSizes = (List<SystemPaperType>)value;
            }
        }

        /// <summary>
        /// Gets a value indicating that this printer supports this particular paper size.
        /// </summary>
        /// <param name="size">The LinearSize structure to compare.</param>
        /// <param name="sizeMetric">True if the given size is in millimeters.</param>
        /// <param name="exactOrientation">True to not compare rotated sizes.</param>
        /// <returns>True if all conditions are met and a size match is found.</returns>
        /// <remarks></remarks>
        public bool SupportsPaperSize(LinearSize size, bool sizeMetric = false, bool exactOrientation = false)
        {
            // two separate for-eaches for time-saving. We don't need to test for sizeMetric for every iteration.
            // we don't need to test exactOrientation every time, either, but that's only referenced once per iteration.

            if (sizeMetric)
            {
                // testing for the millimeters size.
                foreach (var p in _PaperSizes)
                {
                    if (p.SizeMillimeters.Equals(size))
                        return true;
                    if (exactOrientation)
                        continue;
                    {
                        var withBlock = p.SizeMillimeters;
                        if (withBlock.Width == size.Height && withBlock.Height == size.Width)
                            return true;
                    }
                }
            }
            else
            {
                foreach (var p in _PaperSizes)
                {
                    if (p.SizeInches.Equals(size))
                        return true;
                    if (exactOrientation)
                        continue;
                    {
                        var withBlock1 = p.SizeInches;
                        if (withBlock1.Width == size.Height && withBlock1.Height == size.Width)
                            return true;
                    }
                }
            }

            // nothing found!
            return false;
        }

        private List<string> _Bins = new List<string>();

        /// <summary>
        /// Returns a list of all of the available trays this printer serves from.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ICollection<string> PrinterTrays
        {
            get
            {
                return _Bins;
            }

            internal set
            {
                _Bins = (List<string>)value;
            }
        }

        /// <summary>
        /// Gets the server name of this printer.  If this string is null, the printer is served locally.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ServerName
        {
            get
            {
                return _ptr.GetStringIndirect(0 * IntPtr.Size);
            }

            internal set
            {
                _ptr.SetStringIndirect(0 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets the name of the printer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PrinterName
        {
            get
            {
                return _ptr.GetStringIndirect(1 * IntPtr.Size);
            }

            internal set
            {
                _ptr.SetStringIndirect(1 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets the share name of the printer.
        /// If this printer is not shared, this value is null.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ShareName
        {
            get
            {
                return _ptr.GetStringIndirect(2 * IntPtr.Size);
            }

            internal set
            {
                _ptr.SetStringIndirect(2 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets the port name of the printer.  This could be a standard port, or a special port name.
        /// <br/>
        /// If a printer is connected to more than one port, the names of each port must be separated by commas (for example, "LPT1:,LPT2:,LPT3:").
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PortName
        {
            get
            {
                return _ptr.GetStringIndirect(3 * IntPtr.Size);
            }

            internal set
            {
                _ptr.SetStringIndirect(3 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Returns the name of the printer driver.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DriverName
        {
            get
            {
                return _ptr.GetStringIndirect(4 * IntPtr.Size);
            }

            internal set
            {
                _ptr.SetStringIndirect(4 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets a brief desecription of the printer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Comment
        {
            get
            {
                return _ptr.GetStringIndirect(5 * IntPtr.Size);
            }

            internal set
            {
                _ptr.SetStringIndirect(5 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets a string that specifies the physical location of the printer (for example, "Bldg. 38, Room 1164").
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Location
        {
            get
            {
                return _ptr.GetStringIndirect(6 * IntPtr.Size);
            }

            internal set
            {
                _ptr.SetStringIndirect(6 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets the device mode object that reports on and controls further aspect of the printer's current configuration.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DeviceMode DevMode
        {
            get
            {
                return _DevMode;
            }
        }

        /// <summary>
        /// Gets or sets a string that specifies the name of the file used to create the separator page. This page is used to separate print jobs sent to the printer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SepFile
        {
            get
            {
                return _ptr.GetStringIndirect(8 * IntPtr.Size);
            }

            set
            {
                _ptr.SetStringIndirect(8 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets or sets a string that specifies the name of the print processor used by the printer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>You can use the EnumPrintProcessors function to obtain a list of print processors installed on a server.</remarks>
        public string PrintProcessor
        {
            get
            {
                return _ptr.GetStringIndirect(9 * IntPtr.Size);
            }

            set
            {
                _ptr.SetStringIndirect(9 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets or sets a string that specifies the data type used to record the print job.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>You can use the EnumPrintProcessorDatatypes function to obtain a list of data types supported by a specific print processor.</remarks>
        public string Datatype
        {
            get
            {
                return _ptr.GetStringIndirect(10 * IntPtr.Size);
            }

            set
            {
                _ptr.SetStringIndirect(10 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Gets or sets a string that specifies the default print-processor parameters.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Parameters
        {
            get
            {
                return _ptr.GetStringIndirect(11 * IntPtr.Size);
            }

            set
            {
                _ptr.SetStringIndirect(11 * IntPtr.Size, value);
            }
        }

        /// <summary>
        /// Pointer to a SECURITY_DESCRIPTOR structure containing the ACL info.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IntPtr SecurityDescriptor
        {
            get
            {
                return (IntPtr)_ptr.LongAt(12L);
            }

            internal set
            {
                _ptr.LongAt(12L) = (long)value;
            }
        }

        /// <summary>
        /// gets or sets the printer's attributes.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PrinterAttributes Attributes
        {
            get
            {
                return (PrinterAttributes)(_ptr.UIntAt(26L));
            }

            set
            {
                _ptr.UIntAt(26L) = (uint)value;
            }
        }

        /// <summary>
        /// A priority value that the spooler uses to route print jobs.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public uint Priority
        {
            get
            {
                return _ptr.UIntAt(27L);
            }

            set
            {
                _ptr.UIntAt(27L) = value;
            }
        }

        /// <summary>
        /// The default priority value assigned to each print job.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public uint DefaultPriority
        {
            get
            {
                return _ptr.UIntAt(28L);
            }

            internal set
            {
                _ptr.UIntAt(28L) = value;
            }
        }

        /// <summary>
        /// The earliest time of day the printer will start taking print jobs for the day.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public TimeSpan StartTime
        {
            get
            {
                return new TimeSpan(0, _ptr.IntAt(29L), 0);
            }

            internal set
            {
                _ptr.IntAt(29L) = (int)value.TotalMinutes;
            }
        }

        /// <summary>
        /// The time of day that the printer stops taking jobs for the day.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public TimeSpan UntilTime
        {
            get
            {
                return new TimeSpan(0, _ptr.IntAt(30L), 0);
            }

            internal set
            {
                _ptr.IntAt(30L) = (int)value.TotalMinutes;
            }
        }

        private bool _LiveUpdateStatus = false;

        /// <summary>
        /// Gets or sets a value indicating that the status will be updated live, every time it is retrieved.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool LiveUpdateStatus
        {
            get
            {
                return _LiveUpdateStatus;
            }

            set
            {
                _LiveUpdateStatus = value;
            }
        }

        /// <summary>
        /// Gets the current printer status.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PrinterStatus Status
        {
            get
            {
                if (_LiveUpdateStatus)
                    return UpdateStatus();
                return (PrinterStatus)(_ptr.UIntAt(31L));
            }

            internal set
            {
                _ptr.UIntAt(31L) = (uint)value;
            }
        }

        /// <summary>
        /// Updates the printer's current status.
        /// </summary>
        /// <remarks></remarks>
        public PrinterStatus UpdateStatus()
        {
            uint cb = 0U;
            var hprinter = IntPtr.Zero;
            if (!PrinterModule.OpenPrinter(PrinterName, ref hprinter, IntPtr.Zero))
                return PrinterStatus.Error;
            PrinterModule.GetPrinter(hprinter, 6U, IntPtr.Zero, 0U, ref cb);
            if (cb > 16L)
            {
                _mm.ReAlloc(cb);
            }

            PrinterModule.GetPrinter(hprinter, 6U, _mm, cb, ref cb);
            _ptr.UIntAt(31L) = _mm.UIntAt(0L);
            PrinterModule.ClosePrinter(hprinter);
            return (PrinterStatus)(_ptr.UIntAt(31L));
        }

        /// <summary>
        /// The number of printer jobs in this printer's queue.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public uint cJobs
        {
            get
            {
                return _ptr.UIntAt(32L);
            }

            internal set
            {
                _ptr.UIntAt(32L) = value;
            }
        }

        /// <summary>
        /// This printer's average pages per minute.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public uint AveragePPM
        {
            get
            {
                return _ptr.UIntAt(33L);
            }

            internal set
            {
                _ptr.UIntAt(33L) = value;
            }
        }

        /// <summary>
        /// Explicitly cast string to <see cref="PrinterObject"/>
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static explicit operator PrinterObject(string operand)
        {
            try
            {
                return new PrinterObject(operand);
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException("That printer was not found on the system.", ex);
            }
        }
    }
}