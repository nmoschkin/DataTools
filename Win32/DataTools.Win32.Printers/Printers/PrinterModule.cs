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
    internal static class PrinterModule
    {

        
        
        // define DM_ORIENTATION          0x00000001L
        // #define DM_PAPERSIZE            0x00000002L
        // #define DM_PAPERLENGTH          0x00000004L
        // #define DM_PAPERWIDTH           0x00000008L
        // #define DM_SCALE                0x00000010L
        // #if(WINVER >= 0x0500)
        // #define DM_POSITION             0x00000020L
        // #define DM_NUP                  0x00000040L
        // #endif /* WINVER >= 0x0500 */
        // #if(WINVER >= 0x0501)
        // #define DM_DISPLAYORIENTATION   0x00000080L
        // #endif /* WINVER >= 0x0501 */
        // #define DM_COPIES               0x00000100L
        // #define DM_DEFAULTSOURCE        0x00000200L
        // #define DM_PRINTQUALITY         0x00000400L
        // #define DM_COLOR                0x00000800L
        // #define DM_DUPLEX               0x00001000L
        // #define DM_YRESOLUTION          0x00002000L
        // #define DM_TTOPTION             0x00004000L
        // #define DM_COLLATE              0x00008000L
        // #define DM_FORMNAME             0x00010000L
        // #define DM_LOGPIXELS            0x00020000L
        // #define DM_BITSPERPEL           0x00040000L
        // #define DM_PELSWIDTH            0x00080000L
        // #define DM_PELSHEIGHT           0x00100000L
        // #define DM_DISPLAYFLAGS         0x00200000L
        // #define DM_DISPLAYFREQUENCY     0x00400000L
        // #if(WINVER >= 0x0400)
        // #define DM_ICMMETHOD            0x00800000L
        // #define DM_ICMINTENT            0x01000000L
        // #define DM_MEDIATYPE            0x02000000L
        // #define DM_DITHERTYPE           0x04000000L
        // #define DM_PANNINGWIDTH         0x08000000L
        // #define DM_PANNINGHEIGHT        0x10000000L
        // #endif /* WINVER >= 0x0400 */
        // #if(WINVER >= 0x0501)
        // #define DM_DISPLAYFIXEDOUTPUT   0x20000000L
        // #endif /* WINVER >= 0x0501 */

        // /* orientation selections */
        // #define DMORIENT_PORTRAIT   1
        // #define DMORIENT_LANDSCAPE  2

        // /* paper selections */
        // #define DMPAPER_FIRST                DMPAPER_LETTER
        // #define DMPAPER_LETTER               1  /* Letter 8 1/2 x 11 in               */
        // #define DMPAPER_LETTERSMALL          2  /* Letter Small 8 1/2 x 11 in         */
        // #define DMPAPER_TABLOID              3  /* Tabloid 11 x 17 in                 */
        // #define DMPAPER_LEDGER               4  /* Ledger 17 x 11 in                  */
        // #define DMPAPER_LEGAL                5  /* Legal 8 1/2 x 14 in                */
        // #define DMPAPER_STATEMENT            6  /* Statement 5 1/2 x 8 1/2 in         */
        // #define DMPAPER_EXECUTIVE            7  /* Executive 7 1/4 x 10 1/2 in        */
        // #define DMPAPER_A3                   8  /* A3 297 x 420 mm                    */
        // #define DMPAPER_A4                   9  /* A4 210 x 297 mm                    */
        // #define DMPAPER_A4SMALL             10  /* A4 Small 210 x 297 mm              */
        // #define DMPAPER_A5                  11  /* A5 148 x 210 mm                    */
        // #define DMPAPER_B4                  12  /* B4 (JIS) 250 x 354                 */
        // #define DMPAPER_B5                  13  /* B5 (JIS) 182 x 257 mm              */
        // #define DMPAPER_FOLIO               14  /* Folio 8 1/2 x 13 in                */
        // #define DMPAPER_QUARTO              15  /* Quarto 215 x 275 mm                */
        // #define DMPAPER_10X14               16  /* 10x14 in                           */
        // #define DMPAPER_11X17               17  /* 11x17 in                           */
        // #define DMPAPER_NOTE                18  /* Note 8 1/2 x 11 in                 */
        // #define DMPAPER_ENV_9               19  /* Envelope #9 3 7/8 x 8 7/8          */
        // #define DMPAPER_ENV_10              20  /* Envelope #10 4 1/8 x 9 1/2         */
        // #define DMPAPER_ENV_11              21  /* Envelope #11 4 1/2 x 10 3/8        */
        // #define DMPAPER_ENV_12              22  /* Envelope #12 4 \276 x 11           */
        // #define DMPAPER_ENV_14              23  /* Envelope #14 5 x 11 1/2            */
        // #define DMPAPER_CSHEET              24  /* C size sheet                       */
        // #define DMPAPER_DSHEET              25  /* D size sheet                       */
        // #define DMPAPER_ESHEET              26  /* E size sheet                       */
        // #define DMPAPER_ENV_DL              27  /* Envelope DL 110 x 220mm            */
        // #define DMPAPER_ENV_C5              28  /* Envelope C5 162 x 229 mm           */
        // #define DMPAPER_ENV_C3              29  /* Envelope C3  324 x 458 mm          */
        // #define DMPAPER_ENV_C4              30  /* Envelope C4  229 x 324 mm          */
        // #define DMPAPER_ENV_C6              31  /* Envelope C6  114 x 162 mm          */
        // #define DMPAPER_ENV_C65             32  /* Envelope C65 114 x 229 mm          */
        // #define DMPAPER_ENV_B4              33  /* Envelope B4  250 x 353 mm          */
        // #define DMPAPER_ENV_B5              34  /* Envelope B5  176 x 250 mm          */
        // #define DMPAPER_ENV_B6              35  /* Envelope B6  176 x 125 mm          */
        // #define DMPAPER_ENV_ITALY           36  /* Envelope 110 x 230 mm              */
        // #define DMPAPER_ENV_MONARCH         37  /* Envelope Monarch 3.875 x 7.5 in    */
        // #define DMPAPER_ENV_PERSONAL        38  /* 6 3/4 Envelope 3 5/8 x 6 1/2 in    */
        // #define DMPAPER_FANFOLD_US          39  /* US Std Fanfold 14 7/8 x 11 in      */
        // #define DMPAPER_FANFOLD_STD_GERMAN  40  /* German Std Fanfold 8 1/2 x 12 in   */
        // #define DMPAPER_FANFOLD_LGL_GERMAN  41  /* German Legal Fanfold 8 1/2 x 13 in */
        // #if(WINVER >= 0x0400)
        // #define DMPAPER_ISO_B4              42  /* B4 (ISO) 250 x 353 mm              */
        // #define DMPAPER_JAPANESE_POSTCARD   43  /* Japanese Postcard 100 x 148 mm     */
        // #define DMPAPER_9X11                44  /* 9 x 11 in                          */
        // #define DMPAPER_10X11               45  /* 10 x 11 in                         */
        // #define DMPAPER_15X11               46  /* 15 x 11 in                         */
        // #define DMPAPER_ENV_INVITE          47  /* Envelope Invite 220 x 220 mm       */
        // #define DMPAPER_RESERVED_48         48  /* RESERVED--DO NOT USE               */
        // #define DMPAPER_RESERVED_49         49  /* RESERVED--DO NOT USE               */
        // #define DMPAPER_LETTER_EXTRA        50  /* Letter Extra 9 \275 x 12 in        */
        // #define DMPAPER_LEGAL_EXTRA         51  /* Legal Extra 9 \275 x 15 in         */
        // #define DMPAPER_TABLOID_EXTRA       52  /* Tabloid Extra 11.69 x 18 in        */
        // #define DMPAPER_A4_EXTRA            53  /* A4 Extra 9.27 x 12.69 in           */
        // #define DMPAPER_LETTER_TRANSVERSE   54  /* Letter Transverse 8 \275 x 11 in   */
        // #define DMPAPER_A4_TRANSVERSE       55  /* A4 Transverse 210 x 297 mm         */
        // #define DMPAPER_LETTER_EXTRA_TRANSVERSE 56 /* Letter Extra Transverse 9\275 x 12 in */
        // #define DMPAPER_A_PLUS              57  /* SuperA/SuperA/A4 227 x 356 mm      */
        // #define DMPAPER_B_PLUS              58  /* SuperB/SuperB/A3 305 x 487 mm      */
        // #define DMPAPER_LETTER_PLUS         59  /* Letter Plus 8.5 x 12.69 in         */
        // #define DMPAPER_A4_PLUS             60  /* A4 Plus 210 x 330 mm               */
        // #define DMPAPER_A5_TRANSVERSE       61  /* A5 Transverse 148 x 210 mm         */
        // #define DMPAPER_B5_TRANSVERSE       62  /* B5 (JIS) Transverse 182 x 257 mm   */
        // #define DMPAPER_A3_EXTRA            63  /* A3 Extra 322 x 445 mm              */
        // #define DMPAPER_A5_EXTRA            64  /* A5 Extra 174 x 235 mm              */
        // #define DMPAPER_B5_EXTRA            65  /* B5 (ISO) Extra 201 x 276 mm        */
        // #define DMPAPER_A2                  66  /* A2 420 x 594 mm                    */
        // #define DMPAPER_A3_TRANSVERSE       67  /* A3 Transverse 297 x 420 mm         */
        // #define DMPAPER_A3_EXTRA_TRANSVERSE 68  /* A3 Extra Transverse 322 x 445 mm   */
        // #endif /* WINVER >= 0x0400 */

        // #if(WINVER >= 0x0500)
        // #define DMPAPER_DBL_JAPANESE_POSTCARD 69 /* Japanese Double Postcard 200 x 148 mm */
        // #define DMPAPER_A6                  70  /* A6 105 x 148 mm                 */
        // #define DMPAPER_JENV_KAKU2          71  /* Japanese Envelope Kaku #2       */
        // #define DMPAPER_JENV_KAKU3          72  /* Japanese Envelope Kaku #3       */
        // #define DMPAPER_JENV_CHOU3          73  /* Japanese Envelope Chou #3       */
        // #define DMPAPER_JENV_CHOU4          74  /* Japanese Envelope Chou #4       */
        // #define DMPAPER_LETTER_ROTATED      75  /* Letter Rotated 11 x 8 1/2 11 in */
        // #define DMPAPER_A3_ROTATED          76  /* A3 Rotated 420 x 297 mm         */
        // #define DMPAPER_A4_ROTATED          77  /* A4 Rotated 297 x 210 mm         */
        // #define DMPAPER_A5_ROTATED          78  /* A5 Rotated 210 x 148 mm         */
        // #define DMPAPER_B4_JIS_ROTATED      79  /* B4 (JIS) Rotated 364 x 257 mm   */
        // #define DMPAPER_B5_JIS_ROTATED      80  /* B5 (JIS) Rotated 257 x 182 mm   */
        // #define DMPAPER_JAPANESE_POSTCARD_ROTATED 81 /* Japanese Postcard Rotated 148 x 100 mm */
        // #define DMPAPER_DBL_JAPANESE_POSTCARD_ROTATED 82 /* Double Japanese Postcard Rotated 148 x 200 mm */
        // #define DMPAPER_A6_ROTATED          83  /* A6 Rotated 148 x 105 mm         */
        // #define DMPAPER_JENV_KAKU2_ROTATED  84  /* Japanese Envelope Kaku #2 Rotated */
        // #define DMPAPER_JENV_KAKU3_ROTATED  85  /* Japanese Envelope Kaku #3 Rotated */
        // #define DMPAPER_JENV_CHOU3_ROTATED  86  /* Japanese Envelope Chou #3 Rotated */
        // #define DMPAPER_JENV_CHOU4_ROTATED  87  /* Japanese Envelope Chou #4 Rotated */
        // #define DMPAPER_B6_JIS              88  /* B6 (JIS) 128 x 182 mm           */
        // #define DMPAPER_B6_JIS_ROTATED      89  /* B6 (JIS) Rotated 182 x 128 mm   */
        // #define DMPAPER_12X11               90  /* 12 x 11 in                      */
        // #define DMPAPER_JENV_YOU4           91  /* Japanese Envelope You #4        */
        // #define DMPAPER_JENV_YOU4_ROTATED   92  /* Japanese Envelope You #4 Rotated*/
        // #define DMPAPER_P16K                93  /* PRC 16K 146 x 215 mm            */
        // #define DMPAPER_P32K                94  /* PRC 32K 97 x 151 mm             */
        // #define DMPAPER_P32KBIG             95  /* PRC 32K(Big) 97 x 151 mm        */
        // #define DMPAPER_PENV_1              96  /* PRC Envelope #1 102 x 165 mm    */
        // #define DMPAPER_PENV_2              97  /* PRC Envelope #2 102 x 176 mm    */
        // #define DMPAPER_PENV_3              98  /* PRC Envelope #3 125 x 176 mm    */
        // #define DMPAPER_PENV_4              99  /* PRC Envelope #4 110 x 208 mm    */
        // #define DMPAPER_PENV_5              100 /* PRC Envelope #5 110 x 220 mm    */
        // #define DMPAPER_PENV_6              101 /* PRC Envelope #6 120 x 230 mm    */
        // #define DMPAPER_PENV_7              102 /* PRC Envelope #7 160 x 230 mm    */
        // #define DMPAPER_PENV_8              103 /* PRC Envelope #8 120 x 309 mm    */
        // #define DMPAPER_PENV_9              104 /* PRC Envelope #9 229 x 324 mm    */
        // #define DMPAPER_PENV_10             105 /* PRC Envelope #10 324 x 458 mm   */
        // #define DMPAPER_P16K_ROTATED        106 /* PRC 16K Rotated                 */
        // #define DMPAPER_P32K_ROTATED        107 /* PRC 32K Rotated                 */
        // #define DMPAPER_P32KBIG_ROTATED     108 /* PRC 32K(Big) Rotated            */
        // #define DMPAPER_PENV_1_ROTATED      109 /* PRC Envelope #1 Rotated 165 x 102 mm */
        // #define DMPAPER_PENV_2_ROTATED      110 /* PRC Envelope #2 Rotated 176 x 102 mm */
        // #define DMPAPER_PENV_3_ROTATED      111 /* PRC Envelope #3 Rotated 176 x 125 mm */
        // #define DMPAPER_PENV_4_ROTATED      112 /* PRC Envelope #4 Rotated 208 x 110 mm */
        // #define DMPAPER_PENV_5_ROTATED      113 /* PRC Envelope #5 Rotated 220 x 110 mm */
        // #define DMPAPER_PENV_6_ROTATED      114 /* PRC Envelope #6 Rotated 230 x 120 mm */
        // #define DMPAPER_PENV_7_ROTATED      115 /* PRC Envelope #7 Rotated 230 x 160 mm */
        // #define DMPAPER_PENV_8_ROTATED      116 /* PRC Envelope #8 Rotated 309 x 120 mm */
        // #define DMPAPER_PENV_9_ROTATED      117 /* PRC Envelope #9 Rotated 324 x 229 mm */
        // #define DMPAPER_PENV_10_ROTATED     118 /* PRC Envelope #10 Rotated 458 x 324 mm */
        // #endif /* WINVER >= 0x0500 */

        // #if (WINVER >= 0x0500)
        // #define DMPAPER_LAST                DMPAPER_PENV_10_ROTATED
        // #elif (WINVER >= 0x0400)
        // #define DMPAPER_LAST                DMPAPER_A3_EXTRA_TRANSVERSE
        // #Else
        // #define DMPAPER_LAST                DMPAPER_FANFOLD_LGL_GERMAN
        // #End If

        // #define DMPAPER_USER                256

        // /* bin selections */
        // #define DMBIN_FIRST         DMBIN_UPPER
        // #define DMBIN_UPPER         1
        // #define DMBIN_ONLYONE       1
        // #define DMBIN_LOWER         2
        // #define DMBIN_MIDDLE        3
        // #define DMBIN_MANUAL        4
        // #define DMBIN_ENVELOPE      5
        // #define DMBIN_ENVMANUAL     6
        // #define DMBIN_AUTO          7
        // #define DMBIN_TRACTOR       8
        // #define DMBIN_SMALLFMT      9
        // #define DMBIN_LARGEFMT      10
        // #define DMBIN_LARGECAPACITY 11
        // #define DMBIN_CASSETTE      14
        // #define DMBIN_FORMSOURCE    15
        // #define DMBIN_LAST          DMBIN_FORMSOURCE

        // #define DMBIN_USER          256     /* device specific bins start here */

        // /* print qualities */
        // #define DMRES_DRAFT         (-1)
        // #define DMRES_LOW           (-2)
        // #define DMRES_MEDIUM        (-3)
        // #define DMRES_HIGH          (-4)

        // /* color enable/disable for color printers */
        // #define DMCOLOR_MONOCHROME  1
        // #define DMCOLOR_COLOR       2

        // /* duplex enable */
        // #define DMDUP_SIMPLEX    1
        // #define DMDUP_VERTICAL   2
        // #define DMDUP_HORIZONTAL 3

        // /* TrueType options */
        // #define DMTT_BITMAP     1       /* print TT fonts as graphics */
        // #define DMTT_DOWNLOAD   2       /* download TT fonts as soft fonts */
        // #define DMTT_SUBDEV     3       /* substitute device fonts for TT fonts */
        // #if(WINVER >= 0x0400)
        // #define DMTT_DOWNLOAD_OUTLINE 4 /* download TT fonts as outline soft fonts */
        // #endif /* WINVER >= 0x0400 */

        // /* Collation selections */
        // #define DMCOLLATE_FALSE  0
        // #define DMCOLLATE_TRUE   1

        // #if(WINVER >= 0x0501)
        // /* DEVMODE dmDisplayOrientation specifiations */
        // #define DMDO_DEFAULT    0
        // #define DMDO_90         1
        // #define DMDO_180        2
        // #define DMDO_270        3

        // /* DEVMODE dmDisplayFixedOutput specifiations */
        // #define DMDFO_DEFAULT   0
        // #define DMDFO_STRETCH   1
        // #define DMDFO_CENTER    2
        // #endif /* WINVER >= 0x0501 */

        // /* DEVMODE dmDisplayFlags flags */

        // // #define DM_GRAYSCALE            0x00000001 /* This flag is no longer valid */
        // #define DM_INTERLACED           0x00000002
        // #define DMDISPLAYFLAGS_TEXTMODE 0x00000004

        // /* dmNup , multiple logical page per physical page options */
        // #define DMNUP_SYSTEM        1
        // #define DMNUP_ONEUP         2

        // #if(WINVER >= 0x0400)
        // /* ICM methods */
        // #define DMICMMETHOD_NONE    1   /* ICM disabled */
        // #define DMICMMETHOD_SYSTEM  2   /* ICM handled by system */
        // #define DMICMMETHOD_DRIVER  3   /* ICM handled by driver */
        // #define DMICMMETHOD_DEVICE  4   /* ICM handled by device */

        // #define DMICMMETHOD_USER  256   /* Device-specific methods start here */

        // /* ICM Intents */
        // #define DMICM_SATURATE          1   /* Maximize color saturation */
        // #define DMICM_CONTRAST          2   /* Maximize color contrast */
        // #define DMICM_COLORIMETRIC       3   /* Use specific color metric */
        // #define DMICM_ABS_COLORIMETRIC   4   /* Use specific color metric */

        // #define DMICM_USER        256   /* Device-specific intents start here */

        // /* Media types */

        // #define DMMEDIA_STANDARD      1   /* Standard paper */
        // #define DMMEDIA_TRANSPARENCY  2   /* Transparency */
        // #define DMMEDIA_GLOSSY        3   /* Glossy paper */

        // #define DMMEDIA_USER        256   /* Device-specific media start here */

        // /* Dither types */
        // #define DMDITHER_NONE       1      /* No dithering */
        // #define DMDITHER_COARSE     2      /* Dither with a coarse brush */
        // #define DMDITHER_FINE       3      /* Dither with a fine brush */
        // #define DMDITHER_LINEART    4      /* LineArt dithering */
        // #define DMDITHER_ERRORDIFFUSION 5  /* LineArt dithering */
        // #define DMDITHER_RESERVED6      6      /* LineArt dithering */
        // #define DMDITHER_RESERVED7      7      /* LineArt dithering */
        // #define DMDITHER_RESERVED8      8      /* LineArt dithering */
        // #define DMDITHER_RESERVED9      9      /* LineArt dithering */
        // #define DMDITHER_GRAYSCALE  10     /* Device does grayscaling */

        // #define DMDITHER_USER     256   /* Device-specific dithers start here */
        // #

        
        // DevMode extras

        public const int DMRES_DRAFT = -1;
        public const int DMRES_LOW = -2;
        public const int DMRES_MEDIUM = -3;
        public const int DMRES_HIGH = -4;

        //  field selection bits 
        public const int DM_ORIENTATION = 0x1;
        public const int DM_PAPERSIZE = 0x2;
        public const int DM_PAPERLENGTH = 0x4;
        public const int DM_PAPERWIDTH = 0x8;
        public const int DM_SCALE = 0x10;
        // if(WINVER >= 0x0500)
        public const int DM_POSITION = 0x20;
        public const int DM_NUP = 0x40;
        // endif ''  WINVER >= 0x0500 
        // if(WINVER >= 0x0501)
        public const int DM_DISPLAYORIENTATION = 0x80;
        // endif ''  WINVER >= 0x0501 
        public const int DM_COPIES = 0x100;
        public const int DM_DEFAULTSOURCE = 0x200;
        public const int DM_PRINTQUALITY = 0x400;
        public const int DM_COLOR = 0x800;
        public const int DM_DUPLEX = 0x1000;
        public const int DM_YRESOLUTION = 0x2000;
        public const int DM_TTOPTION = 0x4000;
        public const int DM_COLLATE = 0x8000;
        public const int DM_FORMNAME = 0x10000;
        public const int DM_LOGPIXELS = 0x20000;
        public const int DM_BITSPERPEL = 0x40000;
        public const int DM_PELSWIDTH = 0x80000;
        public const int DM_PELSHEIGHT = 0x100000;
        public const int DM_DISPLAYFLAGS = 0x200000;
        public const int DM_DISPLAYFREQUENCY = 0x400000;
        // if(WINVER >= 0x0400)
        public const int DM_ICMMETHOD = 0x800000;
        public const int DM_ICMINTENT = 0x1000000;
        public const int DM_MEDIATYPE = 0x2000000;
        public const int DM_DITHERTYPE = 0x4000000;
        public const int DM_PANNINGWIDTH = 0x8000000;
        public const int DM_PANNINGHEIGHT = 0x10000000;
        // endif ''  WINVER >= 0x0400 
        // if(WINVER >= 0x0501)
        public const int DM_DISPLAYFIXEDOUTPUT = 0x20000000;
        // endif ''  WINVER >= 0x0501 


        //  orientation selections 
        public const int DMORIENT_PORTRAIT = 1;
        public const int DMORIENT_LANDSCAPE = 2; //  paper selections 
        public const int DMPAPER_FIRST = DMPAPER_LETTER;
        public const int DMPAPER_LETTER = 1; //  Letter 8 1/2 x 11 in               
        public const int DMPAPER_LETTERSMALL = 2; //  Letter Small 8 1/2 x 11 in         
        public const int DMPAPER_TABLOID = 3; //  Tabloid 11 x 17 in                 
        public const int DMPAPER_LEDGER = 4; //  Ledger 17 x 11 in                  
        public const int DMPAPER_LEGAL = 5; //  Legal 8 1/2 x 14 in                
        public const int DMPAPER_STATEMENT = 6; //  Statement 5 1/2 x 8 1/2 in         
        public const int DMPAPER_EXECUTIVE = 7; //  Executive 7 1/4 x 10 1/2 in        
        public const int DMPAPER_A3 = 8; //  A3 297 x 420 mm                    
        public const int DMPAPER_A4 = 9; //  A4 210 x 297 mm                    
        public const int DMPAPER_A4SMALL = 10; //  A4 Small 210 x 297 mm              
        public const int DMPAPER_A5 = 11; //  A5 148 x 210 mm                    
        public const int DMPAPER_B4 = 12; //  B4 (JIS) 250 x 354                 
        public const int DMPAPER_B5 = 13; //  B5 (JIS) 182 x 257 mm              
        public const int DMPAPER_FOLIO = 14; //  Folio 8 1/2 x 13 in                
        public const int DMPAPER_QUARTO = 15; //  Quarto 215 x 275 mm                
        public const int DMPAPER_10X14 = 16; //  10x14 in                           
        public const int DMPAPER_11X17 = 17; //  11x17 in                           
        public const int DMPAPER_NOTE = 18; //  Note 8 1/2 x 11 in                 
        public const int DMPAPER_ENV_9 = 19; //  Envelope #9 3 7/8 x 8 7/8          
        public const int DMPAPER_ENV_10 = 20; //  Envelope #10 4 1/8 x 9 1/2         
        public const int DMPAPER_ENV_11 = 21; //  Envelope #11 4 1/2 x 10 3/8        
        public const int DMPAPER_ENV_12 = 22; //  Envelope #12 4 \276 x 11           
        public const int DMPAPER_ENV_14 = 23; //  Envelope #14 5 x 11 1/2            
        public const int DMPAPER_CSHEET = 24; //  C size sheet                       
        public const int DMPAPER_DSHEET = 25; //  D size sheet                       
        public const int DMPAPER_ESHEET = 26; //  E size sheet                       
        public const int DMPAPER_ENV_DL = 27; //  Envelope DL 110 x 220mm            
        public const int DMPAPER_ENV_C5 = 28; //  Envelope C5 162 x 229 mm           
        public const int DMPAPER_ENV_C3 = 29; //  Envelope C3  324 x 458 mm          
        public const int DMPAPER_ENV_C4 = 30; //  Envelope C4  229 x 324 mm          
        public const int DMPAPER_ENV_C6 = 31; //  Envelope C6  114 x 162 mm          
        public const int DMPAPER_ENV_C65 = 32; //  Envelope C65 114 x 229 mm          
        public const int DMPAPER_ENV_B4 = 33; //  Envelope B4  250 x 353 mm          
        public const int DMPAPER_ENV_B5 = 34; //  Envelope B5  176 x 250 mm          
        public const int DMPAPER_ENV_B6 = 35; //  Envelope B6  176 x 125 mm          
        public const int DMPAPER_ENV_ITALY = 36; //  Envelope 110 x 230 mm              
        public const int DMPAPER_ENV_MONARCH = 37; //  Envelope Monarch 3.875 x 7.5 in    
        public const int DMPAPER_ENV_PERSONAL = 38; //  6 3/4 Envelope 3 5/8 x 6 1/2 in    
        public const int DMPAPER_FANFOLD_US = 39; //  US Std Fanfold 14 7/8 x 11 in      
        public const int DMPAPER_FANFOLD_STD_GERMAN = 40; //  German Std Fanfold 8 1/2 x 12 in   
        public const int DMPAPER_FANFOLD_LGL_GERMAN = 41; //  German Legal Fanfold 8 1/2 x 13 in 
        // if(WINVER >= 0x0400)
        public const int DMPAPER_ISO_B4 = 42; //  B4 (ISO) 250 x 353 mm              
        public const int DMPAPER_JAPANESE_POSTCARD = 43; //  Japanese Postcard 100 x 148 mm     
        public const int DMPAPER_9X11 = 44; //  9 x 11 in                          
        public const int DMPAPER_10X11 = 45; //  10 x 11 in                         
        public const int DMPAPER_15X11 = 46; //  15 x 11 in                         
        public const int DMPAPER_ENV_INVITE = 47; //  Envelope Invite 220 x 220 mm       
        public const int DMPAPER_RESERVED_48 = 48; //  RESERVED--DO NOT USE               
        public const int DMPAPER_RESERVED_49 = 49; //  RESERVED--DO NOT USE               
        public const int DMPAPER_LETTER_EXTRA = 50; //  Letter Extra 9 \275 x 12 in        
        public const int DMPAPER_LEGAL_EXTRA = 51; //  Legal Extra 9 \275 x 15 in         
        public const int DMPAPER_TABLOID_EXTRA = 52; //  Tabloid Extra 11.69 x 18 in        
        public const int DMPAPER_A4_EXTRA = 53; //  A4 Extra 9.27 x 12.69 in           
        public const int DMPAPER_LETTER_TRANSVERSE = 54; //  Letter Transverse 8 \275 x 11 in   
        public const int DMPAPER_A4_TRANSVERSE = 55; //  A4 Transverse 210 x 297 mm         
        public const int DMPAPER_LETTER_EXTRA_TRANSVERSE = 56; //  Letter Extra Transverse 9\275 x 12 in 
        public const int DMPAPER_A_PLUS = 57; //  SuperA/SuperA/A4 227 x 356 mm      
        public const int DMPAPER_B_PLUS = 58; //  SuperB/SuperB/A3 305 x 487 mm      
        public const int DMPAPER_LETTER_PLUS = 59; //  Letter Plus 8.5 x 12.69 in         
        public const int DMPAPER_A4_PLUS = 60; //  A4 Plus 210 x 330 mm               
        public const int DMPAPER_A5_TRANSVERSE = 61; //  A5 Transverse 148 x 210 mm         
        public const int DMPAPER_B5_TRANSVERSE = 62; //  B5 (JIS) Transverse 182 x 257 mm   
        public const int DMPAPER_A3_EXTRA = 63; //  A3 Extra 322 x 445 mm              
        public const int DMPAPER_A5_EXTRA = 64; //  A5 Extra 174 x 235 mm              
        public const int DMPAPER_B5_EXTRA = 65; //  B5 (ISO) Extra 201 x 276 mm        
        public const int DMPAPER_A2 = 66; //  A2 420 x 594 mm                    
        public const int DMPAPER_A3_TRANSVERSE = 67; //  A3 Transverse 297 x 420 mm         
        public const int DMPAPER_A3_EXTRA_TRANSVERSE = 68; //  A3 Extra Transverse 322 x 445 mm   
        // endif ''  WINVER >= 0x0400 

        // if(WINVER >= 0x0500)
        public const int DMPAPER_DBL_JAPANESE_POSTCARD = 69; //  Japanese Double Postcard 200 x 148 mm 
        public const int DMPAPER_A6 = 70; //  A6 105 x 148 mm                 
        public const int DMPAPER_JENV_KAKU2 = 71; //  Japanese Envelope Kaku #2       
        public const int DMPAPER_JENV_KAKU3 = 72; //  Japanese Envelope Kaku #3       
        public const int DMPAPER_JENV_CHOU3 = 73; //  Japanese Envelope Chou #3       
        public const int DMPAPER_JENV_CHOU4 = 74; //  Japanese Envelope Chou #4       
        public const int DMPAPER_LETTER_ROTATED = 75; //  Letter Rotated 11 x 8 1/2 11 in 
        public const int DMPAPER_A3_ROTATED = 76; //  A3 Rotated 420 x 297 mm         
        public const int DMPAPER_A4_ROTATED = 77; //  A4 Rotated 297 x 210 mm         
        public const int DMPAPER_A5_ROTATED = 78; //  A5 Rotated 210 x 148 mm         
        public const int DMPAPER_B4_JIS_ROTATED = 79; //  B4 (JIS) Rotated 364 x 257 mm   
        public const int DMPAPER_B5_JIS_ROTATED = 80; //  B5 (JIS) Rotated 257 x 182 mm   
        public const int DMPAPER_JAPANESE_POSTCARD_ROTATED = 81; //  Japanese Postcard Rotated 148 x 100 mm 
        public const int DMPAPER_DBL_JAPANESE_POSTCARD_ROTATED = 82; //  Double Japanese Postcard Rotated 148 x 200 mm 
        public const int DMPAPER_A6_ROTATED = 83; //  A6 Rotated 148 x 105 mm         
        public const int DMPAPER_JENV_KAKU2_ROTATED = 84; //  Japanese Envelope Kaku #2 Rotated 
        public const int DMPAPER_JENV_KAKU3_ROTATED = 85; //  Japanese Envelope Kaku #3 Rotated 
        public const int DMPAPER_JENV_CHOU3_ROTATED = 86; //  Japanese Envelope Chou #3 Rotated 
        public const int DMPAPER_JENV_CHOU4_ROTATED = 87; //  Japanese Envelope Chou #4 Rotated 
        public const int DMPAPER_B6_JIS = 88; //  B6 (JIS) 128 x 182 mm           
        public const int DMPAPER_B6_JIS_ROTATED = 89; //  B6 (JIS) Rotated 182 x 128 mm   
        public const int DMPAPER_12X11 = 90; //  12 x 11 in                      
        public const int DMPAPER_JENV_YOU4 = 91; //  Japanese Envelope You #4        
        public const int DMPAPER_JENV_YOU4_ROTATED = 92; //  Japanese Envelope You #4 Rotated
        public const int DMPAPER_P16K = 93; //  PRC 16K 146 x 215 mm            
        public const int DMPAPER_P32K = 94; //  PRC 32K 97 x 151 mm             
        public const int DMPAPER_P32KBIG = 95; //  PRC 32K(Big) 97 x 151 mm        
        public const int DMPAPER_PENV_1 = 96; //  PRC Envelope #1 102 x 165 mm    
        public const int DMPAPER_PENV_2 = 97; //  PRC Envelope #2 102 x 176 mm    
        public const int DMPAPER_PENV_3 = 98; //  PRC Envelope #3 125 x 176 mm    
        public const int DMPAPER_PENV_4 = 99; //  PRC Envelope #4 110 x 208 mm    
        public const int DMPAPER_PENV_5 = 100; //  PRC Envelope #5 110 x 220 mm    
        public const int DMPAPER_PENV_6 = 101; //  PRC Envelope #6 120 x 230 mm    
        public const int DMPAPER_PENV_7 = 102; //  PRC Envelope #7 160 x 230 mm    
        public const int DMPAPER_PENV_8 = 103; //  PRC Envelope #8 120 x 309 mm    
        public const int DMPAPER_PENV_9 = 104; //  PRC Envelope #9 229 x 324 mm    
        public const int DMPAPER_PENV_10 = 105; //  PRC Envelope #10 324 x 458 mm   
        public const int DMPAPER_P16K_ROTATED = 106; //  PRC 16K Rotated                 
        public const int DMPAPER_P32K_ROTATED = 107; //  PRC 32K Rotated                 
        public const int DMPAPER_P32KBIG_ROTATED = 108; //  PRC 32K(Big) Rotated            
        public const int DMPAPER_PENV_1_ROTATED = 109; //  PRC Envelope #1 Rotated 165 x 102 mm 
        public const int DMPAPER_PENV_2_ROTATED = 110; //  PRC Envelope #2 Rotated 176 x 102 mm 
        public const int DMPAPER_PENV_3_ROTATED = 111; //  PRC Envelope #3 Rotated 176 x 125 mm 
        public const int DMPAPER_PENV_4_ROTATED = 112; //  PRC Envelope #4 Rotated 208 x 110 mm 
        public const int DMPAPER_PENV_5_ROTATED = 113; //  PRC Envelope #5 Rotated 220 x 110 mm 
        public const int DMPAPER_PENV_6_ROTATED = 114; //  PRC Envelope #6 Rotated 230 x 120 mm 
        public const int DMPAPER_PENV_7_ROTATED = 115; //  PRC Envelope #7 Rotated 230 x 160 mm 
        public const int DMPAPER_PENV_8_ROTATED = 116; //  PRC Envelope #8 Rotated 309 x 120 mm 
        public const int DMPAPER_PENV_9_ROTATED = 117; //  PRC Envelope #9 Rotated 324 x 229 mm 
        public const int DMPAPER_PENV_10_ROTATED = 118; //  PRC Envelope #10 Rotated 458 x 324 mm 
        public const int DMPAPER_LAST = DMPAPER_PENV_10_ROTATED;
        public const int DMPAPER_USER = 256; //  bin selections 
        public const int DMBIN_FIRST = DMBIN_UPPER;
        public const int DMBIN_UPPER = 1;
        public const int DMBIN_ONLYONE = 1;
        public const int DMBIN_LOWER = 2;
        public const int DMBIN_MIDDLE = 3;
        public const int DMBIN_MANUAL = 4;
        public const int DMBIN_ENVELOPE = 5;
        public const int DMBIN_ENVMANUAL = 6;
        public const int DMBIN_AUTO = 7;
        public const int DMBIN_TRACTOR = 8;
        public const int DMBIN_SMALLFMT = 9;
        public const int DMBIN_LARGEFMT = 10;
        public const int DMBIN_LARGECAPACITY = 11;
        public const int DMBIN_CASSETTE = 14;
        public const int DMBIN_FORMSOURCE = 15;
        public const int DMBIN_LAST = DMBIN_FORMSOURCE;
        public const int DMBIN_USER = 256; //  device specific bins start here 

        //  print qualities 
        // define DMRES_DRAFT         (-1)
        // define DMRES_LOW           (-2)
        // define DMRES_MEDIUM        (-3)
        // define DMRES_HIGH          (-4)

        //  color enable/disable for color printers 
        public const int DMCOLOR_MONOCHROME = 1;
        public const int DMCOLOR_COLOR = 2; //  duplex enable 
        public const int DMDUP_SIMPLEX = 1;
        public const int DMDUP_VERTICAL = 2;
        public const int DMDUP_HORIZONTAL = 3; //  TrueType options 
        public const int DMTT_BITMAP = 1; //  print TT fonts as graphics 
        public const int DMTT_DOWNLOAD = 2; //  download TT fonts as soft fonts 
        public const int DMTT_SUBDEV = 3; //  substitute device fonts for TT fonts 
        // if(WINVER >= 0x0400)
        public const int DMTT_DOWNLOAD_OUTLINE = 4; //  download TT fonts as outline soft fonts 
        // endif ''  WINVER >= 0x0400 

        //  Collation selections 
        public const int DMCOLLATE_FALSE = 0;
        public const int DMCOLLATE_TRUE = 1;

        // if(WINVER >= 0x0501)
        //  DEVMODE dmDisplayOrientation specifiations 
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3; //  DEVMODE dmDisplayFixedOutput specifiations 
        public const int DMDFO_DEFAULT = 0;
        public const int DMDFO_STRETCH = 1;
        public const int DMDFO_CENTER = 2;
        // endif ''  WINVER >= 0x0501 

        //  DEVMODE dmDisplayFlags flags 

        // Public Const DM_GRAYSCALE = 0x00000001 ''  This flag is no longer valid 
        public const int DM_INTERLACED = 0x2;
        public const int DMDISPLAYFLAGS_TEXTMODE = 0x4; //  dmNup , multiple logical page per physical page options 
        public const int DMNUP_SYSTEM = 1;
        public const int DMNUP_ONEUP = 2;

        // if(WINVER >= 0x0400)
        //  ICM methods 
        public const int DMICMMETHOD_NONE = 1; //  ICM disabled 
        public const int DMICMMETHOD_SYSTEM = 2; //  ICM handled by system 
        public const int DMICMMETHOD_DRIVER = 3; //  ICM handled by driver 
        public const int DMICMMETHOD_DEVICE = 4; //  ICM handled by device 
        public const int DMICMMETHOD_USER = 256; //  Device-specific methods start here 

        //  ICM Intents 
        public const int DMICM_SATURATE = 1; //  Maximize color saturation 
        public const int DMICM_CONTRAST = 2; //  Maximize color contrast 
        public const int DMICM_COLORIMETRIC = 3; //  Use specific color metric 
        public const int DMICM_ABS_COLORIMETRIC = 4; //  Use specific color metric 
        public const int DMICM_USER = 256; //  Device-specific intents start here 

        //  Media types 

        public const int DMMEDIA_STANDARD = 1; //  Standard paper 
        public const int DMMEDIA_TRANSPARENCY = 2; //  Transparency 
        public const int DMMEDIA_GLOSSY = 3; //  Glossy paper 
        public const int DMMEDIA_USER = 256; //  Device-specific media start here 

        //  Dither types 
        public const int DMDITHER_NONE = 1; //  No dithering 
        public const int DMDITHER_COARSE = 2; //  Dither with a coarse brush 
        public const int DMDITHER_FINE = 3; //  Dither with a fine brush 
        public const int DMDITHER_LINEART = 4; //  LineArt dithering 
        public const int DMDITHER_ERRORDIFFUSION = 5; //  LineArt dithering 
        public const int DMDITHER_RESERVED6 = 6; //  LineArt dithering 
        public const int DMDITHER_RESERVED7 = 7; //  LineArt dithering 
        public const int DMDITHER_RESERVED8 = 8; //  LineArt dithering 
        public const int DMDITHER_RESERVED9 = 9; //  LineArt dithering 
        public const int DMDITHER_GRAYSCALE = 10; //  Device does grayscaling 
        public const int DMDITHER_USER = 256; //  Device-specific dithers start here 
        // endif ''  WINVER >= 0x0400 



        // DevCaps

        public const int DRIVERVERSION = 0; //  Device driver version
        public const int TECHNOLOGY = 2; //  Device classification
        public const int HORZSIZE = 4; //  Horizontal size in millimeters
        public const int VERTSIZE = 6; //  Vertical size in millimeters
        public const int HORZRES = 8; //  Horizontal width in pixels
        public const int VERTRES = 10; //  Vertical height in pixels
        public const int BITSPIXEL = 12; //  Number of bits per pixel
        public const int PLANES = 14; //  Number of planes
        public const int NUMBRUSHES = 16; //  Number of brushes the device has
        public const int NUMPENS = 18; //  Number of pens the device has
        public const int NUMMARKERS = 20; //  Number of markers the device has
        public const int NUMFONTS = 22; //  Number of fonts the device has
        public const int NUMCOLORS = 24; //  Number of colors the device supports
        public const int PDEVICESIZE = 26; //  Size required for device descriptor
        public const int CURVECAPS = 28; //  Curve capabilities
        public const int LINECAPS = 30; //  Line capabilities
        public const int POLYGONALCAPS = 32; //  Polygonal capabilities
        public const int TEXTCAPS = 34; //  Text capabilities
        public const int CLIPCAPS = 36; //  Clipping capabilities
        public const int RASTERCAPS = 38; //  Bitblt capabilities
        public const int ASPECTX = 40; //  Length of the X leg
        public const int ASPECTY = 42; //  Length of the Y leg
        public const int ASPECTXY = 44; //  Length of the hypotenuse
        public const int LOGPIXELSX = 88; //  Logical pixels/inch in X
        public const int LOGPIXELSY = 90; //  Logical pixels/inch in Y
        public const int SIZEPALETTE = 104; //  Number of entries in physical palette
        public const int NUMRESERVED = 106; //  Number of reserved entries in palette
        public const int COLORRES = 108; //  Actual color resolution

        // Printing related DeviceCaps. These replace the appropriate Escapes

        public const int PHYSICALWIDTH = 110; //  Physical Width in device units
        public const int PHYSICALHEIGHT = 111; //  Physical Height in device units
        public const int PHYSICALOFFSETX = 112; //  Physical Printable Area x margin
        public const int PHYSICALOFFSETY = 113; //  Physical Printable Area y margin
        public const int SCALINGFACTORX = 114; //  Scaling factor x
        public const int SCALINGFACTORY = 115; //  Scaling factor y

        // Display driver specific

        public const int VREFRESH = 116; //  Current vertical refresh rate of the
        //  display device (for displays only) in Hz
        public const int DESKTOPVERTRES = 117; //  Horizontal width of entire desktop in
        //  pixels
        public const int DESKTOPHORZRES = 118; //  Vertical height of entire desktop in
        //  pixels
        public const int BLTALIGNMENT = 119; //  Preferred blt alignment

        //if(WINVER >= 0x0500)
        public const int SHADEBLENDCAPS = 120; //  Shading and blending caps
        public const int COLORMGMTCAPS = 121; //  Color Management caps
        //endif ''  WINVER >= 0x0500

        //ifndef NOGDICAPMASKS

        //  Device Capability Masks:

        //  Device Technologies
        public const int DT_PLOTTER = 0; //  Vector plotter
        public const int DT_RASDISPLAY = 1; //  Raster display
        public const int DT_RASPRINTER = 2; //  Raster printer
        public const int DT_RASCAMERA = 3; //  Raster camera
        public const int DT_CHARSTREAM = 4; //  Character-stream, PLP
        public const int DT_METAFILE = 5; //  Metafile, VDM
        public const int DT_DISPFILE = 6; //  Display-file

        // Device Capabilities

        public const ushort DC_FIELDS = 1;
        public const ushort DC_PAPERS = 2;
        public const ushort DC_PAPERSIZE = 3;
        public const ushort DC_MINEXTENT = 4;
        public const ushort DC_MAXEXTENT = 5;
        public const ushort DC_BINS = 6;
        public const ushort DC_DUPLEX = 7;
        public const ushort DC_SIZE = 8;
        public const ushort DC_EXTRA = 9;
        public const ushort DC_VERSION = 10;
        public const ushort DC_DRIVER = 11;
        public const ushort DC_BINNAMES = 12;
        public const ushort DC_ENUMRESOLUTIONS = 13;
        public const ushort DC_FILEDEPENDENCIES = 14;
        public const ushort DC_TRUETYPE = 15;
        public const ushort DC_PAPERNAMES = 16;
        public const ushort DC_ORIENTATION = 17;
        public const ushort DC_COPIES = 18;
        public const ushort DC_BINADJUST = 19;
        public const ushort DC_EMF_COMPLIANT = 20;
        public const ushort DC_DATATYPE_PRODUCED = 21;
        public const ushort DC_COLLATE = 22;
        public const ushort DC_MANUFACTURER = 23;
        public const ushort DC_MODEL = 24;
        public const ushort DC_PERSONALITY = 25;
        public const ushort DC_PRINTRATE = 26;
        public const ushort DC_PRINTRATEUNIT = 27;
        public const ushort PRINTRATEUNIT_PPM = 1;
        public const ushort PRINTRATEUNIT_CPS = 2;
        public const ushort PRINTRATEUNIT_LPM = 3;
        public const ushort PRINTRATEUNIT_IPM = 4;
        public const ushort DC_PRINTERMEM = 28;
        public const ushort DC_MEDIAREADY = 29;
        public const ushort DC_STAPLE = 30;
        public const ushort DC_PRINTRATEPPM = 31;
        public const ushort DC_COLORDEVICE = 32;
        public const ushort DC_NUP = 33;
        public const ushort DC_MEDIATYPENAMES = 34;
        public const ushort DC_MEDIATYPES = 35;

        // Printer

        public const int PRINTER_CONTROL_PAUSE = 1;
        public const int PRINTER_CONTROL_RESUME = 2;
        public const int PRINTER_CONTROL_PURGE = 3;
        public const int PRINTER_CONTROL_SET_STATUS = 4;
        public const int PRINTER_STATUS_PAUSED = 0x1;
        public const int PRINTER_STATUS_ERROR = 0x2;
        public const int PRINTER_STATUS_PENDING_DELETION = 0x4;
        public const int PRINTER_STATUS_PAPER_JAM = 0x8;
        public const int PRINTER_STATUS_PAPER_OUT = 0x10;
        public const int PRINTER_STATUS_MANUAL_FEED = 0x20;
        public const int PRINTER_STATUS_PAPER_PROBLEM = 0x40;
        public const int PRINTER_STATUS_OFFLINE = 0x80;
        public const int PRINTER_STATUS_IO_ACTIVE = 0x100;
        public const int PRINTER_STATUS_BUSY = 0x200;
        public const int PRINTER_STATUS_PRINTING = 0x400;
        public const int PRINTER_STATUS_OUTPUT_BIN_FULL = 0x800;
        public const int PRINTER_STATUS_NOT_AVAILABLE = 0x1000;
        public const int PRINTER_STATUS_WAITING = 0x2000;
        public const int PRINTER_STATUS_PROCESSING = 0x4000;
        public const int PRINTER_STATUS_INITIALIZING = 0x8000;
        public const int PRINTER_STATUS_WARMING_UP = 0x10000;
        public const int PRINTER_STATUS_TONER_LOW = 0x20000;
        public const int PRINTER_STATUS_NO_TONER = 0x40000;
        public const int PRINTER_STATUS_PAGE_PUNT = 0x80000;
        public const int PRINTER_STATUS_USER_INTERVENTION = 0x100000;
        public const int PRINTER_STATUS_OUT_OF_MEMORY = 0x200000;
        public const int PRINTER_STATUS_DOOR_OPEN = 0x400000;
        public const int PRINTER_STATUS_SERVER_UNKNOWN = 0x800000;
        public const int PRINTER_STATUS_POWER_SAVE = 0x1000000;
        public const int PRINTER_STATUS_SERVER_OFFLINE = 0x2000000;
        public const int PRINTER_STATUS_DRIVER_UPDATE_NEEDED = 0x4000000;
        public const int PRINTER_ATTRIBUTE_QUEUED = 0x1;
        public const int PRINTER_ATTRIBUTE_DIRECT = 0x2;
        public const int PRINTER_ATTRIBUTE_DEFAULT = 0x4;
        public const int PRINTER_ATTRIBUTE_SHARED = 0x8;
        public const int PRINTER_ATTRIBUTE_NETWORK = 0x10;
        public const int PRINTER_ATTRIBUTE_HIDDEN = 0x20;
        public const int PRINTER_ATTRIBUTE_LOCAL = 0x40;
        public const int PRINTER_ATTRIBUTE_ENABLE_DEVQ = 0x80;
        public const int PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS = 0x100;
        public const int PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST = 0x200;
        public const int PRINTER_ATTRIBUTE_WORK_OFFLINE = 0x400;
        public const int PRINTER_ATTRIBUTE_ENABLE_BIDI = 0x800;
        public const int PRINTER_ATTRIBUTE_RAW_ONLY = 0x1000;
        public const int PRINTER_ATTRIBUTE_PUBLISHED = 0x2000;
        public const int PRINTER_ENUM_DEFAULT = 0x1;
        public const int PRINTER_ENUM_LOCAL = 0x2;
        public const int PRINTER_ENUM_CONNECTIONS = 0x4;
        public const int PRINTER_ENUM_FAVORITE = 0x4;
        public const int PRINTER_ENUM_NAME = 0x8;
        public const int PRINTER_ENUM_REMOTE = 0x10;
        public const int PRINTER_ENUM_SHARED = 0x20;
        public const int PRINTER_ENUM_NETWORK = 0x40;
        public const int PRINTER_ENUM_EXPAND = 0x4000;
        public const int PRINTER_ENUM_CONTAINER = 0x8000;
        public const int PRINTER_ENUM_ICONMASK = 0xFF0000;
        public const int PRINTER_ENUM_ICON1 = 0x10000;
        public const int PRINTER_ENUM_ICON2 = 0x20000;
        public const int PRINTER_ENUM_ICON3 = 0x40000;
        public const int PRINTER_ENUM_ICON4 = 0x80000;
        public const int PRINTER_ENUM_ICON5 = 0x100000;
        public const int PRINTER_ENUM_ICON6 = 0x200000;
        public const int PRINTER_ENUM_ICON7 = 0x400000;
        public const int PRINTER_ENUM_ICON8 = 0x800000;
        public const int PRINTER_ENUM_HIDE = 0x1000000;


        
        
        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "OpenPrinterW")]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPWStr)] string pPrinterName, ref IntPtr hPrinter, IntPtr pDefault);
        [DllImport("winspool.drv")]
        public static extern bool ClosePrinter(IntPtr hPrinter);
        [DllImport("winspool.drv", EntryPoint = "GetPrinterW")]
        public static extern bool GetPrinter(IntPtr hPrinter, uint level, IntPtr pPrinter, uint cbBuf, ref uint pcbNeeded);
        [DllImport("winspool.drv", EntryPoint = "GetJobW")]
        public static extern bool GetJob(IntPtr hPrinter, uint JobId, uint Lovel, IntPtr pJob, uint cbBuf, ref uint pcbNeeded);
        [DllImport("winspool.drv", CharSet = CharSet.Unicode, EntryPoint = "DeviceCapabilitiesW")]
        public static extern uint DeviceCapabilities([MarshalAs(UnmanagedType.LPWStr)] string pDevice, [MarshalAs(UnmanagedType.LPWStr)] string pPort, ushort fwCapability, IntPtr pOutput, IntPtr pDevMode);
        [DllImport("winspool.drv", CharSet = CharSet.Unicode, EntryPoint = "EnumPrintersW")]
        public static extern bool EnumPrinters(uint Flags, [MarshalAs(UnmanagedType.LPWStr)] string Name, uint Level, IntPtr pPrinterEnum, uint cbBuf, ref uint pcbNeeded, ref uint pcbReturned);
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);


        [DllImport("winspool.drv", CharSet = CharSet.Unicode, EntryPoint = "EnumPrinterKeyW")]
        public static extern uint EnumPrinterKey(IntPtr hPrinter, [MarshalAs(UnmanagedType.LPWStr)] string pKeyName, IntPtr pSubkey, uint cbSubKey, ref uint pcbSubkey);

        //public static LinearSize GetMaxDPIForPrinter(PrinterObject printer, IntPtr hPrinter)
        //{
        //    uint cb = 0U;
        //    uint r = 0U;

        //    var mm = new MemPtr();
        //    var l = new List<PrinterObject>();

        //    PrinterObject pn;

        //    IntPtr cpx;

        //    EnumPrinters(6U, "", 2U, IntPtr.Zero, 0U, ref cb, ref r);

        //    if (cb != 0L)
        //    {
        //        mm.Alloc(cb);

        //        EnumPrinters(6U, "", 2U, mm, cb, ref cb, ref r);

        //        cpx = mm;

        //        for (int i = 1; i <= r; i++)
        //        {
        //            pn = new PrinterObject(cpx, i == 1);
                   
        //            cpx = cpx + 136;

        //            l.Add(pn);
        //        }
        //    }

        //    return default;
        //}

        
    }
}
