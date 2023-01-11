using DataTools.Essentials.Converters.ClassDescriptions.Framework;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions.Framework
{
    public class EnumInfo
    {
        /// <summary>
        /// Resolves the default description provider for the specified type.
        /// </summary>
        /// <returns></returns>
        public static IEnumDescriptionProvider ResolveDefaultProvider(Type enumType)
        {
            var attr = enumType.GetCustomAttributes(true)?
                .Where(x => x is EnumDescriptionProviderAttribute)?
                .Select(x => x as EnumDescriptionProviderAttribute)?
                .FirstOrDefault();

            if (attr != null && attr.CreateInstance() is IEnumDescriptionProvider pro && pro.CanConvertType(enumType))
            {
                if (pro is IEnumDescriptionProvider defpro)
                {
                    return defpro;
                }
            }

            return new AttributeDescriptionProvider(enumType);
        }

        /// <summary>
        /// Resolves the default description provider for the specified type.
        /// </summary>
        /// <returns></returns>
        public static IEnumDescriptionProvider<T> ResolveDefaultProvider<T>() where T : struct, Enum
        {
            var attr = typeof(T).GetCustomAttributes(true)?
                 .Where(x => x is EnumDescriptionProviderAttribute)?
                 .Select(x => x as EnumDescriptionProviderAttribute)?
                 .FirstOrDefault();

            if (attr != null && attr.CreateInstance() is IEnumDescriptionProvider pro && pro.CanConvertType(typeof(T)))
            {
                if (pro is IEnumDescriptionProvider<T> defpro)
                {
                    return defpro;
                }
                else
                {
                    return new DefaultProviderWrapper<T>(pro);
                }
            }
            else
            {
                return new AttributeDescriptionProvider<T>();
            }
        }

        /// <summary>
        /// Resolves the name of the enum from various possible attributes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="preferDescription">The default is to prefer serialization names. Set this to true to prefer descriptive names.</param>
        /// <returns></returns>
        public static string GetEnumName(Enum obj, bool preferDescription = false)
        {
            var fis = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var fi in fis)
            {
                var sampleValue = fi.GetValue(null);

                if (sampleValue.Equals(obj))
                {
                    var ema = fi.GetCustomAttribute<EnumMemberAttribute>();

                    var jp = fi.GetCustomAttribute<JsonPropertyAttribute>();

                    var de = fi.GetCustomAttribute<DescriptionAttribute>();

                    if (preferDescription && de != null)
                    {
                        return de.Description;
                    }

                    if (ema != null)
                    {
                        return ema.Value;
                    }

                    if (jp != null)
                    {
                        return jp.PropertyName;
                    }

                    if (de != null)
                    {
                        return de.Description;
                    }

                    return fi.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// This is a class to wrap non-generic <see cref="IEnumDescriptionProvider"/> instances into their generic form.
        /// </summary>
        private class DefaultProviderWrapper<T> : IEnumDescriptionProvider<T> where T : struct, Enum
        {
            private IEnumDescriptionProvider baseprovider;

            public bool CanConvertType(Type enumType)
            {
                return enumType == typeof(T);
            }

            public DefaultProviderWrapper(IEnumDescriptionProvider baseprovider)
            {
                this.baseprovider = baseprovider;
            }

            public TextLoadType LoadType => baseprovider.LoadType;

            public string ProvideDescription(T value)
            {
                return baseprovider.ProvideDescription(value);
            }

            public string ProvideDescription(Enum value)
            {
                return baseprovider.ProvideDescription(value);
            }

            string IDescriptionAncestor.ProvideDescription(params object[] args) => ProvideDescription((Enum)args[0]);
        }
    }
}