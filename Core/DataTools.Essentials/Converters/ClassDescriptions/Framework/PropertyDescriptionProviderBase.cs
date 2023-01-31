using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// Base class for objects that can provide descriptions for the properties of objects.
    /// </summary>
    public abstract class PropertyDescriptionProviderBase : IPropertyDescriptionProvider
    {
        int IDescriptionAncestor.RequiredParameters => 2;

        /// <inheritdoc />
        public abstract TextLoadType LoadType { get; }

        /// <inheritdoc />
        public abstract bool CanConvertType(Type type);

        /// <inheritdoc />
        public abstract string ProvidePropertyDescription(object value, string propertyName);

        string IDescriptionAncestor.ProvideDescription(params object[] args) => ProvidePropertyDescription(args[0], (string)args[1]);
    }

    /// <summary>
    /// Base class for objects that can provide descriptions for the properties of objects of type <typeparamref name="T"/>.
    /// </summary>
    public abstract class PropertyDescriptionProviderBase<T> : PropertyDescriptionProviderBase, IPropertyDescriptionProvider<T>
    {
        /// <inheritdoc />
        public override sealed bool CanConvertType(Type type)
        {
            return type == typeof(T);
        }

        /// <inheritdoc />
        public override sealed string ProvidePropertyDescription(object value, string propertyName)
        {
            return ProvidePropertyDescription((T)value, propertyName);
        }

        /// <inheritdoc />
        public abstract string ProvidePropertyDescription(T value, string propertyName);
    }
}