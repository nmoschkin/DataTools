using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using static System.Math;
using System.Runtime.InteropServices;
using DataTools.MathTools;
using DataTools.Text;
using static DataTools.Text.TextTools;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace DataTools.Extras.Conversion
{
    /// <summary>
    /// General, all-purpose unit conversion class.
    /// </summary>
    /// <remarks></remarks>
    public static class ConversionTool
    {

        #region Public Fields

        public static readonly double[] Multipliers = new double[] { Pow(10d, 0d), Pow(10d, -1), Pow(10d, -2), Pow(10d, -3), Pow(10d, -6), Pow(10d, -9), Pow(10d, -12), Pow(10d, -15), Pow(10d, -18), Pow(10d, -21), Pow(10d, -24), Pow(10d, 1d), Pow(10d, 2d), Pow(10d, 3d), Pow(10d, 6d), Pow(10d, 9d), Pow(10d, 12d), Pow(10d, 15d), Pow(10d, 18d), Pow(10d, 21d), Pow(10d, 24d), Pow(2d, 10d), Pow(2d, 20d), Pow(2d, 30d), Pow(2d, 40d), Pow(2d, 50d), Pow(2d, 60d), Pow(2d, 70d), Pow(2d, 80d) };
        public static readonly string[] Prefixes = new string[] { "", "deci", "centi", "milli", "micro", "nano", "pico", "femto", "atto", "zepto", "yocto", "deca", "hecto", "kilo", "mega", "giga", "tera", "peta", "exa", "zetta", "yotta", "kibi", "mebi", "gibi", "tebi", "pebi", "exbi", "zebi", "yobi" };
        public static readonly string[] ShortPrefixes = new string[] { "", "d", "c", "m", "μ", "n", "p", "f", "a", "z", "y", "da", "h", "k", "M", "G", "T", "P", "E", "Z", "Y", "Ki", "Mi", "Gi", "Ti", "Pi", "Ei", "Zi", "Yi" };

        #endregion Public Fields

        #region Private Fields

        private static int roundingDigits = 4;

        private static UnitCollection units; 

        #endregion Private Fields

        #region Public Constructors

        static ConversionTool()
        {
            units = new UnitCollection();

        }

        #endregion Public Constructors

        #region Public Properties

        [Browsable(true)]
        public static int RoundingDigits
        {
            get
            {
                return roundingDigits;
            }
            set
            {
                roundingDigits = value;
            }
        }

        [Browsable(true)]
        public static UnitCollection Units
        {
            get
            {
                return units;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static void AddUnit(Unit unit)
        {
            units.Add(unit);
        }
        
        /// <summary>
        /// Create a new custom unit, and add it to the global units cache.
        /// </summary>
        /// <param name="measures">The category of measurement.</param>
        /// <param name="name">The long-form name of the unit.</param>
        /// <param name="prefix">The short-form name/unit specifier.</param>
        /// <param name="isBase">Unit represents the base of its class.</param>
        /// <param name="isSIUnit">Is an SI / metric unit.</param>
        /// <param name="pluralName">Plural long-form name of the unit.</param>
        /// <param name="modifies">Long-form name of the unit that this unit modifies.</param>
        /// <param name="multiplier">The conversion multiplier.</param>
        /// <param name="offset">The conversion offset.</param>
        /// <param name="offsetFirst">Calculate offset, first.</param>
        /// <param name="equation">Equation</param>
        /// <param name="displayDefaultLong">Use the long-form display form by default.</param>
        /// <param name="addToCache">Add the newly created unit to the global unit cache.</param>
        /// <returns></returns>
        public static Unit CreateUnit(
            string measures,
            string name,
            string prefix,
            bool isBase,
            bool isSIUnit = false,
            string pluralName = "", 
            string modifies = "", 
            double multiplier = 0.0d, 
            double offset = 0.0d, 
            bool offsetFirst = false, 
            string equation = "",
            bool displayDefaultLong = false,
            bool addToCache = true)
        {
            var unit = new Unit
            {
                DisplayDefaultLong = displayDefaultLong,
                Equation = equation,
                IsBase = isBase,
                IsSIUnit = isSIUnit,
                Measures = measures,
                Modifies = modifies,
                Multiplier = multiplier,
                Name = name,
                Offset = offset,
                OffsetFirst = offsetFirst,
                PluralName = pluralName,
                Prefix = prefix
            };

            if (addToCache) units.Add(unit);
            return unit;
        }

        [Description("Get the base unit for a specific category.")]
        public static Unit GetBaseUnit(string category)
        {
            foreach (Unit u in units)
            {
                if (u.IsBase == true & (u.Measures.ToLower() ?? "") == (category.ToLower() ?? ""))
                    return (Unit)u.Clone();
            }

            return null;
        }

        [Description("Get all base unit names.")]
        public static string[] GetBaseUnitNames()
        {
            string[] c = null;
            int n = 0;

            foreach (Unit u in units)
            {
                if (u.IsBase == true)
                {
                    Array.Resize(ref c, n + 1);
                    c[n] = TitleCase(u.Name);
                    n += 1;
                }
                // base units come first in a unit collection.
                else break;
            }

            Array.Sort(c);
            return c;
        }

        [Description("Get all base units for all categories.")]
        public static UnitCollection GetBaseUnits()
        {
            return new UnitCollection(((IList<Unit>)Units).Where((u) => u.IsBase).ToArray());
        }

        public static double GetMultiplier(string prefix)
        {
            int i;
            int c;

            c = Prefixes.Length;

            if (prefix.Length <= 2)
            {
                for (i = 0; i < c; i++)
                {
                    if ((prefix ?? "") == (ShortPrefixes[i] ?? ""))
                    {
                        return Multipliers[i];
                    }
                }
            }

            prefix = prefix.ToLower();

            for (i = 0; i < c; i++)
            {
                if ((prefix ?? "") == (Prefixes[i] ?? ""))
                {
                    return Multipliers[i];
                }
            }

            // it's always safe to return 1
            return 1d;
        }

        public static Unit GetUnitByName(string name)
        {
            var res = ((IList<Unit>)Units).Where((e) => e.Name.ToLower() == name.ToLower()).FirstOrDefault();
            return res;
        }

        [Description("Get all unit names for a category.")]
        public static string[] GetUnitNames(string category, bool excludeBase = false)
        {
            var uret = ((IList<Unit>)Units).Where((u) => u.Measures.ToLower() == category.ToLower()).Select(u => u.Name).ToArray();
            Array.Sort(uret);
            return uret;
        }

        [Description("Get all units for a category.")]
        public static UnitCollection GetUnits(string category = "")
        {
            return new UnitCollection(GetUnitsArray(category));
        }

        /// <summary>
        /// Get all units for a category as an array.
        /// </summary>
        /// <param name="category">The optional category name.</param>
        /// <returns></returns>
        public static Unit[] GetUnitsArray(string category = "")
        {
            if (string.IsNullOrEmpty(category))
            {
                return ((IList<Unit>)Units).ToArray();
            } 
            else
            {
                category = TitleCase(category);
                return ((IList<Unit>)Units).Where((u) => u.Measures == category)?.ToArray() ?? new Unit[0];
            }
        }

        public static bool HasCategory(string category)
        {
            foreach (Unit u in units)
            {
                if (u.IsBase)
                {
                    if ((u.Measures.ToLower() ?? "") == (category.ToLower() ?? ""))
                        return true;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        public static Unit IdentifyUnit(string text)
        {
            Unit nu;
            string lstra = text.ToLower(), lstrb, spre = "";

            int i, c = ShortPrefixes.Length;
            bool found = false;
            bool lfound = true;
            string[] ups;

            for (i = -1; i < c; i++)
            {
                string p;
                string lp;

                if (i == -1)
                {
                    p = lp = "";
                }
                else
                {
                    p = ShortPrefixes[i];
                    lp = Prefixes[i].ToLower();
                }

                foreach (var unit in units)
                {
                    ups = unit.Prefix.Split(',');

                    foreach (var up in ups)
                    {
                        spre = p + up;
                        if (text == spre)
                        {
                            found = true;
                            lfound = false;

                            break;
                        }
                    }

                    if (!found) 
                    {
                        lstrb = lp + unit.PluralName.ToLower();

                        if (lstrb == lstra)
                        {
                            found = true;
                        }
                        else
                        {
                            lstrb = lp + unit.Name.ToLower();
                            if (lstra == lstrb)
                            {
                                found = true;
                            }
                        }
                    }

                    if (found)
                    {
                        if (lfound)
                        {
                            spre = p + ups[0];
                        }

                        nu = unit.Clone();

                        if (i == -1)
                        {
                            return nu;
                        }

                        var m = Multipliers[i];

                        nu.Modifies = unit.Name;
                        nu.Name = TitleCase(Prefixes[i] + nu.Name.ToLower());
                        nu.DisplayDefaultLong = lfound;

                        if (!string.IsNullOrEmpty(nu.PluralName))
                            nu.PluralName = TitleCase(Prefixes[i] + nu.PluralName.ToLower());

                        nu.IsBase = false;

                        if (nu.Multiplier != 0)
                        {
                            nu.Multiplier *= m;
                        }
                        else
                        {
                            nu.Multiplier = m;
                        }

                        nu.Prefix = spre;

                        return nu;
                    }
                }
            }
            
            return null;
        }


        public static bool GetBaseValue(double value, Unit unit, out double? baseValue, out Unit baseUnit)
        {
            baseValue = null;
            baseUnit = null;

            if (unit.IsBase)
            {
                if (roundingDigits > 0)
                    baseValue = Round(value, roundingDigits);
                else
                    baseValue = value;

                baseUnit = unit.Clone();

                return true;
            }

            var bUnit = ((IList<Unit>)Units).Where((e) => e.Name == unit.Modifies).FirstOrDefault();
            double bv = value;

            if (bUnit == null) return false;

            if (unit.OffsetFirst)
            {
                bv = bv + unit.Offset;
                if (unit.Multiplier != 0) bv = bv * unit.Multiplier;
            }
            else
            {
                if (unit.Multiplier != 0) bv = bv * unit.Multiplier;
                bv = bv + unit.Offset;

            }

            return GetBaseValue(bv, bUnit, out baseValue, out baseUnit);
        }


        public static bool GetBaseValue(decimal value, Unit unit, out decimal? baseValue, out Unit baseUnit)
        {
            baseValue = null;
            baseUnit = null;

            if (unit.IsBase)
            {
                if (roundingDigits > 0)
                    baseValue = Round(value, roundingDigits);
                else
                    baseValue = value;

                baseUnit = (Unit)unit.Clone();

                return true;
            }

            var bUnit = ((IList<Unit>)Units).Where((e) => e.Name == unit.Modifies).FirstOrDefault();
            decimal bv = value;

            if (bUnit == null) return false;

            if (unit.OffsetFirst)
            {
                bv = bv + (decimal)unit.Offset;
                if (unit.Multiplier != 0) bv = bv * (decimal)unit.Multiplier;
            }
            else
            {
                if (unit.Multiplier != 0) bv = bv * (decimal)unit.Multiplier;
                bv = bv + (decimal)unit.Offset;
            }

            return GetBaseValue(bv, unit, out baseValue, out baseUnit);
        }

        public static bool GetDerivedValue(double baseValue, Unit targetUnit, out double? value)
        {
            value = null;

            if (targetUnit.IsBase)
            {
                value = baseValue;
                return true;
            }

            var unitChain = new List<Unit>();
            var sMod = targetUnit.Modifies;

            unitChain.Add(targetUnit);

            while (true)
            {                
                var bUnit = ((IList<Unit>)Units).Where((e) => e.Name == sMod).FirstOrDefault();
                if (bUnit == null) break;

                unitChain.Add(bUnit);
                sMod = bUnit.Modifies;

                if (bUnit.IsBase) break;
            }

            unitChain.Reverse();
            double nv = baseValue;

            foreach(var unit in unitChain)
            {
                if (unit.OffsetFirst)
                {
                    if (unit.Multiplier != 0) nv /= unit.Multiplier;
                    nv = nv - unit.Offset;
                }
                else
                {
                    nv = nv - unit.Offset;
                    if (unit.Multiplier != 0) nv /= unit.Multiplier;
                }
            }
            if (roundingDigits > 0)
                value = Round(nv, roundingDigits);
            else
                value = nv;

            return true;
        }

        public static bool GetDerivedValue(decimal baseValue, Unit targetUnit, out decimal? value)
        {
            value = null;

            if (targetUnit.IsBase)
            {
                value = baseValue;
                return true;
            }

            var unitChain = new List<Unit>();
            var sMod = targetUnit.Modifies;

            while (true)
            {
                var bUnit = ((IList<Unit>)Units).Where((e) => e.Name == sMod).FirstOrDefault();
                if (bUnit == null) break;

                unitChain.Add(bUnit);
                sMod = bUnit.Modifies;

                if (bUnit.IsBase) break;
            }

            unitChain.Reverse();
            decimal nv = baseValue;

            foreach (var unit in unitChain)
            {
                if (unit.OffsetFirst)
                {
                    if (unit.Multiplier != 0) nv /= (decimal)unit.Multiplier;
                    nv = nv - (decimal)unit.Offset;
                }
                else
                {
                    nv = nv - (decimal)unit.Offset;
                    if (unit.Multiplier != 0) nv /= (decimal)unit.Multiplier;
                }
            }

            if (roundingDigits > 0)
                value = Round(nv, roundingDigits);
            else
                value = nv;

            return true;
        }

        public static void WordsTest(string Example)
        {
            string[] s;
            s = Words(Example);
            Console.WriteLine("There are " + s.Length + " words, starting with " + s[0]);
        }

     
        #endregion Public Methods

    }
}
