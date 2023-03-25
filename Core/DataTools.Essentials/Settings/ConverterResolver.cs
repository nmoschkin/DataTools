using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace DataTools.Essentials.Settings
{
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

        private static SettingsTypeConverter Resolve(Type type, bool _from, bool _to, string propertyName = null, bool cache = true, IEnumerable<SettingsTypeConverter> extraConverters = null)
        {
            SettingsTypeConverter kte = null;

            if (extraConverters != null)
            {
                kte = knownConverters.Where(x => (!_from || x.CanConvertFrom(type)) && (!_to || x.CanConvertTo(type))).FirstOrDefault();

                if (kte != null)
                {
                    return kte;
                }
            }

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
        /// <param name="extraConverters">Extra converters to include in lookup. If these are present, they are searched regardless of the value of <paramref name="useCache"/>.</param>
        /// <returns>The resolved type converter or null if no converter could be resolved.</returns>
        public static SettingsTypeConverter ResolveTo(Type type, string propertyName = null, bool useCache = true, IEnumerable<SettingsTypeConverter> extraConverters = null)
        {
            return Resolve(type, false, true, propertyName, useCache, extraConverters);
        }

        /// <summary>
        /// Resolve the 'from' type converter for the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve a converter for.</param>
        /// <param name="propertyName">Optional property name within the specified type.</param>
        /// <param name="useCache">True to use and store cached instances, false to create a new instance.</param>
        /// <param name="extraConverters">Extra converters to include in lookup. If these are present, they are searched regardless of the value of <paramref name="useCache"/>.</param>
        /// <returns>The resolved type converter or null if no converter could be resolved.</returns>
        public static SettingsTypeConverter ResolveFrom(Type type, string propertyName = null, bool useCache = true, IEnumerable<SettingsTypeConverter> extraConverters = null)
        {
            return Resolve(type, true, false, propertyName, useCache, extraConverters);
        }
    }

}
