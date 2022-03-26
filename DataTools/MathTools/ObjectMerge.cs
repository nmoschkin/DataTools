using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DataTools.MathTools
{
    /// <summary>
    /// Object Merging, Diffing, and Comparing Tools
    /// </summary>
    public static class ObjectMerge
    {

        /// <summary>
        /// Merge an ancestor object into a derived object.
        /// </summary>
        /// <typeparam name="T">The type of the base object</typeparam>
        /// <typeparam name="U">The type of the derived object</typeparam>
        /// <param name="baseObj">The base object</param>
        /// <param name="derivedObj">The derived object</param>
        /// <param name="requiredAttributes">Required property attributes (optional)</param>
        /// <remarks>
        /// This method will grab properties from the base object if they are not null and
        /// invoke the setter for the same property on the derived object.
        /// <br /><br />
        /// This method is useful for preserving the unique values of a derived object while updating the core data with refreshed values.
        /// </remarks>
        public static void MergeObjects<T, U>(T baseObj, U derivedObj, Type[] requiredAttributes = null) 
            where T: class 
            where U: class, T
        {

            var pinfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            foreach (var pi in pinfo)
            {
                if (!pi.CanWrite) continue;

                if (requiredAttributes != null && requiredAttributes.Length > 0)
                {
                    foreach (var req in requiredAttributes)
                    {
                        var attrTest = pi.GetCustomAttribute(req);
                        if (attrTest == null) continue;
                    }
                }

                object obj = pi.GetValue(baseObj);

                if (obj != null)
                {
                    pi.SetValue(derivedObj, obj);
                }
            }

        }

    }
}
