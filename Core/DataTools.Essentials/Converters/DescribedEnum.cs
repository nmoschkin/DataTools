using System;
using System.Text;

namespace DataTools.Essentials.Converters
{
    /// <summary>
    /// Provides a durable way to provide descriptive text for enums.
    /// </summary>
    /// <typeparam name="T">The type of Enum to serve</typeparam>
    public sealed class DescribedEnum<T> where T : struct, Enum
    {
        /// <summary>
        /// Gets the default description provider for the current enum type <typeparamref name="T"/>.
        /// </summary>
        public static IEnumDescriptionProvider<T> DefaultProvider { get; set; } = new AttributeDescriptionProvider<T>();

        private T _value;
        private IEnumDescriptionProvider<T> _descriptionProvider;
        private string _description;

        public T Value => _value;

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
        }

        public override string ToString()
        {
            return Description;
        }

        public DescribedEnum(T value) : this(value, DefaultProvider)
        {
        }

        public DescribedEnum(T value, string description)
        {
            _value = value;
            this._description = description;
        }

        public DescribedEnum(T value, IEnumDescriptionProvider<T> descriptionProvider)
        {
            _value = value;
            _descriptionProvider = descriptionProvider;

            if (descriptionProvider.LoadType != TextLoadType.Lazy)
            {
                _description = descriptionProvider.ProvideDescription(_value);
            }
        }

        public static implicit operator T(DescribedEnum<T> descriptor)
        {
            return descriptor._value;
        }

        public static implicit operator DescribedEnum<T>(T value)
        {
            return new DescribedEnum<T>(value);
        }
    }
}