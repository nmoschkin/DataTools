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
    /// <summary>
    /// Represents a Value in a given unit or compound unit.
    /// </summary>
    /// <remarks></remarks>
    [TypeConverter(typeof(UnitValueTypeConverter))]
    public struct UnitValue
    {

        /// <summary>
        /// The value of the measurement.
        /// </summary>
        /// <remarks></remarks>
        public double Value;

        /// <summary>
        /// The string representation of the unit or compound unit of the measurement.
        /// </summary>
        /// <remarks></remarks>
        public string Unit;

        /// <summary>
        /// Represents the internal metric info.
        /// </summary>
        /// <remarks></remarks>
        private MetricInfo info;


        /// <summary>
        /// Parse the contents structure into a formal unit/value string.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            MetricTool.Parse(Value + Unit, ref info);
            Unit = info.ShortName;
            Value = info.Value;
            return info.ShortFormat;
        }

        /// <summary>
        /// Parse a new value.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public void Parse(string value)
        {
            MetricTool.Parse(value, ref info, "", true);
            Unit = info.ShortName;
            Value = info.Value;
        }

        public MetricInfo GetDetails()
        {
            return info;
        }

        #region String CType Operators

        public static implicit operator string(UnitValue operand)
        {
            return operand.ToString();
        }

        public static explicit operator UnitValue(string operand)
        {
            var u = new UnitValue();
            u.Parse(operand);
            return u;
        }

        #endregion

        #region Numeric CType Operators

        public static UnitValue NumberToUnitValue(double operand)
        {
            var u = new UnitValue();
            MetricTool.Parse(operand + "px", ref u.info);
            u.Unit = u.info.ShortName;
            u.Value = u.info.Value;
            return u;
        }

        public static implicit operator long(UnitValue operand)
        {
            return (int)Round(operand.Value);
        }

        public static explicit operator UnitValue(long operand)
        {
            return NumberToUnitValue(operand);
        }

        public static implicit operator int(UnitValue operand)
        {
            return (int)Round(operand.Value);
        }

        public static explicit operator UnitValue(int operand)
        {
            return NumberToUnitValue(operand);
        }

        public static implicit operator short(UnitValue operand)
        {
            return (short)(int)Round(operand.Value);
        }

        public static explicit operator UnitValue(short operand)
        {
            return NumberToUnitValue(operand);
        }

        public static implicit operator double(UnitValue operand)
        {
            return (int)Round(operand.Value);
        }

        public static explicit operator UnitValue(double operand)
        {
            return NumberToUnitValue(operand);
        }

        // Public Shared Widening Operator CType(operand As MetricTool.UnitValue) As Double
        // Return CInt(operand.Value)
        // End Operator

        // Public Shared Narrowing Operator CType(operand As Double) As MetricTool.UnitValue
        // Return NumberToUnitValue(CDbl(operand))
        // End Operator

        #endregion

        #region Equality Operators

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            UnitValue u = (UnitValue)obj;
            if (u.Value != Value)
                return false;
            if ((u.Unit ?? "") != (Unit ?? ""))
                return false;
            return true;
        }

        public static bool operator ==(UnitValue operand1, UnitValue operand2)
        {
            return operand1.Equals(operand2);
        }

        public static bool operator !=(UnitValue operand1, UnitValue operand2)
        {
            return !operand1.Equals(operand2);
        }

        #endregion

    }
}
