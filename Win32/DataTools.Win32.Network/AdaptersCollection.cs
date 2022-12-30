// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: AdapterCollection
//         Encapsulates the network interface environment
//         of the currently running system.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Essentials.SortedLists;
using DataTools.Win32.Memory;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

namespace DataTools.Win32.Network
{
    #region Deprecated

    //    //'' <summary>
    //    //'' System network adapter information thin wrappers.
    //    //'' </summary>
    //    //'' <remarks>
    //    //'' The observable collection is more suitable for use as a WPF data source.
    //    //''
    //    //'' The NetworkAdapter class cannot be created independently.
    //    //''
    //    //'' For most usage cases, the AdaptersCollection object should be used.
    //    //''
    //    //'' The <see cref="NetworkAdapters"/> collection is also a viable option
    //    //'' and possibly of a lighter variety.
    //    //'' </remarks>
    //    // Public Module NetworkWrappers

    //    /// <summary>
    //    /// Managed wrapper collection for all adapters.
    //    /// </summary>
    //    [Category("Devices")]
    //    [Description("Network adapter collection.")]
    //    [Browsable(true)]
    //    public class NetworkAdapters : ICollection<IP_ADAPTER_ADDRESSES>, IDisposable
    //    {
    //        private IP_ADAPTER_ADDRESSES[] _Adapters;
    //        private MemPtr _origPtr;

    //        private Collection<NetworkAdapter> _Col = new Collection<NetworkAdapter>();

    //        /// <summary>
    //        /// Returns an array of <see cref="IP_ADAPTER_ADDRESSES" /> structures
    //        /// </summary>
    //        /// <returns></returns>
    //        [Category("Devices")]
    //        [Description("Network adapter collection.")]
    //        [Browsable(true)]
    //        public IP_ADAPTER_ADDRESSES[] Adapters
    //        {
    //            get
    //            {
    //                return _Adapters;
    //            }

    //            set
    //            {
    //                Clear();
    //                _Adapters = value;
    //            }
    //        }

    //        internal NetworkAdapters()
    //        {
    //            Refresh();
    //        }

    //        /// <summary>
    //        /// Refresh the list by calling <see cref="DeviceEnum.EnumerateNetworkDevices()"/>
    //        /// </summary>
    //        public void Refresh()
    //        {
    //            Free();

    //            var di = DeviceEnum.EnumerateDevices<DeviceInfo>(DevProp.GUID_DEVINTERFACE_NET);

    //            _Adapters = IfDefApi.GetAdapters(ref _origPtr, true);

    //            foreach (var adap in _Adapters)
    //            {
    //                var newp = new NetworkAdapter(adap);

    //                foreach (var de in di)
    //                {
    //                    if ((de.Description ?? "") == (adap.Description ?? ""))
    //                    {
    //                        newp.DeviceInfo = de;
    //                        break;
    //                    }
    //                }

    //                _Col.Add(newp);
    //            }
    //        }

    //        [Browsable(true)]
    //        [Category("Collections")]
    //        public Collection<NetworkAdapter> AdapterCollection
    //        {
    //            get
    //            {
    //                return _Col;
    //            }
    //            internal set
    //            {
    //                _Col = value;
    //            }
    //        }

    //        /// <summary>
    //        /// Returns the <see cref="IP_ADAPTER_ADDRESSES" /> structure at the specified index
    //        /// </summary>
    //        /// <param name="index">Index of item to return.</param>
    //        /// <returns><see cref="IP_ADAPTER_ADDRESSES" /> structure</returns>
    //        public IP_ADAPTER_ADDRESSES this[int index]
    //        {
    //            get
    //            {
    //                return _Adapters[index];
    //            }
    //        }

    //        public IEnumerator<IP_ADAPTER_ADDRESSES> GetEnumerator()
    //        {
    //            return new IP_ADAPTER_ADDRESSES_Enumerator(this);
    //        }

    //        IEnumerator IEnumerable.GetEnumerator()
    //        {
    //            return new IP_ADAPTER_ADDRESSES_Enumerator(this);
    //        }

    //        private bool disposedValue; // To detect redundant calls

    //        // IDisposable
    //        protected virtual void Dispose(bool disposing)
    //        {
    //            if (!disposedValue)
    //            {
    //                if (disposing)
    //                {
    //                    _Adapters = null;
    //                }

    //                Free();
    //            }

    //            disposedValue = true;
    //        }

    //        ~NetworkAdapters()
    //        {
    //            Dispose(false);
    //        }

    //        protected void Free()
    //        {
    //            if (_origPtr.Handle != IntPtr.Zero)
    //            {
    //                _origPtr.Free(true);
    //            }

    //            _Adapters = null;
    //            _Col.Clear();
    //        }

    //        public void Dispose()
    //        {
    //            Dispose(true);
    //            GC.SuppressFinalize(this);
    //        }

    //        public void Add(IP_ADAPTER_ADDRESSES item)
    //        {
    //            throw new AccessViolationException("Cannot add items to a system managed list");
    //        }

    //        public void Clear()
    //        {
    //            _Adapters = null;
    //            Free();
    //        }

    //        public bool Contains(IP_ADAPTER_ADDRESSES item)
    //        {
    //            if (_Adapters is null)
    //                return false;
    //            foreach (var aa in _Adapters)
    //            {
    //                if (aa.NetworkGuid == item.NetworkGuid)
    //                    return true;
    //            }

    //            return false;
    //        }

    //        public void CopyTo(IP_ADAPTER_ADDRESSES[] array, int arrayIndex)
    //        {
    //            if (_Adapters is null)
    //            {
    //                throw new NullReferenceException();
    //            }

    //            _Adapters.CopyTo(array, arrayIndex);
    //        }

    //        public int Count
    //        {
    //            get
    //            {
    //                if (_Adapters is null)
    //                    return 0;
    //                else
    //                    return _Adapters.Count();
    //            }
    //        }

    //        public bool IsReadOnly
    //        {
    //            get
    //            {
    //                return true;
    //            }
    //        }

    //        public bool Remove(IP_ADAPTER_ADDRESSES item)
    //        {
    //            return false;
    //        }
    //    }

    //    public class IP_ADAPTER_ADDRESSES_Enumerator : IEnumerator<IP_ADAPTER_ADDRESSES>
    //    {
    //        private int pos = -1;
    //        private NetworkAdapters subj;

    //        internal IP_ADAPTER_ADDRESSES_Enumerator(NetworkAdapters subject)
    //        {
    //            subj = subject;
    //        }

    //        public IP_ADAPTER_ADDRESSES Current
    //        {
    //            get
    //            {
    //                return subj[pos];
    //            }
    //        }

    //        object IEnumerator.Current
    //        {
    //            get
    //            {
    //                return subj[pos];
    //            }
    //        }

    //        public bool MoveNext()
    //        {
    //            pos += 1;
    //            if (pos >= subj.Count)
    //                return false;
    //            return true;
    //        }

    //        public void Reset()
    //        {
    //            pos = -1;
    //        }

    //        private bool disposedValue; // To detect redundant calls

    //        protected virtual void Dispose(bool disposing)
    //        {
    //            if (!disposedValue)
    //            {
    //                if (disposing)
    //                {
    //                    subj = null;
    //                    pos = -1;
    //                }
    //            }

    //            disposedValue = true;
    //        }

    //        public void Dispose()
    //        {
    //            Dispose(true);
    //            GC.SuppressFinalize(this);
    //        }

    //    }

    #endregion Deprecated

    /// <summary>
    /// Represents the various states of internet connectivity.
    /// </summary>
    public enum InternetStatus
    {
        /// <summary>
        /// Cannot determined connected state.
        /// </summary>
        NotDetermined,

        /// <summary>
        /// Has internet (ping address is reachable.)
        /// </summary>
        HasInternet,

        /// <summary>
        /// Does not have internet (ping address is not reachable.)
        /// </summary>
        NoInternet
    }

    /// <summary>
    /// A managed observable collection wrapper of NetworkAdapter wrapper objects.  This collection wraps the
    /// Windows Network Interface Api.  All objects are thin wrappers around the original unmanaged
    /// memory objects.
    /// </summary>
    /// <remarks>
    /// The array memory is allocated as one very long block by the GetAdapters function.
    /// We keep it in this collection and the members in the unmanaged memory source serve
    /// as the backbone for the collection of NetworkAdapter objects.
    ///
    /// For this reason, the NetworkAdapter object cannot be created publically, as the
    /// AdaptersCollection object is managing a single block of unmanaged memory for the entire collection.
    /// Therefore, there can be no singleton instances of the NetworkAdapter object.
    ///
    /// We will use Finalize() to free this (rather large) resource when this class is destroyed.
    /// </remarks>
    [SecurityCritical()]
    public class AdaptersCollection<T> : IDisposable, IReadOnlyList<T> where T : NetworkAdapter
    {
        private List<T> adapters = new List<T>();
        private MemPtr _origPtr;

        protected object lockObj = new object();
        protected bool sortOnRefresh = true;
        protected Comparison<T> sortComparison = new Comparison<T>((a, b) => a.IfIndex - b.IfIndex);

        /// <summary>
        /// Gets or sets the <see cref="Comparison{T}"/> method that is used to compare elements during a sort.
        /// </summary>
        public virtual Comparison<T> SortComparison
        {
            get => sortComparison;
            set
            {
                lock (lockObj)
                {
                    if (value == null)
                    {
                        SortOnRefresh = false;
                        sortComparison = null;
                    }
                    else
                    {
                        sortComparison = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to sort on refresh.
        /// </summary>
        public virtual bool SortOnRefresh
        {
            get => sortOnRefresh && (sortComparison != null);
            set
            {
                lock (lockObj)
                {
                    sortOnRefresh = value;

                    if (value && sortComparison == null)
                    {
                        SortComparison = new Comparison<T>((a, b) => a.IfIndex - b.IfIndex);
                    }
                }
            }
        }

        public AdaptersCollection()
        {
            Refresh();
        }

        /// <summary>
        /// Create a NetworkAdapter instance.
        /// </summary>
        /// <returns>A new instance or null if not able to create.</returns>
        protected virtual T CreateAdapterInstance()
        {
            ConstructorInfo maker = null;

            var candidates = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            if (candidates.Length == 0)
            {
                candidates = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                if (candidates.Length == 0) return null;
            }

            foreach (var candidate in candidates)
            {
                if (candidate.GetParameters().Length == 0)
                {
                    maker = candidate;
                    break;
                }
            }

            if (maker == null)
            {
                return null;
            }

            return (T)maker.Invoke(new object[0]);
        }

        /// <summary>
        /// Refresh the adapters collection.
        /// </summary>
        public void Refresh()
        {
            lock (lockObj)
            {
                var nad = new Dictionary<int, T>();
                var lOut = new List<T>();

                var newmm = new MemPtr();

                // Get the array of unmanaged IP_ADAPTER_ADDRESSES structures
                var newads = IfDefApi.GetAdapters(ref newmm, true);

                var di = DeviceEnum.EnumerateDevices<DeviceInfo>(DevProp.GUID_DEVINTERFACE_NET);
                var iftab = IfTable.GetIfTable();

                foreach (var adap in newads)
                {
                    var newp = CreateAdapterInstance();
                    if (newp == null)
                    {
                        throw new InvalidOperationException("Unable to create an instance of NetworkAdapter-derived class '" + typeof(T).FullName + "'. Make sure a callable constructor exists!");
                    }

                    newp.AssignNewNativeObject(adap);

                    foreach (var de in di)
                    {
                        if ((de.Description ?? "") == (adap.Description ?? "") || (de.FriendlyName ?? "") == (adap.FriendlyName ?? "") || (de.FriendlyName ?? "") == (adap.Description ?? "") || (de.Description ?? "") == (adap.FriendlyName ?? ""))
                        {
                            newp.DeviceInfo = de;

                            foreach (var iface in iftab)
                            {
                                if (newp.PhysicalAddress == iface.bPhysAddr)
                                {
                                    newp.PhysIfaceInternal.Add(iface);
                                }
                            }

                            nad.Add(newp.IfIndex, newp);
                            _ = Task.Run(() => PopulateInternetStatus(newp));

                            break;
                        }
                    }
                }

                if (adapters == null)
                {
                    adapters = new List<T>();
                }

                if (adapters.Count == 0)
                {
                    foreach (var kv in nad)
                    {
                        adapters.Add(kv.Value);
                    }
                }
                else
                {
                    List<int> kseen;

                    int c = adapters.Count - 1;
                    int i;

                    kseen = new List<int>();

                    for (i = c; i >= 0; i--)
                    {
                        if (nad.ContainsKey(adapters[i].IfIndex))
                        {
                            adapters[i].AssignNewNativeObject(nad[adapters[i].IfIndex]);
                            kseen.Add(adapters[i].IfIndex);
                        }
                        else
                        {
                            RemoveAt(i);
                        }
                    }

                    foreach (var kv in nad)
                    {
                        if (!kseen.Contains(kv.Value.IfIndex))
                        {
                            AddItem(kv.Value);
                        }
                    }
                }

                if (_origPtr != MemPtr.Empty)
                {
                    _origPtr.Free(true);
                }

                _origPtr = newmm;

                if (sortOnRefresh) Sort();
            }
        }

        private void PopulateInternetStatus(NetworkAdapter adapter)
        {
            var addrs = adapter.FirstUnicastAddress.AddressChain;

            foreach (var addr in addrs)
            {
                if (addr.Address.lpSockaddr.IPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Socket socket = new Socket(SocketType.Raw, ProtocolType.Icmp);

                    try
                    {
                        socket.Bind(new IPEndPoint(addr.IPAddress, 0));
                    }
                    catch
                    {
                        continue;
                    }

                    socket.ReceiveTimeout = 2000;
                    socket.SendTimeout = 2000;

                    try
                    {
                        IAsyncResult result = socket.BeginConnect(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 0), null, null);

                        bool success = result.AsyncWaitHandle.WaitOne(2000, true);

                        if (socket.Connected)
                        {
                            socket.EndConnect(result);
                            adapter.HasInternet = InternetStatus.HasInternet;
                        }
                        else
                        {
                            socket.Close();
                            adapter.HasInternet = InternetStatus.NoInternet;
                        }
                    }
                    catch
                    {
                        adapter.HasInternet = InternetStatus.NoInternet;
                    }
                    return;
                }
            }

            adapter.HasInternet = InternetStatus.NoInternet;
        }

        /// <summary>
        /// Sort the list using the <see cref="SortComparison"/> function.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual void Sort()
        {
            lock (lockObj)
            {
                if (sortComparison == null)
                {
                    throw new InvalidOperationException("Cannot sort when no comparison is defined.");
                }

                QuickSort.Sort(adapters, (a, b) => sortComparison(a, b));
            }
        }

        /// <summary>
        /// Add an item to the list.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void AddItem(T value)
        {
            lock (lockObj)
            {
                adapters.Add(value);
            }
        }

        /// <summary>
        /// Removes the item at the specified index from the list.
        /// </summary>
        /// <param name="index"></param>
        protected virtual void RemoveAt(int index)
        {
            lock (lockObj)
            {
                adapters.RemoveAt(index);
            }
        }

        private bool disposedValue; // To detect redundant calls

        public int Count => ((IReadOnlyCollection<T>)adapters).Count;

        public T this[int index] => ((IReadOnlyList<T>)adapters)[index];

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        /// <param name="disposing">true if disposing, false if called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // free up the unmanaged memory and release the memory pressure on the garbage collector.
                _origPtr.Free(true);
            }

            disposedValue = true;
        }

        ~AdaptersCollection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)adapters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)adapters).GetEnumerator();
        }
    }

    /// <summary>
    /// A managed observable collection wrapper of NetworkAdapter wrapper objects.  This collection wraps the
    /// Windows Network Interface Api.  All objects are thin wrappers around the original unmanaged
    /// memory objects.
    /// </summary>
    /// <remarks>
    /// The array memory is allocated as one very long block by the GetAdapters function.
    /// We keep it in this collection and the members in the unmanaged memory source serve
    /// as the backbone for the collection of NetworkAdapter objects.
    ///
    /// For this reason, the NetworkAdapter object cannot be created publically, as the
    /// AdaptersCollection object is managing a single block of unmanaged memory for the entire collection.
    /// Therefore, there can be no singleton instances of the NetworkAdapter object.
    ///
    /// We will use Finalize() to free this (rather large) resource when this class is destroyed.
    /// </remarks>
    [SecurityCritical()]
    public class AdaptersCollection : AdaptersCollection<NetworkAdapter>
    {
        public AdaptersCollection() : base()
        {
        }
    }
}