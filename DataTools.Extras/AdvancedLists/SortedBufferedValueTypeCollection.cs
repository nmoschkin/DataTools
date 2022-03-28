using DataTools.SortedLists;
using DataTools.Text;
using DataTools.Text.ByteOrderMark;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataTools.Extras.AdvancedLists
{
    /// <summary>
    /// A sorted, spatially buffered collection.
    /// </summary>
    /// <typeparam name="T">The type of the collection (must be a class)</typeparam>
    /// <remarks>
    /// Items cannot be <see cref="null"/>.  Null is reserved for buffer space.
    /// </remarks>
    public class SortedBufferedObjectCollection<T> : ICollection<T> where T : class
    {
        protected static byte defaultSpace = 5;

        /// <summary>
        /// Gets or sets the default buffering to utilize when creating new instances of <see cref="SortedBufferedObjectCollection{T}"/> when a space parameter is not explicitly provided.
        /// </summary>
        /// <remarks>
        /// Value must be 1-255.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static byte DefaultSpace
        {
            get => defaultSpace;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
                defaultSpace = value;
            }
        }

        protected SortOrder sortOrder;
        protected int count = 0;
        protected Comparison<T> comp;
        protected List<T> items;
        protected object syncRoot = new object();
        protected byte space = DefaultSpace;
        protected T[] arrspace;

        /// <summary>
        /// Gets the sort order for the current instance.
        /// </summary>
        public SortOrder SortOrder => sortOrder;

        /// <summary>
        /// Gets the spatial buffering for the current instance.
        /// </summary>
        public int Space => space;

        /// <summary>
        /// Gets the total number of actual elements.
        /// </summary>
        public int Count => count;

        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(byte space, IComparer<T> comparer, SortOrder sortOrder) : base()
        {
            if (space < 1) throw new ArgumentOutOfRangeException(nameof(space));
            items = new List<T>();

            this.space = space;
            arrspace = new T[space];

            this.sortOrder = sortOrder;

            if (comparer == null)
            {
                typeof(T).GetInterfaceMap(typeof(IComparable<T>));
                comp = new Comparison<T>((x, y) =>
                {
                    if (x is IComparable<T> a && y is T b)
                    {
                        return a.CompareTo(b);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                });
            }
            else
            {
                comp = comparer.Compare;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection() : this(SortOrder.Ascending) { }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(SortOrder sortOrder) : this(DefaultSpace, (IComparer<T>)null, sortOrder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(IComparer<T> comparer) : this(DefaultSpace, comparer, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(byte space) : this(space, (IComparer<T>)null, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(IEnumerable<T> initialItems, IComparer<T> comparer, SortOrder sortOrder) : this(DefaultSpace, comparer, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(IEnumerable<T> initialItems, IComparer<T> comparer) : this(DefaultSpace, comparer, SortOrder.Ascending)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(IEnumerable<T> initialItems, SortOrder sortOrder) : this(DefaultSpace, (IComparer<T>)null, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(byte space, SortOrder sortOrder) : this(space, (IComparer<T>)null, sortOrder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(byte space, IComparer<T> comparer) : this(space, comparer, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(byte space, IEnumerable<T> initialItems, IComparer<T> comparer, SortOrder sortOrder) : this(space, comparer, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(byte space, IEnumerable<T> initialItems, IComparer<T> comparer) : this(space, comparer, SortOrder.Ascending)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedObjectCollection(byte space, IEnumerable<T> initialItems, SortOrder sortOrder) : this(space, (IComparer<T>)null, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Adds multiple items to the <see cref="SortedBufferedObjectCollection{T}"/> at once.
        /// </summary>
        /// <param name="newItems"></param>
        public void AddRange(IEnumerable<T> newItems)
        {
            foreach (var item in newItems)
            {
                Add(item);
            }
        }

        public void AlterItem(T item, Action<T> alteration)
        {
            lock (syncRoot)
            {
                int idx = GetInsertIndex(item);

                if (idx >= count || idx < 0) throw new KeyNotFoundException();
                if (items[idx] != item) throw new KeyNotFoundException();

                alteration(item);


            }
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        protected virtual void ClearItems()
        {
            lock (syncRoot)
            {
                items.Clear();
                count = 0;
            }
        }

        /// <summary>
        /// Insert an item into the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException" />
        protected virtual void InsertItem(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            lock (syncRoot)
            {
                var index = GetInsertIndex(item);

                if (index < items.Count && items[index] == null)
                {
                    items[index] = item;
                }
                else
                {
                    arrspace[space - 1] = item;
                    items.InsertRange(index, arrspace);
                }

                count++;
            }
        }

        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="index"></param>
        protected virtual void RemoveItem(int index)
        {
            lock (syncRoot)
            {
                int cn = 0;
                int c = items.Count;
                int b = index;
                int e = b + space;

                items[index] = default;

                for (int i = b; i < e; i++)
                {
                    if (items[index] == null)
                    {
                        cn++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (cn >= space) items.RemoveAt(index);
                count--;
            }
        }

        /// <summary>
        /// Get the appropriate insert index for the configured sort direction, based on price.
        /// </summary>
        /// <param name="item1">The order unit to test.</param>
        /// <returns>The calculated insert index based on the sort direction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetInsertIndex(T item1)
        {
            T item2;

            var count = items.Count;
            if (count == 0) return 0;

            int hi = count - 1;
            int lo = 0;
            int mid;
            int xmid;
            int r;

            if (sortOrder == SortOrder.Ascending)
            {
                // ascending
                while (true)
                {
                    if (hi < lo)
                    {
                        

                        xmid = count - 1;
                        hi = lo + (space - 1);
                        if (xmid > hi) xmid = hi;

                        while (lo < xmid)
                        {
                            if (items[lo] == null) break;
                            else if (comp(item1, items[lo]) <= 0) break;
                            lo++;
                        }

                        return lo;
                    }

                    mid = (hi + lo) / 2;

                    for (int t = mid; t <= hi; t++)
                    {
                        if (items[t] != null)
                        {
                            mid = t;
                            break;
                        }
                    }

                    item2 = items[mid];
                    if (item2 == null)
                    {
                        hi = mid - 1;
                    }
                    else
                    {
                        r = comp(item1, item2);

                        if (r > 0)
                        {
                            lo = mid + 1;
                        }
                        else if (r < 0)
                        {
                            hi = mid - 1;
                        }
                        else
                        {
                            return mid;
                        }

                    }
                }
            }
            else
            {
                while (true)
                {
                    if (hi < lo)
                    {
                        

                        xmid = count - 1;
                        hi = lo + (space - 1);
                        if (xmid > hi) xmid = hi;

                        while (lo < xmid)
                        {
                            if (items[lo] == null) break;
                            else if (comp(item1, items[lo]) >= 0) break;
                            lo++;
                        }

                        return lo;
                    }

                    mid = (hi + lo) / 2;

                    for (int t = mid; t <= hi; t++)
                    {
                        if (items[t] != null)
                        {
                            mid = t;
                            break;
                        }
                    }

                    item2 = items[mid];

                    if (item2 == null)
                    {
                        hi = mid - 1;
                    }
                    else
                    {
                        r = comp(item1, item2);

                        if (r < 0)
                        {
                            lo = mid + 1;
                        }
                        else if (r > 0)
                        {
                            hi = mid - 1;
                        }
                        else
                        {
                            return mid;
                        }

                    }
                }

            }

        }

        public void Add(T item)
        {
            InsertItem(item);
        }

        public void Clear()
        {
            ClearItems();
        }

        public bool Contains(T item)
        {
            lock (syncRoot)
            {
                return items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                foreach (var item in items)
                {
                    if (item is null) continue;
                    array[arrayIndex++] = item;
                }
            }
        }

        /// <summary>
        /// Copies <paramref name="count"/> elements of the <see cref="SortedBufferedObjectCollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            lock (syncRoot)
            {
                if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));

                int c = 0;

                foreach (var item in items)
                {
                    if (item is null) continue;

                    array[arrayIndex++] = item;
                    c++;

                    if (c == count) return;
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="Array"/> of the items in this <see cref="SortedBufferedObjectCollection{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="Array"/> with all the actual items.</returns>
        public T[] ToArray()
        {
            lock (syncRoot)
            {
                var l = new List<T>();

                foreach (var item in items)
                {
                    if (item != null)
                    {
                        l.Add(item);
                    }
                }

                return l.ToArray();
            }
        }

        public T[] ToArray(int elementCount)
        {
            lock (syncRoot)
            {
                var l = new List<T>();
                int x = 0;

                foreach (var item in items)
                {
                    if (item != null)
                    {
                        l.Add(item);

                        x++;
                        if (x == elementCount) break;
                    }
                }

                return l.ToArray();
            }
        }


        public bool Remove(T item)
        {
            lock (syncRoot)
            {
                var idx = GetInsertIndex(item);
                if (idx >= count || idx < 0) return false;

                if (items[idx] == item)
                {
                    RemoveItem(idx);
                    return true;
                }
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SortedBufferedObjectCollectionEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SortedBufferedObjectCollectionEnumerator(this);
        }

        /// <summary>
        /// <see cref="SortedBufferedObjectCollection{T}"/> enumerator.
        /// </summary>
        public class SortedBufferedObjectCollectionEnumerator : IEnumerator<T>
        {

            SortedBufferedObjectCollection<T> collection;
            T current = default;

            int idx = -1;
            int count = 0;

            public SortedBufferedObjectCollectionEnumerator(SortedBufferedObjectCollection<T> collection)
            {
                this.collection = collection;
                count = collection.items.Count;
            }

            public T Current => current;
            object IEnumerator.Current => current;

            public void Dispose()
            {
                collection = null;
                Reset();
            }

            public bool MoveNext()
            {
                idx++;
                while (idx < count)
                {
                    current = collection.items[idx];
                    if (current is object)
                    {
                        break;
                    }
                    idx++;
                }

                return idx < count;
            }

            public void Reset()
            {
                idx = -1;
                current = default;
            }
        }
    }


    /// <summary>
    /// A sorted, spatially buffered collection.
    /// </summary>
    /// <typeparam name="T">The type of the collection (must be a class)</typeparam>
    /// <remarks>
    /// Items cannot be <see cref="null"/>.  Null is reserved for buffer space.
    /// </remarks>
    public class SortedBufferedValueTypeCollection<T> : ICollection<T> where T : struct
    {
        protected static byte defaultSpace = 5;

        /// <summary>
        /// Gets or sets the default buffering to utilize when creating new instances of <see cref="SortedBufferedValueTypeCollection{T}"/> when a space parameter is not explicitly provided.
        /// </summary>
        /// <remarks>
        /// Value must be 1-255.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static byte DefaultSpace
        {
            get => defaultSpace;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
                defaultSpace = value;
            }
        }

        protected IComparer<T> comparer;    
        protected SortOrder sortOrder;
        protected int count = 0;
        protected Comparison<T?> comp;
        protected List<T?> items;
        protected object syncRoot = new object();
        protected byte space = DefaultSpace;
        protected T?[] arrspace;

        /// <summary>
        /// Gets the sort order for the current instance.
        /// </summary>
        public SortOrder SortOrder => sortOrder;

        /// <summary>
        /// Gets the spatial buffering for the current instance.
        /// </summary>
        public int Space => space;

        /// <summary>
        /// Gets the total number of actual elements.
        /// </summary>
        public int Count => count;

        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(byte space, IComparer<T> comparer, SortOrder sortOrder) : base()
        {
            if (space < 1) throw new ArgumentOutOfRangeException(nameof(space));
            items = new List<T?>();

            this.space = space;
            arrspace = new T?[space];

            this.sortOrder = sortOrder;

            if (comparer == null)
            {
                typeof(T).GetInterfaceMap(typeof(IComparable<T>));

                comp = new Comparison<T?>((x, y) =>
                {
                    if (x is IComparable<T> a && y is T b)
                    {
                        return a.CompareTo(b);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                });
            }
            else
            {
                this.comparer = comparer;

                comp = new Comparison<T?>((x, y) =>
                {
                    if (x is T a && y is T b)
                    {
                        return this.comparer.Compare(a, b);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                });
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection() : this(SortOrder.Ascending) { }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(SortOrder sortOrder) : this(DefaultSpace, (IComparer<T>)null, sortOrder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(IComparer<T> comparer) : this(DefaultSpace, comparer, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(byte space) : this(space, (IComparer<T>)null, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(IEnumerable<T> initialItems, IComparer<T> comparer, SortOrder sortOrder) : this(DefaultSpace, comparer, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(IEnumerable<T> initialItems, IComparer<T> comparer) : this(DefaultSpace, comparer, SortOrder.Ascending)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(IEnumerable<T> initialItems, SortOrder sortOrder) : this(DefaultSpace, (IComparer<T>)null, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(byte space, SortOrder sortOrder) : this(space, (IComparer<T>)null, sortOrder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(byte space, IComparer<T> comparer) : this(space, comparer, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(byte space, IEnumerable<T> initialItems, IComparer<T> comparer, SortOrder sortOrder) : this(space, comparer, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(byte space, IEnumerable<T> initialItems, IComparer<T> comparer) : this(space, comparer, SortOrder.Ascending)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedValueTypeCollection(byte space, IEnumerable<T> initialItems, SortOrder sortOrder) : this(space, (IComparer<T>)null, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Adds multiple items to the <see cref="SortedBufferedValueTypeCollection{T}"/> at once.
        /// </summary>
        /// <param name="newItems"></param>
        public void AddRange(IEnumerable<T> newItems)
        {
            foreach (var item in newItems)
            {
                Add(item);
            }
        }

        public void AlterItem(T item, Action<T> alteration)
        {
            lock (syncRoot)
            {
                int idx = GetInsertIndex(item);

                if (idx >= count || idx < 0) throw new KeyNotFoundException();
                if (comp(items[idx] ?? throw new KeyNotFoundException(), item) != 0) throw new KeyNotFoundException();

                alteration(item);


            }
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        protected virtual void ClearItems()
        {
            lock (syncRoot)
            {
                items.Clear();
                count = 0;
            }
        }
        public int NullInserts { get; private set; } = 0;

        public int NewInserts { get; private set; } = 0;

        /// <summary>
        /// Insert an item into the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException" />
        protected virtual void InsertItem(T? item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            lock (syncRoot)
            {
                var index = GetInsertIndex(item.Value);

                if (index < items.Count && items[index] == null)
                {
                    items[index] = item;
                    NullInserts++;
                }
                else
                {
                    arrspace[space - 1] = item;
                    items.InsertRange(index, arrspace);
                    NewInserts++;
                }

                count++;
            }
        }

        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="index"></param>
        protected virtual void RemoveItem(int index)
        {
            lock (syncRoot)
            {
                int cn = 0;
                int c = items.Count;
                int b = index;
                int e = b + space;

                items[index] = default;

                for (int i = b; i < e; i++)
                {
                    if (items[index] == null)
                    {
                        cn++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (cn >= space) items.RemoveAt(index);
                count--;
            }
        }
        
        /// <summary>
        /// Get the appropriate insert index for the configured sort direction, based on price.
        /// </summary>
        /// <param name="item1">The order unit to test.</param>
        /// <returns>The calculated insert index based on the sort direction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetInsertIndex(T item1)
        {
            T? item2;

            var count = items.Count;
            if (count == 0) return 0;

            int hi = count - 1;
            int lo = 0;
            int mid;
            int xmid;
            int r;

            if (sortOrder == SortOrder.Ascending)
            {
                // ascending
                while (true)
                {
                    if (hi < lo)
                    {
                        return lo;
                    }

                    mid = (hi + lo) / 2;

                    for (int t = mid; t <= hi; t++)
                    {
                        if (items[t] != null)
                        {
                            mid = t;
                            break;
                        }
                    }

                    item2 = items[mid];
                    if (item2 == null)
                    {
                        hi = mid - 1;
                    }
                    else
                    {
                        r = comp(item1, item2);

                        if (r > 0)
                        {
                            lo = mid + 1;
                        }
                        else if (r < 0)
                        {
                            hi = mid - 1;
                        }
                        else
                        {
                            return mid;
                        }

                    }
                }
            }
            else
            {
                while (true)
                {
                    if (hi < lo)
                    {
                        return lo;
                        //return ClosestNull(item1, lo);
                    }

                    mid = (hi + lo) / 2;

                    for (int t = mid; t <= hi; t++)
                    {
                        if (items[t] != null)
                        {
                            mid = t;
                            break;
                        }
                    }

                    item2 = items[mid];

                    if (item2 == null)
                    {
                        hi = mid - 1;
                    }
                    else
                    {
                        r = comp(item1, item2);

                        if (r < 0)
                        {
                            lo = mid + 1;
                        }
                        else if (r > 0)
                        {
                            hi = mid - 1;
                        }
                        else
                        {
                            return mid;
                        }

                    }
                }

            }

        }
        //private int ClosestNull(T value, int mid)
        //{
        //    int lo = 0;
        //    int hi = count - 1;

        //    if (sortOrder == SortOrder.Ascending)
        //    {
        //        for (int i = mid - 1; i >= lo; i--)
        //        {
        //            if (items[i] == null) return i;
        //            else if (comp(items[i], value) < 0)
        //            {
        //                return i + 1;
        //            }
        //        }

        //        for (int i = mid + 1; i <= hi; i++)
        //        {
        //            if (items[i] == null) return i;
        //            else if (comp(items[i], value) > 0)
        //            {
        //                return i - 1;
        //            }
        //        }

        //    }
        //    else
        //    {
        //        for (int i = mid - 1; i >= lo; i--)
        //        {
        //            if (items[i] == null) return i;
        //            else if (comp(items[i], value) > 0)
        //            {
        //                return i + 1;
        //            }
        //        }

        //        for (int i = mid + 1; i <= hi; i++)
        //        {
        //            if (items[i] == null) return i;
        //            else if (comp(items[i], value) < 0)
        //            {
        //                return i - 1;
        //            }
        //        }
        //    }


        //    return mid;
        //}

        public void Add(T item)
        {
            InsertItem(item);
        }

        public void Clear()
        {
            ClearItems();
        }

        public bool Contains(T item)
        {
            lock (syncRoot)
            {
                return items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                foreach (var item in items)
                {
                    if (item is T o) array[arrayIndex++] = o;
                }
            }
        }

        /// <summary>
        /// Copies <paramref name="count"/> elements of the <see cref="SortedBufferedValueTypeCollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            lock (syncRoot)
            {
                if (count < DefaultSpace) throw new ArgumentOutOfRangeException(nameof(count));

                int c = 0;

                foreach (var item in items)
                {
                    if (item is T o)
                    {
                        array[arrayIndex++] = o;
                        c++;

                        if (c == count) return;
                    }
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="Array"/> of the items in this <see cref="SortedBufferedValueTypeCollection{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="Array"/> with all the actual items.</returns>
        public T[] ToArray()
        {
            lock (syncRoot)
            {
                var l = new List<T>();

                foreach (var item in items)
                {
                    if (item is T o)
                    {
                        l.Add(o);
                    }
                }

                return l.ToArray();
            }
        }

        public T[] ToArray(int elementCount)
        {
            lock (syncRoot)
            {
                var l = new List<T>();
                int x = 0;

                foreach (var item in items)
                {
                    if (item is T o)
                    {
                        l.Add(o);

                        x++;
                        if (x == elementCount) break;
                    }
                }

                return l.ToArray();
            }
        }


        public bool Remove(T item)
        {
            lock (syncRoot)
            {
                var idx = GetInsertIndex(item);
                if (idx >= count || idx < 0) return false;

                if (comp(items[idx], item) == 0)
                {
                    RemoveItem(idx);
                    return true;
                }
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SortedBufferedValueTypeCollectionEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SortedBufferedValueTypeCollectionEnumerator(this);
        }

        /// <summary>
        /// <see cref="SortedBufferedValueTypeCollection{T}"/> enumerator.
        /// </summary>
        public class SortedBufferedValueTypeCollectionEnumerator : IEnumerator<T>
        {

            SortedBufferedValueTypeCollection<T> collection;
            T current = default;

            int idx = -1;
            int count = 0;

            public SortedBufferedValueTypeCollectionEnumerator(SortedBufferedValueTypeCollection<T> collection)
            {
                this.collection = collection;
                count = collection.items.Count;
            }

            public T Current => current;
            object IEnumerator.Current => current;

            public void Dispose()
            {
                collection = null;
                Reset();
            }

            public bool MoveNext()
            {
                idx++;
                while (idx < count)
                {
                    if (collection.items[idx] is T o)
                    {
                        current = o;
                        break;
                    }
                    idx++;
                }

                return idx < count;
            }

            public void Reset()
            {
                idx = -1;
                current = default;
            }
        }
    }






}
