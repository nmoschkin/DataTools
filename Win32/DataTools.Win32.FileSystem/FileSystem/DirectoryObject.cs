﻿using DataTools.FileSystem;
using DataTools.Memory;
using DataTools.Shell.Native;
using DataTools.Win32;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataTools.Desktop
{
    [Flags]
    public enum RootFolderTypes
    {
        QuickAccess = 1,
        NetworkPlaces = 2,
        MyComputer = 4,
        ControlPanel = 8,
        All = 0xf
    }

    public class DirectoryObject : ShellObject, IShellFolderObject
    {
        private List<IShellObject> children = new List<IShellObject>() { null };
        private bool lazy;

        public bool IsLazyLoad
        {
            get => lazy;
            set
            {
                if (lazy != value)
                {
                    lazy = value;
                    Refresh();
                }
            }
        }

        public ICollection<IShellFolderObject> Folders => children.Where(x => x.IsFolder).Select(y => (IShellFolderObject)y).ToList();

        /// <summary>
        /// Create a new root shell view.
        /// </summary>
        /// <param name="folderTypes">The folders to include in the root view.</param>
        /// <param name="iconSize">The icon size to grab for the objects that are created.</param>
        /// <returns></returns>
        public static IShellFolderObject CreateRootView(RootFolderTypes folderTypes, StandardIcons iconSize = StandardIcons.Icon48)
        {
            var d = new DirectoryObject();

            if ((folderTypes & RootFolderTypes.QuickAccess) == RootFolderTypes.QuickAccess)
            {
                var nd = new DirectoryObject("QuickAccessFolder", true, true, iconSize);
                if (nd != null) d.Add(nd);
            }

            if ((folderTypes & RootFolderTypes.MyComputer) == RootFolderTypes.MyComputer)
            {
                var nd = new DirectoryObject("MyComputerFolder", true, true, iconSize);
                if (nd != null) d.Add(nd);
            }

            if ((folderTypes & RootFolderTypes.NetworkPlaces) == RootFolderTypes.NetworkPlaces)
            {
                var nd = new DirectoryObject("NetworkPlacesFolder", true, true, iconSize);
                if (nd != null) d.Add(nd);
            }

            if ((folderTypes & RootFolderTypes.ControlPanel) == RootFolderTypes.ControlPanel)
            {
                var nd = new DirectoryObject("ControlPanelFolder", true, true, iconSize);
                if (nd != null) d.Add(nd);
            }

            return d;
        }

        /// <summary>
        /// Get a directory object from a file object.
        /// The <see cref="FileObject"/> instance passed is retained in the returned <see cref="DirectoryObject"/> object.
        /// </summary>
        /// <param name="fileObj">A <see cref="FileObject"/></param>
        /// <returns>A <see cref="DirectoryObject"/>.</returns>
        public static DirectoryObject FromFileObject(FileObject fileObj)
        {
            var dir = new DirectoryObject(fileObj.Directory, fileObj.IsSpecial, true, StandardIcons.Icon48);
            return dir;
        }

        public DirectoryObject(string parsingName, bool special, bool initialize, StandardIcons iconSize) : base(parsingName, special, initialize, iconSize)
        {
            children = new List<IShellObject>();
        }

        public DirectoryObject(string parsingName, bool isSpecial) : base(parsingName, isSpecial, true, StandardIcons.Icon48)
        {
        }

        public DirectoryObject(string parsingName) : base(parsingName, false, true, StandardIcons.Icon48)
        {
        }

        private DirectoryObject() : base("", false, false, StandardIcons.Icon48)
        {
            DisplayName = "";
        }

        public ICollection<IShellObject> Children => children;

        public override bool IsFolder => true;
        public override long Size => throw new NotImplementedException("Cannot get allocated byte size from folder in this manner.");
        public bool IsReadOnly => true;
        public int Count => children.Count;

        /// <summary>
        /// Returns an item in the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IShellObject this[int index]
        {
            get
            {
                return children[index];
            }
        }

        internal void Add(IShellObject item)
        {
            children.Add(item);
        }

        internal void Clear()
        {
            children.Clear();
        }

        /// <summary>
        /// Determine whether this folder contains an item
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="useParsingName">True to look by <see cref="ParsingName"/>, otherwise use <see cref="DisplayName"/>.</param>
        /// <param name="obj">The shell object that was located.</param>
        /// <returns></returns>
        internal bool Contains(string name, bool useParsingName, out IShellObject obj)
        {
            string fn = name.ToLower();
            string fn2;
            foreach (var f in children)
            {
                fn2 = useParsingName ? f.ParsingName.ToLower() : f.DisplayName.ToLower();
                if ((fn2 ?? "") == (fn ?? ""))
                {
                    obj = f;
                    return true;
                }
            }

            obj = null;
            return false;
        }

        /// <summary>
        /// Determine whether this folder contains an item
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="useParsingName">True to look by <see cref="ParsingName"/>, otherwise use <see cref="DisplayName"/>.</param>
        /// <returns></returns>
        public bool Contains(string name, bool useParsingName)
        {
            return Contains(name, useParsingName, out _);
        }

        public bool Contains(IShellObject item)
        {
            return children.Contains(item);
        }

        public void CopyTo(IShellObject[] array, int arrayIndex)
        {
            children.CopyTo(array, arrayIndex);
        }

        internal bool Remove(IShellObject item)
        {
            return children.Remove(item);
        }

        void ICollection<IShellObject>.Add(IShellObject item)
        {
            ((ICollection<IShellObject>)children).Add(item);
        }

        void ICollection<IShellObject>.Clear()
        {
            ((ICollection<IShellObject>)children).Clear();
        }

        IEnumerator<IShellObject> IEnumerable<IShellObject>.GetEnumerator() => new DirectoryObjectEnumerator(this, true, true);

        IEnumerator IEnumerable.GetEnumerator() => new DirectoryObjectEnumerator(this, true, true);

        bool ICollection<IShellObject>.Remove(IShellObject item)
        {
            return ((ICollection<IShellObject>)children).Remove(item);
        }
    }

    public class DirectoryObjectEnumerator : IEnumerator<IShellObject>
    {
        private DirectoryObject source;

        private IShellObject current;

        private IShellItem shitem;
        private IShellFolder shfld;
        private IEnumIDList enumer;

        private bool folders;
        private bool files;

        public DirectoryObjectEnumerator(DirectoryObject source, bool folders, bool files)
        {
            this.source = source;
            this.folders = folders;
            this.files = files;
            if (this.source.Children.Count == 0) source.Refresh();
            CreateEnumerator();
        }

        public IShellObject Current => current;

        object IEnumerator.Current => current;

        public void Dispose()
        {
            current = null;
            enumer = null;
            shitem = null;
            shfld = null;
            source = null;
        }

        public bool MoveNext()
        {
            if (enumer == null) return false;

            current = NextObject();

            while (!folders && current != null && current.IsFolder)
            {
                current = NextObject();
            }

            while (!files && current != null && !current.IsFolder)
            {
                current = NextObject();
            }

            return current != null;
        }

        public void Reset()
        {
            current = null;
            enumer.Reset();
        }

        private void CreateEnumerator()
        {
            var shguid = ShellIIDGuid.IShellItemUuid;
            var res = NativeShell.SHCreateItemFromParsingName(source.ParsingName, IntPtr.Zero, ref shguid, out shitem);

            if (res == HResult.Ok)
            {
                var fldbind = ShellBHIDGuid.ShellFolderObjectUuid;
                var fld2guid = ShellIIDGuid.IShellFolder2Uuid;

                shitem.BindToHandler(IntPtr.Zero, ref fldbind, ref fld2guid, out shfld);
                if (shfld == null) return;

                shfld.EnumObjects(IntPtr.Zero,
                    ShellFolderEnumerationOptions.Folders |
                    ShellFolderEnumerationOptions.IncludeHidden |
                    ShellFolderEnumerationOptions.NonFolders |
                    ShellFolderEnumerationOptions.InitializeOnFirstNext,
                    out enumer);
            }
        }

        private IShellObject NextObject()
        {
            FileObject fobj;
            DirectoryObject dobj;

            CoTaskMemPtr mm;
            CoTaskMemPtr mm2;

            HResult res;

            if (enumer != null)
            {
                var glist = new List<string>();
                uint cf;
                IntPtr ptr;
                string pout;

                // mm.AllocCoTaskMem((MAX_PATH * 2) + 8)

                using (mm2 = new CoTaskMemPtr(NativeShell.MAX_PATH * 2 + 8))
                {
                    cf = 0U;
                    mm2.ZeroMemory();
                    res = enumer.Next(1U, out ptr, out cf);
                    mm = ptr;

                    if (cf == 0L) return null;
                    if (res != HResult.Ok) return null;

                    mm2.IntAt(0L) = 2;

                    shfld.GetDisplayNameOf(mm, (uint)ShellItemDesignNameOptions.DesktopAbsoluteParsing, mm2);
                    
                    CoTaskMemPtr inv = mm2.NIntAt(1L);
                    
                    var invh = inv.DangerousGetHandle();

                    if (invh != IntPtr.Zero)
                    {
                        if (inv.CharAt(0L) != '\0')
                        {
                            var fp = (string)inv;
                            var lpInfo = new SHFILEINFO();

                            // Dim sgfin As ShellFileGetAttributesOptions = 0,
                            // sgfout As ShellFileGetAttributesOptions = 0

                            int iFlags = User32.SHGFI_PIDL | User32.SHGFI_ATTRIBUTES;

                            lpInfo.dwAttributes = 0;

                            try
                            {
                                User32.SHGetItemInfo(invh, 0, ref lpInfo, Marshal.SizeOf<SHFILEINFO>(), iFlags);
                            }
                            catch
                            {
                                return null;
                            }

                            pout = Path.Combine(source.ParsingName, fp);

                            if (lpInfo.dwAttributes == 0)
                            {
                                lpInfo.dwAttributes = (int)FileTools.GetFileAttributes(pout);
                            }

                            FileAttributes drat = (FileAttributes)(int)(lpInfo.dwAttributes);

                            if ((lpInfo.dwAttributes & (int)FileAttributes.Directory) == (int)FileAttributes.Directory && !File.Exists(pout))
                            {
                                dobj = new DirectoryObject(pout, source.IsSpecial || pout.Contains("$RECYCLE"), false, source.IconSize);
                                dobj.Parent = source;
                                dobj.IconSize = source.IconSize;

                                return dobj;
                            }
                            else
                            {
                                fobj = new FileObject(pout, source.IsSpecial, true, source.IconSize);
                                fobj.Parent = source;
                                fobj.IconSize = source.IconSize;

                                return fobj;
                            }
                        }
                    }
                }

            }

            return null;

        }
    }
}