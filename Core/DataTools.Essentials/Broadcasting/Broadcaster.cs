using DataTools.Essentials.Observable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DataTools.Essentials.Broadcasting
{

    /// <summary>
    /// Options for the Dispatch broadcasting mode
    /// </summary>
    public enum BroadcastDispatchType
    {
        /// <summary>
        /// The broadcast is performed on the main thread, and the process waits for the broadcast method to finish before returning
        /// </summary>
        Send,

        /// <summary>
        /// The broadcast is performed on the main thread, and the process returns immediately without waiting for the broadcast method to finish
        /// </summary>
        Post
    }

    /// <summary>
    /// Base class for broadcasters with weakly-referenced subscribers
    /// </summary>
    public abstract class Broadcaster<T> : ObservableBase, IChannelBroadcaster<T>
    {
        private string name;
        private ChannelToken token;
        private object tag;

        /// <summary>
        /// Lock Object
        /// </summary>
        protected readonly object lockObj = new object();

        /// <summary>
        /// Maximum number of tasks to run in parallel
        /// </summary>
        protected readonly int maxTasks;

        /// <summary>
        /// Disposed Value
        /// </summary>
        protected bool disposedValue;

        /// <summary>
        /// The inner subscription list
        /// </summary>
        protected readonly List<ISubscription<T>> subscriptions = new List<ISubscription<T>>();

        /// <summary>
        /// Gets or sets the name of the broadcaster
        /// </summary>
        public virtual string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
            }
        }

        /// <summary>
        /// Gets or sets the channel token
        /// </summary>
        /// <remarks>
        /// Setting this property to <see cref="ChannelToken.Empty"/> will have the effect of generating a new random token.
        /// </remarks>
        public virtual ChannelToken ChannelToken
        {
            get => token;
            protected set
            {
                if (value == ChannelToken.Empty)
                {
                    value = ChannelToken.CreateToken();
                }

                SetProperty(ref token, value);
            }
        }

        /// <summary>
        /// Miscellaneous data to associate with this broadcaster
        /// </summary>
        public object Tag
        {
            get => tag;
            set
            {
                SetProperty(ref tag, value);
            }
        }

        /// <summary>
        /// The invocation method that this broadcaster uses to broadcast data
        /// </summary>
        public InvocationType InvocationType { get; }


        /// <summary>
        /// Dispatch broadcasting mode
        /// </summary>
        public BroadcastDispatchType DispatchType { get; }

        /// <summary>
        /// The parallel options 
        /// </summary>
        protected readonly ParallelOptions parallelOptions;

        /// <summary>
        /// Create a new broadcaster
        /// </summary>
        /// <param name="invocationType">The invocation method to use when broadcasting data</param>
        /// <param name="channelToken">The channel token to identify the new broadcaster</param>
        /// <param name="name">The name of the channel (can be null)</param>
        /// <param name="maxParallelTasks">Maximum number of parallel tasks to perform at once</param>
        /// <param name="dispatchType">Dispatcher broadcasting mode (only applies if the invocation type is set to Dispatcher)</param>
        protected Broadcaster(InvocationType invocationType, ChannelToken channelToken, string name, int maxParallelTasks = -1, BroadcastDispatchType dispatchType = BroadcastDispatchType.Send)
        {
            Synchronizer.Initialize();

            InvocationType = invocationType;
            ChannelToken = channelToken;
            DispatchType = dispatchType;

            Name = name;

            maxTasks = maxParallelTasks <= 0 ? Environment.ProcessorCount : maxParallelTasks;

            parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = maxTasks
            };
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

        /// <summary>
        /// Create a new subscription for the specified subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <param name="channelToken">Explicit channel token to use (instead of a randomly generated one)</param>
        /// <returns>A new subscription</returns>
        /// <remarks>
        /// This method will always create a new subscription, even if the specified subscriber has a previous subscription.
        /// </remarks>
        public ISubscription<T> Subscribe(ISubscriber<T> subscriber, ChannelToken channelToken)
        {
            return Subscribe(subscriber, false, channelToken);
        }

        ISubscription IBroadcaster.Subscribe(ISubscriber subscriber)
        {
            if (subscriber is ISubscriber<T> valid)
            {
                return Subscribe(valid);
            }

            throw new NotSupportedException();
        }

        ISubscription IChannelBroadcaster.Subscribe(ISubscriber subscriber, ChannelToken channelToken)
        {
            if (subscriber is ISubscriber<T> valid)
            {
                return Subscribe(valid, channelToken);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// Create a new subscription for the specified subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <param name="returnExisting">True to return any existing subscription for the specified subscriber instead of creating a new one</param>
        /// <param name="channelToken">Optional explicit channel token to use (instead of a randomly generated one)</param>
        /// <returns>A new subscription</returns>
        public ISubscription<T> Subscribe(ISubscriber<T> subscriber, bool returnExisting, ChannelToken? channelToken = null)
        {
            lock (lockObj)
            {
                if (returnExisting)
                {
                    CleanupSubscriptions();
                    foreach (var item in subscriptions)
                    {
                        if (item.TryGetSubscriber(out var subtest) && subtest == subscriber)
                        {
                            return item;
                        }
                        else if (channelToken is ChannelToken ct && ct == item.ChannelToken)
                        {
                            return item;
                        }
                    }
                }


                ISubscription<T> sub; 
                
                if (channelToken is ChannelToken cta)
                {
                    sub = new Subscription<T>(cta, subscriber, this);
                }
                else
                {
                    sub = new Subscription<T>(subscriber, this);
                }

                subscriptions.Add(sub);
                return sub;
            }
        }


        /// <summary>
        /// Detach the subscription from this service, ending the relationship
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        internal bool SubDetach(ISubscription<T> sub)
        {
            lock (lockObj)
            {
                return CleanupSubscriptions(sub);
            }
        }

        /// <summary>
        /// Cleanup subscriptions, remove any that fail a call to <see cref="ISubscription.Validate"/>
        /// </summary>
        public bool CleanupSubscriptions() => CleanupSubscriptions(null);
        
        /// <summary>
        /// Cleanup subscriptions, remove any that fail a call to <see cref="ISubscription.Validate"/><br/>
        /// Additionally, if <paramref name="removeItem"/> is not null, remove that item.
        /// </summary>
        /// <param name="removeItem">Item to remove</param>
        /// <returns>True if items were removed</returns>
        protected virtual bool CleanupSubscriptions(ISubscription<T> removeItem)
        {
            var res = false;

            lock (lockObj)
            {
                var c = subscriptions.Count;

                for (int i = c - 1; i >= 0; i--)
                {
                    var sub = subscriptions[i];

                    if ((sub == removeItem) || !sub.Validate())
                    {
                        if (!res) res = true;
                        subscriptions.Remove(sub);
                    }
                }
            }

            return res;
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
        /// Transmit new data
        /// </summary>
        /// <param name="data"></param>
        protected void TransmitData(T data)
        {
            TransmitData(data, InvocationType);
        }

        /// <summary>
        /// Transmit new data
        /// </summary>
        /// <param name="data">The data to transmit</param>
        /// <param name="invocationType">The invocation type</param>
        /// <param name="channels">Explicit channels to broadcast to</param>
        protected void TransmitData(T data, InvocationType invocationType, params ChannelToken[] channels)
        {
            bool dochannels = (channels != null && channels.Length > 0);
            var l = dochannels ? channels.Length : 0;

            if (invocationType == InvocationType.Parallel)
            {
                try
                {
                    Parallel.ForEach(subscriptions, parallelOptions, (sub) =>
                    {
                        try
                        {
                            if (dochannels)
                            {
                                int i;
                                for (i = 0; i < l; i++)
                                {
                                    if (channels[i] == sub.ChannelToken)
                                    {
                                        break;
                                    }
                                }
                             
                                if (i == l) return;
                            }

                            if (sub.TryGetSubscriber(out var subscriber))
                            {
                                var sbb = CreateSideBandData(data, sub, invocationType);
                                subscriber.ReceiveData(data, sbb);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                try
                {
                    foreach (var sub in subscriptions)
                    {

                        if (dochannels)
                        {
                            int i;
                            for (i = 0; i < l; i++)
                            {
                                if (channels[i] == sub.ChannelToken)
                                {
                                    break;
                                }
                            }

                            if (i == l) continue;
                        }

                        if (sub.TryGetSubscriber(out var subscriber))
                        {
                            var sbb = CreateSideBandData(data, sub, invocationType);

                            switch (invocationType)
                            {
                                case InvocationType.Synchronous:
                                    try
                                    {
                                        subscriber.ReceiveData(data, sbb);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex);
                                    }

                                    break;

                                case InvocationType.NoWait:
                                    _ = Task.Run(() =>
                                    {
                                        try
                                        {
                                            subscriber.ReceiveData(data, sbb);
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine(ex);
                                        }
                                    });
                                    break;

                                case InvocationType.Dispatcher:

                                    if (DispatchType == BroadcastDispatchType.Post)
                                    {
                                        Synchronizer.Default.BeginInvoke(() =>
                                        {
                                            try
                                            {
                                                subscriber.ReceiveData(data, sbb);
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex);
                                            }
                                        });
                                    }
                                    else
                                    {
                                        Synchronizer.Default.Invoke(() =>
                                        {
                                            try
                                            {
                                                subscriber.ReceiveData(data, sbb);
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex);
                                            }
                                        });
                                    }

                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
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