using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Specifies options for mapping a data class to an <see cref="IProgramSerializable"/> implementation
    /// </summary>
    public class SettingsMapOptions
    {
        /// <summary>
        /// Create a property map of the specified <see cref="Type"/> using the specified <see cref="SettingsMapOptions"/>
        /// </summary>
        /// <param name="type">The type to map</param>
        /// <param name="options">The options</param>
        /// <param name="currPath">The current path (or null)</param>
        /// <param name="props">Properties of <paramref name="type"/> (optional)</param>
        /// <returns>A map of string/object pairs, where object is either another dictionary or <see cref="PropertyInfo"/></returns>
        public static Dictionary<string, object> MapSettings(Type type, SettingsMapOptions options, string currPath = null, PropertyInfo[] props = null)
        {
            currPath = currPath ?? "";
            props = props ?? type.GetProperties((options.IncludeNonPublic ? 0 : BindingFlags.Public) | BindingFlags.Instance);
            
            var lobj = new Dictionary<string, object>();

            foreach(var prop in props)
            {
                if (!options.IncludeReadOnly && !prop.CanWrite) continue;

                if (options.ExcludeNames?.Contains(prop.Name) ?? true) continue;
                if (options.ExcludeTypes?.Contains(prop.PropertyType) ?? true) continue;

                var npt = $"{currPath}\\{prop.Name}";
                if (options.ExcludePaths?.Contains(npt) ?? true) continue;

                var prote = type.GetProperties((options.IncludeNonPublic ? 0 : BindingFlags.Public) | BindingFlags.Instance);

                if (prote.Length > 0)
                {
                    var submap = MapSettings(prop.PropertyType, options, npt, prote);
                    if (submap != null && submap.Count > 0) 
                    {
                        lobj.Add(npt, submap);
                    }
                    
                }                
                else 
                {
                    lobj.Add(npt, prop);
                }
            }

            return lobj;
        }

        /// <summary>
        /// Gets or sets the <see cref="SettingsTypeConverter"/> instances that will be used for the map
        /// </summary>
        public IList<SettingsTypeConverter> Converters { get; set; } = null;

        /// <summary>
        /// Gets or sets a list of types to exclude
        /// </summary>
        public IList<Type> ExcludeTypes { get; set; } = null;

        /// <summary>
        /// Gets or sets a list of paths to exclude
        /// </summary>
        public IList<string> ExcludePaths { get; set; } = null;

        /// <summary>
        /// Gets or sets a list of pure names to exclude (no paths)
        /// </summary>
        public IList<string> ExcludeNames { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether or not to include read-only properties in the map (not recommended)
        /// </summary>
        public bool IncludeReadOnly { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether or not to include non-public properties in the map (not recommended)
        /// </summary>
        public bool IncludeNonPublic { get; set; } = false;
    }


}
