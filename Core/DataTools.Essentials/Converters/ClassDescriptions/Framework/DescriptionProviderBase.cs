using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// The base class for description providers
    /// </summary>
    public abstract class DescriptionProviderBase : IDescriptionProvider
    {
        /// <summary>
        /// Backing field for the <see cref="IDescriptionAncestor.LoadType"/> property.
        /// </summary>
        protected TextLoadType loadType = TextLoadType.NoPreference;

        TextLoadType IDescriptionAncestor.LoadType => loadType;

        /// <inheritdoc />
        int IDescriptionAncestor.RequiredParameters => 1;

        /// <inheritdoc />
        public abstract string ProvideDescription(object value);

        /// <inheritdoc />
        public abstract bool CanConvertType(Type type);

        string IDescriptionAncestor.ProvideDescription(params object[] args) => ProvideDescription(args[0]);
    }

    /// <summary>
    /// Base class for typed description providers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DescriptionProviderBase<T> : DescriptionProviderBase, IDescriptionProvider<T>
    {
        /// <inheritdoc />
        public override sealed bool CanConvertType(Type type)
        {
            return type == typeof(T);
        }

        /// <inheritdoc />
        public abstract string ProvideDescription(T value);

        /// <inheritdoc />
        public override sealed string ProvideDescription(object value)
        {
            return ProvideDescription((T)value);
        }
    }
}