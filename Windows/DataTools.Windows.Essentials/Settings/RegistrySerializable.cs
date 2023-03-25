using DataTools.Essentials.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Windows.Essentials.Settings
{
    
    /// <summary>
    /// Base class for a registry-serializable data class
    /// </summary>
    public abstract class RegistrySerializable : ProgramSerializableBase
    {
        RegistrySettings regset;

        /// <summary>
        /// Create a registry-backed object based in a hive other than HKEY_CURRENT_USER with the specified base key
        /// </summary>
        /// <param name="hive">The registry hive to use</param>
        /// <param name="baseKey">The base key to store the settings</param>
        protected RegistrySerializable(RegistryHive hive, string baseKey) : base(null)
        {
            this.settings = regset = new RegistrySettings(hive, baseKey);            
            Location = this.settings.Location;
        }

        /// <summary>
        /// Create a registry-backed object using the caller-provided registry settings object
        /// </summary>
        /// <param name="settings">Registry settings instance</param>
        protected RegistrySerializable(RegistrySettings settings) : base(settings)
        {
            this.Location = this.settings.Location;
            regset = settings;
        }

        /// <inheritdoc/>
        public override Uri Location { get; protected set; }

        /// <inheritdoc/>
        public override bool DeleteFromPersistence()
        {
            return settings.ClearSettings();
        }

        /// <inheritdoc/>
        public override bool Load()
        {
            var keys = regset.Refresh();


            return true;
        }

        /// <inheritdoc/>
        public override bool Save()
        {
            return true;
        }
    }
}
