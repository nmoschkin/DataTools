using DataTools.Essentials.Converters.ClassDescriptions.Framework;
using DataTools.Essentials.Observable;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DataTools.Essentials.Converters.EnumDescriptions.Framework
{
    /// <summary>
    /// Provides a mutable, observable way to provide descriptive text for enums.
    /// </summary>
    public class DescribedEnum : ObservableBase
    {
        private Enum value;
        private Type enumType;
        private string _description;
        private IDescriptionAncestor _descriptionProvider;

        /// <summary>
        /// Create a new mutable, observable <see cref="Enum"/> described enum.
        /// </summary>
        /// <param name="enumType">The type of the enum</param>
        /// <param name="descriptionProvider">The optional description provider.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// If <paramref name="descriptionProvider"/> is null, the default <see cref="IEnumDescriptionProvider"/> for the enum is retrieved.
        /// </remarks>
        public DescribedEnum(Type enumType, IEnumDescriptionProvider descriptionProvider = null)
        {
            if (!enumType.IsEnum) throw new ArgumentException("Must be enum");

            _descriptionProvider = descriptionProvider ?? EnumInfo.ResolveDefaultProvider(enumType);

            var fif = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).First();
            Value = (Enum)fif.GetValue(null);
        }

        /// <summary>
        /// Create a new mutable, observable <see cref="Enum"/> described enum.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <param name="descriptionProvider">The optional description provider.</param>
        /// <remarks>
        /// If <paramref name="descriptionProvider"/> is null, the default <see cref="IEnumDescriptionProvider"/> for the enum is retrieved.
        /// </remarks>
        public DescribedEnum(Enum value, IEnumDescriptionProvider descriptionProvider = null)
        {
            _descriptionProvider = descriptionProvider ?? EnumInfo.ResolveDefaultProvider(enumType);
            Value = value;
        }

        /// <summary>
        /// Create a new mutable, observable <see cref="Enum"/> described enum.
        /// </summary>
        /// <param name="descriptionProvider">The optional description provider.</param>
        /// <remarks>
        /// If <paramref name="descriptionProvider"/> is null, the default <see cref="IEnumDescriptionProvider"/> for the enum is retrieved.
        /// </remarks>
        public DescribedEnum(IEnumDescriptionProvider descriptionProvider = null)
        {
            this.enumType = value.GetType();
            _descriptionProvider = descriptionProvider ?? EnumInfo.ResolveDefaultProvider(enumType);
        }

        /// <summary>
        /// Gets the enum type for this instance. This is automatically detected.
        /// </summary>
        public Type EnumType => enumType;

        /// <summary>
        /// Gets or sets the value for this enum.
        /// </summary>
        /// <remarks>
        /// This property is mutable and observable.
        /// </remarks>
        public Enum Value
        {
            get => value;
            set
            {
                if (SetProperty(ref this.value, value))
                {
                    if (value == null)
                    {
                        _description = null;
                        enumType = null;
                        OnPropertyChanged(nameof(Description));
                        OnPropertyChanged(nameof(EnumType));

                        return;
                    }

                    if (enumType != value.GetType())
                    {
                        enumType = value.GetType();
                        OnPropertyChanged(nameof(EnumType));
                    }

                    if (_descriptionProvider.LoadType != TextLoadType.Lazy)
                    {
                        _description = _descriptionProvider.ProvideDescription(value);
                    }
                    else
                    {
                        _description = null;
                    }

                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// Gets or sets the description for this instance
        /// </summary>
        /// <remarks>
        /// If the resolved <see cref="IEnumDescriptionProvider{T}"/> is lazy loaded, it will be invoked, at this time.
        /// <br /><br />
        /// This property is mutable and observable.
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
                        return _descriptionProvider.ProvideDescription(value);
                    }
                    else
                    {
                        _description = _descriptionProvider.ProvideDescription(value);
                    }
                }

                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
            }
        }
    }

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
            _defaultProvider = EnumInfo.ResolveDefaultProvider<T>();
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
                    _defaultProvider = EnumInfo.ResolveDefaultProvider<T>();
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

        /// <summary>
        /// Cast an enum of type <typeparamref name="T"/> to a <see cref="DescribedEnum"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator DescribedEnum<T>(T value)
        {
            return new DescribedEnum<T>(value);
        }

        /// <summary>
        /// Cast a <see cref="DescribedEnum"/> to an enum of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="descriptor"></param>
        public static implicit operator T(DescribedEnum<T> descriptor)
        {
            return descriptor._value;
        }

        /// <summary>
        /// Return true if enum values are not equal
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(DescribedEnum<T> lhs, DescribedEnum<T> rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Return true if enum values are not equal
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(T lhs, DescribedEnum<T> rhs)
        {
            return !lhs.Equals(rhs._value);
        }

        /// <summary>
        /// Returns true if enum values are not equal
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(DescribedEnum<T> lhs, T rhs)
        {
            return !lhs._value.Equals(rhs);
        }

        /// <summary>
        /// Returns true if enum values are equal
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(DescribedEnum<T> lhs, DescribedEnum<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Returns true if enum values are equal
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(T lhs, DescribedEnum<T> rhs)
        {
            return lhs.Equals(rhs._value);
        }

        /// <summary>
        /// Returns true if enum values are equal
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(DescribedEnum<T> lhs, T rhs)
        {
            return lhs._value.Equals(rhs);
        }

        /// <summary>
        /// Returns true if enum values are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DescribedEnum<T> other)
        {
            return _value.Equals(other._value) && _descriptionProvider == other._descriptionProvider && _description == other._description;
        }

        /// <inheritdoc/>
        public bool Equals(T other)
        {
            return _value.Equals(other);
        }

#if NET5_0_OR_GREATER
        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object obj)
#else

        /// <inheritdoc/>
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
    }
}