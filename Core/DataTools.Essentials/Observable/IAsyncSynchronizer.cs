using System;
using System.Threading.Tasks;

namespace DataTools.Essentials.Observable
{
    public interface IAsyncSynchronizer : ISynchronizer
    {
        Task InvokeAsync(Action action);

        Task<T> InvokeAsync<T>(Func<T> action);
    }
}