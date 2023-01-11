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
        private Dictionary<T, string> resourceKeys = null;
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

        public string ContextName => contextName;

        public string ResourceKey => resourceKey;

        public override TextLoadType LoadType { get; }

        public IReadOnlyDictionary<T, string> ResourceKeys => resourceKeys;

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}"/>.
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="resourceKey"></param>
        /// <param name="contextName"></param>
        public GlobalizedDescriptionProvider(Type resourceType, CultureInfo cultureInfo = null, string resourceKey = null, string contextName = null, string separator = "_") : base()
        {
            LoadType = TextLoadType.Lazy;
            this.contextName = contextName ?? typeof(T).Name;
            this.Separator = separator;
            this.resourceKey = resourceKey;
            resmgr = new ResourceManager(resourceType);
            ResourceTypeName = resourceType.FullName;
            ci = cultureInfo ?? CultureInfo.CurrentCulture;
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

        private string ProvideDescription(T value, string propertyName)
        {
            var resourceKey = ComputeKeyName(propertyName);

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
                //                ArgumentException ex = new ArgumentException(
                //                    string.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", resourceKey, ResourceTypeName, ci.Name),
                //                    "Text");
                //#if DEBUG
                //                throw ex;
                //#else
                //                try
                //                {
                //                    translation = resmgr.GetString(resourceKey, new CultureInfo("en")); // default to English
                //                }
                //                catch
                //                {
                //                    translation = resourceKey; // HACK: returns the key, which GETS DISPLAYED TO THE USER
                //                }
                //#endif
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
        protected virtual string ComputeKeyName(string altProp = null)
        {
            if (altProp != null)
            {
                if (classProperties.TryGetValue(altProp, out var tprop))
                {
                    if (tprop.Item2 is TranslationKeyAttribute tattr)
                    {
                        return tattr.Key;
                    }
                    else return tprop.Item1.DeclaringType.Name + Separator + altProp;
                }
            }

            var re = ResourceKey ?? ContextName;
            if (altProp != null && ResourceKey == null) re += Separator + altProp;

            return re;
        }
    }
}