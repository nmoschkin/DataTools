using System;
using System.Runtime.CompilerServices;

namespace DataTools.Essentials.Observable
{
    /// <summary>
    /// Base class for observable classes that are synchronized with the UI thread.
    /// </summary>
    public abstract class SyncObservableBase : ObservableBase
    {
        protected readonly ISynchronizer sync;

        public SyncObservableBase(ISynchronizer sync)
        {
            this.sync = sync;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (sync != null && sync.CanPost)
            {
                sync.BeginInvoke(() =>
                {
                    base.OnPropertyChanged(propertyName);
                });
            }
            else
            {
                base.OnPropertyChanged(propertyName);
            }
        }

        protected override bool OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (sync != null && sync.CanSend)
            {
                return sync.Invoke(() => base.OnPropertyChanging(propertyName));
            }
            else
            {
                return base.OnPropertyChanging(propertyName);
            }
        }
    }
}