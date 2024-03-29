// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NativeShell
//         Wrappers for native and COM shell interfaces.
//
// Some enum documentation copied from the MSDN (and in some cases, updated).
// Some classes and interfaces were ported from the WindowsAPICodePack.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************

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
    /// <summary>
    /// Describes the event that has occurred.
    /// Typically, only one event is specified at a time.
    /// If more than one event is specified,
    /// the values contained in the dwItem1 and dwItem2 parameters must be the same,
    /// respectively, for all specified events.
    /// This parameter can be one or more of the following values:
    /// </summary>
    [Flags]
    public enum ShellObjectChangeTypes : uint
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// The name of a nonfolder item has changed.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the previous PIDL or name of the item.
        /// dwItem2 contains the new PIDL or name of the item.
        /// </summary>
        ItemRename = 0x1,

        /// <summary>
        /// A nonfolder item has been created. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the item that was created.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        ItemCreate = 0x2,

        /// <summary>
        /// A nonfolder item has been deleted. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the item that was deleted.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        ItemDelete = 0x4,

        /// <summary>
        /// A folder has been created. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the folder that was created.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        DirectoryCreate = 0x8,

        /// <summary>
        /// A folder has been removed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the folder that was removed.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        DirectoryDelete = 0x10,

        /// <summary>
        /// Storage media has been inserted into a drive. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the root of the drive that contains the new media.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        MediaInsert = 0x20,

        /// <summary>
        /// Storage media has been removed from a drive. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the root of the drive from which the media was removed.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        MediaRemove = 0x40,

        /// <summary>
        /// A drive has been removed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the root of the drive that was removed.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        DriveRemove = 0x80,

        /// <summary>
        /// A drive has been added. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the root of the drive that was added.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        DriveAdd = 0x100,

        /// <summary>
        /// A folder on the local computer is being shared via the network.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the folder that is being shared.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        NetShare = 0x200,

        /// <summary>
        /// A folder on the local computer is no longer being shared via the network.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the folder that is no longer being shared.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        NetUnshare = 0x400,

        /// <summary>
        /// The attributes of an item or folder have changed.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the item or folder that has changed.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        AttributesChange = 0x800,

        /// <summary>
        /// The contents of an existing folder have changed, but the folder still exists and has not been renamed.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the folder that has changed.
        /// dwItem2 is not used and should be NULL.
        /// If a folder has been created, deleted, or renamed, use SHCNE_MKDIR, SHCNE_RMDIR, or SHCNE_RENAMEFOLDER, respectively.
        /// </summary>
        DirectoryContentsUpdate = 0x1000,

        /// <summary>
        /// An existing item (a folder or a nonfolder) has changed, but the item still exists and has not been renamed.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the item that has changed.
        /// dwItem2 is not used and should be NULL.
        /// If a nonfolder item has been created, deleted, or renamed,
        /// use SHCNE_CREATE, SHCNE_DELETE, or SHCNE_RENAMEITEM, respectively, instead.
        /// </summary>
        Update = 0x2000,

        /// <summary>
        /// The computer has disconnected from a server.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the server from which the computer was disconnected.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        ServerDisconnect = 0x4000,

        /// <summary>
        /// An image in the system image list has changed.
        /// SHCNF_DWORD must be specified in uFlags.
        /// dwItem1 is not used and should be NULL.
        /// dwItem2 contains the index in the system image list that has changed.
        /// </summary> //verify this is not opposite?
        SystemImageUpdate = 0x8000,

        /// <summary>
        /// The name of a folder has changed. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the previous PIDL or name of the folder.
        /// dwItem2 contains the new PIDL or name of the folder.
        /// </summary>
        DirectoryRename = 0x20000,

        /// <summary>
        /// The amount of free space on a drive has changed.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the root of the drive on which the free space changed.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        FreeSpace = 0x40000,

        /// <summary>
        /// A file type association has changed.
        /// SHCNF_IDLIST must be specified in the uFlags parameter.
        /// dwItem1 and dwItem2 are not used and must be NULL.
        /// </summary>
        AssociationChange = 0x8000000,

        /// <summary>
        /// Specifies a combination of all of the disk event identifiers.
        /// </summary>
        DiskEventsMask = 0x2381F,

        /// <summary>
        /// Specifies a combination of all of the global event identifiers.
        /// </summary>
        GlobalEventsMask = 0xC0581E0,

        /// <summary>
        /// All events have occurred.
        /// </summary>
        AllEventsMask = 0x7FFFFFFF,

        /// <summary>
        /// The specified event occurred as a result of a system interrupt.
        /// As this value modifies other event values, it cannot be used alone.
        /// </summary>
        FromInterrupt = 0x80000000
    }
}
