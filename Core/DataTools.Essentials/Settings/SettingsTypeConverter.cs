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

    /// <summary>
    /// Converter for types that implement a static 'Parse(string)' method
    /// </summary>
    public class ParseableTypeConverter : SettingsTypeConverter
    {
        /// <summary>
        /// Try to get the static string parsing method (<paramref name="parseMethod"/>) from the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="parseMethod">Receives method info for the 'Parse' method, if found.</param>
        /// <returns>True if the <paramref name="parseMethod"/> was successfully retrieved.</returns>
        public static bool TryGetParseMethod(Type type, out MethodInfo parseMethod)
        {
            parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });

            if (parseMethod != null && parseMethod.IsStatic)
            {
                return true;
            }
            
            return false;
        }

        /// <inheritdoc/>
        public override bool CanConvertFrom(Type type)
        {
            if (type == typeof(string)) return true;
            return TryGetParseMethod(type, out _);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(Type type)
        {
            if (type == typeof(string)) return true;
            return TryGetParseMethod(type, out _);
        }

        /// <inheritdoc/>
        public override ISetting ConvertFrom(string key, object value, Type sourceType, Type destinationType, ISettings parent = null)
        {
            if (value is string s && TryGetParseMethod(destinationType, out var mtd))
            {
                return new ProgramSetting(key, mtd.Invoke(null, new object[] { s }), parent);
            }
            else if (!(value is string) && TryGetParseMethod(sourceType, out _) && destinationType == typeof(string))
            {
                return new ProgramSetting(key, value?.ToString(), parent);
            }

            throw new NotImplementedException("Type is neither parseable nor string");
        }

        /// <inheritdoc/>
        public override object ConvertTo(ISetting setting, Type sourceType, Type destinationType)
        {
            var value = setting.Value;

            if (value is string s && TryGetParseMethod(destinationType, out var mtd))
            {
                return mtd.Invoke(null, new object[] { s });
            }
            else if (!(value is string) && TryGetParseMethod(sourceType, out _) && destinationType == typeof(string))
            {
                return value?.ToString();
            }

            throw new NotImplementedException("Type is neither parseable nor string");
        }
    }

    /// <summary>
    /// Type converter resolver static utility class
    /// </summary>
    public static class ConverterResolver
    {
        private static readonly List<SettingsTypeConverter> knownConverters = new List<SettingsTypeConverter>();

        /// <summary>
        /// Gets all current known settings converter instances.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<SettingsTypeConverter> GetKnownConverters() => new ReadOnlyCollection<SettingsTypeConverter>(knownConverters);

        /// <summary>
        /// Clears the known converter cache.
        /// </summary>
        public static void ClearKnownConverterCache()
        {
            knownConverters.Clear();
        }

        private static SettingsTypeConverter Resolve(Type type, bool _from, bool _to, string propertyName = null, bool cache = true)
        {
            SettingsTypeConverter kte = null;

            if (propertyName == null)
            {
                if (!type.IsClass && !type.IsInterface && type.GetProperties().Length == 0) 
                {
                    throw new ArgumentException("We can't work with that.");
                }

                if (cache) kte = knownConverters.Where(x => (!_from || x.CanConvertFrom(type)) && (!_to || x.CanConvertTo(type))).FirstOrDefault();

                if (kte != null)
                {
                    return kte;
                }

                var cattr = type.GetCustomAttributes().Where(x => x is SettingsTypeConverterAttribute)?.ToList();
                if (cattr != null)
                {
                    foreach (SettingsTypeConverterAttribute atype in cattr)
                    {
                        var pt = atype.ConverterType;
                        var newconv = (SettingsTypeConverter)Activator.CreateInstance(pt);
                        if (newconv != null)
                        {
                            if (cache) knownConverters.Add(newconv);
                            if (_from)
                            {
                                if (newconv.CanConvertFrom(type))
                                {
                                    return newconv;
                                }
                            }
                            else
                            {
                                if (newconv.CanConvertTo(type))
                                {
                                    return newconv;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var propInfo = type.GetProperty(propertyName);

                if (propInfo != null)
                {

                    if (cache) kte = knownConverters.Where(x => x.CanConvertFrom(propInfo.PropertyType)).FirstOrDefault();
                    if (kte != null)
                    {
                        return kte;
                    }

                    var cattr = propInfo.GetCustomAttributes().Where(x => x is SettingsTypeConverterAttribute)?.ToList();
                    if (cattr != null)
                    {
                        foreach (SettingsTypeConverterAttribute atype in cattr)
                        {
                            var pt = atype.ConverterType;
                            var newconv = (SettingsTypeConverter)Activator.CreateInstance(pt);
                            if (newconv != null)
                            {
                                if (cache) knownConverters.Add(newconv);
                                if (newconv.CanConvertFrom(propInfo.PropertyType))
                                {
                                    return newconv;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Resolve the 'to' type converter for the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve a converter for.</param>
        /// <param name="propertyName">Optional property name within the specified type.</param>
        /// <param name="useCache">True to use and store cached instances, false to create a new instance.</param>
        /// <returns>The resolved type converter or null if no converter could be resolved.</returns>
        public static SettingsTypeConverter ResolveTo(Type type, string propertyName = null, bool useCache = true)
        {
            return Resolve(type, false, true, propertyName, useCache);
        }

        /// <summary>
        /// Resolve the 'from' type converter for the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve a converter for.</param>
        /// <param name="propertyName">Optional property name within the specified type.</param>
        /// <param name="useCache">True to use and store cached instances, false to create a new instance.</param>
        /// <returns>The resolved type converter or null if no converter could be resolved.</returns>
        public static SettingsTypeConverter ResolveFrom(Type type, string propertyName = null, bool useCache = true)
        {
            return Resolve(type, true, false, propertyName, useCache);
        }
    }

    /// <summary>
    /// Add a settings type converter attribute to a property or class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class SettingsTypeConverterAttribute : Attribute
    {
        private static readonly Type otv = typeof(object);
        private static readonly Type stv = typeof(SettingsTypeConverter);

        /// <summary>
        /// The converter type
        /// </summary>
        public Type ConverterType { get; }

        /// <summary>
        /// Create a new converter type attribute
        /// </summary>
        /// <param name="converterType">The type</param>
        public SettingsTypeConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
            var ctv = converterType;

            while(ctv != otv)
            {
                if (ctv == stv) break;
                ctv = ctv.BaseType;
            }

            if (ctv == otv) throw new ArgumentException("Type must be derived from SettingsTypeConverter");
        }

    }

}
