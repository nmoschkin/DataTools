using DataTools.Essentials.Converters.EnumDescriptions.Framework;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace DataTools.Essentials.Converters
{
    /// <summary>
    /// Converts an enum to and from string for JSON parsing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("Use EnumToStringJsonConverter<T> instead.")]
    public class EnumToStringConverter<T> : EnumToStringJsonConverter<T> where T : struct, Enum
    {
    }

    /// <summary>
    /// Converts an enum to and from string for JSON parsing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumToStringJsonConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        /// <inheritdoc/>
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return GetEnumValue<T>((string)reader.Value);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(EnumInfo.GetEnumName(value));
        }

        /// <summary>
        /// Parse an enum value from the specified string
        /// </summary>
        /// <typeparam name="U">The type of the enum to parse.</typeparam>
        /// <param name="text">The string to parse</param>
        /// <returns></returns>
        public static U GetEnumValue<U>(string text) where U : struct, Enum
        {
            var fis = typeof(U).GetFields(BindingFlags.Public | BindingFlags.Static);

            if (Enum.TryParse<U>(text, out var answer))
            {
                return answer;
            }

            foreach (var fi in fis)
            {
                var sampleValue = (U)fi.GetValue(null);

                if (fi.Name?.ToLower() == text?.ToLower())
                {
                    return sampleValue;
                }

                var ema = fi.GetCustomAttribute<EnumMemberAttribute>();

                if (ema != null && ema.Value?.ToLower() == text?.ToLower())
                {
                    return sampleValue;
                }

                var jp = fi.GetCustomAttribute<JsonPropertyAttribute>();

                if (jp != null && jp.PropertyName?.ToLower() == text?.ToLower())
                {
                    return sampleValue;
                }

                var de = fi.GetCustomAttribute<DescriptionAttribute>();

                if (de != null && de.Description?.ToLower() == text?.ToLower())
                {
                    return sampleValue;
                }
            }

            return default;
        }
    }
}