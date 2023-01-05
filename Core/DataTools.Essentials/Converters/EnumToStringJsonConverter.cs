using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace DataTools.Essentials.Converters
{
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
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return GetEnumValue<T>((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(GetEnumName(value));
        }

        public static string GetEnumName(Enum obj)
        {
            var fis = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var fi in fis)
            {
                var sampleValue = fi.GetValue(null);

                if (sampleValue.Equals(obj))
                {
                    var ema = fi.GetCustomAttribute<EnumMemberAttribute>();
                    if (ema != null)
                    {
                        return ema.Value;
                    }

                    var jp = fi.GetCustomAttribute<JsonPropertyAttribute>();

                    if (jp != null)
                    {
                        return jp.PropertyName;
                    }

                    var de = fi.GetCustomAttribute<DescriptionAttribute>();

                    if (de != null)
                    {
                        return de.Description;
                    }

                    return fi.Name;
                }
            }

            return null;
        }

        public static U GetEnumValue<U>(string obj) where U : struct, Enum
        {
            var fis = typeof(U).GetFields(BindingFlags.Public | BindingFlags.Static);

            if (Enum.TryParse<U>(obj, out var answer))
            {
                return answer;
            }

            foreach (var fi in fis)
            {
                var sampleValue = (U)fi.GetValue(null);

                if (fi.Name?.ToLower() == obj?.ToLower())
                {
                    return sampleValue;
                }

                var ema = fi.GetCustomAttribute<EnumMemberAttribute>();

                if (ema != null && ema.Value?.ToLower() == obj?.ToLower())
                {
                    return sampleValue;
                }

                var jp = fi.GetCustomAttribute<JsonPropertyAttribute>();

                if (jp != null && jp.PropertyName?.ToLower() == obj?.ToLower())
                {
                    return sampleValue;
                }

                var de = fi.GetCustomAttribute<DescriptionAttribute>();

                if (de != null && de.Description?.ToLower() == obj?.ToLower())
                {
                    return sampleValue;
                }
            }

            return default;
        }
    }
}