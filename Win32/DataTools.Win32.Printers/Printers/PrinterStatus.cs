// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Printers
//         Windows Printer Api
//
//         Enums are documented in part from the API documentation at MSDN.
//         Other knowledge and references obtained through various sources
//         and all is considered public domain/common knowledge.
//
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''


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
using DataTools.MathTools.PolarMath;

namespace DataTools.Hardware.Printers
{
    /// <summary>
    /// Printer status flags
    /// </summary>
    [Flags]
    public enum PrinterStatus : uint
    {
        /// <summary>
        /// The printer is busy.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is busy.")]
        Busy = PrinterModule.PRINTER_STATUS_BUSY,

        /// <summary>
        /// The printer door is open.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer door is open.")]
        DoorOpen = PrinterModule.PRINTER_STATUS_DOOR_OPEN,

        /// <summary>
        /// The printer is in an error state.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is in an error state.")]
        Error = PrinterModule.PRINTER_STATUS_ERROR,

        /// <summary>
        /// The printer is initializing.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is initializing.")]
        Initializing = PrinterModule.PRINTER_STATUS_INITIALIZING,

        /// <summary>
        /// The printer is in an active input/output state
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is in an active input/output stat.")]
        IoActive = PrinterModule.PRINTER_STATUS_IO_ACTIVE,

        /// <summary>
        /// The printer is in a manual feed state.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is in a manual feed state.")]
        ManualFeed = PrinterModule.PRINTER_STATUS_MANUAL_FEED,

        /// <summary>
        /// The printer is out of toner.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is out of toner.")]
        NoToner = PrinterModule.PRINTER_STATUS_NO_TONER,

        /// <summary>
        /// The printer is not available for printing.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is not available for printing.")]
        NotAvailable = PrinterModule.PRINTER_STATUS_NOT_AVAILABLE,

        /// <summary>
        /// The printer is offline.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is offline.")]
        Offline = PrinterModule.PRINTER_STATUS_OFFLINE,

        /// <summary>
        /// The printer has run out of memory.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer has run out of memory.")]
        OutOfMemory = PrinterModule.PRINTER_STATUS_OUT_OF_MEMORY,

        /// <summary>
        /// The printer's output bin is full.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer's output bin is full.")]
        OutputBinFull = PrinterModule.PRINTER_STATUS_OUTPUT_BIN_FULL,

        /// <summary>
        /// The printer cannot print the current page.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer cannot print the current page.")]
        PagePunt = PrinterModule.PRINTER_STATUS_PAGE_PUNT,

        /// <summary>
        /// Paper is jammed in the printer
        /// </summary>
        /// <remarks></remarks>
        [Description("Paper is jammed in the printe.")]
        PaperJam = PrinterModule.PRINTER_STATUS_PAPER_JAM,

        /// <summary>
        /// The printer is out of paper.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is out of paper.")]
        PaperOut = PrinterModule.PRINTER_STATUS_PAPER_OUT,

        /// <summary>
        /// The printer has a paper problem.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer has a paper problem.")]
        PaperProblem = PrinterModule.PRINTER_STATUS_PAPER_PROBLEM,

        /// <summary>
        /// The printer is paused.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is paused.")]
        Paused = PrinterModule.PRINTER_STATUS_PAUSED,

        /// <summary>
        /// The printer is being deleted.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is being deleted.")]
        PendingDeletion = PrinterModule.PRINTER_STATUS_PENDING_DELETION,

        /// <summary>
        /// The printer is in power save mode.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is in power save mode.")]
        PowerSave = PrinterModule.PRINTER_STATUS_POWER_SAVE,

        /// <summary>
        /// The printer is printing.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is printing.")]
        Printing = PrinterModule.PRINTER_STATUS_PRINTING,

        /// <summary>
        /// The printer is processing a print job.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is processing a print job.")]
        Processing = PrinterModule.PRINTER_STATUS_PROCESSING,

        /// <summary>
        /// The printer status is unknown.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer status is unknown.")]
        ServerUnknown = PrinterModule.PRINTER_STATUS_SERVER_UNKNOWN,

        /// <summary>
        /// The printer is low on toner.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is low on toner.")]
        TonerLow = PrinterModule.PRINTER_STATUS_TONER_LOW,

        /// <summary>
        /// The printer has an error that requires the user to do something.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer has an error that requires the user to do something.")]
        UserIntervention = PrinterModule.PRINTER_STATUS_USER_INTERVENTION,

        /// <summary>
        /// The printer is waiting.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is waiting.")]
        Waiting = PrinterModule.PRINTER_STATUS_WAITING,

        /// <summary>
        /// The printer is warming up.
        /// </summary>
        /// <remarks></remarks>
        [Description("The printer is warming up.")]
        WarmingUp = PrinterModule.PRINTER_STATUS_WARMING_UP
    }
}
