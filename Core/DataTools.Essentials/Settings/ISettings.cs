using System;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Interface for basic settings
    /// </summary>
    public interface ISettings : IEquatable<ISetting>
    {
        /// <summary>
        /// True if settings can be removed
        /// </summary>
        bool CanRemoveSettings { get; }

        /// <summary>
        /// True if settings can be created
        /// </summary>
        bool CanCreateSettings { get; }

        /// <summary>
        /// True if settings can be added
        /// </summary>
        bool CanAddSettings { get; }

        /// <summary>
        /// True if settings can be changed
        /// </summary>
        bool CanChangeSettings { get; }


        /// <summary>
        /// True if the settings can be cleared.
        /// </summary>
        bool CanClearSettings { get; }

        /// <summary>
        /// Gets the value for the specified key
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">Optional default value</param>
        /// <returns></returns>
        object GetValue(string key, object defaultValue = default);

        /// <summary>
        /// Sets the value for the specified key
        /// </summary>
        /// <param name="key">The key to set</param>
        /// <param name="value">The value to set</param>
        void SetValue(string key, object value);

        /// <summary>
        /// Gets the value for the specified key
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">Optional default value</param>
        /// <returns></returns>
        T GetValue<T>(string key, T defaultValue = default);

        /// <summary>
        /// Clear all settings (if supported)
        /// </summary>
        /// <returns>True if the settings were cleared.</returns>
        bool ClearSettings();


        /// <summary>
        /// Sets the value for the specified key
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="value"/></typeparam>
        /// <param name="key">The key to set</param>
        /// <param name="value">The value to set</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// Gets the setting for the specified key
        /// </summary>
        /// <param name="key">The key of the setting to retrieve</param>
        /// <returns>A setting</returns>
        /// <remarks>
        /// Behavior is not defined if key does not exist.
        /// </remarks>
        ISetting GetSetting(string key);

        /// <summary>
        /// Gets the setting for the specified key
        /// </summary>
        /// <typeparam name="T">The type of value the setting stores</typeparam>
        /// <param name="key">The key of the setting to retrieve</param>
        /// <returns>A setting</returns>
        ISetting<T> GetSetting<T>(string key);

        /// <summary>
        /// Sets the setting for the specified key
        /// </summary>
        /// <param name="setting">The setting to store</param>
        void SetSetting(ISetting setting);


        /// <summary>
        /// Sets the setting for the specified key
        /// </summary>
        /// <typeparam name="T">The type of value the setting stores</typeparam>
        /// <param name="setting">The setting to store</param>
        void SetSetting<T>(ISetting<T> setting);

        /// <summary>
        /// Create or update a setting with the specified key and value
        /// </summary>
        /// <typeparam name="T">The type of value the setting stores</typeparam>
        /// <param name="key">The key of the new setting</param>
        /// <param name="value">The initial value of the new setting</param>
        /// <returns></returns>
        ISetting<T> CreateSetting<T>(string key, T value);

        /// <summary>
        /// Returns true if the settings contains the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);
        
        /// <summary>
        /// Remove the setting at the specified key
        /// </summary>
        /// <param name="key">The key of the setting to remove</param>
        /// <returns>True if successful</returns>
        bool RemoveKey(string key);

        /// <summary>
        /// Remove the specified setting
        /// </summary>
        /// <param name="setting">The setting to remove</param>
        /// <returns></returns>
        bool Remove(ISetting setting);

        /// <summary>
        /// Gets or sets the specified setting to the specified value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[string key] { get; set; }

        /// <summary>
        /// True if the settings can be read and written individually.
        /// </summary>
        bool Atomic { get; }

        /// <summary>
        /// Gets a <see cref="Uri"/> instance that specifies the persisted location for settings
        /// </summary>
        Uri Location { get; }
    }

}
