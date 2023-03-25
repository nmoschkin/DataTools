using System.Collections.Generic;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Base class for a simple settings object
    /// </summary>
    public abstract class SettingsBase : ISettings
    {
        /// <summary>
        /// Create a new settings class
        /// </summary>
        /// <param name="atomic">True if the settings can be read and written individually.</param>
        public SettingsBase(bool atomic)
        {
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
    }

}
