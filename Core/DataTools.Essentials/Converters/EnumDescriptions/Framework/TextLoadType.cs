namespace DataTools.Essentials.Converters
{
    /// <summary>
    /// Specify how descriptive text should be loaded
    /// </summary>
    public enum TextLoadType
    {
        /// <summary>
        /// There is no loading preference.
        /// </summary>
        NoPreference,

        /// <summary>
        /// Prefer to be loaded and resolved immediately.
        /// </summary>
        Immediate,

        /// <summary>
        /// Prefer loading and resolving on demand.
        /// </summary>
        Lazy
    }
}