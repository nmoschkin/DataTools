using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTools.Text;
using DataTools.Win32;

namespace DataTools.Win32.Disk.Partition
{

    /// <summary>
    /// Contains the master list of all known partition types as PartitionCodeInfo objects.
    /// </summary>
    /// <remarks></remarks>
    public sealed class PartitionCodeInfo
    {
        private static readonly string[] allOSs;
        private static readonly List<PartitionCodeInfo> partCodes = new List<PartitionCodeInfo>();
        private PartitionAccess partAccess;
        private PartitionBootability bootability;
        private PartitionCharacteristics characteristics;
        private string description;
        private string name;
        private PartitionOccurrence occurrence;
        private string[] origins;
        private string[] os;
        private FriendlyPartitionId partitionId;

        static PartitionCodeInfo()
        {
            var v = InternalPopulationInfo();

            foreach (var l in v)
            {
                partCodes.Add(new PartitionCodeInfo(l));
            }

            partCodes.Sort((x, y) =>
            {
                var los1 = x.SupporedOSes;
                var los2 = y.SupporedOSes;

                if (x.PartitionID.Value == y.PartitionID.Value)
                {
                    if (los1.Length == 1 && los1[0] == "Windows NT" && los2.Length == 1 && los2[0] == "Windows NT")
                    {
                        return string.Compare(x.Description, y.Description);
                    }
                    else if (los1.Length == 1 && los1[0] == "Windows NT")
                    {
                        return -1;
                    }
                    else if (los2.Length == 1 && los2[0] == "Windows NT")
                    {
                        return 1;
                    }
                    else if (los1.Contains("Windows NT") && los2.Contains("Windows NT"))
                    {
                        return string.Compare(x.Description, y.Description);
                    }
                    else if (los1.Contains("Windows NT"))
                    {
                        return -1;
                    }
                    else if (los2.Contains("Windows NT"))
                    {
                        return 1;
                    }
                    else
                    {
                        return string.Compare(x.Description, y.Description);
                    }
                }
                else
                {
                    return x.PartitionID.Value - y.PartitionID.Value;
                }
            });

            var vl = new List<string>();
            foreach (var c in partCodes)
            {
                if (c.SupporedOSes is null) continue;

                foreach (var s in c.SupporedOSes)
                {
                    if (s is null) continue;
                    if (!vl.Contains(s))
                    {
                        vl.Add(s);
                    }
                }
            }

            vl.Sort();
            allOSs = vl.ToArray();
        }

        /// <summary>
        /// Initialize a new instance of the PartitionCodeInfo object.
        /// </summary>
        /// <param name="s">Parsing data with which to initialize the object.</param>
        /// <remarks></remarks>
        private PartitionCodeInfo(string s = null)
        {
            if (s != null) Parse(s);
        }

        /// <summary>
        /// Returns a list of all operating systems listed by the MBR partition types.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string[] AllOSes => allOSs;

        /// <summary>
        /// Returns a list of all known partition types as PartitionInfo objects.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IReadOnlyList<PartitionCodeInfo> GetCodes() => partCodes.ToArray();

        /// <summary>
        /// Specifies the kind of hardware access the partition type supports.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PartitionAccess Access
        {
            get => partAccess;
            private set => partAccess = value;
        }

        /// <summary>
        /// Specifies the partition type's bootability.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PartitionBootability Bootability
        {
            get => bootability;
            private set => bootability = value;
        }

        /// <summary>
        /// Specifies the partition type's characteristics.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PartitionCharacteristics Characteristics
        {
            get => characteristics; 
            private set => characteristics = value;
        }

        /// <summary>
        /// Provides a description of the partition type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Description
        {
            get => description; 
            private set => description = value;
        }

        /// <summary>
        /// Provides the name of the partition type (if different than description).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Name => name;

        /// <summary>
        /// Partition occurrence.  Describes where on the disk a partition entry could be found.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public PartitionOccurrence Occurrence
        {
            get => occurrence; 
            private set => occurrence = value;
        }

        /// <summary>
        /// Specifies the company or companies of origin.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] Origins
        {
            get => origins; 
            private set => origins = value;
        }

        /// <summary>
        /// Partition byte Id.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public FriendlyPartitionId PartitionID
        {
            get => partitionId; 
            private set => partitionId = value;
        }
        /// <summary>
        /// Specifies the supported operating systems.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] SupporedOSes
        {
            get => os; 
            private set => os = value;
        }
        /// <summary>
        /// Finds all partition type information objects that match the specified code.
        /// </summary>
        /// <param name="code">Byte code of the partition type to search.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static PartitionCodeInfo[] FindByCode(byte code)
        {
            var l = new List<PartitionCodeInfo>();
            foreach (var pt in partCodes)
            {
                if (pt.PartitionID.Value == code)
                {
                    l.Add(pt);
                }
            }

            return l.ToArray();
        }

        /// <summary>
        /// Converts this object into its string representation.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            if (Name is null)
            {
                if (os is null || os.Count() == 0)
                {
                    if (Origins is object && Origins.Length > 0)
                    {
                        return Origins[0];
                    }
                    else
                    {
                        return PartitionID.ToString();
                    }
                }
                else
                {
                    return string.Join(", ", os);
                }
            }
            else
            {
                return Name;
            }
        }

        private static PartitionBootability InternalParseBootFlags(string s)
        {
            if (string.IsNullOrEmpty(s)) return PartitionBootability.NoInfo;
            var vs = TextTools.Split(s, ", ");
            var vl = PartitionBootability.NoInfo;
            var sl = Enum.GetNames(vl.GetType());
            foreach (var x in vs)
            {
                if (sl.Contains(x))
                {
                    vl = vl | (PartitionBootability)(int)(Enum.Parse(vl.GetType(), x));
                }
                else if (sl.Contains("M" + x))
                {
                    vl = vl | (PartitionBootability)(int)(Enum.Parse(vl.GetType(), "M" + x));
                }
                else if (sl.Contains("I" + x))
                {
                    vl = vl | (PartitionBootability)(int)(Enum.Parse(vl.GetType(), "I" + x));
                }
                else if (x == "8080/Z80")
                {
                    vl = vl | PartitionBootability.I8080Z80;
                }
            }

            return vl;
        }

        /// <summary>
        /// Returns a string array of comma-seperated partition information entries.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string[] InternalPopulationInfo()
        {

            // This list was adapted from the Wikipedia article: http://en.wikipedia.org/wiki/Partition_type

            var v = new List<string>
            {
#region RAW DATA
                "\"00\",\"MBR, EBR\",\"N/A\",\"No\",\"Free\",\"IBM\",\"All\",\"Empty partition entry\"",
                "\"01\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Filesystem\",\"IBM\",\"DOS 2.0+\",\"FAT12 as primary partition in first physical 32 MB of disk or as logical drive anywhere on disk (else use 06h instead)\"",
                "\"02\",\"MBR\",\"CHS\",\"\",\"\",\"Microsoft, SCO\",\"XENIX\",\"XENIX root\"",
                "\"03\",\"MBR\",\"CHS\",\"\",\"\",\"Microsoft, SCO\",\"XENIX\",\"XENIX usr\"",
                "\"04\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Filesystem\",\"Microsoft\",\"DOS 3.0+\",\"FAT16 with less than 65536 sectors (32 MB). As primary partition it must reside in first physical 32 MB of disk, or as logical drive anywhere on disk (else use 06h instead).\"",
                "\"05\",\"MBR, EBR\",\"CHS, (LBA)\",\"No, AAP\",\"Container\",\"IBM\",\"DOS (3.2) 3.3+\",\"Extended partition with CHS addressing. It must reside in first physical 8 GB of disk, else use 0Fh instead\"",
                "\"06\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Filesystem\",\"Compaq\",\"DOS 3.31+\",\"FAT16B with 65536 or more sectors. It must reside in first physical 8 GB of disk, unless used for logical drives in an 0Fh extended partition (else use 0Eh instead). Also used for FAT12 and FAT16 volumes in primary partitions if they are not residing in first physical 32 MB of disk.\"",
                "\"07\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Filesystem\",\"Microsoft, IBM\",\"OS/2\",\"IFS\"",
                "\"07\",\"MBR, EBR\",\"CHS, LBA\",\"286\",\"Filesystem\",\"IBM\",\"OS/2, Windows NT\",\"HPFS\"",
                "\"07\",\"MBR, EBR\",\"CHS, LBA\",\"386\",\"Filesystem\",\"Microsoft\",\"Windows NT\",\"NTFS\"",
                "\"07\",\"MBR, EBR\",\"CHS, LBA\",\"Yes\",\"Filesystem\",\"Microsoft\",\"Windows Embedded CE\",\"exFAT\"",
                "\"07\",\"\",\"\",\"\",\"\",\"\",\"\",\"Advanced Unix\"",
                "\"07\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 2\",\"QNX 'qnx' (7) (pre-1988 only)\"",
                "\"08\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Commodore\",\"Commodore MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"08\",\"\",\"CHS\",\"x86\",\"Filesystem\",\"IBM\",\"OS/2 1.0-1.3\",\"OS/2\"",
                "\"08\",\"\",\"\",\"\",\"\",\"IBM\",\"AIX\",\"AIX boot/split\"",
                "\"08\",\"\",\"\",\"\",\"\",\"\",\"\",\"SplitDrive\"",
                "\"08\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 1.x/2.x\",\"QNX 'qny' (8)\"",
                "\"08\",\"\",\"\",\"\",\"\",\"Dell\",\"\",\"partition spanning multiple drives\"",
                "\"09\",\"\",\"\",\"\",\"\",\"IBM\",\"AIX\",\"AIX data/boot\"",
                "\"09\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 1.x/2.x\",\"QNX 'qnz' (9)\"",
                "\"09\",\"MBR\",\"CHS\",\"286\",\"Filesystem\",\"Mark Williams Company\",\"Coherent\",\"Coherent file system\"",
                "\"09\",\"MBR\",\"\",\"\",\"Filesystem\",\"Microware\",\"OS-9\",\"OS-9 RBF\"",
                "\"0A\",\"\",\"\",\"\",\"\",\"PowerQuest, IBM\",\"OS/2\",\"OS/2 Boot Manager\"",
                "\"0A\",\"\",\"\",\"\",\"\",\"Mark Williams Company\",\"Coherent\",\"Coherent swap partition\"",
                "\"0A\",\"\",\"\",\"\",\"\",\"Unisys\",\"OPUS\",\"Open Parallel Unisys Server\"",
                "\"0B\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Filesystem\",\"Microsoft\",\"DOS 7.1+\",\"FAT32 with CHS addressing\"",
                "\"0C\",\"MBR, EBR\",\"LBA\",\"x86\",\"Filesystem\",\"Microsoft\",\"DOS 7.1+\",\"FAT32X with LBA\"",
                "\"0E\",\"MBR, EBR\",\"LBA\",\"x86\",\"Filesystem\",\"Microsoft\",\"DOS 7.0+\",\"FAT16X with LBA\"",
                "\"0F\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Container\",\"Microsoft\",\"DOS 7.0+\",\"Extended partition with LBA\"",
                "\"10\",\"\",\"\",\"\",\"\",\"Unisys\",\"OPUS\",\"\"",
                "\"11\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Leading Edge\",\"Leading Edge MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"11\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT12 (corresponds with 01h)\"",
                "\"12\",\"MBR\",\"CHS, LBA\",\"x86\",\"Service, Filesystem\",\"Compaq\",\"\",\"configuration partition (bootable FAT)\"",
                "\"12\",\"\",\"\",\"\",\"Hibernation\",\"Compaq\",\"Compaq Contura\",\"hibernation partition\"",
                "\"12\",\"MBR\",\"\",\"x86\",\"Service, Filesystem\",\"NCR\",\"\",\"diagnostics and firmware partition (bootable FAT)\"",
                "\"12\",\"MBR\",\"\",\"x86\",\"Service, Filesystem\",\"Intel\",\"\",\"service partition (bootable FAT)\"",
                "\"12\",\"\",\"\",\"\",\"Service\",\"IBM\",\"\",\"Rescue and Recovery partition\"",
                "\"14\",\"\",\"\",\"\",\"Filesystem\",\"AST\",\"AST MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"14\",\"\",\"\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT16 (corresponds with 04h)\"",
                "\"14\",\"\",\"LBA\",\"\",\"Filesystem\",\"\",\"Maverick OS\",\"Omega filesystem\"",
                "\"15\",\"\",\"\",\"No, AAP\",\"Hidden, Container\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden extended partition with CHS addressing (corresponds with 05h)\"",
                "\"15\",\"\",\"LBA\",\"\",\"\",\"\",\"Maverick OS\",\"swap\"",
                "\"16\",\"\",\"\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT16B (corresponds with 06h)\"",
                "\"17\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden IFS (corresponds with 07h)\"",
                "\"17\",\"\",\"\",\"\",\"\",\"\",\"\",\"Hidden HPFS (corresponds with 07h)\"",
                "\"17\",\"\",\"\",\"\",\"\",\"\",\"\",\"Hidden NTFS (corresponds with 07h)\"",
                "\"17\",\"\",\"\",\"\",\"\",\"\",\"\",\"Hidden exFAT (corresponds with 07h)\"",
                "\"18\",\"\",\"\",\"No\",\"Hibernation\",\"AST\",\"AST Windows\",\"AST Zero Volt Suspend or SmartSleep partition\"",
                "\"19\",\"\",\"\",\"\",\"\",\"Willow Schlanger\",\"Willowtech Photon coS\",\"Willowtech Photon coS\"",
                "\"1B\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT32 (corresponds with 0Bh)\"",
                "\"1C\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT32X with LBA (corresponds with 0Ch)\"",
                "\"1E\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT16X with LBA (corresponds with 0Eh)\"",
                "\"1F\",\"MBR, EBR\",\"LBA\",\"\",\"Hidden, Container\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden extended partition with LBA addressing (corresponds with 0Fh)\"",
                "\"20\",\"\",\"\",\"\",\"\",\"Microsoft\",\"Windows Mobile\",\"Windows Mobile update XIP\"",
                "\"20\",\"\",\"\",\"\",\"\",\"Willow Schlanger\",\"\",\"Willowsoft Overture File System (OFS1)\"",
                "\"21\",\"MBR\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"21\",\"\",\"\",\"\",\"Filesystem\",\"Dave Poirier\",\"Oxygen\",\"FSo2 (Oxygen File System)\"",
                "\"22\",\"\",\"\",\"\",\"Container\",\"Dave Poirier\",\"Oxygen\",\"Oxygen Extended Partition Table\"",
                "\"23\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"23\",\"\",\"\",\"Yes\",\"\",\"Microsoft\",\"Windows Mobile\",\"Windows Mobile boot XIP\"",
                "\"24\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"NEC\",\"NEC MS-DOS 3.30\",\"Logical sectored FAT12 or FAT16\"",
                "\"25\",\"\",\"\",\"\",\"\",\"Microsoft\",\"Windows Mobile\",\"Windows Mobile IMGFS\"",
                "\"26\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"27\",\"\",\"\",\"\",\"Service, Filesystem\",\"Microsoft\",\"Windows\",\"Windows recovery environment (RE) partition (hidden NTFS partition type 07h)\"",
                "\"27\",\"MBR\",\"CHS, LBA\",\"Yes\",\"Hidden, Service, Filesystem\",\"Acer\",\"PQservice\",\"FAT32 or NTFS rescue partition\"",
                "\"27\",\"\",\"\",\"\",\"\",\"\",\"MirOS BSD\",\"MirOS partition\"",
                "\"27\",\"\",\"\",\"\",\"\",\"\",\"RooterBOOT\",\"RooterBOOT kernel partition (contains a raw ELF Linux kernel, no filesystem)\"",
                "\"2A\",\"\",\"\",\"\",\"Filesystem\",\"Kurt Skauen\",\"AtheOS\",\"AtheOS file system (AthFS, AFS) (an extension of BFS, see 2Bh and EBh)\"",
                "\"2B\",\"\",\"\",\"\",\"\",\"Kristian van der Vliet\",\"SyllableOS\",\"SyllableSecure (SylStor), a variant of AthFS (an extension of BFS, see 2Ah and EBh)\"",
                "\"31\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"32\",\"\",\"\",\"\",\"\",\"Alien Internet Services\",\"NOS\"",
                "\"33\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"34\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"35\",\"MBR, EBR\",\"CHS, LBA\",\"No\",\"Filesystem\",\"IBM\",\"OS/2 Warp Server / eComStation\",\"JFS (OS/2 implementation of AIX Journaling Filesystem)\"",
                "\"36\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"38\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 3.2, 2 GB partition\"",
                "\"39\",\"\",\"\",\"\",\"Container\",\"Bell Labs\",\"Plan 9\",\"Plan 9 edition 3 partition (sub-partitions described in second sector of partition)\"",
                "\"39\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 4 spanned partition\"",
                "\"3A\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 4, 4 GB partition\"",
                "\"3B\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 4 extended partition\"",
                "\"3C\",\"\",\"\",\"\",\"\",\"PowerQuest\",\"PartitionMagic\",\"PqRP (PartitionMagic or DriveImage in progress)\"",
                "\"3D\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"PowerQuest\",\"PartitionMagic\",\"Hidden NetWare\"",
                "\"3F\",\"\",\"\",\"\",\"\",\"\",\"OS/32\"",
                "\"40\",\"\",\"\",\"\",\"\",\"PICK Systems\",\"PICK\",\"PICK R83\"",
                "\"40\",\"\",\"\",\"\",\"\",\"VenturCom\",\"Venix\",\"Venix 80286\"",
                "\"41\",\"\",\"\",\"Yes\",\"\",\"\",\"Personal RISC\",\"Personal RISC Boot\"",
                "\"41\",\"\",\"\",\"\",\"\",\"Linux\",\"Linux\",\"Old Linux/Minix (disk shared with DR DOS 6.0) (corresponds with 81h)\"",
                "\"41\",\"\",\"\",\"PowerPC\",\"\",\"PowerPC\",\"PowerPC\",\"PPC PReP (Power PC Reference Platform) Boot\"",
                "\"42\",\"\",\"\",\"\",\"Secured, Filesystem\",\"Peter Gutmann\",\"SFS\",\"Secure Filesystem (SFS)\"",
                "\"42\",\"\",\"\",\"No\",\"\",\"Linux\",\"Linux\",\"Old Linux swap (disk shared with DR DOS 6.0) (corresponds with 82h)\"",
                "\"42\",\"\",\"\",\"\",\"Container\",\"Microsoft\",\"Windows 2000, XP, etc.\",\"Dynamic extended partition marker\"",
                "\"43\",\"\",\"\",\"Yes\",\"Filesystem\",\"Linux\",\"Linux\",\"Old Linux native (disk shared with DR DOS 6.0) (corresponds with 83h)\"",
                "\"44\",\"\",\"\",\"\",\"\",\"Wildfile\",\"GoBack\",\"Norton GoBack, WildFile GoBack, Adaptec GoBack, Roxio GoBack\"",
                "\"45\",\"\",\"\",\"\",\"\",\"Priam\",\"\",\"Priam\"",
                "\"45\",\"MBR\",\"CHS\",\"Yes\",\"\",\"\",\"Boot-US\",\"Boot-US boot manager (1 cylinder)\"",
                "\"45\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"46\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"47\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"48\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"48\",\"\",\"\",\"\",\"\",\"ERGOS\",\"ERGOS L3\",\"ERGOS L3\"",
                "\"4A\",\"MBR\",\"\",\"Yes\",\"\",\"Nick Roberts\",\"AdaOS\",\"Aquila\"",
                "\"4A\",\"MBR, EBR\",\"CHS, LBA\",\"No\",\"Filesystem\",\"Mark Aitchison\",\"ALFS/THIN\",\"ALFS/THIN advanced lightweight filesystem for DOS\"",
                "\"4C\",\"\",\"\",\"\",\"\",\"ETH Zürich\",\"ETH Oberon\",\"Aos (A2) filesystem (76)\"",
                "\"4D\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 4.x, Neutrino\",\"Primary QNX POSIX volume on disk (77)\"",
                "\"4E\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 4.x, Neutrino\",\"Secondary QNX POSIX volume on disk (78)\"",
                "\"4F\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 4.x, Neutrino\",\"Tertiary QNX POSIX volume on disk (79)\"",
                "\"4F\",\"\",\"\",\"Yes\",\"\",\"ETH Zürich\",\"ETH Oberon\",\"boot / native filesystem (79)\"",
                "\"50\",\"\",\"\",\"\",\"\",\"ETH Zürich\",\"ETH Oberon\",\"Alternative native filesystem (80)\"",
                "\"50\",\"\",\"\",\"No\",\"\",\"OnTrack\",\"Disk Manager 4\",\"Read-only partition (old)\"",
                "\"50\",\"\",\"\",\"\",\"\",\"\",\"LynxOS\",\"Lynx RTOS\"",
                "\"50\",\"\",\"\",\"\",\"\",\"\",\"\",\"Novell\"",
                "\"51\",\"\",\"\",\"\",\"\",\"Novell\",\"\"",
                "\"50\",\"\",\"\",\"No\",\"\",\"OnTrack\",\"Disk Manager 4-6\",\"Read-write partition (Aux 1)\"",
                "\"52\",\"\",\"\",\"\",\"\",\"\",\"CP/M\",\"CP/M\"",
                "\"52\",\"\",\"\",\"\",\"\",\"\",\"Microport\",\"System V/AT, V/386\"",
                "\"53\",\"\",\"\",\"\",\"\",\"OnTrack\",\"Disk Manager 6\",\"Auxiliary 3 (WO)\"",
                "\"54\",\"\",\"\",\"\",\"\",\"OnTrack\",\"Disk Manager 6\",\"Dynamic Drive Overlay (DDO)\"",
                "\"55\",\"\",\"\",\"\",\"\",\"MicroHouse / StorageSoft\",\"EZ-Drive\",\"EZ-Drive, Maxtor, MaxBlast, or DriveGuide INT 13h redirector volume\"",
                "\"56\",\"\",\"\",\"\",\"\",\"AT&T\",\"AT&T MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"56\",\"\",\"\",\"\",\"\",\"MicroHouse / StorageSoft\",\"EZ-Drive\",\"Disk Manager partition converted to EZ-BIOS\"",
                "\"56\",\"\",\"\",\"\",\"\",\"Golden Bow\",\"VFeature\",\"VFeature partitionned volume\"",
                "\"57\",\"\",\"\",\"\",\"\",\"MicroHouse / StorageSoft\",\"DrivePro\"",
                "\"56\",\"\",\"\",\"\",\"\",\"Novell\",\"\",\"VNDI partition\"",
                "\"5C\",\"\",\"\",\"\",\"Container\",\"Priam\",\"EDISK\",\"Priam EDisk Partitioned Volume\"",
                "\"5D\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Policy\",\"\",\"APTI (Alternative Partition Table Identification) conformant systems\",\" APTI alternative partition\"",
                "\"5E\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Policy, Container\",\"\",\"APTI conformant systems\",\" APTI alternative extended partition (corresponds with 0Fh)\"",
                "\"5F\",\"MBR, EBR\",\"CHS\",\"No, AAP\",\"Policy, Container\",\"\",\"APTI conformant systems\",\" APTI alternative extended partition (< 8 GB) (corresponds with 05h)\"",
                "\"61\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"63\",\"\",\"CHS\",\"\",\"Filesystem\",\"\",\"Unix\",\"SCO Unix, ISC, UnixWare, AT&T System V/386, ix, MtXinu BSD 4.3 on Mach, GNU HURD\"",
                "\"64\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"63\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"NetWare\",\"NetWare File System 286/2\"",
                "\"63\",\"\",\"\",\"\",\"\",\"Solomon\",\"\",\"PC-ARMOUR\"",
                "\"65\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"NetWare\",\"NetWare File System 386\"",
                "\"66\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"NetWare\",\"NetWare File System 386\"",
                "\"66\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\",\"Storage Management Services (SMS)\"",
                "\"67\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\",\"Wolf Mountain\"",
                "\"68\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\"",
                "\"69\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare 5\"",
                "\"67\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\",\"Novell Storage Services (NSS)\"",
                "\"70\",\"\",\"\",\"\",\"\",\"\",\"DiskSecure\",\"DiskSecure multiboot\"",
                "\"71\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"72\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT12 (CHS, SFN) (corresponds with 01h)\"",
                "\"72\",\"\",\"\",\"\",\"\",\"Nordier\",\"Unix V7/x86\",\"V7/x86\"",
                "\"73\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"74\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"74\",\"\",\"\",\"\",\"Secured\",\"\",\"\",\"Scramdisk\"",
                "\"75\",\"\",\"\",\"\",\"\",\"IBM\",\"PC/IX\"",
                "\"76\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"77\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"\",\"VNDI, M2FS, M2CS\"",
                "\"78\",\"\",\"\",\"Yes\",\"Filesystem\",\"Geurt Vos\",\"\",\"XOSL bootloader filesystem\"",
                "\"79\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT16 (CHS, SFN) (corresponds with 04h)\"",
                "\"7A\",\"MBR, EBR\",\"LBA\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT16X (LBA, SFN) (corresponds with 0Dh)\"",
                "\"7B\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT16B (CHS, SFN) (corresponds with 06h)\"",
                "\"7C\",\"MBR, EBR\",\"LBA\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT32X (LBA, SFN) (corresponds with 0Ch)\"",
                "\"7D\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT32 (CHS, SFN) (corresponds with 0Bh)\"",
                "\"7E\",\"\",\"\",\"\",\"\",\"\",\"F.I.X.\"",
                "\"7F\",\"MBR, EBR\",\"CHS, LBA\",\"Yes\",\"\",\"AODPS\",\"Varies\",\" Alternative OS Development Partition Standard - reserved for individual or local use and temporary or experimental projects\"",
                "\"80\",\"\",\"\",\"\",\"Filesystem\",\"Andrew Tanenbaum\",\"Minix 1.1-1.4a\",\"Minix file system (old)\"",
                "\"81\",\"\",\"\",\"\",\"Filesystem\",\"Andrew Tanenbaum\",\"Minix 1.4b+\",\"MINIX file system (corresponds with 41h)\"",
                "\"81\",\"\",\"\",\"\",\"\",\"\",\"Linux\",\"Mitac Advanced Disk Manager\"",
                "\"82\",\"\",\"\",\"No\",\"\",\"GNU/Linux\",\"\",\"Linux swap space (corresponds with 42h)\"",
                "\"82\",\"\",\"\",\"x86\",\"Container\",\"Sun Microsystems\",\"\",\"Solaris x86 (for Sun disklabels up to 2005)\"",
                "\"82\",\"\",\"\",\"\",\"\",\"\",\"\",\"Prime\"",
                "\"83\",\"\",\"\",\"\",\"Filesystem\",\"GNU/Linux\",\"\",\"Any native Linux file system\"",
                "\"84\",\"\",\"\",\"No\",\"Hibernation\",\"Microsoft\",\"\",\"APM hibernation (suspend to disk, S2D)\"",
                "\"84\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2\",\"Hidden C: (FAT16)\"",
                "\"84\",\"\",\"\",\"\",\"Hibernation\",\"Intel\",\"Windows 7\",\"Rapid Start technology\"",
                "\"85\",\"\",\"\",\"No, AAP\",\"Container\",\"GNU/Linux\",\"\",\"Linux extended (corresponds with 05h)\"",
                "\"86\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Fault-tolerant FAT16B mirrored volume set\"",
                "\"86\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"Linux\",\"Linux RAID superblock with auto-detect (old)\"",
                "\"87\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Fault-tolerant HPFS/NTFS mirrored volume set\"",
                "\"88\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"\",\"Linux plaintext partition table\"",
                "\"8A\",\"\",\"\",\"\",\"\",\"Martin Kiewitz\",\"AiR-BOOT\",\"Linux kernel image\"",
                "\"8B\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Legacy fault-tolerant FAT32 mirrored volume set\"",
                "\"8C\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Legacy fault-tolerant FAT32X mirrored volume set\"",
                "\"8D\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT12 (corresponds with 01h)\"",
                "\"8E\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"Linux\",\"Linux LVM\"",
                "\"90\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT16 (corresponds with 04h)\"",
                "\"91\",\"MBR, EBR\",\"CHS, LBA\",\"No, AAP\",\"Hidden, Container\",\"FreeDOS\",\"Free FDISK\",\"Hidden extended partition with CHS addressing (corresponds with 05h)\"",
                "\"92\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT16B (corresponds with 06h)\"",
                "\"93\",\"\",\"\",\"\",\"Filesystem\",\"\",\"Amoeba\",\"Amoeba native filesystem\"",
                "\"93\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"\",\"Linux\",\"Hidden Linux filesystem\"",
                "\"94\",\"\",\"\",\"\",\"\",\"\",\"Amoeba\",\"Amoeba bad block table\"",
                "\"95\",\"\",\"\",\"\",\"\",\"MIT\",\"EXOPC\",\"EXOPC native\"",
                "\"96\",\"\",\"\",\"\",\"Filesystem\",\"\",\"CHRP\",\"ISO-9660 filesystem\"",
                "\"97\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT32 (corresponds with 0Bh)\"",
                "\"98\",\"MBR, EBR\",\"LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT32X (corresponds with 0Ch)\"",
                "\"98\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Datalight\",\"ROM-DOS\",\"service partition (bootable FAT) ROM-DOS SuperBoot\"",
                "\"98\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Intel\",\"\",\"service partition (bootable FAT)\"",
                "\"99\",\"\",\"\",\"\",\"Filesystem\",\"\",\"early Unix\"",
                "\"98\",\"\",\"\",\"\",\"Container\",\"Mylex\",\"DCE376\",\"EISA SCSI (> 1024)\"",
                "\"9A\",\"MBR, EBR\",\"LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT16X (corresponds with 0Eh)\"",
                "\"9B\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Hidden, Container\",\"FreeDOS\",\"Free FDISK\",\"Hidden extended partition with LBA (corresponds with 0Fh)\"",
                "\"9E\",\"\",\"\",\"\",\"\",\"Andy Valencia\",\"VSTA\"",
                "\"9B\",\"\",\"\",\"\",\"\",\"Andy Valencia\",\"ForthOS\",\"ForthOS (eForth port)\"",
                "\"9F\",\"\",\"\",\"\",\"\",\"\",\"BSD/OS 3.0+, BSDI\",\"\"",
                "\"A0\",\"MBR\",\"\",\"\",\"Service\",\"Hewlett Packard\",\"\",\"Diagnostic partition for HP laptops\"",
                "\"A0\",\"\",\"\",\"\",\"Hibernation\",\"Phoenix, IBM, Toshiba, Sony\",\"\",\"Hibernate partition\"",
                "\"A1\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"A1\",\"\",\"\",\"\",\"Hibernation\",\"Phoenix, NEC\",\"\",\"Hibernate partition\"",
                "\"A3\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"A4\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"A5\",\"MBR\",\"\",\"\",\"Container\",\"FreeBSD\",\"BSD\",\"BSD slice (BSD/386, 386BSD, NetBSD (old), FreeBSD)\"",
                "\"A6\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"A6\",\"MBR\",\"\",\"\",\"Container\",\"OpenBSD\",\"OpenBSD\",\"OpenBSD slice\"",
                "\"A7\",\"\",\"\",\"386\",\"Filesystem\",\"NeXT\",\"\",\"NeXTSTEP\"",
                "\"A8\",\"\",\"\",\"\",\"Filesystem\",\"Apple\",\"Darwin, Mac OS X\",\"Apple Darwin, Mac OS X UFS\"",
                "\"A9\",\"MBR\",\"\",\"\",\"Container\",\"NetBSD\",\"NetBSD\",\"NetBSD slice\"",
                "\"AA\",\"MBR\",\"CHS\",\"\",\"Service, Image\",\"Olivetti\",\"MS-DOS\",\"Olivetti MS-DOS FAT12 (1.44 MB)\"",
                "\"AB\",\"\",\"\",\"Yes\",\"\",\"Apple\",\"Darwin, Mac OS X\",\"Apple Darwin, Mac OS X boot\"",
                "\"AB\",\"\",\"\",\"\",\"\",\"Stanislav Karchebny\",\"GO! OS\",\"GO!\"",
                "\"AD\",\"\",\"\",\"\",\"Filesystem\",\"Ben Avison, Acorn\",\"RISC OS\",\"ADFS / FileCore format\"",
                "\"AE\",\"\",\"\",\"x86\",\"Filesystem\",\"Frank Barrus\",\"ShagOS\",\"ShagOS file system\"",
                "\"AF\",\"\",\"\",\"\",\"\",\"Apple\",\"\",\"Apple Mac OS X HFS and HFS+\"",
                "\"AF\",\"\",\"\",\"No\",\"\",\"Frank Barrus\",\"ShagOS\",\"ShagOS swap\"",
                "\"B0\",\"MBR\",\"CHS, LBA\",\"x86\",\"Blocker\",\"Star-Tools\",\"Boot-Star\",\"Boot-Star dummy partition\"",
                "\"B1\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"B1\",\"\",\"\",\"\",\"\",\"QNX Software Systems\",\"QNX 6.x\",\"QNX Neutrino power-safe file system\"",
                "\"B2\",\"\",\"\",\"\",\"\",\"QNX Software Systems\",\"QNX 6.x\",\"QNX Neutrino power-safe file system\"",
                "\"B3\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"B3\",\"\",\"\",\"\",\"\",\"QNX Software Systems\",\"QNX 6.x\",\"QNX Neutrino power-safe file system\"",
                "\"B4\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"B6\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"B6\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT16B mirrored master volume\"",
                "\"B7\",\"\",\"\",\"\",\"Filesystem\",\"\",\"BSDI (before 3.0)\",\"BSDI native filesystem / swap\"",
                "\"B7\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant HPFS/NTFS mirrored master volume\"",
                "\"B8\",\"\",\"\",\"\",\"Filesystem\",\"\",\"BSDI (before 3.0)\",\"BSDI swap / native filesystem\"",
                "\"BB\",\"\",\"\",\"\",\"Hidden, (Filesystem)\",\"PhysTechSoft, Acronis, SWsoft\",\"BootWizard, OS Selector\",\"PTS BootWizard 4 / OS Selector 5 for hidden partitions other than 01h, 04h, 06h, 07h, 0Bh, 0Ch, 0Eh and unformatted partitions\"",
                "\"BB\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32 mirrored master volume\"",
                "\"BC\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32X mirrored master volume\"",
                "\"BC\",\"MBR\",\"LBA\",\"\",\"\",\"Acronis\",\"\",\"Backup / Acronis Secure Zone ('ACRONIS SZ')\"",
                "\"BC\",\"MBR, EBR\",\"\",\"\",\"\",\"Paragon Software Group\",\"Backup Capsule\",\"Backup Capsule\"",
                "\"BD\",\"\",\"\",\"\",\"\",\"\",\"BonnyDOS/286\"",
                "\"BE\",\"\",\"\",\"Yes\",\"\",\"Sun Microsystems\",\"Solaris 8\",\"Solaris 8 boot\"",
                "\"BF\",\"\",\"\",\"x86\",\"Container\",\"Sun Microsystems\",\"Solaris\",\"Solaris x86 (for Sun disklabels, since 2005)\"",
                "\"C0\",\"MBR\",\"CHS, LBA\",\"x86\",\"Secured, (Container)\",\"Novell, IMS\",\"DR-DOS, Multiuser DOS, REAL/32\",\"Secured FAT partition (smaller than 32 MB)\"",
                "\"C0\",\"\",\"\",\"\",\"\",\"Novell\",\"\",\"NTFT\"",
                "\"C1\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured FAT12 (corresponds with 01h)\"",
                "\"C2\",\"\",\"\",\"Yes\",\"Hidden, Filesystem\",\"BlueSky Innovations\",\"Power Boot\",\"Hidden Linux native filesystem\"",
                "\"C3\",\"\",\"\",\"No\",\"Hidden\",\"BlueSky Innovations\",\"Power Boot\",\"Hidden Linux swap\"",
                "\"C4\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured FAT16 (corresponds with 04h)\"",
                "\"C5\",\"MBR, EBR\",\"CHS, LBA\",\"No, AAP\",\"Secured, Hidden, Container\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured extended partition with CHS addressing (corresponds with 05h)\"",
                "\"C6\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured FAT16B (corresponds with 06h)\"",
                "\"C6\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT16B mirrored slave volume\"",
                "\"C7\",\"MBR\",\"\",\"Yes\",\"\",\"\",\"Syrinx\",\"Syrinx boot\"",
                "\"C7\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant HPFS/NTFS mirrored slave volume\"",
                "\"C8\",\"\",\"\",\"\",\"\",\"\",\"DR-DOS\",\"Reserved for DR-DOS\"",
                "\"C9\",\"\",\"\",\"\",\"\",\"\",\"DR-DOS\",\"Reserved for DR-DOS\"",
                "\"CA\",\"\",\"\",\"\",\"\",\"\",\"DR-DOS\",\"Reserved for DR-DOS\"",
                "\"CB\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Caldera\",\"DR-DOS 7.\",\"Secured FAT32 (corresponds with 0Bh)\"",
                "\"CB\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32 mirrored slave volume\"",
                "\"CC\",\"MBR, EBR\",\"LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Caldera\",\"DR-DOS 7.\",\"Secured FAT32X (corresponds with 0Ch)\"",
                "\"CC\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32X mirrored slave volume\"",
                "\"CD\",\"\",\"\",\"No\",\"\",\"Convergent Technologies, Unisys\",\"CTOS\",\"Memory dump\"",
                "\"CE\",\"MBR, EBR\",\"LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Caldera\",\"DR-DOS 7.\",\"Secured FAT16X (corresponds with 0Eh)\"",
                "\"CF\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Secured, Hidden, Container\",\"Caldera\",\"DR-DOS 7.\",\"Secured extended partition with LBA (corresponds with 0Fh)\"",
                "\"D0\",\"MBR\",\"CHS, LBA\",\"386\",\"Secured, (Container)\",\"Novell, IMS\",\"Multiuser DOS, REAL/32\",\"Secured FAT partition (larger than 32 MB)\"",
                "\"D1\",\"MBR, EBR\",\"CHS\",\"386\",\"Secured, Hidden, Filesystem\",\"Novell\",\"Multiuser DOS\",\"Secured FAT12 (corresponds with 01h)\"",
                "\"D4\",\"MBR, EBR\",\"CHS\",\"386\",\"Secured, Hidden, Filesystem\",\"Novell\",\"Multiuser DOS\",\"Secured FAT16 (corresponds with 04h)\"",
                "\"D5\",\"MBR, EBR\",\"CHS\",\"No\",\"Secured, Hidden, Container\",\"Novell\",\"Multiuser DOS\",\"Secured extended partition with CHS addressing (corresponds with 05h)\"",
                "\"D6\",\"MBR, EBR\",\"CHS\",\"386\",\"Secured, Hidden, Filesystem\",\"Novell\",\"Multiuser DOS\",\"Secured FAT16B (corresponds with 06h)\"",
                "\"D8\",\"MBR\",\"CHS\",\"\",\"Filesystem\",\"Digital Research\",\"\",\"CP/M-86\"",
                "\"DA\",\"\",\"\",\"No\",\"\",\"John Hardin\",\"\",\"Non-filesystem data\"",
                "\"DA\",\"\",\"\",\"\",\"\",\"DataPower\",\"Powercopy Backup\",\"Shielded disk\"",
                "\"DB\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Digital Research\",\"CP/M-86, Concurrent CP/M-86, Concurrent DOS\",\"CP/M-86, Concurrent CP/M-86, Concurrent DOS\"",
                "\"DB\",\"\",\"\",\"\",\"\",\"Convergent Technologies, Unisys\",\"CTOS\",\"\"",
                "\"DB\",\"\",\"\",\"x86\",\"\",\"KDG Telemetry\",\"D800\",\"boot image for x86 supervisor CPU (SCPU) module\"",
                "\"DB\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Dell\",\"DRMK\",\"FAT32 system restore partition (DSR)\"",
                "\"DD\",\"\",\"\",\"No\",\"\",\"Convergent Technologies, Unisys\",\"CTOS\",\"Hidden memory dump\"",
                "\"DE\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Dell\",\"\",\"FAT16 utility/diagnostic partition\"",
                "\"DF\",\"\",\"\",\"\",\"\",\"Data General\",\"DG/UX\",\"DG/UX virtual disk manager\"",
                "\"DF\",\"MBR\",\"\",\"\",\"Blocker\",\"TeraByte Unlimited\",\"BootIt\",\"EMBRM\"",
                "\"DF\",\"\",\"\",\"\",\"\",\"\",\"\",\"Aviion\"",
                "\"E0\",\"\",\"\",\"\",\"Filesystem\",\"STMicroelectronics\",\"\",\"ST AVFS\"",
                "\"E1\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"Extended FAT12 (> 1023 cylinder)\"",
                "\"E2\",\"\",\"\",\"\",\"Filesystem\",\"\",\"\",\"DOS read-only (XFDISK)\"",
                "\"E3\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"DOS read-only\"",
                "\"E4\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"Extended FAT16 (< 1024 cylinder)\"",
                "\"E5\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Tandy\",\"Tandy MS-DOS\",\"Logical sectored FAT12 or FAT16\"",
                "\"E6\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"E8\",\"\",\"\",\"\",\"\",\"Linux\",\"LUKS\",\"Linux Unified Key Setup\"",
                "\"EB\",\"\",\"\",\"386\",\"Filesystem\",\"Be Inc.\",\"BeOS, Haiku\",\"BFS\"",
                "\"EC\",\"\",\"\",\"\",\"Filesystem\",\"Robert Szeleney\",\"SkyOS\",\"SkyFS\"",
                "\"ED\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"\",\"Matthias Paul\",\"Sprytix\",\"EDC loader\"",
                "\"ED\",\"VirtualMBR\",\"CHS, LBA\",\"x86\",\"\",\"Robert Elliott, Hewlett Packard\",\"EDD 4\",\"GPT hybrid MBR\"",
                "\"EE\",\"MBR\",\"\",\"x86\",\"Blocker, Policy, Container\",\"Microsoft\",\"EFI\",\"GPT protective MBR\"",
                "\"EF\",\"MBR\",\"\",\"\",\"\",\"Intel\",\"EFI\",\"EFI system partition can be a FAT12, FAT16, FAT32 (or other) file system\"",
                "\"F0\",\"\",\"CHS\",\"\",\"\",\"\",\"Linux\",\"PA-RISC Linux boot loader. It must reside in first physical 2 GB.\"",
                "\"F0\",\"\",\"\",\"\",\"\",\"\",\"OS/32\",\"floppy\"",
                "\"F1\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"F2\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Sperry IT, Unisys, Digital Research\",\"Sperry IT MS-DOS 3.x, Unisys MS-DOS 3.3, Digital Research DOS Plus 2.1\",\"Logical sectored FAT12 or FAT16 secondary partition\"",
                "\"F3\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"F4\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"'large' DOS partition\"",
                "\"F4\",\"\",\"\",\"\",\"Filesystem\",\"\",\"Prologue\",\"single volume partition for NGF or TwinFS\"",
                "\"F5\",\"\",\"\",\"\",\"Container\",\"\",\"Prologue\",\"MD0-MD9 multi volume partition for NGF or TwinFS\"",
                "\"F6\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"F7\",\"\",\"\",\"\",\"Filesystem\",\"Natalia Portillo\",\"O.S.G.\",\"EFAT\"",
                "\"F7\",\"\",\"\",\"\",\"Filesystem\",\"DDRdrive\",\"X1\",\"Solid State file system\"",
                "\"F9\",\"\",\"\",\"\",\"\",\"ALC Press\",\"Linux\",\"pCache ext2/ext3 persistent cache\"",
                "\"FA\",\"\",\"\",\"\",\"\",\"MandrakeSoft\",\"Bochs\",\"x86 emulator\"",
                "\"FB\",\"\",\"\",\"\",\"Filesystem\",\"VMware\",\"VMware\",\"VMware VMFS filesystem partition\"",
                "\"FC\",\"\",\"\",\"No\",\"\",\"VMware\",\"VMware\",\"VMware swap / VMKCORE kernel dump partition\"",
                "\"FD\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"Linux\",\"Linux RAID superblock with auto-detect\"",
                "\"FD\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"\",\"FreeDOS\",\"FreeDOS\",\"Reserved for FreeDOS\"",
                "\"FE\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\",\"partition > 1024 cylinder\"",
                "\"FE\",\"\",\"\",\"\",\"\",\"\",\"Intel\",\"LANstep\"",
                "\"FE\",\"\",\"\",\"\",\"Hidden, Service\",\"IBM\",\"\",\"PS/2 IML partition\"",
                "\"FE\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"IBM\",\"\",\"PS/2 recovery partition (FAT12 reference disk floppy image), (corresponds with 01h if activated, all other partitions +10h then)\"",
                "\"FE\",\"\",\"\",\"\",\"Hidden\",\"Microsoft\",\"Windows NT\",\"Disk Administration hidden partition\"",
                "\"FE\",\"\",\"\",\"\",\"\",\"\",\"Linux\",\"old Linux LVM\"",
                "\"FF\",\"MBR\",\"CHS\",\"No\",\"\",\"Microsoft\",\"XENIX\",\"XENIX bad block table\""
#endregion RAWDATA
            };
            return v.ToArray();
        }

        /// <summary>
        /// Parse the data into the new instance from one of the entries in the private partition type cache.
        /// </summary>
        /// <param name="s"></param>
        /// <remarks></remarks>
        private void Parse(string s)
        {
            var vs = TextTools.Split(s, ",", true, true, '"', '"', true, false, true);

            int i;
            int c = vs.Length;

            for (i = 0; i < c; i++)
            {
                vs[i] = vs[i].Trim();
            }

            partitionId = byte.Parse(vs[0], System.Globalization.NumberStyles.HexNumber);

            occurrence = IO.FlagsParse<PartitionOccurrence>(vs[1]);
            partAccess = IO.FlagsParse<PartitionAccess>(vs[2]);

            bootability = InternalParseBootFlags(vs[3]);

            characteristics = IO.FlagsParse<PartitionCharacteristics>(vs[4]);
            origins = TextTools.Split(vs[5], ", ");

            if (vs.Length >= 7)
            {
                os = TextTools.Split(vs[6], ", ");
                if (vs.Length >= 8)
                {
                    description = vs[7];
                    name = vs[7];
                }
            }
        }
    }

}
