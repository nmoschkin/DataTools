using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Specifies options for mapping a data class to an <see cref="IProgramSerializable"/> implementation
    /// </summary>
    public class SettingsMapOptions : ICloneable
    {
        /// <summary>
        /// Gets the default, empty settings options
        /// </summary>
        public static readonly SettingsMapOptions DefaultOptions = new SettingsMapOptions();

        /// <summary>
        /// Lookup a property in a type's hierarchy
        /// </summary>
        /// <param name="source">The source object</param>
        /// <param name="path">The path to the value</param>
        /// <param name="foundIn">The object the value was found in</param>
        /// <param name="property">The property info for the value</param>
        /// <param name="createInstance">True to create an instance of an object in the path of the property if that segment is null</param>
        /// <returns>True if successful</returns>
        /// <remarks>
        /// This method furnishes <see cref="PropertyInfo"/> data, it does not directly provide the value of the property.
        /// </remarks>
        public static bool LookupProperty(object source, string path, out object foundIn, out PropertyInfo property, bool createInstance = false)
        {
            path = path.Replace("/", "\\");
            var parts = new List<string>(path.Split('\\'));
            var t = source.GetType();

            if (parts.Count == 1)
            {
                property = t.GetProperty(parts[0]);
                foundIn = source;

                return true;
            }
            else
            {
                var p = t.GetProperty(parts[0]);
                if (p != null)
                {
                    var objnext = p.GetValue(source);
                    
                    parts.RemoveAt(0);
                    var newpath = string.Join("\\", parts);

                    if (objnext == null)
                    {
                        if (createInstance && p.CanWrite)
                        {
                            try
                            {
                                objnext = Activator.CreateInstance(p.PropertyType);                                
                                p.SetValue(source, objnext);
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine(ex);
#endif
                            }
                        }
                    }

                    if (objnext != null)
                    {
                        var bres = LookupProperty(objnext, newpath, out foundIn, out property);
                        if (bres) return bres;
                    }
                }
            }

            foundIn = null;
            property = null;
            return false;
        }

        /// <summary>
        /// Create a property map of the specified <see cref="Type"/> using the specified <see cref="SettingsMapOptions"/>
        /// </summary>
        /// <param name="type">The type to map</param>
        /// <param name="options">The options</param>
        /// <returns>A complex map</returns>
        /// <remarks>
        /// The return value is a dictionary whose key is the path of the property, <br />
        /// and whose value is a tuple of (<see cref="Type"/>, <see cref="PropertyInfo"/>)
        /// 
        /// </remarks>
        public static IDictionary<string, (Type, PropertyInfo)> MapSettings(Type type, SettingsMapOptions options = null)
        {
            options = options ?? DefaultOptions;
            return MapSettings(type, options, null, null);
        }

        /// <summary>
        /// Create a property map of the specified <see cref="Type"/> using the specified <see cref="SettingsMapOptions"/>
        /// </summary>
        /// <param name="type">The type to map</param>
        /// <param name="options">The options</param>
        /// <param name="currPath">The current path (or null)</param>
        /// <param name="props">Properties of <paramref name="type"/> (optional)</param>
        /// <returns>A complex map</returns>
        /// <remarks>
        /// The return value is a dictionary whose key is the path of the property, <br />
        /// and whose value is a tuple of (<see cref="Type"/>, <see cref="PropertyInfo"/>)
        /// 
        /// </remarks>
        private static IDictionary<string, (Type, PropertyInfo)> MapSettings(Type type, SettingsMapOptions options, string currPath, PropertyInfo[] props)
        {
            currPath = currPath ?? "";
            props = props ?? type.GetProperties((options.IncludeNonPublic ? 0 : BindingFlags.Public) | BindingFlags.Instance);
            
            var lobj = new SortedDictionary<string, (Type, PropertyInfo)>();

            foreach(var prop in props)
            {
                if (!options.IncludeReadOnly && !prop.CanWrite) continue;

                if (options.ExcludeNames?.Contains(prop.Name) ?? false) continue;
                if (options.ExcludeTypes?.Contains(prop.PropertyType) ?? false) continue;

                string npt; 
                
                if (string.IsNullOrEmpty(currPath))
                {
                    npt = prop.Name;
                }
                else
                {
                    npt = $"{currPath}\\{prop.Name}";
                }

                if (options.ExcludePaths?.Contains(npt) ?? false) continue;

                var prote = prop.PropertyType.GetProperties((options.IncludeNonPublic ? 0 : BindingFlags.Public) | BindingFlags.Instance);


                if (prop.PropertyType.GetInterfaces().Where(ix => ix.Name.Contains("IEnumerable")).Any())
                {
                    lobj.Add(npt, (type, prop));
                }
                else if (prop.PropertyType != typeof(string) && prop.PropertyType.IsClass && prote.Length > 0)
                {
                    var submap = MapSettings(prop.PropertyType, options, npt, prote);
                    if (submap != null && submap.Count > 0)
                    {
                        foreach (var kv in submap)
                        {
                            lobj.Add(kv.Key, kv.Value);
                        }
                    }
                }
                else
                {
                    lobj.Add(npt, (type, prop));
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


        /// <summary>
        /// Create a deep copy of this settings object
        /// </summary>
        /// <returns>A copy of this settings object</returns>
        public SettingsMapOptions Clone()
        {
            var smo = (SettingsMapOptions)MemberwiseClone();

            if (Converters != null)
            {
                smo.Converters = new List<SettingsTypeConverter>(Converters);
            }
            if (ExcludeTypes != null)
            {
                smo.ExcludeTypes = new List<Type>(ExcludeTypes);
            }
            if (ExcludeNames != null)
            {
                smo.ExcludeNames = new List<string>(ExcludeNames);
            }
            if (ExcludePaths != null)
            {
                smo.ExcludePaths = new List<string>(ExcludePaths);
            }

            return smo;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

    }


}
