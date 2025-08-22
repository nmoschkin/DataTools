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
    /// Registry-based application settings
    /// </summary>
    public class RegistrySettings : SettingsBase
    {
        /// <summary>
        /// Gets the current working Registry Hive
        /// </summary>
        protected readonly RegistryKey Hive;

        private readonly Type[] parseables = new Type[]
        {
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(float),
            typeof(decimal)
        };

        private string baseKey;
        private List<ISetting> settings = new List<ISetting>();
        /// <summary>
        /// Create a new registry settings object
        /// </summary>
        /// <param name="baseKey">The base key</param>
        public RegistrySettings(string baseKey) : this(RegistryHive.CurrentUser, baseKey)
        {
        }

        /// <summary>
        /// Create a new registry settings object
        /// </summary>
        /// <param name="hive">The desired hive</param>
        /// <param name="baseKey">The base key</param>
        public RegistrySettings(RegistryHive hive, string baseKey) : base(true)
        {
            RegistryKey openHive;

            switch (hive)
            {
                case RegistryHive.LocalMachine:
                    openHive = Registry.LocalMachine;
                    break;

                case RegistryHive.CurrentConfig:
                    openHive = Registry.CurrentConfig;
                    break;

                case RegistryHive.ClassesRoot:
                    openHive = Registry.ClassesRoot;
                    break;

                case RegistryHive.Users:
                    openHive = Registry.Users;
                    break;

                case RegistryHive.CurrentUser:
                default:
                    openHive = Registry.CurrentUser;
                    break;
            }

            Hive = openHive;

            this.baseKey = baseKey;
            this.location = $"{Hive.Name}\\{baseKey}";
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
        public override bool CanAddSettings => true;

        /// <inheritdoc/>
        public override bool CanChangeSettings => true;

        /// <inheritdoc/>
        public override bool CanClearSettings => true;

        /// <inheritdoc/>
        public override bool CanCreateSettings => true;

        /// <inheritdoc/>
        public override bool CanRemoveSettings => true;

        /// <summary>
        /// Gets the name of the current registry hive
        /// </summary>
        public string CurrentHive => Hive.Name;

        /// <inheritdoc/>
        public override object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        /// <inheritdoc/>
        public override bool ClearSettings()
        {
            var settings = GetSettings();

            settings.Clear();

            try
            {
                using (var regKey = Hive.OpenSubKey(BaseKey, true))
                {
                    DeleteKey(regKey);
                }

                Hive.DeleteSubKey(BaseKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override bool ContainsKey(string key)
        {
            using (var regKey = OpenKeyForSetting(key, out var useKey))
            {
                var vals = regKey.GetValueNames();
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
            return OpenGetRegValue(key, defaultValue);
        }

        /// <inheritdoc/>
        public override T GetValue<T>(string key, T defaultValue = default)
        {
            return OpenGetRegValue<T>(key, defaultValue);
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

                if (key.StartsWith(BaseKey + "\\"))
                {
                    key = key.Substring(BaseKey.Length + 1);

                }
                
                var s = new RegistrySetting(key, null, this);
                settings.Add(s);
            }

            return settings.ToList();
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
            using (var regKey = OpenKeyForSetting(key, out var useKey))
            {
                try
                {
                    regKey.DeleteValue(useKey, true);
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

        /// <inheritdoc/>
        public override string ToString() => Location.ToString();

        /// <summary>
        /// Get the registry value from the specified open registry key with the specified type
        /// </summary>
        /// <param name="type">The type of value to retrieve</param>
        /// <param name="regKey">The open registry key to read the value from</param>
        /// <param name="valueName">The value name (no paths)</param>
        /// <param name="defaultValue">An optional default value to return if no value is found</param>
        /// <returns>A value if found, or <paramref name="defaultValue"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected virtual object GetRegValue(Type type, RegistryKey regKey, string valueName, object defaultValue = default)
        {
            if (defaultValue != default && defaultValue.GetType() != type) throw new ArgumentException("Type of defaultValue parameter does not match type parameter");

            if (type == typeof(Guid))
            {
                Guid gu = (Guid)(defaultValue ?? Guid.Empty);
                var str = gu.ToString("d");
                gu = Guid.Parse((string)regKey.GetValue(valueName, str));
                return gu;
            }
            else if (type == typeof(DateTime))
            {
                DateTime dt = (DateTime)(defaultValue ?? DateTime.MinValue);

                var str = dt.ToString("O");
                dt = DateTime.Parse((string)regKey.GetValue(valueName, str));
                return dt;
            }
            else if (type == typeof(System.Drawing.Color))
            {
                System.Drawing.Color c = (System.Drawing.Color)(defaultValue ?? System.Drawing.Color.Empty);

                var str = PrintArgb(c.A, c.R, c.G, c.B);
                byte a, r, g, b;

                str = (string)regKey.GetValue(valueName, str);
                GetArgb(str, out a, out r, out g, out b);
                c = System.Drawing.Color.FromArgb(a, r, g, b);
                return c;
            }
            else if (type == typeof(string) || defaultValue is string s)
            {
                return (string)regKey.GetValue(valueName, (string)(defaultValue ?? ""));
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

                    data = (string[])regKey.GetValue(valueName, data);

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
                    return RegReadEnumerable(regKey, valueName, type, gt, (IEnumerable)defaultValue);
                }
            }
            else
            {
                if (parseables.Contains(type))
                {
                    var str = defaultValue?.ToString() ?? "0";
                    str = (string)regKey.GetValue(valueName, str);

                    var mtd = type.GetMethod("Parse", new Type[] { typeof(string) });
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

                return regKey.GetValue(valueName, defaultValue);
            }
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

            using (var regKey = Hive.CreateSubKey(key))
            {
                var sks = regKey.GetSubKeyNames();
                var vals = regKey.GetValueNames();

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
        /// Opens and gets the value for the specified <paramref name="valueName"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to get</typeparam>
        /// <param name="valueName">The name of the registry value</param>
        /// <param name="defaultValue">Default value (optional)</param>
        /// <returns></returns>
        protected T OpenGetRegValue<T>(string valueName, T defaultValue = default)
        {
            using (var regKey = OpenKeyForSetting(valueName, out var useKey))
            {
                return (T)GetRegValue(typeof(T), regKey, useKey, defaultValue);
            }
        }


        /// <summary>
        /// Opens and gets the value for the specified <paramref name="valueName"/>.
        /// </summary>
        /// <param name="valueName">The name of the registry value</param>
        /// <param name="defaultValue">Default value (optional)</param>
        /// <returns></returns>
        protected object OpenGetRegValue(string valueName, object defaultValue = default)
        {
            using (var regKey = OpenKeyForSetting(valueName, out var useKey))
            {
                return GetRegValue(defaultValue?.GetType() ?? typeof(object), regKey, useKey, defaultValue);
            }
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
                return Hive.CreateSubKey(BaseKey);
            }
            else
            {
                return Hive.CreateSubKey(BaseKey + "\\" + pt);
            }
        }

        /// <summary>
        /// Opens and sets the registry <paramref name="value"/> for the specified <paramref name="valuePath"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to set</typeparam>
        /// <param name="valuePath">The relative path of the registry value</param>
        /// <param name="value">The value</param>
        protected void OpenSetRegValue<T>(string valuePath, T value)
        {
            using (var regKey = OpenKeyForSetting(valuePath, out var useKey))
            {
                SetRegValue(typeof(T), regKey, useKey, value);
            }
        }

        /// <summary>
        /// Opens and sets the registry <paramref name="value"/> for the specified <paramref name="valuePath"/>.
        /// </summary>
        /// <param name="valuePath">The relative path of the registry value</param>
        /// <param name="value">The value</param>
        protected void OpenSetRegValue(string valuePath, object value)
        {
            if (value == null) return;

            using (var regKey = OpenKeyForSetting(valuePath, out var useKey))
            {
                SetRegValue(value.GetType(), regKey, useKey, value);
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
            else if (value is string s)
            {
                regKey.SetValue(valueName, s);
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
        private void GetArgb(string str, out byte a, out byte r, out byte g, out byte b)
        {
            if (str[0] == '#') str = str.Substring(1);
            uint parsed = uint.Parse(str, System.Globalization.NumberStyles.HexNumber);

            a = (byte)(parsed >> 24);
            r = (byte)(parsed >> 16);
            g = (byte)(parsed >> 8);
            b = (byte)(parsed);
        }

        private RegistryKey OpenEnumerableKey(RegistryKey currentKey, string valueName, bool writable = true)
        {
            return currentKey.CreateSubKey(valueName, writable);
        }

        private string PrintArgb(byte a, byte r, byte g, byte b)
        {
            return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
        }

        private object RegReadEnumerable(RegistryKey currentKey, string valueName, Type type, Type dataType, IEnumerable defaultValue = null)
        {
            using(var regKey = OpenEnumerableKey(currentKey, valueName))
            {

                var kvs = regKey.GetValueNames();
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
                        var obj = GetRegValue(dataType, regKey, s);
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
                        SetRegValue(dataType, regKey, (i++).ToString(), obj);
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
            using (var regKey = OpenEnumerableKey(currentKey, valueName))
            {
                var kvs = regKey.GetValueNames();
                
                var ints = new List<int>();
                var objs = new List<object>();

                if (kvs != null && kvs.Length > 0)
                {
                    foreach (var k in kvs)
                    {
                        if (int.TryParse(k, out int y))
                        {
                            regKey.DeleteValue(k);
                        }
                    }
                }

                int i = 0;

                foreach (var obj in data)
                {
                    SetRegValue(dataType, regKey, (i++).ToString(), obj);
                }
            }
        }
    }
}
