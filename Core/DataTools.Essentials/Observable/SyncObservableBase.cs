using System;
using System.Runtime.CompilerServices;

namespace DataTools.Essentials.Observable
{
    /// <summary>
    /// Base class for observable classes that are synchronized with the UI thread.
    /// </summary>
    public abstract class SyncObservableBase : ObservableBase
    {
        /// <summary>
        /// The current synchronizer
        /// </summary>
        protected readonly ISynchronizer sync;

        /// <summary>
        /// Create a new synchronizer from the specified synchronizer
        /// </summary>
        /// <param name="sync"></param>
        public SyncObservableBase(ISynchronizer sync)
        {
            this.sync = sync;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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