using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using System;
using System.Reflection;
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
        protected string callbackMethodName;
        protected MethodInfo mtd;

        /// <summary>
        /// Instantiate a new <see cref="CallbackDescriptionProvider{T}"/>
        /// </summary>
        /// <param name="callback">The provider method.</param>
        public CallbackDescriptionProvider(Func<T, string, string> callback) : base()
        {
            this.callback = callback;
        }

        /// <summary>
        /// Instantiate a new <see cref="CallbackDescriptionProvider{T}"/>
        /// </summary>
        /// <param name="callback">The provider method.</param>
        public CallbackDescriptionProvider(string callbackMethodName) : base()
        {
            this.callbackMethodName = callbackMethodName;
            mtd = typeof(T).GetMethod(callbackMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (!ValidateMethod(mtd)) throw new ArgumentException("Method " + callbackMethodName + " not found or has an incorrect signature");

            if (mtd.IsStatic)
            {
                callback = new Func<T, string, string>((value, propertyName) =>
                {
                    return (string)mtd.Invoke(null, new object[] { value, propertyName });
                });
            }
            else
            {
                callback = new Func<T, string, string>((value, propertyName) =>
                {
                    return (string)mtd.Invoke(value, new object[] { value, propertyName });
                });
            }
        }

        public override TextLoadType LoadType { get; } = TextLoadType.Immediate;

        public override string ProvidePropertyDescription(T value, string propertyName)
        {
            return callback(value, propertyName);
        }

        private bool ValidateMethod(MethodInfo method)
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