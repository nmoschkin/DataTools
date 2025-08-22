using System;
using System.Collections;
using System.Collections.Generic;

namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// An object that is identifiable by name
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Get the name or title
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// An object that is identified by a <see cref="DataTools.Essentials.Broadcasting.ChannelToken"/> structure.
    /// </summary>
    public interface IChannelIdentifiable : IIdentifiable
    {
        /// <summary>
        /// Gets the channel token for this object
        /// </summary>
        ChannelToken ChannelToken { get; }
    }

    /// <summary>
    /// Implements methods that allow a broadcaster to handle subscribers
    /// </summary>
    public interface IBroadcaster : IChannelIdentifiable, IDisposable
    {
        /// <summary>
        /// Gets the expected type for this object
        /// </summary>
        /// <returns>A new subscription which contains a channel token.</returns>
        Type BroadcastObjectType { get; }
    }

    /// <summary>
    /// Implements methods that allow a broadcaster to handle subscribers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBroadcaster<T> : IBroadcaster
    {
        /// <summary>
        /// Subscribe to this broadcasting service
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <returns>A new subscription which contains a channel token.</returns>
        ISubscription<T> Subscribe(ISubscriber<T> subscriber);
    }

    /// <summary>
    /// Implements methods that allow a broadcaster to handle subscribers on different channels
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChannelBroadcaster<T> : IBroadcaster<T>
    {
        /// <summary>
        /// Subscribe to this broadcasting service on the specified channel
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="channelToken"></param>
        /// <returns></returns>
        ISubscription<T> Subscribe(ISubscriber<T> subscriber, ChannelToken channelToken);
    }

    /// <summary>
    /// Implements methods that allow a broadcaster to handle subscribers on multiple channels
    /// </summary>
    public interface IChannelCatalog
    {
        /// <summary>
        /// Gets a dictionary of channels and broadcasters
        /// </summary>
        /// <returns></returns>
        IDictionary<ChannelToken, IBroadcaster> GetChannels();

        /// <summary>
        /// Gets a dictionary of channels and broadcasters for the specified type
        /// </summary>
        /// <typeparam name="T">The type of object that is being broadcast</typeparam>
        /// <returns></returns>
        IDictionary<ChannelToken, IBroadcaster<T>> GetChannels<T>();

        /// <summary>
        /// Gets the broadcaster with the specified channel token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IBroadcaster this[ChannelToken token] { get; }

    }




}
