using System;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Base class for self-serializable settings classes
    /// </summary>
    public abstract class ProgramSerializableBase : IProgramSerializable
    {
        /// <summary>
        /// The settings object
        /// </summary>
        protected ISettings settings;

        /// <summary>
        /// Create a new serializable settings class
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="location"></param>
        public ProgramSerializableBase(ISettings settings, Uri location = null)
        {
            this.settings = settings;
            Location = location;
        }

        /// <inheritdoc/>
        public virtual ISettings GetProgramSettings() => settings;

        /// <inheritdoc/>
        public virtual bool SupportsProgramSettings => true;

        /// <inheritdoc/>
        public abstract Uri Location { get; protected set; }

        /// <inheritdoc/>
        public abstract bool DeleteFromPersistence();
        
        /// <inheritdoc/>        
        public abstract bool Save();
        
        /// <inheritdoc/>        
        public abstract bool Load();
    }


}
