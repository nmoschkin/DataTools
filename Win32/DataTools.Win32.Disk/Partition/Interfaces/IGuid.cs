using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Disk.Partition.Interfaces
{
    /// <summary>
    /// Represents something that is identifiable by its GUID.
    /// </summary>
    /// <remarks></remarks>
    public interface IGuid
    {
        /// <summary>
        /// Gets the UUID
        /// </summary>
        Guid Id { get; }
    }

}
