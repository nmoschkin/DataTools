using DataTools.Essentials.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Windows.Essentials.Settings
{
    
    /// <summary>
    /// Base class for a registry-serializable data class
    /// </summary>
    public abstract class RegistrySerializable : ProgramSerializableBase
    {
        /// <summary>
        /// Registry settings instance 
        /// </summary>
        protected RegistrySettings regset;

        /// <summary>
        /// Create a registry-backed object based in a hive other than HKEY_CURRENT_USER with the specified explicit base key
        /// </summary>
        /// <param name="hive">The registry hive to use</param>
        /// <param name="baseKey">The base key to store the settings</param>
        protected RegistrySerializable(RegistryHive hive, string baseKey) : base(null)
        {
            this.settings = regset = new RegistrySettings(hive, baseKey);
        }

        /// <summary>
        /// Create a registry-backed object using the caller-provided registry settings object
        /// </summary>
        /// <param name="settings">Registry settings instance</param>
        protected RegistrySerializable(RegistrySettings settings) : base(settings)
        {
            regset = settings;
        }

        /// <summary>
        /// Create a registry setting automatically named from the specified <paramref name="author"/> and <paramref name="assembly"/> information
        /// </summary>
        /// <param name="author"></param>
        /// <param name="assembly"></param>
        /// <remarks>
        /// The key will be created in HKEY_CURRENT_USER\Software\<paramref name="author"/>\<see cref="AssemblyName.Name"/>\<see cref="AssemblyName.Version"/>
        /// </remarks>
        protected RegistrySerializable(string author, Assembly assembly) : base(null)
        {
            this.settings = regset = new RegistrySettings(author, assembly);
        }

        /// <inheritdoc/>
        public string RegistryPath => this.settings.Location;

        /// <inheritdoc/>
        public override bool DeleteFromPersistence()
        {
            return settings.ClearSettings();
        }

        /// <inheritdoc/>
        public override bool Load()
        {
            regset.Refresh();

            foreach (var setting in regset)
            {
                if (SettingsMapOptions.LookupProperty(this, setting.Key, out var foundIn, out var property))
                {
                    if (setting.Value != null && property.PropertyType.IsAssignableFrom(setting.Value.GetType()))
                    {
                        property.SetValue(foundIn, setting.Value);
                    }
                    else if (setting.Value is string sp && ParseableTypeConverter.TryGetParseMethod(property.PropertyType, out var mtd))
                    {
                        try
                        {
                            property.SetValue(foundIn, mtd.Invoke(null, new object[] { sp }));
                        }
                        catch (Exception ex) 
                        {
#if DEBUG
                            Debug.WriteLine(setting.Key);
                            Debug.WriteLine(ex);
#endif
                        }
                    }
                    else
                    {
                        try
                        {
                            var o = Activator.CreateInstance(property.PropertyType);
                            o = regset.GetValue(setting.Key, o);
                            property.SetValue(foundIn, o);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(setting.Key);
                            Debug.WriteLine(ex);
#endif
                        }

                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Save()
        {
            var map = SettingsMapOptions.MapSettings(GetType(), new SettingsMapOptions() { IncludeReadOnly = true, ExcludeNames = new string[] { nameof(RegistryPath) } });

            foreach (var kv in map)
            {
                if (SettingsMapOptions.LookupProperty(this, kv.Key, out var foundIn, out var property))
                {
                    settings.SetValue(kv.Key, property.GetValue(foundIn));
                }
            }

            return true;
        }
    }
}
