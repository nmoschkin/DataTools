using System;
using System.Collections.Generic;

namespace DataTools.MathTools
{
    /// <summary>
    /// Represents a collection of items upon which logical operations can be performed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataSet<T> : List<T>, IEquatable<DataSet<T>>
    {
        /// <summary>
        /// Create a new data set
        /// </summary>
        public DataSet() : base()
        {
        }

        /// <summary>
        /// Create a new data set
        /// </summary>
        public DataSet(IEnumerable<T> items) : base(items)
        {
        }

        /// <summary>
        /// Create a new data set
        /// </summary>
        public DataSet(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Returns all the keys that are mutual between set 1 and set 2 (logical AND.)
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static DataSet<T> operator &(DataSet<T> set1, DataSet<T> set2)
        {
            int i, c;

            DataSet<T> result = new DataSet<T>();

            c = set1.Count;

            for (i = 0; i < c; i++)
            {
                if (result.Contains(set1[i])) continue;

                if (set2.Contains(set1[i]))
                {
                    result.Add(set1[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all the keys that are not mutual to set 1 and set 2 (logical XOR.)
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static DataSet<T> operator ^(DataSet<T> set1, DataSet<T> set2)
        {
            int i, c;

            DataSet<T> result = new DataSet<T>();

            c = set1.Count;

            for (i = 0; i < c; i++)
            {
                if (result.Contains(set1[i])) continue;

                if (!set2.Contains(set1[i]))
                {
                    result.Add(set1[i]);
                }
            }

            c = set2.Count;

            for (i = 0; i < c; i++)
            {
                if (result.Contains(set2[i])) continue;

                if (!set1.Contains(set2[i]))
                {
                    result.Add(set2[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a set containing all unique keys present in both set 1 and set 2 (logical OR.)
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static DataSet<T> operator |(DataSet<T> set1, DataSet<T> set2)
        {
            int i, c;

            DataSet<T> result = new DataSet<T>();

            c = set1.Count;

            for (i = 0; i < c; i++)
            {
                if (result.Contains(set1[i])) continue;
                result.Add(set1[i]);
            }

            c = set2.Count;

            for (i = 0; i < c; i++)
            {
                if (result.Contains(set2[i])) continue;
                result.Add(set2[i]);
            }

            return result;
        }

        /// <summary>
        /// Returns a set containing all keys in set 1 that do not exist in set 2 (Logical remainder.)
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static DataSet<T> operator %(DataSet<T> set1, DataSet<T> set2)
        {
            int i, c;

            DataSet<T> result = new DataSet<T>();

            c = set1.Count;

            for (i = 0; i < c; i++)
            {
                if (result.Contains(set1[i])) continue;

                if (!set2.Contains(set1[i]))
                {
                    result.Add(set1[i]);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public static bool operator ==(DataSet<T> set1, DataSet<T> set2)
        {
            if (set1 is object)
            {
                return set1.Equals(set2);
            }
            else if (set2 is object)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public static bool operator !=(DataSet<T> set1, DataSet<T> set2)
        {
            if (set1 is object)
            {
                return !set1.Equals(set2);
            }
            else if (set2 is object)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is DataSet<T> d)
            {
                return Equals(d);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Equals(DataSet<T> set2)
        {
            var set1 = this;

            if (set1 != null && set2 != null)
            {
                if (set1.Count != set2.Count) return false;

                int i, c;
                c = set1.Count;

                for (i = 0; i < c; i++)
                {
                    if (set1[i] is object)
                    {
                        if (!set1[i].Equals(set2[i])) return false;
                    }
                    else if (set2[i] is object)
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (set1 != null || set2 != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}