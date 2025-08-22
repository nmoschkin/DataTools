using DataTools.Essentials.Helpers;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Encapsulates a collection of <see cref="HidUsageInfo"/> objects.
    /// </summary>
    public class HidUsageCollection : HidUsageInfo, IList<HidUsageInfo>
    {
        private List<HidUsageInfo> usages = new List<HidUsageInfo>();

        /// <inheritdoc/>
        public int Count => ((ICollection<HidUsageInfo>)usages).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<HidUsageInfo>)usages).IsReadOnly;

        /// <inheritdoc/>
        public HidUsageInfo this[int index] { get => ((IList<HidUsageInfo>)usages)[index]; set => ((IList<HidUsageInfo>)usages)[index] = value; }

        /// <inheritdoc/>
        public int IndexOf(HidUsageInfo item)
        {
            return ((IList<HidUsageInfo>)usages).IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, HidUsageInfo item)
        {
            ((IList<HidUsageInfo>)usages).Insert(index, item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            ((IList<HidUsageInfo>)usages).RemoveAt(index);
        }

        /// <inheritdoc/>
        public void Add(HidUsageInfo item)
        {
            item.Parent = this;
            ((ICollection<HidUsageInfo>)usages).Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ((ICollection<HidUsageInfo>)usages).Clear();
        }

        /// <inheritdoc/>
        public bool Contains(HidUsageInfo item)
        {
            return ((ICollection<HidUsageInfo>)usages).Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(HidUsageInfo[] array, int arrayIndex)
        {
            ((ICollection<HidUsageInfo>)usages).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(HidUsageInfo item)
        {
            return ((ICollection<HidUsageInfo>)usages).Remove(item);
        }

        /// <inheritdoc/>
        public IEnumerator<HidUsageInfo> GetEnumerator()
        {
            return ((IEnumerable<HidUsageInfo>)usages).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)usages).GetEnumerator();
        }

        /// <summary>
        /// Instantiate a new usage collection for the specified usage type.
        /// </summary>
        /// <param name="collectionType"></param>
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

        /// <summary>
        /// Instantiate a new usage collection for the specified usage type and children.
        /// </summary>
        /// <param name="collectionType"></param>
        /// <param name="items"></param>
        public HidUsageCollection(HidUsageType collectionType, IEnumerable items) : this(collectionType)
        {
            foreach (HidUsageInfo item in items)
            {
                usages.Add(item);
            }
        }

        /// <summary>
        /// Instantiate a new usage collection for the specified usage type, report type, and children.
        /// </summary>
        /// <param name="collectionType"></param>
        /// <param name="items"></param>
        public HidUsageCollection(HidUsageInfo usage, HidReportType reportType, IEnumerable items) : this(usage.UsageType, items)
        {
            var newobj = this;
            ObjectMerge.MergeObjects(usage, newobj);
            ReportType = reportType;
        }

        /// <summary>
        /// Clone into a new <see cref="HidUsageCollection"/>.
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="preserveList">Preserve the contents of the source object list (the list itself will be a new instance of <see cref="List{T}"/>)</param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public new HidUsageCollection Clone(HidReportType reportType, bool preserveList = false, HidUsageCollection parent = null)
        {
            var b = (HidUsageCollection)this.MemberwiseClone();

            b.usages = new List<HidUsageInfo>();
            b.ReportType = reportType;
            b.Parent = parent;

            if (preserveList) b.usages.AddRange(usages);

            return b;
        }
    }
}