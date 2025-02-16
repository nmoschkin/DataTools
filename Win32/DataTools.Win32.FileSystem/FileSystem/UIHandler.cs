// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: System file association utility classes.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Essentials.SortedLists;
using DataTools.Shell.Native;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DataTools.Desktop
{
    /// <summary>
    /// Represents a registered file-type handler program.
    /// </summary>
    public sealed class UIHandler : INotifyPropertyChanged, IDisposable
    {
        private string uiname;
        private string exePath;

        private Icon icon;
        private Bitmap bmpImage;

        private AllSystemFileTypes parent;
        private IAssocHandler handler;

        private List<string> extList = new List<string>();

        private bool isPreferred;

        private System.Collections.ObjectModel.ObservableCollection<SystemFileType> assocList = new System.Collections.ObjectModel.ObservableCollection<SystemFileType>();

        /// <summary>
        /// Gets the list of supported extensions, separated by commas.
        /// </summary>
        /// <returns></returns>
        public string ExtListString
        {
            get
            {
                var sb = new System.Text.StringBuilder();
                int cc = 0;
                int x = 0;
                foreach (var s in extList)
                {
                    if (x > 0)
                        sb.Append(", ");
                    x += 1;
                    if (cc >= 80)
                    {
                        sb.Append("\r\n");
                        cc = 0;
                    }

                    cc += s.Length;
                    sb.Append(s);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets a value indicating that this is a recommended file handler.
        /// </summary>
        /// <returns></returns>
        public bool IsPreferred
        {
            get
            {
                return isPreferred;
            }

            internal set
            {
                isPreferred = value;
            }
        }

        /// <summary>
        /// Returns the size of the program icon.
        /// </summary>
        /// <returns></returns>
        public StandardIcons IconSize
        {
            get
            {
                if (parent is null)
                    return StandardIcons.Icon48;
                return parent.IconSize;
            }
        }

        internal UIHandler(IAssocHandler handler, AllSystemFileTypes parent)
        {
            this.parent = parent;
            Refresh(handler);
        }

        /// <summary>
        /// Retrieves an array of support extensions.
        /// </summary>
        /// <returns></returns>
        public string[] ExtensionList
        {
            get
            {
                return extList.ToArray();
            }

            internal set
            {
                extList.Clear();
                extList.AddRange(value);
            }
        }

        /// <summary>
        /// Rebuild the association handler list.
        /// </summary>
        internal void RebuildAssocList()
        {
            assocList.Clear();
            extList.Sort();

            foreach (var s in extList)
            {
                foreach (var f in parent.FileTypes)
                {
                    if ((f.Extension ?? "") == (s ?? ""))
                    {
                        assocList.Add(f);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the list of associated file handlers.
        /// </summary>
        /// <returns></returns>
        public System.Collections.ObjectModel.ObservableCollection<SystemFileType> AssocList
        {
            get
            {
                if (assocList.Count == 0)
                {
                    RebuildAssocList();
                }
                return assocList;
            }
        }

        /// <summary>
        /// Clears the extension list.
        /// </summary>
        internal void ClearExtList()
        {
            extList.Clear();
        }

        /// <summary>
        /// Add an extension.
        /// </summary>
        /// <param name="e"></param>
        internal void AddExt(string e)
        {
            if (extList.Contains(e) == false) extList.Add(e);
        }

        /// <summary>
        /// Refresh using the IAssocHandler
        /// </summary>
        /// <param name="handler"></param>
        internal void Refresh(IAssocHandler handler)
        {
            string pth = null;

            int idx = 0;

            this.handler = handler;
            IsPreferred = this.handler.IsRecommended() == HResult.Ok;

            handler.GetName(out string epath);

            ExePath = epath;

            if (File.Exists(ExePath) == false)
                throw new SystemException("Program path not found");

            handler.GetUIName(out string name);
            UIName = name;

            handler.GetIconLocation(out pth, out idx);

            Icon = Resources.LoadLibraryIcon(pth, idx, IconSize);

            if (Icon is null)
            {
                int iix = (int)NativeShell.Shell_GetCachedImageIndex(pth, idx, 0U);

                switch (IconSize)
                {
                    case StandardIcons.Icon256:
                        Icon = Resources.GetFileIconFromIndex(iix, Resources.SystemIconSizes.Jumbo);
                        break;

                    case StandardIcons.Icon48:
                        Icon = Resources.GetFileIconFromIndex(iix, Resources.SystemIconSizes.ExtraLarge);
                        break;

                    case StandardIcons.Icon32:
                        Icon = Resources.GetFileIconFromIndex(iix, Resources.SystemIconSizes.Large);
                        break;

                    default:
                        Icon = Resources.GetFileIconFromIndex(iix, Resources.SystemIconSizes.Small);
                        break;
                }
            }
        }

        internal void Refresh()
        {
            Refresh(handler);
        }

        /// <summary>
        /// The friendly name of the program.
        /// </summary>
        /// <returns></returns>
        public string UIName
        {
            get
            {
                return uiname;
            }

            internal set
            {
                uiname = value;
                OnPropertyChanged("UIName");
            }
        }

        /// <summary>
        /// The executable path of the program.
        /// </summary>
        /// <returns></returns>
        public string ExePath
        {
            get
            {
                return exePath;
            }

            internal set
            {
                exePath = value;
                OnPropertyChanged("ExePath");
            }
        }

        /// <summary>
        /// The icon for the executable handler.
        /// </summary>
        /// <returns></returns>
        public Icon Icon
        {
            get
            {
                return icon;
            }

            internal set
            {
                icon = value;
                Image = Resources.IconToTransparentBitmap(icon);
                OnPropertyChanged("Icon");
            }
        }

        /// <summary>
        /// The WPF image for the executable handler.
        /// </summary>
        /// <returns></returns>
        public Bitmap Image
        {
            get
            {
                return bmpImage;
            }

            internal set
            {
                bmpImage = value;
                OnPropertyChanged("Image");
            }
        }

        private bool disposedValue; // To detect redundant calls

        public event PropertyChangedEventHandler PropertyChanged;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    handler = null;
                }
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return UIName;
        }

        private void OnPropertyChanged([CallerMemberName] string e = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }
    }

    public class UIHandlerEnumEventArgs : EventArgs
    {
        public UIHandler Handler { get; private set; }

        public UIHandlerEnumEventArgs(UIHandler handler)
        {
            Handler = handler;
        }
    }

    /// <summary>
    /// Class that describes information for an event fired by the <see cref="AllSystemFileTypes"/> class for a general enumeration of system file types.
    /// </summary>
    public class FileTypeEnumEventArgs : EventArgs
    {
        private SystemFileType _sft;
        private int _index;
        private int _count;

        /// <summary>
        /// The current index of the system file type that is being processed.
        /// </summary>
        /// <returns></returns>
        public int Index
        {
            get
            {
                return _index;
            }
        }

        /// <summary>
        /// Total number of file types to process.
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// The current system file-type being processed.
        /// </summary>
        /// <returns></returns>
        public SystemFileType Type
        {
            get
            {
                return _sft;
            }
        }

        /// <summary>
        /// Create a new event object consisting of these variables.
        /// </summary>
        /// <param name="sf">The <see cref="SystemFileType"/> object</param>
        /// <param name="index">The current index</param>
        /// <param name="count">The total number of file types</param>
        internal FileTypeEnumEventArgs(SystemFileType sf, int index, int count)
        {
            _sft = sf;
            _index = index;
            _count = count;
        }
    }

    /// <summary>
    /// Represents a file type.
    /// </summary>
    public sealed class SystemFileType : INotifyPropertyChanged, IDisposable
    {
        private System.Collections.ObjectModel.ObservableCollection<UIHandler> handlers = new System.Collections.ObjectModel.ObservableCollection<UIHandler>();
        private AllSystemFileTypes parent;

        private string ext;
        private string desc;

        private Icon defaultIcon;
        private Bitmap defaultImage;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the size of the file icon.
        /// </summary>
        /// <returns></returns>
        public StandardIcons IconSize
        {
            get
            {
                return parent.IconSize;
            }
        }

        /// <summary>
        /// Returns the default icon.
        /// </summary>
        /// <returns></returns>
        public Icon DefaultIcon
        {
            get
            {
                return defaultIcon;
            }

            internal set
            {
                defaultIcon = value;
            }
        }

        /// <summary>
        /// Returns the default WPF image.
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        public Bitmap DefaultImage
        {
            get
            {
                if (defaultImage is null)
                    return PreferredHandler.Image;

                return defaultImage;
            }

            internal set
            {
                defaultImage = value;
            }
        }

        /// <summary>
        /// Returns the parent object.
        /// </summary>
        /// <returns></returns>
        public AllSystemFileTypes Parent
        {
            get
            {
                return parent;
            }

            internal set
            {
                parent = value;
            }
        }

        /// <summary>
        /// Returns the description of the file extension.
        /// </summary>
        /// <returns></returns>
        public string Description
        {
            get
            {
                return desc;
            }
        }

        /// <summary>
        /// Returns the first preferred UIHandler for this extension.
        /// </summary>
        /// <returns></returns>
        public UIHandler PreferredHandler
        {
            get
            {
                foreach (var h in handlers)
                {
                    if (h.IsPreferred)
                        return h;
                }

                return handlers[0];
            }
        }

        /// <summary>
        /// Gets a list of handlers for this extension.
        /// </summary>
        /// <returns></returns>
        public System.Collections.ObjectModel.ObservableCollection<UIHandler> Handlers
        {
            get
            {
                return handlers;
            }
        }

        /// <summary>
        /// Gets a list of handlers for this extension.
        /// </summary>
        /// <returns></returns>
        public System.Collections.ObjectModel.ObservableCollection<UIHandler> Items
        {
            get
            {
                return handlers;
            }
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <returns></returns>
        public string Extension
        {
            get
            {
                return ext;
            }
        }

        internal SystemFileType(AllSystemFileTypes p, string ext)
        {
            parent = p;
            this.ext = ext;
        }

        /// <summary>
        /// Create a new <see cref="SystemFileType"/> object for the given extension.
        /// </summary>
        /// <param name="ext"></param>
        public SystemFileType(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return;

            if (ext[0] != '.')
                this.ext = "." + ext.ToLower();
            else
                this.ext = ext.ToLower();

            OnPropertyChanged("Extension");
        }

        /// <summary>
        /// Creates a new <see cref="SystemFileType"/> object from the given extension with the specified parameters.
        /// </summary>
        /// <param name="ext">The file extension.</param>
        /// <param name="parent">The parent <see cref="AllSystemFileTypes"/> object.</param>
        /// <param name="size">The default icon size.</param>
        /// <returns></returns>
        public static SystemFileType FromExtension(string ext, AllSystemFileTypes parent = null, StandardIcons size = StandardIcons.Icon48)
        {
            var c = new SystemFileType(ext);

            if (parent is object)
                c.Parent = parent;

            var assoc = NativeShell.EnumFileHandlers(ext);

            if (assoc is null || assoc.Count() == 0)
                return null;

            c.Populate(assoc, size);

            if (c.Handlers.Count == 0)
                return null;
            else
                return c;
        }

        /// <summary>
        /// Creates a new <see cref="System.Drawing.Bitmap"/> object from the given extension with the specified parameters.
        /// </summary>
        /// <param name="ext">The file extension.</param>
        /// <param name="size">The default icon size.</param>
        /// <returns></returns>
        public static Bitmap ImageFromExtension(string ext, StandardIcons size = StandardIcons.Icon48)
        {
            var sft = FromExtension(ext, size: size);

            if (sft is null)
                return null;

            return sft.DefaultImage;
        }

        /// <summary>
        /// Populate the information for this object.
        /// </summary>
        /// <param name="assoc">Optional array of previously-enumerated IAssocHandlers.</param>
        /// <param name="size">The default icon size.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        internal bool Populate(IAssocHandler[] assoc = null, StandardIcons size = StandardIcons.Icon48)
        {
            if (assoc is null)
                assoc = NativeShell.EnumFileHandlers(ext);

            if (assoc is null)
                return false;

            handlers.Clear();
            string p = null;

            if (parent is null)
            {
                foreach (var a in assoc)
                {
                    p = null;
                    a.GetName(out p);
                    if (File.Exists(p) == false)
                        continue;
                    handlers.Add(new UIHandler(a, parent));
                }
            }
            else
            {
                foreach (var a in assoc)
                {
                    p = null;

                    a.GetName(out p);
                    if (File.Exists(p) == false)
                        continue;
                    handlers.Add(parent.HandlerFromAssocHandler(a, ext));
                }
            }

            OnPropertyChanged("Handlers");
            try
            {
                var pk = Registry.ClassesRoot.OpenSubKey(ext);
                RegistryKey pk2 = null;

                if (pk != null)
                {
                    var pkv = (string)(pk.GetValue(null));

                    if (pkv != null)
                    {
                        pk2 = Registry.ClassesRoot.OpenSubKey(pkv);

                        if (pk2 is object)
                        {
                            string d = (string)(pk2.GetValue(null));
                            if (string.Equals(d, desc) == false)
                            {
                                desc = (string)(pk2.GetValue(null));
                                OnPropertyChanged("Description");
                            }

                            pk2.Close();

                            pk2 = Registry.ClassesRoot.OpenSubKey(pkv + @"\DefaultIcon");

                            pk.Close();

                            if (pk2 is object)
                            {
                                d = (string)(pk2.GetValue(null));
                                pk2.Close();

                                if (d != null)
                                {
                                    int i = d.LastIndexOf(",");
                                    int c;

                                    if (i == -1)
                                    {
                                        c = 0;
                                    }
                                    else
                                    {
                                        c = int.Parse(d.Substring(i + 1));
                                        d = d.Substring(0, i);
                                    }

                                    defaultIcon = Resources.LoadLibraryIcon(d, c, size);

                                    if (defaultIcon is object)
                                    {
                                        defaultImage = Resources.IconToTransparentBitmap(defaultIcon);

                                        OnPropertyChanged(nameof(DefaultImage));
                                        OnPropertyChanged(nameof(DefaultIcon));
                                    }
                                }
                            }
                        }
                    }
                }

                if (desc is null || string.IsNullOrEmpty(desc))
                    desc = ext + " file";
            }
            catch
            {
            }

            QuickSort.Sort(handlers, new UIHandlerComp().Compare);

            return true;
        }

        public override string ToString()
        {
            return Description;
        }

        private bool disposedValue; // To detect redundant calls

        // IDisposable
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    handlers = null;
                    ext = null;
                    if (defaultIcon is object)
                        defaultIcon.Dispose();
                    defaultImage = null;
                }
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnPropertyChanged([CallerMemberName] string e = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }
    }

    /// <summary>
    /// Compares type SystemFileType objects by extension
    /// </summary>
    public class SysFileTypeComp : IComparer<SystemFileType>
    {
        public int Compare(SystemFileType x, SystemFileType y)
        {
            return string.Compare(x.Extension, y.Extension);
        }
    }

    /// <summary>
    /// Compares two UIHandler objects by UIName
    /// </summary>
    public class UIHandlerComp : IComparer<UIHandler>
    {
        public int Compare(UIHandler x, UIHandler y)
        {
            return string.Compare(x.UIName, y.UIName);
        }
    }

    /// <summary>
    /// Reprents a list of all registered file types on the system, and their handlers.
    /// </summary>
    public sealed class AllSystemFileTypes : IDisposable, INotifyPropertyChanged
    {
        private List<SystemFileType> fileTypes = new List<SystemFileType>();
        private List<UIHandler> handlers = new List<UIHandler>();

        private StandardIcons _IconSize = StandardIcons.Icon48;

        public event PopulatingEventHandler Populating;

        public event PopulatingUIHandlersEventHandler PopulatingUIHandlers;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void PopulatingEventHandler(object sender, FileTypeEnumEventArgs e);

        public delegate void PopulatingUIHandlersEventHandler(object sender, UIHandlerEnumEventArgs e);

        /// <summary>
        /// Sets the uniform standard size for all icons and images in this object graph.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public StandardIcons IconSize
        {
            get
            {
                return _IconSize;
            }

            internal set
            {
                _IconSize = value;
                OnPropertyChanged("IconSize");
            }
        }

        /// <summary>
        /// Retrieves the collection of <see cref="SystemFileType" /> objects.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<SystemFileType> FileTypes
        {
            get
            {
                return fileTypes;
            }
        }

        /// <summary>
        /// Retrieves the collection of <see cref="UIHandler"/> objects.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<UIHandler> UIHandlers
        {
            get
            {
                return handlers;
            }
            private set
            {
                handlers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Retrieves a UIHandler object base on the IAssocHandler from a cache or creates and returns a new one if it does not already exist.
        /// </summary>
        /// <param name="assoc">The IAssocHandler from which to build the new object.</param>
        /// <param name="ext">The Extension of the file type the IAssocHandler handles.</param>
        /// <returns>A new UIHandler object</returns>
        /// <remarks></remarks>
        internal UIHandler HandlerFromAssocHandler(IAssocHandler assoc, string ext)
        {
            UIHandler newHandler = null;
            string exepath = null;

            assoc.GetName(out exepath);

            foreach (var u in handlers)
            {
                if ((exepath ?? "") == (u.ExePath ?? ""))
                {
                    u.AddExt(ext);
                    return u;
                }
            }

            newHandler = new UIHandler(assoc, this);
            newHandler.AddExt(ext);

            handlers.Add(newHandler);

            PopulatingUIHandlers?.Invoke(this, new UIHandlerEnumEventArgs(newHandler));

            return newHandler;
        }

        /// <summary>
        /// Builds the system file type cache.
        /// </summary>
        /// <returns>The number of system file type entries enumerated.</returns>
        /// <remarks></remarks>
        public int Populate(bool fireEvent = true)
        {
            fileTypes.Clear();
            handlers.Clear();

            NativeShell.ClearHandlerCache();

            var n = Registry.ClassesRoot.GetSubKeyNames();
            SystemFileType sf;

            int x = 0;
            int y;
            var sn2 = new List<string>();

            foreach (var sn in n)
            {
                if (sn.Substring(0, 1) == ".")
                {
                    sn2.Add(sn);
                }
            }

            y = sn2.Count;

            foreach (var sn in sn2)
            {
                if ((sn == null)) continue;

                sf = SystemFileType.FromExtension(sn, this);

                if (sf is object)
                {
                    fileTypes.Add(sf);
                    x += 1;
                    if (fireEvent)
                    {
                        // If fireEvent AndAlso x Mod 10 = 0 Then
                        Populating.Invoke(this, new FileTypeEnumEventArgs(sf, x, y));
                        Task.Yield();
                    }
                }
            }

            QuickSort.Sort(fileTypes, new SysFileTypeComp().Compare);
            QuickSort.Sort(handlers, new UIHandlerComp().Compare);

            OnPropertyChanged(nameof(UIHandlers));
            OnPropertyChanged(nameof(FileTypes));

            return x;
        }

        private bool disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    fileTypes = null;
                    handlers = null;
                }
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnPropertyChanged([CallerMemberName] string e = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }
    }
}