﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataTools.Essentials.SortedLists
{
    /// <summary>
    /// Implementation of quick sort.
    /// </summary>
    public static class QuickSort
    {
        /// <summary>
        /// Sort an array of class objects by the property specified by propertyName that implements <see cref="IComparable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <typeparam name="U">The type of the property to sort on.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        /// <param name="propertyName">The name of the property to sort on.</param>
        public static void Sort<T, U>(ref T[] values, string propertyName) where T : class where U : IComparable<U>
        {
            if (values == null || values.Length == 0) return;

            PropertyInfo prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop == null) throw new ArgumentException(nameof(propertyName));

            var dynComp = new Func<T, T, int>((a, b) =>
            {
                U vA = (U)prop.GetValue(a);
                U vB = (U)prop.GetValue(b);

                return vA.CompareTo(vB);
            });

            int lo = 0;
            int hi = values.Length - 1;

            Sort(ref values, dynComp, lo, hi);
        }

        /// <summary>
        /// Sort an array of objects that implement <see cref="IComparable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        public static void Sort<T>(ref T[] values) where T : IComparable<T>
        {
            if (values == null || values.Length == 0) return;

            var comp = new Func<T, T, int>((a, b) =>
            {
                return a.CompareTo(b);
            });

            int lo = 0;
            int hi = values.Length - 1;

            Sort(ref values, comp, lo, hi);
        }

        /// <summary>
        /// Sort an array of class objects on the specified property using the specified comparer.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <typeparam name="U">The type of the property to sort on.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        /// <param name="propertyName">The name of the property to sort on.</param>
        /// <param name="comparer">The comparer to use.</param>
        public static void Sort<T, U>(ref T[] values, string propertyName, IComparer<U> comparer) where T : class
        {
            Sort<T, U>(ref values, propertyName, comparer.Compare);
        }

        /// <summary>
        /// Sort an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        /// <param name="comparer">The comparer to use.</param>
        public static void Sort<T>(ref T[] values, IComparer<T> comparer)
        {
            if (values == null || values.Length == 0) return;

            int lo = 0;
            int hi = values.Length - 1;

            Sort(ref values, comparer.Compare, lo, hi);
        }

        /// <summary>
        /// Sort an array of class objects on the specified property using the specified comparison.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <typeparam name="U">The type of the property to sort on.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        /// <param name="propertyName">The name of the property to sort on.</param>
        /// <param name="comparison">The comparison function to use.</param>
        public static void Sort<T, U>(ref T[] values, string propertyName, Comparison<U> comparison) where T : class
        {
            if (values == null || values.Length == 0) return;

            int lo = 0;
            int hi = values.Length - 1;

            PropertyInfo prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop == null) throw new ArgumentException(nameof(propertyName));

            var dynComp = new Func<T, T, int>((a, b) =>
            {
                U vA = (U)prop.GetValue(a);
                U vB = (U)prop.GetValue(b);

                return comparison(vA, vB);
            });

            Sort(ref values, dynComp, lo, hi);
        }

        /// <summary>
        /// Sort an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        /// <param name="comparison">The comparison function to use.</param>
        public static void Sort<T>(ref T[] values, Func<T, T, int> comparison)
        {
            if (values == null || values.Length == 0) return;

            int lo = 0;
            int hi = values.Length - 1;

            Sort(ref values, comparison, lo, hi);
        }

        private static void Sort<T>(ref T[] values, Func<T, T, int> comparison, int lo, int hi)
        {
            if (lo < hi)
            {
                int p = Partition(ref values, comparison, lo, hi);

                Sort(ref values, comparison, lo, p);
                Sort(ref values, comparison, p + 1, hi);
            }
        }

        private static int Partition<T>(ref T[] values, Func<T, T, int> comparison, int lo, int hi)
        {
            var ppt = (hi + lo) / 2;
            var pivot = values[ppt];

            int i = lo - 1;
            int j = hi + 1;

            while (true)
            {
                do
                {
                    ++i;
                } while (i <= hi && comparison(values[i], pivot) < 0);
                do
                {
                    --j;
                } while (j >= 0 && comparison(values[j], pivot) > 0);
                if (i >= j) return j;

                T sw = values[i];

                values[i] = values[j];
                values[j] = sw;
            }
        }

        #region IList sorting

        /// <summary>
        /// Sort an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        /// <param name="comparison">The comparison function to use.</param>
        /// <param name="sortOrder">Specify the sort order.</param>
        public static void Sort<T>(IList<T> values, Func<T, T, int> comparison, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort(values, comparison, sortOrder, out _);
        }

        /// <summary>
        /// Sort an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to sort.</typeparam>
        /// <param name="values">The array of values to sort.</param>
        /// <param name="comparison">The comparison function to use.</param>
        /// <param name="sortOrder">Specify the sort order.</param>
        /// <param name="didChange">True if the collection did change.</param>
        public static void Sort<T>(IList<T> values, Func<T, T, int> comparison, SortOrder sortOrder, out bool didChange)
        {
            if (values == null || values.Count == 0)
            {
                didChange = false;
                return;
            }

            int lo = 0;
            int hi = values.Count - 1;

            Sort(values, comparison, sortOrder, lo, hi, out didChange);
        }

        private static void Sort<T>(IList<T> values, Func<T, T, int> comparison, SortOrder sortOrder, int lo, int hi, out bool didChange)
        {
            bool dcf = false;
            bool dc = false;

            if (lo < hi)
            {
                int p = Partition(values, comparison, sortOrder, lo, hi, out dc);
                dcf |= dc;

                Sort(values, comparison, sortOrder, lo, p, out dc);
                dcf |= dc;

                Sort(values, comparison, sortOrder, p + 1, hi, out dc);
                dcf |= dc;
            }

            didChange = dcf;
        }

        private static int Partition<T>(IList<T> values, Func<T, T, int> comparison, SortOrder sortOrder, int lo, int hi, out bool didChange)
        {
            var ppt = (hi + lo) / 2;
            var pivot = values[ppt];

            int i = lo - 1;
            int j = hi + 1;

            bool dc = false;

            if (sortOrder == SortOrder.Ascending)
            {
                while (true)
                {
                    do
                    {
                        ++i;
                    } while (i <= hi && comparison(values[i], pivot) < 0);
                    do
                    {
                        --j;
                    } while (j >= 0 && comparison(values[j], pivot) > 0);

                    if (i >= j)
                    {
                        didChange = dc;
                        return j;
                    }

                    T sw = values[i];

                    values[i] = values[j];
                    values[j] = sw;

                    dc = true;
                }
            }
            else
            {
                while (true)
                {
                    do
                    {
                        ++i;
                    } while (i <= hi && comparison(values[i], pivot) > 0);
                    do
                    {
                        --j;
                    } while (j >= 0 && comparison(values[j], pivot) < 0);

                    if (i >= j)
                    {
                        didChange = dc;
                        return j;
                    }

                    T sw = values[i];

                    values[i] = values[j];
                    values[j] = sw;

                    dc = true;
                }
            }
        }

        #endregion IList sorting
    }
}