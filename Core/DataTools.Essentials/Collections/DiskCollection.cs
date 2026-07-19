using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DataTools.Essentials.Collections
{

    //public enum BadObjectHandling
    //{
    //    Ignore,
    //    Throw
    //}

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
        /// This is the buffer size for string actions.
        /// </summary>
        private const int BUFFER_SIZE = 65535;
        
        /// <summary>
        /// This is the size of the basic chunk. Compacting will adjust current record size to be a multiple of this number.
        /// </summary>
        private const int CHUNK_SIZE = 256;

        private readonly object lockObj = new object();
        private string filename;
        private FileStream file;
        private bool disposedValue;

        private int recordSize = BUFFER_SIZE;

        private int count = 0;
        private static readonly byte[] CrLf = new byte[] { (byte)'\r', (byte)'\n' };
        private bool asScratch;
        private JsonSerializerSettings jsonSettings;
        private bool isReadOnly = false;
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
        protected DiskCollection(string filename, int recordSize, bool isReadOnly, JsonSerializerSettings jsonSettings = null)
        {
            this.isReadOnly = isReadOnly;
            this.jsonSettings = jsonSettings;
            this.recordSize = recordSize;
            this.filename = filename;

            SetupFromDiskState(false);
        }

        /// <summary>
        /// Get the current filename for the disk collection
        /// </summary>
        public string Filename => filename;

        /// <summary>
        /// Get the current record size of the on-disk collection
        /// </summary>
        public int RecordSize => recordSize;

        /// <summary>
        /// Get the number of items in the collection
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Gets a value indicating whether or not the collection is read-only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the item at the specified index
        /// </summary>
        /// <param name="index">The index of the item to set</param>
        /// <returns></returns>
        public T this[int index]
        {
            get => GetItem(index);
            set
            {
                SetItem(value, index);
            }
        }

        /// <summary>
        /// Serialize an item to the disk collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            lock (lockObj)
            {
                SetItem(item, count);
                count++;
            }
        }

        /// <summary>
        /// Clear the collection (delete and recreate the file)
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Clear()
        {
            if (asScratch) throw new InvalidOperationException("Debug your code! Clearing change on collection change cannot happen/should not happen.");
            lock (lockObj)
            {
                file?.Close();
                count = 0;
                OpenFile(true);
                SetupFromDiskState(false);
            }
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
        public virtual bool Contains(T item)
        {
            foreach (var item2 in this)
            {
                if (item?.Equals(item2) == true) return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the contents of this collection to the specified array, beginning at the specified index.
        /// </summary>
        /// <param name="array">The array into which the elements will be copied.</param>
        /// <param name="arrayIndex">The index within the array to start copying elements.</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
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
            if (asScratch) throw new InvalidOperationException("Debug your code! Removing change on collection change cannot happen/should not happen.");
            lock (lockObj)
            {
                if (index < count)
                {
                    Compact(index, 0);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Remove an item that equals the specified item by using the <see cref="Object.Equals(object)"/> comparer.
        /// </summary>
        /// <param name="item">The item to match for removal.</param>
        /// <returns></returns>
        public virtual bool Remove(T item)
        {
            if (asScratch) throw new InvalidOperationException("Debug your code! Removing change on collection change cannot happen/should not happen.");
            lock (lockObj)
            {
                var idx = 0;
                foreach (var item2 in this)
                {
                    if (item2.Equals(item))
                    {
                        RemoveAt(idx);
                        return true;
                    }
                    idx++;
                }
                return false;
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
        /// Gets a name for the temporary file to use during the compact process
        /// </summary>
        /// <returns>The full path of a file to open for compacting</returns>
        protected virtual string GetTempName()
        {
            var ext = Path.GetExtension(filename);
            var dir = Path.GetDirectoryName(filename);
            var file = Path.GetFileNameWithoutExtension(filename);
            var c = 1;
            string lookFile;
            string path;
            do
            {
                lookFile = $"{file}_{c++}.${ext}";
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
        protected virtual int GetCurrentState(out int records, out int recordSize, out int maxActualSize, out int[] emptyIndices)
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

                if (file == null) return 0;
                
                var curpos = file.Position;
                var lines = 0;
                
                var buffer = new byte[BUFFER_SIZE];
                
                var ns = false;
                
                file.Seek(0, SeekOrigin.Begin);
                
                while (true)
                {
                    var c = file.Read(buffer, 0, BUFFER_SIZE);
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
                file.Seek(curpos, SeekOrigin.Begin);
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

        /// <summary>
        /// Deserializes the item at the specified index from disk
        /// </summary>
        /// <param name="index">The index at which to deserialize the item</param>
        /// <returns></returns>
        protected virtual T GetItem(int index)
        {
            lock (lockObj)
            {
                file.Seek(index * (recordSize + 2), SeekOrigin.Begin);
                var bytes = new byte[recordSize];
                var len = file.Read(bytes, 0, recordSize);
                if (len != recordSize)
                {
                    return default(T);
                }
                var str = Encoding.UTF8.GetString(bytes).Trim();
                return JsonConvert.DeserializeObject<T>(str, jsonSettings);
            }
        }

        /// <summary>
        /// Serialize an item to the file at the specified index, overwriting any existing value.
        /// </summary>
        /// <param name="item">The item to serialize.</param>
        /// <param name="index">The index at which to write the item.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// If the serialized item is longer than the current record size, then a compact action will be triggered to adjust the record size.<br />
        /// Setting the index to <see cref="Count"/> will append the item to the end of the collection.
        /// </remarks>
        protected virtual void SetItem(T item, int index)
        {
            if (index > count) throw new IndexOutOfRangeException();
            lock (lockObj)
            {
                file.Seek(index * (recordSize + 2), SeekOrigin.Begin);
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item, jsonSettings));
                var reclen = bytes.Length;
                if (bytes.Length > recordSize)
                {
                    if (asScratch) throw new InvalidOperationException("Debug your code! Capacity change on collection change cannot happen/should not happen.");
                    var newRecSize = bytes.Length + (CHUNK_SIZE - (bytes.Length % CHUNK_SIZE));
                    if (count > 0)
                    {
                        Compact(-1, newRecSize);
                        file.Seek(index * (recordSize + 2), SeekOrigin.Begin);
                    }
                }

                Array.Resize(ref bytes, recordSize);
                for (int i = reclen; i < recordSize; i++)
                {
                    bytes[i] = (byte)' ';
                }
                file.Write(bytes, 0, recordSize);
                file.Write(CrLf, 0, 2);
            }
        }

        /// <summary>
        /// Compact the disk file
        /// </summary>
        /// <param name="skip">Record to skip or -1 to not skip records. (Used for removing records)</param>
        /// <param name="forceSize">Force the records to have the specified size, or 0 for default.</param>
        protected virtual void Compact(int skip, int forceSize)
        {
            lock (lockObj)
            {
                const int BufferSize = BUFFER_SIZE;

                var decoder = Encoding.UTF8.GetDecoder();

                byte[] byteBuffer = new byte[BufferSize];
                byte[] recordBuffer = new byte[recordSize];
                char[] charBuffer = new char[Encoding.UTF8.GetMaxCharCount(BufferSize)];

                var currentLine = new StringBuilder();
                var recCount = 0;

                SetupFromDiskState(true);

                file?.Close();
                file = null;

                var temp = GetTempName();
                recCount = 0;

                var newColSize = forceSize != 0 ? forceSize : recordSize;
                using (var otherCol = new DiskCollection<T>(temp, newColSize, jsonSettings))
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
                SetupFromDiskState(false);
            }
        }

        private void OpenFile(bool overwrite = false)
        {
            if (isReadOnly)
            {
                file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else if (!overwrite)
            {
                file = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            else
            {
                file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            }            
        }

        private void SetupFromDiskState(bool setRecordSize)
        {
            lock (lockObj)
            {
                if (file == null) OpenFile();
                file.Seek(0, SeekOrigin.End);
                var currcapacity = GetCurrentState(out var exist, out var existSize, out var existActual, out _);

                if (exist > 0) count = exist;
                if (existSize > 0) recordSize = existSize;
                if (existActual == 0) existActual = recordSize;
                if (setRecordSize)
                {
                    recordSize = existActual + (CHUNK_SIZE - (existActual % CHUNK_SIZE));
                }
            }
        }
        
        private void CloseFile()
        {
            lock (lockObj)
            {
                file?.Close();
                file = null;
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
            int i = 0;
            int c = count;
            isenumerating++;
            for (i = 0; i < c; i++)
            {
                if (c != count) throw new InvalidOperationException("Collection changed during enumeration");
                yield return GetItem(i);
            }
            isenumerating--;
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
