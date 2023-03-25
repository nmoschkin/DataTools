using DataTools.Essentials.Settings;

namespace DataTools.Windows.Essentials.Settings
{
    /// <summary>
    /// Represents a registry setting
    /// </summary>
    public class RegistrySetting : ProgramSetting
    {

        /// <inheritdoc/>
        public RegistrySetting(string key, object value, ISettings parent) : base(key, value, parent)
        {
        }

        /// <inheritdoc/>
        public override object Value
        {
            get
            {
                if (TryGetParent(out var parent))
                {
                    base.Value = parent.GetValue<object>(Key);
                }
                return base.Value;
            }
            set
            {
                base.Value = value;

                if (TryGetParent(out var parent))
                {
                    parent.SetSetting(this);
                }
            }
        }
    }

    // <summary>
    /// Represents a registry setting
    /// </summary>
    public class RegistrySetting<T> : ProgramSetting<T>
    {

        /// <inheritdoc/>
        public RegistrySetting(string key, T value, ISettings parent) : base(key, value, parent)
        {
        }

        /// <inheritdoc/>
        public override T Value
        {
            get
            {
                if (base.Value == null)
                {
                    if (TryGetParent(out var parent))
                    {
                        base.Value = parent.GetValue<T>(Key);
                    }
                }

                return base.Value;
            }
            set
            {
                base.Value = value;

                if (TryGetParent(out var parent))
                {
                    parent.SetValue(Key, base.Value);
                }
            }
        }

    }
}
