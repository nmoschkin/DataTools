using System;
using System.Linq;
using System.Text;

namespace DataTools.PluginFramework
{
    /// <summary>
    /// Contains information used to initialize an overlay.
    /// </summary>
    public interface IInitializationBundle
    {
        /// <summary>
        /// A parameter that may be used to carry licensing information.
        /// </summary>
        string LicenseString { get; set; }
    }
}