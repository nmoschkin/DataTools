namespace DataTools.Win32.Disk.Partition.Interfaces
{
    /// <summary>
    /// Represents a partition type with additional information about the partition type
    /// </summary>
    public interface IPartitionType
    {
        /// <summary>
        /// The name of the partition type
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the paritition type
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The partition code, as printed
        /// </summary>
        string PartitionCodeString { get; }

    }

    /// <summary>
    /// Represents a partition type with additional information about the partition type
    /// </summary>
    /// <typeparam name="T">The type of object for the partition type code. Must be a structure or a number.</typeparam>
    public interface IPartitionType<T> : IPartitionType where T : struct
    {
        /// <summary>
        /// The partition code
        /// </summary>
        T PartitionCode { get; }
    }
}
