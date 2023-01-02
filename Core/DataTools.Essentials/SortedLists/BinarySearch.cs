using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataTools.Essentials.SortedLists
{
    /// <summary>
    /// Implementation of binary search.
    /// </summary>
    public static class BinarySearch
    {
        #region Public Methods

        /// <summary>
        /// Perform an assisted binary walk to look for something.
        /// </summary>
        /// <typeparam name="TList">The type of list to search.</typeparam>
        /// <typeparam name="TItem">The type of item to search.</typeparam>
        /// <param name="compare">A function that will take the right-hand object as a parameter and return a comparison integer.</param>
        /// <param name="source">The source list.</param>
        /// <param name="retobj">The optional return object.</param>
        /// <param name="first">True to return the index of the first match in a list with duplicate keys.</param>
        /// <param name="insertIndex">True to return an insert index instead of -1.<br />
        /// If set to true, match success must be tested by checking if the <paramref name="retobj"/> parameter contains an object or is default.</param>
        /// <returns></returns>
        /// <remarks>
        /// The assumption is made that the caller already has the criteria they are looking for.<br />
        /// They can implement a lambda or method to compare their data to the object passed<br /> into the lambda.
        /// <br /><br />
        /// In this use case, the method will be called with the right-hand parameter.<br />
        /// The data the caller has would be treated as the left-hand parameter.
        /// <br /><br />
        /// If <paramref name="insertIndex"/> is true, an insert index will be returned instead of -1, and match<br />
        /// success must be tested by checking if the <paramref name="retobj"/> parameter contains an object<br />
        /// or is default.
        /// <br /><br />
        /// If <typeparamref name="TItem"/> is a structure or value type, consider using <see cref="Nullable{TItem}"/>, instead.
        /// </remarks>
        public static int Search<TList, TItem>(
           Func<TItem, int> compare,
           TList source,
           out TItem retobj,
           bool first = false,
           bool insertIndex = false)
           where TList : IList<TItem>
        {
            if (source == null || source.Count == 0)
            {
                retobj = default;
                return insertIndex ? 0 : -1;
            }

            int lo = 0, hi = source.Count - 1;

            TItem comp;

            while (true)
            {
                if (lo > hi) break;

                int p = (hi + lo) / 2;

                comp = source[p];

                int c = compare(comp);
                if (c == 0)
                {
                    if (first && p > 0)
                    {
                        p--;

                        do
                        {
                            comp = source[p];

                            c = compare(comp);

                            if (c != 0)
                            {
                                break;
                            }

                            p--;
                        } while (p >= 0);

                        ++p;
                        comp = source[p];
                    }

                    retobj = comp;
                    return p;
                }
                else if (c < 0)
                {
                    hi = p - 1;
                }
                else
                {
                    lo = p + 1;
                }
            }

            retobj = default;
            return insertIndex ? lo : -1;
        }

        /// <summary>
        /// Sorts an array and find an object in the specified sorted array of objects that implement <see cref="IComparable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to sort and search.</typeparam>
        /// <param name="values">The array of values to sort and search.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        /// <remarks>
        /// T must implement <see cref="IComparable{T}"/>.
        /// </remarks>
        public static int Search<T>(ref T[] values, T value, bool first = true) where T : IComparable<T>
        {
            QuickSort.Sort(ref values);
            return Search(values, value, first);
        }

        /// <summary>
        /// Find an object in the specified sorted array of objects that implement <see cref="IComparable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to search.</typeparam>
        /// <param name="values">The array of values to search.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        /// <remarks>
        /// T must implement <see cref="IComparable{T}"/>.
        /// </remarks>
        public static int Search<T>(T[] values, T value, bool first = true) where T : IComparable<T>
        {
            var comp = new Func<T, T, int>((a, b) =>
            {
                return a.CompareTo(b);
            });

            return Search(values, value, comp, first);
        }

        /// <summary>
        /// Sort an arra and find an object in the specified sorted array.
        /// </summary>
        /// <typeparam name="T">The type of the object to search.</typeparam>
        /// <param name="values">The array of values to sort and search.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        public static int Search<T>(ref T[] values, T value, IComparer<T> comparer, bool first = true)
        {
            return Search(ref values, value, comparer.Compare, first);
        }

        /// <summary>
        /// Find an object in the specified sorted array.
        /// </summary>
        /// <typeparam name="T">The type of the object to search.</typeparam>
        /// <param name="values">The array of values to search.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        public static int Search<T>(T[] values, T value, IComparer<T> comparer, bool first = true)
        {
            return Search(values, value, comparer.Compare, first);
        }

        /// <summary>
        /// Sort an array and find an object in the specified sorted array.
        /// </summary>
        /// <typeparam name="T">The type of the object to search.</typeparam>
        /// <param name="values">The array of values to sort and search.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparison">The comparison function to use.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        public static int Search<T>(ref T[] values, T value, Func<T, T, int> comparison, bool first = true)
        {
            QuickSort.Sort(ref values, comparison);
            return Search(values, value, comparison, first);
        }

        /// <summary>
        /// Find an object in the specified sorted array.
        /// </summary>
        /// <typeparam name="T">The type of the object to search.</typeparam>
        /// <param name="values">The array of values to search.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparison">The comparison function to use.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        public static int Search<T>(T[] values, T value, Func<T, T, int> comparison, bool first = true)
        {
            int lo = 0, hi = values.Length - 1;

            while (true)
            {
                if (lo > hi) break;

                int p = (hi + lo) / 2;
                T elem = values[p];

                int c = comparison(value, values[p]);
                if (c == 0)
                {
                    if (first && p > 0)
                    {
                        p--;

                        do
                        {
                            c = comparison(value, values[p]);

                            if (c != 0)
                            {
                                break;
                            }
                        } while (--p >= 0);

                        ++p;
                    }

                    return p;
                }
                else if (c < 0)
                {
                    hi = p - 1;
                }
                else
                {
                    lo = p + 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Find an object in the specified sorted array of objects that implement <see cref="IComparable{U}"/>.
        /// </summary>
        /// <typeparam name="T">The type of class object to search.</typeparam>
        /// <typeparam name="U">The type of the property to search.</typeparam>
        /// <param name="values">The array of values to search.</param>
        /// <param name="value">The value of the specified property to find.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="propertyName">The name of the property to search.</param>
        /// <param name="retobj">Contains the object found, or null if not found.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        /// <remarks>
        /// T must be a class type.
        /// propertyName must specify an instance property.
        /// </remarks>
        public static int Search<T, U>(T[] values, U value, IComparer<U> comparer, string propertyName, out T retobj, bool first = true) where T : class
        {
            return Search(values, value, comparer.Compare, propertyName, out retobj, first);
        }

        /// <summary>
        /// Sort an array and find an object in the specified sorted array of objects with a property whose type implements <see cref="IComparable{U}"/>.
        /// </summary>
        /// <typeparam name="T">The type of class object to search.</typeparam>
        /// <typeparam name="U">The type of the property to search.</typeparam>
        /// <param name="values">The array of values to sort and search.</param>
        /// <param name="value">The value of the specified property to find.</param>
        /// <param name="propertyName">The name of the property to search.</param>
        /// <param name="retobj">Contains the object found, or null if not found.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        /// <remarks>
        /// T must be a class type.
        /// U must implement <see cref="IComparable{U}"/>.
        /// propertyName must specify an instance property.
        /// </remarks>
        public static int Search<T, U>(ref T[] values, U value, string propertyName, out T retobj, bool first = true) where T : class where U : IComparable<U>
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop == null) throw new ArgumentException(nameof(propertyName));

            var dynComp = new Func<T, T, int>((a, b) =>
            {
                U vA = (U)prop.GetValue(a);
                U vB = (U)prop.GetValue(b);

                return vA.CompareTo(vB);
            });

            QuickSort.Sort(ref values, dynComp);
            return Search(values, value, propertyName, out retobj, first);
        }

        /// <summary>
        /// Find an object in the specified sorted array of objects with a property whose type implements <see cref="IComparable{U}"/>.
        /// </summary>
        /// <typeparam name="T">The type of class object to search.</typeparam>
        /// <typeparam name="U">The type of the property to search.</typeparam>
        /// <param name="values">The array of values to search.</param>
        /// <param name="value">The value of the specified property to find.</param>
        /// <param name="propertyName">The name of the property to search.</param>
        /// <param name="retobj">Contains the object found, or null if not found.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        /// <remarks>
        /// T must be a class type.
        /// U must implement <see cref="IComparable{U}"/>.
        /// propertyName must specify an instance property.
        /// </remarks>
        public static int Search<T, U>(T[] values, U value, string propertyName, out T retobj, bool first = true) where T : class where U : IComparable<U>
        {
            var comp = new Comparison<U>((a, b) =>
            {
                return a.CompareTo(b);
            });

            return Search(values, value, comp, propertyName, out retobj, first);
        }

        /// <summary>
        /// Sort an array and find an object in the specified sorted array.
        /// </summary>
        /// <typeparam name="T">The type of class object to search.</typeparam>
        /// <typeparam name="U">The type of the property to search.</typeparam>
        /// <param name="values">The array of values to sort and search.</param>
        /// <param name="value">The value of the specified property to find.</param>
        /// <param name="comparison">The comparison function to use.</param>
        /// <param name="propertyName">The name of the property to search.</param>
        /// <param name="retobj">Contains the object found, or null if not found.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        /// <remarks>
        /// T must be a class type.
        /// propertyName must specify an instance property.
        /// </remarks>
        public static int Search<T, U>(ref T[] values, U value, Comparison<U> comparison, string propertyName, out T retobj, bool first = true) where T : class
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop == null) throw new ArgumentException(nameof(propertyName));

            var dynComp = new Func<T, T, int>((a, b) =>
            {
                U vA = (U)prop.GetValue(a);
                U vB = (U)prop.GetValue(b);

                return comparison(vA, vB);
            });

            QuickSort.Sort(ref values, dynComp);
            return Search(values, value, comparison, propertyName, out retobj, first);
        }

        /// <summary>
        /// Find an object in the specified sorted array.
        /// </summary>
        /// <typeparam name="T">The type of class object to search.</typeparam>
        /// <typeparam name="U">The type of the property to search.</typeparam>
        /// <param name="values">The array of values to search.</param>
        /// <param name="value">The value of the specified property to find.</param>
        /// <param name="comparison">The comparison function to use.</param>
        /// <param name="propertyName">The name of the property to search.</param>
        /// <param name="retobj">Contains the object found, or null if not found.</param>
        /// <param name="first">Set true to return the index of the first occurrence of value, otherwise, the first found index will be returned.</param>
        /// <returns>The index to the specified element, or -1 if not found.</returns>
        /// <remarks>
        /// T must be a class type.
        /// propertyName must specify an instance property.
        /// </remarks>
        public static int Search<T, U>(T[] values, U value, Comparison<U> comparison, string propertyName, out T retobj, bool first = true) where T : class
        {
            if (values == null || values.Length == 0)
            {
                retobj = null;
                return -1;
            }

            int lo = 0, hi = values.Length - 1;
            PropertyInfo prop = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop == null) throw new ArgumentException(nameof(propertyName));

            U comp;

            while (true)
            {
                if (lo > hi) break;

                int p = (hi + lo) / 2;

                T elem = values[p];

                comp = (U)prop.GetValue(elem);

                int c = comparison(value, comp);
                if (c == 0)
                {
                    if (first && p > 0)
                    {
                        p--;

                        do
                        {
                            elem = values[p];
                            comp = (U)prop.GetValue(elem);

                            c = comparison(value, comp);

                            if (c != 0)
                            {
                                break;
                            }
                        } while (--p >= 0);

                        ++p;
                        elem = values[p];
                    }

                    retobj = elem;
                    return p;
                }
                else if (c < 0)
                {
                    hi = p - 1;
                }
                else
                {
                    lo = p + 1;
                }
            }

            retobj = null;
            return -1;
        }

        #endregion Public Methods
    }
}