using DataTools.Shell.Native;

using System.Runtime.InteropServices;

namespace DataTools.Desktop
{
    public interface IPropertyBag
    {
        /// <summary>
        /// Returns the metadata proprerty store for this shell object.
        /// </summary>
        /// <returns></returns>
        public IList<PropertyKey> GetPropertyStore();
        /// <summary>
        /// Gets the metadata property with the specified key as the specified type (if available)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetPropertyValue<T>(string key);

        /// <summary>
        /// Gets the metadata property with the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetPropertyValue(string key);
        /// <summary>
        /// Gets the metadata property with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetPropertyValue(PropertyKey key);

        /// <summary>
        /// Get a dictionary that lists property names along with their key structures.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PropertyKey> GetPropertiesWithNames();

        /// <summary>
        /// Set a metadata property with the specified key to the specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPropertyValue<T>(string key, T value);
        /// <summary>
        /// Set a metadata property with the specified key to the specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPropertyValue(string key, object value);

        /// <summary>
        /// Set a metadata property with the specified key to the specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPropertyValue(PropertyKey key, object value);

        /// <summary>
        /// Set multiple metadata properties at once
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SetPropertyValues(Dictionary<string, object> values);
        /// <summary>
        /// Set multiple metadata properties at once
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SetPropertyValues(Dictionary<PropertyKey, object> values);
    }
}