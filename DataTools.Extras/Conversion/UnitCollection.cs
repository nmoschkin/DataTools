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
    /// <summary>
    /// Contains a collection of units of measurement for various categories of measurement.
    /// </summary>
    public class UnitCollection : Collection<Unit>, IReadOnlyDictionary<string, Unit>
    {

        public static readonly Unit[] MasterUnits;
        
        #region Protected Fields

        protected bool autoSort = true;
        protected object lockObj = new object();

        protected Dictionary<string, Unit> shortMap = new Dictionary<string, Unit>();
        protected Dictionary<string, Unit> longMap = new Dictionary<string, Unit>();

        public IEnumerable<string> Keys 
        {
            get
            {
                lock (lockObj)
                {
                    var l = new List<string>();

                    l.AddRange(shortMap.Keys);
                    l.AddRange(longMap.Keys);

                    return l;
                }
            }
        }
        public IEnumerable<Unit> Values
        {
            get
            {
                lock (lockObj)
                {
                    var l = new List<Unit>();

                    l.AddRange(shortMap.Values);
                    l.AddRange(longMap.Values);

                    return l;
                }
            }
        }

        public Unit this[string key]
        {
            get
            {
                if (shortMap.TryGetValue(key, out Unit value))
                {
                    return value;
                }
                else if (longMap.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion Protected Fields

        #region Public Constructors

        static UnitCollection()
        {
            MasterUnits = JsonConvert.DeserializeObject<Unit[]>(Encoding.UTF8.GetString(AppResources.units));
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

        public bool ContainsKey(string key)
        {
            return (shortMap.ContainsKey(key) || longMap.ContainsKey(key));
        }

        public bool TryGetValue(string key, out Unit value)
        {
            if (shortMap.TryGetValue(key, out value)) return true;
            if (longMap.TryGetValue(key, out value)) return true;
            return false;
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

                Unit[] wunits = Items.Where((u) => u.IsSIUnit && wcat.Contains(u.Measures) && !u.Derived && string.IsNullOrEmpty(u.Equation)).ToArray();
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

                            if (Prefixes[i].EndsWith("bi") && unit.Measures != "BinaryData") continue;

                            var nu = unit.Clone();

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

                var sp = item.Prefix.Split(',');

                foreach(var s in sp)
                {
                    try
                    {
                        shortMap.Add(s, item);
                    }
                    catch (Exception ex)
                    {
                        var z = ex.Message;
                    }
                }

                try
                {
                    longMap.Add(item.Name, item);
                }
                catch (Exception mex)
                {
                    var zy = mex.Message;
                }


                if (item.PluralName != null)
                {
                    try
                    {
                        longMap.Add(item.PluralName, item);
                    }
                    catch (Exception ex)
                    {
                        var z = ex.Message;
                    }
                }

                if (autoSort) Sort();
            }
        }

        protected override void RemoveItem(int index)
        {
            lock(lockObj)
            {
                var item = this[index];
                var sp = item.Prefix.Split(',');

                foreach (var s in sp)
                {
                    if (shortMap.ContainsKey(s))
                    {
                        shortMap.Remove(s);
                    }
                }

                if (longMap.ContainsKey(item.Name)) longMap.Add(item.Name, item);
                if (item.PluralName != null)
                {
                    if (longMap.ContainsKey(item.PluralName)) longMap.Add(item.PluralName, item);
                }

                base.RemoveItem(index);
            }

        }

        protected override void SetItem(int index, Unit newItem)
        {
            lock (lockObj)
            {
                var item = this[index];
                var sp = item.Prefix.Split(',');

                foreach (var s in sp)
                {
                    if (shortMap.ContainsKey(s))
                    {
                        shortMap.Remove(s);
                    }
                }

                if (longMap.ContainsKey(item.Name)) longMap.Add(item.Name, item);
                if (item.PluralName != null)
                {
                    if (longMap.ContainsKey(item.PluralName)) longMap.Add(item.PluralName, item);
                }

                base.SetItem(index, newItem);

                sp = item.Prefix.Split(',');

                foreach (var s in sp)
                {
                    shortMap.Add(s, item);
                }

                longMap.Add(item.Name, item);
                if (item.PluralName != null)
                {
                    longMap.Add(item.PluralName, item);
                }

                Sort();
            }
        }

        IEnumerator<KeyValuePair<string, Unit>> IEnumerable<KeyValuePair<string, Unit>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion Protected Methods
    }
}
