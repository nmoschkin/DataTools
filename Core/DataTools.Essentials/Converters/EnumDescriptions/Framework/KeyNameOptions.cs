namespace DataTools.Essentials.Converters
{
    /// <summary>
    /// Options for key name computation
    /// </summary>
    /// <remarks>
    /// <see cref="CustomPrefix"/> and <see cref="CustomSuffix"/> can be combined, but no other combinations are permitted.
    /// </remarks>
    public enum KeyNameOptions
    {
        /// <summary>
        /// There is no affixation
        /// </summary>
        NoAffix = 0,

        /// <summary>
        /// The type name is prefixed
        /// </summary>
        TypePrefix = 1,

        /// <summary>
        /// The full type name is prefixed
        /// </summary>
        FullTypePrefix = 2,

        /// <summary>
        /// There is a custom prefix
        /// </summary>
        CustomPrefix = 3,

        /// <summary>
        /// The type name is suffixed
        /// </summary>
        TypeSuffix = 4,

        /// <summary>
        /// The full type name is suffixed
        /// </summary>
        FullTypeSuffix = 0x10,

        /// <summary>
        /// There is a custom suffix
        /// </summary>
        CustomSuffix = 0x20,

        /// <summary>
        /// The resource keys are provided explicitly
        /// </summary>
        Explicit = 0xff
    }
}