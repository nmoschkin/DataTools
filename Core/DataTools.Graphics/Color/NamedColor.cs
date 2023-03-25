using DataTools.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static DataTools.Essentials.SortedLists.BinarySearch;
using static DataTools.Essentials.SortedLists.QuickSort;

namespace DataTools.Graphics
{
    /// <summary>
    /// Represents a named color that is present in one of the in-use catalogs
    /// </summary>
    public class NamedColor
    {
        private static NamedColor[] catalog;
        private static NamedColor[] webCatalog;

        private UniColor color;

        private string name;

        private string extraInfo;

        private string idxstr;
        private string nidxstr;
        private string eidxstr;

        /// <summary>
        /// The default maximum deviation for fuzzy color matching
        /// </summary>
        public static double DefaultMaxDeviation { get; set; } = 0.013;

        /// <summary>
        /// Find a named color that exactly matches the input color
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static NamedColor FindColor(UniColor value, out int index)
        {
            NamedColor r;
            index = Search(catalog, value, nameof(Color), out r, true);

            return r;
        }

        /// <summary>
        /// Find a named color that exactly or inexactly matches the input color
        /// </summary>
        /// <param name="value"></param>
        /// <param name="closest">Closest named color within the threshold of <see cref="DefaultMaxDeviation"/>.</param>
        /// <param name="useWebCatalog">True to use only the standard web-safe color catalog (and not the extended catalog)</param>
        /// <returns></returns>
        public static NamedColor FindColor(UniColor value, bool closest = false, bool useWebCatalog = true)
        {
            var r = FindColor(value, out _);

            if (r == null && closest)
            {
                r = GetClosestColor(value, useWebCatalog: useWebCatalog);
            }

            return r;
        }

        /// <summary>
        /// Find a named color that exactly or inexactly matches the input color
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxDeviation">The maximum deviation for fuzzy color matching</param>
        /// <param name="ignoreValue">True to ignore the Value component of HSV color data</param>
        /// <param name="ignoreSaturation">True to ignore the Saturation component of HSV color data</param>
        /// <param name="ignoreHue">True to ignore the Hue component of HSV color data</param>
        /// <param name="useWebCatalog">True to use only the standard web-safe color catalog (and not the extended catalog)</param>
        /// <returns>The closest matched color within the <paramref name="maxDeviation"/> threshold.</returns>
        public static NamedColor GetClosestColor(UniColor value, double? maxDeviation = null, bool ignoreValue = true, bool ignoreSaturation = false, bool ignoreHue = false, bool useWebCatalog = false)
        {
            var maxDev = maxDeviation ?? DefaultMaxDeviation;

            var hsv1 = ColorMath.ColorToHSV(value);

            NamedColor closest = null;

            double lhue = 360;
            double lsat = 1;
            double lval = 1;

            double dhue, dsat, dval;
            bool match;

            var mxd = Math.Abs(maxDev);

            var workcat = useWebCatalog ? webCatalog : catalog;

            foreach (var vl in workcat)
            {
                match = true;

                var hsv2 = ColorMath.ColorToHSV(vl.Color);
                if (hsv1.Hue.IsGrayScale && hsv2.Hue >= 0) continue;
                if (hsv2.Hue.IsGrayScale && hsv1.Hue >= 0) continue;

                dhue = hsv2.Hue - hsv1.Hue;
                dsat = Math.Abs(hsv2.Saturation - hsv1.Saturation);
                dval = Math.Abs(hsv2.Value - hsv1.Value);

                if (!ignoreHue)
                {
                    match &= dhue <= lhue && (dhue <= (360 * mxd));
                }

                if (!ignoreSaturation)
                {
                    match &= dsat <= lsat && (dsat <= mxd);
                }

                if (!ignoreValue)
                {
                    match &= dval <= lval && (dval <= mxd);
                }

                if (match)
                {
                    lhue = hsv2.Hue - hsv1.Hue;
                    lsat = Math.Abs(hsv2.Saturation - hsv1.Saturation);
                    lval = Math.Abs(hsv2.Value - hsv1.Value);

                    closest = vl;
                }
            }

            return closest;
        }

        /// <summary>
        /// Search all catalogs for the specified named color
        /// </summary>
        /// <param name="search">The text to search for in the color catalogs</param>
        /// <param name="includeWebCat">True to include the standard web-safe color catalog</param>
        /// <returns>A list of possible matches</returns>
        public static List<NamedColor> SearchAll(string search, bool includeWebCat = true)
        {
            List<NamedColor> output = new List<NamedColor>();
            search = TextTools.NoSpace(search.ToLower());

            if (includeWebCat)
            {
                foreach (var nc in webCatalog)
                {
                    var idx = nc.nidxstr + nc.eidxstr;
                    if (idx.Contains(search))
                    {
                        output.Add(nc);
                    }
                }
            }

            foreach (var nc in catalog)
            {
                var idx = nc.nidxstr + nc.eidxstr;
                if (idx.Contains(search))
                {
                    output.Add(nc);
                }
            }

            return output;
        }

        /// <summary>
        /// Search all catalogs for the specified named color
        /// </summary>
        /// <param name="search">The text to search for in the color catalogs</param>
        /// <param name="anywhere">True to search anywhere in the text of the color names (otherwise a starts-with strategy is used)</param>
        /// <param name="includeWebCat">True to include the standard web-safe color catalog</param>
        /// <returns>A list of possible matches</returns>
        public static List<NamedColor> SearchByName(string search, bool anywhere = false, bool includeWebCat = true)
        {
            List<NamedColor> output = new List<NamedColor>();
            search = TextTools.NoSpace(search.ToLower());

            if (includeWebCat)
            {
                foreach (var nc in webCatalog)
                {
                    if (anywhere)
                    {
                        if (nc.nidxstr.Contains(search))
                        {
                            if (!output.Contains(nc)) output.Add(nc);
                        }
                    }
                    else
                    {
                        if (nc.nidxstr.StartsWith(search))
                        {
                            if (!output.Contains(nc)) output.Add(nc);
                        }
                    }
                }
            }

            foreach (var nc in catalog)
            {
                if (anywhere)
                {
                    if (nc.nidxstr.Contains(search))
                    {
                        if (!output.Contains(nc)) output.Add(nc);
                    }
                }
                else
                {
                    if (nc.nidxstr.StartsWith(search))
                    {
                        if (!output.Contains(nc)) output.Add(nc);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Search all catalogs for the specified text anywhere in extra data
        /// </summary>
        /// <param name="search">The text to search for in extra data</param>
        /// <param name="anywhere">True to search anywhere in the text of the color names (otherwise a starts-with strategy is used)</param>
        /// <param name="includeWebCat">True to include the standard web-safe color catalog</param>
        /// <returns>A list of possible matches</returns>
        /// <remarks>
        /// Search extra data to limit matches to a specific color source or type
        /// </remarks>
        public static List<NamedColor> SearchByExtra(string search, bool anywhere = false, bool includeWebCat = true)
        {
            List<NamedColor> output = new List<NamedColor>();
            search = search.ToLower();

            if (includeWebCat)
            {
                foreach (var nc in webCatalog)
                {
                    if (string.IsNullOrEmpty(nc.ExtraInfo)) continue;

                    if (anywhere)
                    {
                        if (nc.eidxstr.Contains(search))
                        {
                            if (!output.Contains(nc)) output.Add(nc);
                        }
                    }
                    else
                    {
                        if (nc.eidxstr.StartsWith(search))
                        {
                            if (!output.Contains(nc)) output.Add(nc);
                        }
                    }
                }
            }

            foreach (var nc in catalog)
            {
                if (string.IsNullOrEmpty(nc.ExtraInfo)) continue;

                if (anywhere)
                {
                    if (nc.eidxstr.Contains(search))
                    {
                        if (!output.Contains(nc)) output.Add(nc);
                    }
                }
                else
                {
                    if (nc.eidxstr.StartsWith(search))
                    {
                        if (!output.Contains(nc)) output.Add(nc);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the complete catalog of colors known to this application.
        /// </summary>
        public static IReadOnlyList<NamedColor> Catalog
        {
            get
            {
                return catalog;
            }
        }

        /// <summary>
        /// Gets the standard web-safe color catalog.
        /// </summary>
        public static IReadOnlyList<NamedColor> WebCatalog
        {
            get
            {
                return webCatalog;
            }
        }

        static NamedColor()
        {
            if (catalog == null) catalog = LoadColors(AppResources.ColorList, true);
            if (webCatalog == null) webCatalog = LoadColors(AppResources.WebPalette, false);
        }

        /// <summary>
        /// Load colors from a string resoruces
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static NamedColor[] LoadColors(string resource, bool sort)
        {
            NamedColor[] newcat;

            var cl = new List<NamedColor>();
            var craw = resource.Replace("\r\n", "\n").Split("\n");

            foreach (var cen in craw)
            {
                if (string.IsNullOrEmpty(cen.Trim())) continue;
                var et = cen.Split("|");
                UniColor cr = uint.Parse("ff" + et[0], System.Globalization.NumberStyles.HexNumber);

                string text = TextTools.PascalCase(et[1], false).Replace("'S", "'s");
                string extra = null;

                int x = text.IndexOf("(");
                if (x != -1)
                {
                    et = text.Split("(");
                    text = et[0].Trim();
                    extra = et[1].Trim().Trim(')');
                }
                var cc = new NamedColor(text, cr, extra);

                cc.nidxstr = TextTools.NoSpace(text?.ToLower() ?? "");
                cc.eidxstr = TextTools.NoSpace(extra?.ToLower() ?? "");

                cc.idxstr = cc.nidxstr + cc.eidxstr;

                var test = cl.Where((a) => a.Color == cc.Color).FirstOrDefault();
                if (test == null) cl.Add(cc);
            }

            newcat = cl.ToArray();

            if (sort)
            {
                Sort(ref newcat, (a, b) => a.Color.CompareTo(b.Color));
            }

            return newcat;
        }

        /// <summary>
        /// Gets the color value of this named color
        /// </summary>
        public UniColor Color
        {
            get => color;
            private set
            {
                color = value;
            }
        }

        /// <summary>
        /// Gets the name of this color
        /// </summary>
        public string Name
        {
            get => name;
            private set
            {
                name = value;
            }
        }

        /// <summary>
        /// Gets extra info and metadata for this color
        /// </summary>
        public string ExtraInfo
        {
            get => extraInfo;
            set
            {
                extraInfo = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (extraInfo != null)
            {
                return $"{name} ({extraInfo})";
            }
            else
            {
                return name;
            }
        }

        /// <summary>
        /// Create a new named color
        /// </summary>
        /// <param name="name">The color name</param>
        /// <param name="color">The color value</param>
        /// <param name="extra">Extra data and metadata</param>
        public NamedColor(string name, UniColor color, string extra = null)
        {
            Name = name;
            Color = color;
            ExtraInfo = extra;
        }

        /// <summary>
        /// Create a new color without a name
        /// </summary>
        /// <param name="color">The color value</param>
        /// <param name="extra">Extra data and metadata</param>
        public NamedColor(UniColor color, string extra = null)
        {
            Name = color.ToString("whr");
            Color = color;
            extraInfo = extra;
        }

        /// <summary>
        /// Cast this object to a <see cref="System.Drawing.Color"/> structure
        /// </summary>
        /// <param name="c"></param>
        public static implicit operator System.Drawing.Color(NamedColor c)
        {
            return c.Color;
        }

        /// <summary>
        /// Cast this object to a <see cref="UniColor"/> structure.
        /// </summary>
        /// <param name="c"></param>
        public static implicit operator UniColor(NamedColor c)
        {
            return c.Color;
        }
    }
}