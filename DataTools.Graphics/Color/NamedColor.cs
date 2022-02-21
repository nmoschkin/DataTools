﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataTools.Text;

using static DataTools.SortedLists.BinarySearch;
using static DataTools.SortedLists.QuickSort;

namespace DataTools.Graphics
{

    public class NamedColor
    {
        private static NamedColor[] catalog;

        private UniColor color;

        private string name;

        private string extraInfo;

        private string idxstr;
        private string nidxstr;
        private string eidxstr;

        public static NamedColor FindColor(UniColor value, out int index)
        {
            NamedColor r;
            index = Search(catalog, value, nameof(Color), out r, true);

            return r;
        }

        public static NamedColor FindColor(UniColor value, bool closest = false)
        {
            var r = FindColor(value, out _);

            if (r == null && closest)
            {
                r = GetClosestColor(value);
            }

            return r;
        }

        public static NamedColor GetClosestColor(UniColor value, double maxDeviation = 0.013d, bool ignoreValue = true, bool ignoreSaturation = false, bool ignoreHue = false)
        {
            var hsv1 = ColorMath.ColorToHSV(value);

            NamedColor closest = null;

            double lhue = 360;
            double lsat = 1;
            double lval = 1;

            double dhue, dsat, dval;
            bool match;

            var mxd = Math.Abs(maxDeviation);

            foreach (var vl in catalog)
            {
                match = true;

                var hsv2 = ColorMath.ColorToHSV(vl.Color);
                if (hsv1.Hue == -1 && hsv2.Hue >= 0) continue;
                if (hsv2.Hue == -1 && hsv1.Hue >= 0) continue;

                dhue = Math.Abs(hsv2.Hue - hsv1.Hue);
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
                    lhue = Math.Abs(hsv2.Hue - hsv1.Hue);
                    lsat = Math.Abs(hsv2.Saturation - hsv1.Saturation);
                    lval = Math.Abs(hsv2.Value - hsv1.Value);

                    closest = vl;
                }
            }

            return closest;
        }
        public static List<NamedColor> SearchAll(string search)
        {
            List<NamedColor> output = new List<NamedColor>();
            search = TextTools.NoSpace(search.ToLower());

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

        public static List<NamedColor> SearchByName(string search, bool anywhere = false)
        {
            List<NamedColor> output = new List<NamedColor>();
            search = TextTools.NoSpace(search.ToLower());

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

        public static List<NamedColor> SearchByExtra(string search, bool anywhere = false)
        {
            List<NamedColor> output = new List<NamedColor>();
            search = search.ToLower();

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

        public static IReadOnlyList<NamedColor> Catalog
        {
            get
            {
                return catalog;
            }
        }

        static NamedColor()
        {
            if (catalog == null) LoadColors();
        }

        public static void LoadColors()
        {
            if (catalog != null) return;

            var cl = new List<NamedColor>();
            var craw = AppResources.ColorList.Replace("\r\n", "\n").Split("\n");

            foreach (var cen in craw)
            {
                if (string.IsNullOrEmpty(cen.Trim())) continue;
                var et = cen.Split("|");
                UniColor cr = uint.Parse("ff" + et[0], System.Globalization.NumberStyles.HexNumber);

                string text = TextTools.TitleCase(et[1], false).Replace("'S", "'s");
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

            catalog = cl.ToArray();
            Sort(ref catalog, (a, b) => a.Color.CompareTo(b.Color));
        }

        public UniColor Color
        {
            get => color;
            private set
            {
                color = value;
            }
        }

        public string Name
        {
            get => name;
            private set
            {
                name = value;
            }
        }

        public string ExtraInfo
        {
            get => extraInfo;
            set
            {
                extraInfo = value;
            }
        }

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

        public NamedColor(string name, UniColor color, string extra = null)
        {
            Name = name;
            Color = color;
            ExtraInfo = extra;
        }

        public static implicit operator System.Drawing.Color(NamedColor c)
        {
            return c.Color;
        }

        public static implicit operator UniColor(NamedColor c)
        {
            return c.Color;
        }

    }
}
