using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions.Framework
{
    /// <summary>
    /// Provides a durable way to provide descriptive text for enums.
    /// </summary>
    /// <typeparam name="T">The type of Enum to serve</typeparam>
    public struct DescribedEnum<T> : IEquatable<DescribedEnum<T>>, IEquatable<T> where T : struct, Enum
    {
        private static IEnumDescriptionProvider<T> _defaultProvider;
        private string _description;
        private IEnumDescriptionProvider<T> _descriptionProvider;
        private T _value;

        static DescribedEnum()
        {
            _defaultProvider = ResolveDefaultProvider();
        }

        /// <summary>
        /// Resolves the default description provider for the specified type.
        /// </summary>
        /// <returns></returns>
        public static IEnumDescriptionProvider<T> ResolveDefaultProvider()
        {
            var attr = typeof(T).GetCustomAttributes(true)?
                .Where(x => x is DescriptionProviderAttribute)?
                .Select(x => x as DescriptionProviderAttribute)?
                .FirstOrDefault();

            if (attr != null && attr.CreateInstance() is IEnumDescriptionProvider pro && pro.CanConvertType(typeof(T)))
            {
                if (pro is IEnumDescriptionProvider<T> defpro)
                {
                    return defpro;
                }
                else
                {
                    return new DefaultProviderWrapper(pro);
                }
            }
            else
            {
                return new AttributeDescriptionProvider<T>();
            }
        }

        public DescribedEnum(T value) : this(value, DefaultProvider)
        {
        }

        public DescribedEnum(T value, string description)
        {
            _value = value;
            _description = description;
            _descriptionProvider = null;
        }

        public DescribedEnum(T value, IEnumDescriptionProvider<T> descriptionProvider)
        {
            _value = value;
            _descriptionProvider = descriptionProvider;

            if (descriptionProvider.LoadType != TextLoadType.Lazy)
            {
                _description = descriptionProvider.ProvideDescription(_value);
            }
            else
            {
                _description = null;
            }
        }

        /// <summary>
        /// Gets or sets the default description provider for the current enum type <typeparamref name="T"/>.
        /// </summary>
        public static IEnumDescriptionProvider<T> DefaultProvider
        {
            get => _defaultProvider;
            set
            {
                if (value == null)
                {
                    _defaultProvider = ResolveDefaultProvider();
                }
                else
                {
                    _defaultProvider = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the description for this instance
        /// </summary>
        /// <remarks>
        /// If the resolved <see cref="IEnumDescriptionProvider{T}"/> is lazy loaded, it will be invoked, at this time.
        /// </remarks>
        public string Description
        {
            get
            {
                if (_description != null) return _description;

                if (_descriptionProvider != null)
                {
                    if (_descriptionProvider.LoadType == TextLoadType.Lazy)
                    {
                        return _descriptionProvider.ProvideDescription(_value);
                    }
                    else
                    {
                        _description = _descriptionProvider.ProvideDescription(_value);
                    }
                }

                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Gets the current enum value for this instance
        /// </summary>
        public T Value => _value;

        public static implicit operator DescribedEnum<T>(T value)
        {
            return new DescribedEnum<T>(value);
        }

        public static implicit operator T(DescribedEnum<T> descriptor)
        {
            return descriptor._value;
        }

        public static bool operator !=(DescribedEnum<T> lhs, DescribedEnum<T> rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static bool operator !=(T lhs, DescribedEnum<T> rhs)
        {
            return !lhs.Equals(rhs._value);
        }

        public static bool operator !=(DescribedEnum<T> lhs, T rhs)
        {
            return !lhs._value.Equals(rhs);
        }

        public static bool operator ==(DescribedEnum<T> lhs, DescribedEnum<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator ==(T lhs, DescribedEnum<T> rhs)
        {
            return lhs.Equals(rhs._value);
        }

        public static bool operator ==(DescribedEnum<T> lhs, T rhs)
        {
            return lhs._value.Equals(rhs);
        }

        public bool Equals(DescribedEnum<T> other)
        {
            return _value.Equals(other._value) && _descriptionProvider == other._descriptionProvider && _description == other._description;
        }

        public bool Equals(T other)
        {
            return _value.Equals(other);
        }

#if NET5_0_OR_GREATER
        public override bool Equals([NotNullWhen(true)] object obj)
#else

        public override bool Equals(object obj)
#endif
        {
            if (obj is T e) return Equals(e);
            if (obj is DescribedEnum<T> d) return Equals(d);
            return false;
        }

        public override int GetHashCode()
        {
            return (_value, _description, _descriptionProvider).GetHashCode();
        }

        public override string ToString()
        {
            return Description;
        }

        /// <summary>
        /// This is a class to wrap non-generic <see cref="IEnumDescriptionProvider"/> instances into their generic form.
        /// </summary>
        private class DefaultProviderWrapper : IEnumDescriptionProvider<T>
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
        }
    }
}