using DataTools.Win32.Network;

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataTools.Desktop.Network
{
    public class ObservableAdaptersCollection : AdaptersCollection<NetworkAdapterViewModel>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private System.Windows.Application app = System.Windows.Application.Current;

        public ObservableAdaptersCollection() : base()
        {
        }

        public ObservableAdaptersCollection(bool populate, bool sortOnRefresh = true)
        {
            SortOnRefresh = sortOnRefresh;

            if (populate)
            {
                Refresh();
            }
        }

        protected override void Sort()
        {
            lock (lockObj)
            {
                base.Sort();
            }

            app?.Dispatcher?.Invoke(() =>
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        protected override void AddItem(NetworkAdapterViewModel value)
        {
            lock (lockObj)
            {
                base.AddItem(value);
            }

            app?.Dispatcher?.Invoke(() =>
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, Count - 1));
                OnPropertyChanged(nameof(Count));
            });
        }

        protected override void RemoveAt(int index)
        {
            var oldItem = this[index];

            lock (lockObj)
            {
                base.RemoveAt(index);
            }

            app?.Dispatcher?.Invoke(() =>
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
                OnPropertyChanged(nameof(Count));
            });
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ~ObservableAdaptersCollection()
        {
            Dispose(false);
        }
    }
}