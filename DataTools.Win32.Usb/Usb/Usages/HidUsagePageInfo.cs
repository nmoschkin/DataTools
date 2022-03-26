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

        public int PageID { get; protected set; }

        public static HidUsagePageInfo<HidUsageInfo> CreatePage(int pageId)
        {
            return new HidUsagePageInfo<HidUsageInfo>(pageId);
        }

        public static HidUsagePageInfo<TCreate> CreatePage<TCreate>(int pageId)
            where TCreate : HidUsageInfo, new()
        {
            return CreatePage<HidUsagePageInfo<TCreate>, TCreate>(pageId);
        }
    
        public static TPage CreatePage<TPage, TCreate>(int pageId)             
            where TCreate : HidUsageInfo, new()
            where TPage : HidUsagePageInfo<TCreate>
        {
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
                var result = new HidUsagePageInfo<TCreate>(pageId);
                if (result is TPage p)
                {
                    return p;
                }

            }

            return (TPage?)Activator.CreateInstance(typeof(TPage), new object[] { pageId }) ?? throw new BadImageFormatException();
        }

        protected virtual void Parse(params object[] values) 
        { 
            string? json = null;

            if (values[0] is string s)
            {
                json = s;
            }
            else if (values[0] is int pageId)
            {
                var resName = $"_{pageId:x2}";
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
        public HidUsagePageInfo(int pageId)
        {
            PageID = pageId;
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

        public HidUsagePageInfo(int pageId) : base(pageId)
        {
        }

    }
}
