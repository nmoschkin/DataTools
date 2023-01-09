using System;

namespace DataTools.Essentials.Converters.EnumDescriptions.Framework
{
    internal abstract class DefaultProviderWrapper : IEnumDescriptionProvider
    {
        public abstract TextLoadType LoadType { get; }

        public abstract bool CanConvertType(Type type);

        public abstract string ProvideDescription(Enum value);
    }
}