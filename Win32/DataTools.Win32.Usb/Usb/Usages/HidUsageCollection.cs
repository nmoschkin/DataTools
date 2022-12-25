using DataTools.Essentials.Helpers;

using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace DataTools.Win32.Usb
{
    public class HidUsageCollection : HidUsageInfo, IList<HidUsageInfo>
    {
        private List<HidUsageInfo> usages = new List<HidUsageInfo>();

        public int Count => ((ICollection<HidUsageInfo>)usages).Count;

        public bool IsReadOnly => ((ICollection<HidUsageInfo>)usages).IsReadOnly;

        public HidUsageInfo this[int index] { get => ((IList<HidUsageInfo>)usages)[index]; set => ((IList<HidUsageInfo>)usages)[index] = value; }

        public int IndexOf(HidUsageInfo item)
        {
            return ((IList<HidUsageInfo>)usages).IndexOf(item);
        }

        public void Insert(int index, HidUsageInfo item)
        {
            ((IList<HidUsageInfo>)usages).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<HidUsageInfo>)usages).RemoveAt(index);
        }

        public void Add(HidUsageInfo item)
        {
            item.Parent = this;
            ((ICollection<HidUsageInfo>)usages).Add(item);
        }

        public void Clear()
        {
            ((ICollection<HidUsageInfo>)usages).Clear();
        }

        public bool Contains(HidUsageInfo item)
        {
            return ((ICollection<HidUsageInfo>)usages).Contains(item);
        }

        public void CopyTo(HidUsageInfo[] array, int arrayIndex)
        {
            ((ICollection<HidUsageInfo>)usages).CopyTo(array, arrayIndex);
        }

        public bool Remove(HidUsageInfo item)
        {
            return ((ICollection<HidUsageInfo>)usages).Remove(item);
        }

        public IEnumerator<HidUsageInfo> GetEnumerator()
        {
            return ((IEnumerable<HidUsageInfo>)usages).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)usages).GetEnumerator();
        }

        public HidUsageCollection(HidUsageType collectionType)
        {
            //bool b = false;

            //b = b | ((collectionType == HidUsageType.CA));
            //b = b | ((collectionType == HidUsageType.CL));
            //b = b | ((collectionType == HidUsageType.CP));
            //b = b | (collectionType == 0);

            //if (!b) throw new ArgumentException($"{nameof(collectionType)} must be a collection usage type.");

            UsageType = collectionType;
        }

        public HidUsageCollection(HidUsageType collectionType, IEnumerable items) : this(collectionType)
        {
            foreach (HidUsageInfo item in items)
            {
                usages.Add(item);
            }
        }

        public HidUsageCollection(HidUsageInfo usage, HidReportType reportType, IEnumerable items) : this(usage.UsageType, items)
        {
            var newobj = this;
            ObjectMerge.MergeObjects(usage, newobj);
            ReportType = reportType;
        }

        /// <summary>
        /// Clone into a new <see cref="HidUsageCollection"/>.
        /// </summary>
        /// <param name="preserveList">Preserve the contents of the source object list (the list itself will be a new instance of <see cref="List{T}"/>)</param>
        /// <returns></returns>
        public override HidUsageCollection Clone(HidReportType reportType, bool preserveList = false, HidUsageCollection? parent = null)
        {
            var b = (HidUsageCollection)MemberwiseClone();

            b.usages = new List<HidUsageInfo>();
            b.ReportType = reportType;
            b.Parent = parent;

            if (preserveList) b.usages.AddRange(usages);

            return b;
        }
    }
}