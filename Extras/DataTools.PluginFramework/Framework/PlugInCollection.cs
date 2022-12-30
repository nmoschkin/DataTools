using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DataTools.PluginFramework.Framework
{
    /// <summary>
    /// Represents a collection of <see cref="IPlugIn"/> implementations.
    /// </summary>
    public class PlugInCollection : ObservableCollection<IPlugIn>
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="PlugInCollection"/> class.
        /// </summary>
        internal PlugInCollection() : base()
        {
        }
    }
}