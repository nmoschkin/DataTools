using System;
using System.Reflection;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// Converter for types that implement a static 'Parse(string)' method
    /// </summary>
    public class ParseableTypeConverter : SettingsTypeConverter
    {
        /// <summary>
        /// Try to get the static string parsing method (<paramref name="parseMethod"/>) from the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="parseMethod">Receives method info for the 'Parse' method, if found.</param>
        /// <returns>True if the <paramref name="parseMethod"/> was successfully retrieved.</returns>
        public static bool TryGetParseMethod(Type type, out MethodInfo parseMethod)
        {
            parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });

            if (parseMethod != null && parseMethod.IsStatic)
            {
                return true;
            }
            
            return false;
        }

        /// <inheritdoc/>
        public override bool CanConvertFrom(Type type)
        {
            if (type == typeof(string)) return true;
            return TryGetParseMethod(type, out _);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(Type type)
        {
            if (type == typeof(string)) return true;
            return TryGetParseMethod(type, out _);
        }

        /// <inheritdoc/>
        public override ISetting ConvertFrom(string key, object value, Type sourceType, Type destinationType, ISettings parent = null)
        {
            if (value is string s && TryGetParseMethod(destinationType, out var mtd))
            {
                return new ProgramSetting(key, mtd.Invoke(null, new object[] { s }), parent);
            }
            else if (!(value is string) && TryGetParseMethod(sourceType, out _) && destinationType == typeof(string))
            {
                return new ProgramSetting(key, value?.ToString(), parent);
            }

            throw new NotImplementedException("Type is neither parseable nor string");
        }

        /// <inheritdoc/>
        public override object ConvertTo(ISetting setting, Type sourceType, Type destinationType)
        {
            var value = setting.Value;

            if (value is string s && TryGetParseMethod(destinationType, out var mtd))
            {
                return mtd.Invoke(null, new object[] { s });
            }
            else if (!(value is string) && TryGetParseMethod(sourceType, out _) && destinationType == typeof(string))
            {
                return value?.ToString();
            }

            throw new NotImplementedException("Type is neither parseable nor string");
        }
    }

}
