// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: DeviceEnum
//         Native.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Desktop;
using DataTools.Shell.Native;
using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

// TODO: Refactor this entire module

using static DataTools.Win32.DevProp;

namespace DataTools.Win32
{
    /// <summary>
    /// Internal device enumeration functions module.
    /// </summary>
    /// <remarks></remarks>
    internal static class DeviceEnum
    {
        public const int DICLASSPROP_INTERFACE = 2;
        public const int DICLASSPROP_INSTALLER = 1;

        [ThreadStatic]
        private static DEVPROPKEY dkref;

        private static ref DEVPROPKEY GetDevPropRef(DEVPROPKEY val)
        {
            dkref = val;
            return ref dkref;
        }

        /// <summary>
        /// Enumerate (and optionally link) all computer hardware on the system.
        /// </summary>
        /// <param name="noLink">Optionally specify not to link parents to children.</param>
        /// <returns>An array of DeviceInfo objects.</returns>
        /// <remarks></remarks>
        public static DeviceInfo[] InternalGetComputer(bool noLink = false, Guid classOnly = default)
        {
            DeviceInfo[] devOut = null;
     
            int c = 0;
            var devInfo = default(SP_DEVINFO_DATA);
            var devInterface = default(SP_DEVICE_INTERFACE_DATA);
            var lIcon = new Dictionary<Guid, System.Drawing.Icon>();
            
            var hicon = IntPtr.Zero;
            int picon = 0;

            System.Drawing.Icon icn = null;
            IntPtr hDev;

            // classOnly = Guid.Empty

            if (classOnly != Guid.Empty)
            {
                hDev = SetupDiGetClassDevs(classOnly, IntPtr.Zero, IntPtr.Zero, (ClassDevFlags)DIGCF_PRESENT);
            }
            else
            {
                hDev = SetupDiGetClassDevs(Guid.Empty, IntPtr.Zero, IntPtr.Zero, (ClassDevFlags)(DIGCF_ALLCLASSES | DIGCF_PRESENT | DIGCF_DEVICEINTERFACE));
            }

            if (hDev == INVALID_HANDLE_VALUE)
            {
                return null;
            }

            devInfo.cbSize = (uint)Marshal.SizeOf(devInfo);
            devInterface.cbSize = (uint)Marshal.SizeOf(devInterface);

            while (SetupDiEnumDeviceInfo(hDev, (uint)c, out devInfo))
            {
                try
                {
                    Array.Resize(ref devOut, c + 1);

                    devOut[c] = PopulateDeviceInfo<DeviceInfo>(hDev, default, devInfo, devInterface);
                    SetupDiEnumDeviceInterfaces(hDev, IntPtr.Zero, devOut[c].DeviceClassGuid, (uint)c, ref devInterface);

                    if (devInterface.InterfaceClassGuid != Guid.Empty)
                    {
                        devOut[c].DeviceInterfaceClassGuid = devInterface.InterfaceClassGuid;
                        devOut[c].DeviceInterfaceClass = DevEnumHelpers.GetDevInterfaceClassEnumFromGuid(devOut[c].DeviceInterfaceClassGuid);
                    }

                    if (!lIcon.ContainsKey(devOut[c].DeviceClassGuid))
                    {
                        SetupDiLoadClassIcon(devOut[c].DeviceClassGuid, ref hicon, ref picon);

                        if (hicon != IntPtr.Zero)
                        {
                            icn = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hicon).Clone();
                            User32.DestroyIcon(hicon);
                        }

                        lIcon.Add(devOut[c].DeviceClassGuid, icn);
                        devOut[c].DeviceClassIcon = icn;
                    }
                    else
                    {
                        var argvalue = devOut[c].DeviceClassIcon;
                        lIcon.TryGetValue(devOut[c].DeviceClassGuid, out argvalue);
                        devOut[c].DeviceClassIcon = argvalue;
                    }
                }
                catch
                {
                    //Interaction.MsgBox(ex.Message + "\r\n" + "\r\n" + "Stack Trace: " + ex.StackTrace, MsgBoxStyle.Exclamation);
                }

                c += 1;
            }

            SetupDiDestroyDeviceInfoList(hDev);
            if (!noLink)
                DeviceInfo.LinkDevices(ref devOut);
            return devOut;
        }

        public static List<DEVPROPKEY> EnumerateDeviceProperties(DeviceInfo info, ClassDevFlags flags = ClassDevFlags.Present | ClassDevFlags.DeviceInterface, bool useClassId = true)
        {
            var nkey = new List<DEVPROPKEY>();
            uint c = 0U;

            MemPtr mm = new MemPtr();

            IntPtr hDev;
            Guid cid;

            if (useClassId)
            {
                cid = info.DeviceClassGuid;
            }
            else
            {
                cid = info.DeviceInterfaceClassGuid;
            }

            hDev = SetupDiGetClassDevs(cid, IntPtr.Zero, IntPtr.Zero, flags);
            if (hDev == INVALID_HANDLE_VALUE)
            {
                return null;
            }

            SetupDiGetDevicePropertyKeys(hDev, ref info._devInfo, IntPtr.Zero, 0U, ref c, 0U);
            if (c == 0L)
            {
                SetupDiDestroyDeviceInfoList(hDev);
                return null;
            }

            int devpsize = Marshal.SizeOf(DEVPKEY_Device_Address);

            mm.Alloc(c * devpsize);

            SetupDiGetDevicePropertyKeys(hDev, ref info._devInfo, mm, c, ref c, 0U);
            SetupDiDestroyDeviceInfoList(hDev);

            for (int i = 0; i < c; i++)
                nkey.Add(mm.ToStructAt<DEVPROPKEY>(i * devpsize));

            mm.Free();
            return nkey;
        }

        /// <summary>
        /// Get an arbitrary device property from a previously-enumerated device.
        /// </summary>
        /// <param name="info">The DeviceInfo object of the device.</param>
        /// <param name="prop">The DevPropKey value to retrieve.</param>
        /// <param name="type">The property type of the value to retrieve.</param>
        /// <param name="flags">Optional flags to pass.</param>
        /// <param name="useClassId">Optional alternate class Id or interface Id to use.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object GetDeviceProperty(
            DeviceInfo info,
            DEVPROPKEY prop,
            DevPropTypes type,
            ClassDevFlags flags = ClassDevFlags.Present | ClassDevFlags.DeviceInterface,
            bool useClassId = false
            )
        {
            object ires;

            uint c;

            using (var mm = new SafePtr())
            {
                IntPtr hDev;
                Guid cid;

                if (useClassId)
                {
                    cid = info.DeviceClassGuid;
                }
                else
                {
                    cid = info.DeviceInterfaceClassGuid;
                }

                hDev = SetupDiGetClassDevs(cid, IntPtr.Zero, IntPtr.Zero, flags);

                if (hDev == INVALID_HANDLE_VALUE)
                {
                    return null;
                }

                uint utype = (uint)type;

                SetupDiGetDeviceProperty(hDev, ref info._devInfo, ref prop, out utype, IntPtr.Zero, 0U, out c, 0U);

                if (c == 0L)
                {
                    // MsgBox(FormatLastError(Marshal.GetLastWin32Error))
                    SetupDiDestroyDeviceInfoList(hDev);
                    return null;
                }

                mm.AllocZero(c);

                SetupDiGetDeviceProperty(hDev, ref info._devInfo, ref prop, out utype, mm, c, out c, 0U);
                SetupDiDestroyDeviceInfoList(hDev);

                ires = DevPropToObject(type, mm, (int)c);

                return ires;
            }
        }

        /// <summary>
        /// Enumerate devices and return an instance of a specific descendent of DeviceInfo.
        /// </summary>
        /// <typeparam name="T">The DeviceInfo type of objects to return.</typeparam>
        /// <param name="ClassId">The Class Id or Interface Id to enumerate.</param>
        /// <param name="flags">Optional flags.</param>
        /// <returns>An array of T</returns>
        /// <remarks></remarks>
        public static T[] InternalEnumerateDevices<T>(Guid ClassId, ClassDevFlags flags = ClassDevFlags.Present | ClassDevFlags.DeviceInterface) where T : DeviceInfo, new()
        {
            T[] devOut = null;

            int c = 0;

            uint cu = 0U;

            var devInfo = default(SP_DEVINFO_DATA);
            var devInterface = default(SP_DEVICE_INTERFACE_DATA);

            var lIcon = new Dictionary<Guid, System.Drawing.Icon>();

            var mm = new SafePtr();
            var hicon = IntPtr.Zero;

            int picon = 0;

            System.Drawing.Icon icn = null;

            var hDev = SetupDiGetClassDevs(ClassId, IntPtr.Zero, IntPtr.Zero, flags);

            if (hDev == INVALID_HANDLE_VALUE)
            {
                return null;
            }

            devInfo.cbSize = (uint)Marshal.SizeOf(devInfo);
            devInterface.cbSize = (uint)Marshal.SizeOf(devInterface);

            SetupDiLoadClassIcon(ClassId, ref hicon, ref picon);

            if (hicon != IntPtr.Zero)
            {
                icn = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hicon).Clone();
                User32.DestroyIcon(hicon);
            }

            if ((flags & ClassDevFlags.DeviceInterface) == ClassDevFlags.DeviceInterface)
            {
                while (SetupDiEnumDeviceInterfaces(hDev, IntPtr.Zero, ClassId, cu, ref devInterface))
                {
                    c = (int)cu;

                    Array.Resize(ref devOut, c + 1);
                    SetupDiEnumDeviceInfo(hDev, cu, out devInfo);

                    devOut[c] = PopulateDeviceInfo<T>(hDev, ClassId, devInfo, devInterface);
                    devOut[c].DeviceClassIcon = icn;

                    cu += 1;
                }
            }
            else
            {
                while (SetupDiEnumDeviceInfo(hDev, cu, out devInfo))
                {
                    c = (int)cu;
                    Array.Resize(ref devOut, c + 1);

                    devOut[c] = PopulateDeviceInfo<T>(hDev, default, devInfo, devInterface);

                    SetupDiEnumDeviceInterfaces(hDev, IntPtr.Zero, devOut[c].DeviceClassGuid, cu, ref devInterface);

                    if (devInterface.InterfaceClassGuid != Guid.Empty)
                    {
                        devOut[c].DeviceInterfaceClassGuid = devInterface.InterfaceClassGuid;
                        devOut[c].DeviceInterfaceClass = DevEnumHelpers.GetDevInterfaceClassEnumFromGuid(devOut[c].DeviceInterfaceClassGuid);
                    }

                    devOut[c].DeviceClassIcon = icn;
                    cu += 1;
                }

                // If c = 0 Then
                // MsgBox("Internal error: " & FormatLastError(Marshal.GetLastWin32Error))
                // End If
            }

            mm.Dispose();
            SetupDiDestroyDeviceInfoList(hDev);
            return devOut;
        }

        public static System.Drawing.Icon GetClassIcon(Guid ClassId)
        {
            var hicon = IntPtr.Zero;

            System.Drawing.Icon icn = null;

            var hDev = SetupDiGetClassDevs(ClassId, IntPtr.Zero, IntPtr.Zero, 0);

            if (hDev == INVALID_HANDLE_VALUE)
            {
                return null;
            }

            int atmp = default;

            SetupDiLoadClassIcon(ClassId, ref hicon, ref atmp);

            if (hicon != IntPtr.Zero)
            {
                icn = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hicon).Clone();
                User32.DestroyIcon(hicon);
            }

            SetupDiDestroyDeviceInfoList(hDev);
            return icn;
        }

        internal static readonly DeviceClassEnum[] StandardStagingClasses = new[] { DeviceClassEnum.Adapter, DeviceClassEnum.Battery, DeviceClassEnum.Biometric, DeviceClassEnum.Bluetooth, DeviceClassEnum.Infrared, DeviceClassEnum.HidClass, DeviceClassEnum.Infrared, DeviceClassEnum.Keyboard, DeviceClassEnum.Media, DeviceClassEnum.Monitor, DeviceClassEnum.Mouse, DeviceClassEnum.Multifunction, DeviceClassEnum.PnpPrinters, DeviceClassEnum.Printer, DeviceClassEnum.PrinterQueue, DeviceClassEnum.Sound, DeviceClassEnum.Usb };
        internal static readonly DeviceClassEnum[] ExtraStagingClasses = new[] { DeviceClassEnum.Adapter, DeviceClassEnum.Battery, DeviceClassEnum.Biometric, DeviceClassEnum.Bluetooth, DeviceClassEnum.CdRom, DeviceClassEnum.DiskDrive, DeviceClassEnum.FloppyDisk, DeviceClassEnum.Infrared, DeviceClassEnum.HidClass, DeviceClassEnum.Infrared, DeviceClassEnum.Keyboard, DeviceClassEnum.Media, DeviceClassEnum.MediumChanger, DeviceClassEnum.Modem, DeviceClassEnum.Monitor, DeviceClassEnum.Mouse, DeviceClassEnum.Multifunction, DeviceClassEnum.Pcmcia, DeviceClassEnum.PnpPrinters, DeviceClassEnum.Printer, DeviceClassEnum.PrinterQueue, DeviceClassEnum.PrinterUpgrade, DeviceClassEnum.SmartCardReader, DeviceClassEnum.Sound, DeviceClassEnum.TapeDrive, DeviceClassEnum.Usb };

        /// <summary>
        /// Gets the device icon in some way, including looking at the device stage to get the photo-realistic icons from the Devices and Printers control panel folder.
        /// </summary>
        /// <param name="hDev"></param>
        /// <param name="devInfo"></param>
        /// <param name="infoOut"></param>
        /// <remarks></remarks>
        public static void GetDeviceIcon(IntPtr hDev, SP_DEVINFO_DATA devInfo, DeviceInfo infoOut, DeviceClassEnum[] stagingClasses = null, bool noStaging = false)
        {
            IntPtr hIcon = new IntPtr();
            if (stagingClasses is null)
            {
                stagingClasses = StandardStagingClasses;
            }

            if (infoOut.DeviceIcon is null)
            {
                string sKey = @"::{20D04FE0-3AEA-1069-A2D8-08002B30309D}\::{21EC2020-3AEA-1069-A2DD-08002B30309D}\::{A8A91A66-3A7D-4424-8D24-04E180695C7A}";
                string pKey = @"\Provider%5CMicrosoft.Base.DevQueryObjects//DDO:";

                if (infoOut.ContainerId != Guid.Empty & noStaging == false)
                {
                    if (stagingClasses.Contains(infoOut.DeviceClass))
                    {
                        string rKey = "%7B" + infoOut.ContainerId.ToString("D").ToUpper() + "%7D";

                        var mm = new MemPtr();

                        string s = sKey + pKey + rKey;

                        ShellFileGetAttributesOptions argpsfgaoOut = 0;
                        NativeShell.SHParseDisplayName(s, IntPtr.Zero, out mm.handle, (ShellFileGetAttributesOptions)0, out argpsfgaoOut);

                        if (mm != IntPtr.Zero)
                        {
                            infoOut.DeviceIcon = Resources.GetItemIcon(mm, (Resources.SystemIconSizes)(User32.SHIL_JUMBO));
                            mm.Free();
                        }
                    }
                }

                if (infoOut.DeviceIcon is null)
                {
                    if (SetupDiLoadDeviceIcon(hDev, ref devInfo, 64U, 64U, 0U, ref hIcon))
                    {
                        var icn = System.Drawing.Icon.FromHandle(hIcon);
                        System.Drawing.Icon icn2 = (System.Drawing.Icon)icn.Clone();
                        User32.DestroyIcon(hIcon);
                        icn.Dispose();
                        infoOut.DeviceIcon = icn2;
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates and populates a DeviceInfo-derived object with most of the common properties using an open handle to a device enumerator.
        /// </summary>
        /// <typeparam name="T">The type of DeviceInfo-derived object to return.</typeparam>
        /// <param name="hDev">A valid device enumerator handle.</param>
        /// <param name="ClassId">The class or interface Id.</param>
        /// <param name="devInfo">The raw SP_DEVINFO_DATA</param>
        /// <param name="devInterface">The raw SP_DEVICE_INTERFACE_DATA</param>
        /// <param name="mm">An open memory object.</param>
        /// <returns>A new instance of T.</returns>
        /// <remarks></remarks>
        public static T PopulateDeviceInfo<T>(IntPtr hDev, Guid ClassId, SP_DEVINFO_DATA devInfo, SP_DEVICE_INTERFACE_DATA devInterface) where T : DeviceInfo, new()
        {
            var devOut = new T();
            uint cbSize;
            uint propVal;

            //var mm = new SafePtr();

            var mm = new SafePtr();
            var details = default(SP_DEVICE_INTERFACE_DETAIL_DATA);

            MemPtr pt;

            var sb = new System.Text.StringBuilder();

            try
            {
                devOut._devInfo = devInfo;

                devOut.DeviceInterfaceClassGuid = devInterface.InterfaceClassGuid;
                devOut.DeviceInterfaceClass = DevEnumHelpers.GetDevInterfaceClassEnumFromGuid(devOut.DeviceInterfaceClassGuid);

                // Get the DevicePath from DeviceInterfaceDetail
                mm.Length = 0L;
                mm.Length = Marshal.SizeOf(details);

                SetupDiGetDeviceInterfaceDetail(hDev, ref devInterface, IntPtr.Zero, 0U, out cbSize, IntPtr.Zero);
                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.IntAt(0L) = IntPtr.Size == 4 ? 6 : 8;

                    if (cbSize > 4L)
                    {
                        uint argRequiredSize = default;
                        if (SetupDiGetDeviceInterfaceDetail(hDev, ref devInterface, mm.Handle, cbSize, out argRequiredSize, default))
                        {
                            // mm.PullIn(0, 4)
                            devOut.DevicePath = mm.GetString(4L);
                        }
                    }

                    sb.AppendLine("Getting DeviceInterfaceDetail Succeed");
                }
                // ClassGuid

                sb.AppendLine("Property ClassGuid");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_GUID;

                DEVPROPKEY prop = DEVPKEY_Device_ClassGuid;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_ClassGuid), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_GUID;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_ClassGuid), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                    {
                        devOut.DeviceClassGuid = new Guid(mm.ToByteArray(0L, 16));
                        devOut.DeviceClass = DevEnumHelpers.GetDevClassEnumFromGuid(devOut.DeviceClassGuid);
                    }
                }
                // InterfaceClassGuid

                sb.AppendLine("Property InterfaceClassGuid");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_GUID;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_DeviceInterface_ClassGuid), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_GUID;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_DeviceInterface_ClassGuid), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                    {
                        devOut.DeviceInterfaceClassGuid = new Guid(mm.ToByteArray(0L, 16));
                        devOut.DeviceInterfaceClass = DevEnumHelpers.GetDevInterfaceClassEnumFromGuid(devOut.DeviceInterfaceClassGuid);
                    }
                }

                // InstallDate

                sb.AppendLine("Property InstallDate");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_FILETIME;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_InstallDate), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_FILETIME;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_InstallDate), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.InstallDate = (DateTime)DevPropToObject(DevPropTypes.FileTime, mm, (int)cbSize);
                }
                // Characteristics

                sb.AppendLine("Property Characteristics");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_INT32;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Characteristics), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_INT32;
                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Characteristics), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.Characteristics = (DeviceCharacteristcs)mm.IntAt(0L);
                }
                // Removal Policy

                sb.AppendLine("Property Removal Policy");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_INT32;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_RemovalPolicy), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_INT32;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_RemovalPolicy), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.RemovalPolicy = (DeviceRemovalPolicy)mm.IntAt(0L);
                }
                // Safe Removal Required

                sb.AppendLine("Property Safe Removal Required");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_BOOLEAN;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_SafeRemovalRequired), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_BOOLEAN;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_SafeRemovalRequired), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.SafeRemovalRequired = mm.ByteAt(0L) == 1 ? true : false;
                }

                // BusType

                sb.AppendLine("Property BusType");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_GUID;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_BusTypeGuid), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_GUID;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_BusTypeGuid), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.BusType = DevPropDialog.GuidToBusType(new Guid(mm.ToByteArray(0L, 16)));
                }
                // ContainerId

                sb.AppendLine("Property ContainerId");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_GUID;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_ContainerId), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_GUID;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_ContainerId), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.ContainerId = new Guid(mm.ToByteArray(0L, 16));
                }

                // Children

                sb.AppendLine("Property Children");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING_LIST;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Children), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = (cbSize + 4L) * 2L;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING_LIST;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Children), out propVal, mm.Handle, cbSize, out cbSize, 0U);

                    pt = mm.Handle;

                    if (cbSize > 0L)
                        devOut.Children = pt.GetStringArray(0L);
                }
                // HardwareIds

                sb.AppendLine("Property HardwareIds");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING_LIST;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_HardwareIds), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = (cbSize + 4L) * 2L;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING_LIST;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_HardwareIds), out propVal, mm, cbSize, out cbSize, 0U);

                    pt = mm.Handle;

                    if (cbSize > 0L)
                        devOut.HardwareIds = pt.GetStringArray(0L);
                }
                // LocationPaths

                sb.AppendLine("Property LocationPaths");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING_LIST;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_LocationPaths), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING_LIST;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_LocationPaths), out propVal, mm, cbSize, out cbSize, 0U);

                    pt = mm.Handle;

                    if (cbSize > 0L)
                        devOut.LocationPaths = pt.GetStringArray(0L);
                }
                // Parent Device

                sb.AppendLine("Property Parent Device");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Parent), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Parent), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.Parent = mm.ToString();
                }
                // Location Info

                sb.AppendLine("Property Location Info");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_LocationInfo), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_LocationInfo), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.LocationInfo = mm.ToString();
                }
                // Physical Device Location

                sb.AppendLine("Property Physical Device Location");
                sb.AppendLine("Getting cbSize");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_BINARY;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_PhysicalDeviceLocation), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);
                sb.AppendLine("cbSize is " + cbSize);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    sb.AppendLine("Calling to get Physical Device Location");

                    propVal = DEVPROP_TYPE_BINARY;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_PhysicalDeviceLocation), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                    {
                        sb.AppendLine("Grabbing bytes");
                        devOut.PhysicalPath = mm.ToByteArray(0L, (int)cbSize);
                    }
                }
                // PDOName

                sb.AppendLine("Property PDOName");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_PDOName), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_PDOName), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.PDOName = mm.ToString();
                }
                // Description

                sb.AppendLine("Property Description");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_DeviceDesc), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_DeviceDesc), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.Description = mm.ToString();
                }
                // ClassName

                sb.AppendLine("Property ClassName");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Class), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();
                    propVal = DEVPROP_TYPE_STRING;
                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Class), out propVal, mm, cbSize, out cbSize, 0U);
                    if (cbSize > 0L)
                        devOut.ClassName = mm.ToString();
                }

                // Manufacturer

                sb.AppendLine("Property Manufacturer");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Manufacturer), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();
                    propVal = DEVPROP_TYPE_STRING;
                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_Manufacturer), out propVal, mm, cbSize, out cbSize, 0U);
                    if (cbSize > 0L)
                        devOut.Manufacturer = mm.ToString();
                }

                // Model

                sb.AppendLine("Property BusReportedDeviceDesc (string)");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_BusReportedDeviceDesc), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();
                    propVal = DEVPROP_TYPE_STRING;
                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_BusReportedDeviceDesc), out propVal, mm, cbSize, out cbSize, 0U);
                    if (cbSize > 0L)
                        devOut.BusReportedDeviceDesc = mm.ToString();
                }

                // ModelId

                sb.AppendLine("Property ModelId");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_GUID;
                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_ModelId), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_GUID;
                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_ModelId), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                    {
                        devOut.ModelId = mm.GuidAt(0L);
                    }
                }

                // UINumber

                sb.AppendLine("Property UINumber");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_INT32;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_UINumber), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_INT32;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_UINumber), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.UINumber = mm.IntAt(0L);
                }

                // FriendlyName
                sb.AppendLine("Property FriendlyName");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_FriendlyName), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_FriendlyName), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.FriendlyName = mm.ToString();
                }
                // InstanceId

                sb.AppendLine("Property InstanceId");

                cbSize = 0U;
                propVal = DEVPROP_TYPE_STRING;

                SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_InstanceId), out propVal, IntPtr.Zero, 0U, out cbSize, 0U);

                if (cbSize > 0L)
                {
                    mm.Length = cbSize;
                    mm.ZeroMemory();

                    propVal = DEVPROP_TYPE_STRING;

                    SetupDiGetDeviceProperty(hDev, ref devInfo, ref GetDevPropRef(DEVPKEY_Device_InstanceId), out propVal, mm, cbSize, out cbSize, 0U);

                    if (cbSize > 0L)
                        devOut.InstanceId = mm.ToString();
                }

                // Get the device icon
                GetDeviceIcon(hDev, devInfo, devOut);
                return devOut;
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.Message);
                sb.AppendLine(NativeError.FormatLastError((uint)Marshal.GetLastWin32Error()));

                //Interaction.MsgBox(ex.Message + " : See " + dumpFile + " for more clues." + "\r\n" + NativeError.FormatLastError((uint)Marshal.GetLastWin32Error()));

                return devOut;
            }
        }

        /// <summary>
        /// Digests a device property type and raw data and returns the equivalent CLR object.
        /// </summary>
        /// <param name="type">The property type.</param>
        /// <param name="data">The raw data pointer.</param>
        /// <param name="length">The length of the data.</param>
        /// <returns>A managed-memory object equivalent.</returns>
        /// <remarks></remarks>
        public static object DevPropToObject(DevPropTypes type, IntPtr data, int length = 0)
        {
            DataTools.Memory.MemPtr mm = data;
            
            switch (type)
            {
                case DevPropTypes.Binary:
                    return mm.ToByteArray(0L, length);

                case DevPropTypes.Boolean:
                    return mm.ByteAt(0L) != 0;

                case DevPropTypes.Byte:
                    return mm.ByteAt(0L);

                case DevPropTypes.SByte:
                    return mm.SByteAt(0L);

                case DevPropTypes.Int16:
                    return mm.ShortAt(0L);

                case DevPropTypes.UInt16:
                    return mm.UShortAt(0L);

                case DevPropTypes.Int32:
                    return mm.IntAt(0L);

                case DevPropTypes.UInt32:
                    return mm.UIntAt(0L);

                case DevPropTypes.Int64:
                    return mm.LongAt(0L);

                case DevPropTypes.UInt64:
                    return mm.ULongAt(0L);

                case DevPropTypes.Currency:
                    return mm.DecimalAt(0L);

                case DevPropTypes.Float:
                    return mm.SingleAt(0L);

                case DevPropTypes.Date:
                    // based on what the MSDN describes of this property format, this is what
                    // I believe needs to be done to make the value into an acceptable CLR DateTime object.
                    double d = mm.DoubleAt(0L);

                    var t = new TimeSpan(0, 0, (int)(d * 24d * 60d * 60d));
                    var dt = new DateTime(1899, 12, 31, 0, 0, 0);

                    return dt + t;

                case DevPropTypes.Decimal:
                    return mm.DecimalAt(0L);

                case DevPropTypes.FileTime:
                    return mm.ToStruct<FILETIME>().ToDateTime();

                case DevPropTypes.DevPropKey:
                    return mm.ToStruct<DEVPROPKEY>();

                case DevPropTypes.Guid:
                    return mm.GuidAt(0L);

                case DevPropTypes.SecurityDescriptor:
                    return mm.ToStruct<SecurityDescriptor.SECURITY_DESCRIPTOR>();

                case DevPropTypes.String:
                    return mm.ToString();

                case DevPropTypes.StringList:
                    return mm.GetStringArray(0L);

                case DevPropTypes.DevPropType:
                    return mm.IntAt(0L);

                case DevPropTypes.SecurityDescriptorString:
                    return mm.ToString();

                case DevPropTypes.StringIndirect:
                    // load the string resource, itself, from the file.
                    return Resources.LoadStringResource(mm.ToString());

                case DevPropTypes.NTStatus:
                    return mm.IntAt(0L);
            }

            return null;
        }

        /// <summary>
        /// Enumerate devices of DeviceInfo T with the specified hardware class Id.
        /// </summary>
        /// <typeparam name="T">A DeviceInfo-based object.</typeparam>
        /// <param name="ClassId">A system GUID_DEVINTERFACE or GUID_DEVCLASS value.</param>
        /// <param name="flags">Optional flags to pass to the enumerator.</param>
        /// <returns>An array of T</returns>
        /// <remarks></remarks>
        public static T[] EnumerateDevices<T>(Guid ClassId, ClassDevFlags flags = ClassDevFlags.Present | ClassDevFlags.DeviceInterface) where T : DeviceInfo, new()
        {
            return InternalEnumerateDevices<T>(ClassId, flags);
        }

        /// <summary>
        /// Enumerate all devices on the system.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DeviceInfo[] EnumAllDevices()
        {
            return InternalGetComputer();
        }

        /// <summary>
        /// Enumerate COM Ports
        /// </summary>
        /// <returns></returns>
        public static DeviceInfo[] EnumComPorts()
        {
            var p = InternalEnumerateDevices<DeviceInfo>(GUID_DEVINTERFACE_COMPORT, ClassDevFlags.DeviceInterface | ClassDevFlags.Present);
            if (p is object && p.Count() > 0)
            {
                foreach (var x in p)
                {
                    if (string.IsNullOrEmpty(x.FriendlyName))
                        continue;
                }
            }

            if (p is null)
                return null;
            Array.Sort(p, new Comparison<DeviceInfo>((x, y) => { if (x.FriendlyName is object && y.FriendlyName is object) { return string.Compare(x.FriendlyName, y.FriendlyName); } else { return string.Compare(x.Description, y.Description); } }));
            return p;
        }
    }
}