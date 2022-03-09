// ************************************************* ''
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: BlueTooth information (TO DO)
// 
// Copyright (C) 2011-2020 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the MIT License   
// ************************************************* ''


using System.Diagnostics;
using System.Runtime.InteropServices;
using DataTools.Win32;
using static DataTools.Win32.DeviceEnum;

namespace DataTools.Hardware
{


    // TO DO!  
    
    public class BluetoothDeviceInfo : DeviceInfo
    {
        protected BLUETOOTH_ADDRESS _bthaddr;
        protected string _service;
        protected string _major;
        protected string _minor;
        protected uint _btClass;
        protected bool _isradio;





        /// <summary>
        /// Enumerate Bluetooth Radios
        /// </summary>
        /// <returns>Array of <see cref="BluetoothDeviceInfo"/> objects.</returns>
        public static BluetoothDeviceInfo[] EnumBluetoothRadios()
        {
            var bth = Bluetooth._internalEnumBluetoothRadios();
            var p = InternalEnumerateDevices<BluetoothDeviceInfo>(Bluetooth.GUID_BTHPORT_DEVICE_INTERFACE, ClassDevFlags.DeviceInterface | ClassDevFlags.Present);
            if (p is object && p.Count() > 0)
            {
                foreach (var x in p)
                {
                    foreach (var y in bth)
                    {
                        int i = x.InstanceId.LastIndexOf(@"\");
                        if (i > -1)
                        {
                            string s = x.InstanceId.Substring(i + 1);
                            ulong res;
                            bool b = ulong.TryParse(s, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out res);
                            if (b)
                            {
                                if (res == (ulong)(y.address))
                                {
                                    x.IsRadio = true;
                                    x.BluetoothAddress = y.address;
                                    x.BluetoothDeviceClass = y.ulClassofDevice;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (p is null)
                return null;
            Array.Sort(p, new Comparison<DeviceInfo>((x, y) => { if (x.FriendlyName is object && y.FriendlyName is object) { return string.Compare(x.FriendlyName, y.FriendlyName); } else { return string.Compare(x.Description, y.Description); } }));
            return p;
        }

        /// <summary>
        /// Enumerate Bluetooth devices
        /// </summary>
        /// <returns>Array of <see cref="BluetoothDeviceInfo"/> objects.</returns>
        public static BluetoothDeviceInfo[] EnumBluetoothDevices()
        {
            var lOut = new List<BluetoothDeviceInfo>();

            var bth = Bluetooth._internalEnumBluetoothDevices();

            var p = InternalEnumerateDevices<BluetoothDeviceInfo>(DevProp.GUID_DEVCLASS_BLUETOOTH, ClassDevFlags.Present);

            if (p != null && p.Length > 0)
            {
                foreach (var x in p)
                {
                    foreach (var y in bth)
                    {
                        int i = x.InstanceId.LastIndexOf("_");

                        if (i > -1)
                        {
                            string s = x.InstanceId.Substring(i + 1);

                            ulong res;

                            bool b = ulong.TryParse(s, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out res);

                            if (b)
                            {
                                if (res == (ulong)y.Address)
                                {
                                    x.IsRadio = false;

                                    x.BluetoothAddress = y.Address;
                                    x.BluetoothDeviceClass = y.ulClassofDevice;

                                    lOut.Add(x);

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return lOut.ToArray();

            //Array.Sort(p, new Comparison<DeviceInfo>((x, y) => { if (x.FriendlyName is object && y.FriendlyName is object) { return string.Compare(x.FriendlyName, y.FriendlyName); } else { return string.Compare(x.Description, y.Description); } }));
            //return p;
        }


        public virtual bool IsRadio
        {
            get
            {
                return _isradio;
            }

            internal set
            {
                _isradio = value;
            }
        }

        public virtual uint BluetoothDeviceClass
        {
            get
            {
                return _btClass;
            }

            internal set
            {
                _btClass = value;
                var svc = default(ushort);
                var maj = default(ushort);
                var min = default(ushort);
                Bluetooth.ParseClass(_btClass, ref svc, ref maj, ref min);
                _service = Bluetooth.PrintServiceClass(svc);
                _major = Bluetooth.PrintMajorClass(maj);
                _minor = Bluetooth.PrintMinorClass(maj, min);
            }
        }


        /// <summary>
    /// Return the Bluetooth MAC address for this radio
    /// </summary>
    /// <returns></returns>
        public virtual BLUETOOTH_ADDRESS BluetoothAddress
        {
            get
            {
                return _bthaddr;
            }

            internal set
            {
                _bthaddr = value;
            }
        }

        public virtual string BluetoothServiceClasses
        {
            get
            {
                return _service;
            }

            protected set
            {
                _service = value;
            }
        }

        public virtual string BluetoothMajorClasses
        {
            get
            {
                return _major;
            }

            protected set
            {
                _major = value;
            }
        }

        public virtual string BluetoothMinorClasses
        {
            get
            {
                return _minor;
            }

            protected set
            {
                _minor = value;
            }
        }

        public override string UIDescription
        {
            get
            {
                if (string.IsNullOrEmpty(FriendlyName))
                    return base.UIDescription;
                else
                    return FriendlyName;
            }
        }

        public static void ShowBluetoothSettings()
        {
            var shex = new SHELLEXECUTEINFO();
            shex.cbSize = Marshal.SizeOf(shex);
            shex.nShow = User32.SW_SHOW;
            shex.hInstApp = Process.GetCurrentProcess().Handle;
            // shex.hWnd = 
            // shex.lpVerb = "properties"

            // Set the parsing name exactly this way.
            shex.lpFile = "ms-settings:bluetooth";
            shex.fMask = User32.SEE_MASK_ASYNCOK | User32.SEE_MASK_FLAG_DDEWAIT | User32.SEE_MASK_UNICODE;
            User32.ShellExecuteEx(ref shex);

            // Shell("ms-settings:bluetooth")
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(FriendlyName))
                return base.ToString();
            else
                return FriendlyName;
        }
    }
}


