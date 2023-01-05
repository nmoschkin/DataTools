using System;
using System.Linq;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions.Framework
{
    /// <summary>
    /// Specifies how a description might be provided for <see cref="Enum"/> type values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class DescriptionProviderAttribute : Attribute
    {
        private readonly object[] parameters;
        public Type ProviderType { get; }

        /// <summary>
        /// Create an instance of the configured provider
        /// </summary>
        /// <returns></returns>
        public virtual IEnumDescriptionProvider CreateInstance()
        {
            return (IEnumDescriptionProvider)Activator.CreateInstance(ProviderType, parameters);
        }

        /// <summary>
        /// Specifies how a description might be provided for a set of enumeration values.
        /// </summary>
        /// <param name="providerType">The provider type (must implement <see cref="IEnumDescriptionProvider"/>.</param>
        /// <param name="parameters">Optional instantiation parameters for the <paramref name="providerType"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public DescriptionProviderAttribute(Type providerType, params object[] parameters)
        {
            var ifaces = providerType.GetInterfaces().Where(x => x.FullName.StartsWith(typeof(IEnumDescriptionProvider).FullName));
            if (!ifaces.Any()) throw new ArgumentException("providerType must implement IEnumDescriptionProvider");

            ProviderType = providerType;
            this.parameters = parameters;
        }
    }
}