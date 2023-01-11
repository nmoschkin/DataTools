using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// Specifies an explicit resource lookup key name for translation
    /// </summary>
    public class TranslationKeyAttribute : Attribute
    {
        /// <summary>
        /// The resource lookup key name for the translation
        /// </summary>
        public string ResourceKey { get; }

        /// <summary>
        /// Specifies an explicit resource lookup key name for translation
        /// </summary>
        /// <param name="resourceKey">The resource lookup key name</param>
        public TranslationKeyAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }
    }
}