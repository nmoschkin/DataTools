using DataTools.Desktop;
using DataTools.Hardware;
using DataTools.Hardware.Printers;
using DataTools.Hardware.Processor;
using DataTools.Hardware.Usb;
using DataTools.Win32;
using DataTools.Win32.Disk;
using DataTools.Win32.Display;
using DataTools.Win32.Network;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static DataTools.Win32.DeviceEnum;

namespace DataTools.Computer
{
    public static class Info
    {

        /// <summary>
        /// Returns an exhaustive hardware tree of the entire computer with as much information as can be obtained. Each object descends from DeviceInfo.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ObservableCollection<object> _internalEnumComputerExhaustive()
        {
            var col = new ObservableCollection<object>();
            var comp = _internalGetComputer();
            var vol = DiskDeviceInfo.EnumVolumes();
            var dsk = DiskDeviceInfo.EnumDisks();
            var net = DeviceEnum.EnumerateDevices<DeviceInfo>(DevProp.GUID_DEVINTERFACE_NET);
            var hid = DeviceEnum.EnumerateDevices<HidDeviceInfo>(DevProp.GUID_DEVINTERFACE_HID);
            var prt = PrinterDeviceInfo.EnumPrinters();
            var bth = BluetoothDeviceInfo.EnumBluetoothDevices();
            var procs = ProcessorDeviceInfo.EnumProcessors();
            var mons = MonitorDeviceInfo.EnumMonitors();
            var adpt = new AdaptersCollection();

            // Dim part As DeviceInfo() = EnumerateDevices(Of DeviceInfo)(GUID_DEVINTERFACE_PARTITION)

            // this collection will never be empty.
            int i;
            int c = comp.Length;

            // We are going to match up the raw device enumeration with the detailed device interface enumerations
            // for the specific kinds of hardware that we know about (so far).  this is a work in progress
            // and I will be doing more classes going forward.

            try
            {
                for (i = 0; i < c; i++)
                {
                    if (comp[i].DeviceClass == DeviceClassEnum.DiskDrive || comp[i].DeviceClass == DeviceClassEnum.CdRom)
                    {
                        try
                        {
                            foreach (var d in dsk)
                            {
                                try
                                {
                                    if ((d.InstanceId.ToUpper().Trim() ?? "") == (comp[i].InstanceId.ToUpper().Trim() ?? "") || (d.PDOName.ToUpper().Trim() ?? "") == (comp[i].PDOName.ToUpper().Trim() ?? ""))
                                    {
                                        d.DeviceClassIcon = comp[i].DeviceClassIcon;
                                        comp[i] = d;
                                        break;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    else if (comp[i].DeviceClass == DeviceClassEnum.Net)
                    {
                        foreach (var nt in net)
                        {
                            if ((nt.InstanceId.ToUpper().Trim() ?? "") == (comp[i].InstanceId.ToUpper().Trim() ?? ""))
                            {
                                nt.DeviceClassIcon = comp[i].DeviceClassIcon;
                                foreach (var ad in adpt)
                                {
                                    if (ad.DeviceInfo is null)
                                        continue;

                                    if ((ad.DeviceInfo.InstanceId ?? "") == (nt.InstanceId ?? "") && (ad.DeviceIcon != null))
                                    {
                                        var entry = new IconImageEntry(ad.DeviceIcon, IconImage.StandardIconFromSize(ad.DeviceIcon.Width));
                                        nt.DeviceIcon = entry.ToIcon();
                                    }
                                }

                                comp[i] = nt;
                                break;
                            }
                        }
                    }
                    else if (comp[i].DeviceClass == DeviceClassEnum.Volume)
                    {
                        foreach (var vl in vol)
                        {
                            if ((vl.InstanceId.ToUpper().Trim() ?? "") == (comp[i].InstanceId.ToUpper().Trim() ?? "") || (vl.PDOName.ToUpper().Trim() ?? "") == (comp[i].PDOName.ToUpper().Trim() ?? ""))
                            {
                                if (vl.VolumePaths is object && vl.VolumePaths.Count() > 0)
                                {
                                    int? argiIndex = null;
                                    vl.DeviceIcon = Resources.GetFileIcon(vl.VolumePaths[0], (Resources.SystemIconSizes)(int)(User32.SHIL_EXTRALARGE), iIndex: ref argiIndex);
                                }

                                vl.DeviceClassIcon = comp[i].DeviceClassIcon;
                                comp[i] = vl;
                                break;
                            }
                        }
                    }
                    else if (comp[i].DeviceClass == DeviceClassEnum.HidClass || comp[i].BusType == BusType.HID)
                    {
                        foreach (var hd in hid)
                        {
                            if ((hd.InstanceId.ToUpper().Trim() ?? "") == (comp[i].InstanceId.ToUpper().Trim() ?? ""))
                            {
                                hd.DeviceClassIcon = comp[i].DeviceClassIcon;
                                comp[i] = hd;
                                break;
                            }
                        }
                    }
                    else if (comp[i].DeviceClass == DeviceClassEnum.PrinterQueue)
                    {
                        foreach (var pr in prt)
                        {
                            if ((comp[i].FriendlyName ?? "") == (pr.FriendlyName ?? ""))
                            {
                                comp[i] = pr;
                                break;
                            }
                        }
                    }
                    else if (comp[i].DeviceClass == DeviceClassEnum.Monitor)
                    {
                        foreach (var mon in mons)
                        {
                            if ((comp[i].InstanceId ?? "") == (mon.InstanceId ?? ""))
                            {
                                comp[i] = mon;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (comp[i].InstanceId.Substring(0, 8) == @"BTHENUM\")
                        {
                            comp[i].DeviceClass = DeviceClassEnum.Bluetooth;
                        }

                        if (comp[i].DeviceClass == DeviceClassEnum.Bluetooth)
                        {
                            foreach (var bt in bth)
                            {
                                if ((comp[i].InstanceId ?? "") == (bt.InstanceId ?? ""))
                                {
                                    comp[i] = bt;
                                    break;
                                }
                            }
                        }
                        else if (comp[i].DeviceClass == DeviceClassEnum.Processor)
                        {
                            foreach (var proc in procs)
                            {
                                if (comp[i].InstanceId == proc.InstanceId)
                                {
                                    comp[i] = proc;
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
            }

            // Call the shared LinkDevices function to pair parents and offspring to create a coherent tree of the system.
            DeviceInfo.LinkDevices(ref comp);
            foreach (var dad in comp)
                col.Add(dad);
            return col;
        }


        /// <summary>
        /// Returns an exhaustive hardware tree of the entire computer with as much information as can be obtained. Each object descends from DeviceInfo.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ObservableCollection<object> EnumComputerExhaustive()
        {
            return _internalEnumComputerExhaustive();
        }
    }
}
