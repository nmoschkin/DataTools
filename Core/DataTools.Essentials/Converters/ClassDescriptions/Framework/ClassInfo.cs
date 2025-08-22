using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataTools.Essentials.Converters.ClassDescriptions.Framework
{
    /// <summary>
    /// Static utility class for interacting with the description framework
    /// </summary>
    public static class ClassInfo
    {
        /// <summary>
        /// Resolves the name of the property from <see cref="DescriptionAttribute"/> or <see cref="JsonPropertyAttribute"/>.
        /// </summary>
        /// <param name="obj">The object whose property we want to retrieve a description for.</param>
        /// <param name="propertyName">The name of the project to retrieve from <paramref name="obj"/>.</param>
        /// <returns>A description or the property name if no suitable way to get a description was found.</returns>
        public static string GetPropertyDescription(object obj, string propertyName)
        {
            var pi = obj.GetType().GetProperty(propertyName);
            var jp = pi.GetCustomAttribute<JsonPropertyAttribute>();
            var de = pi.GetCustomAttribute<DescriptionAttribute>();

            if (de != null)
            {
                return de.Description;
            }

            if (jp != null)
            {
                return jp.PropertyName;
            }

            return pi.Name;
        }

        /// <summary>
        /// Resolves the description provider for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="explicitOnly">Return a description provider only if the class explicitly has one.</param>
        /// <returns>An object that implements <see cref="IDescriptionAncestor"/> or null if that was not possible.</returns>
        public static IDescriptionAncestor ResolveProvider(Type type, bool explicitOnly = false)
        {
            var attr = type.GetCustomAttributes(true)?
                .Where(x => x is DescriptionProviderAttribute)?
                .Select(x => x as DescriptionProviderAttribute)?
                .FirstOrDefault();

            if (attr != null)
            {
                return attr.CreateInstance();
            }

            if (explicitOnly) return null;

            return new ClassPropertyDescriptionProvider(type);
        }

        /// <summary>
        /// Resolve the description provider for the given property in the specified type.
        /// </summary>
        /// <param name="type">The object whose property we want to retrieve a description for.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="parentProvider">The optional parent provider that can serve as a fallback.</param>
        /// <param name="createProviderFromType">True to attempt to create the parent fallback provider.</param>
        /// <param name="explicitOnly">Return a description provider only if the property explictly has one, or the parent provider can be resolved.</param>
        /// <returns>A <see cref="IDescriptionAncestor"/> implementation.</returns>
        public static IDescriptionAncestor ResolveProvider(Type type, string propertyName, IDescriptionAncestor parentProvider = null, bool createProviderFromType = false, bool explicitOnly = false)
        {
            if (createProviderFromType && parentProvider == null)
            {
                var attr = type.GetCustomAttributes(true)?
                  .Where(x => x is DescriptionProviderAttribute)?
                  .Select(x => x as DescriptionProviderAttribute)?
                  .FirstOrDefault();

                if (attr != null)
                {
                    parentProvider = attr.CreateInstance();
                }
            }

            var prop = type.GetProperty(propertyName);
            return ResolveProvider(prop, parentProvider, explicitOnly);
        }

        /// <summary>
        /// Resolve the description provider for the specified property.
        /// </summary>
        /// <param name="prop">The property to retrieve a description for.</param>
        /// <param name="parentProvider">The optional parent provider that can serve as a fallback.</param>
        /// <param name="explicitOnly">Return a description provider only if the property explictly has one.</param>
        /// <returns>A <see cref="IDescriptionAncestor"/> implementation.</returns>
        public static IDescriptionAncestor ResolveProvider(PropertyInfo prop, IDescriptionAncestor parentProvider = null, bool explicitOnly = false)
        {
            var attr = prop.GetCustomAttributes(true)?
               .Where(x => x is DescriptionProviderAttribute)?
               .Select(x => x as DescriptionProviderAttribute)?
               .FirstOrDefault();

            if (attr != null)
            {
                if (attr.ProviderType.GetInterface(nameof(IDescriptionProvider)) != null)
                {
                    return attr.CreateInstance() as IDescriptionProvider;
                }
                else if (parentProvider is IPropertyDescriptionProvider ppd)
                {
                    return ppd;
                }
                else if (attr.CreateInstance() is IDescriptionAncestor pro && (pro.CanConvertType(prop.PropertyType) || pro.CanConvertType(prop.DeclaringType)))
                {
                    if (pro is IDescriptionAncestor defpro)
                    {
                        return defpro;
                    }
                }
            }

            if (explicitOnly && parentProvider == null) return null;
            return parentProvider ?? new AttributeDescriptionProvider(prop.Name);
        }

        /// <summary>
        /// Gets the description for the specified property from the specified object.
        /// </summary>
        /// <param name="obj">The target object</param>
        /// <param name="propertyName">The target property</param>
        /// <returns></returns>
        public static string GetDescription(object obj, string propertyName)
        {
            var dict = new Dictionary<string, string>();

            var provider = (ResolveProvider(obj.GetType()) as IPropertyDescriptionProvider);
            var prop = obj.GetType().GetProperty(propertyName);

            if (provider is IPropertyDescriptionProvider ppd)
            {
                var proprov = ResolveProvider(prop, explicitOnly: true);

                if (proprov != null)
                {
                    return proprov.ProvideDescription(prop.GetValue(obj), prop.Name);
                }
                else
                {
                    return ppd.ProvidePropertyDescription(obj, prop.Name);
                }
            }
            else if (provider != null)
            {
                return provider.ProvideDescription(obj);
            }
            else if (provider == null)
            {
                var proprov = ResolveProvider(prop);
                return proprov.ProvideDescription(prop.GetValue(obj));
            }

            return propertyName;
        }

        /// <summary>
        /// Gets all descriptions for all public properties in a class.
        /// </summary>
        /// <param name="obj">The object whose descriptions to fetch.</param>
        /// <param name="excludeProps">Any properties we wish to not fetch.</param>
        /// <param name="provider">If we already have a provider, we can use that, here.</param>
        /// <returns>A dictionary of property names and their descriptions.</returns>
        /// <remarks>
        /// This method resolves the <see cref="IDescriptionAncestor"/> for the class, as a whole, as well as for each property, if they are available.
        /// </remarks>
        public static Dictionary<string, string> GetDescriptions(object obj, string[] excludeProps = null, IPropertyDescriptionProvider provider = null)
        {
            var dict = new Dictionary<string, string>();

            var props = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => excludeProps == null || !excludeProps.Contains(x.Name))
                .ToList();

            provider = provider ?? (ResolveProvider(obj.GetType()) as IPropertyDescriptionProvider);

            if (provider is IPropertyDescriptionProvider ppd)
            {
                foreach (var prop in props)
                {
                    var proprov = ResolveProvider(prop, explicitOnly: true);

                    if (proprov != null)
                    {
                        dict.Add(prop.Name, proprov.ProvideDescription(prop.GetValue(obj), prop.Name));
                    }
                    else
                    {
                        dict.Add(prop.Name, ppd.ProvidePropertyDescription(obj, prop.Name));
                    }
                }
            }
            else if (provider != null)
            {
                dict.Add(obj.GetType().Name, provider.ProvideDescription(obj));
            }
            else if (provider == null)
            {
                foreach (var prop in props)
                {
                    var proprov = ResolveProvider(prop);
                    dict.Add(prop.Name, proprov.ProvideDescription(prop.GetValue(obj)));
                }
            }

            return dict;
        }
    }
}