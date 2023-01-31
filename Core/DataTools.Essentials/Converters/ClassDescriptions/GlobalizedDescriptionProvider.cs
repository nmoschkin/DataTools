using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions
{
    /// <summary>
    /// Provides descriptions for class properties based on globalizated resources.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlobalizedDescriptionProvider<T> : PropertyDescriptionProviderBase<T>, IDescriptionProvider<T>
    {
        private CultureInfo ci = null;
        private ResourceManager resmgr;
        private string contextName;
        private string resourceKey;
        protected static readonly Dictionary<string, (PropertyInfo, TranslationKeyAttribute)> classProperties;

        static GlobalizedDescriptionProvider()
        {
            classProperties = new Dictionary<string, (PropertyInfo, TranslationKeyAttribute)>();
            var l = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in l)
            {
                classProperties.Add(prop.Name, (prop, prop.GetCustomAttribute<TranslationKeyAttribute>()));
            }
        }

        /// <summary>
        /// Gets the context name for the globalized resolver (usually the class name)
        /// </summary>
        public string ContextName => contextName;

        /// <summary>
        /// Gets an optional explicit resource key
        /// </summary>
        public string ResourceKey => resourceKey;

        public override TextLoadType LoadType { get; }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}"/>.
        /// </summary>
        /// <param name="resourceType">The resource manager type</param>
        /// <param name="resourceKey">The explicit resource key (can be null)</param>
        /// <param name="contextName">The optional context name (usually name of <typeparamref name="T"/>)</param>
        /// <param name="separator">The separator (can be null)</param>
        /// <param name="cultureInfo">Alternative culture info</param>
        public GlobalizedDescriptionProvider(Type resourceType, string resourceKey, string contextName = null, string separator = "_", CultureInfo cultureInfo = null) : base()
        {
            LoadType = TextLoadType.Lazy;
            this.contextName = contextName ?? typeof(T).Name;
            this.Separator = separator ?? "";
            this.resourceKey = resourceKey;
            resmgr = new ResourceManager(resourceType);
            ResourceTypeName = resourceType.FullName;
            ci = cultureInfo ?? CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}"/>.
        /// </summary>
        /// <param name="resourceType">The resource manager type</param>
        public GlobalizedDescriptionProvider(Type resourceType) : this(resourceType, null)
        {
        }

        /// <summary>
        /// Gets or sets the CultureInfo associated with this instance.
        /// </summary>
        public CultureInfo CultureInfo
        {
            get => ci;
            set => ci = value;
        }

        /// <summary>
        /// Gets the current Resource type name string.
        /// </summary>
        public virtual string ResourceTypeName { get; private set; }

        /// <summary>
        /// Gets the string that will separate the prefix or suffix from the key name.
        /// </summary>
        public string Separator { get; set; } = "_";

        /// <summary>
        /// Custom name suffix
        /// </summary>
        public virtual string Suffix { get; protected set; }

        public override string ProvidePropertyDescription(T obj, string propertyName)
        {
            return ProvideDescription(obj, propertyName);
        }

        /// <summary>
        /// Provide the description for the specified value and property name
        /// </summary>
        /// <param name="value">The target object</param>
        /// <param name="propertyName">The property name</param>
        /// <returns></returns>
        protected virtual string ProvideDescription(T value, string propertyName)
        {
            var resourceKey = ComputeKeyName(propertyName, value);

            if (resourceKey == null)
                return string.Empty;

            string translation = null;

            try
            {
                translation = resmgr.GetString(resourceKey, ci);
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

        public string ProvideDescription(T value)
        {
            return ProvideDescription(value, null);
        }

        string IDescriptionProvider.ProvideDescription(object value)
        {
            return ProvideDescription((T)value);
        }

        /// <summary>
        /// Compute the resource key name for the given value based on the current options.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string ComputeKeyName(string altProp = null, T value = default)
        {
            if (altProp != null)
            {
                if (classProperties.TryGetValue(altProp, out var tprop))
                {
                    if (tprop.Item2 is TranslationKeyAttribute tattr)
                    {
                        return tattr.ResourceKey;
                    }
                    else return tprop.Item1.DeclaringType.Name + Separator + altProp;
                }
                else if (value is object && (value.GetType().GetProperty(altProp) is PropertyInfo spe) && spe.GetCustomAttribute<TranslationKeyAttribute>() is TranslationKeyAttribute matt)
                {
                    return matt.ResourceKey;
                }
            }

            var re = ResourceKey ?? ContextName;
            if (altProp != null && ResourceKey == null) re += Separator + altProp;

            return re;
        }
    }
}