using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb.Keyboard
{
    public class HidKeyboardUsageInfo : HidUsageInfo
    {

        [JsonProperty("boot")]
        public virtual string? Boot { get; set; }

        [JsonProperty("mac")]
        public virtual bool? Mac { get; set; }

        [JsonProperty("pcat")]
        public virtual bool? PCAT { get; set; }

        [JsonProperty("unix")]
        public virtual bool? Unix { get; set; }

        [JsonProperty("at101")]
        public virtual int? AT101 { get; set; }

    }
}
