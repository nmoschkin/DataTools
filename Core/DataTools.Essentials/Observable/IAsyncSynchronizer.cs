using System;
using System.Threading.Tasks;

namespace DataTools.Essentials.Observable
{
    /// <summary>
    /// Implements a synchronizer/dispatcher that can invoke async
    /// </summary>
    public interface IAsyncSynchronizer : ISynchronizer
    {
        /// <summary>
        /// Invoke the specified action asynchronously.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns></returns>
        Task InvokeAsync(Action action);

        /// <summary>
        /// Invoke the specified function asynchronously.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <returns>The return value from the invoked method.</returns>
        Task<T> InvokeAsync<T>(Func<T> method);
    }
}