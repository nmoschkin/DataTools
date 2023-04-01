using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Disk.Partition
{
    /// <summary>
    /// Disk Geometry Media Types
    /// </summary>
    /// <remarks></remarks>
    public enum MediaType
    {
        /// <summary>
        /// Format is unknown
        /// </summary>
        [Description("Format is unknown")]
        Unknown,

        /// <summary>
        /// A 5.25" floppy, with 1.2MB and 512 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 1.2MB and 512 bytes/sector.")]
        F5_1Pt2_512,

        /// <summary>
        /// A 3.5" floppy, with 1.44MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 1.44MB and 512 bytes/sector.")]
        F3_1Pt44_512,

        /// <summary>
        /// A 3.5" floppy, with 2.88MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 2.88MB and 512 bytes/sector.")]
        F3_2Pt88_512,

        /// <summary>
        /// A 3.5" floppy, with 20.8MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 20.8MB and 512 bytes/sector.")]
        F3_20Pt8_512,

        /// <summary>
        /// A 3.5" floppy, with 720KB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 720KB and 512 bytes/sector.")]
        F3_720_512,

        /// <summary>
        /// A 5.25" floppy, with 360KB and 512 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 360KB and 512 bytes/sector.")]
        F5_360_512,

        /// <summary>
        /// A 5.25" floppy, with 320KB and 512 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 320KB and 512 bytes/sector.")]
        F5_320_512,

        /// <summary>
        /// A 5.25" floppy, with 320KB and 1024 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 320KB and 1024 bytes/sector.")]
        F5_320_1024,

        /// <summary>
        /// A 5.25" floppy, with 180KB and 512 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 180KB and 512 bytes/sector.")]
        F5_180_512,

        /// <summary>
        /// A 5.25" floppy, with 160KB and 512 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 160KB and 512 bytes/sector.")]
        F5_160_512,

        /// <summary>
        /// Removable media other than floppy.
        /// </summary>
        [Description("Removable media other than floppy.")]
        RemovableMedia,

        /// <summary>
        /// Fixed hard disk media.
        /// </summary>
        [Description("Fixed hard disk media.")]
        FixedMedia,

        /// <summary>
        /// A 3.5" floppy, with 120MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 120MB and 512 bytes/sector.")]
        F3_120M_512,

        /// <summary>
        /// A 3.5" floppy, with 640KB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 640KB and 512 bytes/sector.")]
        F3_640_512,

        /// <summary>
        /// A 5.25" floppy, with 640KB and 512 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 640KB and 512 bytes/sector.")]
        F5_640_512,

        /// <summary>
        /// A 5.25" floppy, with 720KB and 512 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 720KB and 512 bytes/sector.")]
        F5_720_512,

        /// <summary>
        /// A 3.5" floppy, with 1.2MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 1.2MB and 512 bytes/sector.")]
        F3_1Pt2_512,

        /// <summary>
        /// A 3.5" floppy, with 1.23MB and 1024 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 1.23MB and 1024 bytes/sector.")]
        F3_1Pt23_1024,

        /// <summary>
        /// A 5.25" floppy, with 1.23MB and 1024 bytes/sector.
        /// </summary>
        [Description("A 5.25\" floppy, with 1.23MB and 1024 bytes/sector.")]
        F5_1Pt23_1024,

        /// <summary>
        /// A 3.5" floppy, with 128MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 128MB and 512 bytes/sector.")]
        F3_128Mb_512,

        /// <summary>
        /// A 3.5" floppy, with 230MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 230MB and 512 bytes/sector.")]
        F3_230Mb_512,

        /// <summary>
        /// An 8" floppy, with 256KB and 128 bytes/sector.
        /// </summary>
        [Description("An 8\" floppy, with 256KB and 128 bytes / sector.")]
        F8_256_128,
        
        /// <summary>
        /// A 3.5" floppy, with 200MB and 512 bytes/sector. (HiFD).
        /// </summary>
        [Description("A 3.5\" floppy, with 200MB and 512 bytes/sector. (HiFD).")]
        F3_200Mb_512,
        
        /// <summary>
        /// A 3.5" floppy, with 240MB and 512 bytes/sector. (HiFD).
        /// </summary>
        [Description("A 3.5\" floppy, with 240MB and 512 bytes/sector. (HiFD).")]
        F3_240M_512,
        
        /// <summary>
        /// A 3.5" floppy, with 32MB and 512 bytes/sector.
        /// </summary>
        [Description("A 3.5\" floppy, with 32MB and 512 bytes/sector.")]
        F3_32M_512,

    }

}

