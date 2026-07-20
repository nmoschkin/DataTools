using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DataTools.Essentials.Collections
{

    /// <summary>
    /// A collection that is persisted and read from the disk, in real time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Useful for telemetry caches, or data that must be preserved between sessions.
    /// </remarks>
    public class DiskCollection<T> : ICollection<T>, IDisposable
    {
        /// <summary>
        /// Represents a copy of a <see cref="DiskCollection{T}"/> instance
        /// </summary>
        protected class DiskCollectionSnapshot : ISnapshot<T>
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
            public bool IsExpired { get; internal set; }

            /// <inheritdoc />            
            public DateTime Timestamp { get; }

            /// <summary>
            /// Gets the filename of the temporary snapshot
            /// </summary>
            public string Filename { get; }

            /// <inheritdoc />            
            public bool RestoreOnDispose { get; set; }

            /// <inheritdoc />            
            public virtual IEnumerable<T> Promote(string filename)
            {                
                if (owner?.TryGetTarget(out var target) == true)
                {
                    if (string.Equals(filename, target.filename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new AccessViolationException("Cannot persist to known target.");
                    }
                    File.Copy(Filename, filename);
                    var newobj = new DiskCollection<T>(filename, target.jsonSettings);
                    return newobj;
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
                if (!File.Exists(Filename))
                {
                    IsExpired = true;
                    return false;
                }
                if (this?.owner.TryGetTarget(out var owner) == true)
                {
                    return owner.Restore(this);
                }
                return false;
            }

            /// <summary>
            /// Dispose of the snapshot
            /// </summary>
            /// <param name="disposing">True if disposing through <see cref="IDisposable.Dispose()"/></param>
            /// <param name="restore">True to restore the backup</param>
            protected virtual void Dispose(bool disposing, bool restore)
            {
                if (!disposedValue)
                {
                    try
                    {
                        if (!IsExpired && restore) Restore();
                    }
                    catch { }
                    finally
                    {
                        if (File.Exists(Filename))
                        {
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
        }



        /// <summary>
        /// This is the buffer size for string actions.
        /// </summary>
        private const int BUFFER_SIZE = 65535;
        
        /// <summary>
        /// This is the size of the basic chunk. Compacting will adjust current record size to be a multiple of this number.
        /// </summary>
        private const int CHUNK_SIZE = 256;

        private static readonly byte[] CrLf = new byte[] { (byte)'\r', (byte)'\n' };

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
        private FileStream fileStream;
        private int isenumerating = 0;

        /// <summary>
        /// Create a new disk collection
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public DiskCollection(string filename, JsonSerializerSettings jsonSettings = null) : this(filename, CHUNK_SIZE, false, jsonSettings)
        {
        }

        /// <summary>
        /// Create a new disk collection
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="recordSize">The default record size.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public DiskCollection(string filename, int recordSize, JsonSerializerSettings jsonSettings = null) : this(filename, recordSize, false, jsonSettings)
        {
        }

        /// <summary>
        /// Create a new disk collection
        /// </summary>
        /// <param name="filename">The full path to the file where the new collection will be stored.</param>
        /// <param name="recordSize">The default record size.</param>
        /// <param name="isReadOnly">True if the collection will be opened read-only.</param>
        /// <param name="jsonSettings">Optional <see cref="JsonSerializerSettings"/>.</param>
        public DiskCollection(string filename, int recordSize, bool isReadOnly, JsonSerializerSettings jsonSettings = null)
        {
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
        }

        private DiskCollection(string filename, int recordSize, bool isReadOnly, bool asCompactTarget, JsonSerializerSettings jsonSettings = null)
            : this(filename, recordSize, isReadOnly, jsonSettings)
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
                lock(lockObj) 
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
        public void AddRange(IEnumerable<T> items)
        {
            int idx = 0;
            foreach (var item in items)
            {
                lock (lockObj)
                {
                    idx = count;
                    SetItem(item, count);
                    count++;
                }
                OnItemAdded(item, idx);
            }
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
            if (index >= count) throw new IndexOutOfRangeException();
            T old = this[index];
            Compact(index, 0);
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
            foreach(var item2 in this)
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
        /// Create a snapshot of the current collection state and return a <see cref="ISnapshot"/> token that can be used to restore from the backup
        /// </summary>
        /// <returns></returns>
        public virtual ISnapshot<T> CreateSnapshot()
        {
            lock (lockObj)
            {
                var snapfile = GetTempName(true);
                var token = new DiskCollectionSnapshot(this, snapfile);
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
                if (dsn.owner.TryGetTarget(out var target) && target == this)
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
                        catch
                        {
                            return false;
                        }
                        finally
                        {
                            Reset();
                        }
                    }
                }
            }
            return false;
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
        /// <returns>The full path of a file to open for compacting</returns>
        protected virtual string GetTempName(bool forSnapshot)
        {
            var ext = Path.GetExtension(filename);
            var dir = Path.GetDirectoryName(filename);
            var file = Path.GetFileNameWithoutExtension(filename);
            var c = 1;
            string lookFile;
            string path;
            do
            {
                if (forSnapshot)
                {
                    lookFile = $"{file}_{c++}__snapshot.${ext}";
                }
                else
                {
                    lookFile = $"{file}_{c++}.${ext}";
                }
                path = Path.Combine(dir, lookFile);
            } while (File.Exists(path));

            return path;
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
                var reccount = 0;
                var emptycount = 0;
                var rs = 0;
                var maxrs = 0;
                
                var cs = 0;
                var maxcs = 0;

                if (fileStream == null) return 0;
                
                var curpos = fileStream.Position;
                var lines = 0;
                
                var buffer = new byte[BUFFER_SIZE];
                
                var ns = false;
                
                fileStream.Seek(0, SeekOrigin.Begin);
                
                while (true)
                {
                    var c = fileStream.Read(buffer, 0, BUFFER_SIZE);
                    if (c == 0) break;
                    
                    for(var i = 0; i < c; i++) 
                    {
                        var b = buffer[i];
                        if (b == (byte)'\n')
                        {
                            if (ns)
                            {
                                reccount++;
                                ns = false;
                            }
                            else
                            {
                                emptycount++;
                                empties.Add(lines);
                            }
                            lines++;
                            if (rs > maxrs)
                            {
                                maxrs = rs;
                            }
                            if (cs > maxcs)
                            {
                                maxcs = cs;
                            }
                            cs = 0;
                            rs = 0;
                        }
                        else if (b == '\r')
                        {
                            continue;
                        }
                        else
                        {
                            if (b != ' ')
                            {
                                ns = true;
                                cs = rs;
                            }
                            rs++;
                        }
                    }
                    if (c < BUFFER_SIZE) break;
                }
                fileStream.Seek(curpos, SeekOrigin.Begin);
                records = reccount;
                recordSize = maxrs;
                maxActualSize = maxcs;
                if (empties.Count > 0)
                {
                    emptyIndices = empties.ToArray();
                }
                return lines;
            }
        }

        private T GetItem(int index, byte[] bytes = null)
        {
            if (bytes == null)
            {
                bytes = new byte[recordSize];
            }
            lock (lockObj)
            {
                fileStream.Seek(index * (recordSize + 2), SeekOrigin.Begin);
                if (fileStream.Read(bytes, 0, recordSize) != recordSize)
                {
                    return default;
                }
            }
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes).Trim(), jsonSettings);
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

                fileStream.Seek(index * (recordSize + 2), SeekOrigin.Begin);
                fileStream.Write(bytes, 0, recordSize);
                fileStream.Write(CrLf, 0, 2);
                //fileStream.Flush();
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

                fileStream?.Close();
                fileStream = null;

                var temp = GetTempName(false);
                recCount = 0;

                var newColSize = forceSize != 0 ? forceSize : recordSize;
                using (var otherCol = new DiskCollection<T>(temp, newColSize, false, true, jsonSettings))
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
                                        catch { }

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

                File.Delete(filename);
                File.Move(temp, filename);

                OpenFile();
                RefreshFromDiskState(false);
            }
            if (!del) OnCompacted();
        }

        private int GetRecordSize(int dataSize)
        {
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
