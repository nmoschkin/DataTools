using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DataTools.Essentials.Collections
{
    
    /// <summary>
    /// The strategies for how caching is utilized in a <see cref="DiskCollection{T}"/> or its descendants
    /// </summary>
    public enum CacheStrategy
    {
        /// <summary>
        /// No caching will be used. Items will be read from the disk every time.<br/>
        /// New reference objects will be instantiated every time they are accessed.
        /// </summary>
        None,

        /// <summary>
        /// Items are loaded from the disk on demand
        /// </summary>
        Lazy,

        /// <summary>
        /// All pre-existing items are loaded from disk on instantiation
        /// </summary>
        Complete
    }

    /// <summary>
    /// Return true to create a boundary starting with the current item in the collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">The item to test</param>
    /// <param name="index">The current index of the item in the owning collection</param>
    /// <returns></returns>
    public delegate bool SeparationPredicate<T>(T item, int index);

    /// <summary>
    /// A collection that is persisted and read from the disk, in real time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Useful for telemetry caches, or data that must be preserved between sessions.
    /// <br/><br/>
    /// When caching is used, you must call <see cref="CommitCachedItems"/> to update the disk collection with any mutated objects.<br/>
    /// The disposal method will not do this.
    /// </remarks>
    public class DiskCollection<T> : IDiskCollection<T>
    {
        private static readonly byte[] CrLf = new byte[] { (byte)'\r', (byte)'\n' };

        /// <summary>
        /// Gets the text of the last exception caught by this class
        /// </summary>
        /// <remarks>
        /// In many cases, a function will simply fail rather than cause a system crash.<br />
        /// In those instances, the exception will be caught and quietly bypassed when<br />
        /// possible. This property will contain the last such error at any given time.<br />
        /// </remarks>
        public static string LastErrorMessage { get; private set; }

        /// <summary>
        /// Represents a copy of a <see cref="DiskCollection{T}"/> instance
        /// </summary>
        protected internal class DiskCollectionSnapshot : ISnapshot<T>
        {

            /// <summary>
            /// The disposed state
            /// </summary>
            protected bool disposedValue;

            /// <summary>
            /// The current owner of the token as a weak reference
            /// </summary>
            protected internal WeakReference<DiskCollection<T>> owner;

            /// <inheritdoc />            
            public bool IsExpired { get; private set; }

            /// <inheritdoc />            
            public DateTime Timestamp { get; }

            /// <summary>
            /// Gets the filename of the temporary snapshot
            /// </summary>
            public string Filename { get; }

            /// <inheritdoc />            
            public bool RestoreOnDispose { get; set; }


            /// <inheritdoc />            
            public bool CanApply => true;

            /// <inheritdoc />            
            public virtual IDiskCollection<T> Promote(string filename)
            {
                if (owner?.TryGetTarget(out var target) == true && !target.disposedValue)
                {
                    if (string.Equals(filename, target.filename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new AccessViolationException("Cannot persist to known target.");
                    }
                    File.Copy(Filename, filename);
                    var newobj = new DiskCollection<T>(filename, target.CacheStrategy, target.jsonSettings);
                    return newobj;
                }
                else
                {
                    // We have determined that the owner no longer exists. Take this opportunity to expire the token.
                    IsExpired = true;
                }
                return null;
            }

            /// <summary>
            /// Create a new disk collection snapshot
            /// </summary>
            /// <param name="owner">The owning object</param>
            /// <param name="filename">The filename of the temporary snapshot</param>
            /// <param name="restoreOnDispose">Whether to automatically restore the snapshot to the original collection on token dispose</param>
            /// <param name="timestamp">A timestamp the consumer provides or null to automatically add one</param>
            protected internal DiskCollectionSnapshot(DiskCollection<T> owner, string filename, bool restoreOnDispose = false, DateTime? timestamp = null)
            {
                this.owner = new WeakReference<DiskCollection<T>>(owner);
                Filename = filename;
                RestoreOnDispose = restoreOnDispose;
                if (timestamp is DateTime d) Timestamp = d;
                else Timestamp = DateTime.Now;
            }

            /// <inheritdoc />            
            public bool Restore()
            {
                if (disposedValue) throw new ObjectDisposedException("Token is expired");
                if (IsExpired) return false;
                if (!File.Exists(Filename))
                {
                    IsExpired = true;
                    return false;
                }
                if (this?.owner.TryGetTarget(out var target) == true && !target.disposedValue)
                {
                    IsExpired = target.Restore(this);
                }
                else
                {
                    // We have determined that the owner no longer exists. Take this opportunity to expire the token.
                    IsExpired = true;
                    return false;
                }
                return IsExpired;
            }

            /// <inheritdoc />            
            public bool Apply()
            {
                if (disposedValue) throw new ObjectDisposedException("Token is expired");
                if (IsExpired) return false;
                if (!File.Exists(Filename))
                {
                    IsExpired = true;
                    return false;
                }
                try
                {
                    if (owner?.TryGetTarget(out var target) == true && !target.disposedValue)
                    {
                        target.AddRange(this);
                        IsExpired = true;
                    }
                    else
                    {
                        // We have determined that the owner no longer exists. Take this opportunity to expire the token.
                        IsExpired = true;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    LastErrorMessage = ex.ToString();
#if DEBUG
                    Console.WriteLine(ex);
#endif
                }
                return IsExpired;
            }

            /// <summary>
            /// Duplicate the file on disk to a new serialized copy
            /// </summary>
            /// <returns></returns>
            public ISnapshot<T> Clone()
            {
                if (DuplicateFile(true, out var parent, out var filename))
                {
                    return new DiskCollectionSnapshot(parent, filename, RestoreOnDispose, Timestamp);
                }
                return null;
            }

            object ICloneable.Clone()
            {
                return Clone();
            }

            /// <summary>
            /// Create a copy of the current backing store for the snapshot
            /// </summary>
            /// <param name="ownerRequired">True if the owner is required to exist and be in an active state</param>
            /// <param name="parent">Receives the owner object for the current instance</param>
            /// <param name="filename">Receives the new filename for the copy</param>
            /// <returns>True if the file was copied successfully.</returns>
            protected internal bool DuplicateFile(bool ownerRequired, out DiskCollection<T> parent, out string filename)
            {
                try
                {
                    if (owner?.TryGetTarget(out var target) == true && !target.disposedValue)
                    {
                        var copyname = target.GetTempName(true);
                        File.Copy(Filename, copyname);
                        parent = target;
                        filename = copyname;
                        return true;
                    }
                    else if (!ownerRequired)
                    {
                        if (TryParseTempName(Filename, out var original, out _, out _))
                        {
                            var copyname = CreateTempName(original, true);
                            File.Copy(Filename, copyname);
                            filename = copyname;
                            parent = null;
                            return true;
                        }
                    }

                    // We have determined that the owner no longer exists. Take this opportunity to expire the token.
                    IsExpired = true;
                }
                catch (Exception ex)
                {
                    LastErrorMessage = ex.ToString();
#if DEBUG
                    Console.WriteLine(ex);
#endif
                }
                parent = null;
                filename = null;
                return false;
            }


            /// <summary>
            /// Dispose of the snapshot
            /// </summary>
            /// <param name="disposing">True if disposing through <see cref="IDisposable.Dispose()"/></param>
            /// <param name="restore">True to restore the backup</param>
            protected virtual void Dispose(bool disposing, bool restore)
            {
                var mkbackup = false;
                if (!disposedValue)
                {
                    try
                    {
                        if (!IsExpired && restore) Restore();
                    }
                    catch (Exception ex)
                    {
                        LastErrorMessage = ex.ToString();
                        mkbackup = true;
                    }
                    finally
                    {
                        if (File.Exists(Filename))
                        {
                            if (mkbackup)
                            {
                                DuplicateFile(false, out _, out _);
                            }
                            File.Delete(Filename);
                        }
                        owner?.SetTarget(null);
                        owner = null;
                        IsExpired = true;
                        disposedValue = true;
                    }
                }
            }

            internal void Dispose(bool restore)
            {
                Dispose(disposing: true, restore: restore);
            }

            /// <summary>
            /// Dispose of the snapshot and delete the file.
            /// Once <see cref="Dispose()"/> has been called, the snapshot is not retrievable.
            /// </summary>
            public void Dispose()
            {
                Dispose(disposing: true, restore: RestoreOnDispose);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Enumerate through the items in the snapshot
            /// </summary>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            public IEnumerator<T> GetEnumerator()
            {
                if (owner?.TryGetTarget(out var target) == true && !target.disposedValue)
                {
                    using (var snapfile = new DiskCollection<T>(Filename, target.recordSize, true, CacheStrategy.None, true, target.jsonSettings))
                    {
                        var c = snapfile.Count;
                        for (var i = 0; i < c; i++)
                        {
                            yield return snapfile.GetItem(i);
                        }
                    }
                }
                else
                {
                    IsExpired = true;
                }
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// Create a name for the temporary file or snapshot file from the provided base filename
        /// </summary>
        /// <param name="filename">The input filename</param>
        /// <param name="forSnapshot">True if the filename is being created for a snapshot</param>
        /// <param name="startIndex">Start index of a volume set</param>
        /// <param name="stopIndex">Stop index (exclusive) of a volume set</param>
        /// <param name="indexFormat">Number format for indexes printed into the filename</param>
        /// <returns>The full path of a file to open for compacting</returns>
        public static string CreateTempName(string filename, bool forSnapshot, int startIndex = -1, int stopIndex = -1, string indexFormat = "00000")
        {
            var ext = Path.GetExtension(filename);
            var dir = Path.GetDirectoryName(filename);
            var file = Path.GetFileNameWithoutExtension(filename);
            var c = 1;
            string lookFile;
            string path;
            do
            {
                if (startIndex != -1 && stopIndex != -1)
                {
                    lookFile = $"{file}_{startIndex.ToString(indexFormat)}_{stopIndex.ToString(indexFormat)}_{c++}.@{ext}";
                }
                else
                {
                    if (forSnapshot)
                    {
                        lookFile = $"{file}_{c++}__snapshot.${ext}";
                    }
                    else
                    {
                        lookFile = $"{file}_{c++}.${ext}";
                    }
                }
                path = Path.Combine(dir, lookFile);
            } while (File.Exists(path));

            return path;
        }

        /// <summary>
        /// Try to parse a temporary filename and recover the encoded information
        /// </summary>
        /// <param name="filename">The input filename to parse</param>
        /// <param name="basename">Receives the original base filename</param>
        /// <param name="isSnapshot">Receives a boolean indicating it is a snapshot</param>
        /// <param name="counter">Receives the incremental counter of the file</param>
        /// <returns></returns>
        public static bool TryParseTempName(string filename, out string basename, out bool isSnapshot, out int counter)
        {
            var temprgx = @"^(.+)_(\d+)\.\$\.(\w+)$";
            var snaprgx = @"^(.+)_(\d+)__snapshot\.\$\.(\w+)$";

            filename = Path.GetFileName(filename);

            if (Regex.IsMatch(filename, temprgx))
            {
                var parsed = Regex.Match(filename, temprgx, RegexOptions.IgnoreCase);

                basename = parsed.Groups[1].Value + "." + parsed.Groups[3].Value;
                isSnapshot = false;
                counter = int.Parse(parsed.Groups[2].Value);
            }
            else if (Regex.IsMatch(filename, snaprgx))
            {
                var parsed = Regex.Match(filename, snaprgx);

                basename = parsed.Groups[1].Value + "." + parsed.Groups[3].Value;
                isSnapshot = true;
                counter = int.Parse(parsed.Groups[2].Value);
            }
            else
            {
                basename = null;
                isSnapshot = false;
                counter = -1;
                return false;
            }
            return true;
        }



        /// <summary>
        /// This is the buffer size for string actions.
        /// </summary>
        private const int BUFFER_SIZE = 65535;

        /// <summary>
        /// This is the size of the basic chunk. Compacting will adjust current record size to be a multiple of this number.
        /// </summary>
        private const int CHUNK_SIZE = 256;

        /// <summary>
        /// The synchronizer object
        /// </summary>
        protected readonly object lockObj = new object();

        private readonly bool isReadOnly = false;
        private readonly string filename;
        private readonly JsonSerializerSettings jsonSettings;

        private bool disposedValue;

        private int recordSize = BUFFER_SIZE;
        private int count = 0;

        private bool asCompactTarget;
        private bool noCache;
        private FileStream fileStream;
        private int isenumerating = 0;

        private Dictionary<int, T> cachedItems;


        /// <summary>
        /// Open or Create a disk collection
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public DiskCollection(string filename, JsonSerializerSettings jsonSettings = null) : this(filename, CHUNK_SIZE, false, CacheStrategy.Lazy, jsonSettings)
        {
        }

        /// <summary>
        /// Open or Create a disk collection
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="cacheStrategy">The caching strategy to use for this instance. Change this if short on memory.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public DiskCollection(string filename, CacheStrategy cacheStrategy, JsonSerializerSettings jsonSettings = null) : this(filename, CHUNK_SIZE, false, cacheStrategy, jsonSettings)
        {
        }

        /// <summary>
        /// Open or Create a disk collection with items
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="items">The items to add to the collection</param>
        /// <param name="isReadOnly">True if the collection will be opened read-only.</param>
        /// <param name="cacheStrategy">The caching strategy to use for this instance. Change this if short on memory.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        /// <remarks>
        /// If a cache already exists, it will be appended.
        /// </remarks>
        public DiskCollection(string filename, IEnumerable<T> items, bool isReadOnly, CacheStrategy cacheStrategy, JsonSerializerSettings jsonSettings = null) : this(filename, CHUNK_SIZE, isReadOnly, cacheStrategy, jsonSettings)
        {
            AddRange(items);
        }

        /// <summary>
        /// Open or Create a disk collection with items
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="items">The items to add to the collection</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        /// <remarks>
        /// If a cache already exists, it will be appended.
        /// </remarks>
        public DiskCollection(string filename, IEnumerable<T> items, JsonSerializerSettings jsonSettings = null) : this(filename, CHUNK_SIZE, false, CacheStrategy.Lazy, jsonSettings)
        {
            AddRange(items);
        }

        /// <summary>
        /// Open or Create a disk collection
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="recordSize">The default record size.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public DiskCollection(string filename, int recordSize, JsonSerializerSettings jsonSettings = null) : this(filename, recordSize, false, CacheStrategy.Lazy, jsonSettings)
        {
        }

        /// <summary>
        /// Open or Create a disk collection
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="recordSize">The default record size.</param>
        /// <param name="isReadOnly">True if the collection will be opened read-only.</param>
        /// <param name="cacheStrategy">The caching strategy to use for this instance. Change this if short on memory.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public DiskCollection(string filename, int recordSize, bool isReadOnly, CacheStrategy cacheStrategy, JsonSerializerSettings jsonSettings = null)
        {
            noCache = cacheStrategy == CacheStrategy.None;
            if (!noCache)
            {
                cachedItems = new Dictionary<int, T>();
            }
            this.isReadOnly = isReadOnly;
            this.recordSize = recordSize;
            this.filename = filename;

            if (jsonSettings == null)
            {
                if (JsonConvert.DefaultSettings != null)
                {
                    this.jsonSettings = JsonConvert.DefaultSettings();
                    this.jsonSettings.Formatting = Formatting.None;
                }
            }
            else
            {
                this.jsonSettings = jsonSettings;
            }

            CheckSettings(this.jsonSettings);
            RefreshFromDiskState(false);
            CacheStrategy = cacheStrategy;

            if (cacheStrategy == CacheStrategy.Complete)
            {
                FreshenCachedItems(true);
            }
        }

        private DiskCollection(string filename, int recordSize, bool isReadOnly, CacheStrategy cacheStrategy, bool asCompactTarget, JsonSerializerSettings jsonSettings)
            : this(filename, recordSize, isReadOnly, cacheStrategy, jsonSettings)
        {
            this.asCompactTarget = asCompactTarget;
        }

        /// <summary>
        /// Get the current filename for the disk collection
        /// </summary>
        public string Filename => filename;

        /// <summary>
        /// Get the current record size of the on-disk collection
        /// </summary>
        public int RecordSize
        {
            get
            {
                lock (lockObj)
                {
                    return recordSize;
                }
            }
        }

        /// <summary>
        /// Get the number of items in the collection
        /// </summary>
        public int Count
        {
            get
            {
                lock (lockObj)
                {
                    return count;
                }
            }
        }

        /// <summary>
        /// Get the active cache strategy
        /// </summary>
        public CacheStrategy CacheStrategy { get; }

        /// <summary>
        /// Returns true if the file is open. Otherwise false.
        /// </summary>
        public bool IsFileOpen => fileStream != null && !disposedValue;

        /// <summary>
        /// Gets a value indicating whether or not the collection is read-only
        /// </summary>
        public bool IsReadOnly => isReadOnly;

        /// <summary>
        /// Gets the current size, in bytes, of the disk file that contains the collection.
        /// </summary>
        /// <remarks>
        /// If the object is disposed then the size will be -1.
        /// </remarks>
        public long Size
        {
            get
            {
                lock (lockObj)
                {
                    if (fileStream != null)
                    {
                        return fileStream.Length;
                    }
                    else return -1;
                }
            }
        }

        /// <summary>
        /// Gets or sets the item at the specified index
        /// </summary>
        /// <param name="index">The index of the item to set</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index >= count) throw new IndexOutOfRangeException();
                return GetItem(index);
            }
            set
            {
                if (index >= count) throw new IndexOutOfRangeException();
                if (isReadOnly) throw new ReadOnlyException("Collection is read-only");
                T olditem;
                lock (lockObj)
                {
                    olditem = GetItem(index);
                    SetItem(value, index);
                }
                OnItemChanged(olditem, value, index);
            }
        }

        /// <summary>
        /// Serialize an item to the disk collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            int idx = 0;
            lock (lockObj)
            {
                idx = count;
                SetItem(item, count);
                count++;
            }
            OnItemAdded(item, idx);
        }

        /// <summary>
        /// Add multiple items to the collection
        /// </summary>
        /// <param name="items">The items to add</param>
        public bool AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items)); 

            var succeed = false;
            var startIndex = -1;
            var addedItems = new List<T>();

            lock (lockObj)
            {
                startIndex = count;
                using (var rollback = CreateSnapshot())
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            SetItem(item, count);
                            count++;
                            addedItems.Add(item);
                        }
                        succeed = true;
                    }
                    catch
                    {
                        rollback.Restore();
                    }
                }
            }

            if (succeed)
            {
                OnItemsAdded(addedItems, startIndex);
            }
            return succeed;
        }

        /// <summary>
        /// Clear the collection (delete and recreate the file)
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Clear()
        {
            if (isReadOnly) throw new ReadOnlyException("Collection is read-only");
            if (asCompactTarget) throw new InvalidOperationException("Debug your code! Clearing change on compact target cannot happen/should not happen.");
            lock (lockObj)
            {
                fileStream?.Close();
                count = 0;
                OpenFile(true);
                RefreshFromDiskState(false);
            }
            OnCollectionCleared();
        }

        /// <summary>
        /// Returns true if an item in the collection exists that is equal to the value passed by calling <see cref="Object.Equals(object)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks>
        /// For reference objects, items in the collection will never be equal to items outside of the collection as the items are created<br />
        /// from disk, on demand, unless their equality method is explicitly overridden to test of object content fidelity, and not memory reference.
        /// </remarks>
        public bool Contains(T item)
        {
            foreach (var item2 in this)
            {
                if (Equals(item, item2)) return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the contents of this collection to the specified array, beginning at the specified index.
        /// </summary>
        /// <param name="array">The array into which the elements will be copied.</param>
        /// <param name="arrayIndex">The index within the array to start copying elements.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (lockObj)
            {
                var pos = arrayIndex;
                foreach (var item in this)
                {
                    array[pos++] = item;
                }
            }
        }

        /// <summary>
        /// Removes the item at the specified index
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool RemoveAt(int index)
        {
            if (isReadOnly) throw new ReadOnlyException("Collection is read-only");
            if (asCompactTarget) throw new InvalidOperationException("Debug your code! Removing change on compact target cannot happen/should not happen.");
            if (index >= count || index < 0) throw new IndexOutOfRangeException();
            T old = this[index];
            lock (lockObj)
            {
                Dictionary<int, T> oldDict = null;
                if (!noCache && cachedItems.Count > 0)
                {
                    oldDict = new Dictionary<int, T>(cachedItems);
                }

                Compact(index, 0);

                if (!noCache && oldDict != null && oldDict.Count > 0)
                {
                    var keys = oldDict.Keys.ToArray();
                    foreach (var key in keys)
                    {
                        if (key > index)
                        {
                            var newkey = key - 1;
                            oldDict[newkey] = oldDict[key];
                            oldDict.Remove(key);
                        }
                    }
                    cachedItems = oldDict;
                }
            }
            OnItemRemoved(old, index);
            return true;
        }

        /// <summary>
        /// Remove an item that equals the specified item by using the <see cref="Object.Equals(object)"/> comparer.
        /// </summary>
        /// <param name="item">The item to match for removal.</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            if (isReadOnly) throw new ReadOnlyException("Collection is read-only");
            if (asCompactTarget) throw new InvalidOperationException("Debug your code! Removing change on compact target cannot happen/should not happen.");
            var idx = 0;
            var c = count;
            foreach (var item2 in this)
            {
                if (Equals(item, item2))
                {
                    break;
                }
                idx++;
            }
            if (idx == -1 || idx >= count)
            {
                return false;
            }
            else
            {
                RemoveAt(idx);
                return true;
            }
        }

        /// <summary>
        /// Compact and repair the disk collection
        /// </summary>
        public void Compact()
        {
            Compact(-1, 0);
        }

        /// <summary>
        /// Reload the file from disk and rescan for structure
        /// </summary>
        public void Reset()
        {
            lock (lockObj)
            {
                CloseFile();
                RefreshFromDiskState(false);
            }
        }

        /// <summary>
        /// Create a snapshot of the current collection state and return a <see cref="ISnapshot{T}"/> token that can be used to restore from the backup
        /// </summary>
        /// <returns></returns>
        public virtual ISnapshot<T> CreateSnapshot()
        {
            lock (lockObj)
            {
                var snapfile = GetTempName(true);
                var token = new DiskCollectionSnapshot(this, snapfile);
                fileStream?.Flush();
                File.Copy(filename, snapfile, true);
                return token;
            }
        }

        /// <summary>
        /// Restore a snapshot
        /// </summary>
        /// <param name="snapshot">The snapshot to restore.</param>
        /// <returns>True if successful</returns>
        /// <remarks>
        /// All changes made since this snapshot will be reverted.
        /// </remarks>
        public virtual bool Restore(ISnapshot<T> snapshot)
        {
            if (snapshot is DiskCollectionSnapshot dsn && !dsn.IsExpired)
            {
                if (dsn.owner.TryGetTarget(out var target) && target == this && !target.disposedValue)
                {
                    if (!File.Exists(dsn.Filename)) return false;
                    lock (lockObj)
                    {
                        try
                        {
                            CloseFile();
                            File.Copy(dsn.Filename, filename, true);
                            dsn.Dispose(false);
                            return true;
                        }
                        catch (Exception ex)
                        {

                            LastErrorMessage = ex.ToString();
                            return false;
                        }
                        finally
                        {
                            Reset();
                        }
                    }
                }
                else
                {
                    dsn.Dispose(false);
                }
            }
            return false;
        }

        /// <summary>
        /// Write all currently cached items to disk. Object references will be preserved.
        /// </summary>
        public void CommitCachedItems()
        {
            if (noCache) return;
            lock (lockObj)
            {
                foreach (var kv in cachedItems)
                {
                    SetItem(kv.Value, kv.Key);
                }
            }
        }

        /// <summary>
        /// Refresh cached items from the disk. All object references will be lost.
        /// </summary>
        /// <param name="all">Load all existing records from disk</param>
        public void FreshenCachedItems(bool all)
        {
            if (noCache) return;
            lock (lockObj)
            {
                if (all)
                {
                    for (var key = 0; key < count; key++)
                    {
                        GetItem(key, force: true);
                    }
                }
                else
                {
                    var keys = cachedItems.Keys.ToArray();
                    foreach (var key in keys)
                    {
                        GetItem(key, force: true);
                    }
                }
            }
        }

        /// <summary>
        /// Clear the cache. All object references will be lost.
        /// </summary>
        public void ResetCache()
        {
            if (noCache) return;
            lock (lockObj)
            {
                cachedItems?.Clear();
            }
        }

        /// <summary>
        /// Divide the collection into a series of files, each containing a segment of items
        /// </summary>
        /// <param name="maxItems">The maximum number of items in each segment</param>
        /// <returns>A list of the created files</returns>
        public IReadOnlyList<string> DivideCollection(int maxItems)
        {
            return MakeDaughters(maxItems);
        }

        /// <summary>
        /// Divide the collection into a series of files, each containing a segment of items
        /// </summary>
        /// <param name="predicate">A function that returns true where the next file should be created</param>
        /// <returns>A list of the created files</returns>
        public IReadOnlyList<string> DivideCollection(SeparationPredicate<T> predicate)
        {
            return MakeDaughters(predicate);
        }

        // Overrideables

        /// <summary>
        /// Called when the collection is cleared
        /// </summary>
        protected virtual void OnCollectionCleared()
        {
        }

        /// <summary>
        /// Called when an item is added to the collection
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        protected virtual void OnItemAdded(T item, int index)
        {
        }

        /// <summary>
        /// Called when multiple items are added with <see cref="AddRange(IEnumerable{T})"/>
        /// </summary>
        /// <param name="items"></param>
        /// <param name="startIndex"></param>
        protected virtual void OnItemsAdded(IEnumerable<T> items, int startIndex)
        {
        }

        /// <summary>
        /// Called when an item is removed from the collection
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        protected virtual void OnItemRemoved(T item, int index)
        {
        }

        /// <summary>
        /// Called when an item in the collection is changed
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        /// <param name="index"></param>
        protected virtual void OnItemChanged(T oldItem, T newItem, int index)
        {
        }

        /// <summary>
        /// Called when the collection file has been compacted
        /// </summary>
        protected virtual void OnCompacted()
        {
        }

        /// <summary>
        /// Determines whether two items of type <typeparamref name="T"/> are equal
        /// </summary>
        /// <param name="inputItem">The item being compared</param>
        /// <param name="serializedItem">The item from the collection to compare</param>
        /// <returns>True if the items are equal</returns>
        /// <remarks>
        /// This method is used by <see cref="Remove(T)"/> and <see cref="Contains(T)"/>
        /// </remarks>
        protected virtual bool Equals(T inputItem, T serializedItem)
        {
            return inputItem?.Equals(serializedItem) == true;
        }

        /// <summary>
        /// Gets a name for the temporary file to use during the compact process
        /// </summary>
        /// <param name="forSnapshot">True if the name is being created for a snapshot</param>
        /// <param name="startIndex">Start index of a volume set</param>
        /// <param name="stopIndex">Stop index (exclusive) of a volume set</param>
        /// <param name="indexFormat">Number format for indexes printed into the filename</param>
        /// <returns>The full path of a file to open for compacting</returns>
        protected virtual string GetTempName(bool forSnapshot, int startIndex = -1, int stopIndex = -1, string indexFormat = "00000")
        {
            return CreateTempName(filename, forSnapshot, startIndex, stopIndex, indexFormat);
        }

        /// <summary>
        /// Gets the current state of the underlying file
        /// </summary>
        /// <param name="records">The number of actual records counted</param>
        /// <param name="recordSize">The maximum record size</param>
        /// <param name="maxActualSize">The maximum size of actual data</param>
        /// <param name="emptyIndices">An array of indices with no entries</param>
        /// <returns>The total number of lines counted in the file</returns>
        protected int GetCurrentState(out int records, out int recordSize, out int maxActualSize, out int[] emptyIndices)
        {
            lock (lockObj)
            {
                records = 0;
                recordSize = 0;
                maxActualSize = 0;
                emptyIndices = null;
                var empties = new List<int>();
                var recCount = 0;
                var emptycount = 0;
                var currentSpace = 0;
                var fixedRecordSize = 0;

                var currentRecordSize = 0;
                var largestRecordSize = 0;

                if (fileStream == null) return 0;

                long curpos = fileStream.Position;
                var lines = 0;

                var buffer = new byte[BUFFER_SIZE];

                var nonSpaceChars = false;

                fileStream.Seek(0, SeekOrigin.Begin);

                while (true)
                {
                    var c = fileStream.Read(buffer, 0, BUFFER_SIZE);
                    if (c == 0) break;

                    for (var i = 0; i < c; i++)
                    {
                        var b = buffer[i];
                        if (b == (byte)'\n')
                        {
                            if (nonSpaceChars)
                            {
                                recCount++;
                                nonSpaceChars = false;
                            }
                            else
                            {
                                emptycount++;
                                empties.Add(lines);
                            }
                            lines++;
                            if (currentSpace > fixedRecordSize)
                            {
                                fixedRecordSize = currentSpace;
                            }
                            if (currentRecordSize > largestRecordSize)
                            {
                                largestRecordSize = currentRecordSize;
                            }
                            currentRecordSize = 0;
                            currentSpace = 0;
                        }
                        else if (b == '\r')
                        {
                            continue;
                        }
                        else
                        {
                            currentSpace++;
                            if (b != ' ')
                            {
                                nonSpaceChars = true;
                                currentRecordSize = currentSpace;
                            }
                        }
                    }
                    if (c < BUFFER_SIZE) break;
                }
                fileStream.Seek(curpos, SeekOrigin.Begin);
                records = recCount;
                recordSize = fixedRecordSize;
                maxActualSize = largestRecordSize;
                if (empties.Count > 0)
                {
                    emptyIndices = empties.ToArray();
                }
                return lines;
            }
        }

        private T GetItem(int index, byte[] bytes = null, bool force = false)
        {
            if (!force && !noCache && cachedItems.TryGetValue(index, out var item)) return item;
            if (bytes == null)
            {
                bytes = new byte[recordSize];
            }
            lock (lockObj)
            {
                fileStream.Seek((long)index * (recordSize + 2), SeekOrigin.Begin);
                if (fileStream.Read(bytes, 0, recordSize) != recordSize)
                {
                    return default;
                }
            }
            item = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes).Trim(), jsonSettings);
            if (!noCache) cachedItems[index] = item;
            return item;
        }

        private void SetItem(T item, int index)
        {
            if (isReadOnly) throw new ReadOnlyException("Collection is read-only");
            if (index > count) throw new IndexOutOfRangeException();

            // Always check settings.
            // Provided (or even "created") settings can be mutated externally.
            CheckSettings(jsonSettings);

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item, jsonSettings));
            var reclen = bytes.Length;

            lock (lockObj)
            {
                if (bytes.Length > recordSize)
                {
                    if (asCompactTarget) throw new InvalidOperationException("Debug your code! Capacity change on compact target cannot happen/should not happen.");
                    var newRecSize = GetRecordSize(bytes.Length);
                    if (count > 0)
                    {
                        Compact(-1, newRecSize);
                    }
                    else
                    {
                        recordSize = newRecSize;
                    }
                }

                Array.Resize(ref bytes, recordSize);

                for (int i = reclen; i < recordSize; i++)
                {
                    bytes[i] = (byte)' ';
                }

                fileStream.Seek((long)index * (recordSize + 2), SeekOrigin.Begin);
                fileStream.Write(bytes, 0, recordSize);
                fileStream.Write(CrLf, 0, 2);
                if (!noCache) cachedItems[index] = item;
            }
        }

        private void Compact(int skip, int forceSize)
        {
            if (isReadOnly) throw new ReadOnlyException("Collection is read-only");
            var del = skip != -1;
            lock (lockObj)
            {
                const int BufferSize = BUFFER_SIZE;
                var decoder = Encoding.UTF8.GetDecoder();

                byte[] byteBuffer = new byte[BufferSize];
                byte[] recordBuffer = new byte[recordSize];
                char[] charBuffer = new char[Encoding.UTF8.GetMaxCharCount(BufferSize)];

                var currentLine = new StringBuilder();
                var recCount = 0;

                RefreshFromDiskState(true);

                ResetCache();
                fileStream?.Close();
                fileStream = null;

                var temp = GetTempName(false);
                recCount = 0;

                var newColSize = forceSize != 0 ? forceSize : recordSize;
                using (var otherCol = new DiskCollection<T>(temp, newColSize, false, CacheStrategy.None, true, jsonSettings))
                {
                    using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None, BufferSize, FileOptions.SequentialScan))
                    {
                        while (true)
                        {
                            int bytesRead = fs.Read(byteBuffer, 0, byteBuffer.Length);
                            if (bytesRead == 0) break;

                            int charsDecoded = decoder.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0, flush: false);

                            for (int i = 0; i < charsDecoded; i++)
                            {
                                char c = charBuffer[i];
                                switch (c)
                                {
                                    case '\r':
                                        break;

                                    case '\n':
                                        try
                                        {
                                            var obj = JsonConvert.DeserializeObject<T>(currentLine.ToString().Trim(), jsonSettings);
                                            if (recCount != skip && obj != null)
                                            {
                                                otherCol.Add(obj);
                                                recCount++;
                                            }
                                            else if (recCount == skip)
                                            {
                                                skip = -1;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LastErrorMessage = ex.ToString();
                                        }

                                        currentLine.Clear();
                                        break;

                                    default:
                                        currentLine.Append(c);
                                        break;
                                }
                            }

                            if (bytesRead < byteBuffer.Length) break;
                        }
                    }
                }

                count = recCount;

                File.Copy(temp, filename, true);
                File.Delete(temp);

                OpenFile();
                RefreshFromDiskState(false);
            }
            if (!del) OnCompacted();
        }

        private List<string> MakeDaughters(int maxCount)
        {
            return MakeDaughters((item, index) => index % maxCount == 0);
        }

        private List<string> MakeDaughters(SeparationPredicate<T> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            lock (lockObj)
            {
                var cidx = 0;
                var bidx = -1;
                List<(int, int)> ranges = new List<(int, int)>();
                List<string> dnames = new List<string>();

                foreach (var item in this)
                {
                    if (cidx == 0 || predicate(item, cidx))
                    {
                        if (bidx != -1)
                        {
                            ranges.Add((bidx, cidx));
                            bidx = -1;
                        }
                        bidx = cidx;
                    }
                    cidx++;
                }

                if (bidx != -1)
                {
                    ranges.Add((bidx, cidx));
                }

                try
                {
                    var buffer = new byte[recordSize];
                    foreach (var range in ranges)
                    {
                        var startIdx = range.Item1;
                        var stopIdx = range.Item2;
                        var dbname = GetTempName(false, startIdx, stopIdx);
                        dnames.Add(dbname);
                        using (var col = new DiskCollection<T>(dbname, recordSize, false, CacheStrategy.None, true, jsonSettings))
                        {
                            for (var i = startIdx; i < stopIdx; i++)
                            {
                                col.Add(GetItem(i, buffer));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LastErrorMessage = ex.ToString();

                    foreach (var rollback in dnames)
                    {
                        File.Delete(rollback);
                    }
                    return null;
                }
                return dnames;
            }
        }

        private int GetRecordSize(int dataSize)
        {
            if (dataSize % CHUNK_SIZE == 0) return dataSize;
            return dataSize + (CHUNK_SIZE - (dataSize % CHUNK_SIZE));
        }

        private void CheckSettings(JsonSerializerSettings settings)
        {
            // This record cannot have cr/lf
            if (settings != null && settings.Formatting == Formatting.Indented)
            {
                // Gracefully overriding this setting is theoretically an option.
                // But there's also the fact that this slows the system down,
                // and the consumer should know how to use this API correctly.
                // settings.Formatting = Formatting.None;

                // Throw for now.
                throw new InvalidDataException("Indented formatting is not permitted in serialization. Check your JsonSerializerSettings.");
            }
        }

        private void OpenFile(bool overwrite = false)
        {
            if (isReadOnly)
            {
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else if (!overwrite)
            {
                fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            else
            {
                fileStream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            }
        }

        private void RefreshFromDiskState(bool setRecordSize)
        {
            lock (lockObj)
            {
                ResetCache();
                if (fileStream == null) OpenFile();
                fileStream.Seek(0, SeekOrigin.End);
                var currcapacity = GetCurrentState(out var exist, out var existSize, out var existActual, out _);

                if (exist > 0) count = exist;
                if (existSize > 0) recordSize = existSize;
                if (existActual == 0) existActual = recordSize;
                if (setRecordSize)
                {
                    recordSize = GetRecordSize(existActual);
                }
            }
        }

        private void CloseFile()
        {
            lock (lockObj)
            {
                fileStream?.Flush();
                fileStream?.Close();
                fileStream = null;
            }
        }

        /// <summary>
        /// Dispose of the object and close the disk file
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose()"/>, false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
            CloseFile();
        }

        /// <summary>
        /// Finalize the object and close the file
        /// </summary>
        ~DiskCollection()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Dispose of the object and close the disk file
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get the enumerator
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IEnumerator<T> GetEnumerator()
        {
            isenumerating++;
            int i = 0;
            int c = count;
            var bytes = new byte[recordSize];
            for (i = 0; i < c; i++)
            {
                if (c != count) throw new InvalidOperationException("Collection changed during enumeration");
                yield return GetItem(i, bytes);
            }
            isenumerating--;
            bytes = null;
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
