namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Data transmission invocation types
    /// </summary>
    public enum InvocationType
    {
        /// <summary>
        /// Invoke data transmission synchronously
        /// </summary>
        Synchronous,

        /// <summary>
        /// Invoke data transmission in parallel
        /// </summary>
        Parallel,

        /// <summary>
        /// Invoke data transmission asynchronously
        /// </summary>
        NoWait,

        /// <summary>
        /// Invoke data transmission on the dispatcher or main thread
        /// </summary>
        Dispatcher
    }
}
