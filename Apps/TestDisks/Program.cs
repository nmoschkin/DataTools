using System;
using System.Diagnostics;
using System.Linq;
using DataTools.Text;
using DataTools.Win32.Disk.Partition;
using DataTools.Win32.Disk.Partition.Gpt;
using DataTools.Win32.Disk.Partition.Mbr;

namespace TestNetwork
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var disks = DataTools.Win32.Disk.DiskDeviceInfo.EnumDisks();
            var bd = Guid.Parse("EBD0A0A2-B9E5-4433-87C0-68B6B72699C7");

            foreach (var disk in disks)
            {
                Console.WriteLine();
                Console.WriteLine("Disk: " + disk.ToString());
                Console.WriteLine("Is Virtual: " + (disk.VirtualDisk != null).ToString());

                if (disk.VirtualDisk?.BackingStore != null)
                {
                    Console.WriteLine("Virtual Disk Files");
                    Console.WriteLine("------------------");
                    Console.WriteLine(string.Join("\r\n", disk.VirtualDisk.BackingStore));
                    Console.WriteLine("------------------");
                }
                Console.WriteLine("Physical Disk: " + disk.BusReportedDeviceDesc);
                Console.WriteLine("Partition Style: " + disk.DiskLayout?.ToString() ?? "None");

                if (disk.DiskLayout != null)
                {
                    if (disk.DiskLayout.PartitionStyle == PartitionStyle.Gpt)
                    {
                        RawGptDisk.RAW_GPT_DISK? gptInfo = default;
                        RawGptDisk.ReadRawGptDisk(disk.DevicePath, out gptInfo);

                        if (gptInfo?.Header.IsValid ?? false)
                        {
                            Console.WriteLine("Successfully read raw GPT disk.  Total logical partitions: " + gptInfo?.Partitions.Length.ToString());

                            var c = 1;
                            foreach (var pt in gptInfo?.Partitions)
                            {
                                var pc = pt.PartitionCode;
                                var pn = gptInfo.Value.PartitionTypes[c - 1];
                                if (pc != null)
                                {
                                    Console.Write($"Partition {c++}: {new FriendlySizeLong(pt.Size)} {pc.Name}");
                                    
                                    if (pc.Guid == bd || pn != "Unknown")
                                    {
                                        if (pn == "MSDOS5.0") pn = "FAT32";
                                        Console.WriteLine($" ({pn})");
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                    }

                                }                                
                            }
                        }

                        
                    }
                    else if (disk.DiskLayout.PartitionStyle == PartitionStyle.Mbr)
                    {
                        try
                        {
                            RawMbrDisk.RAW_MBR_INFO? mbrInfo = default;
                            RawMbrDisk.ReadRawMbrDisk(disk.DevicePath, disk.SectorSize, out mbrInfo);

                            Console.WriteLine("Successfully read raw MBR disk.  Total logical partitions: " + mbrInfo?.PartitionCount.ToString());
                            if (mbrInfo != null)
                            {

                                var c = 1;

                                foreach (var pt in mbrInfo?.Mbr.PartTable)
                                {
                                    var pc = pt.PartType;
                                    if (pc == 0) continue;
                                    Console.WriteLine($"Partition {c++}: {new FriendlySizeLong(pt.Size)} {PartitionCodeInfo.FindByCode(pt.PartType).First().Name}");
                                }

                                foreach (var pt in mbrInfo?.Extended)
                                {
                                    var pc = pt.PartType;
                                    if (pc == 0) continue;
                                    Console.WriteLine($"Partition {c++}: {PartitionCodeInfo.FindByCode(pt.PartType).First().Name}");
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                    }
                }
            }
        }
    }
}