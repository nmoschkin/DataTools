using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Runtime.CompilerServices;

namespace DataTools.Essentials.Converters.ClassDescriptions
{
    /// <summary>
    /// Provides a property description based on one of 2 attributes:<br /><br />
    /// <see cref="System.ComponentModel.DescriptionAttribute"/><br />
    /// <see cref="Newtonsoft.Json.JsonPropertyAttribute"/><br />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This is the default provider type for the system.
    /// </remarks>
    public class AttributeDescriptionProvider<T> : DescriptionProviderBase<T>
    {
        public TextLoadType LoadType { get; set; } = TextLoadType.NoPreference;

        public string PropertyName { get; }

        public AttributeDescriptionProvider([CallerMemberName] string propertyName = null)
        {
            PropertyName = propertyName;
        }

        public override string ProvideDescription(T value)
        {
            return ClassInfo.GetPropertyDescription(value, PropertyName);
        }
    }

    /// <summary>
    /// Provides a property description based on one of 2 attributes:<br /><br />
    /// <see cref="System.ComponentModel.DescriptionAttribute"/><br />
    /// <see cref="Newtonsoft.Json.JsonPropertyAttribute"/><br />
    /// </summary>
    /// <remarks>
    /// This is the default provider type for the system.
    /// </remarks>
    public class AttributeDescriptionProvider : DescriptionProviderBase
    {
        public TextLoadType LoadType { get; set; } = TextLoadType.NoPreference;

        public string PropertyName { get; }

        public AttributeDescriptionProvider([CallerMemberName] string propertyName = null)
        {
            PropertyName = propertyName;
        }

        public override string ProvideDescription(object value)
        {
            return ClassInfo.GetPropertyDescription(value, PropertyName);
        }

        public override bool CanConvertType(Type type)
        {
            return type.IsEnum;
        }
    }
}