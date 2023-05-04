using DataTools.Essentials.Observable;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Essentials.Broadcasting
{

    /// <summary>
    /// Base class for broadcasters
    /// </summary>
    public abstract class Broadcaster<T> : ObservableBase, IBroadcaster<T>
    {
        private object lockObj = new object();
        private bool disposedValue;
        private readonly List<ISubscription<T>> subscriptions = new List<ISubscription<T>>();

        /// <summary>
        /// Gets or sets the name of the broadcaster
        /// </summary>
        public virtual string Name { get; set; }

        /// <inheritdoc/>
        public virtual ChannelToken ChannelToken { get; protected set; } = ChannelToken.CreateToken();

        /// <summary>
        /// Create a new broadcaster
        /// </summary>
        public Broadcaster()
        {
            Synchronizer.Initialize();
        }

        /// <summary>
        /// Create a new subscription for the specified subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <returns>A new subscription</returns>
        /// <remarks>
        /// This method will always create a new subscription, even if the specified subscriber has a previous subscription.
        /// </remarks>
        public ISubscription<T> Subscribe(ISubscriber<T> subscriber)
        {
            return Subscribe(subscriber, false);
        }

        ISubscription IBroadcaster.Subscribe(ISubscriber subscriber)
        {
            if (subscriber is ISubscriber<T> valid)
            {
                return Subscribe(valid);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Create a new subscription for the specified subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <param name="returnExisting">True to return any existing subscription for the specified subscriber instead of creating a new one</param>
        /// <returns>A new subscription</returns>
        public ISubscription<T> Subscribe(ISubscriber<T> subscriber, bool returnExisting)
        {
            lock (lockObj)
            {
                if (returnExisting)
                {
                    foreach (var item in subscriptions)
                    {
                        if (item.TryGetSubscriber(out var subtest) && subtest == subscriber)
                        {
                            return item;
                        }
                    }
                }

                var sub = new Subscription<T>(subscriber, this);

                subscriptions.Add(sub);
                return sub;
            }
        }

        internal void SubDetach(object sender, EventArgs e)
        {
            if (sender is Subscription<T> sub)
            {
                lock(lockObj)
                {
                    subscriptions.Remove(sub);
                }
            }
        }

        /// <summary>
        /// Create the sideband data necessary for this push
        /// </summary>
        /// <param name="data">The data being pushed</param>
        /// <param name="sub">The target subscriber</param>
        /// <param name="invocationType"></param>
        /// <returns></returns>
        protected virtual ISideBandData CreateSideBandData(T data, ISubscription<T> sub, InvocationType invocationType)
        {
            return new SideBandData(invocationType, sub.ChannelToken);
        }

        /// <summary>
        /// Transmit the data
        /// </summary>
        /// <param name="data">The data to transmit</param>
        /// <param name="invocationType">The invocation type</param>
        protected void TransmitData(T data, InvocationType invocationType = InvocationType.Synchronous)
        {
            lock (lockObj)
            {

                if (invocationType == InvocationType.Parallel)
                {
                    Parallel.ForEach(subscriptions, (sub) =>
                    {
                        if (sub.TryGetSubscriber(out var subscriber))
                        {
                            var sbb = CreateSideBandData(data, sub, invocationType);
                            subscriber.ReceiveData(data, sbb);
                        }
                    });
                }
                else
                {
                    foreach (var sub in subscriptions)
                    {
                        if (sub.TryGetSubscriber(out var subscriber))
                        {
                            var sbb = CreateSideBandData(data, sub, invocationType);

                            switch (invocationType)
                            {
                                case InvocationType.Synchronous:
                                    subscriber.ReceiveData(data, sbb);
                                    break;

                                case InvocationType.Async:
                                    _ = Task.Run(() => subscriber.ReceiveData(data, sbb));
                                    break;

                                case InvocationType.Dispatcher:
                                    Synchronizer.Default.BeginInvoke(() =>
                                    {
                                        subscriber.ReceiveData(data, sbb);
                                    });
                                    break;
                            }

                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    subscriptions.Clear();                   
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
    }
}
