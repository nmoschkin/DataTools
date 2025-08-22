using System;
using System.Linq;
using System.Text;

namespace DataTools.Essentials.SortedLists
{
    /// <summary>
    /// Specifies the sort order for various sorting and search mechanisms in this assembly
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Ascending (least to greatest) order
        /// </summary>
        Ascending,

        /// <summary>
        /// Descending (greatest to least) order
        /// </summary>
        /// <remarks>
        /// Also known as reverse order
        /// </remarks>
        Descending,
    }
}