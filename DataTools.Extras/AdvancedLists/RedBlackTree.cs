using DataTools.SortedLists;
using DataTools.Text;
using DataTools.Text.ByteOrderMark;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    public class RedBlackTree<T> : ICollection<T> 
    {
        protected SortOrder sortOrder;
        protected int count = 0;
        protected Comparison<T> comp;
        protected IComparer<T> comparer;
        protected List<T> items;
        protected object syncRoot = new object();
        protected T[] arrspace;
        protected TreeWalker<T> walker;

        /// <summary>
        /// Gets the sort order for the current instance.
        /// </summary>
        public SortOrder SortOrder => sortOrder;

        /// <summary>
        /// Gets the total number of actual elements.
        /// </summary>
        public int Count => count;

        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(IComparer<T> comparer, SortOrder sortOrder) : base()
        {
            items = new List<T>();
            
            arrspace = new T[2];
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
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree() : this(SortOrder.Ascending) { }

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(SortOrder sortOrder) : this((IComparer<T>)null, sortOrder)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(IComparer<T> comparer) : this(comparer, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(byte space) : this((IComparer<T>)null, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(IEnumerable<T> initialItems, IComparer<T> comparer, SortOrder sortOrder) : this(comparer, sortOrder)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(IEnumerable<T> initialItems, IComparer<T> comparer) : this(comparer, SortOrder.Ascending)
        {
            AddRange(initialItems);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="initialItems">The initial items used to populate the collection.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(IEnumerable<T> initialItems, SortOrder sortOrder) : this((IComparer<T>)null, sortOrder)
        {
            AddRange(initialItems);
        }

      
        /// <summary>
        /// Adds multiple items to the <see cref="RedBlackTree{T}"/> at once.
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
                    if (index % 2 == 0)
                    {
                        arrspace[0] = item;
                        arrspace[1] = default;

                    }
                    else
                    {
                        arrspace[0] = default;
                        arrspace[1] = item;
                    }

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
                items[index] = default;
                walker.BalanceTree(index);
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
        /// Copies <paramref name="count"/> elements of the <see cref="RedBlackTree{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
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
        /// Create a new <see cref="Array"/> of the items in this <see cref="RedBlackTree{T}"/>.
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
            return new RedBlackTreeEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RedBlackTreeEnumerator(this);
        }

        /// <summary>
        /// <see cref="RedBlackTree{T}"/> enumerator.
        /// </summary>
        public class RedBlackTreeEnumerator : IEnumerator<T>
        {

            RedBlackTree<T> collection;
            T current = default;

            int idx = -1;
            int count = 0;

            public RedBlackTreeEnumerator(RedBlackTree<T> collection)
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
        private List<T> items;
        private readonly SortOrder sortOrder;

        protected Comparison<T> comp;

        protected int lo;
        protected int hi;
        protected int mid;
        protected int count;

        private int m;

        public IReadOnlyList<T> Items => (IReadOnlyList<T>)items;

        public SortOrder SortOrder => sortOrder;

        public static TreeWalker<TComp> CreateFromIComparable<TComp>(List<TComp> items, SortOrder sortOrder = SortOrder.Ascending) where TComp: IComparable<T>
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

        protected TreeWalker(List<T> items, SortOrder sortOrder = SortOrder.Ascending)
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

        public TreeWalker(List<T> items, Comparison<T> comparerFunc, SortOrder sortOrder = SortOrder.Ascending) : this(items, sortOrder)
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
            return x;

        }

        protected virtual int InnerWalk(T item1, TreeWalkMode locateOrNull)
        {
            T item2;
            int r;

            bool isred;
            bool isnullred = false;

            if (hi < lo)
            {
                if (locateOrNull == TreeWalkMode.Null && lo % 2 == 0)
                {
                    if (lo < count - 1 && !(items[lo + 1] is object))
                    {
                        r = comp(item1, items[lo]) * m;
                        if (r >= 0) lo++;
                    }
                    
                    else if (lo > 0 && !(items[lo - 1] is object))
                    {
                        if (lo < count)
                        {
                            r = comp(item1, items[lo]) * m;
                            if (r <= 0) lo--;
                        }
                        else
                        {
                            lo--;
                        }
                    }
                }
                return lo;
            }

            mid = (hi + lo) / 2;
            isred = (mid % 2) == 1;

            item2 = items[mid];
            
            if (!(item2 is object))
            {
                if (isred)
                {
                    item2 = items[mid - 1];
                    isnullred = true;
                }
                else
                {
                    throw new TreeUnbalancedException("Black node is null!");
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
                    lo = mid;
                    hi = mid - 1;
                }
            }

            return InnerWalk(item1, locateOrNull);
        }


        public void BalanceTree(int startNode)
        {
            if (startNode == -1 || startNode >= count) return;
            var isred = startNode % 2 == 1;

            if (isred)
            {
                int i = startNode - 1;

                if (!(items[startNode] is object) && !(items[i] is object))
                {
                    items.RemoveRange(i, 2);
                }
                else if (items[startNode] is object && !(items[i] is object))
                {
                    items[i] = items[startNode];
                    items[startNode] = default;
                }
            }
            else
            {
                int i = startNode + 1;

                if (!(items[startNode] is object) && !(items[i] is object))
                {
                    items.RemoveRange(startNode, 2);
                }
                else if (!(items[startNode] is object) && (items[i] is object))
                {
                    items[startNode] = items[i];
                    items[i] = default;
                }

            }
        }

    }

    public class TreeUnbalancedException : Exception
    {
        public TreeUnbalancedException() : base()
        {

        }
        public TreeUnbalancedException(string message) : base(message)
        {
        }

    }


}
