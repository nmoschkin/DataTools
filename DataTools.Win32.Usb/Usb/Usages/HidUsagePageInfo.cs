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
                    var strm = arch.Entries[0].Open();
                    byte[] buffer = new byte[strm.Length];
                    strm.Read(buffer, 0, (int)strm.Length);
                    json = Encoding.UTF8.GetString(buffer);
                    strm.Dispose();
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
