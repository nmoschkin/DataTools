using System;

namespace DataTools.MathTools
{

    /// <summary>
    /// Represents a collection of comparable items upon which logical operations can be performed.
    /// </summary>
    /// <typeparam name="T">The type that inherits from <see cref="IComparable{T}"/></typeparam>
    public class OrderAwareDataSet<T> : DataSet<T> where T : class, IComparable<T>
    {
        /// <inheritdoc/>
        public static bool operator >(OrderAwareDataSet<T> val1, OrderAwareDataSet<T> val2)
        {
            int x = 0;

            foreach (var item1 in val1)
            {
                foreach (var item2 in val2)
                {
                    var ev = item1.CompareTo(item2);
                    x += ev;
                }
            }

            return x > 0;
        }

        /// <inheritdoc/>
        public static bool operator <(OrderAwareDataSet<T> val1, OrderAwareDataSet<T> val2)
        {
            int x = 0;

            foreach (var item1 in val1)
            {
                foreach (var item2 in val2)
                {
                    var ev = item1.CompareTo(item2);
                    x += ev;
                }
            }

            return x < 0;
        }

        /// <inheritdoc/>
        public static bool operator >=(OrderAwareDataSet<T> val1, OrderAwareDataSet<T> val2)
        {
            int x = 0;

            foreach (var item1 in val1)
            {
                foreach (var item2 in val2)
                {
                    var ev = item1.CompareTo(item2);
                    x += ev;
                }
            }

            return x >= 0;
        }

        /// <inheritdoc/>
        public static bool operator <=(OrderAwareDataSet<T> val1, OrderAwareDataSet<T> val2)
        {
            int x = 0;

            foreach (var item1 in val1)
            {
                foreach (var item2 in val2)
                {
                    var ev = item1.CompareTo(item2);
                    x += ev;
                }
            }

            return x <= 0;
        }
    }
}