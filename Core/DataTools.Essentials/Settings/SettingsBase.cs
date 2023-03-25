using System;
using System.Collections.Generic;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Base class for a simple settings object
    /// </summary>
    public abstract class SettingsBase : ISettings
    {
        /// <summary>
        /// The <see cref="string"/> for the location of storage persistence
        /// </summary>
        protected string location;

        /// <summary>
        /// Gets or sets the list of converters that will be used.
        /// </summary>
        public virtual IList<SettingsTypeConverter> Converters { get; set; } = null;

        /// <summary>
        /// Create a new settings class
        /// </summary>
        /// <param name="location">The location of the persisted storage</param>
        /// <param name="atomic">True if the settings can be read and written individually.</param>
        /// <param name="converters">Optional list of converters that will be used.</param>
        public SettingsBase(string location, bool atomic, IEnumerable<SettingsTypeConverter> converters = null)
        {            
            this.location = location;
            if (converters != null) Converters = new List<SettingsTypeConverter>(converters);
            Atomic = atomic;
        }

        /// <summary>
        /// Create a new settings class
        /// </summary>
        /// <param name="atomic">True if the settings can be read and written individually.</param>
        /// <param name="converters">Optional list of converters that will be used.</param>
        public SettingsBase(bool atomic, IEnumerable<SettingsTypeConverter> converters = null)
        {
            this.location = null;
            if (converters != null) Converters = new List<SettingsTypeConverter>(converters);
            Atomic = atomic;
        }

        /// <inheritdoc/>
        public virtual bool Atomic { get; protected set; }

        /// <inheritdoc/>
        public abstract bool CanRemoveSettings { get; }

        /// <inheritdoc/>
        public abstract bool CanCreateSettings { get; }
        /// <inheritdoc/>
        public abstract bool CanAddSettings { get; }
        /// <inheritdoc/>
        public abstract bool CanChangeSettings { get; }

        /// <inheritdoc/>
        public abstract bool CanClearSettings { get; }

        /// <inheritdoc/>
        public virtual string Location => location;

        /// <inheritdoc/>
        public abstract object this[string key] { get; set; }

        /// <inheritdoc/>
        public abstract bool ContainsKey(string key);
        /// <inheritdoc/>
        public abstract ISetting<T> CreateSetting<T>(string key, T value);
        
        /// <summary>
        /// Returns true if the settings collection contains a setting with an equal key and value for the provided setting.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(ISetting other);

        /// <inheritdoc/>
        public abstract IEnumerator<ISetting> GetEnumerator();
        /// <inheritdoc/>
        public abstract ISetting GetSetting(string key);
        /// <inheritdoc/>
        public abstract ISetting<T> GetSetting<T>(string key);
        /// <inheritdoc/>
        public abstract object GetValue(string key, object defaultValue = default);
        /// <inheritdoc/>
        public abstract T GetValue<T>(string key, T defaultValue = default);

        /// <inheritdoc/>
        public abstract bool Remove(ISetting setting);
        /// <inheritdoc/>
        public abstract bool RemoveKey(string key);
        /// <inheritdoc/>
        public abstract void SetSetting(ISetting setting);
        /// <inheritdoc/>
        public abstract void SetSetting<T>(ISetting<T> setting);
        /// <inheritdoc/>
        public abstract void SetValue(string key, object value);
        /// <inheritdoc/>
        public abstract void SetValue<T>(string key, T value);

        /// <inheritdoc/>
        public abstract bool ClearSettings();

        /// <inheritdoc/>
        public override string ToString()
        {
            return Location?.ToString() ?? base.ToString();
        }
    }

}
