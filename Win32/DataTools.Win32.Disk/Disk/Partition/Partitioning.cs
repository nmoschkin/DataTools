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

using DataTools.Text;
using DataTools.Win32.Disk.Partition.Gpt;
using DataTools.Win32.Memory;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace DataTools.Win32.Disk.Partition
{
    [SecurityCritical()]
    internal static class Partitioning
    {
        /// <summary>
        /// Windows system MBR partition information structure.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct PARTITION_INFORMATION_MBR
        {
            public byte PartitionType;

            [MarshalAs(UnmanagedType.Bool)]
            public bool BootIndicator;

            [MarshalAs(UnmanagedType.Bool)]
            public bool RecognizedPartition;

            public uint HiddenSectors;

            /// <summary>
            /// Returns a list of all partition types that were found for the current partition code.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public PartitionCodeInfo[] PartitionCodes
            {
                get
                {
                    return PartitionCodeInfo.FindByCode(PartitionType);
                }
            }

            /// <summary>
            /// Returns the strongest partition type match for Windows NT from the available partition types for the current partition code.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public PartitionCodeInfo PartitionCode
            {
                get
                {
                    var c = PartitionCodes;
                    if (c is object)
                    {
                        return c[0];
                    }

                    return null;
                }
            }

            public override string ToString()
            {
                var c = PartitionCodes;
                if (c is object)
                {
                    return c[0].Description;
                }

                return c[0].PartitionID.ToString();
            }
        }

        /// <summary>
        /// Windows system GPT partition information structure.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct PARTITION_INFORMATION_GPT
        {
            public Guid PartitionType;
            public Guid PartitionId;
            public GptPartitionAttributes Attributes;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 36)]
            private char[] _Name;
            
            /// <summary>
            /// Returns the name of this partition.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public string Name
            {
                get
                {
                    return new string(_Name).Trim('\0');
                }
            }

            /// <summary>
            /// Returns the partition code information for this structure (if any).
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public GptCodeInfo PartitionCode
            {
                get
                {
                    return GptCodeInfo.FindByCode(PartitionType);
                }
            }

            public override string ToString()
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    return Name;
                }
                else if (PartitionCode != null)
                {
                    return PartitionCode.Name ?? PartitionCode.Guid.ToString("D");
                }
                else
                {
                    return base.ToString();
                }
            }
        }

        /// <summary>
        /// Contains extended partition information for any kind of disk device with a partition table.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct PARTITION_INFORMATION_EX
        {
            public PartitionStyle PartitionStyle;
            public long StartingOffset;
            public long PartitionLength;
            public uint PartitionNumber;

            [MarshalAs(UnmanagedType.Bool)]
            public bool RewritePartition;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 112)]
            private byte[] _PartitionInfo;

            /// <summary>
            /// Returns the Mbr structure or nothing if this partition is not an MBR partition.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public PARTITION_INFORMATION_MBR Mbr
            {
                get
                {
                    if (PartitionStyle == PartitionStyle.Gpt) return default;

                    using (var mm = new SafePtr(_PartitionInfo))
                    {
                        return mm.ToStruct<PARTITION_INFORMATION_MBR>();
                    }                    
                }
            }

            /// <summary>
            /// Returns the Gpt structure or nothing if this partition is not a GPT partition.
            /// </summary>
            /// <value></value>
            /// <returns></returns>
            /// <remarks></remarks>
            public PARTITION_INFORMATION_GPT Gpt
            {
                get
                {
                    if (PartitionStyle == PartitionStyle.Mbr) return default;
                    using (var mm = new SafePtr(_PartitionInfo))
                    {
                        return mm.ToStruct<PARTITION_INFORMATION_GPT>();
                    }                        
                }
            }

            public override string ToString()
            {
                if (StartingOffset == 0L && PartitionLength == 0L) return "Empty or Invalid Structure";

                var fs = TextTools.PrintFriendlySize(PartitionLength);

                if (PartitionStyle == PartitionStyle.Mbr)
                {
                    return $"{Mbr} [{fs}]";
                }
                else
                {
                    return $"{Gpt} [{fs}]";
                }

            }
        }

        /// <summary>
        /// Drive layout information for an MBR partition table.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct DRIVE_LAYOUT_INFORMATION_MBR
        {
            public uint Signature;
        }

        /// <summary>
        /// Drive layout information for a GPT partition table.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct DRIVE_LAYOUT_INFORMATION_GPT
        {
            public Guid DiskId;
            public long StartingUsableOffset;
            public long UsableLength;
            public uint MaxPartitionCount;
        }

        /// <summary>
        /// Windows system disk drive partition layout information for any kind of disk.
        /// </summary>
        /// <remarks></remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct DRIVE_LAYOUT_INFORMATION_EX
        {
            public PartitionStyle PartitionStyle;
            public uint ParititionCount;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 40)]
            private byte[] _LayoutInfo;

            public DRIVE_LAYOUT_INFORMATION_MBR Mbr
            {
                get
                {
                    using (var mm = new SafePtr(_LayoutInfo))
                    {
                        return mm.ToStruct<DRIVE_LAYOUT_INFORMATION_MBR>();
                    }
                }
            }

            public DRIVE_LAYOUT_INFORMATION_GPT Gpt
            {
                get
                {
                    using (var mm = new SafePtr(_LayoutInfo))
                    {
                        return mm.ToStruct<DRIVE_LAYOUT_INFORMATION_GPT>();
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates all the partitions on a physical device.
        /// </summary>
        /// <param name="devicePath">The disk device path to query.</param>
        /// <param name="hfile">Optional valid disk handle.</param>
        /// <param name="layInfo">Optionally receives the layout information.</param>
        /// <returns>An array of PARTITION_INFORMATION_EX structures.</returns>
        /// <remarks></remarks>
        public static PARTITION_INFORMATION_EX[] GetPartitions(string devicePath, DiskHandle hfile, out DRIVE_LAYOUT_INFORMATION_EX layInfo)
        {
            bool alreadyOpen = false;

            if (hfile != null)
            {
                alreadyOpen = true;
            }
            else
            {
                hfile = DiskHandle.OpenDisk(devicePath);
            }

            if (hfile?.IsInvalid ?? true)
            {
                layInfo = default;
                return null;
            }

            try
            {
                using (var pex = new SafePtr())
                {
                    DataTools.Memory.MemPtr pexBegin;

                    List<PARTITION_INFORMATION_EX> pOut = null;
                    DRIVE_LAYOUT_INFORMATION_EX lay;

                    int pexLen = Marshal.SizeOf<PARTITION_INFORMATION_EX>();

                    int i, c, sbs = 32768;
                    uint cb = 0U;

                    bool succeed = false;

                    do
                    {
                        pex.ReAlloc(sbs);
                        succeed = NativeDisk.DeviceIoControl(hfile, NativeDisk.IOCTL_DISK_GET_DRIVE_LAYOUT_EX, IntPtr.Zero, 0U, pex, (uint)pex.Length, ref cb, IntPtr.Zero);
                        if (!succeed)
                        {
                            int xErr = User32.GetLastError();
                    
                            if (xErr != NativeDisk.ERROR_MORE_DATA & xErr != NativeDisk.ERROR_INSUFFICIENT_BUFFER)
                            {
                                string s = NativeError.Message;
                                sbs = -1;
                                break;
                            }
                        }

                        sbs *= 2;
                    }
                    while (!succeed);

                    if (sbs == -1)
                    {
                        if (!alreadyOpen) hfile.Close();
                        layInfo = default;
                        return null;
                    }

                    lay = pex.ToStruct<DRIVE_LAYOUT_INFORMATION_EX>();
                    pexBegin = (IntPtr)((long)(pex.DangerousGetHandle()) + 48);

                    c = (int)lay.ParititionCount;
                    pOut = new List<PARTITION_INFORMATION_EX>();

                    for (i = 0; i < c; i++)
                    {
                        var testPart = pexBegin.ToStruct<PARTITION_INFORMATION_EX>();
                        if (lay.PartitionStyle == PartitionStyle.Mbr)
                        {
                            if (testPart.Mbr.PartitionType != 0 && testPart.Mbr.PartitionType != 0x5 && testPart.Mbr.PartitionType != 0x0f)
                            {
                                pOut.Add(testPart);
                            }
                        }
                        else
                        {
                            pOut.Add(testPart);
                        }

                        pexBegin += pexLen;
                    }

                    if (!alreadyOpen) hfile.Close();
                    lay.ParititionCount = (uint)pOut.Count;

                    layInfo = lay;
                    return pOut.ToArray();
                }

            }
            catch
            {
                layInfo = default;
                return null;
            }
            finally
            {
                if (!alreadyOpen) hfile.Close();
            }
        }
    }
}