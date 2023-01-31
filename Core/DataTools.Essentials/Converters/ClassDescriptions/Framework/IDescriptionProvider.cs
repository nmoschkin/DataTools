using System;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// An interface for an object that can provide descriptions for object properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDescriptionProvider : IDescriptionAncestor
    {
        /// <summary>
        /// Provide the description for the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ProvideDescription(object value);
    }

    /// <summary>
    /// An interface for an object that can provide descriptions for object properties of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDescriptionProvider<T> : IDescriptionProvider
    {
        /// <summary>
        /// Provide the description for the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ProvideDescription(T value);
    }
}