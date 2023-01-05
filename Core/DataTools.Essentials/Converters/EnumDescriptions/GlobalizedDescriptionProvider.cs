﻿using DataTools.Essentials.Converters.EnumDescriptions.Framework;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions
{
    /// <summary>
    /// Provides descriptions for enum values based on globalizated resources.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlobalizedDescriptionProvider<T> : EnumDescriptionProviderBase<T> where T : struct, Enum
    {
        private CultureInfo ci = null;
        private ResourceManager resmgr;
        private Dictionary<T, string> resourceKeys = null;

        public IReadOnlyDictionary<T, string> ResourceKeys => resourceKeys;

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="nameOptions">Describe how the resource name is computed before looking up the string value.</param>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <param name="customPrefix">The custom prefix to specify for the resource name values.</param>
        /// <param name="customSuffix">The custom suffix to specify for the resource name values.</param>
        /// <param name="customSeparator">The string that will separate parts of the computed resource key name.</param>
        /// <remarks>
        /// If <paramref name="assembly"/> is null, the assembly is taken from <typeparamref name="T"/>.
        /// </remarks>
        public GlobalizedDescriptionProvider(KeyNameOptions nameOptions, string resourceTypeName, Assembly assembly, CultureInfo cultureInfo = null, string customPrefix = null, string customSuffix = null, string customSeparator = "_") : base()
        {
            if (nameOptions == KeyNameOptions.Explicit)
            {
                nameOptions = KeyNameOptions.TypePrefix;
#if DEBUG
                System.Diagnostics.Debug.WriteLine("KeyNameOptions.Explicit is invalid for this constructor. Using default TypePrefix, instead.", "Warning");
#endif
            }

            Prefix = customPrefix;
            Suffix = customSuffix;
            Separator = customSeparator;
            NameOptions = nameOptions;
            ResourceTypeName = resourceTypeName;
            assembly = assembly ?? typeof(T).Assembly;
            resmgr = new ResourceManager(ResourceTypeName, assembly);
            ci = cultureInfo ?? CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="resourceKeys">The keys that will be used to look up globalized resources.</param>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <remarks>
        /// The <paramref name="resourceKeys"/> must contain exactly one description for each unique field in the <see cref="Enum"/> type.<br /><br />
        /// Flag combinations are not checked. It's up to the consumer to provide the correct values for expected flag combinations.
        /// </remarks>
        public GlobalizedDescriptionProvider(IEnumerable<KeyValuePair<T, string>> resourceKeys, string resourceTypeName, Assembly assembly, CultureInfo cultureInfo) : base()
        {
            NameOptions = KeyNameOptions.Explicit;

            this.resourceKeys = new Dictionary<T, string>();

            foreach (var kv in resourceKeys)
            {
                this.resourceKeys.Add(kv.Key, kv.Value);
            }

            ResourceTypeName = resourceTypeName;
            assembly = assembly ?? typeof(T).Assembly;
            resmgr = new ResourceManager(ResourceTypeName, assembly);
            ci = cultureInfo ?? CultureInfo.CurrentCulture;

            ExplicitDescriptionProvider<T>.CheckSource(this.resourceKeys);
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <remarks>
        /// If <paramref name="assembly"/> is null, the assembly is taken from <typeparamref name="T"/>.
        /// </remarks>
        public GlobalizedDescriptionProvider(string resourceTypeName, Assembly assembly, CultureInfo cultureInfo = null) : this(KeyNameOptions.TypePrefix, resourceTypeName, assembly, cultureInfo: cultureInfo)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <param name="customSuffix">The custom suffix to specify for the resource name values.</param>
        /// <remarks>
        /// If <paramref name="assembly"/> is null, the assembly is taken from <typeparamref name="T"/>.
        /// </remarks>
        public GlobalizedDescriptionProvider(string resourceTypeName, Assembly assembly, string customSuffix, CultureInfo cultureInfo = null) : this(KeyNameOptions.CustomSuffix, resourceTypeName, assembly, customSuffix: customSuffix, cultureInfo: cultureInfo)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <param name="customPrefix">The custom prefix to specify for the resource name values.</param>
        /// <remarks>
        /// If <paramref name="assembly"/> is null, the assembly is taken from <typeparamref name="T"/>.
        /// </remarks>
        public GlobalizedDescriptionProvider(string customPrefix, string resourceTypeName, Assembly assembly, CultureInfo cultureInfo = null) : this(KeyNameOptions.CustomPrefix, resourceTypeName, assembly, customPrefix: customPrefix, cultureInfo: cultureInfo)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <remarks>
        /// In this usage, the assembly is taken from <typeparamref name="T"/>.
        /// </remarks>
        public GlobalizedDescriptionProvider(string resourceTypeName, CultureInfo cultureInfo = null) : this(KeyNameOptions.TypePrefix, resourceTypeName, (Assembly)null, cultureInfo: cultureInfo)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="customPrefix">The custom prefix to specify for the resource name values.</param>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <remarks>
        /// In this usage, the assembly is taken from <typeparamref name="T"/>.
        /// </remarks>
        public GlobalizedDescriptionProvider(string customPrefix, string resourceTypeName, CultureInfo cultureInfo = null) : this(KeyNameOptions.CustomPrefix, resourceTypeName, (Assembly)null, cultureInfo: cultureInfo, customPrefix: customPrefix)
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

        public virtual TextLoadType LoadType { get; } = TextLoadType.Lazy;

        /// <summary>
        /// Describe how the resource name is computed before looking up the string value.
        /// </summary>
        public virtual KeyNameOptions NameOptions { get; protected set; } = KeyNameOptions.TypePrefix;

        /// <summary>
        /// Custom name prefix
        /// </summary>
        public virtual string Prefix { get; protected set; }

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

        public override string ProvideDescription(T value)
        {
            var resourceKey = ComputeKeyName(value);

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
                ArgumentException ex = new ArgumentException(
                    string.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", resourceKey, ResourceTypeName, ci.Name),
                    "Text");
#if DEBUG
                throw ex;
#else
                try
                {
                    translation = resmgr.GetString(resourceKey, new CultureInfo("en")); // default to English
                }
                catch
                {
                    translation = resourceKey; // HACK: returns the key, which GETS DISPLAYED TO THE USER
                }
#endif
            }
            return translation;
        }

        /// <summary>
        /// Compute the resource key name for the given value based on the current options.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string ComputeKeyName(T value)
        {
            if (NameOptions == KeyNameOptions.Explicit)
            {
                if (!resourceKeys.ContainsKey(value)) throw new ArgumentException("A globalized resource key name was not found for the enumeration value '" + value.GetType().FullName + "." + value.ToString() + "' using explicit reckoning. Please check your instance of GlobalizationDescriptionProvider");
                return resourceKeys[value];
            }

            var sb = new StringBuilder();

            if ((NameOptions & KeyNameOptions.CustomPrefix) == KeyNameOptions.CustomPrefix && !string.IsNullOrEmpty(Prefix))
            {
                sb.Append(Prefix);
            }
            else
            {
                switch (NameOptions)
                {
                    case KeyNameOptions.TypePrefix:
                        sb.Append(typeof(T).Name);
                        break;

                    case KeyNameOptions.FullTypePrefix:
                        sb.Append(typeof(T).FullName);
                        break;
                }
            }

            if (sb.Length > 0 && !string.IsNullOrEmpty(Separator))
            {
                sb.Append(Separator);
            }

            sb.Append(value.ToString());

            if ((NameOptions & KeyNameOptions.CustomSuffix) == KeyNameOptions.CustomSuffix && !string.IsNullOrEmpty(Suffix))
            {
                if (!string.IsNullOrEmpty(Separator)) sb.Append(Separator);
                sb.Append(Suffix);
            }
            else
            {
                switch (NameOptions)
                {
                    case KeyNameOptions.TypeSuffix:
                        if (!string.IsNullOrEmpty(Separator)) sb.Append(Separator);
                        sb.Append(typeof(T).Name);
                        break;

                    case KeyNameOptions.FullTypeSuffix:
                        if (!string.IsNullOrEmpty(Separator)) sb.Append(Separator);
                        sb.Append(typeof(T).FullName);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}