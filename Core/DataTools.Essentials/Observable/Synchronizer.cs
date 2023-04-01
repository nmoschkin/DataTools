using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataTools.Essentials.Observable
{
    /// <summary>
    /// <see cref="ISynchronizer"/> and <see cref="IAsyncSynchronizer"/> default implementation.
    /// </summary>
    public class Synchronizer : IAsyncSynchronizer
    {
        private static SynchronizationContext defsync;

        private static bool defnopost;
        private static bool defnosend;

        private bool nopost;
        private bool nosend;

        private SynchronizationContext sync;

        /// <summary>
        /// Returns true if we can obtain a synchronization context automatically from the current point in the program.
        /// </summary>
        public static bool CanInitialize => SynchronizationContext.Current != null;

        /// <summary>
        /// Gets the default synchronizer for the current application domain
        /// </summary>
        public static Synchronizer Default { get; private set; }

        /// <summary>
        /// Initialize the default synchronizer.
        /// </summary>
        /// <param name="initSync">Initial synchronizer, if available.</param>
        /// <returns></returns>
        public static bool Initialize(SynchronizationContext initSync = null)
        {
            var odefsync = initSync ?? SynchronizationContext.Current;
            if (odefsync == null) return false;

            (var dnosend, var dnopost) = TestSendPost(odefsync);

            var b = !dnosend || !dnopost;

            if (b)
            {
                defsync = odefsync;
                defnopost = dnopost;
                defnosend = dnosend;

                Default = new Synchronizer(defsync);
            }
            else if (defsync == null)
            {
                defnosend = defnopost = true;
            }

            return b;
        }

        static Synchronizer()
        {
            if (CanInitialize) Initialize();
        }

        /// <summary>
        /// Call this method from the main thread to initialize the global UI context thread dispatcher.
        /// </summary>
        /// <param name="initSync"></param>
        public Synchronizer(SynchronizationContext initSync = null)
        {
            sync = initSync ?? defsync ?? SynchronizationContext.Current;
            if (defsync == null && sync != null) defsync = sync;

            if (defsync != null && sync == defsync)
            {
                nosend = defnosend;
                nopost = defnopost;
            }
            else
            {
                (nosend, nopost) = TestSendPost(sync);
            }
        }

        /// <summary>
        /// Test the send or post ability of a synchronization object
        /// </summary>
        /// <param name="sync">The synchronization context to test.</param>
        /// <returns>
        /// Bool tuple (nosend, nopost)
        /// </returns>
        protected static (bool, bool) TestSendPost(SynchronizationContext sync)
        {
            bool nosend, nopost;

            if (sync == null)
            {
                return (true, true);
            }

            try
            {
                sync.Post((o) => { int i = 0; }, null);
                nopost = false;
            }
            catch
            {
                nopost = true;
            }

            try
            {
                sync.Send((o) => { int i = 0; }, null);
                nosend = false;
            }
            catch
            {
                nosend = true;
            }

            return (nosend, nopost);
        }

        /// <summary>
        /// Begin invoking a method on the main thread, and return immediately.
        /// </summary>
        /// <param name="action">The action or lambda</param>
        public void BeginInvoke(Action action)
        {
            if (nosend && nopost)
            {
                _ = Task.Run(() => action());
            }
            else if (nopost)
            {
                _ = InvokeAsync(action);
            }
            else
            {
                sync.Post((o) => action(), null);
            }
        }

        /// <summary>
        /// Begin invoking a method on the main thread, and return immediately.
        /// </summary>
        /// <param name="action">The action or lambda that takes a single object as a parameter</param>
        /// <param name="oparam">Parameter object (can be null)</param>
        public void BeginInvoke(SendOrPostCallback action, object oparam)
        {
            if (nosend && nopost)
            {
                _ = Task.Run(() => action(oparam));
            }
            else if (nopost)
            {
                _ = InvokeAsync(action, oparam);
            }
            else
            {
                sync.Post(action, oparam);
            }
        }

        /// <summary>
        /// Invoke the method on the main thread asynchronously
        /// </summary>
        /// <param name="action">The action or lambda</param>
        public Task InvokeAsync(Action action)
        {
            if (nosend && nopost)
            {
                return Task.Run(() => action());
            }
            else if (nosend)
            {
                BeginInvoke(action);
                return Task.CompletedTask;
            }
            else
            {
                return Task.Run(() => sync.Send((o) => action(), null));
            }
        }

        /// <summary>
        /// Invoke the method on the main thread asynchronously
        /// </summary>
        /// <param name="action">The action or lambda that takes a single object as a parameter</param>
        /// <param name="oparam">Parameter object (can be null)</param>
        public Task InvokeAsync(SendOrPostCallback action, object oparam)
        {
            if (nosend && nopost)
            {
                return Task.Run(() => action(oparam));
            }
            else if (nosend)
            {
                BeginInvoke(action, oparam);
                return Task.CompletedTask;
            }
            else
            {
                return Task.Run(() => sync.Send(action, oparam));
            }
        }

        /// <summary>
        /// Invoke a method on the main thread asynchronously that returns a result of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="action">The action or lambda that takes a single object as a parameter and returns a value of type <typeparamref name="T"/>.</param>
        /// <param name="oparam">Parameter object (can be null)</param>
        /// <returns></returns>
        public Task<T> InvokeAsync<T>(Func<object, T> action, object oparam)
        {
            var tsk = new TaskCompletionSource<T>();

            if (nosend)
            {
                tsk.SetResult(action(oparam));
            }
            else
            {
                return Task.Run(() =>
                {
                    T result = default;
                    sync.Send((o) => result = action(o), oparam);
                    return result;
                });
            }

            return tsk.Task;
        }

        /// <summary>
        /// Invoke a method on the main thread asynchronously that returns a result of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="action">The action or lambda that takes a single object as a parameter and returns a value of type <typeparamref name="T"/>.</param>
        /// <returns></returns>
        public Task<T> InvokeAsync<T>(Func<T> action)
        {
            var tsk = new TaskCompletionSource<T>();

            if (nosend)
            {
                tsk.SetResult(action());
            }
            else
            {
                return Task.Run(() =>
                {
                    T result = default;
                    sync.Send((o) => result = action(), null);
                    return result;
                });
            }

            return tsk.Task;
        }

        /// <inheritdoc/>
        public void Invoke(Action action)
        {
            if (nosend)
            {
                action();
            }
            else
            {
                sync.Send((o) => action(), null);
            }
        }

        /// <inheritdoc/>
        public T Invoke<T>(Func<T> action)
        {
            if (nosend)
            {
                return action();
            }
            else
            {
                T result = default;
                sync.Send((o) => result = action(), null);
                return result;
            }
        }

        /// <inheritdoc/>
        public virtual bool CanPost => !nopost;

        /// <inheritdoc/>
        public virtual bool CanSend => !nosend;
    }
}