using DataTools.Win32.Usb.Globalization;
using DataTools.Win32.Usb.Keyboard;
using DataTools.Win32.Usb.Power;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using static DataTools.Text.TextTools;

namespace DataTools.Win32.Usb
{
    /// <summary>
    /// Encapsulates HID usage page information for the specified <see cref="HidUsageInfo"/>-derived type
    /// </summary>
    /// <typeparam name="T">A <see cref="HidUsageInfo"/>-derived type</typeparam>
    public class HidUsagePageInfo<T> : IReadOnlyList<T> where T : HidUsageInfo, new()
    {

        /// <summary>
        /// Internal backing store for items in this object
        /// </summary>
        protected List<T> _items = new List<T>();

        /// <summary>
        /// Cached <see cref="HidUsagePage"/>-keyed objects
        /// </summary>
        protected static Dictionary<HidUsagePage, object> pageInfo = new Dictionary<HidUsagePage, object>();

        /// <summary>
        /// Gets the HID usage page configured for this instance
        /// </summary>
        public HidUsagePage PageId { get; protected set; }

        /// <summary>
        /// Create a <see cref="HidUsagePageInfo"/> instance for the specified <see cref="HidUsagePage"/>.
        /// </summary>
        /// <param name="pageId">The HID usage page to configure</param>
        /// <returns></returns>
        public static HidUsagePageInfo CreatePage(HidUsagePage pageId)
        {
            if (pageInfo.TryGetValue(pageId, out object obj) && obj is HidUsagePageInfo cached)
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

        /// <summary>
        /// Create the HID Page Catalog for the specified page.
        /// </summary>
        /// <typeparam name="TCreate">The type of Usage to create.</typeparam>
        /// <param name="pageId">The HID Page ID</param>
        /// <returns></returns>
        /// <exception cref="AccessViolationException"></exception>
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
            TPage result;

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
                if (pageInfo.TryGetValue(pageId, out object obj) && obj is TPage cached)
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

            result = (TPage)Activator.CreateInstance(typeof(TPage), new object[] { pageId }) ?? (TPage)Activator.CreateInstance(typeof(TPage));

            if (!pageInfo.ContainsKey(pageId) && result != null)
            {
                pageInfo.Add(pageId, result);
            }

            return result ?? throw new AccessViolationException($"Cannot create instance of type ({typeof(TPage)})");
        }

        /// <summary>
        /// Gets a usage by its usage name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>A <see cref="HidUsageInfo"/> or null.</returns>
        /// <remarks>
        /// Name matching is done with casing neutral and spaces are discarded.
        /// </remarks>
        public HidUsageInfo GetUsageByName(string name)
        {
            name = NoSpace(name).ToLower();
            return this.Where((x) => NoSpace(x.UsageName).ToLower() == name).FirstOrDefault();
        }

        /// <summary>
        /// Populate the instance by parsing the input parameters
        /// </summary>
        /// <param name="values"></param>
        /// <exception cref="BadImageFormatException"></exception>
        /// <remarks>
        /// The default implementation of this method reads from one of a series of en-US zip-archived data files<br />
        /// stored as embedded resources in the DLL for every HID usage page of the USB HID 4.1.1 standard.
        /// <br /><br />
        /// Alternatively, the default implementation can also read a JSON string compatible with the internal schema<br />
        /// to add support for pages not in the USB HID standard.
        /// </remarks>
        protected virtual void Parse(params object[] values)
        {
            string json = null;

            if (values[0] is string s)
            {
                json = s;
            }
            else if (values[0] is HidUsagePage pageId)
            {
                var resName = $"_{((int)pageId):x2}";
                var compdat = (byte[])AppResources.ResourceManager.GetObject(resName);

                if (compdat != null)
                {
                    using (var arch = new ZipArchive(new MemoryStream(compdat)))
                    {
                        var entry = arch.Entries[0];
                        using (var strm = entry.Open())
                        {
                            using (var mem = new MemoryStream()) 
                            {
                                strm.CopyTo(mem);

                                byte[] buffer = mem.ToArray();

                                json = Encoding.UTF8.GetString(buffer);

                                mem.Close();
                                strm.Close();
                            }
                        }
                    }
                }
            }

            if (json == null) return;

            _items = new List<T>(JsonConvert.DeserializeObject<List<T>>(json) ?? throw new BadImageFormatException());
        }

        /// <summary>
        /// Create a new HID Device Usage Page information object
        /// </summary>
        /// <param name="pageId">The HID Usage Page to configure</param>
        public HidUsagePageInfo(HidUsagePage pageId)
        {
            PageId = pageId;
            Parse(pageId);
        }

        /// <summary>
        /// Gets the <see cref="HidUsageInfo"/> object at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] => ((IReadOnlyList<T>)_items)[index];

        /// <summary>
        /// Gets the number of <see cref="HidUsageInfo"/> objects in this instance
        /// </summary>
        public int Count => ((IReadOnlyCollection<T>)_items).Count;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }

    /// <summary>
    /// Default <see cref="HidUsagePageInfo{T}"/>-derived class
    /// </summary>
    public class HidUsagePageInfo : HidUsagePageInfo<HidUsageInfo>
    {
        /// <summary>
        /// Instantiate a new <see cref="HidUsagePageInfo"/> object with the specified HID usage page
        /// </summary>
        /// <param name="pageId">The HID usage page to configure</param>
        public HidUsagePageInfo(HidUsagePage pageId) : base(pageId)
        {
        }
    }
}