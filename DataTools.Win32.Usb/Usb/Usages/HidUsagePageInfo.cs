using DataTools.Win32.Usb.Keyboard;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public class HidUsagePageInfo<T> : IReadOnlyList<T> where T : HidUsageInfo, new()
    {
        protected List<T> items = new List<T>();

        protected static Dictionary<HidUsagePage, object> pageInfo = new Dictionary<HidUsagePage, object>();

        public HidUsagePage PageId { get; protected set; }

        public static HidUsagePageInfo CreatePage(HidUsagePage pageId)
        {
            if (pageInfo.TryGetValue(pageId, out object? obj) && obj is HidUsagePageInfo cached)
            {
                return cached;           
            }

            var result = new HidUsagePageInfo(pageId);

            if (!pageInfo.ContainsKey(pageId))
            {
                pageInfo.Add(pageId, result);   
            }

            return result;
        }

        public static HidUsagePageInfo<TCreate> CreatePage<TCreate>(HidUsagePage pageId)
            where TCreate : HidUsageInfo, new()
        {
            return CreatePage<HidUsagePageInfo<TCreate>, TCreate>(pageId);
        }
    
        /// <summary>
        /// Create the HID Page Catalog for the specified page.
        /// </summary>
        /// <typeparam name="TPage">The type of Usage Page to create.</typeparam>
        /// <typeparam name="TCreate">The type of Usage to create.</typeparam>
        /// <param name="pageId">The HID Page ID</param>
        /// <returns></returns>
        /// <exception cref="AccessViolationException"></exception>
        public static TPage CreatePage<TPage, TCreate>(HidUsagePage pageId)             
            where TCreate : HidUsageInfo, new()
            where TPage : HidUsagePageInfo<TCreate>
        {
            TPage? result = null;

            if (typeof(TCreate) == typeof(HidKeyboardUsageInfo))
            {
                if (HidKeyboardDevicePageInfo.Instance is TPage page)
                {
                    return page;
                }
            }
            else if (typeof(TCreate) == typeof(HidPowerDevicePageInfo))
            {
                if (HidPowerDevicePageInfo.Instance is TPage page)
                {
                    return page;
                }
            }
            else if (typeof(TCreate) == typeof(HidBatteryDevicePageInfo))
            {
                if (HidBatteryDevicePageInfo.Instance is TPage page)
                {
                    return page;
                }
            }
            else
            {
                if (pageInfo.TryGetValue(pageId, out object? obj) && obj is TPage cached)
                {
                    return cached;
                }

                if (typeof(HidUsagePageInfo<TCreate>) == typeof(TPage))
                {
                    var res2 = new HidUsagePageInfo<TCreate>(pageId);

                    if (res2 is TPage p)
                    {
                        if (!pageInfo.ContainsKey(pageId))
                        {
                            pageInfo.Add(pageId, res2);
                        }

                        return p;
                    }
                }

            }

            result = (TPage?)Activator.CreateInstance(typeof(TPage), new object[] { pageId }) ?? (TPage?)Activator.CreateInstance(typeof(TPage));

            if (!pageInfo.ContainsKey(pageId) && result != null)
            {
                pageInfo.Add(pageId, result);
            }

            return result ?? throw new AccessViolationException($"Cannot create instance of type ({typeof(TPage)})");
        }

        protected virtual void Parse(params object[] values) 
        { 
            string? json = null;

            if (values[0] is string s)
            {
                json = s;
            }
            else if (values[0] is HidUsagePage pageId)
            {
                var resName = $"_{((int)pageId):x2}";
                var compdat = (byte[]?)AppResources.ResourceManager.GetObject(resName);

                if (compdat != null)
                {
                    var arch = new ZipArchive(new MemoryStream(compdat));
                    var entry = arch.Entries[0];

                    var strm = entry.Open();
                    var mem = new MemoryStream();
                    strm.CopyTo(mem);

                    byte[] buffer = mem.ToArray();

                    json = Encoding.UTF8.GetString(buffer);

                    mem.Close();
                    strm.Close();                            
                    arch.Dispose();
                }
            }

            if (json == null) return;

            items = new List<T>(JsonConvert.DeserializeObject<List<T>>(json) ?? throw new BadImageFormatException());
        }
        public HidUsagePageInfo(HidUsagePage pageId)
        {
            PageId = pageId;
            Parse(pageId);
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

    public class HidUsagePageInfo : HidUsagePageInfo<HidUsageInfo>
    {
        public HidUsagePageInfo(HidUsagePage pageId) : base(pageId)
        {
        }

    }
}
