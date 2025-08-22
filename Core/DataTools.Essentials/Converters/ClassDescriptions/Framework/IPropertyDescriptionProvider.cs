namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// A description provider that can provide descriptions or manage description providers for its properties
    /// </summary>
    public interface IPropertyDescriptionProvider : IDescriptionAncestor
    {
        /// <summary>
        /// Provide the description for the given property in the given object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string ProvidePropertyDescription(object value, string propertyName);
    }

    /// <summary>
    /// A description provider that can provide descriptions or manage description providers for its properties
    /// </summary>
    public interface IPropertyDescriptionProvider<T> : IPropertyDescriptionProvider
    {
        /// <summary>
        /// Provide the description for the given property in the given object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string ProvidePropertyDescription(T value, string propertyName);
    }
}