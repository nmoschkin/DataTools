using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Implements an application setting
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// Gets the key of the setting
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets or sets the value of the setting
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Try to get the parent settings of this setting
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        bool TryGetParent(out ISettings parent);

        /// <summary>
        /// True if this settings class supports <see cref="TryGetParent(out ISettings)"/>.
        /// </summary>
        bool SupportsTryGetParent { get; }

    }

    /// <summary>
    /// Implements an application setting
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISetting<T> : ISetting
    {
        /// <summary>
        /// Gets or sets the value of the setting
        /// </summary>
        new T Value { get; set; }
        
    }


}
