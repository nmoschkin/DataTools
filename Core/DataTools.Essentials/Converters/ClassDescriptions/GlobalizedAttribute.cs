using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions
{
    /// <summary>
    /// Adds <see cref="IPropertyDescriptionProvider"/> resource-based globalization support to objects.
    /// </summary>
    /// <remarks>
    /// This is an alternative to using <see cref="DescriptionProviderAttribute"/> with <see cref="GlobalizedDescriptionProvider{T}"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GlobalizedAttribute : Attribute, IPropertyDescriptionProvider
    {
        private System.Resources.ResourceManager resmgr;

        /// <summary>
        /// Gets the resource key name prefix
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Gets the resource key name suffix
        /// </summary>
        public string Suffix { get; }

        /// <summary>
        /// Gets the resource name parts separator string
        /// </summary>
        public string Separator { get; }

        public TextLoadType LoadType { get; }

        /// <summary>
        /// Gets the declared type
        /// </summary>
        public Type DeclaringType { get; }

        private readonly Dictionary<string, (PropertyInfo, TranslationKeyAttribute)> classProperties = new Dictionary<string, (PropertyInfo, TranslationKeyAttribute)>();

        /// <summary>
        /// Initialize a new <see cref="GlobalizedAttribute"/>.
        /// </summary>
        /// <param name="declaringType">The type this attribute is attached to.</param>
        /// <param name="resourceType">The resource manager type.</param>
        /// <param name="prefix">Optional prefix to be added to each key before lookup.</param>
        /// <param name="suffix">Optional suffix to be added to each key before lookup.</param>
        /// <param name="separator">Optional separator string that is used to separate the name parts (can be null.)</param>
        public GlobalizedAttribute(Type declaringType, Type resourceType, string prefix = null, string suffix = null, string separator = "_")
        {
            resmgr = new System.Resources.ResourceManager(resourceType);
            Prefix = prefix ?? declaringType.Name;
            Suffix = suffix;
            this.DeclaringType = declaringType;

            classProperties = new Dictionary<string, (PropertyInfo, TranslationKeyAttribute)>();
            var l = declaringType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in l)
            {
                classProperties.Add(prop.Name, (prop, prop.GetCustomAttribute<TranslationKeyAttribute>()));
            }
            Separator = separator ?? "";
        }

        public string ProvidePropertyDescription(object value, string propertyName)
        {
            var resourceKey = ComputeKeyName(propertyName, value);

            if (resourceKey == null)
                return string.Empty;

            string translation = null;

            try
            {
                translation = resmgr.GetString(resourceKey, CultureInfo.CurrentCulture);
            }
            catch
            {
                try
                {
                    translation = resmgr.GetString(resourceKey, new CultureInfo("en")); // default to English
                }
                catch
                {
                    translation = "bad translation for " + resourceKey;
                }
            }

            if (translation == null)
            {
                return resourceKey;
            }
            return translation;
        }

        public bool CanConvertType(Type type)
        {
            return type == DeclaringType;
        }

        string IDescriptionAncestor.ProvideDescription(params object[] args) => ProvidePropertyDescription(args[0], (string)args[1]);

        /// <summary>
        /// Compute the key name for the specified property
        /// </summary>
        /// <param name="propertyName">The property to compute the key for</param>
        /// <param name="value">A target value.</param>
        /// <returns>A resource lookup key</returns>
        /// <remarks>
        /// This method checks for the presence of the <see cref="TranslationKeyAttribute"/><br /><br />
        /// The properties for the class that this attribute is applied to are cached.<br />
        /// The presence of the <see cref="TranslationKeyAttribute"/> for properties of descendant classes are resolved during invocation.
        /// </remarks>
        protected virtual string ComputeKeyName(string propertyName, object value = default)
        {
            if (propertyName != null)
            {
                if (classProperties.TryGetValue(propertyName, out var tprop))
                {
                    if (tprop.Item2 is TranslationKeyAttribute tattr)
                    {
                        return tattr.ResourceKey;
                    }
                    else return tprop.Item1.DeclaringType.Name + Separator + propertyName;
                }
                else if (value is object && (value.GetType().GetProperty(propertyName) is PropertyInfo spe) && spe.GetCustomAttribute<TranslationKeyAttribute>() is TranslationKeyAttribute matt)
                {
                    return matt.ResourceKey;
                }
            }

            var re = Prefix ?? "";

            if (propertyName != null) re += Separator + propertyName;

            if (Suffix != null) re += Separator + Suffix;

            return re;
        }
    }
}