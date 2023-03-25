using System;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Represents a data class that can self-persist using the settings framework
    /// </summary>
    public interface IProgramSerializable
    {

        /// <summary>
        /// Retrieve the program settings object associated with this instance
        /// </summary>
        /// <returns><see cref="ISettings"/> implementation or null if not supported</returns>
        ISettings GetProgramSettings();

        /// <summary>
        /// Gets a value indicating that program settings is supported by this class
        /// </summary>
        bool SupportsProgramSettings { get; }

        /// <summary>
        /// Gets the location of the persisted data
        /// </summary>
        Uri Location { get; }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <returns>True if succcessful</returns>
        bool Save();

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns>True if successful</returns>
        bool Load();

        /// <summary>
        /// Delete settings from wherever they are persisted by this object
        /// </summary>
        /// <returns></returns>
        bool DeleteFromPersistence();
    }


}
