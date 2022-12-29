using DataTools.Desktop;
using DataTools.Memory;
using DataTools.Shell.Native;
using DataTools.Win32;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataTools.Desktop_Old
{
    /// <summary>
    /// Provides a file-system-locked object to represent the contents of a directory.
    /// </summary>
    public class DirectoryObject : IShellFolderObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string displayName;
        private Icon icon;
        private Bitmap iconBmp;
        private StandardIcons iconSize = StandardIcons.Icon48;
        private IShellFolderObject parent;

        private bool special;

        private IShellFolder shellFld;

        private List<IShellObject> children = new List<IShellObject>();
        private List<IShellFolderObject> folders = new List<IShellFolderObject>();

        public bool IsBound => shellFld != null;

        public DirectoryObject(string parsingName) : this(parsingName, true, StandardIcons.Icon48)
        {
        }

        public DirectoryObject(string name, bool initialize, StandardIcons iconSize) : this(name, !System.IO.Directory.Exists(name) || name.Contains("::"), initialize, iconSize)
        {
        }

        internal DirectoryObject(string parsingName, bool special, bool initialize, StandardIcons iconSize)
        {
            if (string.IsNullOrEmpty(parsingName))
            {
                throw new ArgumentNullException(nameof(parsingName));
            }

            this.special = special;
            this.iconSize = iconSize;

            #region Unused Code - Why keeping!?

            // Dim mm As New MemPtr
            // Dim res As HResult = SHCreateItemFromParsingName(ParsingName, Nothing, ShellIIDGuid.IShellItemUuid, shitem)
            // Dim fp As String = Nothing
            // Dim parts As String() = Nothing

            // If (res = HResult.Ok) Then
            // shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, mm)

            // fp = mm
            // ParsingName = fp

            // mm.CoTaskMemFree()
            // ElseIf (ParsingName.Substring(0, 2) = "::") Then
            // parts = ParsingName.Split("\")

            // res = SHCreateItemFromParsingName(parts(parts.Length - 1), Nothing, ShellIIDGuid.IShellItemUuid, shitem)
            // If res = HResult.Ok Then
            // shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, mm)

            // fp = mm
            // ParsingName = fp

            // mm.CoTaskMemFree()
            // End If
            // End If

            // res = SHCreateItemFromParsingName("shell:" + If(parts IsNot Nothing, parts(parts.Length - 1), If(fp, ParsingName)), Nothing, ShellIIDGuid.IShellItemUuid, shitem)

            // If (res = HResult.Ok) Then
            // Dim ip As nint

            // res = shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, ip)
            // mm = ip

            // If (res = HResult.Ok) Then
            // CanonicalName = If(fp Is Nothing, "shell:" + ParsingName, mm.ToString())
            // If (ParsingName Is Nothing) Then ParsingName = mm

            // _Path = ParsingName

            // mm.CoTaskMemFree()
            // shitem.GetDisplayName(ShellItemDesignNameOptions.Normal, mm)

            // DisplayName = mm
            // mm.CoTaskMemFree()
            // End If
            // End If

            // shitem = Nothing

            #endregion Unused Code - Why keeping!?

            try
            {
                if (this.special)
                {
                    // let's see if we can parse it.
                    IShellItem shitem = null;

                    CoTaskMemPtr mem;

                    var riid = ShellIIDGuid.IShellItemUuid;
                    var res = NativeShell.SHCreateItemFromParsingName(parsingName, nint.Zero, ref riid, out shitem);

                    string absname = null;

                    if (res == HResult.Ok)
                    {
                        shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, out mem);

                        absname = (string)mem;

                        ParsingName = absname;
                        CanonicalName = absname;

                        shitem.GetDisplayName(ShellItemDesignNameOptions.Normal, out mem);

                        DisplayName = (string)mem;

                        this.special = true;

                        if (initialize)
                        {
                            Refresh(this.iconSize);
                        }
                        else
                        {
                            folders.Add(new DirectoryObject());
                            OnPropertyChanged(nameof(Folders));
                        }

                        return;
                    }

                    HResult localSHCreateItemFromParsingName()
                    {
                        var riid = ShellIIDGuid.IShellItemUuid;
                        return NativeShell.SHCreateItemFromParsingName("shell:" + (absname ?? parsingName), nint.Zero, ref riid, out shitem);
                    }

                    res = localSHCreateItemFromParsingName();

                    if (res == HResult.Ok)
                    {
                        shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, out mem);

                        CanonicalName = (string)mem;

                        if (ParsingName is null)
                        {
                            ParsingName = (string)mem;
                        }

                        shitem.GetDisplayName(ShellItemDesignNameOptions.Normal, out mem);
                        DisplayName = (string)mem;
                    }

                    shitem = null;
                    if (!string.IsNullOrEmpty(DisplayName) && !string.IsNullOrEmpty(parsingName))
                    {
                        this.special = true;
                        if (initialize)
                        {
                            Refresh(this.iconSize);
                        }
                        else
                        {
                            folders.Add(new DirectoryObject());
                            OnPropertyChanged(nameof(Folders));
                        }

                        return;
                    }
                }

                if (!string.IsNullOrEmpty(ParsingName))
                {
                    ParsingName = ParsingName;
                }
                else
                {
                    ParsingName = parsingName;
                    ParsingName = parsingName;
                }

                if (System.IO.Directory.Exists(ParsingName) == false)
                {
                    return;
                }
                // Throw New DirectoryNotFoundException("Directory Not Found: " & _Path)
                else
                {
                    DisplayName = Path.GetFileName(ParsingName);
                    CanonicalName = ParsingName;
                    parsingName = ParsingName;
                }

                if (initialize)
                {
                    Refresh(this.iconSize);
                }
                else
                {
                    folders.Add(new DirectoryObject());
                    OnPropertyChanged(nameof(Folders));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private DirectoryObject()
        {
            DisplayName = "";
        }

        /// <summary>
        /// Gets or sets the directory attributes.
        /// </summary>
        /// <returns></returns>
        public FileAttributes Attributes
        {
            get
            {
                return FileTools.GetFileAttributes(ParsingName);
            }

            set
            {
                FileTools.SetFileAttributes(ParsingName, value);
            }
        }

        /// <summary>
        /// Gets the canonical name of a known folder
        /// </summary>
        /// <returns></returns>
        public string CanonicalName { get; private set; }

        /// <summary>
        /// Returns the number of files in the directory.
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return children.Count;
            }
        }

        /// <summary>
        /// Returns the full ParsingName of the directory.
        /// </summary>
        /// <returns></returns>
        public string Directory
        {
            get
            {
                return ParsingName;
            }
        }

        /// <summary>
        /// Gets the display name of the folder
        /// </summary>
        /// <returns></returns>
        public string DisplayName
        {
            get
            {
                return displayName;
            }

            set
            {
                displayName = value;
            }
        }

        public ICollection<IShellFolderObject> Folders
        {
            get
            {
                return folders;
            }
        }

        /// <summary>
        /// Returns a Windows Forms compatible icon for the directory
        /// </summary>
        /// <returns></returns>
        public Icon Icon
        {
            get
            {
                if (icon is null)
                {
                    int? argiIndex = null;
                    icon = Resources.GetFileIcon(ParsingName, FileObject.StandardToSystem(iconSize), iIndex: ref argiIndex);
                }

                return icon;
            }
        }

        /// <summary>
        /// Returns a WPF-compatible icon image for the directory
        /// </summary>
        /// <returns></returns>
        public Bitmap IconImage
        {
            get
            {
                if (iconBmp is null)
                {
                    int? idx = 0;
                    iconBmp = Resources.IconToTransparentBitmap(Resources.GetFileIcon(ParsingName, FileObject.StandardToSystem(iconSize), ref idx));
                }

                return iconBmp;
            }
        }

        /// <summary>
        /// Gets or sets the default icon size for the directory.
        /// Individual files can override this setting, but they will be reset if this setting is changed while this directory is the parent of the file.
        /// </summary>
        /// <returns></returns>
        public StandardIcons IconSize
        {
            get
            {
                return iconSize;
            }
            set
            {
                if (iconSize == value && icon is object && iconBmp is object) return;

                iconSize = value;
                int? argiIndex = null;

                icon = Resources.GetFileIcon(ParsingName, FileObject.StandardToSystem(iconSize), iIndex: ref argiIndex);
                iconBmp = Resources.IconToTransparentBitmap(icon);

                if (children != null && children.Count > 0)
                {
                    foreach (var f in children)
                    {
                        f.IconSize = iconSize;
                    }
                }

                OnPropertyChanged();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns whether or not this directory is a special folder
        /// </summary>
        /// <returns></returns>
        public bool IsSpecial
        {
            get
            {
                return special;
            }
        }

        /// <summary>
        /// Get or set the creation time of the directory.
        /// </summary>
        /// <returns></returns>
        public DateTime CreationTime
        {
            get
            {
                DateTime c, a, m;
                FileTools.GetFileTime(ParsingName, out c, out a, out m);
                return c;
            }

            set
            {
                DateTime c, a, m;
                FileTools.GetFileTime(ParsingName, out c, out a, out m);
                FileTools.SetFileTime(ParsingName, value, a, m);
            }
        }

        /// <summary>
        /// Get or set the last access time of the directory.
        /// </summary>
        /// <returns></returns>
        public DateTime LastAccessTime
        {
            get
            {
                DateTime c, a, m;
                FileTools.GetFileTime(ParsingName, out c, out a, out m);
                return a;
            }

            set
            {
                DateTime c, a, m;
                FileTools.GetFileTime(ParsingName, out c, out a, out m);
                FileTools.SetFileTime(ParsingName, c, value, m);
            }
        }

        /// <summary>
        /// Get or set the last write time of the directory.
        /// </summary>
        /// <returns></returns>
        public DateTime LastWriteTime
        {
            get
            {
                DateTime c, a, m;
                FileTools.GetFileTime(ParsingName, out c, out a, out m);
                return m;
            }

            set
            {
                DateTime c, a, m;
                FileTools.GetFileTime(ParsingName, out c, out a, out m);
                FileTools.SetFileTime(ParsingName, c, a, value);
            }
        }

        /// <summary>
        /// Returns the parent directory object if one exists.
        /// </summary>
        /// <returns></returns>
        public IShellFolderObject Parent
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
        /// Gets the shell parsing name of a special folder
        /// </summary>
        /// <returns></returns>
        public string ParsingName { get; private set; }

        ICollection<IShellObject> IShellFolderObject.Children
        {
            get
            {
                return this;
            }
        }

        bool IShellObject.IsFolder { get => true; }

        /// <summary>
        /// Not implemented in directory
        /// </summary>
        /// <returns></returns>
        long IShellObject.Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal IShellFolder SysInterface
        {
            get
            {
                return shellFld;
            }
        }

        public bool CanMoveObject => ((Attributes & FileAttributes.ReadOnly) == 0) && !IsSpecial;

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
                d.Add(new DirectoryObject("QuickAccessFolder", true, true, iconSize));
            }

            if ((folderTypes & RootFolderTypes.MyComputer) == RootFolderTypes.MyComputer)
            {
                d.Add(new DirectoryObject("MyComputerFolder", true, true, iconSize));
            }

            if ((folderTypes & RootFolderTypes.NetworkPlaces) == RootFolderTypes.NetworkPlaces)
            {
                d.Add(new DirectoryObject("NetworkPlacesFolder", true, false, iconSize));
            }

            if ((folderTypes & RootFolderTypes.ControlPanel) == RootFolderTypes.ControlPanel)
            {
                d.Add(new DirectoryObject("ControlPanelFolder", true, true, iconSize));
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

        /// <summary>
        /// Refresh the contents of the directory.
        /// </summary>
        public void Refresh(StandardIcons? iconSize = null)
        {
            children.Clear();
            folders.Clear();

            FileObject fobj;
            DirectoryObject dobj;
            IShellItem shitem;
            IShellFolder shfld;
            IEnumIDList enumer;

            DataTools.Win32.Memory.MemPtr mm;
            var mm2 = new DataTools.Win32.Memory.MemPtr();

            string fp;

            string pname = ParsingName;

            if (pname is object && pname.LastIndexOf(@"\") == pname.Length - 1)
                pname = pname.Substring(0, pname.Length - 1);

            var argriid = ShellIIDGuid.IShellItemUuid;
            var res = NativeShell.SHCreateItemFromParsingName(ParsingName, nint.Zero, ref argriid, out shitem);

            int? argiIndex = null;

            this.iconSize = iconSize ?? this.iconSize;

            icon = Resources.GetFileIcon(ParsingName, FileObject.StandardToSystem(this.iconSize), iIndex: ref argiIndex);
            iconBmp = Resources.IconToTransparentBitmap(icon);

            if (res == HResult.Ok)
            {
                IShellItemPS pssh = null;
                IShellItem2 shitem2 = null;

                shellObj = shitem;

                var riid = ShellIIDGuid.IShellItemUuid;
                NativeShell.SHCreateItemFromParsingName(ParsingName, nint.Zero, ref riid, out pssh);
                shellObjPS = pssh;

                riid = ShellIIDGuid.IShellItem2Uuid;
                NativeShell.SHCreateItemFromParsingName(ParsingName, nint.Zero, ref riid, out shitem2);
                shellObj2 = shitem2;

                var fldbind = ShellBHIDGuid.ShellFolderObjectUuid;
                var fld2guid = ShellIIDGuid.IShellFolder2Uuid;

                shitem.BindToHandler(nint.Zero, ref fldbind, ref fld2guid, out shfld);
                shellFld = shfld;

                shfld.EnumObjects(nint.Zero, ShellFolderEnumerationOptions.Folders | ShellFolderEnumerationOptions.IncludeHidden | ShellFolderEnumerationOptions.NonFolders | ShellFolderEnumerationOptions.InitializeOnFirstNext, out enumer);

                if (enumer != null)
                {
                    var glist = new List<string>();
                    uint cf;
                    var ptr = nint.Zero;
                    string pout;

                    // mm.AllocCoTaskMem((MAX_PATH * 2) + 8)

                    mm2.Alloc(NativeShell.MAX_PATH * 2 + 8);

                    do
                    {
                        cf = 0U;
                        mm2.ZeroMemory(0L, NativeShell.MAX_PATH * 2 + 8);
                        res = enumer.Next(1U, out ptr, out cf);
                        mm = ptr;

                        if (cf == 0L)
                            break;

                        if (res != HResult.Ok)
                            break;

                        mm2.IntAt(0L) = 2;

                        shfld.GetDisplayNameOf(mm, (uint)ShellItemDesignNameOptions.ParentRelativeParsing, mm2.handle);
                        DataTools.Win32.Memory.MemPtr inv;

                        if (IntPtr.Size == 4)
                        {
                            inv = (nint)mm2.IntAt(1L);
                        }
                        else
                        {
                            inv = (nint)mm2.LongAt(1L);
                        }

                        if (inv.Handle != nint.Zero)
                        {
                            if (inv.CharAt(0L) != '\0')
                            {
                                fp = (string)inv;
                                var lpInfo = new SHFILEINFO();

                                // Dim sgfin As ShellFileGetAttributesOptions = 0,
                                // sgfout As ShellFileGetAttributesOptions = 0

                                int iFlags = User32.SHGFI_PIDL | User32.SHGFI_ATTRIBUTES;
                                lpInfo.dwAttributes = 0;
                                ptr = User32.SHGetItemInfo(mm.Handle, 0, ref lpInfo, Marshal.SizeOf<SHFILEINFO>(), iFlags);
                                if (ParsingName is object)
                                {
                                    if (pname.LastIndexOf(@"\") == pname.Length - 1)
                                        pname = pname.Substring(0, pname.Length - 1);
                                    pout = $@"{pname}\{fp}";
                                }
                                else
                                {
                                    pout = fp;
                                }

                                if (lpInfo.dwAttributes == 0)
                                {
                                    lpInfo.dwAttributes = (int)FileTools.GetFileAttributes(pout);
                                }

                                FileAttributes drat = (FileAttributes)(int)(lpInfo.dwAttributes);
                                if ((lpInfo.dwAttributes & (int)FileAttributes.Directory) == (int)FileAttributes.Directory && !File.Exists(pout))
                                {
                                    dobj = new DirectoryObject(pout, special, false, StandardIcons.Icon48);
                                    dobj.Parent = this;
                                    dobj.IconSize = this.iconSize;
                                    children.Add(dobj);
                                    folders.Add(dobj);
                                }
                                else
                                {
                                    fobj = new FileObject(pout, special, true, this.iconSize);
                                    fobj.Parent = this;
                                    fobj.IconSize = this.iconSize;
                                    children.Add(fobj);
                                }
                            }

                            inv.CoTaskMemFree();
                        }

                        mm.CoTaskMemFree();
                    }
                    while (res == HResult.Ok);
                    mm2.Free();
                }
            }

            OnPropertyChanged(nameof(Folders));
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(IconImage));
            OnPropertyChanged(nameof(IconSize));
            OnPropertyChanged(nameof(ParsingName));
            OnPropertyChanged(nameof(DisplayName));
        }

        void ICollection<IShellObject>.Add(IShellObject item)
        {
            ((ICollection<IShellObject>)children).Add(item);
        }

        void ICollection<IShellObject>.Clear()
        {
            ((ICollection<IShellObject>)children).Clear();
        }

        IEnumerator<IShellObject> IEnumerable<IShellObject>.GetEnumerator()
        {
            foreach (var child in children)
            {
                yield return child;
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<IShellObject>)this).GetEnumerator();

        bool ICollection<IShellObject>.Remove(IShellObject item)
        {
            return ((ICollection<IShellObject>)children).Remove(item);
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

        internal bool Remove(IShellObject item)
        {
            return children.Remove(item);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsLazyLoad { get; set; }

        public bool TryMoveObject(string newName)
        {
            if (!CanMoveObject) return false;
            try
            {
                FileTools.MoveFile(ParsingName, newName);
                ParsingName = newName;
                Refresh();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Property Experiments

        private IShellItemPS shellObjPS;
        private IShellItem2 shellObj2;
        private IShellItem shellObj;

        /// <summary>
        /// Returns the metadata proprerty store for this shell object.
        /// </summary>
        /// <returns></returns>
        public IList<PropertyKey> GetPropertyStore()
        {
            if (shellObjPS == null) return null;

            var g1 = ShellBHIDGuid.PropertyStoreUuid;
            var g2 = ShellIIDGuid.IPropertyStoreUuid;

            IPropertyStore p;

            var h = shellObjPS.BindToHandler(nint.Zero, ref g1, ref g2, out p);

            if (h != HResult.Ok) return null;

            PropertyKey pk = new PropertyKey();
            uint count;
            int i;

            h = p.GetCount(out count);
            if (h != HResult.Ok) return null;

            List<PropertyKey> pks = new List<PropertyKey>();

            for (i = 0; i < count; i++)
            {
                h = p.GetAt((uint)i, ref pk);
                if (h != HResult.Ok) return null;

                pks.Add(pk);
            }

            return pks;
        }

        /// <summary>
        /// Gets the metadata property with the specified key as the specified type (if available)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetPropertyValue<T>(string key)
        {
            var pk = PropertyKeys.FindByName(key);
            if (!pk.HasValue) return default;

            return (T)GetPropertyValue(pk.Value);
        }

        /// <summary>
        /// Gets the metadata property with the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetPropertyValue(string key)
        {
            var pk = PropertyKeys.FindByName(key);
            if (!pk.HasValue) return null;

            return GetPropertyValue(pk.Value);
        }

        /// <summary>
        /// Gets the metadata property with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetPropertyValue(PropertyKey key)
        {
            if (shellObjPS == null) return null;

            var g1 = ShellBHIDGuid.PropertyStoreUuid;
            var g2 = ShellIIDGuid.IPropertyStoreUuid;

            IPropertyStore p;

            var h = shellObjPS.BindToHandler(nint.Zero, ref g1, ref g2, out p);

            if (h != HResult.Ok) return null;

            try
            {
                PropVariant pv = new PropVariant();

                h = p.GetValue(ref key, pv);

                if (h == HResult.Ok) return pv.Value;
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public Dictionary<string, PropertyKey> GetPropertiesWithNames()
        {
            return PropertyKeys.PropListToDict(GetPropertyStore());
        }

        /// <summary>
        /// Set a metadata property with the specified key to the specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPropertyValue<T>(string key, T value)
        {
            var pk = PropertyKeys.FindByName(key);
            if (!pk.HasValue) return false;
            else return SetPropertyValue((PropertyKey)pk, value);
        }

        /// <summary>
        /// Set a metadata property with the specified key to the specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPropertyValue(string key, object value)
        {
            var pk = PropertyKeys.FindByName(key);
            if (!pk.HasValue) return false;
            else return SetPropertyValue((PropertyKey)pk, value);
        }

        /// <summary>
        /// Set a metadata property with the specified key to the specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPropertyValue(PropertyKey key, object value)
        {
            IPropertyStore p = null;
            var b = SetPropertyValue(key, value, true, ref p);
            Marshal.FinalReleaseComObject(p);
            return b;
        }

        /// <summary>
        /// Set multiple metadata properties at once
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SetPropertyValues(Dictionary<string, object> values)
        {
            IPropertyStore p = null;
            bool b = true;

            foreach (var kv in values)
            {
                var pk = PropertyKeys.FindByName(kv.Key);
                if (!pk.HasValue)
                {
                    b = false;
                    continue;
                }

                b &= SetPropertyValue((PropertyKey)pk, kv.Value, false, ref p);

                if (p == null)
                {
                    b = false;
                    break;
                }
            }

            if (p != null)
            {
                p.Commit();

                Marshal.FinalReleaseComObject(p);
                p = null;
                GC.Collect(0);
            }

            return b;
        }

        /// <summary>
        /// Set multiple metadata properties at once
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SetPropertyValues(Dictionary<PropertyKey, object> values)
        {
            IPropertyStore p = null;
            bool b = true;

            foreach (var kv in values)
            {
                b &= SetPropertyValue(kv.Key, kv.Value, false, ref p);

                if (p == null)
                {
                    b = false;
                    break;
                }
            }

            if (p != null)
            {
                p.Commit();
                Marshal.FinalReleaseComObject(p);
                p = null;
                GC.Collect(0);
            }

            return b;
        }

        private bool SetPropertyValue(PropertyKey key, object value, bool commit, ref IPropertyStore p)
        {
            if (shellObjPS == null)
            {
                return false;
            }

            HResult h;

            if (p == null)
            {
                var g1 = ShellBHIDGuid.PropertyStoreUuid;
                var g2 = ShellIIDGuid.IPropertyStoreUuid;

                h = (HResult)shellObj2.GetPropertyStore(GetPropertyStoreOptions.ReadWrite, g2, out p);

                if (h != HResult.Ok)
                {
                    var str = NativeError.FormatLastError((uint)h);

                    h = shellObjPS.BindToHandler(nint.Zero, ref g1, ref g2, out p);

                    if (h != HResult.Ok) return false;
                }
            }

            if (p == null) return false;

            try
            {
                PropVariant pv = PropVariant.FromObject(value);

                h = p.SetValue(ref key, pv);
                if (commit) p.Commit();

                if (h != HResult.Ok) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Property Experiments
    }
}