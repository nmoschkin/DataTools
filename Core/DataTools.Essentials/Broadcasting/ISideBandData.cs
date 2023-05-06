namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Sideband data
    /// </summary>
    public interface ISideBandData
    {
        /// <summary>
        /// Gets the invocation type for this push notification
        /// </summary>
        InvocationType InvocationType { get; }

        /// <summary>
        /// Gets the channel token for this push notification
        /// </summary>
        ChannelToken ChannelToken { get; }
    }
}
