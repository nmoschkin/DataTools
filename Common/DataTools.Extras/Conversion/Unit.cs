using System;
using System.Collections.Generic;
using static DataTools.Text.TextTools;
using Newtonsoft.Json;
using System.Linq;

namespace DataTools.Extras.Conversion
{
    
    public class Unit : ICloneable
    {
        #region Private Fields

        public static JsonSerializerSettings JsonSettings { get; set; } = new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        private bool derived = false;

        private bool displayDefaultLong = false;

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

        #region Internal Constructors 

        protected internal Unit()
        {
        }

        #endregion

        #region Public Properties

        [JsonProperty("derived")]
        public bool Derived
        {
            get
            {
                return derived;
            }
            protected internal set
            {
                derived = value;
            }
        }


        /// <summary>
        /// Displays the long form as the default string form.
        /// </summary>
        [JsonProperty("displayDefaultLong")]
        public bool DisplayDefaultLong
        {
            get { return displayDefaultLong; }
            protected internal set { displayDefaultLong = value; }
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
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
            protected internal set
            {
                prefix = value;
                
                if (value.Contains(","))
                {
                    var vs = value.Split(',');

                    int c = vs[0].Length;                    
                    int x = 0;
                    
                    for (int i = 1; i < vs.Length; i++)
                    {
                        int y = vs[i].Length;

                        if (y < c)
                        {
                            c = y;
                            x = i;
                        }
                    }

                    shortestPrefix = vs[x];
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

        public static Unit FromJson(string json)
        {            
            var unit = JsonConvert.DeserializeObject<Unit>(json, JsonSettings);
            return unit;
        }

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
                return JsonConvert.SerializeObject(this, JsonSettings) == JsonConvert.SerializeObject(mu, JsonSettings);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return JsonConvert.SerializeObject(this, JsonSettings).GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, JsonSettings);
        }

        #endregion Public Methods
    }
}
