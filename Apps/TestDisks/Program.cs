using System;
using System.Collections;
using System.Collections.Generic;
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
                IList<RawDiskPartition> rawParts = null;

                if (disk.DeviceClass != DataTools.Win32.DeviceClassEnum.CdRom)
                {

                    try
                    {
                        rawParts = RawDiskPartition.ReadPartitions(disk);
                    }
                    catch (Exception ex) 
                    {
                        //Console.WriteLine($"Can't read disk '{disk.DevicePath}'");
                        //Console.WriteLine(ex.Message);
                    }
                }
                

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

                if (rawParts != null)
                {
                    var iq = 1;
                    foreach (var part in rawParts)
                    {
                        Console.WriteLine($"Partition {iq++}: {part}");
                    }
                }
                else
                {
                    Console.WriteLine("No disk in drive, skipping...");
                }
            }
        }
    }
}