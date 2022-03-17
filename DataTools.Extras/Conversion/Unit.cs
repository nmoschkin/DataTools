using System;
using System.Collections.Generic;
using static DataTools.Text.TextTools;
using Newtonsoft.Json;

namespace DataTools.Extras.Conversion
{
    
    public class Unit : ICloneable
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

        private string shortestPrefix = "";

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

        /// <summary>
        /// The equation for the unit.
        /// </summary>
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


        /// <summary>
        /// Indicates this is the base unit.
        /// </summary>
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

        /// <summary>
        /// Indicates that the unit is an SI-defined (metric) unit.
        /// </summary>
        [JsonProperty("isSI")]
        public bool IsSIUnit
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

        /// <summary>
        /// Measurement category
        /// </summary>
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

        /// <summary>
        /// Base unit that this unit modifies
        /// </summary>
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

        /// <summary>
        /// Multiplier
        /// </summary>
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

        /// <summary>
        /// Name
        /// </summary>
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

        /// <summary>
        /// Offset
        /// </summary>
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

        /// <summary>
        /// Compute offset before multiplier
        /// </summary>
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

        /// <summary>
        /// Plural name
        /// </summary>
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

        /// <summary>
        /// Prefix (short name)
        /// </summary>
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
                if (value.Contains(","))
                {
                    var vs = new List<string>(value.Split(','));
                    vs.Sort((a, b) =>
                    {
                        if (a.Length < b.Length)
                        {
                            return -1;
                        }
                        else if (b.Length < a.Length)
                        {
                            return 1;
                        }
                        else
                        {
                            return string.Compare(a, b);
                        }
                    });

                    shortestPrefix = vs[0];
                }
                else
                {
                    shortestPrefix = value;
                }
            }
        }

        public string ShortestPrefix
        {
            get => shortestPrefix;
        }

        #endregion Public Properties

        #region Public Methods


        public Unit Clone()
        {
            return (Unit)MemberwiseClone();  
        }
        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            if (obj is Unit mu)
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
