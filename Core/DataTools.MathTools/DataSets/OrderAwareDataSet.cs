using System;

namespace DataTools.MathTools
{
    public class OrderAwareDataSet<T> : DataSet<T> where T : class, IComparable<T>
    {
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