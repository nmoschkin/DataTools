using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// The base class for description providers
    /// </summary>
    public abstract class DescriptionProviderBase : IDescriptionProvider
    {
        protected TextLoadType loadType = TextLoadType.NoPreference;

        TextLoadType IDescriptionAncestor.LoadType => loadType;

        public abstract string ProvideDescription(object value);

        public abstract bool CanConvertType(Type type);

        string IDescriptionAncestor.ProvideDescription(params object[] args) => ProvideDescription(args[0]);
    }

    /// <summary>
    /// Base class for typed description providers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DescriptionProviderBase<T> : DescriptionProviderBase, IDescriptionProvider<T>
    {
        public override sealed bool CanConvertType(Type type)
        {
            return type == typeof(T);
        }

        public abstract string ProvideDescription(T value);

        public override sealed string ProvideDescription(object value)
        {
            return ProvideDescription((T)value);
        }
    }
}