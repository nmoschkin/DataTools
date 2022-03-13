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
    /// Unit value type converter.
    /// </summary>
    /// <remarks></remarks>
    public class UnitValueTypeConverter : DoubleConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if (sourceType == typeof(string))
                return true;
            else if (sourceType.IsPrimitive)
                return true;

            return
                base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            if (destType == typeof(string))
                return true;
            else if (destType.IsPrimitive)
                return true;
            return base.CanConvertTo(context, destType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var uv = new UnitValue();

            if (value.GetType() == typeof(string))
            {
                uv.Parse((string)value);
                return uv;
            }
            else if (IsNumber(value))
            {
                uv.Value = (double)value;
                uv.Unit = "px";

                return uv;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            UnitValue uv = (UnitValue)value;

            if (destinationType == typeof(string))
            {
                return value.ToString();
            }
            else if (destinationType.IsPrimitive)
            {
                return uv.Value;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
