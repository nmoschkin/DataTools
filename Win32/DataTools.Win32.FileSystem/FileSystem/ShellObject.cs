/* *************************************************
 * DataTools C# Utility Library
 * Copyright (C) 2011-2023 Nathaniel Moschkin
 * All Rights Reserved
 *
 * Licensed Under the Apache 2.0 License
 * *************************************************/

using DataTools.Desktop;
using DataTools.Memory;
using DataTools.Shell.Native;
using DataTools.Win32;

using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static DataTools.Shell.Native.NativeShell;

namespace DataTools.FileSystem
{
    public abstract class ShellObject : IShellObject, IPropertyBag
    {
        private string displayName;
        private string canonicalName;
        private string parsingName;

        private Icon icon;
        private Bitmap iconBmp;
        private StandardIcons iconSize = StandardIcons.Icon48;
        private IShellObject parent;
        private readonly bool special;

        private IShellItemPS shellObjPS;
        private IShellItem2 shellObj2;
        private IShellItem shellObj;

        /// <summary>
        /// Full initialization of <see cref="ShellObject"/>.
        /// </summary>
        /// <param name="parsingName">The shell parsing name of the item.</param>
        /// <param name="special">Is the item known to be special?</param>
        /// <param name="initialize">True to get item info and load icons.</param>
        /// <param name="iconSize">Default icon size. This can be changed with the <see cref="IconSize"/> property.</param>
        protected ShellObject(string parsingName, bool special, bool initialize, StandardIcons iconSize)
        {
            if (string.IsNullOrEmpty(parsingName))
            {
                throw new ArgumentNullException(nameof(parsingName));
            }

            this.special = special;
            this.iconSize = iconSize;
            this.parsingName = parsingName;

            BindToShell();
            if (initialize) Refresh();
        }

        private void BindToShell()
        {
            if (string.IsNullOrEmpty(parsingName))
            {
                throw new ArgumentNullException(nameof(parsingName));
            }

            try
            {
                CoTaskMemPtr strmem;
                var guid = ShellIIDGuid.IShellItemUuid;

                HResult res;

                // let's see if we can parse it.
                IShellItem shitem;
                res = SHCreateItemFromParsingName(parsingName, nint.Zero, ref guid, out shitem);

                string fp = null;

                if (res == HResult.Ok)
                {
                    IShellItemPS shitemps = null;
                    IShellItem2 shitem2 = null;

                    SHCreateItemFromParsingName(parsingName, nint.Zero, ref guid, out shitemps);
                    shellObjPS = shitemps;

                    guid = ShellIIDGuid.IShellItem2Uuid;
                    SHCreateItemFromParsingName(parsingName, nint.Zero, ref guid, out shitem2);
                    shellObj2 = shitem2;

                    shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, out strmem);
                    fp = (string)strmem;

                    ParsingName = fp;
                    CanonicalName = fp;

                    shitem.GetDisplayName(ShellItemDesignNameOptions.Normal, out strmem);

                    DisplayName = (string)strmem;
                    shellObj = shitem;

                    return;
                }

                guid = ShellIIDGuid.IShellItemUuid;
                res = SHCreateItemFromParsingName("shell:" + (fp ?? parsingName), nint.Zero, ref guid, out shitem);

                if (res == HResult.Ok)
                {
                    shitem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteParsing, out strmem);
                    CanonicalName = (string)strmem;

                    if (ParsingName is null)
                    {
                        ParsingName = (string)strmem;
                    }

                    shitem.GetDisplayName(ShellItemDesignNameOptions.Normal, out strmem);
                    DisplayName = (string)strmem;
                }

                shellObj = shitem;
                shitem = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public virtual void Refresh(StandardIcons? iconSize = null)
        {
            this.iconSize = iconSize ?? this.iconSize;

            if (!special)
            {
                if (IsFolder)
                {
                    if (!Directory.Exists(ParsingName)) return;
                }
                else
                {
                    if (!File.Exists(ParsingName)) return;
                }
            }

            if (parent is IShellFolderObject sf)
            {
                if (special || sf.IsSpecial)
                {
                    var st = StandardToSystem(this.iconSize);
                    int? argiIndex = null;
                    icon = Resources.GetFileIcon(ParsingName, st, iIndex: ref argiIndex);
                    iconBmp = Resources.IconToTransparentBitmap(icon);
                }

                // if we are no longer in the directory of the original parent, set to null
                if (!special)
                {
                    if (sf.ParsingName != Path.GetDirectoryName(ParsingName))
                    {
                        sf.Remove(this);
                    }
                }
            }

            // get the shell interface for the specified file item.
            // we can use this to get more interfaces from the system.
            if (shellObj == null)
            {
                IShellItem shitem;
                IShellItemPS pssh;
                IShellItem2 shitem2;

                var riid = ShellIIDGuid.IShellItemUuid;
                HResult res = SHCreateItemFromParsingName(ParsingName, nint.Zero, ref riid, out shitem);

                if (res == HResult.Ok)
                {
                    shellObj = shitem;

                    SHCreateItemFromParsingName(ParsingName, nint.Zero, ref riid, out pssh);
                    shellObjPS = pssh;

                    riid = ShellIIDGuid.IShellItem2Uuid;
                    SHCreateItemFromParsingName(ParsingName, nint.Zero, ref riid, out shitem2);
                    shellObj2 = shitem2;
                }
            }

            OnPropertyChanged(nameof(Attributes));
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(IconImage));
            OnPropertyChanged(nameof(IconSize));
            OnPropertyChanged(nameof(ParsingName));
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(Parent));
            OnPropertyChanged(nameof(LastWriteTime));
            OnPropertyChanged(nameof(LastAccessTime));
            OnPropertyChanged(nameof(CreationTime));
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

        public virtual FileAttributes Attributes
        {
            get
            {
                return FileTools.GetFileAttributes(parsingName);
            }

            set
            {
                FileTools.SetFileAttributes(parsingName, value);
            }
        }

        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        /// <summary>
        /// Returns an ICO for the shell object.
        /// </summary>
        /// <returns></returns>
        public virtual Icon Icon
        {
            get
            {
                if (icon is null)
                {
                    int? idx = null;
                    icon = Resources.GetFileIcon(ParsingName, StandardToSystem(iconSize), iIndex: ref idx);
                }

                return icon;
            }
            protected set
            {
                icon = value;
            }
        }

        public virtual Bitmap IconImage
        {
            get
            {
                if (iconBmp is null)
                {
                    int? idx = 0;
                    iconBmp = Resources.IconToTransparentBitmap(Resources.GetFileIcon(ParsingName, StandardToSystem(iconSize), ref idx));
                }

                return iconBmp;
            }
            protected set
            {
                iconBmp = value;
            }
        }

        public virtual StandardIcons IconSize
        {
            get
            {
                return iconSize;
            }

            set
            {
                if (iconSize == value) return;
                iconSize = value;
                Refresh(iconSize);
            }
        }

        public abstract bool IsFolder { get; }

        public bool IsSpecial => special;

        public IShellFolderObject Parent { get; protected internal set; }

        public string ParsingName
        {
            get => parsingName;
            protected set
            {
                parsingName = value;
            }
        }

        public string CanonicalName
        {
            get => canonicalName;
            protected set
            {
                canonicalName = value;
            }
        }

        public abstract long Size { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual DateTime CreationTime
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

        public virtual DateTime LastAccessTime
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

        public virtual DateTime LastWriteTime
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

        public virtual bool CanMoveObject
        {
            get
            {
                return ((Attributes & FileAttributes.ReadOnly) == 0) && !IsSpecial;
            }
        }

        public virtual void MoveObject(string value)
        {
            if (!CanMoveObject) throw new InvalidOperationException("This object cannot be moved.");

            if (parsingName != null)
            {
                if (!FileTools.MoveFile(parsingName, value))
                {
                    throw new AccessViolationException("Unable to rename/move file.");
                }
            }
            else if (!File.Exists(value))
            {
                throw new FileNotFoundException("File Not Found: " + DisplayName);
            }

            parsingName = value;
            Refresh();
        }

        public virtual bool TryMoveObject(string value)
        {
            if (!CanMoveObject) return false;

            if (parsingName != null)
            {
                if (!FileTools.MoveFile(parsingName, value))
                {
                    return false;
                }
            }
            else if (!File.Exists(value))
            {
                return false;
            }

            parsingName = value;
            Refresh();

            return true;
        }

        #region Property Bag

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

            // the command to send to the binder that tells it what to bind
            var g1 = ShellBHIDGuid.PropertyStoreUuid;

            // the UUID of the interface that will be bound.
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

        /// <summary>
        /// Get a dictionary that lists property names along with their key structures.
        /// </summary>
        /// <returns></returns>
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

            try
            {
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

                return b;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (p != null)
                {
                    p.Commit();

                    Marshal.FinalReleaseComObject(p);
                }
            }
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
                    var str = NativeErrorMethods.FormatLastError((uint)h);

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

        #endregion Property Bag

        public override string ToString()
        {
            return DisplayName;
        }
    }
}