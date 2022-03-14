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
using Newtonsoft.Json;

namespace DataTools.Extras.Conversion
{
    
    public class MetricUnit : ICloneable
    {
        #region Private Fields

        private bool derived = false;
        private string equation = "";
        private bool isBase = false;

        private string measures = "";

        private string modifies = "";

        private double multiplier = 0.0d;

        private string name = "";

        private double offset = 0.0d;

        private bool offsetFirst = false;

        private string pluralName = "";

        private string prefix = "";

        private bool isSI = true;

        #endregion Private Fields

        #region Public Properties

        [JsonProperty("derived")]
        public bool Derived
        {
            get
            {
                return derived;
            }

            set
            {
                derived = value;
            }
        }

        // ' Equation for derived bases.  $ is used to denote a variable.
        [JsonProperty("equation")]
        public string Equation
        {
            get
            {
                return equation;
            }

            set
            {
                equation = value;
            }
        }

        [JsonProperty("isBase")]
        public bool IsBase
        {
            get
            {
                return isBase;
            }

            set
            {
                isBase = value;
            }
        }

        [JsonProperty("isSI")]
        public bool IsSI
        {
            get => isSI;
            set
            {
                if (isSI != value)
                {
                    isSI = value;
                }
            }
        }

        [JsonProperty("measures")]
        public string Measures
        {
            get
            {
                return measures;
            }
            set
            {
                measures = (value);
            }
        }
        [JsonProperty("modifies")]
        public string Modifies
        {
            get
            {
                return modifies;
            }

            set
            {
                modifies = (value);
            }
        }

        [JsonProperty("multiplier")]
        public double Multiplier
        {
            get
            {
                return multiplier;
            }

            set
            {
                multiplier = value;
            }
        }

        [JsonProperty("name")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = (value);
            }
        }
        [JsonProperty("offset")]
        public double Offset
        {
            get
            {
                return offset;
            }

            set
            {
                offset = value;
            }
        }

        [JsonProperty("offsetFirst")]
        public bool OffsetFirst
        {
            get
            {
                return offsetFirst;
            }

            set
            {
                offsetFirst = value;
            }
        }

        [JsonProperty("pluralName")]
        public string PluralName
        {
            get
            {
                return pluralName;
            }

            set
            {
                pluralName = (value);
            }
        }

        [JsonProperty("prefix")]
        public string Prefix
        {
            get
            {
                return prefix;
            }

            set
            {
                prefix = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            if (obj is MetricUnit mu)
            {
                return JsonConvert.SerializeObject(this) == JsonConvert.SerializeObject(mu);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return JsonConvert.SerializeObject(this).GetHashCode();
        }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(PluralName) ? Name : PluralName) + " (" + Separate(Measures) + ")";
        }

        #endregion Public Methods
    }
}
