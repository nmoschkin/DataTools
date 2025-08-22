using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions.Framework
{
    /// <summary>
    /// Base class for enum description providers.
    /// </summary>
    public abstract class EnumDescriptionProviderBase : IEnumDescriptionProvider
    {
        /// <summary>
        /// The backing field for <see cref="IDescriptionAncestor.LoadType"/>.
        /// </summary>
        protected TextLoadType loadType = TextLoadType.NoPreference;

        /// <summary>
        /// The backing field for <see cref="EnumType"/>.
        /// </summary>
        protected readonly Type enumType;

        /// <inheritdoc/>
        public int RequiredParameters => 1;

        /// <summary>
        /// Gets the type of enumeration that is being serviced by this instance.
        /// </summary>
        public Type EnumType => enumType;

        /// <summary>
        /// Instantiate a new enum description provider.
        /// </summary>
        /// <param name="enumType">The type of enumeration that is being serviced by this instance.</param>
        protected EnumDescriptionProviderBase(Type enumType)
        {
            this.enumType = enumType;
            if (enumType.IsEnum == false) throw new ArgumentException("Type must be an enumeration!");
        }

        TextLoadType IDescriptionAncestor.LoadType => loadType;

        string IDescriptionAncestor.ProvideDescription(params object[] args) => ProvideDescription((Enum)args[0]);

        /// <inheritdoc/>
        public abstract string ProvideDescription(Enum value);

        /// <inheritdoc/>
        public abstract bool CanConvertType(Type type);

        string IDescriptionProvider.ProvideDescription(object value)
        {
            return ProvideDescription((Enum)value);
        }
    }

    /// <summary>
    /// Generic base class for enum description providers.
    /// </summary>
    public abstract class EnumDescriptionProviderBase<T> : EnumDescriptionProviderBase, IEnumDescriptionProvider<T> where T : struct, Enum
    {
        /// <summary>
        /// Instantiate a new description provider.
        /// </summary>
        protected EnumDescriptionProviderBase() : base(typeof(T))
        {
        }

        /// <inheritdoc/>
        public override sealed bool CanConvertType(Type type)
        {
            return type == typeof(T);
        }

        /// <inheritdoc/>
        public abstract string ProvideDescription(T value);

        /// <inheritdoc/>
        public override string ProvideDescription(Enum value)
        {
            return ProvideDescription((T)value);
        }
    }
}