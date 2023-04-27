using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Disk.Partition
{
    /// <summary>
    /// Prints a friendly hexadecimal Mbr partition byte code.
    /// </summary>
    /// <remarks></remarks>
    public struct FriendlyPartitionId 
    {
        /// <summary>
        /// Value of the partition ID
        /// </summary>
        public byte Value;

        /// <summary>
        /// Create a friendly partition Id
        /// </summary>
        /// <param name="v"></param>
        public FriendlyPartitionId(byte v)
        {
            Value = v;
        }

        /// <summary>
        /// Gets the PartitionId in the format xxh, where xx is a hexidecimal number between 0 and ff.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString("X2") + "h";
        }

        /// <summary>
        /// Cast from byte
        /// </summary>
        /// <param name="operand"></param>
        public static implicit operator FriendlyPartitionId(byte operand)
        {
            return new FriendlyPartitionId(operand);
        }

        /// <summary>
        /// Cast to byte
        /// </summary>
        /// <param name="operand"></param>
        public static implicit operator byte(FriendlyPartitionId operand)
        {
            return operand.Value;
        }

        /// <summary>
        /// Cast to string
        /// </summary>
        /// <param name="operand"></param>
        public static explicit operator string(FriendlyPartitionId operand)
        {
            return operand.ToString();
        }
    }

}
