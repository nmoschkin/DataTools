using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions
{
    /// <summary>
    /// Provide <see cref="Enum"/> descriptions on-demand using a consumer-provided callback method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CallbackDescriptionProvider<T> : PropertyDescriptionProviderBase<T>
    {
        protected Func<T, string, string> callback;

        /// <summary>
        /// Instantiate a new <see cref="CallbackDescriptionProvider{T}"/>
        /// </summary>
        /// <param name="callback">The provider method.</param>
        public CallbackDescriptionProvider(Func<T, string, string> callback) : base()
        {
            this.callback = callback;
        }

        public override TextLoadType LoadType { get; } = TextLoadType.Immediate;

        public override string ProvidePropertyDescription(T value, string propertyName)
        {
            return callback(value, propertyName);
        }
    }
}