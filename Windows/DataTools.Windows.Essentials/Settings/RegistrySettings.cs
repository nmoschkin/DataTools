using DataTools.Essentials.Settings;
using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataTools.Windows.Essentials.Settings
{

    /// <summary>
    /// Represents a registry setting
    /// </summary>
    public class RegistrySetting : ProgramSetting
    {

        /// <inheritdoc/>
        public RegistrySetting(string key, object value, ISettings parent) : base(key, value, parent)
        {
        }

        /// <inheritdoc/>
        public override object Value
        {
            get
            {
                if (TryGetParent(out var parent))
                {
                    base.Value = parent.GetValue<object>(Key);
                }
                return base.Value;
            }
            set
            {
                base.Value = value;

                if (TryGetParent(out var parent))
                {
                    parent.SetSetting(this);
                }
            }
        }

    }

    /// <summary>
    /// Represents a registry setting
    /// </summary>
    public class RegistrySetting<T> : ProgramSetting<T>
    {

        /// <inheritdoc/>
        public RegistrySetting(string key, T value, ISettings parent) : base(key, value, parent)
        {
        }

        /// <inheritdoc/>
        public override T Value
        {
            get
            {
                if (base.Value == null)
                {
                    if (TryGetParent(out var parent))
                    {
                        base.Value = parent.GetValue<T>(Key);
                    }
                }

                return base.Value;
            }
            set
            {
                base.Value = value;

                if (TryGetParent(out var parent))
                {
                    parent.SetValue(Key, base.Value);
                }
            }
        }

    }

    /// <summary>
    /// Registry-based application settings
    /// </summary>
    public class RegistrySettings : SettingsBase
    {
        private string baseKey;

        private List<ISetting> settings = new List<ISetting>();

        private readonly Type[] parseables = new Type[]
        {
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(float),
            typeof(decimal)
        };

        /// <summary>
        /// Generate a base key name based on the specified information.
        /// </summary>
        /// <param name="author">The author/company name</param>
        /// <param name="program">The application name</param>
        /// <param name="version">Optional version</param>
        /// <returns></returns>
        public static string GenerateBaseKeyName(string author, string program, string version = null)
        {
            return $"Software\\{author}\\{program}" + (version != null ? "\\" + version : "");
        }        
        
        /// <summary>
        /// Generate a base key name from an author and <see cref="Assembly"/> information
        /// </summary>
        /// <param name="author">The author/company</param>
        /// <param name="assembly">The assembly</param>
        /// <returns></returns>
        public static string GenerateBaseKeyName(string author, Assembly assembly)
        {
            var nm = AssemblyName.GetAssemblyName(assembly.Location);
            return GenerateBaseKeyName(author, nm.Name, nm.Version?.ToString());
        }

        /// <summary>
        /// Refresh the settings list from the registry.
        /// </summary>
        /// <returns></returns>
        public IList<ISetting> Refresh()
        {
            var regsets = GetSettings();
            settings.Clear();

            foreach (var k in regsets)
            {
                var key = k;

                if (key.StartsWith(BaseKey))
                {
                    key = key.Substring(BaseKey.Length);

                }
                
                var s = new RegistrySetting(key, null, this);
                settings.Add(s);
            }

            return settings.ToList();
        }

        /// <summary>
        /// Enumerate the settings for the specified relative key (no paths)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected List<string> GetSettings(string key = null)
        {
            key = key ?? BaseKey;
            var l = new List<string>();
            var r = new List<string>();

            using (var reg = Registry.CurrentUser.CreateSubKey(key))
            {
                var sks = reg.GetSubKeyNames();
                var vals = reg.GetValueNames();

                foreach (var val in vals)
                {
                    l.Add($"{key}\\{val}");
                }

                foreach (var sk in sks)
                {
                    r.Add($"{key}\\{sk}");
                }
            }

            foreach (var k in r)
            {
                l.AddRange(GetSettings(k));  
            }

            return l;
        }

        /// <summary>
        /// Opens the key for the setting at the specified <paramref name="path"/>
        /// </summary>
        /// <param name="path">The key of the registry key to open (relative to <see cref="BaseKey"/>)</param>
        /// <param name="keyName">Receives the name of the key to use to read and write the value</param>
        /// <returns>A <see cref="RegistryKey"/> object</returns>
        protected RegistryKey OpenKeyForSetting(string path, out string keyName)
        {
            path = path.Replace("/", "\\");

            var kn = Path.GetFileName(path);
            var pt = Path.GetDirectoryName(path);

            keyName = kn;

            if (string.IsNullOrEmpty(pt))
            {
                return Registry.CurrentUser.CreateSubKey(BaseKey);
            }
            else
            {
                return Registry.CurrentUser.CreateSubKey(BaseKey + "\\" + pt);
            }
        }

        /// <summary>
        /// Create a new registry settings object
        /// </summary>
        /// <param name="baseKey">The base key</param>
        public RegistrySettings(string baseKey) : base(true)
        {
            this.baseKey = baseKey;
        }

        /// <summary>
        /// Create a new registry settings object
        /// </summary>
        /// <param name="author">The author/company name</param>
        /// <param name="program">The application name</param>
        /// <param name="version">Optional version</param>
        public RegistrySettings(string author, string program, string version = null) : this(GenerateBaseKeyName(author, program, version))
        {
        }

        /// <summary>
        /// Create a new registry settings object
        /// </summary>
        /// <param name="author">The author/company name</param>
        /// <param name="assembly">The assembly</param>
        public RegistrySettings(string author, Assembly assembly) : this(GenerateBaseKeyName(author, assembly))
        {
        }

        /// <summary>
        /// Gets the base key for this registry settings set
        /// </summary>
        public string BaseKey => baseKey;

        /// <inheritdoc/>
        public override object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public override bool CanRemoveSettings => true;

        /// <inheritdoc/>
        public override bool CanCreateSettings => true;

        /// <inheritdoc/>
        public override bool CanAddSettings => true;

        /// <inheritdoc/>
        public override bool CanChangeSettings => true;

        /// <inheritdoc/>
        public override bool CanClearSettings => true;


        /// <inheritdoc/>
        public override bool ClearSettings()
        {
            var settings = GetSettings();
            
            settings.Clear();

            try
            {                
                using (var reg = Registry.CurrentUser.OpenSubKey(BaseKey, true))
                {
                    DeleteKey(reg);
                }

                Registry.CurrentUser.DeleteSubKey(BaseKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DeleteKey(RegistryKey regKey)
        {
            var sks = regKey.GetSubKeyNames();

            foreach (var sk in sks)
            {
                using (var ssb = regKey.OpenSubKey(sk, true))
                {
                    DeleteKey(ssb);
                }
                
                regKey.DeleteSubKey(sk);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsKey(string key)
        {
            using (var reg = OpenKeyForSetting(key, out var useKey))
            {
                var vals = reg.GetValueNames();
                return vals.Contains(useKey);
            }
        }

        /// <inheritdoc/>
        public override ISetting<T> CreateSetting<T>(string key, T value)
        {

            var ts = settings.Where(x => x.Key == key).FirstOrDefault();

            if (ts != null)
            {
                if (ts is RegistrySetting<T> rtz)
                {
                    rtz.Value = value;
                    return rtz;
                }
                else
                {
                    throw new InvalidOperationException("That key already exists but is not the correct data type.");
                }
            }

            OpenSetRegValue(key, value);

            var rts = new ProgramSetting<T>(key, value, this);
            settings.Add(rts);

            return rts;
        }

        /// <inheritdoc/>
        public override bool Equals(ISetting other)
        {
            foreach (var s in settings)
            {
                if (s.Key == other.Key)
                {
                    return s.Value?.Equals(other.Value) ?? other.Value == null;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override IEnumerator<ISetting> GetEnumerator()
        {
            foreach (var s in settings)
            {
                yield return s;
            }

            yield break;
        }

        /// <inheritdoc/>
        public override ISetting GetSetting(string key)
        {
            return GetSetting<object>(key);
        }

        /// <inheritdoc/>
        public override ISetting<T> GetSetting<T>(string key)
        {
            var ts = settings.Where(x => x.Key == key).FirstOrDefault();
            
            if (ts != null)
            {
                if (ts is RegistrySetting<T> rtz)
                {
                    return rtz;
                }
                else
                {
                    throw new InvalidOperationException("That key already exists but is not the correct data type.");
                }
            }

            var val = OpenGetRegValue<T>(key);
            var rts = new RegistrySetting<T>(key, val, this);

            settings.Add(rts);

            return rts;
        }

        /// <inheritdoc/>
        public override object GetValue(string key, object defaultValue = default)
        {
            return OpenGetRegValue<object>(key, defaultValue);
        }

        /// <inheritdoc/>
        public override T GetValue<T>(string key, T defaultValue = default)
        {
            return OpenGetRegValue<T>(key, defaultValue);
        }

        /// <inheritdoc/>
        public override bool Remove(ISetting setting)
        {
            var b = RemoveKey(setting.Key);

            if (settings.Contains(setting))
            {
                b |= settings.Remove(setting);
            }
            else
            {
                var ts = settings.Where(x => x.Key == setting.Key).FirstOrDefault();

                if (ts != null)
                {
                    b |= settings.Remove(ts);
                }
            }

            return b;
        }

        /// <inheritdoc/>
        public override bool RemoveKey(string key)
        {
            using(var reg = OpenKeyForSetting(key, out var useKey))
            {
                try
                {
                    reg.DeleteValue(useKey, true);
                    var ts = settings.Where(x => x.Key == key).FirstOrDefault();

                    if (ts != null)
                    {
                        return settings.Remove(ts);
                    }

                    return true;
                }
                catch { return false; }                
            }
        }

        /// <inheritdoc/>
        public override void SetSetting(ISetting setting)
        {
            OpenSetRegValue(setting.Key, setting.Value);
        }

        /// <inheritdoc/>
        public override void SetSetting<T>(ISetting<T> setting)
        {
            CreateSetting(setting.Key, setting.Value);
        }

        /// <inheritdoc/>
        public override void SetValue(string key, object value)
        {
            OpenSetRegValue(key, value);
        }

        /// <inheritdoc/>
        public override void SetValue<T>(string key, T value)
        {
            OpenSetRegValue(key, value);
        }

        /// <summary>
        /// Opens and sets the registry <paramref name="value"/> for the specified <paramref name="valuePath"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to set</typeparam>
        /// <param name="valuePath">The relative path of the registry value</param>
        /// <param name="value">The value</param>
        protected void OpenSetRegValue<T>(string valuePath, T value)
        {
            using (var reg = OpenKeyForSetting(valuePath, out var useKey))
            {
                SetRegValue(typeof(T), reg, useKey, value);
            }
        }

        /// <summary>
        /// Sets the <paramref name="value"/> for the specified <paramref name="regKey"/> to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="type">The type of the object to write.</param>
        /// <param name="regKey">The registry key instance (must be open for write)</param>
        /// <param name="valueName">The name of the value to write (no paths allowed)</param>
        /// <param name="value">The value to write</param>
        protected virtual void SetRegValue(Type type, RegistryKey regKey, string valueName, object value)
        {
            if (value != default && value.GetType() != type) throw new ArgumentException("Type of value parameter does not match type parameter");

            if (type == typeof(Guid) && value is Guid g)
            {
                var str = g.ToString("d");
                regKey.SetValue(valueName, str);
            }
            else if (type == typeof(DateTime) && value is DateTime dt)
            {
                var str = dt.ToString("O");
                regKey.SetValue(valueName, str);
            }
            else if (type == typeof(System.Drawing.Color) && value is System.Drawing.Color c)
            {
                var str = PrintArgb(c.A, c.R, c.G, c.B);
                regKey.SetValue(valueName, str);
            }
            else if (value is IEnumerable<string> strlist)
            {
                regKey.SetValue(valueName, strlist.ToArray());
            }
            else if (value is IEnumerable b)
            {
                var it = type.GetInterfaces().Where(x => x.Name.Contains("IEnumerable`1")).First();
                var gt = it.GetGenericArguments().FirstOrDefault();

                RegWriteEnumerable(regKey, valueName, gt, b);
            }
            else
            {

                if (parseables.Contains(type))
                {
                    var str = value.ToString();
                    regKey.SetValue(valueName, str);
                }
                else
                {
                    regKey.SetValue(valueName, value);
                }
            }
        }

        /// <summary>
        /// Opens and gets the registry <paramref name="value"/> for the specified <paramref name="valName"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to get</typeparam>
        /// <param name="valName">The name of the registry value</param>
        /// <param name="defaultValue">Default value (optional)</param>
        /// <returns></returns>
        protected T OpenGetRegValue<T>(string valName, T defaultValue = default)
        {
            using (var reg = OpenKeyForSetting(valName, out var useKey))
            {
                return (T)GetRegValue(typeof(T), reg, useKey, defaultValue);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reg"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected virtual object GetRegValue(Type type, RegistryKey reg, string valueName, object defaultValue = default) 
        {
            if (defaultValue != default && defaultValue.GetType() != type) throw new ArgumentException("Type of defaultValue parameter does not match type parameter");

            if (type == typeof(Guid))
            {
                Guid gu = (Guid)(defaultValue ?? Guid.Empty);
                var str = gu.ToString("d");
                gu = Guid.Parse((string)reg.GetValue(valueName, str));
                return gu;
            }
            else if (type == typeof(DateTime))
            {
                DateTime dt = (DateTime)(defaultValue ?? DateTime.MinValue);

                var str = dt.ToString("O");
                dt = DateTime.Parse((string)reg.GetValue(valueName, str));
                return dt;
            }
            else if (type == typeof(System.Drawing.Color))
            {
                System.Drawing.Color c = (System.Drawing.Color)(defaultValue ?? System.Drawing.Color.Empty);

                var str = PrintArgb(c.A, c.R, c.G, c.B);
                byte a, r, g, b;

                str = (string)reg.GetValue(valueName, str);
                GetArgb(str, out a, out r, out g, out b);
                c = System.Drawing.Color.FromArgb(a, r, g, b);
                return c;
            }                
            else if (type.GetInterfaces().Where(x => x.Name.Contains("ICollection`1")).Any() || type.GetInterfaces().Where(x => x.Name.Contains("IList`1")).Any())
            {
                var it = type.GetInterfaces().Where(x => x.Name.Contains("IEnumerable`1")).First();
                var gt = it.GetGenericArguments().FirstOrDefault();

                if (gt == typeof(string))
                {
                    string[] data = null;

                    if (defaultValue is IEnumerable<string> isu)
                    {
                        data = isu.ToArray();
                    }

                    data = (string[])reg.GetValue(valueName, data);

                    if (type == typeof(string[])) return data;
                    object outlist;
                    
                    try
                    {
                        outlist = Activator.CreateInstance(type, new object[] { data });
                        if (outlist != null) return outlist;
                    }
                    catch
                    {
                        try
                        {
                            outlist = Activator.CreateInstance(type);
                        }
                        catch
                        {
                            outlist = null;
                        }
                    }

                    if (outlist == null) throw new InvalidOperationException("Cannot create instance of " + type.FullName);

                    var amt = type.GetMethod("Add", new Type[] { it.GetGenericArguments()[0] });

                    if (amt != null)
                    {
                        foreach (var obj in data)
                        {
                            amt.Invoke(outlist, new object[] { obj });
                        }
                    }

                    return outlist;
                }
                else
                {
                    return RegReadEnumerable(reg, valueName, type, gt, (IEnumerable)defaultValue);
                }
            }
            else
            {
                if (parseables.Contains(type))
                {
                    var str = defaultValue.ToString();
                    str = (string)reg.GetValue(valueName, str);

                    var mtd = type.GetMethod("Parse", new Type[] { typeof (string) });
                    if (mtd != null)
                    {
                        var res = mtd.Invoke(null, new object[] { str });
                        if (res != null)
                        {
                            return res;
                        }
                    }

                    return defaultValue;
                }

                return reg.GetValue(valueName, defaultValue);
            }
        }
               
        private RegistryKey OpenEnumerableKey(RegistryKey currentKey, string valueName, bool writable = true)
        {
            return currentKey.CreateSubKey(valueName, writable);
        }

        private object RegReadEnumerable(RegistryKey currentKey, string valueName, Type type, Type dataType, IEnumerable defaultValue = null)
        {
            using(var reg = OpenEnumerableKey(currentKey, valueName))
            {

                var kvs = reg.GetValueNames();
                var ints = new List<int>();
                var objs = new List<object>();

                if (kvs != null && kvs.Length > 0)
                {
                    foreach (var k in kvs)
                    {
                        if (int.TryParse(k, out int y))
                        {
                            ints.Add(y);
                        }
                    }
                }

                if (ints.Count > 0)
                {
                    foreach (var i in ints)
                    {
                        var s = i.ToString();
                        var obj = GetRegValue(dataType, reg, s);
                        if (obj != null)
                        {
                            objs.Add(obj);
                        }
                    }
                }
                else if (defaultValue != null)
                {
                    int i = 0;

                    foreach (var obj in defaultValue)
                    {
                        SetRegValue(dataType, reg, (i++).ToString(), obj);
                        objs.Add(obj);
                    }
                }
                else
                {
                    return null;
                }

                return CreateEnumerable(type, dataType, objs);
            }
        }

        private void RegWriteEnumerable(RegistryKey currentKey, string valueName, Type dataType, IEnumerable data)
        {
            using (var reg = OpenEnumerableKey(currentKey, valueName))
            {
                var kvs = reg.GetValueNames();
                
                var ints = new List<int>();
                var objs = new List<object>();

                if (kvs != null && kvs.Length > 0)
                {
                    foreach (var k in kvs)
                    {
                        if (int.TryParse(k, out int y))
                        {
                            reg.DeleteValue(k);
                        }
                    }
                }

                int i = 0;

                foreach (var obj in data)
                {
                    SetRegValue(dataType, reg, (i++).ToString(), obj);
                }
            }
        }

        private object CreateEnumerable(Type type, Type dataType, IEnumerable data = null)
        {
            object retobj = null;

            ConstructorInfo altcons = null;

            if (!type.IsArray)
            {
                altcons = type.GetConstructors()
                    .Where(ac1 => ac1.GetParameters().Where(ac2 => ac2.ParameterType.Name.Contains("IEnumerable`1")).Any())
                    .Where(ac3 => ac3.GetParameters().Length == 1).FirstOrDefault();

            }

            if (type.IsArray || altcons != null)
            {
                var cnt = 0;
                foreach (var obj in data)
                {
                    cnt++;
                }

                Array arr = Array.CreateInstance(dataType, cnt);

                cnt = 0;
                foreach (var obj in data)
                {
                    arr.SetValue(obj, cnt++);
                }

                if (altcons != null)
                {
                    retobj = altcons.Invoke(new object[] { arr });
                    if (retobj != null) return retobj;
                }
                else
                {
                    return arr;
                }
            }
            else
            {
                retobj = Activator.CreateInstance(type);

                if (data == null) return retobj;

                var amt = type.GetMethod("Add", new Type[] { dataType });

                if (retobj != null && amt != null)
                {
                    foreach (var obj in data)
                    {
                        amt.Invoke(retobj, new object[] { obj });
                    }
                }
            }

            return retobj;
        }


        private string PrintArgb(byte a, byte r, byte g, byte b)
        {
            return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
        }

        private void GetArgb(string str, out byte a, out byte r, out byte g, out byte b)
        {
            if (str[0] == '#') str = str.Substring(1);
            uint parsed = uint.Parse(str, System.Globalization.NumberStyles.HexNumber);

            a = (byte)(parsed >> 24);
            r = (byte)(parsed >> 16);
            g = (byte)(parsed >> 8);
            b = (byte)(parsed);
        }

    }
}
