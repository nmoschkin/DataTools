using DataTools.Essentials.Converters.EnumDescriptions.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions
{
    /// <summary>
    /// A class for providing an explicit dictionary of descriptions for individual enum values and/or flags combinations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExplicitDescriptionProvider<T> : EnumDescriptionProviderBase<T> where T : struct, Enum
    {
        private readonly Dictionary<T, string> descriptions = new Dictionary<T, string>();

        /// <summary>
        /// Gets the text loading preference for this object.
        /// </summary>
        public TextLoadType LoadType => loadType;

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        public IReadOnlyDictionary<T, string> Descriptions
        {
            get => descriptions;
        }

        /// <summary>
        /// Get a description by key value.
        /// </summary>
        public virtual string this[T key] => descriptions[key];

        /// <summary>
        /// Instantiate a new explicit description provider from a key/value source.
        /// </summary>
        /// <param name="descriptions"></param>
        /// <remarks>
        /// The source <paramref name="descriptions"/> must contain exactly one description for each unique field in the <see cref="Enum"/> type.<br /><br />
        /// Flag combinations are not checked. It's up to the consumer to provide the correct values for expected flag combinations.
        /// </remarks>
        public ExplicitDescriptionProvider(IEnumerable<KeyValuePair<T, string>> descriptions)
        {
            loadType = TextLoadType.Immediate;

            foreach (var kv in descriptions)
            {
                this.descriptions.Add(kv.Key, kv.Value);
            }

            CheckSource(this.descriptions);
        }

        /// <summary>
        /// Verify that the description source contains all necessary entries.
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <remarks>
        /// A <see cref="KeyNotFoundException"/> exception is thrown if the source does not fulfill the criteria.
        /// </remarks>
        internal static void CheckSource(Dictionary<T, string> descriptions)
        {
            var fi = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var f in fi)
            {
                T obj = (T)f.GetValue(null);
                if (!descriptions.ContainsKey(obj)) throw new KeyNotFoundException($"Must provide a description for '{obj}'");
            }
        }

        public override string ProvideDescription(T value)
        {
            descriptions.TryGetValue(value, out var description);
            return description ?? value.ToString();
        }
    }
}