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
    public class ConversionInfo : ICloneable
    {
        #region Private Fields

        private string baseUnit;
        private double baseValue;
        private string format;
        private string measures;
        private double multiplier;
        private string name;
        private string pluralName;
        private string shortFormat;
        private string shortName;
        private Unit unit;
        private double value;

        #endregion Private Fields

        #region Public Properties

        public string BaseUnit
        {
            get => baseUnit;
            set => baseUnit = value;
        }

        public double BaseValue
        {
            get => baseValue;
            set => baseValue = value;
        }

        public string Format
        {
            get => format;
            set => format = value;
        }

        public string Measures
        {
            get => measures;
            set => measures = value;
        }

        public double Multiplier
        {
            get => multiplier;
            set => multiplier = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string PluralName
        {
            get => pluralName;
            set => pluralName = value;
        }

        public string ShortFormat
        {
            get => shortFormat;
            set => shortFormat = value;
        }

        public string ShortName
        {
            get => shortName;
            set => shortName = value;
        }

        public Unit Unit
        {
            get => unit;
            set => unit = value;
        }

        public double Value
        {
            get => value;
            set => this.value = value;
        }


        #endregion Public Properties

        #region Public Methods

        public object Clone()
        {
            ConversionInfo objNew = (ConversionInfo)MemberwiseClone();
            objNew.Unit = (Unit)unit?.Clone();
            return objNew;
        }

        public override string ToString()
        {
            return unit.ToString() + " " + value;
        }

        #endregion Public Methods
    }
}
