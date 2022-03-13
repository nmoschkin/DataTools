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


namespace DataTools.Extras.Conversion
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MetricInfo : ICloneable
    {
        private double _Value;

        public double Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }
        }

        private double _BaseValue;

        public double BaseValue
        {
            get
            {
                return _BaseValue;
            }

            set
            {
                _BaseValue = value;
            }
        }

        private double _Multiplier;

        public double Multiplier
        {
            get
            {
                return _Multiplier;
            }

            set
            {
                _Multiplier = value;
            }
        }

        private string _BaseUnit;

        public string BaseUnit
        {
            get
            {
                return _BaseUnit;
            }

            set
            {
                _BaseUnit = value;
            }
        }

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }

        private string _PluralName;

        public string PluralName
        {
            get
            {
                return _PluralName;
            }

            set
            {
                _PluralName = value;
            }
        }

        private string _ShortName;

        public string ShortName
        {
            get
            {
                return _ShortName;
            }

            set
            {
                _ShortName = value;
            }
        }

        private string _Measures;

        public string Measures
        {
            get
            {
                return _Measures;
            }

            set
            {
                _Measures = value;
            }
        }

        private string _Format;

        public string Format
        {
            get
            {
                return _Format;
            }

            set
            {
                _Format = value;
            }
        }

        private string _ShortFormat;

        public string ShortFormat
        {
            get
            {
                return _ShortFormat;
            }

            set
            {
                _ShortFormat = value;
            }
        }

        private MetricUnit _Unit = new MetricUnit();

        public MetricUnit Unit
        {
            get
            {
                return _Unit;
            }

            set
            {
                _Unit = value;
            }
        }

        public override string ToString()
        {
            return _Unit.ToString() + " " + _Value;
        }

        public object Clone()
        {
            MetricInfo objNew = (MetricInfo)MemberwiseClone();
            objNew.Unit = (MetricUnit)_Unit.Clone();
            return objNew;
        }
    }
}
