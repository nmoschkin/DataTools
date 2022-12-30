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

using DataTools.Desktop;
using DataTools.Shell.Native;
using DataTools.Text;
using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("DataTools.Desktop.Network")]

namespace DataTools.Win32.Network
{
    /// <summary>
    /// Managed wrapper class for the native network adapter information API.
    /// </summary>
    /// <remarks></remarks>
    public class NetworkAdapter : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IP_ADAPTER_ADDRESSES Source;

        private DeviceInfo deviceInfo;
        private bool canShowNet;
        private System.Drawing.Bitmap deviceIcon;

        private List<MIB_IFROW> physifaces = new List<MIB_IFROW>();
        private static readonly PropertyInfo[] allProps = typeof(NetworkAdapter).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // This class should not be created outside of the context of AdaptersCollection.

        internal NetworkAdapter()
        {
        }

        internal void AssignNewNativeObject(NetworkAdapter newSource)
        {
            bool dnes = Source.OperStatus == newSource.OperStatus;
            AssignNewNativeObject(newSource.Source, dnes);
        }

        internal void AssignNewNativeObject(IP_ADAPTER_ADDRESSES nativeObject, bool noCreateIcon = false)
        {
            // Store the native object.
            Source = nativeObject;

            if (!noCreateIcon)
            {
                // First thing's first... let's get the icon for the object from its parsing name.
                // Which is magically the parsing name of the network device list and the adapter's GUID name.
                string s = @"::{7007ACC7-3202-11D1-AAD2-00805FC1270E}\" + AdapterName;
                var mm = new MemPtr();

                NativeShell.SHParseDisplayName(s, IntPtr.Zero, out mm.handle, 0, out _);

                if (mm.Handle != IntPtr.Zero)
                {
                    // Get a WPFImage

                    // string library = @"%systemroot%\system32\shell32.dll"

                    if (OperStatus == IfOperStatus.IfOperStatusUp)
                    {
                        //if (HasInternet == InternetStatus.HasInternet)
                        //{
                        //    var icn = Resources.LoadLibraryIcon(Environment.ExpandEnvironmentVariables(@"%systemroot%\system32\netcenter.dll") + ",2", StandardIcons.Icon16);
                        //    var icn2 = Resources.GetItemIcon(mm, Resources.SystemIconSizes.ExtraLarge);

                        //    var bmp = new System.Drawing.Bitmap(icn2.Width, icn2.Height,  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        //    var g = System.Drawing.Graphics.FromImage(bmp);

                        //    g.DrawIcon(icn2, 0, 0);
                        //    g.DrawIcon(icn, 0, icn2.Height - 16);

                        //    g.Dispose();

                        //    _Icon = Resources.MakeWPFImage(bmp);
                        //    bmp.Dispose();
                        //    icn.Dispose();
                        //    icn2.Dispose();
                        //}
                        //else
                        //{
                        var icn = Resources.GetItemIcon(mm, Resources.SystemIconSizes.ExtraLarge);
                        DeviceIcon = Resources.IconToTransparentBitmap(icn);
                    }
                    else
                    {
                        var icn = Resources.GetItemIcon(mm, Resources.SystemIconSizes.ExtraLarge);
                        DeviceIcon = (System.Drawing.Bitmap)Resources.GrayIcon(icn);
                    }
                    mm.Free();

                    canShowNet = true;
                }
                else
                {
                    canShowNet = false;
                }
            }

            OnPropertyChanged(nameof(ReceiveLinkSpeed));
            OnPropertyChanged(nameof(TransmitLinkSpeed));
            OnPropertyChanged(nameof(OperStatus));

            foreach (var pr in allProps)
            {
                if (pr.Name.Contains("Address"))
                    OnPropertyChanged(pr.Name);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string e = null)
        {
            if (e == null) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }

        /// <summary>
        /// Is true if the device dialog can be displayed for this adapter.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CanShowDeviceDialog
        {
            get
            {
                return deviceInfo is object;
            }
        }

        /// <summary>
        /// Returns a Bitmap of the device's icon.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(false)]
        public virtual System.Drawing.Bitmap DeviceIcon
        {
            get
            {
                return deviceIcon;
            }
            protected set
            {
                deviceIcon = value;
            }
        }

        /// <summary>
        /// Is true if this device can display the network dialog.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CanShowNetworkDialogs
        {
            get
            {
                return canShowNet;
            }
        }

        /// <summary>
        /// Raise the device properties dialog.
        /// </summary>
        /// <remarks></remarks>
        public void ShowDevicePropertiesDialog()
        {
            if (deviceInfo is null)
                return;
            deviceInfo.ShowDevicePropertiesDialog();
        }

        /// <summary>
        /// Raise the connection properties dialog.  This may throw the UAC.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <remarks></remarks>
        public void ShowConnectionPropertiesDialog(IntPtr hwnd = default)
        {
            if (deviceInfo is null) return;

            var shex = new SHELLEXECUTEINFO()
            {
                cbSize = Marshal.SizeOf<SHELLEXECUTEINFO>(),
                nShow = User32.SW_SHOW,
                hInstApp = Process.GetCurrentProcess().Handle,
                hWnd = hwnd,
                lpVerb = "properties",
                lpDirectory = "::{7007ACC7-3202-11D1-AAD2-00805FC1270E}",
                lpFile = @"::{7007ACC7-3202-11D1-AAD2-00805FC1270E}\" + AdapterName,
                fMask = User32.SEE_MASK_ASYNCOK | User32.SEE_MASK_FLAG_DDEWAIT | User32.SEE_MASK_UNICODE
            };

            User32.ShellExecuteEx(ref shex);
        }

        /// <summary>
        /// Raise the connection status dialog.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <remarks></remarks>
        public void ShowNetworkStatusDialog(IntPtr hwnd = default)
        {
            var shex = new SHELLEXECUTEINFO
            {
                cbSize = Marshal.SizeOf<SHELLEXECUTEINFO>(),
                hWnd = hwnd,
                nShow = User32.SW_SHOW,
                lpVerb = "",
                hInstApp = Process.GetCurrentProcess().Handle,
                lpDirectory = "::{7007ACC7-3202-11D1-AAD2-00805FC1270E}",
                lpFile = @"::{7007ACC7-3202-11D1-AAD2-00805FC1270E}\" + AdapterName,
                fMask = User32.SEE_MASK_ASYNCOK | User32.SEE_MASK_FLAG_DDEWAIT | User32.SEE_MASK_UNICODE
            };

            User32.ShellExecuteEx(ref shex);
        }

        /// <summary>
        /// Retrieves the DeviceInfo object for the system device instance of the network adapter.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DeviceInfo DeviceInfo
        {
            get
            {
                return deviceInfo;
            }
            internal set
            {
                deviceInfo = value;
                if (deviceIcon is null)
                {
                    // if the adapter doesn't have its own icon, the device class surely will.
                    // let's see if we can get an icon from the device object!
                    if (deviceInfo?.DeviceIcon != null)
                    {
                        DeviceIcon = Resources.IconToTransparentBitmap(deviceInfo.DeviceIcon);
                        OnPropertyChanged("DeviceIcon");
                    }
                }
            }
        }

        /// <summary>
        /// The interface adapter index.  This can be used in PowerShell calls.
        /// </summary>
        [Browsable(true)]
        public int IfIndex
        {
            get
            {
                return (int)Source.Header.IfIndex;
            }
        }

        /// <summary>
        /// The GUID adapter name.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public string AdapterName
        {
            get
            {
                return Source.AdapterName;
            }
        }

        [Browsable(true)]
        public IPAddress IPV4Address
        {
            get
            {
                var addrs = Source.FirstUnicastAddress.AddressChain;
                if (addrs == null || addrs.Length == 0) return null;

                foreach (var addr in addrs)
                {
                    var ip = addr.IPAddress;
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip;
                    }
                }

                return null;
            }
        }

        [Browsable(true)]
        public IPAddress IPV6Address
        {
            get
            {
                var addrs = Source.FirstUnicastAddress.AddressChain;
                if (addrs == null || addrs.Length == 0) return null;

                foreach (var addr in addrs)
                {
                    var ip = addr.IPAddress;
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        return ip;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// The first IP address of this device.  Usually IPv6. The IPv4 address resides at FirstUnicastAddress.Next.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public LPADAPTER_UNICAST_ADDRESS FirstUnicastAddress
        {
            get
            {
                return Source.FirstUnicastAddress;
            }
        }

        /// <summary>
        /// The first Anycast address.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public LPADAPTER_MULTICAST_ADDRESS FirstAnycastAddress
        {
            get
            {
                return Source.FirstAnycastAddress;
            }
        }

        /// <summary>
        /// First multicast address.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public LPADAPTER_MULTICAST_ADDRESS FirstMulticastAddress
        {
            get
            {
                return Source.FirstMulticastAddress;
            }
        }

        /// <summary>
        /// First Dns server address.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public LPADAPTER_MULTICAST_ADDRESS FirstDnsServerAddress
        {
            get
            {
                return Source.FirstDnsServerAddress;
            }
        }

        /// <summary>
        /// Dns Suffix. This is typically the name of your ISP's internal domain if you are connected to an ISP.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public string DnsSuffix
        {
            get
            {
                return Source.DnsSuffix;
            }
        }

        /// <summary>
        /// This is always the friendly name of the device instance of the network adapter.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public string Description
        {
            get
            {
                return Source.Description;
            }
        }

        /// <summary>
        /// Friendly name of the network connection (Ethernet 1, WiFi 2, etc).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public string FriendlyName
        {
            get
            {
                return Source.FriendlyName;
            }
        }

        /// <summary>
        /// The MAC address of the adapter.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public MACADDRESS PhysicalAddress
        {
            get
            {
                return Source.PhysicalAddress;
            }
        }

        [Browsable(true)]
        public uint PhysicalAddressLength
        {
            get
            {
                return Source.PhysicalAddressLength;
            }
        }

        /// <summary>
        /// Adapter configuration flags and capabilities.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public IPAdapterAddressesFlags Flags
        {
            get
            {
                return Source.Flags;
            }
        }

        /// <summary>
        /// Maximum transmission unit, in bytes.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public int Mtu
        {
            get
            {
                return Source.Mtu;
            }
        }

        /// <summary>
        /// Interface type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public IFTYPE IfType
        {
            get
            {
                return Source.IfType;
            }
        }

        [Browsable(true)]
        public string IfTypeDescription
        {
            get
            {
                var fi = typeof(IFTYPE).GetField(Source.IfType.ToString());
                if (fi.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute da)
                {
                    return da.Description;
                }
                return Source.IfType.ToString();
            }
        }

        /// <summary>
        /// Operational status.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public IfOperStatus OperStatus
        {
            get
            {
                return Source.OperStatus;
            }
        }

        /// <summary>
        /// Ipv6 IF Index
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public uint Ipv6IfIndex
        {
            get
            {
                return Source.Ipv6IfIndex;
            }
        }

        /// <summary>
        /// Zone Indices
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public uint[] ZoneIndices
        {
            get
            {
                return Source.ZoneIndices;
            }
        }

        /// <summary>
        /// Get the first <see cref="LPIP_ADAPTER_PREFIX" />
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public LPIP_ADAPTER_PREFIX FirstPrefix
        {
            get
            {
                return Source.FirstPrefix;
            }
        }

        /// <summary>
        /// Current upstream link speed (in bytes).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public FriendlySpeedLong TransmitLinkSpeed
        {
            get
            {
                return Source.TransmitLinkSpeed;
            }
        }

        /// <summary>
        /// Current downstream link speed (in bytes).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(true)]
        public FriendlySpeedLong ReceiveLinkSpeed
        {
            get
            {
                return Source.ReceiveLinkSpeed;
            }
        }

        /// <summary>
        /// First WINS server address.
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public LPADAPTER_MULTICAST_ADDRESS FirstWinsServerAddress
        {
            get
            {
                return Source.FirstWinsServerAddress;
            }
        }

        /// <summary>
        /// First gateway address.
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public LPADAPTER_MULTICAST_ADDRESS FirstGatewayAddress
        {
            get
            {
                return Source.FirstGatewayAddress;
            }
        }

        /// <summary>
        /// Ipv4 Metric
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public uint Ipv4Metric
        {
            get
            {
                return Source.Ipv4Metric;
            }
        }

        /// <summary>
        /// Ipv6 Metric
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public uint Ipv6Metric
        {
            get
            {
                return Source.Ipv6Metric;
            }
        }

        /// <summary>
        /// LUID
        /// </summary>
        /// <returns>A <see cref="LUID"/> structure</returns>
        [Browsable(true)]
        public LUID Luid
        {
            get
            {
                return Source.Luid;
            }
        }

        /// <summary>
        /// Ipv4 DHCP server
        /// </summary>
        /// <returns>A <see cref="SOCKET_ADDRESS"/> structure</returns>
        [Browsable(true)]
        public SOCKET_ADDRESS Dhcp4Server
        {
            get
            {
                return Source.Dhcp4Server;
            }
        }

        /// <summary>
        /// Compartment ID
        /// </summary>
        /// <returns><see cref="UInt32"/></returns>

        [Browsable(true)]
        public uint CompartmentId
        {
            get
            {
                return Source.CompartmentId;
            }
        }

        /// <summary>
        /// Network <see cref="Guid"/>
        /// </summary>
        /// <returns>A <see cref="Guid"/></returns>
        [Browsable(true)]
        public Guid NetworkGuid
        {
            get
            {
                return Source.NetworkGuid;
            }
        }

        /// <summary>
        /// Network connection type
        /// </summary>
        /// <returns>A <see cref="NetIfConnectionType"/> structure</returns>
        [Browsable(true)]
        public NetIfConnectionType ConnectionType
        {
            get
            {
                return Source.ConnectionType;
            }
        }

        /// <summary>
        /// Tunnel type
        /// </summary>
        /// <returns>A <see cref="Network.TunnelType"/> value.</returns>
        [Browsable(true)]
        public TunnelType TunnelType
        {
            get
            {
                return Source.TunnelType;
            }
        }

        /// <summary>
        /// DHCP v6 server
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public SOCKET_ADDRESS Dhcpv6Server
        {
            get
            {
                return Source.Dhcpv6Server;
            }
        }

        /// <summary>
        /// DHCP v6 Client DUID
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public byte[] Dhcpv6ClientDuid
        {
            get
            {
                return Source.Dhcpv6ClientDuid;
            }
        }

        /// <summary>
        /// DHCP v6 Client DUID Length
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public uint Dhcpv6ClientDuidLength
        {
            get
            {
                return Source.Dhcpv6ClientDuidLength;
            }
        }

        /// <summary>
        /// DHCP v6 AIID
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public uint Dhcpv6Iaid
        {
            get
            {
                return Source.Dhcpv6Iaid;
            }
        }

        /// <summary>
        /// First DNS Suffix
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        public LPIP_ADAPTER_DNS_SUFFIX FirstDnsSuffix
        {
            get
            {
                return Source.FirstDnsSuffix;
            }
        }

        /// <summary>
        /// First DNS Suffix
        /// </summary>
        /// <returns></returns>
        [Browsable(true)]
        [TypeConverter(typeof(ArrayConverter))]
        public MIB_IFROW[] PhysicalInterfaces
        {
            get
            {
                return physifaces.ToArray();
            }
        }

        private InternetStatus hasInet = InternetStatus.NotDetermined;

        /// <summary>
        /// Returns a value indicating whether or not this interface is connected to the internet.
        /// </summary>
        public InternetStatus HasInternet
        {
            get => hasInet;
            internal set
            {
                if (hasInet == value) return;
                hasInet = value;

                AssignNewNativeObject(Source, true);
            }
        }

        internal List<MIB_IFROW> PhysIfaceInternal
        {
            get => physifaces;
            set
            {
                physifaces = value;
            }
        }

        /// <summary>
        /// Returns the adapter's friendly name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FriendlyName;
        }

        private bool disposedValue; // To detect redundant calls

        ~NetworkAdapter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose of the object.
        /// </summary>
        /// <param name="disposing">true if disposing, false if called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
            {
                disposedValue = true;
                Source = default;
                deviceIcon = null;
                deviceInfo = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}