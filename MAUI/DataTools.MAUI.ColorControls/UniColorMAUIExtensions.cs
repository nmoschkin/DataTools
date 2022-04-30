using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataTools.Graphics;
using DataTools.MathTools;

namespace DataTools.MAUI.ColorControls
{
    public static class UniColorMAUIExtensions
    {
        public static UniColor GetUniColor(this Color c)
        {
            UniColor clr;
            
            var hx = c.ToHex().Replace("#", "");
            clr = uint.Parse(hx, System.Globalization.NumberStyles.HexNumber);
            return clr;
        }

        public static Color GetXamarinColor(this UniColor c)
        {

            return Color.FromArgb(c.ToString(UniColorFormatOptions.HexArgbWebFormat));
        }

    }
}
