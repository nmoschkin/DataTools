using System;

namespace DataTools.Essentials.Broadcasting
{

    /// <summary>
    /// Implements a subscription that ties subscriber and broadcaster together
    /// </summary>
    public interface ISubscription : IDisposable
    {
        /// <summary>
        /// Fired when the subscription wishes to be detached
        /// </summary>
        event EventHandler Detach;

        /// <summary>
        /// Try to get the broadcaster associated with this subscription
        /// </summary>
        /// <param name="broadcaster"></param>
        /// <returns></returns>
        bool TryGetBroadcaster(out IBroadcaster broadcaster);

        /// <summary>
        /// Try to get the subscriber associated with this subscription
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        bool TryGetSubscriber(out ISubscriber subscriber);

        /// <summary>
        /// Gets the channel token associated with this subscription
        /// </summary>
        ChannelToken ChannelToken { get; }

        /// <summary>
        /// Checks if the channel is open and functioning normally.
        /// </summary>
        /// <returns>
        /// True if the channel is open and functioning normally, otherwise false.
        /// </returns>
        bool Validate();
    }

    /// <summary>
    /// Implements a subscription that ties subscriber and broadcaster together
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISubscription<T> : ISubscription
    {
        /// <summary>
        /// Try to get the broadcaster associated with this subscription
        /// </summary>
        /// <param name="broadcaster"></param>
        /// <returns></returns>
        bool TryGetBroadcaster(out IBroadcaster<T> broadcaster);

        /// <summary>
        /// Try to get the subscriber associated with this subscription
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        bool TryGetSubscriber(out ISubscriber<T> subscriber);
    }
}
