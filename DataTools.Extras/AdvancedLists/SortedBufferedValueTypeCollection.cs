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
using System.Xml.Linq;

namespace DataTools.Extras.AdvancedLists
{
    /// <summary>
    /// A sorted, spatially buffered collection.
    /// </summary>
    /// <typeparam name="T">The type of the collection (must be a class)</typeparam>
    /// <remarks>
    /// Items cannot be <see cref="null"/>.  Null is reserved for buffer space.
    /// </remarks>
    public class SortedBufferedCollection<T> : ICollection<T> 
    {
        protected static byte defaultSpace = 5;

        /// <summary>
        /// Gets or sets the default buffering to utilize when creating new instances of <see cref="SortedBufferedCollection{T}"/> when a space parameter is not explicitly provided.
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
        protected IComparer<T> comparer;
        protected List<T> items;
        protected object syncRoot = new object();
        protected byte space = DefaultSpace;
        protected T[] arrspace;
        protected TreeWalker<T> walker;

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
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(byte space, IComparer<T> comparer, SortOrder sortOrder) : base()
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
                        throw new ArgumentNullException();
                    }
                });
            }
            else
            {
                this.comparer = comparer;
            
                comp = new Comparison<T>((x, y) =>
                {
                    if (x is object && y is object)
                    {
                        return this.comparer.Compare(x, y);
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }
                });

            }

            walker = new TreeWalker<T>(items, comp, sortOrder);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection() : this(SortOrder.Ascending) { }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(SortOrder sortOrder) : this(DefaultSpace, (IComparer<T>)null, sortOrder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(IComparer<T> comparer) : this(DefaultSpace, comparer, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(byte space) : this(space, (IComparer<T>)null, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(IEnumerable<T> initialItems, IComparer<T> comparer, SortOrder sortOrder) : this(DefaultSpace, comparer, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(IEnumerable<T> initialItems, IComparer<T> comparer) : this(DefaultSpace, comparer, SortOrder.Ascending)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(IEnumerable<T> initialItems, SortOrder sortOrder) : this(DefaultSpace, (IComparer<T>)null, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(byte space, SortOrder sortOrder) : this(space, (IComparer<T>)null, sortOrder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(byte space, IComparer<T> comparer) : this(space, comparer, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(byte space, IEnumerable<T> initialItems, IComparer<T> comparer, SortOrder sortOrder) : this(space, comparer, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(byte space, IEnumerable<T> initialItems, IComparer<T> comparer) : this(space, comparer, SortOrder.Ascending)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SortedBufferedCollection{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public SortedBufferedCollection(byte space, IEnumerable<T> initialItems, SortOrder sortOrder) : this(space, (IComparer<T>)null, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Adds multiple items to the <see cref="SortedBufferedCollection{T}"/> at once.
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
                if (!items[idx].Equals(item)) throw new KeyNotFoundException();

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
            return walker.Walk(item1);
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
                    if (!(item is object)) continue;
                    array[arrayIndex++] = item;
                }
            }
        }

        /// <summary>
        /// Copies <paramref name="count"/> elements of the <see cref="SortedBufferedCollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
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
                    if (!(item is object)) continue;

                    array[arrayIndex++] = item;
                    c++;

                    if (c == count) return;
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="Array"/> of the items in this <see cref="SortedBufferedCollection{T}"/>.
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
                var idx = walker.Walk(item, TreeWalkMode.Locate);
                if (idx >= count || idx < 0) return false;

                if (items[idx] is object && items[idx].Equals(item))
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
        /// <see cref="SortedBufferedCollection{T}"/> enumerator.
        /// </summary>
        public class SortedBufferedObjectCollectionEnumerator : IEnumerator<T>
        {

            SortedBufferedCollection<T> collection;
            T current = default;

            int idx = -1;
            int count = 0;

            public SortedBufferedObjectCollectionEnumerator(SortedBufferedCollection<T> collection)
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
    public enum TreeWalkMode
    {
        Null,
        Locate
    }

    public class TreeWalker<T>
    {
        private IList<T> items;
        private readonly SortOrder sortOrder;

        protected Comparison<T> comp;

        protected int lo;
        protected int hi;
        protected int mid;
        protected int count;

        private int m;

        public IReadOnlyList<T> Items => (IReadOnlyList<T>)items;

        public SortOrder SortOrder => sortOrder;

        public static TreeWalker<TComp> CreateFromIComparable<TComp>(IList<TComp> items, SortOrder sortOrder = SortOrder.Ascending) where TComp: IComparable<T>
        {
            var tw = new TreeWalker<TComp>(items);

            tw.comp = new Comparison<TComp>((x, y) =>
            {
                if (y is T b)
                {
                    return x.CompareTo(b);
                }
                else
                {
                    throw new ArgumentNullException();
                }
            });

            return tw;
        }

        protected TreeWalker(IList<T> items, SortOrder sortOrder = SortOrder.Ascending)
        {
            this.sortOrder = sortOrder;
            this.items = items;
            if (sortOrder == SortOrder.Ascending)
            {
                m = 1;
            }
            else
            {
                m = -1;
            }

            Reset();
        }

        public TreeWalker(IList<T> items, Comparison<T> comparerFunc, SortOrder sortOrder = SortOrder.Ascending) : this(items, sortOrder)
        {
            comp = comparerFunc;
        }

        public void Reset()
        {
            lo = 0;
            hi = items.Count - 1;
            mid = 0;
        }
                
        public int Walk(T item1, TreeWalkMode locateOrNull = TreeWalkMode.Null)
        {
            Reset();
            int x = InnerWalk(item1, locateOrNull);

            if (locateOrNull == TreeWalkMode.Null && count > 0 && items[x] is object)
            {
                if (x > 0 && !(items[x - 1] is object))
                {
                    items[x - 1] = items[x];
                    items[x] = default;
                }
                else if (x < count - 1 && !(items[x + 1] is object))
                {
                    items[x + 1] = items[x];
                    items[x] = default;
                }
            }

            return x;

        }

        protected virtual int InnerWalk(T item1, TreeWalkMode locateOrNull)
        {
            T item2;

            int r;

            if (hi < lo)
            {
                return lo;
            }

            mid = (hi + lo) / 2;

            item2 = items[mid];

            if (item2 == null)
            {
                int r1 = 2, r2 = 2;
                int x1 = -1, x2 = -1;

                if (mid <= hi)
                {
                    T item3 = default;
                    x2 = mid + 1;
                    while (!(item3 is object) && x2 <= hi)
                    {
                        item3 = items[x2];
                        x2++;
                    }

                    if (item3 is object)
                    {
                        r2 = comp(item1, item3) * m;
                        if (r2 > 0)
                        {
                            lo = mid + 1;
                            return InnerWalk(item1, locateOrNull);
                        }
                    }
                }

                if (mid >= lo)
                {
                    x1 = mid - 1;
                    while (!(item2 is object) && x1 >= lo)
                    {
                        item2 = items[x1];
                        x1--;
                    }

                    if (item2 is object)
                    {
                        r1 = comp(item1, item2) * m;
                        if (r1 < 0)
                        {
                            hi = mid - 1;
                            return InnerWalk(item1, locateOrNull);
                        }
                    }
                }

                if (r2 != 2 && r1 != 2)
                {
                    if (r1 == 0 && r2 == 0)
                    {
                        return mid;
                    }
                    else if (r1 == 0)
                    {
                        return x1 + 1;
                    }
                    else if (r2 == 0)
                    {
                        return x2 - 1;
                    }
                    else if (r1 > 0 && r2 < 0) return mid;
                }

                if (item2 == null)
                {
                    hi = mid - 1;
                }
            }
            
            if (item2 != null)
            {
                r = comp(item1, item2) * m;

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

            return InnerWalk(item1, locateOrNull);
        }

    }




}
