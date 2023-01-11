using System;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// The ancestor interface for objects that can provide descriptions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDescriptionAncestor
    {
        /// <summary>
        /// Check if this interface can convert the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanConvertType(Type type);

        /// <summary>
        /// Gets the text loading preference for this object.
        /// </summary>
        TextLoadType LoadType { get; }

        /// <summary>
        /// Provide the description.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        string ProvideDescription(params object[] args);
    }
}