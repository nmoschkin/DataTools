// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NativeShell
//         Wrappers for native and COM shell interfaces.
//
// Some enum documentation copied from the MSDN (and in some cases, updated).
// Some classes and interfaces were ported from the WindowsAPICodePack.
// 
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

//using DataTools.Hardware.MessageResources;
//using DataTools.Hardware;
//using DataTools.Hardware.Native;

namespace DataTools.Shell.Native
{
    public sealed class KnownFolderIds
    {

        /// <summary>
        /// Computer
        /// </summary>
        public static readonly Guid Computer = new Guid(0xAC0837C, 0xBBF8, 0x452A, 0x85, 0xD, 0x79, 0xD0, 0x8E, 0x66, 0x7C, 0xA7);

        /// <summary>
        /// Conflicts
        /// </summary>
        public static readonly Guid Conflict = new Guid(0x4BFEFB45, 0x347D, 0x4006, 0xA5, 0xBE, 0xAC, 0xC, 0xB0, 0x56, 0x71, 0x92);

        /// <summary>
        /// Control Panel
        /// </summary>
        public static readonly Guid ControlPanel = new Guid(0x82A74AEB, 0xAEB4, 0x465C, 0xA0, 0x14, 0xD0, 0x97, 0xEE, 0x34, 0x6D, 0x63);

        /// <summary>
        /// Desktop
        /// </summary>
        public static readonly Guid Desktop = new Guid(0xB4BFCC3A, 0xDB2C, 0x424C, 0xB0, 0x29, 0x7F, 0xE9, 0x9A, 0x87, 0xC6, 0x41);

        /// <summary>
        /// Internet Explorer
        /// </summary>
        public static readonly Guid Internet = new Guid(0x4D9F7874, 0x4E0C, 0x4904, 0x96, 0x7B, 0x40, 0xB0, 0xD2, 0xC, 0x3E, 0x4B);

        /// <summary>
        /// Network
        /// </summary>
        public static readonly Guid Network = new Guid(0xD20BEEC4, 0x5CA8, 0x4905, 0xAE, 0x3B, 0xBF, 0x25, 0x1E, 0xA0, 0x9B, 0x53);

        /// <summary>
        /// Printers
        /// </summary>
        public static readonly Guid Printers = new Guid(0x76FC4E2D, 0xD6AD, 0x4519, 0xA6, 0x63, 0x37, 0xBD, 0x56, 0x6, 0x81, 0x85);

        /// <summary>
        /// Sync Center
        /// </summary>
        public static readonly Guid SyncManager = new Guid(0x43668BF8, 0xC14E, 0x49B2, 0x97, 0xC9, 0x74, 0x77, 0x84, 0xD7, 0x84, 0xB7);

        /// <summary>
        /// Network Connections
        /// </summary>
        public static readonly Guid Connections = new Guid(0x6F0CD92B, 0x2E97, 0x45D1, 0x88, 0xFF, 0xB0, 0xD1, 0x86, 0xB8, 0xDE, 0xDD);

        /// <summary>
        /// Sync Setup
        /// </summary>
        public static readonly Guid SyncSetup = new Guid(0xF214138, 0xB1D3, 0x4A90, 0xBB, 0xA9, 0x27, 0xCB, 0xC0, 0xC5, 0x38, 0x9A);

        /// <summary>
        /// Sync Results
        /// </summary>
        public static readonly Guid SyncResults = new Guid(0x289A9A43, 0xBE44, 0x4057, 0xA4, 0x1B, 0x58, 0x7A, 0x76, 0xD7, 0xE7, 0xF9);

        /// <summary>
        /// Recycle Bin
        /// </summary>
        public static readonly Guid RecycleBin = new Guid(0xB7534046, 0x3ECB, 0x4C18, 0xBE, 0x4E, 0x64, 0xCD, 0x4C, 0xB7, 0xD6, 0xAC);

        /// <summary>
        /// Fonts
        /// </summary>
        public static readonly Guid Fonts = new Guid(0xFD228CB7, 0xAE11, 0x4AE3, 0x86, 0x4C, 0x16, 0xF3, 0x91, 0xA, 0xB8, 0xFE);

        /// <summary>
        /// Startup
        /// </summary>
        public static readonly Guid Startup = new Guid(0xB97D20BB, 0xF46A, 0x4C97, 0xBA, 0x10, 0x5E, 0x36, 0x8, 0x43, 0x8, 0x54);

        /// <summary>
        /// Programs
        /// </summary>
        public static readonly Guid Programs = new Guid(0xA77F5D77, 0x2E2B, 0x44C3, 0xA6, 0xA2, 0xAB, 0xA6, 0x1, 0x5, 0x4A, 0x51);

        /// <summary>
        /// Start Menu
        /// </summary>
        public static readonly Guid StartMenu = new Guid(0x625B53C3, 0xAB48, 0x4EC1, 0xBA, 0x1F, 0xA1, 0xEF, 0x41, 0x46, 0xFC, 0x19);

        /// <summary>
        /// Recent Items
        /// </summary>
        public static readonly Guid Recent = new Guid(0xAE50C081, 0xEBD2, 0x438A, 0x86, 0x55, 0x8A, 0x9, 0x2E, 0x34, 0x98, 0x7A);

        /// <summary>
        /// SendTo
        /// </summary>
        public static readonly Guid SendTo = new Guid(0x8983036C, 0x27C0, 0x404B, 0x8F, 0x8, 0x10, 0x2D, 0x10, 0xDC, 0xFD, 0x74);

        /// <summary>
        /// Documents
        /// </summary>
        public static readonly Guid Documents = new Guid(0xFDD39AD0, 0x238F, 0x46AF, 0xAD, 0xB4, 0x6C, 0x85, 0x48, 0x3, 0x69, 0xC7);

        /// <summary>
        /// Favorites
        /// </summary>
        public static readonly Guid Favorites = new Guid(0x1777F761, 0x68AD, 0x4D8A, 0x87, 0xBD, 0x30, 0xB7, 0x59, 0xFA, 0x33, 0xDD);

        /// <summary>
        /// Network Shortcuts
        /// </summary>
        public static readonly Guid NetHood = new Guid(0xC5ABBF53, 0xE17F, 0x4121, 0x89, 0x0, 0x86, 0x62, 0x6F, 0xC2, 0xC9, 0x73);

        /// <summary>
        /// Printer Shortcuts
        /// </summary>
        public static readonly Guid PrintHood = new Guid(0x9274BD8D, 0xCFD1, 0x41C3, 0xB3, 0x5E, 0xB1, 0x3F, 0x55, 0xA7, 0x58, 0xF4);

        /// <summary>
        /// Templates
        /// </summary>
        public static readonly Guid Templates = new Guid(0xA63293E8, 0x664E, 0x48DB, 0xA0, 0x79, 0xDF, 0x75, 0x9E, 0x5, 0x9, 0xF7);

        /// <summary>
        /// Startup
        /// </summary>
        public static readonly Guid CommonStartup = new Guid(0x82A5EA35, 0xD9CD, 0x47C5, 0x96, 0x29, 0xE1, 0x5D, 0x2F, 0x71, 0x4E, 0x6E);

        /// <summary>
        /// Programs
        /// </summary>
        public static readonly Guid CommonPrograms = new Guid(0x139D44E, 0x6AFE, 0x49F2, 0x86, 0x90, 0x3D, 0xAF, 0xCA, 0xE6, 0xFF, 0xB8);

        /// <summary>
        /// Start Menu
        /// </summary>
        public static readonly Guid CommonStartMenu = new Guid(0xA4115719, 0xD62E, 0x491D, 0xAA, 0x7C, 0xE7, 0x4B, 0x8B, 0xE3, 0xB0, 0x67);

        /// <summary>
        /// Public Desktop
        /// </summary>
        public static readonly Guid PublicDesktop = new Guid(0xC4AA340D, 0xF20F, 0x4863, 0xAF, 0xEF, 0xF8, 0x7E, 0xF2, 0xE6, 0xBA, 0x25);

        /// <summary>
        /// ProgramData
        /// </summary>
        public static readonly Guid ProgramData = new Guid(0x62AB5D82, 0xFDC1, 0x4DC3, 0xA9, 0xDD, 0x7, 0xD, 0x1D, 0x49, 0x5D, 0x97);

        /// <summary>
        /// Templates
        /// </summary>
        public static readonly Guid CommonTemplates = new Guid(0xB94237E7, 0x57AC, 0x4347, 0x91, 0x51, 0xB0, 0x8C, 0x6C, 0x32, 0xD1, 0xF7);

        /// <summary>
        /// Public Documents
        /// </summary>
        public static readonly Guid PublicDocuments = new Guid(0xED4824AF, 0xDCE4, 0x45A8, 0x81, 0xE2, 0xFC, 0x79, 0x65, 0x8, 0x36, 0x34);

        /// <summary>
        /// Roaming
        /// </summary>
        public static readonly Guid RoamingAppData = new Guid(0x3EB685DB, 0x65F9, 0x4CF6, 0xA0, 0x3A, 0xE3, 0xEF, 0x65, 0x72, 0x9F, 0x3D);

        /// <summary>
        /// Local
        /// </summary>
        public static readonly Guid LocalAppData = new Guid(0xF1B32785, 0x6FBA, 0x4FCF, 0x9D, 0x55, 0x7B, 0x8E, 0x7F, 0x15, 0x70, 0x91);

        /// <summary>
        /// LocalLow
        /// </summary>
        public static readonly Guid LocalAppDataLow = new Guid(0xA520A1A4, 0x1780, 0x4FF6, 0xBD, 0x18, 0x16, 0x73, 0x43, 0xC5, 0xAF, 0x16);

        /// <summary>
        /// Temporary Internet Files
        /// </summary>
        public static readonly Guid InternetCache = new Guid(0x352481E8, 0x33BE, 0x4251, 0xBA, 0x85, 0x60, 0x7, 0xCA, 0xED, 0xCF, 0x9D);

        /// <summary>
        /// Cookies
        /// </summary>
        public static readonly Guid Cookies = new Guid(0x2B0F765D, 0xC0E9, 0x4171, 0x90, 0x8E, 0x8, 0xA6, 0x11, 0xB8, 0x4F, 0xF6);

        /// <summary>
        /// History
        /// </summary>
        public static readonly Guid History = new Guid(0xD9DC8A3B, 0xB784, 0x432E, 0xA7, 0x81, 0x5A, 0x11, 0x30, 0xA7, 0x59, 0x63);

        /// <summary>
        /// System32
        /// </summary>
        public static readonly Guid System = new Guid(0x1AC14E77, 0x2E7, 0x4E5D, 0xB7, 0x44, 0x2E, 0xB1, 0xAE, 0x51, 0x98, 0xB7);

        /// <summary>
        /// System32
        /// </summary>
        public static readonly Guid SystemX86 = new Guid(0xD65231B0, 0xB2F1, 0x4857, 0xA4, 0xCE, 0xA8, 0xE7, 0xC6, 0xEA, 0x7D, 0x27);

        /// <summary>
        /// Windows
        /// </summary>
        public static readonly Guid Windows = new Guid(0xF38BF404, 0x1D43, 0x42F2, 0x93, 0x5, 0x67, 0xDE, 0xB, 0x28, 0xFC, 0x23);

        /// <summary>
        /// The user's username (%USERNAME%)
        /// </summary>
        public static readonly Guid Profile = new Guid(0x5E6C858F, 0xE22, 0x4760, 0x9A, 0xFE, 0xEA, 0x33, 0x17, 0xB6, 0x71, 0x73);

        /// <summary>
        /// Pictures
        /// </summary>
        public static readonly Guid Pictures = new Guid(0x33E28130, 0x4E1E, 0x4676, 0x83, 0x5A, 0x98, 0x39, 0x5C, 0x3B, 0xC3, 0xBB);

        /// <summary>
        /// Program Files
        /// </summary>
        public static readonly Guid ProgramFilesX86 = new Guid(0x7C5A40EF, 0xA0FB, 0x4BFC, 0x87, 0x4A, 0xC0, 0xF2, 0xE0, 0xB9, 0xFA, 0x8E);

        /// <summary>
        /// Common Files
        /// </summary>
        public static readonly Guid ProgramFilesCommonX86 = new Guid(0xDE974D24, 0xD9C6, 0x4D3E, 0xBF, 0x91, 0xF4, 0x45, 0x51, 0x20, 0xB9, 0x17);

        /// <summary>
        /// Program Files
        /// </summary>
        public static readonly Guid ProgramFilesX64 = new Guid(0x6D809377, 0x6AF0, 0x444B, 0x89, 0x57, 0xA3, 0x77, 0x3F, 0x2, 0x20, 0xE);

        /// <summary>
        /// Common Files
        /// </summary>
        public static readonly Guid ProgramFilesCommonX64 = new Guid(0x6365D5A7, 0xF0D, 0x45E5, 0x87, 0xF6, 0xD, 0xA5, 0x6B, 0x6A, 0x4F, 0x7D);

        /// <summary>
        /// Program Files
        /// </summary>
        public static readonly Guid ProgramFiles = new Guid(0x905E63B6, 0xC1BF, 0x494E, 0xB2, 0x9C, 0x65, 0xB7, 0x32, 0xD3, 0xD2, 0x1A);

        /// <summary>
        /// Common Files
        /// </summary>
        public static readonly Guid ProgramFilesCommon = new Guid(0xF7F1ED05, 0x9F6D, 0x47A2, 0xAA, 0xAE, 0x29, 0xD3, 0x17, 0xC6, 0xF0, 0x66);

        /// <summary>
        /// Administrative Tools
        /// </summary>
        public static readonly Guid AdminTools = new Guid(0x724EF170, 0xA42D, 0x4FEF, 0x9F, 0x26, 0xB6, 0xE, 0x84, 0x6F, 0xBA, 0x4F);

        /// <summary>
        /// Administrative Tools
        /// </summary>
        public static readonly Guid CommonAdminTools = new Guid(0xD0384E7D, 0xBAC3, 0x4797, 0x8F, 0x14, 0xCB, 0xA2, 0x29, 0xB3, 0x92, 0xB5);

        /// <summary>
        /// Music
        /// </summary>
        public static readonly Guid Music = new Guid(0x4BD8D571, 0x6D19, 0x48D3, 0xBE, 0x97, 0x42, 0x22, 0x20, 0x8, 0xE, 0x43);

        /// <summary>
        /// Videos
        /// </summary>
        public static readonly Guid Videos = new Guid(0x18989B1D, 0x99B5, 0x455B, 0x84, 0x1C, 0xAB, 0x7C, 0x74, 0xE4, 0xDD, 0xFC);

        /// <summary>
        /// Public Pictures
        /// </summary>
        public static readonly Guid PublicPictures = new Guid(0xB6EBFB86, 0x6907, 0x413C, 0x9A, 0xF7, 0x4F, 0xC2, 0xAB, 0xF0, 0x7C, 0xC5);

        /// <summary>
        /// Public Music
        /// </summary>
        public static readonly Guid PublicMusic = new Guid(0x3214FAB5, 0x9757, 0x4298, 0xBB, 0x61, 0x92, 0xA9, 0xDE, 0xAA, 0x44, 0xFF);

        /// <summary>
        /// Public Videos
        /// </summary>
        public static readonly Guid PublicVideos = new Guid(0x2400183A, 0x6185, 0x49FB, 0xA2, 0xD8, 0x4A, 0x39, 0x2A, 0x60, 0x2B, 0xA3);

        /// <summary>
        /// Resources
        /// </summary>
        public static readonly Guid ResourceDir = new Guid(0x8AD10C31, 0x2ADB, 0x4296, 0xA8, 0xF7, 0xE4, 0x70, 0x12, 0x32, 0xC9, 0x72);

        /// <summary>
        /// None
        /// </summary>
        public static readonly Guid LocalizedResourcesDir = new Guid(0x2A00375E, 0x224C, 0x49DE, 0xB8, 0xD1, 0x44, 0xD, 0xF7, 0xEF, 0x3D, 0xDC);

        /// <summary>
        /// OEM Links
        /// </summary>
        public static readonly Guid CommonOEMLinks = new Guid(0xC1BAE2D0, 0x10DF, 0x4334, 0xBE, 0xDD, 0x7A, 0xA2, 0xB, 0x22, 0x7A, 0x9D);

        /// <summary>
        /// Temporary Burn Folder
        /// </summary>
        public static readonly Guid CDBurning = new Guid(0x9E52AB10, 0xF80D, 0x49DF, 0xAC, 0xB8, 0x43, 0x30, 0xF5, 0x68, 0x78, 0x55);

        /// <summary>
        /// Users
        /// </summary>
        public static readonly Guid UserProfiles = new Guid(0x762D272, 0xC50A, 0x4BB0, 0xA3, 0x82, 0x69, 0x7D, 0xCD, 0x72, 0x9B, 0x80);

        /// <summary>
        /// Playlists
        /// </summary>
        public static readonly Guid Playlists = new Guid(0xDE92C1C7, 0x837F, 0x4F69, 0xA3, 0xBB, 0x86, 0xE6, 0x31, 0x20, 0x4A, 0x23);

        /// <summary>
        /// Sample Playlists
        /// </summary>
        public static readonly Guid SamplePlaylists = new Guid(0x15CA69B3, 0x30EE, 0x49C1, 0xAC, 0xE1, 0x6B, 0x5E, 0xC3, 0x72, 0xAF, 0xB5);

        /// <summary>
        /// Sample Music
        /// </summary>
        public static readonly Guid SampleMusic = new Guid(0xB250C668, 0xF57D, 0x4EE1, 0xA6, 0x3C, 0x29, 0xE, 0xE7, 0xD1, 0xAA, 0x1F);

        /// <summary>
        /// Sample Pictures
        /// </summary>
        public static readonly Guid SamplePictures = new Guid(0xC4900540, 0x2379, 0x4C75, 0x84, 0x4B, 0x64, 0xE6, 0xFA, 0xF8, 0x71, 0x6B);

        /// <summary>
        /// Sample Videos
        /// </summary>
        public static readonly Guid SampleVideos = new Guid(0x859EAD94, 0x2E85, 0x48AD, 0xA7, 0x1A, 0x9, 0x69, 0xCB, 0x56, 0xA6, 0xCD);

        /// <summary>
        /// Slide Shows
        /// </summary>
        public static readonly Guid PhotoAlbums = new Guid(0x69D2CF90, 0xFC33, 0x4FB7, 0x9A, 0xC, 0xEB, 0xB0, 0xF0, 0xFC, 0xB4, 0x3C);

        /// <summary>
        /// Public
        /// </summary>
        public static readonly Guid Public = new Guid(0xDFDF76A2, 0xC82A, 0x4D63, 0x90, 0x6A, 0x56, 0x44, 0xAC, 0x45, 0x73, 0x85);

        /// <summary>
        /// Programs and Features
        /// </summary>
        public static readonly Guid ChangeRemovePrograms = new Guid(0xDF7266AC, 0x9274, 0x4867, 0x8D, 0x55, 0x3B, 0xD6, 0x61, 0xDE, 0x87, 0x2D);

        /// <summary>
        /// Installed Updates
        /// </summary>
        public static readonly Guid AppUpdates = new Guid(0xA305CE99, 0xF527, 0x492B, 0x8B, 0x1A, 0x7E, 0x76, 0xFA, 0x98, 0xD6, 0xE4);

        /// <summary>
        /// Get Programs
        /// </summary>
        public static readonly Guid AddNewPrograms = new Guid(0xDE61D971, 0x5EBC, 0x4F02, 0xA3, 0xA9, 0x6C, 0x82, 0x89, 0x5E, 0x5C, 0x4);

        /// <summary>
        /// Downloads
        /// </summary>
        public static readonly Guid Downloads = new Guid(0x374DE290, 0x123F, 0x4565, 0x91, 0x64, 0x39, 0xC4, 0x92, 0x5E, 0x46, 0x7B);

        /// <summary>
        /// Public Downloads
        /// </summary>
        public static readonly Guid PublicDownloads = new Guid(0x3D644C9B, 0x1FB8, 0x4F30, 0x9B, 0x45, 0xF6, 0x70, 0x23, 0x5F, 0x79, 0xC0);

        /// <summary>
        /// Searches
        /// </summary>
        public static readonly Guid SavedSearches = new Guid(0x7D1D3A04, 0xDEBB, 0x4115, 0x95, 0xCF, 0x2F, 0x29, 0xDA, 0x29, 0x20, 0xDA);

        /// <summary>
        /// Quick Launch
        /// </summary>
        public static readonly Guid QuickLaunch = new Guid(0x52A4F021, 0x7B75, 0x48A9, 0x9F, 0x6B, 0x4B, 0x87, 0xA2, 0x10, 0xBC, 0x8F);

        /// <summary>
        /// Contacts
        /// </summary>
        public static readonly Guid Contacts = new Guid(0x56784854, 0xC6CB, 0x462B, 0x81, 0x69, 0x88, 0xE3, 0x50, 0xAC, 0xB8, 0x82);

        /// <summary>
        /// Gadgets
        /// </summary>
        public static readonly Guid SidebarParts = new Guid(0xA75D362E, 0x50FC, 0x4FB7, 0xAC, 0x2C, 0xA8, 0xBE, 0xAA, 0x31, 0x44, 0x93);

        /// <summary>
        /// Gadgets
        /// </summary>
        public static readonly Guid SidebarDefaultParts = new Guid(0x7B396E54, 0x9EC5, 0x4300, 0xBE, 0xA, 0x24, 0x82, 0xEB, 0xAE, 0x1A, 0x26);

        /// <summary>
        /// Tree property value folder
        /// </summary>
        public static readonly Guid TreeProperties = new Guid(0x5B3749AD, 0xB49F, 0x49C1, 0x83, 0xEB, 0x15, 0x37, 0xF, 0xBD, 0x48, 0x82);

        /// <summary>
        /// GameExplorer
        /// </summary>
        public static readonly Guid PublicGameTasks = new Guid(0xDEBF2536, 0xE1A8, 0x4C59, 0xB6, 0xA2, 0x41, 0x45, 0x86, 0x47, 0x6A, 0xEA);

        /// <summary>
        /// GameExplorer
        /// </summary>
        public static readonly Guid GameTasks = new Guid(0x54FAE61, 0x4DD8, 0x4787, 0x80, 0xB6, 0x9, 0x2, 0x20, 0xC4, 0xB7, 0x0);

        /// <summary>
        /// Saved Games
        /// </summary>
        public static readonly Guid SavedGames = new Guid(0x4C5C32FF, 0xBB9D, 0x43B0, 0xB5, 0xB4, 0x2D, 0x72, 0xE5, 0x4E, 0xAA, 0xA4);

        /// <summary>
        /// Games
        /// </summary>
        public static readonly Guid Games = new Guid(0xCAC52C1A, 0xB53D, 0x4EDC, 0x92, 0xD7, 0x6B, 0x2E, 0x8A, 0xC1, 0x94, 0x34);

        /// <summary>
        /// Recorded TV
        /// </summary>
        public static readonly Guid RecordedTV = new Guid(0xBD85E001, 0x112E, 0x431E, 0x98, 0x3B, 0x7B, 0x15, 0xAC, 0x9, 0xFF, 0xF1);

        /// <summary>
        /// Microsoft Office Outlook
        /// </summary>
        public static readonly Guid SearchMapi = new Guid(0x98EC0E18, 0x2098, 0x4D44, 0x86, 0x44, 0x66, 0x97, 0x93, 0x15, 0xA2, 0x81);

        /// <summary>
        /// Offline Files
        /// </summary>
        public static readonly Guid SearchCsc = new Guid(0xEE32E446, 0x31CA, 0x4ABA, 0x81, 0x4F, 0xA5, 0xEB, 0xD2, 0xFD, 0x6D, 0x5E);

        /// <summary>
        /// Links
        /// </summary>
        public static readonly Guid Links = new Guid(0xBFB9D5E0, 0xC6A9, 0x404C, 0xB2, 0xB2, 0xAE, 0x6D, 0xB6, 0xAF, 0x49, 0x68);

        /// <summary>
        /// The user's full name (for instance, Jean Philippe Bagel) entered when the user account was created.
        /// </summary>
        public static readonly Guid UsersFiles = new Guid(0xF3CE0F7C, 0x4901, 0x4ACC, 0x86, 0x48, 0xD5, 0xD4, 0x4B, 0x4, 0xEF, 0x8F);

        /// <summary>
        /// Search home
        /// </summary>
        public static readonly Guid SearchHome = new Guid(0x190337D1, 0xB8CA, 0x4121, 0xA6, 0x39, 0x6D, 0x47, 0x2D, 0x16, 0x97, 0x2A);

        /// <summary>
        /// Original Images
        /// </summary>
        public static readonly Guid OriginalImages = new Guid(0x2C36C0AA, 0x5812, 0x4B87, 0xBF, 0xD0, 0x4C, 0xD0, 0xDF, 0xB1, 0x9B, 0x39);

        /// <summary>
        /// SkyDrive; Windows 8.1 Folder
        /// </summary>
        public static readonly Guid SkyDrive = new Guid(0xA52BBA46, 0xE9E1, 0x435F, 0xB3, 0xD9, 0x28, 0xDA, 0xA6, 0x48, 0xC0, 0xF6);

        /// <summary>
        /// OneDrive; Windows 8.1/Windows 10 Folder
        /// </summary>
        public static readonly Guid OneDrive = new Guid(0xA52BBA46, 0xE9E1, 0x435F, 0xB3, 0xD9, 0x28, 0xDA, 0xA6, 0x48, 0xC0, 0xF6);
    }
}
