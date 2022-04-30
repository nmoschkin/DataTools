using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataTools.MathTools.PolarMath;
using DataTools.Win32;
using DataTools.Win32.Memory;

using static DataTools.Win32.DeviceEnum;
namespace DataTools.Win32.Display
{
    public class MonitorDeviceInfo : DeviceInfo
    {

        MonitorInfo source;

        public MonitorInfo Source
        {
            get => source;
            internal set
            {
                source = value;
            }
        }

        /// <summary>
        /// Returns the monitor index, or the order in which this monitor was reported to the monitor collection.
        /// </summary>
        /// <returns></returns>
        public int Index
        {
            get
            {
                return source?.Index ?? -1;
            }
            internal set
            {
                if (source != null) source.Index = value;
            }
        }

        /// <summary>
        /// Specifies the current monitor's device path.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string MonitorDevicePath
        {
            get
            {
                return source.DevicePath;
            }
        }

        /// <summary>
        /// Gets the total desktop screen area and coordinates for this monitor.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public LinearRect MonitorBounds
        {
            get
            {
                return source.MonitorBounds;
            }
        }

        /// <summary>
        /// Gets the available desktop area and screen coordinates for this monitor.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public LinearRect WorkBounds
        {
            get
            {
                {
                    return source.WorkBounds;
                }
            }
        }

        /// <summary>
        /// True if this monitor is the primary monitor.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsPrimary
        {
            get
            {
                return (bool)source.IsPrimary;
            }
        }

        /// <summary>
        /// Gets the hMonitor handle for this device.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        internal IntPtr hMonitor
        {
            get
            {
                return (IntPtr)source.hMonitor;
            }
        }

        /// <summary>
        /// Refresh the monitor device information.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Refresh()
        {
            return (bool)source.Refresh();
        }

        /// <summary>
        /// Create a new instance of a monitor object from the given hMonitor.
        /// </summary>
        /// <param name="hMonitor"></param>
        /// <remarks></remarks>
        internal MonitorDeviceInfo(MonitorInfo source)
        {
            this.source = source;
        }


        /// <summary>
        /// Enumerate all display monitors.
        /// </summary>
        /// <returns>An array of PrinterDeviceInfo objects.</returns>
        /// <remarks></remarks>
        public static MonitorDeviceInfo[] EnumMonitors()
        {
            var minf = InternalEnumerateDevices<MonitorDeviceInfo>(DevProp.GUID_DEVINTERFACE_MONITOR, ClassDevFlags.DeviceInterface | ClassDevFlags.Present);
            var mon = new Monitors();

            DISPLAY_DEVICE dd;

            dd.cb = (uint)Marshal.SizeOf<DISPLAY_DEVICE>();
            var mm = new MemPtr();

            mm.Alloc(dd.cb);
            mm.UIntAt(0) = dd.cb;

            if (minf is object && minf.Count() > 0)
            {
                foreach (var x in minf)
                {
                    foreach (var y in mon)
                    {
                        if (MultiMon.EnumDisplayDevices(y.DevicePath, 0, mm, MultiMon.EDD_GET_DEVICE_INTERFACE_NAME))
                        {
                            dd = mm.ToStruct<DISPLAY_DEVICE>();
                            DEVMODE dev = new DEVMODE();

                            dev.dmSize = (ushort)Marshal.SizeOf<DEVMODE>();
                            dev.dmDriverExtra = 0;

                            var mm2 = new MemPtr(65535 + dev.dmSize);



                            var b = MultiMon.EnumDisplaySettingsEx(y.DevicePath, 0xffffffff, ref dev, 0);
                            if (!b)
                            {
                                var s = NativeErrorMethods.FormatLastError();
                            }


                            mm2.Free();

                            if (dd.DeviceID.ToUpper() == x.DevicePath.ToUpper())
                            {
                                x.Source = y;
                                break;
                            }
                        }
                    }
                }
            }

            mm.Free();

            if (minf is null)
                return null;

            Array.Sort(minf, new Comparison<MonitorDeviceInfo>((x, y) => { if (x.FriendlyName is object && y.FriendlyName is object) { return string.Compare(x.FriendlyName, y.FriendlyName); } else { return string.Compare(x.Description, y.Description); } }));
            return minf;
        }



        public MonitorDeviceInfo()
        {
        }

    }







}
