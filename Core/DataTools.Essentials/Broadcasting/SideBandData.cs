namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Sideband data
    /// </summary>
    public struct SideBandData : ISideBandData
    {
        /// <inheritdoc/>
        public InvocationType InvocationType { get; set; }

        /// <inheritdoc/>
        public ChannelToken ChannelToken { get; set; }

        /// <summary>
        /// Create a new sideband data instance
        /// </summary>
        /// <param name="invocationType"></param>
        /// <param name="channelToken"></param>
        public SideBandData(InvocationType invocationType, ChannelToken channelToken) : this(channelToken)
        {
            InvocationType = invocationType;
        }

        /// <summary>
        /// Create a new sideband data instance
        /// </summary>
        /// <param name="channelToken"></param>
        public SideBandData(ChannelToken channelToken)
        {
            ChannelToken = channelToken;
            InvocationType = InvocationType.Synchronous;
        }

    }
}
