using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Disk.Partition.Structs
{
    // 0x0 	__le32 	s_inodes_count 	Total inode count.
    // 0x4 	__le32 	s_blocks_count_lo 	Total block count.
    // 0x8 	__le32 	s_r_blocks_count_lo 	This number of blocks can only be allocated by the super-user.
    // 0xC 	__le32 	s_free_blocks_count_lo 	Free block count.
    // 0x10 	__le32 	s_free_inodes_count 	Free inode count.
    // 0x14 	__le32 	s_first_data_block 	First data block. This must be at least 1 for 1k-block filesystems and is typically 0 for all other block sizes.
    // 0x18 	__le32 	s_log_block_size 	Block size is 2 ^ (10 + s_log_block_size).
    // 0x1C 	__le32 	s_log_cluster_size 	Cluster size is (2 ^ s_log_cluster_size) blocks if bigalloc is enabled. Otherwise s_log_cluster_size must equal s_log_block_size.
    // 0x20 	__le32 	s_blocks_per_group 	Blocks per group.
    // 0x24 	__le32 	s_clusters_per_group 	Clusters per group, if bigalloc is enabled. Otherwise s_clusters_per_group must equal s_blocks_per_group.
    // 0x28 	__le32 	s_inodes_per_group 	Inodes per group.
    // 0x2C 	__le32 	s_mtime 	Mount time, in seconds since the epoch.
    // 0x30 	__le32 	s_wtime 	Write time, in seconds since the epoch.
    // 0x34 	__le16 	s_mnt_count 	Number of mounts since the last fsck.
    // 0x36 	__le16 	s_max_mnt_count 	Number of mounts beyond which a fsck is needed.
    // 0x38 	__le16 	s_magic 	Magic signature, 0xEF53
    // 0x3A 	__le16 	s_state 	File system state. Valid values are:
    // 0x0001 	Cleanly umounted
    // 0x0002 	Errors detected
    // 0x0004 	Orphans being recovered
    // 0x3C 	__le16 	s_errors 	Behaviour when detecting errors. One of:
    // 1 	Continue
    // 2 	Remount read-only
    // 3 	Panic
    // 0x3E 	__le16 	s_minor_rev_level 	Minor revision level.
    // 0x40 	__le32 	s_lastcheck 	Time of last check, in seconds since the epoch.
    // 0x44 	__le32 	s_checkinterval 	Maximum time between checks, in seconds.
    // 0x48 	__le32 	s_creator_os 	OS. One of:
    // 0 	Linux
    // 1 	Hurd
    // 2 	Masix
    // 3 	FreeBSD
    // 4 	Lites
    // 0x4C 	__le32 	s_rev_level 	Revision level. One of:
    // 0 	Original format
    // 1 	v2 format w/ dynamic inode sizes
    // 0x50 	__le16 	s_def_resuid 	Default uid for reserved blocks.
    // 0x52 	__le16 	s_def_resgid 	Default gid for reserved blocks. 

    /// <summary>
    /// Drive state flags for super block
    /// </summary>
    [Flags]
    public enum DriveState : ushort
    {
        /// <summary>
        /// Cleanly umounted
        /// </summary>
        CleanlyUnmounted = 1,

        /// <summary>
        /// Errors detected
        /// </summary>
        ErrorsDetected = 2,

        /// <summary>
        /// Orphans being recovered
        /// </summary>
        OrphansBeingRecovered = 4
    }


    /// <summary>
    /// Error codes for super block
    /// </summary>
    public enum DriveErrors : ushort
    {

        /// <summary>
        /// Continue
        /// </summary>
        Continue = 1,

        /// <summary>
        /// Remount read-only
        /// </summary>
        RemoutReadOnly = 2,

        /// <summary>
        /// Panic
        /// </summary>
        Panic = 3
    }

    /// <summary>
    /// The OS that created the file system
    /// </summary>

    public enum CreatorOS : uint
    {
        /// <summary>
        /// Linux
        /// </summary>
        Linux = 0,

        /// <summary>
        /// Hurd
        /// </summary>
        Hurd = 1,
        
        /// <summary>
        /// Masix
        /// </summary>
        Masix = 2,

        /// <summary>
        /// FreeBSD
        /// </summary>
        FreeBSD = 3,

        /// <summary>
        /// Lites
        /// </summary>
        Lites = 4
    }

    /// <summary>
    /// Linux ext2/3/4 superblock structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SuperBlock
    {
        
        /// <summary>
        /// The File System Magic Signature
        /// </summary>
        public const ushort MagicSignature = 0xEF53;

        /// <summary>
        /// Total inode count.
        /// </summary>
        public uint s_inodes_count;

        /// <summary>
        /// Total block count.
        /// </summary>
        public uint s_blocks_count_lo;

        /// <summary>
        /// This number of blocks can only be allocated by the super-user.
        /// </summary>
        public uint s_r_blocks_count_lo;

        /// <summary>
        /// Free block count.
        /// </summary>
        public uint s_free_blocks_count_lo;

        /// <summary>
        /// Free inode count.
        /// </summary>
        public uint s_free_inodes_count;

        /// <summary>
        /// First data block. This must be at least 1 for 1k-block filesystems and is typically 0 for all other block sizes.
        /// </summary>
        public uint s_first_data_block;

        /// <summary>
        /// Block size is 2 ^ (10 + s_log_block_size).
        /// </summary>
        public uint s_log_block_size;

        /// <summary>
        /// Cluster size is (2 ^ s_log_cluster_size) blocks if bigalloc is enabled. Otherwise s_log_cluster_size must equal s_log_block_size.
        /// </summary>
        public uint s_log_cluster_size;

        /// <summary>
        /// Blocks per group.
        /// </summary>
        public uint s_blocks_per_group;

        /// <summary>
        /// Clusters per group, if bigalloc is enabled. Otherwise s_clusters_per_group must equal s_blocks_per_group.
        /// </summary>
        public uint s_clusters_per_group;

        /// <summary>
        /// Inodes per group.
        /// </summary>
        public uint s_inodes_per_group;

        /// <summary>
        /// Mount time, in seconds since the epoch.
        /// </summary>
        public uint s_mtime;

        /// <summary>
        /// Write time, in seconds since the epoch.
        /// </summary>
        public uint s_wtime;

        /// <summary>
        /// Number of mounts since the last fsck.
        /// </summary>
        public ushort s_mnt_count;

        /// <summary>
        /// Number of mounts beyond which a fsck is needed.
        /// </summary>
        public ushort s_max_mnt_count;

        /// <summary>
        /// Magic signature, 0xEF53
        /// </summary>
        public ushort s_magic;

        /// <summary>
        /// File system state. Valid values are:
        /// </summary>
        public DriveState s_state;
        
        /// <summary>
        /// Behaviour when detecting errors. One of:
        /// </summary>
        public DriveErrors s_errors;

        /// <summary>
        /// Minor revision level.
        /// </summary>
        public ushort s_minor_rev_level;

        /// <summary>
        /// Time of last check, in seconds since the epoch.
        /// </summary>
        public uint s_lastcheck;

        /// <summary>
        /// Maximum time between checks, in seconds.
        /// </summary>
        public uint s_checkinterval;

        /// <summary>
        /// Creator OS
        /// </summary>
        public CreatorOS s_creator_os;

        /// <summary>
        /// Revision level. One of:
        /// </summary>
        public uint s_rev_level;

        // 0 	Original format
        // 1 	v2 format w/ dynamic inode sizes
        /// <summary>
        /// Default uid for reserved blocks.
        /// </summary>
        public ushort s_def_resuid;

        /// <summary>
        /// Default gid for reserved blocks. 
        /// </summary>
        public ushort s_def_resgid;

    }
}
