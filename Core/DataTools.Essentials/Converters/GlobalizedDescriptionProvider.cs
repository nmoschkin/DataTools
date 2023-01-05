using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace DataTools.Essentials.Converters
{
    /// <summary>
    /// Provides descriptions for enum values based on globalizated resources.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlobalizedDescriptionProvider<T> : IEnumDescriptionProvider<T> where T : struct, Enum
    {
        private CultureInfo ci = null;
        private ResourceManager resmgr;

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="nameOptions">Describe how the resource name is computed before looking up the string value.</param>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <param name="customPrefix">The custom prefix to specify for the resource name values.</param>
        /// <param name="customSuffix">The custom suffix to specify for the resource name values.</param>
        /// <param name="customSeparator">The string that will separate parts of the computer resource key name.</param>
        public GlobalizedDescriptionProvider(KeyNameOptions nameOptions, string resourceTypeName, Assembly assembly, CultureInfo cultureInfo = null, string customPrefix = null, string customSuffix = null, string customSeparator = "_")
        {
            Prefix = customPrefix;
            Suffix = customSuffix;
            Separator = customSeparator;
            NameOptions = nameOptions;
            ResourceTypeName = resourceTypeName;
            resmgr = new ResourceManager(ResourceTypeName, assembly);
            ci = cultureInfo ?? CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
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
        public GlobalizedDescriptionProvider(string resourceTypeName, Assembly assembly, string customSuffix, CultureInfo cultureInfo = null) : this(KeyNameOptions.TypePrefix, resourceTypeName, assembly, customSuffix: customSuffix, cultureInfo: cultureInfo)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="GlobalizedDescriptionProvider{T}" />
        /// </summary>
        /// <param name="resourceTypeName">The fully qualified type name of the resource class.</param>
        /// <param name="assembly">The assembly that contains the resource class.</param>
        /// <param name="cultureInfo">CultureInfo to reference when rendering a string.</param>
        /// <param name="customPrefix">The custom prefix to specify for the resource name values.</param>
        public GlobalizedDescriptionProvider(string customPrefix, string resourceTypeName, Assembly assembly, CultureInfo cultureInfo = null) : this(KeyNameOptions.TypePrefix, resourceTypeName, assembly, customPrefix: customPrefix, cultureInfo: cultureInfo)
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

        public string ProvideDescription(T value)
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
        protected string ComputeKeyName(T value)
        {
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