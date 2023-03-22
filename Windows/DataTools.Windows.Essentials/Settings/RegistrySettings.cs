using DataTools.Essentials.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private string baseKey;

        /// <summary>
        /// Create a new registry settings object
        /// </summary>
        /// <param name="baseKey">The base key</param>
        public RegistrySettings(string baseKey) : base(true)
        {
            this.baseKey = baseKey;
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
        public override bool ContainsKey(string key)
        {
            using (var reg = Registry.CurrentUser.CreateSubKey(baseKey))
            {
                var vals = reg.GetValueNames();
                return vals.Contains(key);
            }
        }

        /// <inheritdoc/>
        public override ISetting<T> CreateSetting<T>(string key, T value)
        {
            SetRegValue(key, value);
            return new ProgramSetting<T>(key, value, this);
        }

        /// <inheritdoc/>
        public override bool Equals(ISetting other)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IEnumerator<ISetting> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ISetting GetSetting(string key)
        {
            return GetSetting<object>(key);
        }

        /// <inheritdoc/>
        public override ISetting<T> GetSetting<T>(string key)
        {
            var val = GetRegValue<T>(key);
            return new ProgramSetting<T>(key, val, this);
        }

        /// <inheritdoc/>
        public override object GetValue(string key)
        {
            return GetRegValue<object>(key);
        }

        /// <inheritdoc/>
        public override T GetValue<T>(string key)
        {
            return GetRegValue<T>(key);
        }

        /// <inheritdoc/>
        public override bool Remove(ISetting setting)
        {
            return RemoveKey(setting.Key);
        }

        /// <inheritdoc/>
        public override bool RemoveKey(string key)
        {
            using(var reg = Registry.CurrentUser.OpenSubKey(baseKey))
            {
                try
                {
                    reg.DeleteValue(key, true);
                    return true;
                }
                catch { return false; }                
            }
        }

        /// <inheritdoc/>
        public override void SetSetting(ISetting setting)
        {
            SetRegValue(setting.Key, setting.Value);
        }

        /// <inheritdoc/>
        public override void SetSetting<T>(ISetting<T> setting)
        {
            CreateSetting(setting.Key, setting.Value);
        }

        /// <inheritdoc/>
        public override void SetValue(string key, object value)
        {
            SetRegValue(key, value);
        }

        /// <inheritdoc/>
        public override void SetValue<T>(string key, T value)
        {
            SetRegValue(key, value);
        }

        /// <summary>
        /// Set the registry value for the specified value
        /// </summary>
        /// <typeparam name="T">The type of the value to set</typeparam>
        /// <param name="valName">The name of the registry value</param>
        /// <param name="value">The value</param>
        protected void SetRegValue<T>(string valName, T value)
        {
            using (var reg = Registry.CurrentUser.CreateSubKey(baseKey))
            {
                if (typeof(T) == typeof(Guid) && value is Guid g)
                {
                    var str = g.ToString("d");
                    reg.SetValue(valName, str);
                }
                else if (typeof(T) == typeof(DateTime) && value is DateTime dt)
                {
                    var str = dt.ToString("O");
                    reg.SetValue(valName, str);
                }
                else if (typeof(T) == typeof(System.Drawing.Color) && value is System.Drawing.Color c)
                {
                    var str = PrintArgb(c.A, c.R, c.G, c.B);
                    reg.SetValue(valName, str);
                }
                else
                {
                    reg.SetValue(valName, value);
                }
            }
        }

        /// <summary>
        /// Get the registry value for the specified value
        /// </summary>
        /// <typeparam name="T">The type of the value to get</typeparam>
        /// <param name="valName">The name of the registry value</param>
        /// <param name="defaultValue">Default value (optional)</param>
        /// <returns></returns>
        protected T GetRegValue<T>(string valName, T defaultValue = default) 
        {
            using (var reg = Registry.CurrentUser.CreateSubKey(baseKey))
            {
                if (typeof(T) == typeof(Guid) && defaultValue is Guid gu)
                {
                    var str = gu.ToString("d");
                    gu = Guid.Parse((string)reg.GetValue(valName, str));
                    return (T)(object)gu;
                }
                else if (typeof(T) == typeof(DateTime) && defaultValue is DateTime dt)
                {
                    var str = dt.ToString("O");
                    dt = DateTime.Parse((string)reg.GetValue(valName, str));
                    return (T)(object)dt;
                }
                else if (typeof(T) == typeof(System.Drawing.Color) && defaultValue is System.Drawing.Color c)
                {
                    var str = PrintArgb(c.A, c.R, c.G, c.B);
                    byte a, r, g, b;

                    str = (string)reg.GetValue(valName, str);
                    GetArgb(str, out a, out r, out g, out b);
                    c = System.Drawing.Color.FromArgb(a, r, g, b);
                    return (T)(object)c;
                }
                else
                {
                    return (T)reg.GetValue(valName, defaultValue);
                }
            }
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
