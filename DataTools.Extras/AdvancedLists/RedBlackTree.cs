﻿
/********************************************
 * 
 * DataTools Extras
 * Advanced Lists
 * 
 * Red/Black Binary Tree
 * 
 * **EXPERIMENTAL**
 * 
 * Copyright(C) 2022 Nathaniel N. Moschkin
 * All Rights Reserved
 * 
 * Licensed under the MIT License
 ********************************************/


using System;
using System.Collections;
using System.Collections.Generic;


namespace DataTools.Extras.AdvancedLists
{
    /// <summary>
    /// The result of the rebalance activity.
    /// </summary>
    public enum RebalanceResult
    {
        /// <summary>
        /// The tree was not walked.
        /// </summary>
        NotPerformed,

        /// <summary>
        /// The was walked but not rebalanced.
        /// </summary>
        Unchanged,

        /// <summary>
        /// The tree was walked and changed.
        /// </summary>
        Changed
    }

    /// <summary>
    /// Rebalance strategies
    /// </summary>
    public enum RebalanceStrategy : int
    {
        /// <summary>
        /// Examine 4 nodes locally.
        /// </summary>
        Cadance4 = 4,

        /// <summary>
        /// Examine 8 nodes locally.
        /// </summary>
        Cadence8 = 8,

        /// <summary>
        /// Examine 16 nodes locally.
        /// </summary>
        Cadence16 = 16
    }

    /// <summary>
    /// The method to use to walk the tree.
    /// </summary>
    public enum TreeWalkMode
    {
        /// <summary>
        /// Find a suitable insert index for the specified item.
        /// </summary>
        InsertIndex,

        /// <summary>
        /// Locate the specified item.
        /// </summary>
        Locate
    }

    /// <summary>
    /// A version of red/black tree with an additional buffer for keys.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public abstract class KeyedRedBlackTree<TKey, TValue> : RedBlackTree<TValue> // Do not implement: IReadOnlyDictionary<TKey, TValue>
    {
        #region Protected Fields

        protected SortedDictionary<TKey, TValue> keyDict = new SortedDictionary<TKey, TValue>();

        #endregion Protected Fields

        #region Public Constructors

        public KeyedRedBlackTree() : base()
        {
        }

        public KeyedRedBlackTree(IComparer<TValue> comparer) : base(comparer)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<TKey> Keys => keyDict.Keys;
        public IEnumerable<TValue> Values => keyDict.Values;

        #endregion Public Properties

        #region Public Indexers

        public TValue this[TKey key] => keyDict[key];

        #endregion Public Indexers

        #region Public Methods

        public bool ContainsKey(TKey key)
        {
            lock (syncRoot)
            {
                return keyDict.ContainsKey(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (syncRoot)
            {
                return keyDict.TryGetValue(key, out value);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void InsertItem(TValue item)
        {
            lock (syncRoot)
            {
                keyDict.Add(ProvideKey(item), item);
                base.InsertItem(item);
            }
        }

        protected abstract TKey ProvideKey(TValue value);
        protected override void RemoveItem(int index)
        {
            lock (syncRoot)
            {
                var item = items[index];
                keyDict.Remove(ProvideKey(item));
                base.RemoveItem(index);
            }
        }

        #endregion Protected Methods
    }

    /// <summary>
    /// A red/black tree implementation.
    /// </summary>
    /// <typeparam name="T">The type of the collection (must be a class)</typeparam>
    /// <remarks>
    /// Items cannot be <see cref="null"/>.
    /// </remarks>
    public class RedBlackTree<T> : ICollection<T>
    {
        #region Protected Fields

        protected T[] arrspace;
        protected Comparison<T> comp;
        protected IComparer<T> comparer;
        protected int count = 0;
        protected RebalanceStrategy globalStrategy = RebalanceStrategy.Cadance4;
        protected List<T> items;
        protected RebalanceStrategy localStrategy = RebalanceStrategy.Cadence16;
        protected float rebalanceThreshold = 1.2f;
        protected object syncRoot = new object();

        #endregion Protected Fields

        #region Private Fields

        int changedRebalances = 0;

        int hardInserts = 0;

        int hardRemoves = 0;

        bool metrics = true;

        int localRebalances = 0;

        int softInserts = 0;

        int softRemoves = 0;

        int unchangedRebalances = 0;

        float averageInsertIndex = 0f;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="space">The number of total new elements to insert for each single new element inserted.</param>
        /// <param name="comparer">The comparer class.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(IComparer<T> comparer, float threshold = 1.2f, RebalanceStrategy globStrategy = RebalanceStrategy.Cadance4, RebalanceStrategy locStrategy = RebalanceStrategy.Cadence16) : base()
        {
            rebalanceThreshold = threshold;
            globalStrategy = globStrategy;
            localStrategy = locStrategy;

            items = new List<T>();

            arrspace = new T[2];

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
                comp = comparer.Compare;
            }

        }

        /// <summary>
        /// Creates a new instance of <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RedBlackTree(float threshold = 1.2f, RebalanceStrategy globStrategy = RebalanceStrategy.Cadance4, RebalanceStrategy locStrategy = RebalanceStrategy.Cadence16) : this((IComparer<T>)null, threshold, globStrategy, locStrategy)
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
        public RedBlackTree(IEnumerable<T> initialItems, IComparer<T> comparer, float threshold = 1.2f, RebalanceStrategy globStrategy = RebalanceStrategy.Cadance4, RebalanceStrategy locStrategy = RebalanceStrategy.Cadence16) : this(comparer, threshold, globStrategy, locStrategy)
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
        public RedBlackTree(IEnumerable<T> initialItems, float threshold = 1.2f, RebalanceStrategy globStrategy = RebalanceStrategy.Cadance4, RebalanceStrategy locStrategy = RebalanceStrategy.Cadence16) : this((IComparer<T>)null, threshold, globStrategy, locStrategy)
        {
            AddRange(initialItems);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that determines whether metrics are recorded.
        /// </summary>
        /// <remarks>
        /// Setting this value will reset all metrics to 0.
        /// </remarks>
        public bool EnableMetrics
        {
            get => metrics;
            set
            {
                if (metrics == value) return;

                lock (syncRoot)
                {
                    metrics = value;
                    ResetMetrics();
                }
            }
        }

        /// <summary>
        /// (Metrics) The average insert index.
        /// </summary>
        public float AverageInsertIndex => averageInsertIndex;

        /// <summary>
        /// (Metrics) Number of inserts performed by resizing the tree.
        /// </summary>
        public int HardInserts => hardInserts;

        /// <summary>
        /// (Metrics) Number of removes performed by resizing the tree.
        /// </summary>
        public int HardRemoves => hardRemoves;

        /// <summary>
        /// (Metrics) Number of local rebalances.
        /// </summary>
        public int LocalRebalances => localRebalances;

        /// <summary>
        /// (Metrics) Number of inserts performed without resizing the tree.
        /// </summary>
        public int SoftInserts => softInserts;

        /// <summary>
        /// (Metrics) Number of removes performed without resizing the tree.
        /// </summary>
        public int SoftRemoves => softRemoves;

        /// <summary>
        /// (Metrics) Number of global rebalances that resulted in changes to the tree.
        /// </summary>
        public int ChangedRebalances => changedRebalances;

        /// <summary>
        /// (Metrics) Number of global rebalances that were attempted but resulted in no changes to the tree.
        /// </summary>
        public int UnchangedRebalances => unchangedRebalances;


        /// <summary>
        /// Gets the total number of actual elements.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Gets the actual size of the tree.
        /// </summary>
        public int TreeSize => items.Count;

        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Gets the first element in the sorted collection.
        /// </summary>
        public T First
        {
            get => count == 0 ? default : items[0];
        }

        /// <summary>
        /// Gets the last element in the sorted collection.
        /// </summary>
        public T Last
        {
            get
            {
                var ic = items.Count;

                if (ic == 0) return default;
                if (items[ic - 1] is object)
                {
                    return items[ic - 1];
                }
                else
                {
                    return items[ic - 2];
                }
            }
        }

        /// <summary>
        /// The rebalance strategy to use when performing a global rebalance.
        /// </summary>
        public virtual RebalanceStrategy GlobalStrategy
        {
            get => globalStrategy;
            set
            {
                lock (syncRoot)
                {
                    globalStrategy = value;
                }
            }
        }

        /// <summary>
        /// The rebalance strategy to use when performing a local rebalance.
        /// </summary>
        public virtual RebalanceStrategy LocalStrategy
        {
            get => localStrategy;
            set
            {
                lock (syncRoot)
                {
                    localStrategy = value;
                }
            }
        }

        /// <summary>
        /// The size tolerance difference between the logical size and the tree size before a global rebalance is triggered.
        /// </summary>
        /// <remarks>
        /// Must be a value between 1 ans 2.<br/><br/>
        /// The default value is 1.2f.<br/><br/>
        /// Setting this value below 1.2f is not recommended.
        /// </remarks>
        public virtual float RebalanceThreshold
        {
            get => rebalanceThreshold;
            set
            {
                if (value < 1f || value > 2f) throw new ArgumentOutOfRangeException();

                lock (syncRoot)
                {
                    rebalanceThreshold = value;
                }
            }
        }


        #endregion Public Properties

        #region Public Methods

        public void Add(T item)
        {
            InsertItem(item);
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

        /// <summary>
        /// Alter an item.
        /// </summary>
        /// <param name="item">The item to alter.</param>
        /// <param name="alteration">The alteration function that returns the changed item.</param>
        public void AlterItem(T item, Func<T, T> alteration)
        {
            lock (syncRoot)
            {
                int idx = Walk(item, TreeWalkMode.Locate);
                if (idx == -1)
                {
                    int c = items.Count;
                    string err = $"{idx} for {item} Is Incorrect!";

                    Console.WriteLine(err);

                    for (int i = 0; i < c; i++)
                    {
                        if (items[i] is object && items[i].Equals(item))
                        {
                            err += $"\r\n{i} is the correct index for {item}!";
                            Console.WriteLine($"{i} is the correct index for {item}!");
                            break;
                        }
                    }

                    if (err == null) err += "\r\nKey Not Found!";
                    throw new KeyNotFoundException(err);
                }

                AlterItem(item, alteration, idx);
            }
        }

        public void Clear()
        {
            ClearItems();
        }

        public bool Contains(T item)
        {
            lock (syncRoot)
            {
                return Walk(item, TreeWalkMode.Locate) != -1;
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

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                if (item is object) yield return item;
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(T item)
        {
            lock (syncRoot)
            {
                var idx = Walk(item, TreeWalkMode.Locate);
                if (idx == -1) return false;

                RemoveItem(idx);
                return true;
            }
        }

        /// <summary>
        /// Reset all metrics to 0.
        /// </summary>
        public void ResetMetrics()
        {
            lock (syncRoot)
            {
                hardInserts = 0;
                softInserts = 0;

                hardRemoves = 0;
                softRemoves = 0;

                localRebalances = 0;

                changedRebalances = 0;
                unchangedRebalances = 0;

                averageInsertIndex = 0f;
            }
        }

        /// <summary>
        /// Return a new <see cref="Array"/> of the items in this <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="Array"/>.</returns>
        public T[] ToArray()
        {
            lock (syncRoot)
            {
                var l = new List<T>();

                foreach (var item in this)
                {
                    l.Add(item);
                }

                return l.ToArray();
            }
        }

        /// <summary>
        /// Return a new <see cref="Array"/> of at most <paramref name="elementCount"/> items in this <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="Array"/> with at most <paramref name="elementCount"/> items.</returns>
        public T[] ToArray(int elementCount)
        {
            if (elementCount < 1) throw new ArgumentOutOfRangeException();

            lock (syncRoot)
            {
                var l = new List<T>();
                int x = 0;

                foreach (var item in this)
                {
                    l.Add(item);

                    x++;
                    if (x >= elementCount) break;
                }

                return l.ToArray();
            }
        }

        /// <summary>
        /// Try to alter an item.
        /// </summary>
        /// <param name="item">The item to alter.</param>
        /// <param name="alteration">The alteration function that returns the changed item.</param>
        /// <returns>True if successful.</returns>
        public bool TryAlterItem(T item, Func<T, T> alteration)
        {
            var idx = Walk(item, TreeWalkMode.Locate);

            if (idx != -1)
            {
                AlterItem(item, alteration, idx);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempt to Rebalance The Tree
        /// </summary>
        /// <param name="threshold">The ratio of the tree size over the logical count at which a rebalance should be performed. Default is 1.2 : 1</param>
        /// <returns>A <see cref="RebalanceResult"/> of <see cref="RebalanceResult.NotPerformed"/>, <see cref="RebalanceResult.Unchanged"/>, or <see cref="RebalanceResult.Changed"/>.</returns>
        public RebalanceResult TryRebalance()
        {
            lock (syncRoot)
            {
                if (count > 1024 && ((float)items.Count / count) >= rebalanceThreshold)
                {
                    bool b = false;

                    for (int i = items.Count - 2; i >= 2; i -= 2)
                    {
                        b = b | LocalRebalance(i, globalStrategy, true);
                    }

                    if (b)
                    {
                        if (metrics) changedRebalances++;
                        return RebalanceResult.Changed;
                    }
                    else
                    {
                        if (metrics) unchangedRebalances++;
                        return RebalanceResult.Unchanged;
                    }
                }

                return RebalanceResult.NotPerformed;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Alter an item.
        /// </summary>
        /// <param name="item">The item to alter.</param>
        /// <param name="alteration">The alteration function that returns the changed item.</param>
        /// <param name="idx">The index of the item.</param>
        /// <remarks>
        /// This function should only be used in conjunction with a call to either <see cref="Locate(T, out int)"/> or <see cref="Walk(T, TreeWalkMode)"/>.
        /// </remarks>
        protected virtual void AlterItem(T item, Func<T, T> alteration, int idx)
        {
            lock (syncRoot)
            {

                // TODO See about soft moving the item.
                RemoveItem(idx);
                var newitem = alteration(item);
                InsertItem(newitem);
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
                var index = Walk(item);
                int rc = items.Count;

                if (index < rc && items[index] == null)
                {
                    items[index] = item;
                    if (metrics) softInserts++;
                }
                else if (index > 0 && items[index - 1] == null)
                {
                    items[index - 1] = item;
                    if (metrics) softInserts++;
                }
                else if (index < rc - 2 && items[index + 2] == null)
                {
                    items[index + 2] = items[index + 1];
                    items[index + 1] = items[index];
                    items[index] = item;

                    if (metrics) softInserts++;
                }
                else
                {
                    if ((index & 1) == 0)
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
                    if (metrics) hardInserts++;
                }

                if (metrics)
                {
                    var ins = softInserts + hardInserts;
                    averageInsertIndex = ((averageInsertIndex * (ins - 1)) + index) / ins;
                }

                count++;
            }
        }

        /// <summary>
        /// Rebalance the tree locally. This usually happens after item removal, but can be performed at any time.
        /// </summary>
        /// <param name="index">The index in the tree that defines the locality.</param>
        /// <param name="strategy">The rebalance strategy to use.</param>
        /// <param name="globalRebalanceOperation">True to indicate this function is being called by <see cref="TryRebalance"/>.</param>
        /// <returns></returns>
        protected bool LocalRebalance(int index, RebalanceStrategy strategy, bool globalRebalanceOperation)
        {
            lock (syncRoot)
            {
                if (strategy == RebalanceStrategy.Cadence16)
                {
                    if ((index & 1) == 1) index--;

                    if (index + 8 > items.Count) return false;
                    if (index - 8 < 0) return false;

                    index -= 8;

                    if (
                        items[index] is object && !(items[index + 1] is object)
                        && items[index + 2] is object && !(items[index + 3] is object)
                        && items[index + 4] is object && !(items[index + 5] is object)
                        && items[index + 6] is object && !(items[index + 7] is object)
                        && items[index + 8] is object && !(items[index + 9] is object)
                        && items[index + 10] is object && !(items[index + 11] is object)
                        && items[index + 12] is object && !(items[index + 13] is object)
                        && items[index + 14] is object && !(items[index + 15] is object)
                        )
                    {
                        items[index + 1] = items[index + 2];
                        items[index + 2] = items[index + 4];
                        items[index + 3] = items[index + 6];
                        items[index + 4] = items[index + 8];
                        items[index + 5] = items[index + 10];
                        items[index + 6] = items[index + 12];
                        items[index + 7] = items[index + 14];

                        items.RemoveRange(index + 8, 8);

                        if (metrics && !globalRebalanceOperation)
                        {
                            softRemoves--;
                            localRebalances++;
                            hardRemoves++;
                        }

                        return true;
                    }
                }
                else if (strategy == RebalanceStrategy.Cadence8)
                {
                    if ((index & 1) == 1) index--;

                    if (index + 4 > items.Count) return false;
                    if (index - 4 < 0) return false;

                    index -= 4;

                    if (
                        items[index] is object && !(items[index + 1] is object)
                        && items[index + 2] is object && !(items[index + 3] is object)
                        && items[index + 4] is object && !(items[index + 5] is object)
                        && items[index + 6] is object && !(items[index + 7] is object)
                        )
                    {
                        items[index + 1] = items[index + 2];
                        items[index + 2] = items[index + 4];
                        items[index + 3] = items[index + 6];

                        items.RemoveRange(index + 4, 4);

                        if (metrics && !globalRebalanceOperation)
                        {
                            softRemoves--;
                            localRebalances++;
                            hardRemoves++;
                        }

                        return true;
                    }

                }
                else if (strategy == RebalanceStrategy.Cadance4)
                {
                    if ((index & 1) == 1) index--;

                    if (index + 2 > items.Count) return false;
                    if (index - 2 < 0) return false;

                    index -= 2;

                    if (
                        items[index] is object && !(items[index + 1] is object)
                        && items[index + 2] is object && !(items[index + 3] is object)
                        )
                    {
                        items[index + 1] = items[index + 2];
                        items.RemoveRange(index + 2, 2);

                        if (metrics && !globalRebalanceOperation)
                        {
                            softRemoves--;
                            localRebalances++;
                            hardRemoves++;
                        }

                        return true;
                    }
                }

                return false;

            }
        }

        /// <summary>
        /// Returns true if the item exists in the collection, and provides the current index for that item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <param name="index">The current index in the tree.</param>
        /// <returns>True if the item exists.</returns>
        protected virtual bool Locate(T item, out int index)
        {
            index = Walk(item, TreeWalkMode.Locate);
            return index != -1;
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
                count--;

                if ((index & 1) == 0)
                {
                    if (items[index + 1] is object)
                    {
                        items[index] = items[index + 1];
                        items[index + 1] = default;

                        if (metrics) softRemoves++;
                    }
                    else if (index < items.Count - 3 && items[index + 2] is object && items[index + 3] is object)
                    {
                        items[index] = items[index + 2];
                        items[index + 2] = items[index + 3];
                        items[index + 3] = default;

                        if (metrics) softRemoves++;
                    }
                    else
                    {
                        items.RemoveRange(index, 2);
                        if (metrics) hardRemoves++;
                    }
                }
                else
                {
                    if (metrics) softRemoves++;

                    if (TryRebalance() == RebalanceResult.NotPerformed)
                    {
                        LocalRebalance(index, localStrategy, false);
                    }
                }
            }
        }

        /// <summary>
        /// Walk the tree and look for the appropriate index for the specified item.
        /// </summary>
        /// <param name="item1">The item to look for.</param>
        /// <param name="walkMode">The type of walk (either for insert or locate)</param>
        /// <returns>The index where the item is or should be.</returns>
        protected virtual int Walk(T item1, TreeWalkMode walkMode = TreeWalkMode.InsertIndex)
        {
            int count = items.Count;
            int lo = 0;
            int hi = count - 1;
            int mid = 0;

            T item2, item3;
            int r;

            while (true)
            {
                if (hi < lo)
                {
                    if (walkMode == TreeWalkMode.InsertIndex && (lo & 1) == 0)
                    {
                        if (lo < count - 1 && !(items[lo + 1] is object))
                        {
                            r = comp(item1, items[lo]);
                            if (r >= 0) lo++;
                        }

                        else if (lo > 0 && !(items[lo - 1] is object))
                        {
                            if (lo < count)
                            {
                                r = comp(item1, items[lo]);
                                if (r <= 0) lo--;
                            }
                            else
                            {
                                lo--;
                            }
                        }
                    }
                    else
                    {
                        if (lo < 0 || lo >= count) return -1;
                        else if (!(items[lo] is object)) return -1;
                        else if (!Equals(item1, items[lo])) return -1;
                    }

                    return lo;
                }

                mid = (hi + lo) / 2;

                if (((mid & 1)) == 1) mid--;

                item2 = items[mid];
                item3 = items[mid + 1];

                r = comp(item1, item2);

                if (r > 0)
                {
                    if (item3 is object)
                    {
                        r = comp(item1, item3);

                        if (r <= 0)
                        {
                            return mid + 1;
                        }
                    }

                    lo = mid + 2;
                }
                else if (r < 0)
                {
                    hi = mid - 2;
                }
                else
                {
                    lo = mid;
                    hi = lo - 2;
                }
            }

        }

        #endregion Protected Methods

    }

    /// <summary>
    /// An exception that is thrown when a black node in a red/black tree is null or empty.
    /// </summary>
    /// <remarks>
    /// This is only used for debugging. In production, if the tree cannot be walked in every case, it should not be in production.<br /><br />
    /// This exception indicates an untenable bug.
    /// </remarks>
    public class TreeUnbalancedException : Exception
    {
        #region Public Constructors

        public TreeUnbalancedException() : base()
        {
        }

        public TreeUnbalancedException(string message) : base(message)
        {
        }

        #endregion Public Constructors

    }
}
