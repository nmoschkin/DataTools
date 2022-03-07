using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public class HidPageInfo : IReadOnlyList<HidUsageInfo>
    {


        protected List<HidUsageInfo> _items = new List<HidUsageInfo>();

        public int PageID { get; protected set; }

        public HidUsageInfo this[int index] => ((IReadOnlyList<HidUsageInfo>)_items)[index];

        public int Count => ((IReadOnlyCollection<HidUsageInfo>)_items).Count;

        public IEnumerator<HidUsageInfo> GetEnumerator()
        {
            return ((IEnumerable<HidUsageInfo>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

    }
}
