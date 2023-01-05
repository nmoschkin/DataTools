using System;

namespace DataTools.Essentials.Converters
{
    public class AttributeDescriptionProvider<T> : IEnumDescriptionProvider<T> where T : struct, Enum
    {
        public TextLoadType LoadType { get; set; } = TextLoadType.NoPreference;

        public string ProvideDescription(T value)
        {
            return EnumToStringConverter<T>.GetEnumName(value);
        }
    }
}