using System;
using System.Collections.Generic;

namespace DataTools.Essentials.Collections
{
    /// <summary>
    /// An object that represents a reference to a backup snapshot within a given context
    /// </summary>
    public interface ISnapshot<T> : IDisposable
    {
        /// <summary>
        /// True if the token is no longer redeemable for a restoration
        /// </summary>
        bool IsExpired { get; }
        
        /// <summary>
        /// The timestamp of the token creation
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Restore the backup and expire the token
        /// </summary>
        /// <returns>True if the backup was restored successfully.</returns>
        /// <remarks>
        /// If the backup was not restored successfully, the token will not be expired.
        /// </remarks>
        bool Restore();

        /// <summary>
        /// Set the value to true to restore the backup automatically when the object is disposed via <see cref="IDisposable.Dispose()"/>
        /// </summary>
        bool RestoreOnDispose { get; set; }

        /// <summary>
        /// Promote the snapshot to a new durable collection
        /// </summary>
        /// <param name="name">The name of the new collection</param>
        /// <returns>A new durable collection</returns>        
        IEnumerable<T> Promote(string name);
    }
}
