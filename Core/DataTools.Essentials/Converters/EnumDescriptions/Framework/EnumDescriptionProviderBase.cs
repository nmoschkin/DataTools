using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions.Framework
{
    public abstract class EnumDescriptionProviderBase : IEnumDescriptionProvider
    {
        protected TextLoadType loadType = TextLoadType.NoPreference;
        protected readonly Type enumType;

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

        public abstract string ProvideDescription(Enum value);

        public abstract bool CanConvertType(Type type);
    }

    public abstract class EnumDescriptionProviderBase<T> : EnumDescriptionProviderBase, IEnumDescriptionProvider<T> where T : struct, Enum
    {
        protected EnumDescriptionProviderBase() : base(typeof(T))
        {
        }

        public override sealed bool CanConvertType(Type type)
        {
            return type == typeof(T);
        }

        public abstract string ProvideDescription(T value);

        public override string ProvideDescription(Enum value)
        {
            return ProvideDescription((T)value);
        }
    }
}