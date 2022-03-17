using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;
using Newtonsoft.Json;

using DataTools.SortedLists;
using System;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Drawing.Printing;
using static DataTools.Extras.Conversion.ConversionTool;
using DataTools.Text;

namespace DataTools.Extras.Conversion
{
    public class UnitCollection : Collection<Unit>
    {
        #region Protected Fields

        public static readonly Unit[] MasterUnits;
        public static readonly string[] DefaultCategoryList;


        protected bool autoSort = true;
        protected object lockObj = new object();
        protected ConversionTool parent;

        #endregion Protected Fields

        #region Public Constructors

        static UnitCollection()
        {
            MasterUnits = JsonConvert.DeserializeObject<Unit[]>(Encoding.UTF8.GetString(AppResources.units));
            DefaultCategoryList = MasterUnits.Select((u) => u.Measures).Distinct().ToArray();
        }

        /// <summary>
        /// Initialize a new UnitList with the specified units.
        /// </summary>
        /// <param name="initValues"></param>
        public UnitCollection(IEnumerable<Unit> initValues) : base()
        {
            lock (lockObj)
            {
                autoSort = false;
                foreach (var unit in initValues) Add(unit);
                autoSort = true;
                Sort();

            }
        }

        /// <summary>
        /// Create a new UnitList with the default units loaded from a JSON resource.
        /// </summary>
        public UnitCollection() : this(true)
        {
        }

        /// <summary>
        /// Createa new UnitList, optionally loading the default units from a JSON resource.
        /// </summary>
        /// <param name="loadDefaultUnits">True to load the default units.</param>
        public UnitCollection(bool loadDefaultUnits)
        {
            if (loadDefaultUnits)
            {
                lock (lockObj)
                {
                    autoSort = false;
                    foreach (var unit in MasterUnits) Add(unit);
                    autoSort = true;
                    Sort();
                }
            }
        }

        /// <summary>
        /// Initialize a unit collection containing only the specified categories.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="categories"></param>
        public UnitCollection(string category, params string[] categories)
        {
            lock (lockObj)
            {
                List<string> categoriesList = new List<string>(new[] { category.ToLower() });
                foreach (var str in categories) categoriesList.Add(str.ToLower());

                var units = ((IList<Unit>)MasterUnits).Where((u) => categoriesList.Contains(u.Measures.ToLower())).ToArray();

                autoSort = false;
                foreach (var unit in units) Add(unit);
                autoSort = true;
                Sort();
            }

        }

        #endregion Public Constructors

        #region Internal Constructors

        /// <summary>
        /// Initialize with the specified conversion tool parent.
        /// </summary>
        /// <param name="parent"></param>
        internal UnitCollection(ConversionTool parent) : this()
        {
            this.parent = parent;
        }

        #endregion Internal Constructors

        #region Public Properties

        public ConversionTool Parent => parent;

        #endregion Public Properties

        #region Public Methods

        public void AddRange(IEnumerable<Unit> items)
        {
            lock(lockObj) 
            {
                autoSort = false;
                foreach (var item in items) Add(item);
                autoSort = true;
                Sort();
            }
        }

        public void InitializeSIDerivedUnits(params string[] categories)
        {
            lock (lockObj)
            {
                string[] wcat;
                if (categories != null && categories.Length > 0)
                {
                    wcat = categories.ToArray();
                }
                else
                {
                    wcat = base.Items.Select((u) => u.Measures).Distinct().ToArray();
                }

                Unit[] wunits = Items.Where((u) => u.IsSIUnit && wcat.Contains(u.Measures) && u.IsBase).ToArray();
                List<Unit> addUnits = new List<Unit>();

                foreach (var p in ShortPrefixes)
                {
                    foreach (var unit in wunits)
                    {
                        var ups = unit.Prefix.Split(',');

                        foreach (var up in ups)
                        {
                            var text = p + up;

                            var i = ((IList<string>)ShortPrefixes).IndexOf(p);
                            var m = Multipliers[i];

                            if (unit.Measures != "BinaryData" && Prefixes[i].EndsWith("bi")) continue;

                            var nu = (Unit)unit.Clone();

                            nu.Modifies = unit.Name;
                            nu.Name = TextTools.TitleCase(Prefixes[i] + nu.Name.ToLower());

                            if (!string.IsNullOrEmpty(nu.PluralName))
                                nu.PluralName = TextTools.TitleCase(Prefixes[i] + nu.PluralName.ToLower());

                            nu.IsBase = false;

                            if (nu.Multiplier != 0)
                            {
                                nu.Multiplier *= m;
                            }
                            else
                            {
                                nu.Multiplier = m;
                            }

                            nu.Prefix = text;
                            addUnits.Add(nu);
                        }

                    }
                }
                
                AddRange(addUnits); 
            }

        }

        public void Sort()
        {
            QuickSort.Sort(base.Items,
               new Comparison<Unit>((a, b) =>
               {
                   int i;
                   if (a.IsBase == b.IsBase)
                   {
                       i = string.Compare(a.Measures, b.Measures);
                       if (i == 0)
                       {
                           i = string.Compare(a.Name, b.Name);
                           if (i == 0)
                           {
                               i = string.Compare(a.PluralName, b.PluralName);
                           }
                       }
                   }
                   else
                   {
                       if (a.IsBase) i = -1;
                       else i = 1;
                   }

                   return i;
               }));
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void InsertItem(int index, Unit item)
        {
            lock(lockObj)
            {
                base.InsertItem(index, item);
                if (autoSort) Sort();
            }
        }

        #endregion Protected Methods
    }
}
