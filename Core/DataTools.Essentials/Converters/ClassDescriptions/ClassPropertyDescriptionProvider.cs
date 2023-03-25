using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions
{
    /// <summary>
    /// A class-level description provider whose default behavior is to let each public property provide their own descriptions.
    /// </summary>
    public class ClassPropertyDescriptionProvider : PropertyDescriptionProviderBase
    {
        private IPropertyDescriptionProvider custass;

        /// <summary>
        /// Explicit providers discovered for specific properties.
        /// </summary>
        protected readonly Dictionary<string, (PropertyInfo, IDescriptionAncestor)> propertyProviders = new Dictionary<string, (PropertyInfo, IDescriptionAncestor)>();

        /// <inheritdoc/>
        public override TextLoadType LoadType { get; }

        /// <summary>
        /// Gets the type of class this description provider is attached to.
        /// </summary>
        public virtual Type ClassType { get; }

        /// <summary>
        /// Create a new class-level description provider whose default behavior is to let each public property provide their own descriptions.
        /// </summary>
        /// <param name="objType">The type this instance is for.</param>
        public ClassPropertyDescriptionProvider(Type objType)
        {
            ClassType = objType;

            var custass = (IPropertyDescriptionProvider)objType.GetCustomAttributes().Where(x => x is IPropertyDescriptionProvider).FirstOrDefault();
            if (custass != null)
            {
                this.custass = custass;
            }

            var props = objType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var prop in props)
            {
                var getdefprov = ClassInfo.ResolveProvider(prop, explicitOnly: true);

                if (getdefprov != null)
                {
                    propertyProviders.Add(prop.Name, (prop, getdefprov));
                }
                else
                {
                    propertyProviders.Add(prop.Name, (prop, null));
                }
            }
        }

        /// <inheritdoc/>
        public override bool CanConvertType(Type type)
        {
            return (type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)?.Length ?? 0) > 0;
        }

        /// <inheritdoc/>
        public override string ProvidePropertyDescription(object value, string propertyName)
        {
            if (custass != null) return custass.ProvideDescription(value, propertyName);

            if (propertyProviders.TryGetValue(propertyName, out var desc) && desc.Item2 != null)
            {
                return desc.Item2.ProvideDescription(desc.Item1.GetValue(value), propertyName);
            }

            return ClassInfo.GetPropertyDescription(value, propertyName);
        }
    }
}