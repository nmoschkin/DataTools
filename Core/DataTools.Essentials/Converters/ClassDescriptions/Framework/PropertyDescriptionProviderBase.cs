using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// Base class for objects that can provide descriptions for the properties of objects.
    /// </summary>
    public abstract class PropertyDescriptionProviderBase : IPropertyDescriptionProvider
    {
        public abstract TextLoadType LoadType { get; }

        public abstract bool CanConvertType(Type type);

        public abstract string ProvidePropertyDescription(object value, string propertyName);

        string IDescriptionAncestor.ProvideDescription(params object[] args) => ProvidePropertyDescription(args[0], (string)args[1]);
    }

    /// <summary>
    /// Base class for objects that can provide descriptions for the properties of objects of type <typeparamref name="T"/>.
    /// </summary>
    public abstract class PropertyDescriptionProviderBase<T> : PropertyDescriptionProviderBase, IPropertyDescriptionProvider<T>
    {
        public override sealed bool CanConvertType(Type type)
        {
            return type == typeof(T);
        }

        public override sealed string ProvidePropertyDescription(object value, string propertyName)
        {
            return ProvidePropertyDescription((T)value, propertyName);
        }

        public abstract string ProvidePropertyDescription(T value, string propertyName);
    }
}