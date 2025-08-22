using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Base class for settings type converters
    /// </summary>
    public abstract class SettingsTypeConverter
    {
        /// <summary>
        /// Check if the converter can convert from the specified type.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns></returns>
        public abstract bool CanConvertFrom(Type type);

        /// <summary>
        /// Check if the converter can convert to the specified type.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns></returns>
        public abstract bool CanConvertTo(Type type);

        /// <summary>
        /// Convert an <see cref="ISetting"/> object into the specified destination type.
        /// </summary>
        /// <param name="setting">The setting to convert</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="destinationType">The destination type</param>
        /// <returns>The converted value</returns>
        public abstract object ConvertTo(ISetting setting, Type sourceType, Type destinationType);

        /// <summary>
        /// Convert a value from the specified type into a new <see cref="ISetting"/>.
        /// </summary>
        /// <param name="key">The name of the new setting</param>
        /// <param name="value">The value to convert</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="destinationType">The destination type</param>
        /// <param name="parent">Optional parent <see cref="ISettings"/> implementation.</param>
        /// <returns>The new setting</returns>
        public abstract ISetting ConvertFrom(string key, object value, Type sourceType, Type destinationType, ISettings parent = null);
    }

}
