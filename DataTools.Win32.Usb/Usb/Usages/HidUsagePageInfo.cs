using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public abstract class HidUsagePageInfo<T> : IReadOnlyList<T> where T : HidUsageInfo, new()
    {
        protected List<T> items = new List<T>();

        public int PageID { get; protected set; }

        protected abstract void Parse(params object[] values);

        protected HidUsagePageInfo(int pageId)
        {
            PageID = pageId;
        }

        public T this[int index] => ((IReadOnlyList<T>)items)[index];

        public int Count => ((IReadOnlyCollection<T>)items).Count;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

    }
}
