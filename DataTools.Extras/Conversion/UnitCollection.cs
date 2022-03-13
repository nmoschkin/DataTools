using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using static System.Math;
using System.Runtime.InteropServices;
using DataTools.MathTools;
using System.Text;
using static DataTools.Text.TextTools;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace DataTools.Extras.Conversion
{
    public class UnitCollection : ObservableCollection<MetricUnit>
    {

        protected MetricTool parent;

        public MetricTool Parent => parent;

        internal UnitCollection(MetricTool parent) : this()
        {
            this.parent = parent;
        }

        public UnitCollection() : this(true)
        {
        }

        public UnitCollection(bool loadDefaultUnits)
        {
            if (loadDefaultUnits)
            {
                var units = JsonConvert.DeserializeObject<MetricUnit[]>(Encoding.UTF8.GetString(AppResources.units));
                foreach (var unit in units)
                {
                    Add(unit);
                }
            }   
        }
    }
}
