using System;
using System.Linq;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// Indicate how a description should be provided for something
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class DescriptionProviderAttribute : Attribute
    {
        private readonly object[] parameters;
        public Type ProviderType { get; }

        /// <summary>
        /// Create an instance of the configured provider
        /// </summary>
        /// <returns></returns>
        public virtual IDescriptionAncestor CreateInstance()
        {
            return (IDescriptionAncestor)Activator.CreateInstance(ProviderType, parameters);
        }

        /// <summary>
        /// Specifies how a description is provided for a set of enumeration values.
        /// </summary>
        /// <param name="providerType">The provider type (must implement <see cref="IEnumDescriptionProvider"/>.</param>
        /// <param name="parameters">Optional instantiation parameters for the <paramref name="providerType"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public DescriptionProviderAttribute
            (Type providerType, params object[] parameters)
        {
            var ifaces = providerType.GetInterfaces().Where(x => x.FullName.StartsWith(typeof(IDescriptionProvider).FullName));
            if (!ifaces.Any()) throw new ArgumentException("providerType must implement IDescriptionProvider");

            ProviderType = providerType;
            this.parameters = parameters;
        }
    }
}