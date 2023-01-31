using System;
using System.Text;

namespace DataTools.Essentials.Observable
{
    public interface ISynchronizer
    {
        bool CanPost { get; }

        bool CanSend { get; }

        void Invoke(Action action);

        T Invoke<T>(Func<T> action);

        void BeginInvoke(Action action);
    }
}