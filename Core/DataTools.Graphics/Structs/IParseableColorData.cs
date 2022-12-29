using System;
using System.Linq;
using System.Text;

namespace DataTools.Graphics.Structs
{
    public interface IParseableColorData
    {
#if NET7_0_OR_GREATER

        /// <summary>
        /// Invoke the <see cref="Parse(string)"/> method for the specified object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="FormatException"></exception>
        public static object Parse(Type t, string value)
        {
            var mtd = t.GetMethod(nameof(Parse));
            if (mtd == null) throw new NotSupportedException();

            var obj = mtd.Invoke(null, new object[] { value });
            if (obj is object && obj.GetType().GetInterface(nameof(IParseableColorData)) is object)
            {
                return obj;
            }

            throw new FormatException();
        }

        /// <summary>
        /// Invoke the <see cref="Parse(string)"/> method for the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="FormatException"></exception>
        public static T Parse<T>(string value) where T : IParseableColorData<T>
        {
            return (T)Parse(typeof(T), value);
        }

        /// <summary>
        /// Invoke the <see cref="FromColor(UniColor)"/> method for the specified object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="FormatException"></exception>
        public static object FromColor(Type t, UniColor value)
        {
            var mtd = t.GetMethod(nameof(FromColor));
            if (mtd == null) throw new NotSupportedException();

            var obj = mtd.Invoke(null, new object[] { value });
            if (obj is object && obj.GetType().GetInterface(nameof(IParseableColorData)) is object)
            {
                return obj;
            }

            throw new FormatException();
        }

        /// <summary>
        /// Invoke the <see cref="FromColor(UniColor)"/> method for the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="FormatException"></exception>
        public static T FromColor<T>(UniColor value) where T : IParseableColorData<T>
        {
            return (T)FromColor(typeof(T), value);
        }
#endif
    }

    /// <summary>
    /// Describes an object that represents a full color data value that can be parsed.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public interface IParseableColorData<TSelf> : IParseableColorData, IFormattable, IEquatable<TSelf> where TSelf : IParseableColorData<TSelf>
    {
#if NET7_0_OR_GREATER

        /// <summary>
        /// Parse the string expression into a new instance of this type.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <returns></returns>
        static abstract TSelf Parse(string value);

        /// <summary>
        /// Create a new instance of this type from a <see cref="UniColor"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static abstract TSelf FromColor(UniColor value);
#endif

        /// <summary>
        /// Create a <see cref="UniColor"/> from this instance.
        /// </summary>
        /// <returns></returns>
        UniColor CreateColor();
    }
}