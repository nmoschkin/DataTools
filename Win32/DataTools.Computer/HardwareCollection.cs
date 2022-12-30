﻿// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: HardwareCollection
//         Computer information collection class.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Win32;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DataTools.Computer
{
    /// <summary>
    /// Encapsulates a hierarchical representation of all visible devices on the system.
    /// </summary>
    /// <remarks></remarks>
    public class HardwareCollection : ObservableCollection<object>
    {
        private DeviceClassEnum deviceClass;
        private System.Drawing.Icon icon;

        /// <summary>
        /// Returns the class device enumeration for this device collection.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DeviceClassEnum DeviceClass
        {
            get
            {
                return deviceClass;
            }

            private set
            {
                deviceClass = value;
            }
        }

        /// <summary>
        /// Returns the class description associated with this device collection.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Description
        {
            get
            {
                return DevEnumHelpers.GetEnumDescription(DeviceClass);
            }
        }

        /// <summary>
        /// Returns the class icon associated with this device collection.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public System.Drawing.Icon ClassIcon
        {
            get
            {
                return icon;
            }

            private set
            {
                icon = value;
            }
        }

        /// <summary>
        /// For the heirarchical data arrangement, returns the members of this collection instance.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ObservableCollection<object> LinkedChildren
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Enumerates all computer devices on the system and creates a coherent hierarchy divided into device class sections.
        /// </summary>
        /// <returns>A hierarchical enumeration of all visible devices on the system.</returns>
        /// <remarks></remarks>
        public static HardwareCollection CreateComputerHierarchy()
        {
            var c = new List<DeviceInfo>();
            var e = new List<DeviceInfo>();
            var f = new HardwareCollection();
            HardwareCollection g = null;
            var chw = DeviceClassEnum.Undefined;

            // do the initial enumeration.
            var d = Info.EnumComputerExhaustive();

            // Filter out all non-top-level devices to create the top level.
            foreach (var x in d)
            {
                if (((DeviceInfo)x).LinkedParent is null)
                {
                    c.Add((DeviceInfo)x);
                }
            }

            // sort all top-level devices by their device class.
            c.Sort((a, b) =>
            {
                if (a.DeviceClass.ToString() == b.DeviceClass.ToString())
                {
                    if (a.FriendlyName == b.FriendlyName)
                    {
                        return string.Compare(a.InstanceId, b.InstanceId);
                    }
                    else
                    {
                        return string.Compare(a.FriendlyName, b.FriendlyName);
                    }
                }
                else
                {
                    return string.Compare(a.DeviceClass.ToString(), b.DeviceClass.ToString());
                }
            });

            foreach (var x in c)
            {
                // If we don't already have an object devoted to particular type create it
                if (x.DeviceClass != chw)
                {
                    chw = x.DeviceClass;
                    f.Add(g);

                    g = new HardwareCollection();

                    g.DeviceClass = chw;
                    g.ClassIcon = x.DeviceClassIcon;
                }

                g.Add(x);
            }

            if (g is object && g.Count > 0)
                f.Add(g);
            return f;
        }
    }

    /// <summary>
    /// Compare two DeviceInfo-derived objects by DeviceClass (alphabetically).
    /// </summary>
    /// <remarks></remarks>
    public class HardwareObjectSorter : IComparer<DeviceInfo>
    {
        /// <summary>
        /// Compare two DeviceInfo-derived objects by DeviceClass (alphabetically).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Compare(DeviceInfo a, DeviceInfo b)
        {
            if (a.DeviceClass.ToString() == b.DeviceClass.ToString())
            {
                if (a.FriendlyName == b.FriendlyName)
                {
                    return string.Compare(a.InstanceId, b.InstanceId);
                }
                else
                {
                    return string.Compare(a.FriendlyName, b.FriendlyName);
                }
            }
            else
            {
                return string.Compare(a.DeviceClass.ToString(), b.DeviceClass.ToString());
            }
        }
    }
}