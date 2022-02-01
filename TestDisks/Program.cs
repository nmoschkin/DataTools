


using DataTools.Win32.Disk.Partition.Mbr;
using DataTools.Win32.Disk.Partition.Gpt;
using DataTools.Win32;
using DataTools.Win32.Disk;

using static DataTools.Text.TextTools;

namespace TestNetwork
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {


            var disks = DataTools.Win32.Disk.DiskDeviceInfo.EnumDisks();
            
            foreach (var disk in disks)
            {
                Console.WriteLine("Physical Disk: " + disk.BusReportedDeviceDesc);
                Console.WriteLine("Partition Style: " + disk.DiskLayout?.ToString() ?? "None");

                if (disk.DiskLayout != null)
                {
                    if (disk.DiskLayout.PartitionStyle == DataTools.Win32.Disk.Partition.PartitionStyle.Gpt)
                    {
                        RawGptDisk.RAW_GPT_DISK? gptInfo = default;
                        RawGptDisk.ReadRawGptDisk(disk.DevicePath, out gptInfo);

                        if (gptInfo?.Header.IsValid ?? false)
                        {
                            Console.WriteLine("Successfully read raw GPT disk.  Total partitions: " + gptInfo?.Partitions.Length.ToString());
                        }
                    }
                    else if (disk.DiskLayout.PartitionStyle == DataTools.Win32.Disk.Partition.PartitionStyle.Mbr)
                    {
                        try
                        {
                            RawMbrDisk.RAW_MBR_INFO? mbrInfo = default;
                            RawMbrDisk.ReadRawMbrDisk(disk.DevicePath, disk.SectorSize, out mbrInfo);

                            Console.WriteLine("Successfully read raw MBR disk.  Total partitions: " + mbrInfo?.PartitionCount.ToString());

                        }
                        catch { }
                    }
                }
            }
        }
    }
}