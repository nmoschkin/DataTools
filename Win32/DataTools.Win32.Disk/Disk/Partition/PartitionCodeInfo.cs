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
                "\"0x00\",\"MBR, EBR\",\"N/A\",\"No\",\"Free\",\"IBM\",\"All\",\"Empty partition entry\"",
                "\"0x01\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Filesystem\",\"IBM\",\"DOS 2.0+\",\"FAT12 as primary partition in first physical 32 MB of disk or as logical drive anywhere on disk (else use 06h instead)\"",
                "\"0x02\",\"MBR\",\"CHS\",\"\",\"\",\"Microsoft, SCO\",\"XENIX\",\"XENIX root\"",
                "\"0x03\",\"MBR\",\"CHS\",\"\",\"\",\"Microsoft, SCO\",\"XENIX\",\"XENIX usr\"",
                "\"0x04\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Filesystem\",\"Microsoft\",\"DOS 3.0+\",\"FAT16 with less than 65536 sectors (32 MB). As primary partition it must reside in first physical 32 MB of disk, or as logical drive anywhere on disk (else use 06h instead).\"",
                "\"0x05\",\"MBR, EBR\",\"CHS, (LBA)\",\"No, AAP\",\"Container\",\"IBM\",\"DOS (3.2) 3.3+\",\"Extended partition with CHS addressing. It must reside in first physical 8 GB of disk, else use 0Fh instead\"",
                "\"0x06\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Filesystem\",\"Compaq\",\"DOS 3.31+\",\"FAT16B with 65536 or more sectors. It must reside in first physical 8 GB of disk, unless used for logical drives in an 0Fh extended partition (else use 0Eh instead). Also used for FAT12 and FAT16 volumes in primary partitions if they are not residing in first physical 32 MB of disk.\"",
                "\"0x07\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Filesystem\",\"Microsoft, IBM\",\"OS/2\",\"IFS\"",
                "\"0x07\",\"MBR, EBR\",\"CHS, LBA\",\"286\",\"Filesystem\",\"IBM\",\"OS/2, Windows NT\",\"HPFS\"",
                "\"0x07\",\"MBR, EBR\",\"CHS, LBA\",\"386\",\"Filesystem\",\"Microsoft\",\"Windows NT\",\"NTFS\"",
                "\"0x07\",\"MBR, EBR\",\"CHS, LBA\",\"Yes\",\"Filesystem\",\"Microsoft\",\"Windows Embedded CE\",\"exFAT\"",
                "\"0x07\",\"\",\"\",\"\",\"\",\"\",\"\",\"Advanced Unix\"",
                "\"0x07\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 2\",\"QNX 'qnx' (7) (pre-1988 only)\"",
                "\"0x08\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Commodore\",\"Commodore MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"0x08\",\"\",\"CHS\",\"x86\",\"Filesystem\",\"IBM\",\"OS/2 1.0-1.3\",\"OS/2\"",
                "\"0x08\",\"\",\"\",\"\",\"\",\"IBM\",\"AIX\",\"AIX boot/split\"",
                "\"0x08\",\"\",\"\",\"\",\"\",\"\",\"\",\"SplitDrive\"",
                "\"0x08\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 1.x/2.x\",\"QNX 'qny' (8)\"",
                "\"0x08\",\"\",\"\",\"\",\"\",\"Dell\",\"\",\"partition spanning multiple drives\"",
                "\"0x09\",\"\",\"\",\"\",\"\",\"IBM\",\"AIX\",\"AIX data/boot\"",
                "\"0x09\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 1.x/2.x\",\"QNX 'qnz' (9)\"",
                "\"0x09\",\"MBR\",\"CHS\",\"286\",\"Filesystem\",\"Mark Williams Company\",\"Coherent\",\"Coherent file system\"",
                "\"0x09\",\"MBR\",\"\",\"\",\"Filesystem\",\"Microware\",\"OS-9\",\"OS-9 RBF\"",
                "\"0x0A\",\"\",\"\",\"\",\"\",\"PowerQuest, IBM\",\"OS/2\",\"OS/2 Boot Manager\"",
                "\"0x0A\",\"\",\"\",\"\",\"\",\"Mark Williams Company\",\"Coherent\",\"Coherent swap partition\"",
                "\"0x0A\",\"\",\"\",\"\",\"\",\"Unisys\",\"OPUS\",\"Open Parallel Unisys Server\"",
                "\"0x0B\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Filesystem\",\"Microsoft\",\"DOS 7.1+\",\"FAT32 with CHS addressing\"",
                "\"0x0C\",\"MBR, EBR\",\"LBA\",\"x86\",\"Filesystem\",\"Microsoft\",\"DOS 7.1+\",\"FAT32X with LBA\"",
                "\"0x0E\",\"MBR, EBR\",\"LBA\",\"x86\",\"Filesystem\",\"Microsoft\",\"DOS 7.0+\",\"FAT16X with LBA\"",
                "\"0x0F\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Container\",\"Microsoft\",\"DOS 7.0+\",\"Extended partition with LBA\"",
                "\"0x10\",\"\",\"\",\"\",\"\",\"Unisys\",\"OPUS\",\"\"",
                "\"0x11\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Leading Edge\",\"Leading Edge MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"0x11\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT12 (corresponds with 01h)\"",
                "\"0x12\",\"MBR\",\"CHS, LBA\",\"x86\",\"Service, Filesystem\",\"Compaq\",\"\",\"configuration partition (bootable FAT)\"",
                "\"0x12\",\"\",\"\",\"\",\"Hibernation\",\"Compaq\",\"Compaq Contura\",\"hibernation partition\"",
                "\"0x12\",\"MBR\",\"\",\"x86\",\"Service, Filesystem\",\"NCR\",\"\",\"diagnostics and firmware partition (bootable FAT)\"",
                "\"0x12\",\"MBR\",\"\",\"x86\",\"Service, Filesystem\",\"Intel\",\"\",\"service partition (bootable FAT)\"",
                "\"0x12\",\"\",\"\",\"\",\"Service\",\"IBM\",\"\",\"Rescue and Recovery partition\"",
                "\"0x14\",\"\",\"\",\"\",\"Filesystem\",\"AST\",\"AST MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"0x14\",\"\",\"\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT16 (corresponds with 04h)\"",
                "\"0x14\",\"\",\"LBA\",\"\",\"Filesystem\",\"\",\"Maverick OS\",\"Omega filesystem\"",
                "\"0x15\",\"\",\"\",\"No, AAP\",\"Hidden, Container\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden extended partition with CHS addressing (corresponds with 05h)\"",
                "\"0x15\",\"\",\"LBA\",\"\",\"\",\"\",\"Maverick OS\",\"swap\"",
                "\"0x16\",\"\",\"\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT16B (corresponds with 06h)\"",
                "\"0x17\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden IFS (corresponds with 07h)\"",
                "\"0x17\",\"\",\"\",\"\",\"\",\"\",\"\",\"Hidden HPFS (corresponds with 07h)\"",
                "\"0x17\",\"\",\"\",\"\",\"\",\"\",\"\",\"Hidden NTFS (corresponds with 07h)\"",
                "\"0x17\",\"\",\"\",\"\",\"\",\"\",\"\",\"Hidden exFAT (corresponds with 07h)\"",
                "\"0x18\",\"\",\"\",\"No\",\"Hibernation\",\"AST\",\"AST Windows\",\"AST Zero Volt Suspend or SmartSleep partition\"",
                "\"0x19\",\"\",\"\",\"\",\"\",\"Willow Schlanger\",\"Willowtech Photon coS\",\"Willowtech Photon coS\"",
                "\"0x1B\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT32 (corresponds with 0Bh)\"",
                "\"0x1C\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT32X with LBA (corresponds with 0Ch)\"",
                "\"0x1E\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden FAT16X with LBA (corresponds with 0Eh)\"",
                "\"0x1F\",\"MBR, EBR\",\"LBA\",\"\",\"Hidden, Container\",\"IBM\",\"OS/2 Boot Manager\",\"Hidden extended partition with LBA addressing (corresponds with 0Fh)\"",
                "\"0x20\",\"\",\"\",\"\",\"\",\"Microsoft\",\"Windows Mobile\",\"Windows Mobile update XIP\"",
                "\"0x20\",\"\",\"\",\"\",\"\",\"Willow Schlanger\",\"\",\"Willowsoft Overture File System (OFS1)\"",
                "\"0x21\",\"MBR\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0x21\",\"\",\"\",\"\",\"Filesystem\",\"Dave Poirier\",\"Oxygen\",\"FSo2 (Oxygen File System)\"",
                "\"0x22\",\"\",\"\",\"\",\"Container\",\"Dave Poirier\",\"Oxygen\",\"Oxygen Extended Partition Table\"",
                "\"0x23\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x23\",\"\",\"\",\"Yes\",\"\",\"Microsoft\",\"Windows Mobile\",\"Windows Mobile boot XIP\"",
                "\"0x24\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"NEC\",\"NEC MS-DOS 3.30\",\"Logical sectored FAT12 or FAT16\"",
                "\"0x25\",\"\",\"\",\"\",\"\",\"Microsoft\",\"Windows Mobile\",\"Windows Mobile IMGFS\"",
                "\"0x26\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x27\",\"\",\"\",\"\",\"Service, Filesystem\",\"Microsoft\",\"Windows\",\"Windows recovery environment (RE) partition (hidden NTFS partition type 07h)\"",
                "\"0x27\",\"MBR\",\"CHS, LBA\",\"Yes\",\"Hidden, Service, Filesystem\",\"Acer\",\"PQservice\",\"FAT32 or NTFS rescue partition\"",
                "\"0x27\",\"\",\"\",\"\",\"\",\"\",\"MirOS BSD\",\"MirOS partition\"",
                "\"0x27\",\"\",\"\",\"\",\"\",\"\",\"RooterBOOT\",\"RooterBOOT kernel partition (contains a raw ELF Linux kernel, no filesystem)\"",
                "\"0x2A\",\"\",\"\",\"\",\"Filesystem\",\"Kurt Skauen\",\"AtheOS\",\"AtheOS file system (AthFS, AFS) (an extension of BFS, see 2Bh and EBh)\"",
                "\"0x2B\",\"\",\"\",\"\",\"\",\"Kristian van der Vliet\",\"SyllableOS\",\"SyllableSecure (SylStor), a variant of AthFS (an extension of BFS, see 2Ah and EBh)\"",
                "\"0x31\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x32\",\"\",\"\",\"\",\"\",\"Alien Internet Services\",\"NOS\"",
                "\"0x33\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x34\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x35\",\"MBR, EBR\",\"CHS, LBA\",\"No\",\"Filesystem\",\"IBM\",\"OS/2 Warp Server / eComStation\",\"JFS (OS/2 implementation of AIX Journaling Filesystem)\"",
                "\"0x36\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x38\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 3.2, 2 GB partition\"",
                "\"0x39\",\"\",\"\",\"\",\"Container\",\"Bell Labs\",\"Plan 9\",\"Plan 9 edition 3 partition (sub-partitions described in second sector of partition)\"",
                "\"0x39\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 4 spanned partition\"",
                "\"0x3A\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 4, 4 GB partition\"",
                "\"0x3B\",\"\",\"\",\"\",\"\",\"Timothy Williams\",\"THEOS\",\"THEOS version 4 extended partition\"",
                "\"0x3C\",\"\",\"\",\"\",\"\",\"PowerQuest\",\"PartitionMagic\",\"PqRP (PartitionMagic or DriveImage in progress)\"",
                "\"0x3D\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"PowerQuest\",\"PartitionMagic\",\"Hidden NetWare\"",
                "\"0x3F\",\"\",\"\",\"\",\"\",\"\",\"OS/32\"",
                "\"0x40\",\"\",\"\",\"\",\"\",\"PICK Systems\",\"PICK\",\"PICK R83\"",
                "\"0x40\",\"\",\"\",\"\",\"\",\"VenturCom\",\"Venix\",\"Venix 80286\"",
                "\"0x41\",\"\",\"\",\"Yes\",\"\",\"\",\"Personal RISC\",\"Personal RISC Boot\"",
                "\"0x41\",\"\",\"\",\"\",\"\",\"Linux\",\"Linux\",\"Old Linux/Minix (disk shared with DR DOS 6.0) (corresponds with 81h)\"",
                "\"0x41\",\"\",\"\",\"PowerPC\",\"\",\"PowerPC\",\"PowerPC\",\"PPC PReP (Power PC Reference Platform) Boot\"",
                "\"0x42\",\"\",\"\",\"\",\"Secured, Filesystem\",\"Peter Gutmann\",\"SFS\",\"Secure Filesystem (SFS)\"",
                "\"0x42\",\"\",\"\",\"No\",\"\",\"Linux\",\"Linux\",\"Old Linux swap (disk shared with DR DOS 6.0) (corresponds with 82h)\"",
                "\"0x42\",\"\",\"\",\"\",\"Container\",\"Microsoft\",\"Windows 2000, XP, etc.\",\"Dynamic extended partition marker\"",
                "\"0x43\",\"\",\"\",\"Yes\",\"Filesystem\",\"Linux\",\"Linux\",\"Old Linux native (disk shared with DR DOS 6.0) (corresponds with 83h)\"",
                "\"0x44\",\"\",\"\",\"\",\"\",\"Wildfile\",\"GoBack\",\"Norton GoBack, WildFile GoBack, Adaptec GoBack, Roxio GoBack\"",
                "\"0x45\",\"\",\"\",\"\",\"\",\"Priam\",\"\",\"Priam\"",
                "\"0x45\",\"MBR\",\"CHS\",\"Yes\",\"\",\"\",\"Boot-US\",\"Boot-US boot manager (1 cylinder)\"",
                "\"0x45\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"0x46\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"0x47\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"0x48\",\"\",\"\",\"\",\"\",\"Jochen Liedtke, GMD\",\"EUMEL/ELAN\",\"EUMEL/ELAN (L2)\"",
                "\"0x48\",\"\",\"\",\"\",\"\",\"ERGOS\",\"ERGOS L3\",\"ERGOS L3\"",
                "\"0x4A\",\"MBR\",\"\",\"Yes\",\"\",\"Nick Roberts\",\"AdaOS\",\"Aquila\"",
                "\"0x4A\",\"MBR, EBR\",\"CHS, LBA\",\"No\",\"Filesystem\",\"Mark Aitchison\",\"ALFS/THIN\",\"ALFS/THIN advanced lightweight filesystem for DOS\"",
                "\"0x4C\",\"\",\"\",\"\",\"\",\"ETH Zürich\",\"ETH Oberon\",\"Aos (A2) filesystem (76)\"",
                "\"0x4D\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 4.x, Neutrino\",\"Primary QNX POSIX volume on disk (77)\"",
                "\"0x4E\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 4.x, Neutrino\",\"Secondary QNX POSIX volume on disk (78)\"",
                "\"0x4F\",\"\",\"\",\"\",\"\",\"Quantum Software Systems\",\"QNX 4.x, Neutrino\",\"Tertiary QNX POSIX volume on disk (79)\"",
                "\"0x4F\",\"\",\"\",\"Yes\",\"\",\"ETH Zürich\",\"ETH Oberon\",\"boot / native filesystem (79)\"",
                "\"0x50\",\"\",\"\",\"\",\"\",\"ETH Zürich\",\"ETH Oberon\",\"Alternative native filesystem (80)\"",
                "\"0x50\",\"\",\"\",\"No\",\"\",\"OnTrack\",\"Disk Manager 4\",\"Read-only partition (old)\"",
                "\"0x50\",\"\",\"\",\"\",\"\",\"\",\"LynxOS\",\"Lynx RTOS\"",
                "\"0x50\",\"\",\"\",\"\",\"\",\"\",\"\",\"Novell\"",
                "\"0x51\",\"\",\"\",\"\",\"\",\"Novell\",\"\"",
                "\"0x50\",\"\",\"\",\"No\",\"\",\"OnTrack\",\"Disk Manager 4-6\",\"Read-write partition (Aux 1)\"",
                "\"0x52\",\"\",\"\",\"\",\"\",\"\",\"CP/M\",\"CP/M\"",
                "\"0x52\",\"\",\"\",\"\",\"\",\"\",\"Microport\",\"System V/AT, V/386\"",
                "\"0x53\",\"\",\"\",\"\",\"\",\"OnTrack\",\"Disk Manager 6\",\"Auxiliary 3 (WO)\"",
                "\"0x54\",\"\",\"\",\"\",\"\",\"OnTrack\",\"Disk Manager 6\",\"Dynamic Drive Overlay (DDO)\"",
                "\"0x55\",\"\",\"\",\"\",\"\",\"MicroHouse / StorageSoft\",\"EZ-Drive\",\"EZ-Drive, Maxtor, MaxBlast, or DriveGuide INT 13h redirector volume\"",
                "\"0x56\",\"\",\"\",\"\",\"\",\"AT&T\",\"AT&T MS-DOS 3.x\",\"Logical sectored FAT12 or FAT16\"",
                "\"0x56\",\"\",\"\",\"\",\"\",\"MicroHouse / StorageSoft\",\"EZ-Drive\",\"Disk Manager partition converted to EZ-BIOS\"",
                "\"0x56\",\"\",\"\",\"\",\"\",\"Golden Bow\",\"VFeature\",\"VFeature partitionned volume\"",
                "\"0x57\",\"\",\"\",\"\",\"\",\"MicroHouse / StorageSoft\",\"DrivePro\"",
                "\"0x56\",\"\",\"\",\"\",\"\",\"Novell\",\"\",\"VNDI partition\"",
                "\"0x5C\",\"\",\"\",\"\",\"Container\",\"Priam\",\"EDISK\",\"Priam EDisk Partitioned Volume\"",
                "\"0x5D\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Policy\",\"\",\"APTI (Alternative Partition Table Identification) conformant systems\",\" APTI alternative partition\"",
                "\"0x5E\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Policy, Container\",\"\",\"APTI conformant systems\",\" APTI alternative extended partition (corresponds with 0Fh)\"",
                "\"0x5F\",\"MBR, EBR\",\"CHS\",\"No, AAP\",\"Policy, Container\",\"\",\"APTI conformant systems\",\" APTI alternative extended partition (< 8 GB) (corresponds with 05h)\"",
                "\"0x61\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"0x63\",\"\",\"CHS\",\"\",\"Filesystem\",\"\",\"Unix\",\"SCO Unix, ISC, UnixWare, AT&T System V/386, ix, MtXinu BSD 4.3 on Mach, GNU HURD\"",
                "\"0x64\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"0x63\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"NetWare\",\"NetWare File System 286/2\"",
                "\"0x63\",\"\",\"\",\"\",\"\",\"Solomon\",\"\",\"PC-ARMOUR\"",
                "\"0x65\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"NetWare\",\"NetWare File System 386\"",
                "\"0x66\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"NetWare\",\"NetWare File System 386\"",
                "\"0x66\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\",\"Storage Management Services (SMS)\"",
                "\"0x67\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\",\"Wolf Mountain\"",
                "\"0x68\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\"",
                "\"0x69\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare 5\"",
                "\"0x67\",\"\",\"\",\"\",\"\",\"Novell\",\"NetWare\",\"Novell Storage Services (NSS)\"",
                "\"0x70\",\"\",\"\",\"\",\"\",\"\",\"DiskSecure\",\"DiskSecure multiboot\"",
                "\"0x71\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x72\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT12 (CHS, SFN) (corresponds with 01h)\"",
                "\"0x72\",\"\",\"\",\"\",\"\",\"Nordier\",\"Unix V7/x86\",\"V7/x86\"",
                "\"0x73\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x74\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x74\",\"\",\"\",\"\",\"Secured\",\"\",\"\",\"Scramdisk\"",
                "\"0x75\",\"\",\"\",\"\",\"\",\"IBM\",\"PC/IX\"",
                "\"0x76\",\"\",\"\",\"\",\"\",\"Microsoft, IBM\",\"\",\"Reserved\"",
                "\"0x77\",\"\",\"\",\"\",\"Filesystem\",\"Novell\",\"\",\"VNDI, M2FS, M2CS\"",
                "\"0x78\",\"\",\"\",\"Yes\",\"Filesystem\",\"Geurt Vos\",\"\",\"XOSL bootloader filesystem\"",
                "\"0x79\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT16 (CHS, SFN) (corresponds with 04h)\"",
                "\"0x7A\",\"MBR, EBR\",\"LBA\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT16X (LBA, SFN) (corresponds with 0Dh)\"",
                "\"0x7B\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT16B (CHS, SFN) (corresponds with 06h)\"",
                "\"0x7C\",\"MBR, EBR\",\"LBA\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT32X (LBA, SFN) (corresponds with 0Ch)\"",
                "\"0x7D\",\"MBR, EBR\",\"CHS\",\"x86\",\"Policy, Filesystem\",\"\",\"APTI conformant systems\",\"APTI alternative FAT32 (CHS, SFN) (corresponds with 0Bh)\"",
                "\"0x7E\",\"\",\"\",\"\",\"\",\"\",\"F.I.X.\"",
                "\"0x7F\",\"MBR, EBR\",\"CHS, LBA\",\"Yes\",\"\",\"AODPS\",\"Varies\",\" Alternative OS Development Partition Standard - reserved for individual or local use and temporary or experimental projects\"",
                "\"0x80\",\"\",\"\",\"\",\"Filesystem\",\"Andrew Tanenbaum\",\"Minix 1.1-1.4a\",\"Minix file system (old)\"",
                "\"0x81\",\"\",\"\",\"\",\"Filesystem\",\"Andrew Tanenbaum\",\"Minix 1.4b+\",\"MINIX file system (corresponds with 41h)\"",
                "\"0x81\",\"\",\"\",\"\",\"\",\"\",\"Linux\",\"Mitac Advanced Disk Manager\"",
                "\"0x82\",\"\",\"\",\"No\",\"\",\"GNU/Linux\",\"\",\"Linux swap space (corresponds with 42h)\"",
                "\"0x82\",\"\",\"\",\"x86\",\"Container\",\"Sun Microsystems\",\"\",\"Solaris x86 (for Sun disklabels up to 2005)\"",
                "\"0x82\",\"\",\"\",\"\",\"\",\"\",\"\",\"Prime\"",
                "\"0x83\",\"\",\"\",\"\",\"Filesystem\",\"GNU/Linux\",\"\",\"Any native Linux file system\"",
                "\"0x84\",\"\",\"\",\"No\",\"Hibernation\",\"Microsoft\",\"\",\"APM hibernation (suspend to disk, S2D)\"",
                "\"0x84\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"IBM\",\"OS/2\",\"Hidden C: (FAT16)\"",
                "\"0x84\",\"\",\"\",\"\",\"Hibernation\",\"Intel\",\"Windows 7\",\"Rapid Start technology\"",
                "\"0x85\",\"\",\"\",\"No, AAP\",\"Container\",\"GNU/Linux\",\"\",\"Linux extended (corresponds with 05h)\"",
                "\"0x86\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Fault-tolerant FAT16B mirrored volume set\"",
                "\"0x86\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"Linux\",\"Linux RAID superblock with auto-detect (old)\"",
                "\"0x87\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Fault-tolerant HPFS/NTFS mirrored volume set\"",
                "\"0x88\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"\",\"Linux plaintext partition table\"",
                "\"0x8A\",\"\",\"\",\"\",\"\",\"Martin Kiewitz\",\"AiR-BOOT\",\"Linux kernel image\"",
                "\"0x8B\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Legacy fault-tolerant FAT32 mirrored volume set\"",
                "\"0x8C\",\"\",\"\",\"\",\"Filesystem\",\"Microsoft\",\"Windows NT 4 Server\",\"Legacy fault-tolerant FAT32X mirrored volume set\"",
                "\"0x8D\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT12 (corresponds with 01h)\"",
                "\"0x8E\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"Linux\",\"Linux LVM\"",
                "\"0x90\",\"MBR, EBR\",\"CHS, LBA\",\"x86, 68000, 8080/Z80\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT16 (corresponds with 04h)\"",
                "\"0x91\",\"MBR, EBR\",\"CHS, LBA\",\"No, AAP\",\"Hidden, Container\",\"FreeDOS\",\"Free FDISK\",\"Hidden extended partition with CHS addressing (corresponds with 05h)\"",
                "\"0x92\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT16B (corresponds with 06h)\"",
                "\"0x93\",\"\",\"\",\"\",\"Filesystem\",\"\",\"Amoeba\",\"Amoeba native filesystem\"",
                "\"0x93\",\"\",\"\",\"\",\"Hidden, Filesystem\",\"\",\"Linux\",\"Hidden Linux filesystem\"",
                "\"0x94\",\"\",\"\",\"\",\"\",\"\",\"Amoeba\",\"Amoeba bad block table\"",
                "\"0x95\",\"\",\"\",\"\",\"\",\"MIT\",\"EXOPC\",\"EXOPC native\"",
                "\"0x96\",\"\",\"\",\"\",\"Filesystem\",\"\",\"CHRP\",\"ISO-9660 filesystem\"",
                "\"0x97\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT32 (corresponds with 0Bh)\"",
                "\"0x98\",\"MBR, EBR\",\"LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT32X (corresponds with 0Ch)\"",
                "\"0x98\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Datalight\",\"ROM-DOS\",\"service partition (bootable FAT) ROM-DOS SuperBoot\"",
                "\"0x98\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Intel\",\"\",\"service partition (bootable FAT)\"",
                "\"0x99\",\"\",\"\",\"\",\"Filesystem\",\"\",\"early Unix\"",
                "\"0x98\",\"\",\"\",\"\",\"Container\",\"Mylex\",\"DCE376\",\"EISA SCSI (> 1024)\"",
                "\"0x9A\",\"MBR, EBR\",\"LBA\",\"x86\",\"Hidden, Filesystem\",\"FreeDOS\",\"Free FDISK\",\"Hidden FAT16X (corresponds with 0Eh)\"",
                "\"0x9B\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Hidden, Container\",\"FreeDOS\",\"Free FDISK\",\"Hidden extended partition with LBA (corresponds with 0Fh)\"",
                "\"0x9E\",\"\",\"\",\"\",\"\",\"Andy Valencia\",\"VSTA\"",
                "\"0x9B\",\"\",\"\",\"\",\"\",\"Andy Valencia\",\"ForthOS\",\"ForthOS (eForth port)\"",
                "\"0x9F\",\"\",\"\",\"\",\"\",\"\",\"BSD/OS 3.0+, BSDI\",\"\"",
                "\"0xA0\",\"MBR\",\"\",\"\",\"Service\",\"Hewlett Packard\",\"\",\"Diagnostic partition for HP laptops\"",
                "\"0xA0\",\"\",\"\",\"\",\"Hibernation\",\"Phoenix, IBM, Toshiba, Sony\",\"\",\"Hibernate partition\"",
                "\"0xA1\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xA1\",\"\",\"\",\"\",\"Hibernation\",\"Phoenix, NEC\",\"\",\"Hibernate partition\"",
                "\"0xA3\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xA4\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xA5\",\"MBR\",\"\",\"\",\"Container\",\"FreeBSD\",\"BSD\",\"BSD slice (BSD/386, 386BSD, NetBSD (old), FreeBSD)\"",
                "\"0xA6\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xA6\",\"MBR\",\"\",\"\",\"Container\",\"OpenBSD\",\"OpenBSD\",\"OpenBSD slice\"",
                "\"0xA7\",\"\",\"\",\"386\",\"Filesystem\",\"NeXT\",\"\",\"NeXTSTEP\"",
                "\"0xA8\",\"\",\"\",\"\",\"Filesystem\",\"Apple\",\"Darwin, Mac OS X\",\"Apple Darwin, Mac OS X UFS\"",
                "\"0xA9\",\"MBR\",\"\",\"\",\"Container\",\"NetBSD\",\"NetBSD\",\"NetBSD slice\"",
                "\"0xAA\",\"MBR\",\"CHS\",\"\",\"Service, Image\",\"Olivetti\",\"MS-DOS\",\"Olivetti MS-DOS FAT12 (1.44 MB)\"",
                "\"0xAB\",\"\",\"\",\"Yes\",\"\",\"Apple\",\"Darwin, Mac OS X\",\"Apple Darwin, Mac OS X boot\"",
                "\"0xAB\",\"\",\"\",\"\",\"\",\"Stanislav Karchebny\",\"GO! OS\",\"GO!\"",
                "\"0xAD\",\"\",\"\",\"\",\"Filesystem\",\"Ben Avison, Acorn\",\"RISC OS\",\"ADFS / FileCore format\"",
                "\"0xAE\",\"\",\"\",\"x86\",\"Filesystem\",\"Frank Barrus\",\"ShagOS\",\"ShagOS file system\"",
                "\"0xAF\",\"\",\"\",\"\",\"\",\"Apple\",\"\",\"Apple Mac OS X HFS and HFS+\"",
                "\"0xAF\",\"\",\"\",\"No\",\"\",\"Frank Barrus\",\"ShagOS\",\"ShagOS swap\"",
                "\"0xB0\",\"MBR\",\"CHS, LBA\",\"x86\",\"Blocker\",\"Star-Tools\",\"Boot-Star\",\"Boot-Star dummy partition\"",
                "\"0xB1\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xB1\",\"\",\"\",\"\",\"\",\"QNX Software Systems\",\"QNX 6.x\",\"QNX Neutrino power-safe file system\"",
                "\"0xB2\",\"\",\"\",\"\",\"\",\"QNX Software Systems\",\"QNX 6.x\",\"QNX Neutrino power-safe file system\"",
                "\"0xB3\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xB3\",\"\",\"\",\"\",\"\",\"QNX Software Systems\",\"QNX 6.x\",\"QNX Neutrino power-safe file system\"",
                "\"0xB4\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xB6\",\"\",\"\",\"\",\"\",\"Hewlett Packard\",\"\",\"HP Volume Expansion (SpeedStor)\"",
                "\"0xB6\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT16B mirrored master volume\"",
                "\"0xB7\",\"\",\"\",\"\",\"Filesystem\",\"\",\"BSDI (before 3.0)\",\"BSDI native filesystem / swap\"",
                "\"0xB7\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant HPFS/NTFS mirrored master volume\"",
                "\"0xB8\",\"\",\"\",\"\",\"Filesystem\",\"\",\"BSDI (before 3.0)\",\"BSDI swap / native filesystem\"",
                "\"0xBB\",\"\",\"\",\"\",\"Hidden, (Filesystem)\",\"PhysTechSoft, Acronis, SWsoft\",\"BootWizard, OS Selector\",\"PTS BootWizard 4 / OS Selector 5 for hidden partitions other than 01h, 04h, 06h, 07h, 0Bh, 0Ch, 0Eh and unformatted partitions\"",
                "\"0xBB\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32 mirrored master volume\"",
                "\"0xBC\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32X mirrored master volume\"",
                "\"0xBC\",\"MBR\",\"LBA\",\"\",\"\",\"Acronis\",\"\",\"Backup / Acronis Secure Zone ('ACRONIS SZ')\"",
                "\"0xBC\",\"MBR, EBR\",\"\",\"\",\"\",\"Paragon Software Group\",\"Backup Capsule\",\"Backup Capsule\"",
                "\"0xBD\",\"\",\"\",\"\",\"\",\"\",\"BonnyDOS/286\"",
                "\"0xBE\",\"\",\"\",\"Yes\",\"\",\"Sun Microsystems\",\"Solaris 8\",\"Solaris 8 boot\"",
                "\"0xBF\",\"\",\"\",\"x86\",\"Container\",\"Sun Microsystems\",\"Solaris\",\"Solaris x86 (for Sun disklabels, since 2005)\"",
                "\"0xC0\",\"MBR\",\"CHS, LBA\",\"x86\",\"Secured, (Container)\",\"Novell, IMS\",\"DR-DOS, Multiuser DOS, REAL/32\",\"Secured FAT partition (smaller than 32 MB)\"",
                "\"0xC0\",\"\",\"\",\"\",\"\",\"Novell\",\"\",\"NTFT\"",
                "\"0xC1\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured FAT12 (corresponds with 01h)\"",
                "\"0xC2\",\"\",\"\",\"Yes\",\"Hidden, Filesystem\",\"BlueSky Innovations\",\"Power Boot\",\"Hidden Linux native filesystem\"",
                "\"0xC3\",\"\",\"\",\"No\",\"Hidden\",\"BlueSky Innovations\",\"Power Boot\",\"Hidden Linux swap\"",
                "\"0xC4\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured FAT16 (corresponds with 04h)\"",
                "\"0xC5\",\"MBR, EBR\",\"CHS, LBA\",\"No, AAP\",\"Secured, Hidden, Container\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured extended partition with CHS addressing (corresponds with 05h)\"",
                "\"0xC6\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Digital Research\",\"DR DOS 6.0+\",\"Secured FAT16B (corresponds with 06h)\"",
                "\"0xC6\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT16B mirrored slave volume\"",
                "\"0xC7\",\"MBR\",\"\",\"Yes\",\"\",\"\",\"Syrinx\",\"Syrinx boot\"",
                "\"0xC7\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant HPFS/NTFS mirrored slave volume\"",
                "\"0xC8\",\"\",\"\",\"\",\"\",\"\",\"DR-DOS\",\"Reserved for DR-DOS\"",
                "\"0xC9\",\"\",\"\",\"\",\"\",\"\",\"DR-DOS\",\"Reserved for DR-DOS\"",
                "\"0xCA\",\"\",\"\",\"\",\"\",\"\",\"DR-DOS\",\"Reserved for DR-DOS\"",
                "\"0xCB\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Caldera\",\"DR-DOS 7.\",\"Secured FAT32 (corresponds with 0Bh)\"",
                "\"0xCB\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32 mirrored slave volume\"",
                "\"0xCC\",\"MBR, EBR\",\"LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Caldera\",\"DR-DOS 7.\",\"Secured FAT32X (corresponds with 0Ch)\"",
                "\"0xCC\",\"EBR\",\"\",\"\",\"\",\"Microsoft\",\"Windows NT 4 Server\",\"Corrupted fault-tolerant FAT32X mirrored slave volume\"",
                "\"0xCD\",\"\",\"\",\"No\",\"\",\"Convergent Technologies, Unisys\",\"CTOS\",\"Memory dump\"",
                "\"0xCE\",\"MBR, EBR\",\"LBA\",\"x86\",\"Secured, Hidden, Filesystem\",\"Caldera\",\"DR-DOS 7.\",\"Secured FAT16X (corresponds with 0Eh)\"",
                "\"0xCF\",\"MBR, EBR\",\"LBA\",\"No, AAP\",\"Secured, Hidden, Container\",\"Caldera\",\"DR-DOS 7.\",\"Secured extended partition with LBA (corresponds with 0Fh)\"",
                "\"0xD0\",\"MBR\",\"CHS, LBA\",\"386\",\"Secured, (Container)\",\"Novell, IMS\",\"Multiuser DOS, REAL/32\",\"Secured FAT partition (larger than 32 MB)\"",
                "\"0xD1\",\"MBR, EBR\",\"CHS\",\"386\",\"Secured, Hidden, Filesystem\",\"Novell\",\"Multiuser DOS\",\"Secured FAT12 (corresponds with 01h)\"",
                "\"0xD4\",\"MBR, EBR\",\"CHS\",\"386\",\"Secured, Hidden, Filesystem\",\"Novell\",\"Multiuser DOS\",\"Secured FAT16 (corresponds with 04h)\"",
                "\"0xD5\",\"MBR, EBR\",\"CHS\",\"No\",\"Secured, Hidden, Container\",\"Novell\",\"Multiuser DOS\",\"Secured extended partition with CHS addressing (corresponds with 05h)\"",
                "\"0xD6\",\"MBR, EBR\",\"CHS\",\"386\",\"Secured, Hidden, Filesystem\",\"Novell\",\"Multiuser DOS\",\"Secured FAT16B (corresponds with 06h)\"",
                "\"0xD8\",\"MBR\",\"CHS\",\"\",\"Filesystem\",\"Digital Research\",\"\",\"CP/M-86\"",
                "\"0xDA\",\"\",\"\",\"No\",\"\",\"John Hardin\",\"\",\"Non-filesystem data\"",
                "\"0xDA\",\"\",\"\",\"\",\"\",\"DataPower\",\"Powercopy Backup\",\"Shielded disk\"",
                "\"0xDB\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Digital Research\",\"CP/M-86, Concurrent CP/M-86, Concurrent DOS\",\"CP/M-86, Concurrent CP/M-86, Concurrent DOS\"",
                "\"0xDB\",\"\",\"\",\"\",\"\",\"Convergent Technologies, Unisys\",\"CTOS\",\"\"",
                "\"0xDB\",\"\",\"\",\"x86\",\"\",\"KDG Telemetry\",\"D800\",\"boot image for x86 supervisor CPU (SCPU) module\"",
                "\"0xDB\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Dell\",\"DRMK\",\"FAT32 system restore partition (DSR)\"",
                "\"0xDD\",\"\",\"\",\"No\",\"\",\"Convergent Technologies, Unisys\",\"CTOS\",\"Hidden memory dump\"",
                "\"0xDE\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"Dell\",\"\",\"FAT16 utility/diagnostic partition\"",
                "\"0xDF\",\"\",\"\",\"\",\"\",\"Data General\",\"DG/UX\",\"DG/UX virtual disk manager\"",
                "\"0xDF\",\"MBR\",\"\",\"\",\"Blocker\",\"TeraByte Unlimited\",\"BootIt\",\"EMBRM\"",
                "\"0xDF\",\"\",\"\",\"\",\"\",\"\",\"\",\"Aviion\"",
                "\"0xE0\",\"\",\"\",\"\",\"Filesystem\",\"STMicroelectronics\",\"\",\"ST AVFS\"",
                "\"0xE1\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"Extended FAT12 (> 1023 cylinder)\"",
                "\"0xE2\",\"\",\"\",\"\",\"Filesystem\",\"\",\"\",\"DOS read-only (XFDISK)\"",
                "\"0xE3\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"DOS read-only\"",
                "\"0xE4\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"Extended FAT16 (< 1024 cylinder)\"",
                "\"0xE5\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Tandy\",\"Tandy MS-DOS\",\"Logical sectored FAT12 or FAT16\"",
                "\"0xE6\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"0xE8\",\"\",\"\",\"\",\"\",\"Linux\",\"LUKS\",\"Linux Unified Key Setup\"",
                "\"0xEB\",\"\",\"\",\"386\",\"Filesystem\",\"Be Inc.\",\"BeOS, Haiku\",\"BFS\"",
                "\"0xEC\",\"\",\"\",\"\",\"Filesystem\",\"Robert Szeleney\",\"SkyOS\",\"SkyFS\"",
                "\"0xED\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"\",\"Matthias Paul\",\"Sprytix\",\"EDC loader\"",
                "\"0xED\",\"VirtualMBR\",\"CHS, LBA\",\"x86\",\"\",\"Robert Elliott, Hewlett Packard\",\"EDD 4\",\"GPT hybrid MBR\"",
                "\"0xEE\",\"MBR\",\"\",\"x86\",\"Blocker, Policy, Container\",\"Microsoft\",\"EFI\",\"GPT protective MBR\"",
                "\"0xEF\",\"MBR\",\"\",\"\",\"\",\"Intel\",\"EFI\",\"EFI system partition can be a FAT12, FAT16, FAT32 (or other) file system\"",
                "\"0xF0\",\"\",\"CHS\",\"\",\"\",\"\",\"Linux\",\"PA-RISC Linux boot loader. It must reside in first physical 2 GB.\"",
                "\"0xF0\",\"\",\"\",\"\",\"\",\"\",\"OS/32\",\"floppy\"",
                "\"0xF1\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"0xF2\",\"MBR\",\"CHS\",\"x86\",\"Filesystem\",\"Sperry IT, Unisys, Digital Research\",\"Sperry IT MS-DOS 3.x, Unisys MS-DOS 3.3, Digital Research DOS Plus 2.1\",\"Logical sectored FAT12 or FAT16 secondary partition\"",
                "\"0xF3\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"0xF4\",\"\",\"\",\"\",\"Filesystem\",\"Storage Dimensions\",\"SpeedStor\",\"'large' DOS partition\"",
                "\"0xF4\",\"\",\"\",\"\",\"Filesystem\",\"\",\"Prologue\",\"single volume partition for NGF or TwinFS\"",
                "\"0xF5\",\"\",\"\",\"\",\"Container\",\"\",\"Prologue\",\"MD0-MD9 multi volume partition for NGF or TwinFS\"",
                "\"0xF6\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\"",
                "\"0xF7\",\"\",\"\",\"\",\"Filesystem\",\"Natalia Portillo\",\"O.S.G.\",\"EFAT\"",
                "\"0xF7\",\"\",\"\",\"\",\"Filesystem\",\"DDRdrive\",\"X1\",\"Solid State file system\"",
                "\"0xF9\",\"\",\"\",\"\",\"\",\"ALC Press\",\"Linux\",\"pCache ext2/ext3 persistent cache\"",
                "\"0xFA\",\"\",\"\",\"\",\"\",\"MandrakeSoft\",\"Bochs\",\"x86 emulator\"",
                "\"0xFB\",\"\",\"\",\"\",\"Filesystem\",\"VMware\",\"VMware\",\"VMware VMFS filesystem partition\"",
                "\"0xFC\",\"\",\"\",\"No\",\"\",\"VMware\",\"VMware\",\"VMware swap / VMKCORE kernel dump partition\"",
                "\"0xFD\",\"\",\"\",\"\",\"\",\"GNU/Linux\",\"Linux\",\"Linux RAID superblock with auto-detect\"",
                "\"0xFD\",\"MBR, EBR\",\"CHS, LBA\",\"x86\",\"\",\"FreeDOS\",\"FreeDOS\",\"Reserved for FreeDOS\"",
                "\"0xFE\",\"\",\"\",\"\",\"\",\"Storage Dimensions\",\"SpeedStor\",\"partition > 1024 cylinder\"",
                "\"0xFE\",\"\",\"\",\"\",\"\",\"\",\"Intel\",\"LANstep\"",
                "\"0xFE\",\"\",\"\",\"\",\"Hidden, Service\",\"IBM\",\"\",\"PS/2 IML partition\"",
                "\"0xFE\",\"MBR\",\"CHS, LBA\",\"x86\",\"Hidden, Service, Filesystem\",\"IBM\",\"\",\"PS/2 recovery partition (FAT12 reference disk floppy image), (corresponds with 01h if activated, all other partitions +10h then)\"",
                "\"0xFE\",\"\",\"\",\"\",\"Hidden\",\"Microsoft\",\"Windows NT\",\"Disk Administration hidden partition\"",
                "\"0xFE\",\"\",\"\",\"\",\"\",\"\",\"Linux\",\"old Linux LVM\"",
                "\"0xFF\",\"MBR\",\"CHS\",\"No\",\"\",\"Microsoft\",\"XENIX\",\"XENIX bad block table\""
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
            var vs = TextTools.Split(s, ",", true, true, unquote: true);

            int i;
            int c = vs.Length;

            for (i = 0; i < c; i++)
            {
                vs[i] = vs[i].Trim();
            }

            partitionId = (byte)TextTools.FVal(vs[0]);

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
