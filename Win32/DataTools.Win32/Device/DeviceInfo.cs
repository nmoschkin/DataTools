﻿// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: DeviceInfo Hardware Information Class
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DataTools.Win32
{
    /// <summary>
    /// An object that represents a hardware device on the system.
    /// </summary>
    /// <remarks></remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DeviceInfo : ICloneable
    {
        internal SP_DEVINFO_DATA _devInfo;

        /// <summary>
        /// Backing Store for <see cref="Pid"/>
        /// </summary>
        protected internal ushort _Pid;

        /// <summary>
        /// Backing Store for <see cref="Vid"/>
        /// </summary>
        protected internal ushort _Vid;

        /// <summary>
        /// Backing Store for <see cref="BusReportedDeviceDesc" />
        /// </summary>
        protected string _BusReportedDeviceDesc;

        /// <summary>
        /// Backing Store for <see cref="BusType" />
        /// </summary>
        protected BusType _BusType;

        /// <summary>
        /// Backing Store for <see cref="Characteristics" />
        /// </summary>
        protected DeviceCharacteristcs _Characteristics;

        /// <summary>
        /// Backing Store for <see cref="Children" />
        /// </summary>
        protected string[] _Children;

        /// <summary>
        /// Backing Store for <see cref="ClassName" />
        /// </summary>
        protected string _ClassName;

        /// <summary>
        /// Backing Store for <see cref="ContainerId" />
        /// </summary>
        protected Guid _ContainerId;

        /// <summary>
        /// Backing Store for <see cref="Description" />
        /// </summary>
        protected string _Description;

        /// <summary>
        /// Backing Store for <see cref="DeviceClass" />
        /// </summary>
        protected DeviceClassEnum _DeviceClass;

        /// <summary>
        /// Backing Store for <see cref="DeviceClassGuid" />
        /// </summary>
        protected Guid _DeviceClassGuid;

        /// <summary>
        /// Backing Store for <see cref="DeviceClassIcon" />
        /// </summary>
        protected System.Drawing.Icon _DeviceClassIcon;

        /// <summary>
        /// Backing Store for <see cref="DeviceIcon" />
        /// </summary>
        protected System.Drawing.Icon _DeviceIcon;

        /// <summary>
        /// Backing Store for <see cref="DeviceInterfaceClass" />
        /// </summary>
        protected DeviceInterfaceClassEnum _DeviceInterfaceClass;

        /// <summary>
        /// Backing Store for <see cref="DeviceInterfaceClassGuid" />
        /// </summary>
        protected Guid _DeviceInterfaceClassGuid;

        /// <summary>
        /// Backing Store for <see cref="DevicePath"/>
        /// </summary>
        protected string _DevicePath;

        /// <summary>
        /// Backing Store for <see cref="FriendlyName" />
        /// </summary>
        protected string _FriendlyName;

        /// <summary>
        /// Backing Store for <see cref="HardwareIds"/>
        /// </summary>
        protected string[] _HardwareIds;

        /// <summary>
        /// Backing Store for <see cref="InstallDate" />
        /// </summary>
        protected DateTime _InstallDate;

        /// <summary>
        /// Backing Store for <see cref="InstanceId" />
        /// </summary>
        protected string _InstanceId;

        /// <summary>
        /// Backing Store for <see cref="LinkedChildren" />
        /// </summary>
        protected DeviceInfo[] _LinkedChildren;

        /// <summary>
        /// Backing Store for <see cref="LinkedParent" />
        /// </summary>
        protected DeviceInfo _LinkedParent;

        /// <summary>
        /// Backing Store for <see cref="LocationInfo" />
        /// </summary>
        protected string _LocationInfo;

        /// <summary>
        /// Backing Store for <see cref="LocationPaths" />
        /// </summary>
        protected string[] _LocationPaths;

        /// <summary>
        /// Backing Store for <see cref="Manufacturer" />
        /// </summary>
        protected string _Manufacturer;

        /// <summary>
        /// Backing Store for <see cref="ModelId" />
        /// </summary>
        protected Guid _ModelId;

        /// <summary>
        /// Backing Store for <see cref="Parent" />
        /// </summary>
        protected string _Parent;

        /// <summary>
        /// Backing Store for <see cref="PDOName" />
        /// </summary>
        protected string _PDOName;

        /// <summary>
        /// Backing Store for <see cref="PhysicalPath" />
        /// </summary>
        protected byte[] _PhysicalPath;

        /// <summary>
        /// Backing Store for <see cref="ProductId" />
        /// </summary>
        protected string _ProductId = null;

        /// <summary>
        /// Backing Store for <see cref="RemovalPolicy" />
        /// </summary>
        protected DeviceRemovalPolicy _RemovalPolicy;

        /// <summary>
        /// Backing Store for <see cref="SafeRemovalRequired" />
        /// </summary>
        protected bool _SafeRemovalRequired;

        /// <summary>
        /// Backing Store for <see cref="UINumber" />
        /// </summary>
        protected int _UINumber;

        /// <summary>
        /// Backing Store for <see cref="VendorId" />
        /// </summary>
        protected string _VenderId = null;

        //protected System.Windows.Media.Imaging.BitmapSource _DeviceIcon;

        /// <summary>
        /// Get the Bus-Reported device description.
        /// </summary>
        /// <returns></returns>
        public virtual string BusReportedDeviceDesc
        {
            get => _BusReportedDeviceDesc;
            internal set => _BusReportedDeviceDesc = value;
        }

        /// <summary>
        /// Returns the type of bus that the device is hosted on.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public BusType BusType
        {
            get => _BusType;
            internal set => _BusType = value;
        }

        /// <summary>
        /// Retrieves any device characteristcs associated with the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DeviceCharacteristcs Characteristics
        {
            get => _Characteristics;
            internal set => _Characteristics = value;
        }

        /// <summary>
        /// Returns all child instance ids of this device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] Children
        {
            get => _Children;
            internal set => _Children = value;
        }

        /// <summary>
        /// Gets the device class description.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string ClassDescription
        {
            get
            {
                return DevEnumHelpers.GetEnumDescription(DeviceClass);
            }
        }

        /// <summary>
        /// Gets the device class name.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string ClassName
        {
            get => _ClassName;
            internal set => _ClassName = value;
        }

        /// <summary>
        /// Gets the hardware container Id.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid ContainerId
        {
            get => _ContainerId;
            internal set => _ContainerId = value;
        }

        /// <summary>
        /// Gets the description of the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string Description
        {
            get => _Description;
            internal set => _Description = value;
        }

        /// <summary>
        /// Gets the device class type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual DeviceClassEnum DeviceClass
        {
            get => _DeviceClass;
            internal set => _DeviceClass = value;
        }

        /// <summary>
        /// Returns a detailed description of the device class.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string DeviceClassDescription => DevEnumHelpers.GetEnumDescription(_DeviceClass);

        /// <summary>
        /// Gets the device class <see cref="Guid"/>.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid DeviceClassGuid
        {
            get => _DeviceClassGuid;
            internal set => _DeviceClassGuid = value;
        }

        /// <summary>
        /// Get the system icon image for this hardware device class.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(false)]
        public virtual System.Drawing.Icon DeviceClassIcon
        {
            get => _DeviceClassIcon;
            internal set => _DeviceClassIcon = value;
        }

        /// <summary>
        /// Get the system icon image for this hardware device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(false)]
        public virtual System.Drawing.Icon DeviceIcon
        {
            get
            {
                if (_DeviceIcon == null)
                {
                    _DeviceIcon = _DeviceClassIcon;
                }

                return _DeviceIcon;
            }
            internal set => _DeviceIcon = value;
        }

        /// <summary>
        /// Gets the device class type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual DeviceInterfaceClassEnum DeviceInterfaceClass
        {
            get => _DeviceInterfaceClass;
            internal set => _DeviceInterfaceClass = value;
        }

        /// <summary>
        /// Gets the device interface class guid.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Guid DeviceInterfaceClassGuid
        {
            get => _DeviceInterfaceClassGuid;
            internal set => _DeviceInterfaceClassGuid = value;
        }

        /// <summary>
        /// Get the physical device path that can be used by CreateFile
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string DevicePath
        {
            get => _DevicePath;
            internal set => _DevicePath = value;
        }

        /// <summary>
        /// Gets the friendly name for the device
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string FriendlyName
        {
            get => _FriendlyName ?? _BusReportedDeviceDesc ?? _Description;
            internal set => _FriendlyName = value;
        }

        /// <summary>
        /// Gets all hardware Ids. Hardware Ids contain extractable data, including but not limited to
        /// device vendor id, device product id and USB HID page implementations.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] HardwareIds
        {
            get
            {
                return _HardwareIds;
            }
            internal set
            {
                _HardwareIds = value;
                ParseHardwareIds();
            }
        }

        /// <summary>
        /// Retrieves the install date for the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime InstallDate
        {
            get => _InstallDate;
            internal set => _InstallDate = value;
        }

        /// <summary>
        /// Gets the device instance id which is unique and can be passed to RunDLL property sheet functions and to match children with their parents.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string InstanceId
        {
            get => _InstanceId;
            internal set => _InstanceId = value;
        }

        /// <summary>
        /// Array of linked child DeviceInfo objects.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DeviceInfo[] LinkedChildren
        {
            get => _LinkedChildren;
            internal set => _LinkedChildren = value;
        }

        /// <summary>
        /// The linked parent DeviceInfo object.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DeviceInfo LinkedParent
        {
            get => _LinkedParent;
            internal set => _LinkedParent = value;
        }

        /// <summary>
        /// Gets location information.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string LocationInfo
        {
            get => _LocationInfo;
            internal set => _LocationInfo = value;
        }

        /// <summary>
        /// Gets all hardware location paths.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] LocationPaths
        {
            get => _LocationPaths;
            internal set => _LocationPaths = value;
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string Manufacturer
        {
            get => _Manufacturer;
            internal set => _Manufacturer = value;
        }

        /// <summary>
        /// Gets the Model Id.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual Guid ModelId
        {
            get => _ModelId;
            internal set => _ModelId = value;
        }

        /// <summary>
        /// Returns the parent instance id of this device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Parent
        {
            get => _Parent;
            internal set => _Parent = value;
        }

        /// <summary>
        /// Gets the PDO name. This value is used to match physical disk device instances with their respective interfaces.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PDOName
        {
            get => _PDOName;
            internal set => _PDOName = value;
        }

        /// <summary>
        /// Gets the binary physical path.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] PhysicalPath
        {
            get => _PhysicalPath;
            internal set => _PhysicalPath = value;
        }

        /// <summary>
        /// Gets the product id raw number.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ushort Pid => _Pid;

        /// <summary>
        /// Gets the product Id string, which may or may not be a number.  If it is a number, it is returned as a 4-character (WORD) hexadecimal string.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ProductId => _ProductId ?? _Pid.ToString("X4");

        /// <summary>
        /// Specifies the removal policy of the device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DeviceRemovalPolicy RemovalPolicy
        {
            get => _RemovalPolicy;
            internal set => _RemovalPolicy = value;
        }

        /// <summary>
        /// Specifies whether or not the device must be removed safely.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SafeRemovalRequired
        {
            get => _SafeRemovalRequired;
            internal set => _SafeRemovalRequired = value;
        }

        /// <summary>
        /// Returns a description of the device. The default is the value of ToString().
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string UIDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(Description))
                {
                    return Description;
                }
                else if (!string.IsNullOrEmpty(FriendlyName))
                {
                    return FriendlyName;
                }
                else
                {
                    return ToString();
                }
            }
        }

        /// <summary>
        /// Gets the UINumber
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int UINumber
        {
            get => _UINumber;
            internal set => _UINumber = value;
        }

        /// <summary>
        /// Gets the vendor Id string, which may or may not be a number.  If it is a number, it is returned as a 4-character (WORD) hexadecimal string.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string VendorId => _VenderId ?? _Vid.ToString("X4");

        /// <summary>
        /// Gets the vendor id raw number.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ushort Vid => _Vid;

        /// <summary>
        /// Link all devices in an array of DeviceInfo objects with their relative parent and child objects.
        /// </summary>
        /// <param name="devInfo"></param>
        /// <remarks></remarks>
        public static void LinkDevices(ref DeviceInfo[] devInfo)
        {
            if (devInfo == null) return;

            foreach (var dprep in devInfo)
            {
                dprep.LinkedParent = null;
                dprep.LinkedChildren = null;
            }

            var lchild = new List<DeviceInfo>();

            foreach (var de in devInfo)
            {
                lchild.Clear();

                foreach (var fe in devInfo)
                {
                    if ((de.InstanceId?.ToUpper().Trim() ?? "") == (fe?.Parent?.ToUpper().Trim() ?? ""))
                    {
                        fe.LinkedParent = de;
                        lchild.Add(fe);
                    }
                }

                de._LinkedChildren = lchild.ToArray();
            }
        }

        /// <summary>
        /// Copy the data of this object into the specified <see cref="DeviceInfo"/>-derived object instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="DeviceInfo"/>-derived destination type.</typeparam>
        /// <param name="obj">The object to copy into.</param>
        public virtual void CopyTo<T>(T obj) where T : DeviceInfo
        {
            var f = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var field in f)
            {
                var field2 = typeof(T).GetField(field.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field2 != null)
                {
                    field2.SetValue(obj, field.GetValue(this));
                }
            }
        }

        object ICloneable.Clone()
        {
            return Clone<DeviceInfo>();
        }

        /// <summary>
        /// Clone the data of this object into a new <see cref="DeviceInfo"/> object instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="DeviceInfo"/> object.
        /// </returns>
        public virtual DeviceInfo Clone()
        {
            return Clone<DeviceInfo>();
        }

        /// <summary>
        /// Clone the data of this object into a new <see cref="DeviceInfo"/>-derived object instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="DeviceInfo"/>-derived object type to create.</typeparam>
        /// <returns>
        /// A new <typeparamref name="T"/> object.
        /// </returns>
        public virtual T Clone<T>() where T : DeviceInfo, new()
        {
            var result = new T();
            CopyTo(result);
            return result;
        }

        /// <summary>
        /// Display the system device properties dialog page for this device.
        /// </summary>
        /// <remarks></remarks>
        public virtual void ShowDevicePropertiesDialog(IntPtr? hwnd = null)
        {
            DevPropDialog.OpenDeviceProperties(InstanceId, hwnd ?? IntPtr.Zero);
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            string d = Description;
            string f = FriendlyName;
            if (d is null && f is null)
            {
                return InstanceId;
            }
            else if (d is null && f is object)
            {
                return f;
            }
            else
            {
                return d;
            }
        }
                
        /// <summary>
        /// Parses the <see cref="HardwareIds"/> into <see cref="VendorId"/> and <see cref="ProductId"/>, if possible.
        /// </summary>
        /// <remarks></remarks>
        /// <returns>True if the value was successfully parsed into intelligible <see cref="VendorId"/> and <see cref="ProductId"/> values.</returns>
        protected virtual bool ParseHardwareIds()
        {
            var s = TextTools.Split(_HardwareIds[0], @"\");
            if (s is null || s.Length < 2) return false;

            string[] v;
            
            if (s[1].IndexOf(";") > 0)
            {
                v = TextTools.Split(s[1], ";");
            }
            else
            {
                v = TextTools.Split(s[1], "&");
            }

            foreach (string vp in v)
            {
                var x = TextTools.Split(vp, "_");
                if (x.Length < 2) continue;

                switch (x[0] ?? "")
                {
                    case "VID":
                        {
                            if (!ushort.TryParse(x[1], System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out _Vid))
                            {
                                _VenderId = x[1];
                            }

                            break;
                        }

                    case "PID":
                        {
                            if (!ushort.TryParse(x[1], System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out _Pid))
                            {
                                _ProductId = x[1];
                            }

                            break;
                        }

                    case "VEN":
                        {
                            if (!ushort.TryParse(x[1], System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out _Vid))
                            {
                                _VenderId = x[1];
                            }

                            break;
                        }

                    case "DEV":
                        {
                            if (!ushort.TryParse(x[1], System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out _Pid))
                            {
                                _ProductId = x[1];
                            }

                            break;
                        }
                }
            }

            return (_VenderId != null && _ProductId != null) || (_Vid != 0 && _Pid != 0);

        }
    }
}