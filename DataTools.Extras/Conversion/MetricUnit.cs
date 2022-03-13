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
        private string measures = "";

        [JsonProperty("measures")]
        public string Measures
        {
            get
            {
                return measures;
            }
            set
            {
                measures = TitleCase(value);
            }
        }

        private string name = "";

        [JsonProperty("name")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = TitleCase(value);
            }
        }

        private string prefix = "";

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

        private string pluralName = "";

        [JsonProperty("pluralName")]
        public string PluralName
        {
            get
            {
                return pluralName;
            }

            set
            {
                pluralName = TitleCase(value);
            }
        }

        // ' Indicates this is a base unit to which all other units in this category convert
        private bool isBase = false;

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

        // ' for derived bases.  $ is used to denote a variable.
        private string modifies = "";

        [JsonProperty("modifies")]
        public string Modifies
        {
            get
            {
                return modifies;
            }

            set
            {
                modifies = TitleCase(value);
            }
        }

        private double multiplier = 0.0d;

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

        private double offset = 0.0d;

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

        private bool offsetFirst = false;

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

        // ' Set to True for derived bases with equations.

        private bool derived = false;

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

        private string equation = "";

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

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return PluralName + " (" + Measures + ")";
        }
    }
}
