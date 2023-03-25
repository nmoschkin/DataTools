using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Implements JSON-file based settings
    /// </summary>
    public class JsonSettings : SettingsBase
    {
        private Dictionary<string, ISetting> settings = new Dictionary<string, ISetting>();

        /// <summary>
        /// Create a new JSON settings object
        /// </summary>
        public JsonSettings() : base(true)
        {
        }

        /// <summary>
        /// Create a new JSON settings object
        /// </summary>
        /// <param name="fileName">The name of the file that contains persisted storage</param>
        /// <param name="load">True to load the data from persistence on instantiation</param>
        public JsonSettings(string fileName, bool load) : base(new Uri("file:///" + Path.GetFullPath(fileName)), true)
        {            
            if (load) this.LoadSettings(fileName);
        }

        /// <summary>
        /// Create a new JSON settings object
        /// </summary>
        /// <param name="location">The location of the resource that contains persisted storage</param>
        /// <param name="load">True to load the data from persistence on instantiation</param>
        public JsonSettings(Uri location, bool load) : base(location, true)
        {
            if (load) this.LoadSettings(location);
        }

        /// <summary>
        /// Gets or sets the name of the current JSON file.
        /// </summary>
        public string Filename
        {
            get => location.AbsolutePath;
            set
            {
                if (Uri.TryCreate(value, UriKind.Absolute, out var location))
                {
                    this.location = location;
                }
                else if (File.Exists(value))
                {
                    if (value.StartsWith("\\\\") || value.StartsWith("//"))
                    {
                        this.location = new Uri("file://" + Path.GetFullPath(value));
                    }
                    else
                    {
                        this.location = new Uri("file:///" + Path.GetFullPath(value));
                    }
                }
            }
        }

        /// <summary>
        /// Load JSON settings from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public JsonSettings(string fileName): this()
        {
            LoadSettings(fileName);
        }

        /// <inheritdoc/>
        public override bool ClearSettings()
        {
            settings.Clear();

            try
            {
                if (!string.IsNullOrEmpty(Filename) && File.Exists(Filename))
                {
                    File.Delete(Filename);
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Performs the act of clearing the settings and loading the new JSON data
        /// </summary>
        /// <param name="json">The string to load</param>
        protected virtual void InternalLoadJson(string json)
        {
            var serials = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            settings.Clear();

            foreach (var kvp in serials)
            {
                var n = new ProgramSetting(kvp.Key, kvp.Value);
                settings.Add(kvp.Key, n);
            }
        }

        /// <summary>
        /// Performs the act of serializing the settings data and returns a JSON string.
        /// </summary>
        /// <returns>A JSON string with the current settings.</returns>
        protected virtual string InternalSaveJson()
        {
            var dict = new Dictionary<string, object>();

            foreach (var kvp in settings)
            {
                dict.Add(kvp.Key, kvp.Value);
            }

            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }

        /// <summary>
        /// Loads settings from the specified file
        /// </summary>
        /// <param name="fileName">The name of the file to load</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public virtual void LoadSettings(string fileName)
        {
            if (File.Exists(fileName))
            {
                LoadSettings(new Uri("file:///" + fileName));
            }
        }

        /// <summary>
        /// Loads settings from the specified resource
        /// </summary>
        /// <param name="location">The name of the resource to load</param>
        /// <param name="encoding">Optional encoding for network resources (defaults to UTF-8)</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public virtual void LoadSettings(Uri location, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            string json;

            if (location.IsFile)
            {
                json = File.ReadAllText(location.LocalPath);
            }
            else if (location.IsUnc)
            {
                json = File.ReadAllText(location.AbsolutePath);
            }
            else
            {
                throw new NotSupportedException("For off-site resources, you must override this method");
            }

            InternalLoadJson(json);
            this.location = location;
        }

        /// <summary>
        /// Saves settings to the specified resource
        /// </summary>
        /// <param name="location">The location of the resource to save to</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public virtual void SaveSettings(Uri location)
        {
            var json = InternalSaveJson();

            if (location.IsFile)
            {
                File.WriteAllText(location.LocalPath, json);
            }
            else if (location.IsUnc)
            {
                File.WriteAllText(location.AbsolutePath, json);
            }
            else
            {
                throw new NotSupportedException("For off-site resources, you must override this method");
            }
        }

        /// <summary>
        /// Saves settings to the specified file
        /// </summary>
        /// <param name="fileName">The filename of the resource to save to</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public virtual void SaveSettings(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Filename = fileName;
            SaveSettings(location);
        }


        /// <summary>
        /// Saves settings to the last known location
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public virtual void SaveSettings()
        {            
            SaveSettings(location);
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException"></exception>
        public override object this[string key]
        {
            get
            {
                if (settings.ContainsKey(key))
                {
                    return settings[key].Value;
                }

                throw new KeyNotFoundException();
            }
            set
            {
                if (settings.ContainsKey((string)key))
                {
                    settings[key].Value = value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }

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
        public override bool ContainsKey(string key)
        {
            return settings.ContainsKey(key);
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException"></exception>
        public override ISetting<T> CreateSetting<T>(string key, T value)
        {
            if (!settings.ContainsKey(key))
            {
                var v = new ProgramSetting<T>(key, value, this);
                settings.Add(key, v);
                return v;
            }
            else
            {
                if (settings[key] is ISetting<T> setting)
                {
                    setting.Value = value;
                    return setting;
                }
                
                throw new InvalidOperationException("Key exists and is not same type as target");
            }
        }

        /// <inheritdoc/>
        public override bool Equals(ISetting other)
        {
            if (!settings.ContainsKey(other.Key)) return false;
            var test = settings[other.Key];

            return test.Key == other.Key && test.Value == other.Value;
        }

        /// <inheritdoc/>
        public override IEnumerator<ISetting> GetEnumerator()
        {
            var vals = settings.Values;

            foreach (var val in vals)
            {
                yield return val;
            }

            yield break;
        }

        /// <inheritdoc/>
        public override ISetting GetSetting(string key)
        {
            return settings[key];
        }

        /// <inheritdoc/>
        public override ISetting<T> GetSetting<T>(string key)
        {
            return settings[key] as ISetting<T>;
        }

        /// <inheritdoc/>
        public override object GetValue(string key, object defaultValue = default)
        {
            if (!settings.TryGetValue(key, out var v))
            {
                CreateSetting(key, defaultValue);
            }

            return settings[key].Value;
        }

        /// <inheritdoc/>
        public override T GetValue<T>(string key, T defaultValue = default)
        {
            if (!settings.TryGetValue(key, out var v))
            {
                CreateSetting(key, defaultValue);
            }

            return (T)settings[key].Value;
        }

        /// <inheritdoc/>
        public override bool Remove(ISetting setting)
        {
            return settings.Remove(setting.Key);
        }

        /// <inheritdoc/>
        public override bool RemoveKey(string key)
        {
            return settings.Remove(key);
        }

        /// <inheritdoc/>
        public override void SetSetting(ISetting setting)
        {
            settings[setting.Key] = setting;
        }

        /// <inheritdoc/>
        public override void SetSetting<T>(ISetting<T> setting)
        {
            settings[setting.Key] = setting;
        }

        /// <inheritdoc/>
        public override void SetValue(string key, object value)
        {
            settings[key].Value = value;
        }

        /// <inheritdoc/>
        public override void SetValue<T>(string key, T value)
        {
            settings[key].Value = value;
        }
    }
}
