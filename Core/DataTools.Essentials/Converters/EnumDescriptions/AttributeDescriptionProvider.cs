﻿using DataTools.Essentials.Converters.EnumDescriptions.Framework;

using System;

namespace DataTools.Essentials.Converters.EnumDescriptions
{
    /// <summary>
    /// Provides an enum description based on one of three attributes:<br /><br />
    /// <see cref="System.Runtime.Serialization.EnumMemberAttribute"/><br />
    /// <see cref="Newtonsoft.Json.JsonPropertyAttribute"/><br />
    /// <see cref="System.ComponentModel.DescriptionAttribute"/><br />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This is the default provider type for the system.
    /// </remarks>
    public class AttributeDescriptionProvider<T> : EnumDescriptionProviderBase<T> where T : struct, Enum
    {
        public TextLoadType LoadType { get; set; } = TextLoadType.NoPreference;

        public override string ProvideDescription(T value)
        {
            return EnumInfo.GetEnumName(value, true);
        }

        public AttributeDescriptionProvider() : base()
        {
        }
    }

    /// <summary>
    /// Provides an enum description based on one of three attributes:<br /><br />
    /// <see cref="System.Runtime.Serialization.EnumMemberAttribute"/><br />
    /// <see cref="Newtonsoft.Json.JsonPropertyAttribute"/><br />
    /// <see cref="System.ComponentModel.DescriptionAttribute"/><br />
    /// </summary>
    /// <remarks>
    /// This is the default provider type for the system.
    /// </remarks>
    public class AttributeDescriptionProvider : EnumDescriptionProviderBase
    {
        public TextLoadType LoadType { get; set; } = TextLoadType.NoPreference;

        public override string ProvideDescription(Enum value)
        {
            return EnumInfo.GetEnumName(value, true);
        }

        public override bool CanConvertType(Type type)
        {
            return type.IsEnum;
        }

        public AttributeDescriptionProvider(Type enumType) : base(enumType)
        {
        }
    }
}