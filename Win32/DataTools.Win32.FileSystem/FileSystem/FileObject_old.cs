using DataTools.Desktop;
using DataTools.Memory;
using DataTools.Shell.Native;
using DataTools.Win32;

using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataTools.Desktop_Old
{
    /// <summary>
    /// Provides a file-system-locked object to represent a file.
    /// </summary>
    public class FileObject : IShellObject, IPropertyBag
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string displayName;
        private Icon icon;
        private Bitmap iconBmp;
        private StandardIcons iconSize = StandardIcons.Icon48;
        private IShellFolderObject parent;

        private bool special;

        private IShellItemPS shellObjPS;
        private IShellItem2 shellObj2;
        private IShellItem shellObj;

        private SystemFileType fileType;

        public bool IsBound => shellObj != null;

        /// <summary>
        /// Advanced initialization of FileObject.  Use this for items in special folders.
        /// </summary>
        /// <param name="parsingName">The shell parsing name of the file.</param>
        /// <param name="isSpecial">Is the file known to be special?</param>
        /// <param name="initialize">True to get file info and load icons.</param>
        /// <param name="iconSize">Default icon size.  This can be changed with the <see cref="IconSize"/> property.</param>
        internal FileObject(string parsingName, bool isSpecial, bool initialize, StandardIcons iconSize = StandardIcons.Icon48)
        {
            if (string.IsNullOrEmpty(parsingName))
            {
                throw new ArgumentNullException(nameof(parsingName));
            }

            this.special = true; // isSpecial;
            ParsingName = parsingName;
            this.iconSize = iconSize;

            try
            {
                var mm = new CoTaskMemPtr(0, true, false);
                var riid = ShellIIDGuid.IShellItemUuid;
                HResult res;

                if (this.special)
                {
                    // let's see if we can parse it.
                    IShellItem shitem = null;

                    res = NativeShell.SHCreateItemFromParsingName(parsingName, IntPtr.Zero, ref riid, out shitem);
                    string fp = null;

                    if (res == HResult.Ok)
                    {
                        IShellItemPS shitemps = null;
                        IShellItem2 shitem2 = null;

                        NativeShell.SHCreateItemFromParsingName(parsingName, IntPtr.Zero, ref riid, out shitemps);
                        shellObjPS = shitemps;

                        riid = ShellIIDGuid.IShellItem2Uuid;
                        NativeShell.SHCreateItemFromParsingName(parsingName, IntPtr.Zero, ref riid, out shitem2);
                        shellObj2 = shitem2;

                        shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, out mm);
                        fp = (string)mm;

                        ParsingName = fp;
                        CanonicalName = fp;

                        shitem.GetDisplayName(ShellItemDesignNameOptions.Normal, out mm);

                        DisplayName = (string)mm;

                        this.special = true;

                        if (initialize)
                            Refresh();

                        shellObj = shitem;

                        return;
                    }

                    HResult localSHCreateItemFromParsingName()
                    {
                        var riid = ShellIIDGuid.IShellItemUuid;
                        return NativeShell.SHCreateItemFromParsingName("shell:" + (fp ?? parsingName), IntPtr.Zero, ref riid, out shitem);
                    }

                    res = localSHCreateItemFromParsingName();

                    if (res == HResult.Ok)
                    {
                        shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, out mm);
                        CanonicalName = (string)mm;

                        if (ParsingName is null)
                        {
                            ParsingName = (string)mm;
                        }

                        shitem.GetDisplayName(ShellItemDesignNameOptions.Normal, out mm);
                        DisplayName = (string)mm;
                    }

                    shellObj = shitem;
                    shitem = null;

                    if (!string.IsNullOrEmpty(DisplayName) && !string.IsNullOrEmpty(ParsingName))
                    {
                        this.special = true;
                        if (initialize)
                            Refresh(this.iconSize);
                        return;
                    }
                }

                if (File.Exists(parsingName) == false)
                {
                    if (!this.special)
                    {
                        throw new FileNotFoundException("File Not Found: " + parsingName);
                    }
                }
                else if (initialize)
                {
                    Refresh(this.iconSize);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Create a new FileObject from the given filename.
        /// If the file does not exist, an exception will be thrown.
        /// </summary>
        /// <param name="filename"></param>
        public FileObject(string filename, bool initialize, StandardIcons iconSize) : this(filename, (!File.Exists(filename) || filename.Contains("::")), initialize, iconSize)
        {
        }

        public FileObject(string filename) : this(filename, true, StandardIcons.Icon48)
        {
        }

        /// <summary>
        /// Create a blank file object.
        /// </summary>
        internal FileObject()
        {
        }

        /// <summary>
        /// Gets or sets the file attributes.
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
        /// Gets the canonical name of a special file.
        /// </summary>
        /// <returns></returns>
        public string CanonicalName { get; private set; }

        /// <summary>
        /// Get the containing directory of the file.
        /// </summary>
        /// <returns></returns>
        public string Directory
        {
            get
            {
                return Path.GetDirectoryName(ParsingName);
            }

            set
            {
                if (!Move(value))
                {
                    throw new AccessViolationException("Unable to move file.");
                }
            }
        }

        /// <summary>
        /// Gets the display name of the file
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

        /// <summary>
        /// Get the full ParsingName of the file.
        /// </summary>
        /// <returns></returns>
        public string Filename
        {
            get
            {
                return ParsingName;
            }

            internal set
            {
                if (ParsingName is object)
                {
                    if (!FileTools.MoveFile(ParsingName, value))
                    {
                        throw new AccessViolationException("Unable to rename/move file.");
                    }
                }
                else if (!File.Exists(value))
                {
                    throw new FileNotFoundException("File Not Found: " + Filename);
                }

                ParsingName = value;
                Refresh();
            }
        }

        /// <summary>
        /// Returns the file type description
        /// </summary>
        /// <returns></returns>
        public string FileType
        {
            get
            {
                if (fileType is null)
                    return "Unknown";
                return fileType.Description;
            }
        }

        /// <summary>
        /// Returns the file type icon
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Icon FileTypeIcon
        {
            get
            {
                if (fileType is null)
                    return null;
                return fileType.DefaultIcon;
            }
        }

        /// <summary>
        /// Returns the WPF-compatible file type icon image
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Bitmap FileTypeIconImage
        {
            get
            {
                if (fileType is null)
                    return null;
                return fileType.DefaultImage;
            }
        }

        /// <summary>
        /// Returns a Windows Forms compatible icon for the file
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Icon Icon
        {
            get
            {
                if (special | (parent is object && parent.IsSpecial))
                {
                    if (icon is null)
                    {
                        int? argiIndex = null;
                        icon = Resources.GetFileIcon(ParsingName, StandardToSystem(iconSize), iIndex: ref argiIndex);
                    }
                }

                if (icon is object)
                    return icon;
                else
                    return FileTypeIcon;
            }
        }

        /// <summary>
        /// Returns a WPF-compatible icon image for the file
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Bitmap IconImage
        {
            get
            {
                if (special | (parent is object && parent.IsSpecial))
                {
                    if (iconBmp is null)
                    {
                        int? idx = 0;
                        iconBmp = Resources.IconToTransparentBitmap(Resources.GetFileIcon(ParsingName, StandardToSystem(iconSize), ref idx));
                    }
                }

                if (iconBmp is object)
                    return iconBmp;
                else
                    return FileTypeIconImage;
            }
        }

        /// <summary>
        /// Gets or sets the default icon size for the file.
        /// Individual files can override this setting, but they will be reset if the IconSize property of the parent directory is changed.
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
                if (iconSize == value)
                    return;
                iconSize = value;
                Refresh(iconSize);
            }
        }

        public bool IsFolder { get; private set; } = false;

        /// <summary>
        /// Returns whether or not this file is a special file / in a special folder
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
        /// Get or set the creation time of the file.
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
        /// Get or set the last access time of the file.
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
        /// Get or set the last write time of the file.
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
        /// Gets the shell parsing name of a special file
        /// </summary>
        /// <returns></returns>
        public string ParsingName { get; private set; }

        /// <summary>
        /// Get the size of the file, in bytes.
        /// </summary>
        /// <returns></returns>
        public long Size
        {
            get
            {
                return FileTools.GetFileSize(ParsingName);
            }
        }

        /// <summary>
        /// Return the file type object.
        /// </summary>
        /// <returns></returns>
        public SystemFileType TypeObject
        {
            get
            {
                return fileType;
            }
        }

        internal IShellItem SysInterface
        {
            get
            {
                return shellObj;
            }
        }

        public bool CanMoveObject
        {
            get
            {
                return true;
            }
        }

        public Dictionary<string, PropertyKey> GetPropertiesWithNames()
        {
            return PropertyKeys.PropListToDict(GetPropertyStore());
        }

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

            var h = shellObjPS.BindToHandler(IntPtr.Zero, ref g1, ref g2, out p);

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

            var h = shellObjPS.BindToHandler(IntPtr.Zero, ref g1, ref g2, out p);

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

        /// <summary>
        /// Attempt to move the file to a new directory.
        /// </summary>
        /// <param name="newDirectory">Destination directory.</param>
        /// <returns>True if successful.</returns>
        public bool Move(string newDirectory)
        {
            if (special)
                return false;
            if (newDirectory.Substring(newDirectory.Length - 1, 1) == @"\")
                newDirectory = newDirectory.Substring(0, newDirectory.Length - 1);
            if (!System.IO.Directory.Exists(newDirectory))
                return false;
            string p = Path.GetFileName(ParsingName);
            string f = newDirectory + @"\" + p;
            if (FileTools.MoveFile(ParsingName, f))
            {
                ParsingName = f;
                Refresh();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Refresh the state of the file object from the disk.
        /// </summary>
        /// <param name="iconSize">The size of the icon to fetch from the system.</param>
        /// <returns></returns>
        public void Refresh(StandardIcons? iconSize = default)
        {
            if (iconSize is null)
                iconSize = this.iconSize;
            else
                this.iconSize = (StandardIcons)iconSize;

            if (!File.Exists(ParsingName))
                return;

            fileType = SystemFileType.FromExtension(Path.GetExtension(ParsingName), size: this.iconSize);

            if (special | (parent is object && parent.IsSpecial))
            {
                var st = StandardToSystem(this.iconSize);
                int? argiIndex = null;
                icon = Resources.GetFileIcon(ParsingName, st, iIndex: ref argiIndex);
                iconBmp = Resources.IconToTransparentBitmap(icon);
            }

            // if we are no longer in the directory of the original parent, set to null
            if (!special && parent is object)
            {
                DirectoryObject v = (DirectoryObject)parent;
                if ((v.Directory.ToLower() ?? "") != (Directory.ToLower() ?? ""))
                {
                    v.Remove(this);
                    parent = null;
                }
            }

            // get the shell interface for the specified non-special file item.
            // we can use this to get more interfaces from the system.

            if (shellObj == null && !special)
            {
                IShellItem shitem = null;
                IShellItemPS pssh = null;
                IShellItem2 shitem2 = null;

                var riid = ShellIIDGuid.IShellItemUuid;

                HResult res = NativeShell.SHCreateItemFromParsingName(ParsingName, IntPtr.Zero, ref riid, out shitem);

                if (res == HResult.Ok)
                {
                    shellObj = shitem;

                    NativeShell.SHCreateItemFromParsingName(ParsingName, IntPtr.Zero, ref riid, out pssh);
                    shellObjPS = pssh;

                    riid = ShellIIDGuid.IShellItem2Uuid;
                    NativeShell.SHCreateItemFromParsingName(ParsingName, IntPtr.Zero, ref riid, out shitem2);
                    shellObj2 = shitem2;
                }
            }

            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(IconImage));
            OnPropertyChanged(nameof(IconSize));
            OnPropertyChanged(nameof(ParsingName));
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(LastWriteTime));
            OnPropertyChanged(nameof(LastAccessTime));
            OnPropertyChanged(nameof(CreationTime));
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

        public override string ToString()
        {
            return DisplayName ?? Filename;
        }

        internal static Resources.SystemIconSizes StandardToSystem(StandardIcons stdIcon)
        {
            Resources.SystemIconSizes st;
            switch (stdIcon)
            {
                case StandardIcons.Icon16:
                    {
                        st = Resources.SystemIconSizes.Small;
                        break;
                    }

                case StandardIcons.Icon32:
                    {
                        st = Resources.SystemIconSizes.Large;
                        break;
                    }

                case StandardIcons.Icon48:
                case StandardIcons.Icon64:
                    {
                        st = Resources.SystemIconSizes.ExtraLarge;
                        break;
                    }

                default:
                    {
                        st = Resources.SystemIconSizes.Jumbo;
                        break;
                    }
            }

            return st;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

                    h = shellObjPS.BindToHandler(IntPtr.Zero, ref g1, ref g2, out p);

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
    }
}