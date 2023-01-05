using System;
using System.Text;

namespace DataTools.Essentials.Converters
{
    /// <summary>
    /// An interface for an object that can provide descriptions for enum values of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnumDescriptionProvider
        <T> where T : struct, Enum
    {
        /// <summary>
        /// Gets the text loading preference for this object.
        /// </summary>
        TextLoadType LoadType { get; }

        /// <summary>
        /// Provide the description for the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ProvideDescription(T value);
    }
}