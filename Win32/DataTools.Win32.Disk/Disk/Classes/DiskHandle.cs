// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: DiskApi
//         Native Disk Serivces.
// 
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************


using System;
using System.Runtime.InteropServices;
using DataTools.Win32.Disk;

namespace DataTools.Win32
{
    /// <summary>
    /// Represents the open file handle for a physical disk device
    /// </summary>
    internal class DiskHandle : SafeHandle
    {
        /// <summary>
        /// Gets the device path that was used to open the disk device.
        /// </summary>
        public string DevicePath { get; private set; }

        /// <inheritdoc/>
        public override string ToString() => DevicePath;

        /// <summary>
        /// Open the disk at with the specified device path. The device must be a disk.
        /// </summary>
        /// <param name="devicePath">The device to open.</param>
        /// <param name="throwOnError">True to throw an <see cref="NativeException"/>, otherwise, return null.</param>
        /// <returns>A <see cref="DiskHandle"/> object that represents the open disk.</returns>
        /// 
        /// <exception cref="NativeException"></exception>
        protected internal static DiskHandle OpenDisk(string devicePath, bool throwOnError = true)
        {
            var file = IO.CreateFile(devicePath, IO.GENERIC_READ, IO.FILE_SHARE_READ | IO.FILE_SHARE_WRITE, IntPtr.Zero, IO.OPEN_EXISTING, IO.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (file == IO.INVALID_HANDLE_VALUE)
            {
                if (throwOnError) throw new NativeException();
                return null;
            }
            
            return new DiskHandle(file)
            {
                DevicePath = devicePath
            };
        }

        /// <summary>
        /// Opens the specified <see cref="DiskDeviceInfo"/> object.
        /// </summary>
        /// <param name="disk">The disk to open.</param>
        /// <returns>A <see cref="DiskHandle"/> object that represents the open disk.</returns>
        /// <exception cref="NativeException"></exception>
        public static DiskHandle OpenDisk(DiskDeviceInfo disk) => OpenDisk(disk.DevicePath); 

        private DiskHandle(IntPtr handle) : base(IO.INVALID_HANDLE_VALUE, true)
        {
            this.handle = handle;
        }

        /// <inheritdoc/>
        public override bool IsInvalid => handle == IO.INVALID_HANDLE_VALUE;

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            if (IsInvalid) return true;

            var b = User32.CloseHandle(handle);
            if (b) handle = IO.INVALID_HANDLE_VALUE;

            return b;
        }

        /// <summary>
        /// Implicit conversion to <see cref="IntPtr"/>
        /// </summary>
        /// <param name="handle"></param>
        public static implicit operator IntPtr(DiskHandle handle)
        {
            return handle.handle;
        }

        /// <summary>
        /// Implicit conversion to void*
        /// </summary>
        /// <param name="handle"></param>
        public static unsafe implicit operator void*(DiskHandle handle)
        {
            return (void*)handle.handle;
        }
    }
}
