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
        /// <summary>
        /// Gets or sets the <see cref="TextLoadType"/> for the current attribute description provider.
        /// </summary>
        public TextLoadType LoadType { get; set; } = TextLoadType.NoPreference;

        /// <summary>
        /// Gets the property name associated with this description provider.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Create a new attribute-based description provider
        /// </summary>
        /// <param name="propertyName"></param>
        public AttributeDescriptionProvider([CallerMemberName] string propertyName = null)
        {
            PropertyName = propertyName;
        }

        /// <inheritdoc/>
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
        /// <summary>
        /// Gets or sets the <see cref="TextLoadType"/> for the current attribute description provider.
        /// </summary>
        public TextLoadType LoadType { get; set; } = TextLoadType.NoPreference;

        /// <summary>
        /// Gets the property name associated with this description provider.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Create a new attribute-based description provider
        /// </summary>
        /// <param name="propertyName"></param>
        public AttributeDescriptionProvider([CallerMemberName] string propertyName = null)
        {
            PropertyName = propertyName;
        }

        /// <inheritdoc/>
        public override string ProvideDescription(object value)
        {
            return ClassInfo.GetPropertyDescription(value, PropertyName);
        }

        /// <inheritdoc/>
        public override bool CanConvertType(Type type)
        {
            return true;
        }
    }
}