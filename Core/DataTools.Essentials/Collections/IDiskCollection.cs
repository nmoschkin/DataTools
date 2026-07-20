using System;
using System.Collections.Generic;

namespace DataTools.Essentials.Collections
{
    /// <summary>
    /// An interface for an object that implements a cache
    /// </summary>
    public interface ICachingSurface 
    {
        /// <summary>
        /// Get the active cache strategy
        /// </summary>
        CacheStrategy CacheStrategy { get; }

        /// <summary>
        /// Write all currently cached items to the source. Object references will be preserved.
        /// </summary>
        void CommitCachedItems();

        /// <summary>
        /// Refresh cached items from the source. All object references will be lost.
        /// </summary>
        /// <param name="all">Load all existing records from the source</param>
        void FreshenCachedItems(bool all);

        /// <summary>
        /// Clear the cache. All object references will be lost.
        /// </summary>
        void ResetCache();
    }

    /// <summary>
    /// Interface for enumerations that can produce and restore state snapshots
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnumerableSnapshot<T> : IEnumerable<T>, IReadOnlyList<T>, IDisposable
    {
        /// <summary>
        /// Create a snapshot of the current collection state and return a <see cref="ISnapshot{T}"/> token that can be used to restore from the backup
        /// </summary>
        /// <returns></returns>
        ISnapshot<T> CreateSnapshot();

        /// <summary>
        /// Restore a snapshot
        /// </summary>
        /// <param name="snapshot">The snapshot to restore.</param>
        /// <returns>True if successful</returns>
        /// <remarks>
        /// All changes made since this snapshot will be reverted.
        /// </remarks>
        bool Restore(ISnapshot<T> snapshot);
    }

    /// <summary>
    /// Interface for an object that implements compacting.
    /// </summary>
    public interface ICompactable
    {
        /// <summary>
        /// Compact and repair the disk collection
        /// </summary>
        void Compact();
    }
   

    /// <summary>
    /// An interface for an object that implements a collection that is persisted and read from the disk, in real time.
    /// </summary>
    public interface IDiskCollection<T> : IEnumerableSnapshot<T>, ICollection<T>, ICompactable, ICachingSurface
    {
        /// <summary>
        /// Get the current filename for the disk collection
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Returns true if the file is open. Otherwise false.
        /// </summary>
        bool IsFileOpen { get; }

        /// <summary>
        /// Get the current record size of the on-disk collection
        /// </summary>
        int RecordSize { get; }

        /// <summary>
        /// Gets the current size, in bytes, of the disk file that contains the collection.
        /// </summary>
        /// <remarks>
        /// If the object is disposed then the size will be -1.
        /// </remarks>
        long Size { get; }

        /// <summary>
        /// Add multiple items to the collection
        /// </summary>
        /// <param name="items">The items to add</param>
        bool AddRange(IEnumerable<T> items);

        /// <summary>
        /// Removes the item at the specified index
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        bool RemoveAt(int index);

        /// <summary>
        /// Reload the file from disk and rescan for structure
        /// </summary>
        void Reset();
    }
}