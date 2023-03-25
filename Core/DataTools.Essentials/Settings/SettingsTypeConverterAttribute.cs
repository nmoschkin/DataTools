using System;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Add a settings type converter attribute to a property or class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class SettingsTypeConverterAttribute : Attribute
    {
        private static readonly Type otv = typeof(object);
        private static readonly Type stv = typeof(SettingsTypeConverter);

        /// <summary>
        /// The converter type
        /// </summary>
        public Type ConverterType { get; }

        /// <summary>
        /// Create a new converter type attribute
        /// </summary>
        /// <param name="converterType">The type</param>
        public SettingsTypeConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
            var ctv = converterType;

            while(ctv != otv)
            {
                if (ctv == stv) break;
                ctv = ctv.BaseType;
            }

            if (ctv == otv) throw new ArgumentException("Type must be derived from SettingsTypeConverter");
        }

    }

}
