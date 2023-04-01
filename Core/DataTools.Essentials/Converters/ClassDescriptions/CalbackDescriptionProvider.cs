using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Reflection;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions
{
    /// <summary>
    /// Provide property descriptions on-demand using a consumer-provided callback method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CallbackDescriptionProvider<T> : PropertyDescriptionProviderBase<T>
    {
        
        /// <summary>
        /// The callback method
        /// </summary>
        protected Func<T, string, string> callback;

        /// <summary>
        /// The calback method name
        /// </summary>
        protected string callbackMethodName;

        /// <summary>
        /// The callback method reflection information
        /// </summary>
        protected MethodInfo method;

        /// <summary>
        /// Instantiate a new <see cref="CallbackDescriptionProvider{T}"/>
        /// </summary>
        /// <param name="callback">The provider method.</param>
        /// <remarks>
        /// The callback takes <typeparamref name="T"/> and <see cref="string"/> as an argument, where <see cref="string"/> is the property name.<br />
        /// The return value for the callback is the description <see cref="string"/>.
        /// </remarks>
        public CallbackDescriptionProvider(Func<T, string, string> callback) : base()
        {
            this.callback = callback;
        }

        /// <summary>
        /// Instantiate a new <see cref="CallbackDescriptionProvider{T}"/>
        /// </summary>
        /// <param name="callbackMethodName">The name of the callback method in <typeparamref name="T"/>.</param>
        /// <remarks>
        /// The callback takes <typeparamref name="T"/> and <see cref="string"/> as an argument, where <see cref="string"/> is the property name.<br />
        /// The return value for the callback is the description <see cref="string"/>.
        /// </remarks>
        public CallbackDescriptionProvider(string callbackMethodName) : this(callbackMethodName, null) { }

        /// <summary>
        /// Instantiate a new <see cref="CallbackDescriptionProvider{T}"/>
        /// </summary>
        /// <param name="callbackMethodName">The name of the callback method in <paramref name="methodType"/>.</param>
        /// <param name="methodType">The type in which the <paramref name="callbackMethodName"/> is located.</param>
        /// <remarks>
        /// The callback takes <typeparamref name="T"/> and <see cref="string"/> as an argument, where <see cref="string"/> is the property name.<br />
        /// The return value for the callback is the description <see cref="string"/>.
        /// </remarks>
        public CallbackDescriptionProvider(string callbackMethodName, Type methodType) : base()
        {
            this.callbackMethodName = callbackMethodName;
            method = (methodType ?? typeof(T)).GetMethod(callbackMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (!ValidateMethod(method)) throw new ArgumentException("Method " + callbackMethodName + " not found or has an incorrect signature");

            if (method.IsStatic)
            {
                callback = new Func<T, string, string>((value, propertyName) =>
                {
                    return (string)method.Invoke(null, new object[] { value, propertyName });
                });
            }
            else
            {
                callback = new Func<T, string, string>((value, propertyName) =>
                {
                    return (string)method.Invoke(value, new object[] { value, propertyName });
                });
            }
        }

        /// <inheritdoc/>
        public override TextLoadType LoadType { get; } = TextLoadType.Immediate;

        /// <inheritdoc/>
        public override string ProvidePropertyDescription(T value, string propertyName)
        {
            return callback(value, propertyName);
        }

        /// <summary>
        /// Validate that the specified method meets the criteria for the callback.
        /// </summary>
        /// <param name="method">The method to validate</param>
        /// <returns>True if the method meets the criteria.</returns>
        protected virtual bool ValidateMethod(MethodInfo method)
        {
            if (method == null) return false;

            if (method.ReturnType != typeof(string)) return false;

            var p = method.GetParameters();
            int i = 0;
            foreach (var pr in p)
            {
                if (i == 0)
                {
                    if (pr.ParameterType != typeof(T)) return false;
                }
                else if (i == 1)
                {
                    if (pr.ParameterType != typeof(string)) return false;
                }
                else
                {
                    return false;
                }
                i++;
            }

            return true;
        }
    }
}