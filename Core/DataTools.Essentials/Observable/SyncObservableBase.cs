using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DataTools.Essentials.Observable
{
    /// <summary>
    /// Base class for observable classes that are synchronized with the UI thread.
    /// </summary>
    public abstract class SyncObservableBase : ObservableBase
    {
        private static SynchronizationContext sync;

        private static bool nopost;
        private static bool nosend;

        #region Static and Synchronization Context

        static SyncObservableBase()
        {
            sync = SynchronizationContext.Current;
            TestSendPost();
        }

        /// <summary>
        /// Call this method from the main thread to initialize the global UI context thread dispatcher.
        /// </summary>
        /// <param name="initSync"></param>
        public static void Initialize(SynchronizationContext initSync = null)
        {
            sync = initSync ?? SynchronizationContext.Current;
            TestSendPost();
        }

        /// <summary>
        /// Test the send or post ability of the synchronization object
        /// </summary>
        protected static void TestSendPost()
        {
            if (sync == null)
            {
                nosend = true;
                nopost = true;

                return;
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
        }

        /// <summary>
        /// Begin invoking a method on the main thread, and return immediately.
        /// </summary>
        /// <param name="action">The action or lambda</param>
        protected void BeginInvoke(Action action)
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
        protected void BeginInvoke(SendOrPostCallback action, object oparam)
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
        protected Task InvokeAsync(Action action)
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
        protected Task InvokeAsync(SendOrPostCallback action, object oparam)
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
        protected Task<T> InvokeAsync<T>(Func<object, T> action, object oparam)
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
        protected Task<T> InvokeAsync<T>(Func<T> action)
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

        #endregion Static and Synchronization Context

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            BeginInvoke(() => base.OnPropertyChanged(propertyName));
        }

        protected override bool OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            var b = false;

            if (nosend)
            {
                return base.OnPropertyChanging(propertyName);
            }
            else
            {
                sync.Send((o) => b = base.OnPropertyChanging(propertyName), null);
                return b;
            }
        }
    }
}