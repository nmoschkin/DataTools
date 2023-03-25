using System;
using System.Text;

namespace DataTools.Essentials.Observable
{
    /// <summary>
    /// Implements a synchronizer/dispatcher
    /// </summary>
    public interface ISynchronizer
    {
        /// <summary>
        /// True if can post asynchronously
        /// </summary>
        bool CanPost { get; }

        /// <summary>
        /// True if can send synchronously
        /// </summary>
        bool CanSend { get; }

        /// <summary>
        /// Invoke the specified action 
        /// </summary>
        /// <param name="action"></param>
        void Invoke(Action action);

        /// <summary>
        /// Invoke the specified method 
        /// </summary>
        /// <typeparam name="T">The return value type</typeparam>
        /// <param name="method">The method to invoke</param>
        /// <returns>The return value from the method</returns>
        T Invoke<T>(Func<T> method);

        /// <summary>
        /// Begin invoke the specified action
        /// </summary>
        /// <param name="action">The action to run asynchronously</param>
        /// <remarks>
        /// This method returns immediately and does not wait for the <paramref name="action"/> to complete.
        /// </remarks>
        void BeginInvoke(Action action);
    }
}