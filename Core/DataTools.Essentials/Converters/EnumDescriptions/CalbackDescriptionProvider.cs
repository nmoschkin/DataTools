using DataTools.Essentials.Converters.EnumDescriptions.Framework;

using System;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions
{
    /// <summary>
    /// Provide <see cref="Enum"/> descriptions on-demand using a consumer-provided callback method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CallbackDescriptionProvider<T> : EnumDescriptionProviderBase<T> where T : struct, Enum
    {
        
        /// <summary>
        /// Callback method
        /// </summary>
        protected Func<T, string> callback;

        /// <summary>
        /// Callback method
        /// </summary>
        public virtual Func<T, string> Callback => callback;

        /// <summary>
        /// Instantiate a new <see cref="CallbackDescriptionProvider{T}"/>
        /// </summary>
        /// <param name="callback">The provider method.</param>
        public CallbackDescriptionProvider(Func<T, string> callback) : base()
        {
            this.callback = callback;
        }

        /// <inheritdoc/>
        public override string ProvideDescription(T value)
        {
            return callback(value);
        }
    }
}