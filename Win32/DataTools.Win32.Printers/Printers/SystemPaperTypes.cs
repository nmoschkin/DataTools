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
using DataTools.MathTools.PolarMath;

namespace DataTools.Hardware.Printers
{
    /// <summary>
    /// Represents a collection of all known system paper types
    /// </summary>
    public static class SystemPaperTypes
    {
        
        private static readonly string _SizeDataString = "LETTER	1	US Letter 8 1/2 x 11 in" + "\r\n" + "LETTER_SMALL	2	US Letter Small 8 1/2 x 11 in" + "\r\n" + "TABLOID	3	US Tabloid 11 x 17 in" + "\r\n" + "LEDGER	4	US Ledger 17 x 11 in" + "\r\n" + "LEGAL	5	US Legal 8 1/2 x 14 in" + "\r\n" + "STATEMENT	6	US Statement 5 1/2 x 8 1/2 in" + "\r\n" + "EXECUTIVE	7	US Executive 7 1/4 x 10 1/2 in" + "\r\n" + "A3	8	A3 297 x 420 mm" + "\r\n" + "A4	9	A4 210 x 297 mm" + "\r\n" + "A4_SMALL	10	A4 Small 210 x 297 mm" + "\r\n" + "A5	11	A5 148 x 210 mm" + "\r\n" + "B4	12	B4 (JIS) 257 x 364 mm" + "\r\n" + "B5	13	B5 (JIS) 182 x 257 mm" + "\r\n" + "FOLIO	14	Folio 8 1/2 x 13 in" + "\r\n" + "QUARTO	15	Quarto 215 x 275 mm" + "\r\n" + "10X14	16	10 x 14 in" + "\r\n" + "11X17	17	11 x 17 in" + "\r\n" + "NOTE	18	US Note 8 1/2 x 11 in" + "\r\n" + "ENV_9	19	US Envelope #9 - 3 7/8 x 8 7/8" + "\r\n" + "ENV_10	20	US Envelope #10 - 4 1/8 x 9 1/2" + "\r\n" + "ENV_11	21	US Envelope #11 - 4 1/2 x 10 3/8" + "\r\n" + "ENV_12	22	US Envelope #12 - 4 3/4 x 11 in" + "\r\n" + "ENV_14	23	US Envelope #14 - 5 x 11 1/2" + "\r\n" + "ENV_DL	27	Envelope DL 110 x 220 mm" + "\r\n" + "ENV_C5	28	Envelope C5 - 162 x 229 mm" + "\r\n" + "ENV_C3	29	Envelope C3 - 324 x 458 mm" + "\r\n" + "ENV_C4	30	Envelope C4 - 229 x 324 mm" + "\r\n" + "ENV_C6	31	Envelope C6 - 114 x 162 mm" + "\r\n" + "ENV_C65	32	Envelope C65 - 114 x 229 mm" + "\r\n" + "ENV_B4	33	Envelope B4 - 250 x 353 mm" + "\r\n" + "ENV_B5	34	Envelope B5 - 176 x 250 mm" + "\r\n" + "ENV_B6	35	Envelope B6 - 176 x 125 mm" + "\r\n" + "ENV_ITALY	36	Envelope 110 x 230 mm" + "\r\n" + "ENV_MONARCH	37	US Envelope Monarch 3.875 x 7.5 in" + "\r\n" + "ENV_PERSONAL	38	6 3/4 US Envelope 3 5/8 x 6 1/2 in" + "\r\n" + "FANFOLD_US	39	US Std Fanfold 14 7/8 x 11 in" + "\r\n" + "FANFOLD_STD_GERMAN	40	German Std Fanfold 8 1/2 x 12 in" + "\r\n" + "FANFOLD_LGL_GERMAN	41	German Legal Fanfold 8 1/2 x 13 in" + "\r\n" + "ISO_B4	42	B4 (ISO) 250 x 353 mm" + "\r\n" + "JAPANESE_POSTCARD	43	Japanese Postcard 100 x 148 mm" + "\r\n" + "9X11	44	9 x 11 in" + "\r\n" + "10X11	45	10 x 11 in" + "\r\n" + "15X11	46	15 x 11 in" + "\r\n" + "ENV_INVITE	47	Envelope Invite 220 x 220 mm" + "\r\n" + "LETTER_EXTRA	50	US Letter Extra 9 1/2 x 12 in" + "\r\n" + "LEGAL_EXTRA	51	US Legal Extra 9 1/2 x 15 in" + "\r\n" + "TABLOID_EXTRA	52	US Tabloid Extra 11.69 x 18 in" + "\r\n" + "A4_EXTRA	53	A4 Extra 9.27 x 12.69 in" + "\r\n" + "LETTER_TRANSVERSE	54	Letter Transverse 8 1/2 x 11 in" + "\r\n" + "A4_TRANSVERSE	55	A4 Transverse 210 x 297 mm" + "\r\n" + "LETTER_EXTRA_TRANSVERSE	56	Letter Extra Transverse 9 1/2 x 12 in" + "\r\n" + "A_PLUS	57	SuperA/SuperA/A4 227 x 356 mm" + "\r\n" + "B_PLUS	58	SuperB/SuperB/A3 305 x 487 mm" + "\r\n" + "LETTER_PLUS	59	US Letter Plus 8.5 x 12.69 in" + "\r\n" + "A4_PLUS	60	A4 Plus 210 x 330 mm" + "\r\n" + "A5_TRANSVERSE	61	A5 Transverse 148 x 210 mm" + "\r\n" + "B5_TRANSVERSE	62	B5 (JIS) Transverse 182 x 257 mm" + "\r\n" + "A3_EXTRA	63	A3 Extra 322 x 445 mm" + "\r\n" + "A5_EXTRA	64	A5 Extra 174 x 235 mm" + "\r\n" + "B5_EXTRA	65	B5 (ISO) Extra 201 x 276 mm" + "\r\n" + "A2	66	A2 420 x 594 mm" + "\r\n" + "A3_TRANSVERSE	67	A3 Transverse 297 x 420 mm" + "\r\n" + "A3_EXTRA_TRANSVERSE	68	A3 Extra Transverse 322 x 445 mm" + "\r\n" + "DBL_JAPANESE_POSTCARD	69	Japanese Double Postcard 200 x 148 mm" + "\r\n" + "A6	70	A6 105 x 148 mm" + "\r\n" + "LETTER_ROTATED	75	Letter Rotated 11 x 8 1/2 11 in" + "\r\n" + "A3_ROTATED	76	A3 Rotated 420 x 297 mm" + "\r\n" + "A4_ROTATED	77	A4 Rotated 297 x 210 mm" + "\r\n" + "A5_ROTATED	78	A5 Rotated 210 x 148 mm" + "\r\n" + "B4_JIS_ROTATED	79	B4 (JIS) Rotated 364 x 257 mm" + "\r\n" + "B5_JIS_ROTATED	80	B5 (JIS) Rotated 257 x 182 mm" + "\r\n" + "JAPANESE_POSTCARD_ROTATED	81	Japanese Postcard Rotated 148 x 100 mm" + "\r\n" + "DBL_JAPANESE_POSTCARD_ROTATED	82	Double Japanese Postcard Rotated 148 x 200 mm" + "\r\n" + "A6_ROTATED	83	A6 Rotated 148 x 105 mm" + "\r\n" + "B6_JIS	88	B6 (JIS) 128 x 182 mm" + "\r\n" + "B6_JIS_ROTATED	89	B6 (JIS) Rotated 182 x 128 mm" + "\r\n" + "12X11	90	12 x 11 in" + "\r\n" + "P16K	93	PRC 16K 146 x 215 mm" + "\r\n" + "P32K	94	PRC 32K 97 x 151 mm" + "\r\n" + "P32KBIG	95	PRC 32K(Big) 97 x 151 mm" + "\r\n" + "PENV_1	96	PRC Envelope #1 - 102 x 165 mm" + "\r\n" + "PENV_2	97	PRC Envelope #2 - 102 x 176 mm" + "\r\n" + "PENV_3	98	PRC Envelope #3 - 125 x 176 mm" + "\r\n" + "PENV_4	99	PRC Envelope #4 - 110 x 208 mm" + "\r\n" + "PENV_5	100	PRC Envelope #5 - 110 x 220 mm" + "\r\n" + "PENV_6	101	PRC Envelope #6 - 120 x 230 mm" + "\r\n" + "PENV_7	102	PRC Envelope #7 - 160 x 230 mm" + "\r\n" + "PENV_8	103	PRC Envelope #8 - 120 x 309 mm" + "\r\n" + "PENV_9	104	PRC Envelope #9 - 229 x 324 mm" + "\r\n" + "PENV_10	105	PRC Envelope #10 - 324 x 458 mm" + "\r\n" + "PENV_1_ROTATED	109	PRC Envelope #1 Rotated 165 x 102 mm" + "\r\n" + "PENV_2_ROTATED	110	PRC Envelope #2 Rotated 176 x 102 mm" + "\r\n" + "PENV_3_ROTATED	111	PRC Envelope #3 Rotated 176 x 125 mm" + "\r\n" + "PENV_4_ROTATED	112	PRC Envelope #4 Rotated 208 x 110 mm" + "\r\n" + "PENV_5_ROTATED	113	PRC Envelope #5 Rotated 220 x 110 mm" + "\r\n" + "PENV_6_ROTATED	114	PRC Envelope #6 Rotated 230 x 120 mm" + "\r\n" + "PENV_7_ROTATED	115	PRC Envelope #7 Rotated 230 x 160 mm" + "\r\n" + "PENV_8_ROTATED	116	PRC Envelope #8 Rotated 309 x 120 mm" + "\r\n" + "PENV_9_ROTATED	117	PRC Envelope #9 Rotated 324 x 229 mm" + "\r\n" + "PENV_10_ROTATED	118	PRC Envelope #10 Rotated 458 x 324 mm";

        
        /// <summary>
        /// Returns the list of supported system paper types.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ReadOnlyCollection<SystemPaperType> PaperTypes
        {
            get
            {
                return _PaperList;
            }
        }

        private static ReadOnlyCollection<SystemPaperType> _PaperList;

        private static void ParsePapers()
        {
            var objOut = new List<SystemPaperType>();
            var paperList = TextTools.Split(_SizeDataString, "\r\n");
            foreach (var paper in paperList)
            {
                var p = new SystemPaperType();
                var data = TextTools.Split(paper, "\t");
                p.Name = TextTools.CamelCase(data[0]);
                p.Code = int.Parse(data[1]);
                p.Description = data[2];
                p.IsTransverse = data[2].IndexOf("Transverse") != -1;
                p.IsPostcard = data[2].IndexOf("Postcard") != -1;
                p.IsRotated = data[2].IndexOf("Rotated") != -1;
                p.IsEnvelope = data[2].IndexOf("Envelope") != -1;
                if (data[2].IndexOf("German") != -1)
                {
                    p.Nationality = PaperNationalities.German;
                }
                else if (data[2].IndexOf("US ") != -1)
                {
                    p.Nationality = PaperNationalities.American;
                }
                else if (data[2].IndexOf("PRC") != -1)
                {
                    p.Nationality = PaperNationalities.Chinese;
                }
                else if (data[2].IndexOf("Japan") != -1)
                {
                    p.Nationality = PaperNationalities.Japanese;
                }
                else if (data[2].IndexOf("(JIS)") != -1)
                {
                    p.Nationality = PaperNationalities.Japanese;
                }

                bool ismm = false;
                var size = FindSize(data[2], ref ismm);
                if (ismm)
                {
                    p.SizeMillimeters = size;
                }
                else
                {
                    p.SizeInches = size;
                }

                objOut.Add(p);
                p = null;
            }

            _PaperList = new ReadOnlyCollection<SystemPaperType>(objOut);
        }

        //'' <summary>
        //'' Parses a size from any kind of text.
        //'' </summary>
        //'' <param name="text">The text to parse.</param>
        //'' <param name="isMM">Receives a value indicating metric system.</param>
        //'' <param name="scanforDblQuote">Scan for double quotes as inches.</param>
        //'' <param name="acceptComma">Accept a comma as a separator in addition to the 'x'.</param>
        //'' <returns></returns>
        //'' <remarks></remarks>
        private static LinearSize FindSize(string text, ref bool isMM, bool scanforDblQuote = false, bool acceptComma = false)
        {
            char[] ch;
            var sOut = new LinearSize();
            bool pastX = false;
            int i;
            int c;
            int x = 0;
            string t;
            if (scanforDblQuote)
            {
                text = text.Replace("\"", "in").Trim();
            }

            t = text.Substring(text.Length - 2, 2).ToLower();
            isMM = t == "mm";
            if (t == "in" || t == "mm")
            {
                text = text.Substring(0, text.Length - 2).Trim();
            }

            ch = text.ToCharArray();
            i = ch.Count() - 1;

            // not allowed space (for metric)
            bool nas = false;
            for (c = i; c >= 0; c -= 1)
            {
                if (ch[c] == 'x')
                {
                    pastX = true;
                    x = i;
                }
                else if (acceptComma && ch[c] == ',')
                {
                    pastX = true;
                    x = i;
                }
                else if (pastX)
                {
                    if (ch[c] == ' ')
                    {
                        if (isMM & nas)
                            break;
                        continue;
                    }

                    if (ch[c] == '/')
                    {
                        continue;
                    }

                    if (ch[c] == '.')
                    {
                        continue;
                    }

                    if (TextTools.IsNumber(ch[c]) == false)
                    {
                        break;
                    }
                    else
                    {
                        nas = true;
                    }
                }
            }

            text = text.Substring(c + 1).Trim();
            text = text.Replace(",", "x");
            var sizes = TextTools.Split(text, "x");
            double d;
            double e;
            double f;
            int sc = 0;
            foreach (var num in sizes)
            {
                d = 0d;
                var nch = TextTools.Split(num.Trim(), " ");
                if (nch.Count() == 2)
                {
                    var div = TextTools.Split(nch[1], "/");
                    if (div.Count() == 2)
                    {
                        d = double.Parse(div[0]);
                        e = double.Parse(div[1]);
                        d /= e;
                    }
                }

                f = double.Parse(nch[0]) + d;
                if (sc == 0)
                    sOut.Width = f;
                else
                    sOut.Height = f;
                sc = 1;
            }

            return sOut;
        }

        internal static List<SystemPaperType> TypeListFromCodes(IEnumerable<short> list)
        {
            var o = new List<SystemPaperType>();
            foreach (var p in _PaperList)
            {
                foreach (var i in list)
                {
                    if (p.Code == i)
                    {
                        o.Add(p);
                        break;
                    }
                }
            }

            return o;
        }

        static SystemPaperTypes()
        {
            ParsePapers();
        }
    }
}
