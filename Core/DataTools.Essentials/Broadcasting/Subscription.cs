using System;
using System.Threading.Tasks;

namespace DataTools.Essentials.Broadcasting
{
    /// <summary>
    /// Represents a subscription relationship between a subscriber and a broadcaster
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Subscription<T> : ISubscription<T>, IIdentifiable
    {
        /// <inheritdoc/>
        public event EventHandler Detach;

        private bool disposedValue;
        private WeakReference<IBroadcaster<T>> broadcaster;
        private WeakReference<ISubscriber<T>> subscriber;
        private ChannelToken channelToken;
        private string channelName;

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="channelToken"></param>
        /// <param name="subscriber"></param>
        /// <param name="broadcaster"></param>
        public Subscription(ChannelToken channelToken, ISubscriber<T> subscriber, IBroadcaster<T> broadcaster) : this(subscriber, broadcaster)
        {
            this.channelToken = channelToken;
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="subscriber"></param>
        /// <param name="broadcaster"></param>
        public Subscription(string channelName, ISubscriber<T> subscriber, IBroadcaster<T> broadcaster) : this(subscriber, broadcaster)
        {
            channelToken = ChannelToken.CreateToken(channelName);
            this.channelName = channelName;
        }

        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="broadcaster"></param>
        public Subscription(ISubscriber<T> subscriber, IBroadcaster<T> broadcaster)
        {
            this.subscriber = new WeakReference<ISubscriber<T>>(subscriber);
            this.broadcaster = new WeakReference<IBroadcaster<T>>(broadcaster);

            channelToken = ChannelToken.CreateToken(subscriber.GetHashCode().ToString() + broadcaster.GetHashCode().ToString());
        }

        /// <inheritdoc/>
        public ChannelToken ChannelToken => channelToken;

        /// <summary>
        /// Gets the channel name, if any, otherwise null
        /// </summary>
        public string ChannelName => channelName;

        string IIdentifiable.Name => channelName;

        /// <inheritdoc/>
        public bool TryGetBroadcaster(out IBroadcaster<T> broadcaster)
        {
            return this.broadcaster.TryGetTarget(out broadcaster);
        }

        /// <inheritdoc/>
        public bool TryGetSubscriber(out ISubscriber<T> subscriber)
        {
            return this.subscriber.TryGetTarget(out subscriber);
        }

        /// <inheritdoc/>
        public bool Validate()
        {
            return !disposedValue && (broadcaster?.TryGetTarget(out _) ?? false) && (subscriber?.TryGetTarget(out _) ?? false);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (broadcaster.TryGetTarget(out var ib) && ib is Broadcaster<T> b)
                    {
                        b.SubDetach(this, EventArgs.Empty);
                    }

                    Task.Run(() => Detach?.Invoke(this, EventArgs.Empty));

                    broadcaster = null;
                    subscriber = null;
                }

                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        bool ISubscription.TryGetBroadcaster(out IBroadcaster broadcaster)
        {
            var r = TryGetBroadcaster(out var b);
            broadcaster = b;
            return r;
        }

        bool ISubscription.TryGetSubscriber(out ISubscriber subscriber)
        {
            var r = TryGetSubscriber(out var s);
            subscriber = s;
            return r;
        }
    }
}
