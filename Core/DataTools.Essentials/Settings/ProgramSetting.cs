using System;

namespace DataTools.Essentials.Settings
{
    /// <summary>
    /// A program setting
    /// </summary>
    public class ProgramSetting : ISetting
    {
        private readonly WeakReference<ISettings> parent = new WeakReference<ISettings>(null);

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        /// <param name="value">The value for the new setting</param>
        /// <param name="parent">The parent object</param>
        public ProgramSetting(string key, object value, ISettings parent) 
        {
            if (Key ==null) throw new ArgumentNullException("key");

            Key = key;
            Value = value;

            if (parent != null)
            {
                this.parent.SetTarget(parent);
            }
        }

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        /// <param name="value">The value for the new setting</param>
        public ProgramSetting(string key, object value) : this(key, value, null)
        {
        }

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        /// <param name="parent">The parent object</param>
        public ProgramSetting(string key, ISettings parent) : this(key, null, parent)
        {
        }

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        public ProgramSetting(string key) : this(key, null, null) 
        {
        }

        /// <inheritdoc/>
        public virtual string Key { get; protected set; }

        /// <inheritdoc/>
        public virtual object Value { get; set; } = null;

        /// <inheritdoc/>
        public virtual bool SupportsTryGetParent => true;

        /// <inheritdoc/>
        public virtual bool TryGetParent(out ISettings parent)
        {
            return this.parent.TryGetTarget(out parent);
        }
    }

    /// <summary>
    /// A strongly-typed program setting
    /// </summary>
    public class ProgramSetting<T> : ISetting<T>
    {
        private readonly WeakReference<ISettings> parent = new WeakReference<ISettings>(null);
        private T value;

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        /// <param name="value">The value for the new setting</param>
        /// <param name="parent">The parent object</param>
        public ProgramSetting(string key, T value, ISettings parent)
        {
            if (Key == null) throw new ArgumentNullException("key");

            Key = key;
            Value = value;

            if (parent != null)
            {
                this.parent.SetTarget(parent);
            }
        }

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        /// <param name="value">The value for the new setting</param>
        public ProgramSetting(string key, T value) : this(key, value, null)
        {
        }

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        /// <param name="parent">The parent object</param>
        public ProgramSetting(string key, ISettings parent) : this(key, default, parent)
        {
        }

        /// <summary>
        /// Create a new program setting
        /// </summary>
        /// <param name="key">The key for the new setting</param>
        public ProgramSetting(string key) : this(key, default, null)
        {
        }


        /// <inheritdoc/>
        public virtual string Key { get; protected set; }


        /// <inheritdoc/>
        public virtual T Value
        {
            get => value;
            set
            {
                this.value = value;
            }
        }

        object ISetting.Value
        {
            get => value;
            set => this.value = (T)value;
        }

        /// <inheritdoc/>
        public virtual bool SupportsTryGetParent => true;

        /// <inheritdoc/>
        public virtual bool TryGetParent(out ISettings parent)
        {
            return this.parent.TryGetTarget(out parent);
        }
    }

}
